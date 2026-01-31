# OrgStructBuilder (Alpha)

**OrgStructBuilder** is a high-performance .NET library designed for calculating **Indirect Ownership** and **Effective Control** within complex corporate structures. By leveraging matrix-based linear algebra (via `MathNet.Numerics`), it efficiently handles multi-layered investment chains and circular ownership scenarios.

> **Note:** This is an **Alpha version**. It is currently undergoing active development. New features and breaking changes may be introduced frequently.

## Key Features
* **Indirect Ownership Calculation**: Automatically derives final ownership percentages through complex investment chains.
* **Full Control Simulation**: Supports modeling where ownership above a certain threshold is treated as 100% control (Ideal for Pillar 2 Global Minimum Tax analysis).
* **Matrix-based Engine**: Optimized for enterprise-level data processing using sparse and dense matrix operations.



## Configuration Options
You can fine-tune the control logic using `OrgStructBuildOption`.

| Option | Type | Description |
| :--- | :--- | :--- |
| `FullControlThreshold` | `double` | The threshold to assume 100% control (e.g., `0.5` for 50%). |
| `MoreThanOrEqual` | `bool` | `true` for "Greater than or equal to" (>=), `false` for "Strictly greater than" (>). |

## Practical Example
**Scenario**: `Entity A (90%) -> Entity B (50%) -> Entity C`

1. **Standard Equity Method** (`FullControlThreshold = 1.0`):
   * A's ownership of C = **45%** ($0.9 \times 0.5$).
2. **Control-Based Method** (`FullControlThreshold = 0.5`, `MoreThanOrEqual = true`):
   * Since B owns >= 50% of C, B's control is treated as 100%.
   * A's ownership of C = **90%** ($0.9 \times 1.0$).
3. **Strict Control Method** (`FullControlThreshold = 0.5`, `MoreThanOrEqual = false`):
   * Since 50% does not *exceed* the threshold, no full control is granted.
   * A's ownership of C = **45%**.

## Quick Start
```csharp
var builder = new OrgStructBuilder();
var options = new OrgStructBuildOption 
{ 
    FullControlThreshold = 0.5, 
    MoreThanOrEqual = true 
};

var result = builder.CalculateIndirectOwnership(dtoList, fromId, toId, options);
Console.WriteLine($"Total Ownership: {result.TotalOwnershipPercent:P2}");
