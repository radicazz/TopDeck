#!/bin/bash
# Quick MCP-based validation using Unity MCP tools
# This script demonstrates how to use MCP tools for validation

PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

echo "=================================="
echo "MCP Unity Validation"
echo "=================================="
echo "This script shows MCP commands for validation."
echo "Run these through your MCP-enabled AI client."
echo ""

cat << 'EOF'
## MCP Validation Workflow

### 1. Check Compilation (via Unity Console Logs)
Use MCP tool: `unity://logs`

Ask: "Show me the Unity console logs and check for compilation errors"

Expected: No errors with "error CS" in the output


### 2. Check Scene Hierarchy
Use MCP tool: `unity://scenes-hierarchy`

Ask: "Show me the scene hierarchy for the current scene"

Expected: Scene loads without missing references


### 3. Verify Prefab Integrity
Use MCP tool: `unity://assets` with filter for prefabs

Ask: "List all prefabs and check for missing script references"

Expected: No "Missing (Mono Script)" warnings


### 4. Recompile Scripts
Use MCP tool: `recompile_scripts`

Ask: "Recompile all Unity scripts and report any errors"

Expected: Clean compilation with no errors


### 5. Run Edit Mode Tests (if tests exist)
Use MCP tool: `run_tests` with mode="EditMode"

Ask: "Run all EditMode tests and report results"

Expected: All tests pass


## Quick Validation Command

After making changes, ask your AI agent:

"Validate my changes: 
1. Recompile scripts
2. Check console logs for errors  
3. Verify prefab references are intact
4. Confirm scenes load properly"

EOF

echo ""
echo "For local file-based validation, use:"
echo "  ./Tools/validate_changes.sh"
echo ""
