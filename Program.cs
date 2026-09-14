using WorkflowDemo;
using WorkflowDemo.Engine;
using WorkflowDemo.Executors;
using WorkflowDemo.Scripts;

var (vendor, firmware) = ReadScenario(args);

Console.WriteLine("Workflow engine demo");
Console.WriteLine("--------------------");
Console.WriteLine($"Input vendor   : {vendor}");
Console.WriteLine($"Input firmware : {firmware}\n");

var scripts = new InMemoryScriptRepository();

var resolver = new NodeExecutorResolver(
[
    new StartNodeExecutor(),
    new EndNodeExecutor(),
    new MockSnmpNodeExecutor(),
    new DecisionNodeExecutor(),
    new ScriptNodeExecutor(scripts)
]);

var engine = new WorkflowEngine(resolver);
var workflow = WorkflowFactory.Create();
var context = new WorkflowContext();

context.Set("input.vendor", vendor);
context.Set("input.firmware", firmware);

await engine.ExecuteAsync(workflow, context);

static (string Vendor, string Firmware) ReadScenario(string[] args)
{
    if (args.Length >= 2)
        return (args[0], args[1]);

    Console.WriteLine("Choose scenario:");
    Console.WriteLine("  1 - Huawei modern firmware (12.1)");
    Console.WriteLine("  2 - Huawei legacy firmware (8.5)");
    Console.WriteLine("  3 - Juniper");
    Console.WriteLine("  4 - Unknown vendor -> DEFAULT");
    Console.Write("Scenario [1]: ");

    var choice = Console.ReadLine();

    return choice switch
    {
        "2" => ("Huawei", "8.5"),
        "3" => ("Juniper", "22.4"),
        "4" => ("Cisco", "17.9"),
        _ => ("Huawei", "12.1")
    };
}
