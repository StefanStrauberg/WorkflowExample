using WorkflowDemo.Engine;
using WorkflowDemo.Model;

namespace WorkflowDemo.Executors;

public sealed class DecisionNodeExecutor : INodeExecutor
{
    public string Type => "Decision";

    public Task<NodeResult> ExecuteAsync(
        NodeDefinition node,
        WorkflowContext context,
        CancellationToken cancellationToken)
    {
        var variable = node.Config["variable"]?.ToString()
                       ?? throw new InvalidOperationException("Decision node requires config.variable");

        var mode = node.Config.GetValueOrDefault("mode")?.ToString() ?? "value";
        var value = context.Get(variable)?.ToString() ?? string.Empty;

        var decision = mode switch
        {
            "value" => value,
            "firmware-major" => GetFirmwareDecision(value),
            _ => throw new InvalidOperationException($"Unknown decision mode '{mode}'.")
        };

        Console.WriteLine($"    READ CONTEXT: {variable} = {value}");
        return Task.FromResult(NodeResult.WithDecision(decision));
    }

    private static string GetFirmwareDecision(string firmware)
    {
        var majorText = firmware.Split('.', '-', '_')[0];

        if (!int.TryParse(majorText, out var major))
            return "LEGACY";

        return major >= 10 ? "MODERN" : "LEGACY";
    }
}
