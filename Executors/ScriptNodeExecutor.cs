using WorkflowDemo.Engine;
using WorkflowDemo.Model;
using WorkflowDemo.Scripts;

namespace WorkflowDemo.Executors;

public sealed class ScriptNodeExecutor(IScriptRepository scripts) : INodeExecutor
{
    public string Type => "Script";

    public Task<NodeResult> ExecuteAsync(
        NodeDefinition node,
        WorkflowContext context,
        CancellationToken cancellationToken)
    {
        var scriptKey = node.Config["scriptKey"]?.ToString()
                        ?? throw new InvalidOperationException("Script node requires config.scriptKey");

        Console.WriteLine($"    SCRIPT: execute '{scriptKey}'");

        var script = scripts.Get(scriptKey);
        var outputs = script(context);

        return Task.FromResult(NodeResult.Ok(outputs));
    }
}
