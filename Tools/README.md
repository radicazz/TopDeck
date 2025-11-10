# Validation Tools Guide

This directory contains validation scripts to ensure non-breaking changes to the Unity project.

## Scripts

### `validate_changes.sh`
**Main validation script - run this after making any changes.**

Performs checks on:
1. C# compilation errors (from Unity logs)
2. Script syntax validation (braces, class names)
3. Meta file integrity (ensures all assets have .meta files)
4. Prefab reference integrity (checks for missing references)
5. Scene file validity (YAML format check)

**Usage:**
```bash
./Tools/validate_changes.sh
```

**Exit codes:**
- `0` = All checks passed
- `1` = One or more checks failed

### `mcp_validation_guide.sh`
**Reference guide for MCP-based validation.**

Shows how to use Unity MCP tools for validation through AI agents.

**Usage:**
```bash
./Tools/mcp_validation_guide.sh
```

## Validation Workflow for AI Agents

### After Script Changes
```bash
./Tools/validate_changes.sh
```
Expected: All syntax checks pass, no compilation errors

### After Asset Changes (Prefabs, Scenes, Models)
```bash
./Tools/validate_changes.sh
```
Expected: Meta files intact, no missing references

### Before Committing
```bash
./Tools/validate_changes.sh
git status
git add -A
git commit -m "your message"
```
Expected: Clean validation, then commit

## MCP Tools Alternative

If using MCP-enabled AI agents, use these MCP tools instead:

1. **Recompile**: `recompile_scripts`
2. **Check Logs**: `unity://logs`
3. **Scene Check**: `unity://scenes-hierarchy`
4. **Asset Check**: `unity://assets`
5. **Run Tests**: `run_tests` (EditMode)

See `mcp_validation_guide.sh` for details.

## Common Failures and Fixes

### Compilation Errors
- Open Unity Editor and check Console
- Fix syntax errors in reported files
- Re-run validation

### Missing Meta Files
- Unity generates these automatically
- Ensure Unity Editor has imported the asset
- May need to restart Unity Editor

### Brace Mismatches
- Check script for unclosed braces `{}`
- Use IDE auto-formatting
- Fix and re-run validation

### Class Name Mismatches
- Filename must match class name exactly
- Example: `GameController.cs` must contain `public class GameController`
- Rename file or class to match
