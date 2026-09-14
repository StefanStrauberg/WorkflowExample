using WorkflowDemo.Model;

namespace WorkflowDemo.Engine;

public sealed class WorkflowEngine(NodeExecutorResolver resolver)
{
    public async Task ExecuteAsync(WorkflowDefinition workflow,
                                   WorkflowContext context,
                                   CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"=== WORKFLOW: {workflow.Name} v{workflow.Version} ===\n");

        var currentNodeId = workflow.StartNodeId;
        var step = 1;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var node = workflow.Nodes.Single(x => x.Id == currentNodeId);
            context.CurrentNodeId = node.Id;

            Console.WriteLine($"[{step}] NODE: {node.Name}");
            Console.WriteLine($"    Type: {node.Type}");

            var executor = resolver.Resolve(node.Type);
            var result = await executor.ExecuteAsync(node, context, cancellationToken);

            if (!result.Success)
                throw new InvalidOperationException(
                    $"Node '{node.Name}' failed: {result.Error}");

            foreach (var (key, value) in result.Outputs)
            {
                context.Set(key, value);
                Console.WriteLine($"    OUTPUT: {key} = {Format(value)}");
            }

            if (result.Decision is not null)
                Console.WriteLine($"    DECISION: {result.Decision}");

            if (node.Type.Equals("End", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("\n=== WORKFLOW FINISHED ===");
                return;
            }

            var next = ResolveNextEdge(workflow, node, result.Decision);

            Console.WriteLine(
                $"    EDGE: {DescribeCondition(next.Condition)} -> " +
                $"{workflow.Nodes.Single(x => x.Id == next.ToNodeId).Name}\n");

            currentNodeId = next.ToNodeId;
            step++;
        }
    }

    private static EdgeDefinition ResolveNextEdge(WorkflowDefinition workflow,
                                                  NodeDefinition node,
                                                  string? decision)
    {
        var edges = workflow.Edges
                            .Where(x => x.FromNodeId == node.Id)
                            .OrderBy(x => x.Priority)
                            .ToList();

        if (edges.Count == 0)
            throw new InvalidOperationException(
                $"Node '{node.Name}' has no outgoing edges.");

        Console.WriteLine("    Candidate edges:");
        foreach (var edge in edges)
            Console.WriteLine(
                $"      priority={edge.Priority}, condition={DescribeCondition(edge.Condition)}");

        if (decision is not null)
        {
            var exact = edges.FirstOrDefault(x =>
                string.Equals(x.Condition, decision, StringComparison.OrdinalIgnoreCase));

            if (exact is not null)
            {
                Console.WriteLine(
                    $"    WHY: exact edge condition '{exact.Condition}' matches decision '{decision}'.");
                return exact;
            }
        }

        var fallback = edges.FirstOrDefault(x =>
                           string.Equals(x.Condition, "DEFAULT", StringComparison.OrdinalIgnoreCase))
                       ?? edges.FirstOrDefault(x => x.Condition is null);

        if (fallback is not null)
        {
            Console.WriteLine(
                $"    WHY: no exact match; fallback edge {DescribeCondition(fallback.Condition)} selected.");
            return fallback;
        }

        throw new InvalidOperationException(
            $"Cannot resolve next edge for node '{node.Name}', decision='{decision ?? "<null>"}'.");
    }

    private static string DescribeCondition(string? condition) =>
        condition is null ? "<unconditional>" : $"'{condition}'";

    private static string Format(object? value) => value switch
    {
        null => "<null>",
        IEnumerable<string> strings => "[" + string.Join(", ", strings) + "]",
        _ => value.ToString() ?? "<null>"
    };
}
