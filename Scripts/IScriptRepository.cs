using WorkflowDemo.Engine;

namespace WorkflowDemo.Scripts;

public interface IScriptRepository
{
    Func<WorkflowContext, Dictionary<string, object?>> Get(string scriptKey);
}
