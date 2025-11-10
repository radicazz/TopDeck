# TopDeck

3D tower-defense prototype with procedural terrain, multi-path waves, and scalable difficulty.

## Requirements
- Unity: 6.2.10f1 (open via Unity Hub)

## Run
- Open the project in Unity Hub and press Play in the primary scene: `Assets/Scenes/Start Scene.unity`.
- Tests: `Unity -batchmode -projectPath <path> -runTests -testResults results.xml -quit`
- Build: `Unity -batchmode -projectPath <path> -executeMethod BuildScript.Build -quit`

## MCP Integration (for agents)
- MCP tools are integrated into Unity and exposed to agents via the CLI.
- No manual Node server is required or supported.
- Available tools and conventions are documented in `AGENTS.md`.
- The editor reads `UNITY_MCP_HOST` (or `MCP_SERVER_URL`) at start; set it to the CLI proxy URL so Unity connects only when the proxy is available. Without the env var the connection stays idle (see `Assets/Editor/McpHostBootstrap.cs`).

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
- Telemetry shown in-game via `VariantTelemetryPresenter` UI component
- See `Docs/ProceduralFeaturePlan.md` for design details

## Part 3 Scope
- Core code/features to implement are summarized in `REQUIRMENTS.md` (upgrades, shaders, VFX, and a custom procedural system).
- `IMPLEMENTATION.md` tracks current progress plus remaining tasks (upgrade UI, shaders, VFX, procedural polish).
