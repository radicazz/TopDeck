# TopDeck Python Tools for Unity MCP Integration

## Overview
These Python tools are designed to enhance your TopDeck tower defense game development workflow by providing capabilities that complement Unity's built-in features. They integrate with the Unity MCP server to provide powerful development assistance.

## Quick Win Tools (Completed)

### 1. Config Validator (`config_validator.py`)
**Purpose:** Validates ScriptableObject configurations and game parameters to ensure balance and catch errors early.

**Features:**
- Range validation for all game parameters
- Cross-config balance checking (upgrades vs waves)
- Economy balance validation
- Progression curve validation
- Detailed error/warning reports with suggestions

**Usage:**
```python
python3 config_validator.py [config_file.json]
```

**Integration with Unity MCP:**
- Can be called from Unity to validate configs before build
- Reads Unity asset data via MCP resource access
- Returns validation results that can trigger Unity warnings

### 2. Debug Command Generator (`debug_commands.py`)
**Purpose:** Generates comprehensive debug commands for testing all game features quickly.

**Features:**
- 61+ pre-configured debug commands across 8 categories
- Hotkey support for frequent commands
- Unity C# script generation
- Complete documentation generation
- Test scenario automation

**Categories:**
- Upgrades (11 commands)
- Waves (11 commands)
- Spawning (7 commands)
- Economy (5 commands)
- Defenders (7 commands)
- Performance (6 commands)
- Visual Effects (7 commands)
- Testing Scenarios (7 commands)

**Output Files:**
- `output/DebugCommandManager.cs` - Ready-to-use Unity script
- `output/debug_commands.md` - Full documentation
- `output/debug_commands.json` - Command data for processing

**Usage:**
```python
python3 debug_commands.py
```

### 3. Stats Dashboard (`stats_dashboard.py`)
**Purpose:** Real-time gameplay statistics tracking and analysis with performance insights.

**Features:**
- Session tracking with detailed metrics
- Wave statistics and efficiency calculations
- Combat analytics (accuracy, DPS, K/D ratio)
- Economy tracking (income/spend rates, cost per kill)
- Upgrade progression monitoring
- FPS and performance monitoring
- Achievement and recommendation system
- Data export for further analysis

**Metrics Tracked:**
- Session duration and progress
- Wave completion and efficiency
- Combat accuracy and damage
- Economic efficiency
- Upgrade effectiveness
- Performance stability

**Usage:**
```python
python3 stats_dashboard.py
```

## Unity MCP Integration Steps

### 1. Configure PythonTools Asset
1. Open Unity Editor
2. Select the `Assets/PythonTools.asset` file
3. Add the Python tool files to the `pythonFiles` array
4. Enable content hashing for change detection

### 2. Call Tools from Unity
```csharp
// Example: Validate configurations
MCPForUnity.RunPythonTool("config_validator.py", "Assets/Resources/GameConfig.json");

// Example: Generate debug commands
MCPForUnity.RunPythonTool("debug_commands.py");

// Example: Start stats tracking
MCPForUnity.RunPythonTool("stats_dashboard.py", "--start-session");
```

### 3. Process Results in Unity
```csharp
// Handle validation results
void OnValidationComplete(string jsonResult) {
    var result = JsonUtility.FromJson<ValidationResult>(jsonResult);
    if (result.hasErrors) {
        Debug.LogError($"Config validation failed: {result.errorMessage}");
    }
}
```

## Benefits for TopDeck Development

### For Part 3 Requirements:

**Upgradable Defenders & Tower:**
- Config Validator ensures upgrade multipliers are balanced
- Debug commands allow instant testing of all upgrade levels
- Stats Dashboard tracks upgrade effectiveness

**Custom Shaders:**
- Debug commands include shader property cycling
- Config Validator checks shader parameter ranges

**Custom Visual Effects:**
- Debug commands trigger VFX for testing
- Stats Dashboard monitors performance impact

**Procedural Generation:**
- Config Validator ensures variant parameters are within bounds
- Stats Dashboard tracks procedural difficulty effectiveness

**Balancing & Integration:**
- Config Validator detects balance issues early
- Stats Dashboard provides real-time balance feedback
- Debug commands enable rapid iteration testing

## Advanced Usage Examples

### Automated Balance Testing
```python
# Run balance validation
python3 config_validator.py

# If validation passes, start game with debug mode
if [ $? -eq 0 ]; then
    unity -projectPath . -executeMethod GameController.StartDebugMode
fi
```

### Performance Profiling
```python
# Start stats tracking with performance focus
python3 stats_dashboard.py --focus=performance --export-interval=60
```

### Rapid Iteration Testing
```bash
# Generate fresh debug commands after code changes
python3 debug_commands.py
# Copy to Unity project
cp output/DebugCommandManager.cs ../../Assets/Scripts/Debug/
```

## Future Enhancement Ideas

Based on these quick win tools, consider developing:

1. **Wave Balancing Analyzer** - Simulate thousands of scenarios
2. **Upgrade Economy Calculator** - Optimize cost curves
3. **Shader Parameter Optimizer** - Find best visual settings
4. **Procedural Enemy Variant Generator** - Create interesting combinations
5. **VFX Sequence Designer** - Design complex effect chains

## Troubleshooting

### Tools not recognized by Unity MCP
- Ensure Python 3.x is installed
- Check file paths in PythonTools.asset
- Verify MCP server is running

### Validation errors on valid configs
- Check if config format matches expected structure
- Ensure numerical values are within defined ranges
- Review the rules dictionary in config_validator.py

### Debug commands not working
- Add DebugCommandManager.cs to a GameObject in scene
- Ensure all referenced managers exist in scene
- Check Unity console for error messages

## Support

For issues or suggestions regarding these tools:
1. Check the individual tool documentation
2. Review the Unity console for MCP communication logs
3. Ensure all dependencies are properly installed

These tools are designed to accelerate your TopDeck development and help meet Part 3 requirements efficiently!
