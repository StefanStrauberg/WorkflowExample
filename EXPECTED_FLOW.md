# Ожидаемые маршруты

## Huawei 12.1

```text
Start
 -> Read vendor
 -> Choose vendor        decision=Huawei
 -> Huawei: read firmware
 -> Huawei: choose parser by firmware   decision=MODERN
 -> Huawei modern: read interfaces
 -> Huawei modern transform
 -> End
```

Причины выбора:

```text
Choose vendor:
  Huawei   priority=1   <-- exact match
  Juniper  priority=2
  DEFAULT  priority=100

Firmware:
  MODERN   priority=1   <-- because major version 12 >= 10
  LEGACY   priority=2
```

## Huawei 8.5

```text
Start
 -> Read vendor
 -> Choose vendor        decision=Huawei
 -> Huawei: read firmware
 -> Huawei: choose parser by firmware   decision=LEGACY
 -> Huawei legacy: read interfaces
 -> Huawei legacy transform
 -> End
```

## Juniper 22.4

```text
Start
 -> Read vendor
 -> Choose vendor        decision=Juniper
 -> Juniper: read interfaces
 -> Juniper transform
 -> End
```

Huawei-ноды вообще не выполняются.

## Cisco 17.9

```text
Start
 -> Read vendor
 -> Choose vendor        decision=Cisco
 -> Unknown vendor fallback
 -> End
```

Нет edge с condition `Cisco`, поэтому Engine выбирает `DEFAULT`.
