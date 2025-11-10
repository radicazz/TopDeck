# Procedural Enemy Variant Feature Plan

## Purpose
Player runs often feel identical once the optimal defender placements are solved. To keep Part 3 exciting we need a procedural layer that continuously nudges the combat sandbox. The existing `EnemyVariantGenerator` already mutates attacker stats per spawn using the current wave index and defender upgrade level. This document formalizes how that system satisfies the 20% procedural requirement, what telemetry we surface to players, and which extra hooks are needed so future agents can tune difficulty without touching code.

## Existing System Snapshot
1. `GameController.SpawnEnemies` builds a spawn queue each combat phase.
2. Before each instantiate call, `EnemyVariantGenerator.CreateVariant(baseType, wave, defenderLevel)` returns a lightweight `Variant` struct with a synthesized `AttackerTypeDefinition` plus a tint colour.
3. The synthesized definition adjusts health, movement speed, tower damage, attack range/rate, and damage-to-player within ±10–20% bands scaled by `waveFactor` (1 + 0.05 * (wave - 1)) and `counterFactor` (1 + 0.03 * defenderLevel). This means higher waves and a well-upgraded defender line both produce nastier invaders, keeping pressure on players that invest heavily in upgrades.
4. After instantiating, `AttackerController.Initialize` receives the procedurally mutated stats, while `EnemyVariantAppearance.Initialize` applies the colour tint so the run-specific mutations are readable in 3D.

## Data Flow
```
Wave Start
   ↓ (currentWave, money, upgrades)
GameController.SpawnEnemies
   ↓ per spawn
EnemyVariantGenerator
   ↙                ↘
Stat Resample      Tint Selection
   ↓                ↓
AttackerController ← EnemyVariantAppearance
```

## Player-Facing Surfacing
To earn full marks we must clearly communicate how variants behave. Planned UI work:
- Extend `Panel HUD` with a small `VariantTelemetry` widget that shows the latest multiplier (e.g., “Wave 5 Mutators: +18% HP, +5% SPD”) using the generated `Variant` data.
- Whenever `EnemyVariantGenerator` rolls an extreme value (top 10% of its band) fire a tooltip via `UpgradeHudPresenter`’s status label (“Scouts report nimble enemies this wave!”).
- Feed the tint colour into the upgrade shaders once they exist so defenders visually read the threat level (warm colours = aggressive, cool = slower but tanky).

## Balancing & Telemetry
- Introduce serialized AnimationCurves on `EnemyVariantGenerator` (or a new `ProceduralVariantConfig` ScriptableObject) to replace the current hard-coded coefficients. Designers can then shape difficulty spikes without code edits.
- Log aggregate averages each wave (mean HP, speed, tint hue) so tuning passes have data. We can reuse the existing `debugMode` flag on `GameController` to gate this spam.
- Add a lightweight Edit Mode test (`EnemyVariantGeneratorTests`) that seeds the RNG and asserts health/colour bounds. This protects against regressions when retuning curves.

## Implementation Checklist
- [ ] Create `VariantTelemetryPresenter` UI component that subscribes to a new `GameController.VariantRolled` C# event carrying the `Variant` struct.
- [ ] Expose serialized curves/min-max clamps via a `ProceduralVariantConfig` asset referenced by `GameController`.
- [ ] Pipe variant summary text into `UpgradeHudPresenter` so upgrades and procedural difficulty share a single HUD panel.
- [ ] Document tint meanings inside `README.md` so QA knows what “cyan enemies” signifies.

Once these items land, the procedural feature is both technically robust and player-comprehensible, satisfying the planning deliverable without leaving future agents guessing about intent.
