using WorkflowDemo.Engine;
using WorkflowDemo.Model;

namespace WorkflowDemo.Executors;

public sealed class MockSnmpNodeExecutor : INodeExecutor
{
    public string Type => "MockSnmp";

    public Task<NodeResult> ExecuteAsync(
        NodeDefinition node,
        WorkflowContext context,
        CancellationToken cancellationToken)
    {
        var source = node.Config["source"]?.ToString()
                     ?? throw new InvalidOperationException("MockSnmp node requires config.source");

        var output = node.Config["output"]?.ToString()
                     ?? throw new InvalidOperationException("MockSnmp node requires config.output");

        object value = source switch
        {
            "vendor" => context.Get<string>("input.vendor") ?? "Unknown",
            "firmware" => context.Get<string>("input.firmware") ?? "0.0",
            "interfaces" => new[] { "GE0/0/1", "GE0/0/2", "Eth-Trunk1" },
            "vlans" => new[] { "10:users", "20:voice", "99:management" },
            _ => throw new InvalidOperationException($"Unknown mock source '{source}'.")
        };

        Console.WriteLine($"    MOCK SNMP: reading '{source}'");

        return Task.FromResult(NodeResult.Ok(new()
        {
            [output] = value
        }));
    }
}
