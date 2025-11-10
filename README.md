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

## Repo Layout
- `Assets/Scripts/` C# gameplay code
- `Assets/Scenes/` Unity scenes
- `Assets/Prefabs/` gameplay prefabs

## Part 3 Scope
- Core code/features to implement are summarized in `REQUIRMENTS.md` (upgrades, shaders, VFX, and a custom procedural system).
