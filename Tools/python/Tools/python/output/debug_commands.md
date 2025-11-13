# TopDeck Debug Commands Documentation

## Overview
This document lists all available debug commands for testing TopDeck.

## Table of Contents
- [Upgrades](#upgrades)
- [Waves](#waves)
- [Spawning](#spawning)
- [Economy](#economy)
- [Defenders](#defenders)
- [Performance](#performance)
- [Visual](#visual)
- [Testing](#testing)

## Upgrades

| Command | Description | Hotkey | Parameters |
|---------|-------------|--------|------------|
| Upgrade All Defenders | Upgrades all defenders to the next level | Shift+U | - |
| Max All Upgrades | Sets all defenders and tower to maximum upgrade level | Ctrl+Shift+U | - |
| Reset All Upgrades | Resets all upgrades to level 0 | Ctrl+R | - |
| Upgrade Tower | Upgrades the main tower by one level | T | - |
| Test Upgrade Visuals | Cycles through all upgrade visuals for testing | - | {"duration": 2.0, "loop": true} |
| Apply Random Upgrades | Applies 3 random upgrades to random defenders | - | {"count": 3} |
| Set All Upgrades Level 1 | Sets all defenders to upgrade level 1 | - | {"level": 1} |
| Set All Upgrades Level 2 | Sets all defenders to upgrade level 2 | - | {"level": 2} |
| Set All Upgrades Level 3 | Sets all defenders to upgrade level 3 | - | {"level": 3} |
| Set All Upgrades Level 4 | Sets all defenders to upgrade level 4 | - | {"level": 4} |
| Set All Upgrades Level 5 | Sets all defenders to upgrade level 5 | - | {"level": 5} |

### Code Examples
```csharp
// Upgrades all defenders to the next level
UpgradeManager.UpgradeAllDefenders();

// Sets all defenders and tower to maximum upgrade level
UpgradeManager.MaxAllUpgrades();

// Resets all upgrades to level 0
UpgradeManager.ResetAllUpgrades();

```

## Waves

| Command | Description | Hotkey | Parameters |
|---------|-------------|--------|------------|
| Skip to Wave | Skip to a specific wave number | Ctrl+W | {"wave": 10} |
| Force Next Wave | Immediately starts the next wave | N | - |
| End Current Wave | Immediately ends the current wave | E | - |
| Pause Wave Spawning | Pauses enemy spawning in current wave | P | - |
| Test Boss Wave | Spawns a test boss wave | - | {"boss_count": 1, "minion_count": 20} |
| Test Elite Wave | Spawns a wave with only elite enemies | - | {"elite_count": 10} |
| Jump to Wave 5 | Immediately jump to wave 5 | - | {"wave": 5} |
| Jump to Wave 10 | Immediately jump to wave 10 | - | {"wave": 10} |
| Jump to Wave 20 | Immediately jump to wave 20 | - | {"wave": 20} |
| Jump to Wave 50 | Immediately jump to wave 50 | - | {"wave": 50} |
| Jump to Wave 100 | Immediately jump to wave 100 | - | {"wave": 100} |

### Code Examples
```csharp
// Skip to a specific wave number
GameController.SkipToWave(10);

// Immediately starts the next wave
GameController.ForceNextWave();

// Immediately ends the current wave
GameController.EndCurrentWave();

```

## Spawning

| Command | Description | Hotkey | Parameters |
|---------|-------------|--------|------------|
| Spawn Enemy Type 1 | Spawns a Type 1 enemy at random spawn point | 1 | - |
| Spawn Enemy Type 2 | Spawns a Type 2 enemy at random spawn point | 2 | - |
| Spawn Enemy Type 3 | Spawns a Type 3 enemy at random spawn point | 3 | - |
| Spawn Elite Enemy | Spawns an elite variant enemy | - | {"multiplier": 2.5} |
| Spawn Mini Boss | Spawns a mini-boss enemy | - | {"health_multiplier": 5.0, "reward_multiplier": 3.0} |
| Spawn Enemy Cluster | Spawns a cluster of enemies | - | {"count": 10, "radius": 2.0} |
| Test Spawn Pattern | Tests a specific spawn pattern | - | {"pattern": "alternating"} |

### Code Examples
```csharp
// Spawns a Type 1 enemy at random spawn point
GameController.SpawnEnemy(AttackerType.Type1, SpawnPoint.Random);

// Spawns a Type 2 enemy at random spawn point
GameController.SpawnEnemy(AttackerType.Type2, SpawnPoint.Random);

// Spawns a Type 3 enemy at random spawn point
GameController.SpawnEnemy(AttackerType.Type3, SpawnPoint.Random);

```

## Economy

| Command | Description | Hotkey | Parameters |
|---------|-------------|--------|------------|
| Add Money | Adds 1000 currency to player | M | {"amount": 1000} |
| Set Money | Sets player money to specific amount | - | {"amount": 5000} |
| Infinite Money | Enables infinite money mode | Ctrl+M | {"enabled": true} |
| Test Economy Balance | Runs economy simulation for balance testing | - | - |
| Double Rewards | Doubles all enemy kill rewards | - | {"multiplier": 2.0} |

### Code Examples
```csharp
// Adds 1000 currency to player
GameController.AddMoney(1000);

// Sets player money to specific amount
GameController.SetMoney(5000);

// Enables infinite money mode
GameController.SetInfiniteMoney(true);

```

## Defenders

| Command | Description | Hotkey | Parameters |
|---------|-------------|--------|------------|
| Spawn Basic Defender | Spawns a basic defender at mouse position | B | - |
| Spawn Long Range Defender | Spawns a long-range defender at mouse position | L | - |
| Spawn Short Range Defender | Spawns a short-range defender at mouse position | S | - |
| Heal All Defenders | Fully heals all defenders | H | - |
| Damage All Defenders | Damages all defenders by 50 HP | - | {"damage": 50} |
| God Mode Defenders | Makes all defenders invincible | G | {"enabled": true} |
| Boost Attack Speed | Doubles defender attack speed | - | {"multiplier": 2.0} |

### Code Examples
```csharp
// Spawns a basic defender at mouse position
GameController.SpawnDefender('Basic', MousePosition);

// Spawns a long-range defender at mouse position
GameController.SpawnDefender('Long', MousePosition);

// Spawns a short-range defender at mouse position
GameController.SpawnDefender('Short', MousePosition);

```

## Performance

| Command | Description | Hotkey | Parameters |
|---------|-------------|--------|------------|
| Stress Test Enemies | Spawns 100 enemies for stress testing | - | {"count": 100} |
| Stress Test Projectiles | Spawns 500 projectiles for stress testing | - | {"count": 500} |
| Toggle FPS Display | Shows/hides FPS counter | F | - |
| Profile Frame | Captures profiling data for current frame | Ctrl+P | - |
| Memory Snapshot | Takes a memory snapshot for analysis | - | - |
| Toggle Pooling | Toggles object pooling on/off | - | - |

### Code Examples
```csharp
// Spawns 100 enemies for stress testing
PerformanceTest.SpawnEnemies(100);

// Spawns 500 projectiles for stress testing
PerformanceTest.SpawnProjectiles(500);

// Shows/hides FPS counter
Debug.ToggleFPSDisplay();

```

## Visual

| Command | Description | Hotkey | Parameters |
|---------|-------------|--------|------------|
| Test Upgrade VFX | Plays upgrade particle effect at mouse | V | - |
| Test Hit VFX | Plays hit particle effect at mouse | - | - |
| Test Screen Shake | Tests screen shake effect | - | {"intensity": 1.0, "duration": 0.5} |
| Test Post Process Pulse | Triggers post-processing pulse effect | Ctrl+V | - |
| Cycle Shader Properties | Cycles through shader property variations | - | - |
| Toggle Wireframe Mode | Toggles wireframe rendering mode | W | - |
| Test Color Variants | Shows all enemy color variations | - | - |

### Code Examples
```csharp
// Plays upgrade particle effect at mouse
VFXManager.PlayUpgradeEffect(MousePosition);

// Plays hit particle effect at mouse
VFXManager.PlayHitEffect(MousePosition);

// Tests screen shake effect
CameraController.ScreenShake(1.0, 0.5);

```

## Testing

| Command | Description | Hotkey | Parameters |
|---------|-------------|--------|------------|
| Test Early Game | Sets up early game scenario (waves 1-5) | - | {"waves": "1-5", "money": 500, "upgrades": 0} |
| Test Mid Game | Sets up mid game scenario (waves 10-15) | - | {"waves": "10-15", "money": 2000, "upgrades": 2} |
| Test Late Game | Sets up late game scenario (waves 20+) | - | {"waves": "20+", "money": 5000, "upgrades": 4} |
| Test Boss Battle | Sets up boss battle scenario | - | - |
| Test Upgrade Progression | Tests all upgrade levels in sequence | - | - |
| Test Defense Layout | Places defenders in optimal test configuration | - | - |
| Run All Tests | Runs complete test suite | Ctrl+T | - |

### Code Examples
```csharp
// Sets up early game scenario (waves 1-5)
TestScenario.RunEarlyGame();

// Sets up mid game scenario (waves 10-15)
TestScenario.RunMidGame();

// Sets up late game scenario (waves 20+)
TestScenario.RunLateGame();

```

## Usage Instructions
1. Press ` (backtick) to open the debug menu
2. Click any button to execute the command
3. Use hotkeys for frequently used commands
4. Check the Unity console for command output

## Integration
1. Add the DebugCommandManager script to a GameObject in your scene
2. Ensure all referenced managers and controllers are present
3. Commands will automatically be available in play mode