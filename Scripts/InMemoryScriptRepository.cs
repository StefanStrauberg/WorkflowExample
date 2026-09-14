using WorkflowDemo.Engine;

namespace WorkflowDemo.Scripts;

// Demo replacement for JavaScript stored in DB.
// In production this abstraction can return JS text and ScriptNodeExecutor
// can execute it in a sandbox (for example, Jint).
public sealed class InMemoryScriptRepository : IScriptRepository
{
    private readonly Dictionary<string, Func<WorkflowContext, Dictionary<string, object?>>> _scripts =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["huawei-modern"] = context =>
            {
                var interfaces = context.Get<string[]>("raw.interfaces") ?? [];
                return new()
                {
                    ["result.parser"] = "Huawei modern parser",
                    ["result.ports"] = interfaces.Select(x => $"HW-MODERN:{x}").ToArray()
                };
            },

            ["huawei-legacy"] = context =>
            {
                var interfaces = context.Get<string[]>("raw.interfaces") ?? [];
                return new()
                {
                    ["result.parser"] = "Huawei legacy parser",
                    ["result.ports"] = interfaces.Select(x => $"HW-LEGACY:{x}").ToArray()
                };
            },

            ["juniper"] = context =>
            {
                var interfaces = context.Get<string[]>("raw.interfaces") ?? [];
                return new()
                {
                    ["result.parser"] = "Juniper parser",
                    ["result.ports"] = interfaces.Select(x => $"JUNIPER:{x}").ToArray()
                };
            },

            ["generic"] = context => new()
            {
                ["result.parser"] = "Generic parser",
                ["result.ports"] = Array.Empty<string>()
            }
        };

    public Func<WorkflowContext, Dictionary<string, object?>> Get(string scriptKey)
    {
        if (_scripts.TryGetValue(scriptKey, out var script))
            return script;

        throw new InvalidOperationException($"Script '{scriptKey}' not found.");
    }
}
