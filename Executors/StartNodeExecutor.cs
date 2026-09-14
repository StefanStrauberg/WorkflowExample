using WorkflowDemo.Engine;
using WorkflowDemo.Model;

namespace WorkflowDemo.Executors;

public sealed class StartNodeExecutor : INodeExecutor
{
    public string Type => "Start";

    public Task<NodeResult> ExecuteAsync(
        NodeDefinition node,
        WorkflowContext context,
        CancellationToken cancellationToken)
        => Task.FromResult(NodeResult.Ok());
}
