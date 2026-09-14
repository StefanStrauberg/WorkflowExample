namespace WorkflowDemo.Engine;

public sealed class NodeExecutorResolver(IEnumerable<INodeExecutor> executors)
{
    private readonly Dictionary<string, INodeExecutor> _executors = executors.ToDictionary(x => x.Type,
                                                                                           StringComparer.OrdinalIgnoreCase);

    public INodeExecutor Resolve(string type)
    {
        if (_executors.TryGetValue(type, out var executor))
            return executor;

        throw new InvalidOperationException($"Executor for node type '{type}' is not registered.");
    }
}
