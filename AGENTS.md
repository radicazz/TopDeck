# Repository Guidelines

## Project Structure & Module Organization
- Unity layout:
  - `Assets/` content
    - `Assets/Scripts/` C# code (required for MCP tools)
    - `Assets/Scenes/` `.unity` scenes
    - `Assets/Prefabs/` prefabs
  - `ProjectSettings/`, `Packages/`, `Library/` (Unity-managed)
  - Primary scene: `Assets/Scenes/Start Scene.unity`
- Unity MCP (Integrated):
  - MCP tools are built into the Unity project and accessible to agents; do not run a separate Node server.
  - Use the MCP tools exposed by the CLI; no additional setup docs are required.
  - Any Node entry points (e.g., `index.js`) are not used for manual runtime.

## Build, Test, and Development Commands
- Unity Editor: open with Unity Hub `6.2.10f1`.
- Unity CLI:
  - Tests: `Unity -batchmode -projectPath <path> -runTests -testResults results.xml -quit`
  - Build: `Unity -batchmode -projectPath <path> -executeMethod BuildScript.Build -quit`
  
## Gameplay Requirements
- Core, code-focused Part 3 requirements are summarized in `REQUIRMENTS.md`. Consult this when implementing upgrades, shaders, VFX, and the custom procedural feature.

## Coding Style & Naming Conventions
- C#
  - 4-space indent, UTF-8, Unix EOL; Allman braces.
  - Names: `PascalCase` types/methods; `_camelCase` private fields; `camelCase` locals/params.
  - Namespace mirrors folders under `Assets/Scripts/` (e.g., `Assets/Scripts/AI/` → `TopDeck.AI`).
  - Keep MonoBehaviours focused; prefer composition.
- Assets: Scenes like `Level_01.unity`; prefabs like `Enemy_Goblin.prefab`.

## Testing Guidelines
- Unity Test Framework.
  - Locations: `Assets/Tests/EditMode/`, `Assets/Tests/PlayMode/`.
  - Names: `FeatureNameTests.cs`; methods `[Test] Should_DoThing()`.
- In PRs, include: what changed, how verified (edit/play), and any coverage deltas.

## Commit & Pull Request Guidelines
- Commits: imperative, concise (≤72 chars). Example: `Add enemy spawn controller and wave config`.
- If changes were made via MCP tools, prefix scope with `mcp:` (e.g., `mcp: add EnemySpawner.cs`).
- PRs: description, linked issues, screenshots/GIFs for scene/prefab/UI, and test results. Note Unity version.

## Security & Configuration Tips
- No secrets or local paths in repo. Respect `.gitignore` (e.g., `Library/`, `Logs/`).
- MCP is integrated; client access is provided via the CLI tools (no separate setup doc).

## Agent-Specific Notes (Unity-MCP)
- Available tools: `list_scripts`, `read_script`, `write_script`, `search_code`, `list_scenes`, `list_prefabs`, `get_project_structure`.
- Paths are relative to `Assets/`. Scripts must live under `Assets/Scripts/` for listing/search to work.
- `write_script` creates/updates files and Unity will generate `.meta` on import; do not hand-edit `.meta` files.
- Resource URIs use `file://...`; reading is text-only.
- When generating new scripts, match filename/class, include a namespace aligned to folder, and add a brief XML summary. Example: `Assets/Scripts/AI/EnemySpawner.cs` → `namespace TopDeck.AI { public class EnemySpawner : MonoBehaviour { } }`.
- Do not rename top-level folders (`Assets/Scenes`, `Assets/Prefabs`, `Assets/Scripts`) — MCP tool pathing assumes these conventions.

## MCP Usage (Tasks → Tools)
- Find code by name or concept: use `search_code` then `read_script` for details.
- List available scripts/scenes/prefabs: use `list_scripts`, `list_scenes`, `list_prefabs`.
- Add or change C# code: use `write_script` under `Assets/Scripts/…` with proper namespace and matching class/filename.
- Survey the project layout: use `get_project_structure`.
- Wireups and assets: guide changes in scenes/prefabs; use code hooks and clear instructions where MCP cannot author assets.
- Testing/build: output Unity CLI commands in responses; do not attempt to run the Unity Editor.

When asked to implement Part 3 items (see `REQUIRMENTS.md`):
- Upgrades: create systems/components under `Assets/Scripts/Upgrades/…`; expose tunables via `SerializeField`; ensure visuals update via materials/VFX.
- Shaders/VFX: add scripts to apply and drive effects; reference materials/post-processing profiles by name; keep code in `Assets/Scripts/Visual/…`.
- Procedural feature: place runtime systems under `Assets/Scripts/Procedural/…`; integrate with the main loop (e.g., enemy waves, loot, music).

### Example: `write_script` payload (creates a new MonoBehaviour)

Use `write_script` with a path under `Assets/Scripts/...` and content that follows our C# standards. Filename and class name must match. Namespaces mirror the folder path under `Assets/Scripts/`.

Request body (representative):

```
{
  "path": "Assets/Scripts/Upgrades/TowerUpgrade.cs",
  "overwrite": true,
  "content": "<C# shown below>"
}
```

TowerUpgrade.cs:

```
using UnityEngine;

namespace TopDeck.Upgrades
{
    /// <summary>
    /// Applies incremental health and visual upgrades to a tower.
    /// Levels and effects are serialized for tuning in the Inspector.
    /// </summary>
    public class TowerUpgrade : MonoBehaviour
    {
        [Header("Upgrade Settings")]
        [SerializeField] private int _level = 0;
        [SerializeField] private int _maxLevel = 2;
        [SerializeField] private int _healthBonusPerLevel = 25;

        [Header("Visuals")] 
        [SerializeField] private Renderer _rendererRef;
        [SerializeField] private Material _upgradedMaterial;

        /// <summary>Current upgrade level.</summary>
        public int Level => _level;
        /// <summary>True when another upgrade can be applied.</summary>
        public bool CanUpgrade => _level < _maxLevel;

        /// <summary>Apply one upgrade step: increases health and updates visuals.</summary>
        public void ApplyUpgrade()
        {
            if (!CanUpgrade) return;
            _level++;

            var health = GetComponent<Health>();
            if (health != null)
            {
                health.MaxHealth += _healthBonusPerLevel;
                health.CurrentHealth = Mathf.Min(health.CurrentHealth + _healthBonusPerLevel, health.MaxHealth);
            }

            UpdateVisuals();
        }

        /// <summary>Updates materials or other effects to reflect the current level.</summary>
        private void UpdateVisuals()
        {
            if (_rendererRef != null && _upgradedMaterial != null)
            {
                _rendererRef.sharedMaterial = _upgradedMaterial;
            }
        }
    }
}
```

Notes:
- 4-space indent, Allman braces; private fields use `_camelCase`.
- Place scripts under `Assets/Scripts/...`; Unity will generate `.meta` files automatically.
- Keep the class name identical to the filename and align the namespace to the folder path.
