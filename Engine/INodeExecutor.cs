using WorkflowDemo.Model;

namespace WorkflowDemo.Engine;

public interface INodeExecutor
{
    string Type { get; }

    Task<NodeResult> ExecuteAsync(
        NodeDefinition node,
        WorkflowContext context,
        CancellationToken cancellationToken);
}
