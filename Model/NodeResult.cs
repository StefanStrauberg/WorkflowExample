namespace WorkflowDemo.Model;

public sealed class NodeResult
{
    public bool Success { get; init; } = true;
    public string? Decision { get; init; }
    public Dictionary<string, object?> Outputs { get; init; } = [];
    public string? Error { get; init; }

    public static NodeResult Ok(Dictionary<string, object?>? outputs = null) =>
        new() { Outputs = outputs ?? [] };

    public static NodeResult WithDecision(string decision) =>
        new() { Decision = decision };

    public static NodeResult Failed(string error) =>
        new() { Success = false, Error = error };
}
