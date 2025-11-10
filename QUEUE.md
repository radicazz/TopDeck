# Work Queue

Central list of ready-to-pick tasks. Update when you claim/finish an item.

## 1. Upgrade HUD Wiring (High)
- Scene: `Assets/Scenes/Game.unity`
- Add UpgradePanel UI (buttons, labels, status) per `UNITY_WIRING_GUIDE.md`.
- Assign `UpgradeShop`, `UpgradeHudPresenter`, button events, and ensure costs/status reflect runtime values.

## 2. Variant Telemetry UI (High)
- Based on `Docs/ProceduralFeaturePlan.md`.
- Create `VariantTelemetryPresenter` script + HUD widget showing wave mutators.
- Emit `GameController.VariantRolled` event from `SpawnEnemies` and subscribe for text updates/tooltips.

## 3. Shader Authoring (Medium)
- Need two custom shaders (`Assets/Shaders/`): vertex displacement + albedo tint.
- Create materials and hook via `UpgradeVisualShaderDriver`.
- Document hookup steps in README/AGENTS so future agents know assignments.

## 4. VFX Hookup (Medium)
- Build particle prefab for upgrade burst + screen-space post-process profile.
- Assign to `UpgradeVfxSpawner` and `PostProcessPulseController` in scene.
- Trigger via `UpgradeShop` UnityEvents and low-health logic.

## 5. Procedural Balancing & Tests (Medium)
- Move variant scaling constants into serialized curves/config asset.
- Add EditMode tests for `EnemyVariantGenerator`.
- Update `GameController` difficulty knobs + log aggregates when `debugMode` is true.

## 6. Validation Checklist
- After each task run `./Tools/validate_changes.sh`.
- If scenes/prefabs touched, also run MCP hierarchy checks for missing refs.
