# TopDeck

3D tower-defense prototype with procedural terrain, multi-path waves, and scalable difficulty.

## Requirements
- Unity: 6.2.10f1 (open via Unity Hub)

## Run
- Open the project in Unity Hub and press Play in the primary scene: `Assets/Scenes/Start Scene.unity`.
- Tests: `Unity -batchmode -projectPath <path> -runTests -testResults results.xml -quit`
- Build: `Unity -batchmode -projectPath <path> -executeMethod BuildScript.Build -quit`

## Development
- Edit scripts in `Assets/Scripts/`
- Open scenes in Unity Editor from `Assets/Scenes/`
- Run tests via Unity Test Runner or command line
- See `AGENTS.md` for coding conventions and workflow

## Repo Layout
- `Assets/Scripts/` C# gameplay code
- `Assets/Scenes/` Unity scenes
- `Assets/Prefabs/` gameplay prefabs
- `Assets/Shaders/` custom URP shaders
- `Assets/Tests/` edit and play mode tests
- `Docs/` implementation plans and documentation

## Custom Shaders
- **UpgradeVertexDisplacement.shader**: Animated vertex displacement based on upgrade level
  - Assign to defender materials, drive with `_UpgradeLevel` property (0-1)
  - Displacement amount and frequency controllable via material properties
- **UpgradeColorModulation.shader**: Color lerping and emission effects for upgrades
  - Lerps from base color to upgrade color based on `_UpgradeLevel`
  - Includes pulsing effect and emission strength
  - Use `UpgradeVisualShaderDriver` component to auto-update properties

## Procedural Variant System
- Enemies are procedurally varied each spawn with stat mutations and visual tints
- Controlled by `ProceduralVariantConfig` ScriptableObject asset (create via Assets menu)
- Difficulty scales with wave number and defender upgrade level
- Variant categories (Normal / Elite / Mini-Boss) apply additional multipliers + tinted feedback pulled from the config
- Telemetry shown in-game via `VariantTelemetryPresenter` UI component (pattern, difficulty rating, elite/mini-boss counts)
- See `Docs/ProceduralFeaturePlan.md` for design details

## Adaptive Difficulty & Spawn Patterns
- `AdaptiveWaveDifficulty` records wave completion time + player health loss to adjust enemy counts, spawn tempo, and elite/mini-boss quotas per wave.
- `AdaptiveWaveDifficultyConfig` ScriptableObjects (Assets → Create → TopDeck) store the curves/weights; assign one to `GameController` to tune difficulty without code.
- `ProceduralSpawnPatternPlanner` builds alternating, burst, focused, surround, and escort spawn instructions that `GameController` consumes when instantiating attackers.
- Per-wave summaries (pattern + threat multipliers) surface on the HUD so players understand procedural spikes.

## Part 3 Implementation Status

### ✅ **Upgradable Defenders & Tower (20%)**
- **Complete upgrade system** with UpgradeManager, UpgradeShop, DefenderUpgrade
- **Two upgrade levels** per defender/tower with stat scaling (health +50%/+100%, damage +25%/+50%)
- **Visual feedback** via UpgradeVisualShaderDriver (auto-applies to prefabs)
- **Costs configured**: Defender upgrades $200, Tower upgrades $300
- **UI framework** in place with UpgradeHudPresenter managing purchase flow
- **Manual wiring required**: Assign UpgradeShop reference and buttons in UpgradeHudPresenter inspector

### ✅ **Custom Shaders (10%)**
- **UpgradeVertexDisplacement.shader**: Vertex animation based on upgrade level
- **UpgradeColorModulation.shader**: Color lerping and emission effects
- **Materials created**: UpgradeVertexMaterial.mat, UpgradeColorMaterial.mat
- **Manual assignment needed**: Set shader on materials, assign materials to prefabs
- **Integration**: UpgradeVisualShaderDriver component updates shader properties automatically

### ✅ **Procedural Enemy Variants (20%)**
- **EnemyVariantGenerator**: Runtime stat mutations (health, speed, damage scaling)
- **Visual differentiation**: EnemyVariantAppearance applies color tinting per category
- **Adaptive difficulty**: AdaptiveWaveDifficulty adjusts based on player performance  
- **Spawn patterns**: SpawnPatternPlanner creates alternating, burst, focused patterns
- **Designer tools**: ProceduralVariantConfig & AdaptiveWaveDifficultyConfig ScriptableObjects
- **Live telemetry**: VariantTelemetryPresenter shows pattern/difficulty in-game

### ⚠️ **VFX System (10%)**
- **Particle setup**: UpgradeVfxSpawner configured for upgrade effects
- **Post-processing**: PostProcessPulseController for screen effects
- **Manual setup needed**: Create particle prefabs, assign Volume Profile for pulse effects

### ✅ **System Integration**
- **Scene structure**: Game.unity has UpgradePanel with buttons and UpgradeHudPresenter
- **Component wiring**: GameController has UpgradeShop and UpgradeVfxSpawner
- **Prefab integration**: Defender prefabs have UpgradeVisualShaderDriver component
- **Cost balance**: Defender upgrades $200, Tower $300, enemy rewards $20

## Manual Setup Required
1. **Unity Inspector**: Wire UpgradeHudPresenter references (upgradeShop, defenderButton, towerButton)
2. **Materials**: Assign shaders to materials (UpgradeVertexDisplacement → UpgradeVertexMaterial.mat)
3. **Prefab materials**: Apply upgrade materials to defender models 
4. **VFX**: Create particle prefabs and assign to UpgradeVfxSpawner
5. **Volume Profile**: Create and assign post-process profile for pulse effects

## Part 3 Scope
- Core code/features to implement are summarized in `REQUIRMENTS.md` (upgrades, shaders, VFX, and a custom procedural system).
- `IMPLEMENTATION.md` tracks current progress plus remaining tasks (upgrade UI, shaders, VFX, procedural polish).
