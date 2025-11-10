# Implementation Notes (Part 3)

This document records what has already been implemented for the Part 3 requirements (`REQUIRMENTS.md`) and what is still outstanding. Use it as the working checklist for the remaining scope.

## Current State

### Upgradable Defenders & Tower
- `Assets/Scripts/Upgrades/UpgradeManager.cs` centralizes defender/tower upgrade levels and provides health/damage/fire-rate modifiers.
- `Assets/Scripts/Upgrades/DefenderUpgrade.cs` applies UpgradeManager modifiers to each defender and swaps materials for basic visual changes.
- `Assets/Scripts/SimpleHealth.cs` supplies a lightweight health component for defenders.
- Tower health bonus is applied when the game initializes (`GameController.InitializeGame`).
- `Assets/Scripts/Upgrades/UpgradeShop.cs` provides purchasable upgrades (costs, hotkeys, UnityEvents) that spend currency via `GameController.TrySpendMoney`.
- `GameController` now exposes `TrySpendMoney`, `AddMoney`, and `GetTowerMaxHealth` helpers for UI/scripts.
- `Assets/Scripts/UI/UpgradeHudPresenter.cs` is the runtime HUD driver (buttons, labels, status) that wraps `UpgradeShop`; scene wiring + references are still pending.
- `Assets/Scripts/Visual/UpgradeVisualShaderDriver.cs` now auto-finds renderers/material property blocks so defenders react visually to upgrade levels (prefab hookup added).

### Procedural Variant System
- `Assets/Scripts/Procedural/EnemyVariantGenerator.cs` mutates attacker stats per spawn, factoring in wave index and defender upgrade level.
- `Assets/Scripts/Procedural/EnemyVariantAppearance.cs` tints spawned enemies to differentiate variants.
- `GameController.SpawnEnemies` now instantiates variants and applies their stats/tints.

### Documentation & Agent Guidance
- `AGENTS.md` explains tool usage, Part 3 scope, and provides a compliant `write_script` example.
- `README.md` is up to date (Unity version, run/build/test commands, main scene reference).
- `REQUIRMENTS.md` summarizes Part 3 deliverables with mark weights.

## Outstanding Work

### 1. Upgrade Gameplay Loop
- Build player-facing UI around `UpgradeShop` (buttons, input, costs) so upgrades can be purchased outside of debug hotkeys.
- Ensure at least two meaningful upgrade tiers per defender and for the tower (stats + visuals).
- Replace remaining reflection in `DefenderUpgrade` by exposing proper setters/API on `DefenseController` or caching baseline stats on Awake (partial: baseline caching done; still consider explicit API).
- Track per-defender upgrades (individual vs global) based on intended design; document the approach.
- Balance resource costs, cooldowns, and upgrade timing; update `GameController` to account for new flows.
- Expand visual feedback (materials, meshes, lights) so each level reads clearly.

### 2. Custom Shaders (≥2)
- ✅ Created `Assets/Shaders/UpgradeVertexDisplacement.shader` (vertex displacement with upgrade level control)
- ✅ Created `Assets/Shaders/UpgradeColorModulation.shader` (base color modulation with lerp, pulse, and emission)
- Both shaders are URP-compatible and integrate with `UpgradeVisualShaderDriver.cs`
- [ ] Create materials using these shaders
- [ ] Assign materials to defender/tower prefabs
- [ ] Wire `UpgradeVisualShaderDriver` to update shader properties based on upgrade level

### 3. Custom Visual Effects (≥1 particle + 1 screen space)
- `Assets/Scripts/Visual/UpgradeVfxSpawner.cs` and `Assets/Scripts/Visual/PostProcessPulseController.cs` exist (helpers auto-manage anchors/volumes) but still need concrete prefabs/Volume assets assigned via MCP.
- Ensure both effects are triggered from gameplay events (upgrade, low tower health, wave start) via `UpgradeShop` UnityEvents and scene-level wiring.

### 4. Procedural Feature Plan & Polish
- ✅ Created `Docs/ProceduralFeaturePlan.md` planning document
- ✅ Created `ProceduralVariantConfig` ScriptableObject for designer-tunable curves
- ✅ Created `VariantTelemetryPresenter` UI component for displaying variant stats
- ✅ Updated `EnemyVariantGenerator` to support config-driven curves and expose multipliers
- ✅ Integrated telemetry into `GameController` to show average variant stats per wave
- ✅ Added tests for variant generation (`EnemyVariantGeneratorTests`)
- [ ] Create `ProceduralVariantConfig` asset instance and assign to GameController
- [ ] Wire `VariantTelemetryPresenter` UI in scene
- [ ] Tune AnimationCurves in config for balanced difficulty scaling

### 5. Balancing & Complexity
- ✅ Added `UpgradeManagerTests` for verifying upgrade modifier calculations
- ✅ Added `EnemyVariantGeneratorTests` for variant generation bounds checking
- [ ] Rebalance enemy counts, spawn cadence, and rewards so the game remains challenging after upgrades
- [ ] Tune config fields in `ProceduralVariantConfig` for procedural variant difficulty
- [ ] Run edit-mode tests to verify upgrade math and variant generation
- [ ] Clean up excessive `Debug.Log` spam before final delivery

### 6. VFX/Shader Integration Tasks
- Add placeholder assets or references so agents know where to assign materials/post effects (document paths).
- Verify Start Scene references new scripts (UpgradeManager, VFX controllers, post-process volume).
- Ensure new scripts follow the folder→namespace rule (namespace-free per current convention).

## Recommended Execution Order
1. Finish upgrade gameplay loop (UI, costs, baseline stat handling).
2. Implement shaders/materials plus corresponding control scripts.
3. Add particle + screen-space effects and hook them to gameplay events.
4. Write the procedural feature plan document and align implementation/UI.
5. Final balancing pass (enemy scaling, upgrade pacing) and polish logs/documentation.

This sequencing keeps deliverables aligned with the mark weights (upgrades/shaders/VFX/procedural) and minimizes rework across agents.
