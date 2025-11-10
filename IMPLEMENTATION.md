# Implementation Notes (Part 3)

This document records what has already been implemented for the Part 3 requirements (`REQUIRMENTS.md`) and what is still outstanding. Use it as the working checklist for the remaining scope.

## Current State

### Upgradable Defenders & Tower
- `Assets/Scripts/Upgrades/UpgradeManager.cs` centralizes defender/tower upgrade levels and provides health/damage/fire-rate modifiers.
- `Assets/Scripts/Upgrades/DefenderUpgrade.cs` applies UpgradeManager modifiers to each defender and swaps materials for basic visual changes.
- `Assets/Scripts/SimpleHealth.cs` supplies a lightweight health component for defenders.
- Tower health bonus is applied when the game initializes (`GameController.InitializeGame`).

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
- Add upgrade triggers/UI (`UpgradeShop` or similar) so players can spend resources to raise defender/tower levels.
- Ensure at least two meaningful upgrade tiers per defender and for the tower.
- Replace reflection in `DefenderUpgrade` by exposing proper setters/API on `DefenseController` or caching baseline stats on Awake.
- Track per-defender upgrades (individual vs global) based on intended design; document the approach.
- Balance resource costs, cooldowns, and upgrade timing; update `GameController` to account for new flows.
- Expand visual feedback (materials, meshes, lights) so each level reads clearly.

### 2. Custom Shaders (≥2)
- Author two shaders under `Assets/Shaders/`:
  1. Vertex displacement shader (e.g., animated plating on upgraded towers).
  2. Base-color modulation shader (e.g., glowy cores responding to tower health).
- Provide materials/prefabs in `Assets/Materials/` that reference these shaders.
- Create driver scripts (e.g., `UpgradeVisualShaderDriver.cs`) to push gameplay data into shader parameters.
- Document hookup steps in README/AGENTS for consistency.

### 3. Custom Visual Effects (≥1 particle + 1 screen space)
- Design at least one custom particle system (upgrade burst, projectile impact, etc.) and expose it via a controller script (e.g., `UpgradeVfxSpawner.cs`).
- Implement a custom screen-space/post-process effect (chromatic pulse, vignette, etc.) with a controlling script (e.g., `PostProcessController.cs`).
- Ensure both effects are triggered from gameplay events (upgrade, low tower health, wave start).

### 4. Procedural Feature Plan & Polish
- Draft the required 400–500 word planning document with diagrams describing the procedural feature (variants or alternative idea), store it under `Docs/ProceduralFeaturePlan.md`.
- Verify the implemented feature matches the document (adjust behavior/UI as needed).
- Surface procedural variant metadata to the player (UI panel, enemy inspector, etc.) to demonstrate impact.

### 5. Balancing & Complexity
- Rebalance enemy counts, spawn cadence, and rewards so the game remains challenging after upgrades.
- Add config fields in `GameController` for tuning procedural variant difficulty.
- Consider edit-mode tests for UpgradeManager math or variant generation to demonstrate rigor.
- Clean up excessive `Debug.Log` spam before final delivery.

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

