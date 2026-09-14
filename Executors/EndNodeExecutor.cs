using WorkflowDemo.Engine;
using WorkflowDemo.Model;

namespace WorkflowDemo.Executors;

public sealed class EndNodeExecutor : INodeExecutor
{
    public string Type => "End";

    public Task<NodeResult> ExecuteAsync(
        NodeDefinition node,
        WorkflowContext context,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("    Final context:");
        foreach (var (key, value) in context.Variables.OrderBy(x => x.Key))
            Console.WriteLine($"      {key} = {value}");

        return Task.FromResult(NodeResult.Ok());
    }
}
