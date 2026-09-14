namespace WorkflowDemo.Model;

public sealed class EdgeDefinition
{
    public Guid FromNodeId { get; init; }
    public Guid ToNodeId { get; init; }
    public string? Condition { get; init; }
    public int Priority { get; init; }
}
