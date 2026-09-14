namespace WorkflowDemo.Engine;

public sealed class WorkflowContext
{
    private readonly Dictionary<string, object?> _variables = new(StringComparer.OrdinalIgnoreCase);

    public Guid? CurrentNodeId { get; set; }

    public IReadOnlyDictionary<string, object?> Variables => _variables;

    public void Set(string name, object? value) => _variables[name] = value;

    public object? Get(string name)
    {
        _variables.TryGetValue(name, out var value);
        return value;
    }

    public T? Get<T>(string name)
    {
        var value = Get(name);

        if (value is null)
            return default;

        if (value is T typed)
            return typed;

        return (T)Convert.ChangeType(value, typeof(T));
    }
}
