# Agent Cheat Sheet

## Core Loop
- Work inside the existing Unity layout (scripts in `Assets/Scripts`, scenes in `Assets/Scenes`, prefabs in `Assets/Prefabs`).
- Read `REQUIRMENTS.md`, `IMPLEMENTATION.md`, and `QUEUE.md` before grabbing a task so you know priorities and current owners.
- Keep scripts namespace-free, PascalCase classes, `_camelCase` serialized privates, and Allman braces.

## Validation = Non‑Negotiable
- After **any** code, asset, prefab, or scene change run `./Tools/validate_changes.sh` from the repo root.
- If the script fails, open the Unity Editor (Hub 6.2.10f1), let it compile, clear console errors, and re-run tests before continuing.

## Unity Bridge MCP
- Use the bridge for quick context instead of hunting through the editor manually.
- Most used resources:
  - `unity://editor/state` – confirms play/compilation status and active scene.
  - `unity://project/info` – project path, Unity version, target platform.
  - `unity://editor/selection` – inspect the currently selected GameObject/components.
  - `mcpforunity://tests` – list available Edit/Play Mode tests before running them.

## Fast Workflow Reminders
- Boot through `Assets/Scenes/Start Menu.unity`, but treat `Assets/Scenes/Game.unity` as the primary gameplay scene—keep serialized references intact when editing either.
- Place new feature scripts under the correct feature folder (`Upgrades`, `Visual`, `Procedural`, etc.) so inspectors and tests can find them.
- Scene/prefab edits must be done in the Unity Editor; Unity writes `.meta` files automatically—never hand-edit them.
- Commit messages stay short and imperative; include validation output in PR notes.
