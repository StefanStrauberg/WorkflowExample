namespace WorkflowDemo.Model;

public sealed class NodeDefinition
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Type { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Key { get; init; }
    public Dictionary<string, object?> Config { get; init; } = [];
}
