# WorkflowDemo

Минимальный консольный пример workflow engine для опроса сетевых устройств.

## Что показывает пример

1. Workflow состоит из `NodeDefinition` и `EdgeDefinition`.
2. `WorkflowEngine` ничего не знает про Huawei/Juniper.
3. Каждая node выполняется своим `INodeExecutor`.
4. `DecisionNodeExecutor` возвращает `Decision`.
5. Engine сравнивает `Decision` с `Edge.Condition`.
6. Если точного совпадения нет, используется `DEFAULT` или unconditional edge.
7. Для Huawei есть **вторая развилка** по firmware:
   - major >= 10 -> MODERN
   - major < 10 -> LEGACY
8. `ScriptNodeExecutor` демонстрирует место, где позднее можно подключить JavaScript из БД.

## Граф

```text
Start
  |
Read vendor
  |
Choose vendor
  |---------------- Huawei ------------------|
  |                                           v
  |                                  Read firmware
  |                                           |
  |                                  Choose firmware
  |                                      /        \
  |                                 MODERN        LEGACY
  |                                   |             |
  |                            Read interfaces      |
  |                                   |             |
  |                         Huawei modern JS   Huawei legacy JS
  |                                   \             /
  |                                    \           /
  |                                         End
  |
  |---------------- Juniper -----------------> Read interfaces
  |                                                |
  |                                           Juniper JS
  |                                                |
  |                                               End
  |
  `---------------- DEFAULT -----------------> Generic JS -> End
```

## Запуск

```bash
dotnet run
```

или без интерактивного меню:

```bash
dotnet run -- Huawei 12.1
dotnet run -- Huawei 8.5
dotnet run -- Juniper 22.4
dotnet run -- Cisco 17.9
```

## Что смотреть в выводе

Движок для каждой Decision-ноды печатает:

- значение `DECISION`;
- все candidate edges;
- `WHY`, объясняющий почему выбрана конкретная edge;
- следующую node.

Это специально сделано для понимания алгоритма маршрутизации.

## Где потом появится настоящий JavaScript

Сейчас `InMemoryScriptRepository` хранит C# delegates только для того, чтобы demo не зависело от NuGet.

Production-вариант:

```text
DB
  -> ScriptDefinition { Key, Version, JavaScript }
  -> ScriptRepository
  -> ScriptNodeExecutor
  -> sandboxed JS engine (например Jint)
  -> JSON outputs
  -> WorkflowContext
```

При этом `WorkflowEngine`, `EdgeDefinition` и алгоритм переходов менять не потребуется.
