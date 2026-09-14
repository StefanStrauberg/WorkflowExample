using WorkflowDemo.Model;

namespace WorkflowDemo;

public static class WorkflowFactory
{
    public static WorkflowDefinition Create()
    {
        var start = Node("Start", "Start");
        var getVendor = Node("MockSnmp", "Read vendor", new() { ["source"] = "vendor", ["output"] = "device.vendor" });
        var vendorDecision = Node("Decision", "Choose vendor", new() { ["variable"] = "device.vendor" });

        var getHuaweiFirmware = Node("MockSnmp", "Huawei: read firmware", new() { ["source"] = "firmware", ["output"] = "device.firmware" });
        var firmwareDecision = Node("Decision", "Huawei: choose parser by firmware", new()
        {
            ["variable"] = "device.firmware",
            ["mode"] = "firmware-major"
        });

        var getHuaweiModernInterfaces = Node("MockSnmp", "Huawei modern: read interfaces", new() { ["source"] = "interfaces", ["output"] = "raw.interfaces" });
        var getHuaweiLegacyInterfaces = Node("MockSnmp", "Huawei legacy: read interfaces", new() { ["source"] = "interfaces", ["output"] = "raw.interfaces" });
        var huaweiModern = Node("Script", "Huawei modern transform", new() { ["scriptKey"] = "huawei-modern" });
        var huaweiLegacy = Node("Script", "Huawei legacy transform", new() { ["scriptKey"] = "huawei-legacy" });

        var getJuniperInterfaces = Node("MockSnmp", "Juniper: read interfaces", new() { ["source"] = "interfaces", ["output"] = "raw.interfaces" });
        var juniperScript = Node("Script", "Juniper transform", new() { ["scriptKey"] = "juniper" });

        var genericScript = Node("Script", "Unknown vendor fallback", new() { ["scriptKey"] = "generic" });
        var end = Node("End", "End");

        return new WorkflowDefinition
        {
            Name = "Network device discovery demo",
            Version = 1,
            StartNodeId = start.Id,
            Nodes =
            [
                start,
                getVendor,
                vendorDecision,
                getHuaweiFirmware,
                firmwareDecision,
                getHuaweiModernInterfaces,
                getHuaweiLegacyInterfaces,
                huaweiModern,
                huaweiLegacy,
                getJuniperInterfaces,
                juniperScript,
                genericScript,
                end
            ],
            Edges =
            [
                Edge(start, getVendor),
                Edge(getVendor, vendorDecision),

                Edge(vendorDecision, getHuaweiFirmware, "Huawei", 1),
                Edge(vendorDecision, getJuniperInterfaces, "Juniper", 2),
                Edge(vendorDecision, genericScript, "DEFAULT", 100),

                Edge(getHuaweiFirmware, firmwareDecision),
                Edge(firmwareDecision, getHuaweiModernInterfaces, "MODERN", 1),
                Edge(firmwareDecision, getHuaweiLegacyInterfaces, "LEGACY", 2),
                Edge(getHuaweiModernInterfaces, huaweiModern),
                Edge(getHuaweiLegacyInterfaces, huaweiLegacy),

                Edge(huaweiModern, end),
                Edge(huaweiLegacy, end),

                Edge(getJuniperInterfaces, juniperScript),
                Edge(juniperScript, end),

                Edge(genericScript, end)
            ]
        };
    }

    private static NodeDefinition Node(
        string type,
        string name,
        Dictionary<string, object?>? config = null)
        => new()
        {
            Type = type,
            Name = name,
            Key = name.ToLowerInvariant().Replace(' ', '-'),
            Config = config ?? []
        };

    private static EdgeDefinition Edge(
        NodeDefinition from,
        NodeDefinition to,
        string? condition = null,
        int priority = 0)
        => new()
        {
            FromNodeId = from.Id,
            ToNodeId = to.Id,
            Condition = condition,
            Priority = priority
        };
}
