#!/usr/bin/env python3
"""
Debug Command Generator for TopDeck Unity Game
Generates useful debug commands for testing game features.
Creates commands for upgrades, waves, spawning, and testing scenarios.
"""

import json
import random
from typing import List, Dict, Any, Optional
from dataclasses import dataclass
from enum import Enum


class CommandCategory(Enum):
    """Categories of debug commands"""
    UPGRADES = "Upgrades"
    WAVES = "Waves"
    SPAWNING = "Spawning"
    ECONOMY = "Economy"
    DEFENDERS = "Defenders"
    PERFORMANCE = "Performance"
    VISUAL = "Visual"
    TESTING = "Testing"


@dataclass
class DebugCommand:
    """Represents a debug command"""
    category: CommandCategory
    name: str
    command: str
    description: str
    parameters: Dict[str, Any] = None
    hotkey: str = ""


class DebugCommandGenerator:
    """Generates debug commands for Unity game testing"""
    
    def __init__(self):
        self.commands: List[DebugCommand] = []
        self._generate_all_commands()
    
    def _generate_all_commands(self):
        """Generate all debug commands"""
        self._generate_upgrade_commands()
        self._generate_wave_commands()
        self._generate_spawn_commands()
        self._generate_economy_commands()
        self._generate_defender_commands()
        self._generate_performance_commands()
        self._generate_visual_commands()
        self._generate_test_scenario_commands()
    
    def _generate_upgrade_commands(self):
        """Generate upgrade-related debug commands"""
        # Basic upgrade commands
        self.commands.extend([
            DebugCommand(
                category=CommandCategory.UPGRADES,
                name="Upgrade All Defenders",
                command="UpgradeManager.UpgradeAllDefenders()",
                description="Upgrades all defenders to the next level",
                hotkey="Shift+U"
            ),
            DebugCommand(
                category=CommandCategory.UPGRADES,
                name="Max All Upgrades",
                command="UpgradeManager.MaxAllUpgrades()",
                description="Sets all defenders and tower to maximum upgrade level",
                hotkey="Ctrl+Shift+U"
            ),
            DebugCommand(
                category=CommandCategory.UPGRADES,
                name="Reset All Upgrades",
                command="UpgradeManager.ResetAllUpgrades()",
                description="Resets all upgrades to level 0",
                hotkey="Ctrl+R"
            ),
            DebugCommand(
                category=CommandCategory.UPGRADES,
                name="Upgrade Tower",
                command="UpgradeManager.UpgradeTower()",
                description="Upgrades the main tower by one level",
                hotkey="T"
            ),
            DebugCommand(
                category=CommandCategory.UPGRADES,
                name="Test Upgrade Visuals",
                command="UpgradeManager.TestVisualProgression()",
                description="Cycles through all upgrade visuals for testing",
                parameters={"duration": 2.0, "loop": True}
            ),
            DebugCommand(
                category=CommandCategory.UPGRADES,
                name="Apply Random Upgrades",
                command="UpgradeManager.ApplyRandomUpgrades(3)",
                description="Applies 3 random upgrades to random defenders",
                parameters={"count": 3}
            )
        ])
        
        # Generate upgrade commands for specific levels
        for level in range(1, 6):
            self.commands.append(DebugCommand(
                category=CommandCategory.UPGRADES,
                name=f"Set All Upgrades Level {level}",
                command=f"UpgradeManager.SetAllUpgradeLevel({level})",
                description=f"Sets all defenders to upgrade level {level}",
                parameters={"level": level}
            ))
    
    def _generate_wave_commands(self):
        """Generate wave-related debug commands"""
        self.commands.extend([
            DebugCommand(
                category=CommandCategory.WAVES,
                name="Skip to Wave",
                command="GameController.SkipToWave(10)",
                description="Skip to a specific wave number",
                parameters={"wave": 10},
                hotkey="Ctrl+W"
            ),
            DebugCommand(
                category=CommandCategory.WAVES,
                name="Force Next Wave",
                command="GameController.ForceNextWave()",
                description="Immediately starts the next wave",
                hotkey="N"
            ),
            DebugCommand(
                category=CommandCategory.WAVES,
                name="End Current Wave",
                command="GameController.EndCurrentWave()",
                description="Immediately ends the current wave",
                hotkey="E"
            ),
            DebugCommand(
                category=CommandCategory.WAVES,
                name="Pause Wave Spawning",
                command="GameController.PauseWaveSpawning()",
                description="Pauses enemy spawning in current wave",
                hotkey="P"
            ),
            DebugCommand(
                category=CommandCategory.WAVES,
                name="Test Boss Wave",
                command="GameController.SpawnBossWave()",
                description="Spawns a test boss wave",
                parameters={"boss_count": 1, "minion_count": 20}
            ),
            DebugCommand(
                category=CommandCategory.WAVES,
                name="Test Elite Wave",
                command="GameController.SpawnEliteWave()",
                description="Spawns a wave with only elite enemies",
                parameters={"elite_count": 10}
            )
        ])
        
        # Generate wave skip commands
        for wave in [5, 10, 20, 50, 100]:
            self.commands.append(DebugCommand(
                category=CommandCategory.WAVES,
                name=f"Jump to Wave {wave}",
                command=f"GameController.SkipToWave({wave})",
                description=f"Immediately jump to wave {wave}",
                parameters={"wave": wave}
            ))
    
    def _generate_spawn_commands(self):
        """Generate enemy spawning debug commands"""
        self.commands.extend([
            DebugCommand(
                category=CommandCategory.SPAWNING,
                name="Spawn Enemy Type 1",
                command="GameController.SpawnEnemy(AttackerType.Type1, SpawnPoint.Random)",
                description="Spawns a Type 1 enemy at random spawn point",
                hotkey="1"
            ),
            DebugCommand(
                category=CommandCategory.SPAWNING,
                name="Spawn Enemy Type 2",
                command="GameController.SpawnEnemy(AttackerType.Type2, SpawnPoint.Random)",
                description="Spawns a Type 2 enemy at random spawn point",
                hotkey="2"
            ),
            DebugCommand(
                category=CommandCategory.SPAWNING,
                name="Spawn Enemy Type 3",
                command="GameController.SpawnEnemy(AttackerType.Type3, SpawnPoint.Random)",
                description="Spawns a Type 3 enemy at random spawn point",
                hotkey="3"
            ),
            DebugCommand(
                category=CommandCategory.SPAWNING,
                name="Spawn Elite Enemy",
                command="GameController.SpawnEliteEnemy()",
                description="Spawns an elite variant enemy",
                parameters={"multiplier": 2.5}
            ),
            DebugCommand(
                category=CommandCategory.SPAWNING,
                name="Spawn Mini Boss",
                command="GameController.SpawnMiniBoss()",
                description="Spawns a mini-boss enemy",
                parameters={"health_multiplier": 5.0, "reward_multiplier": 3.0}
            ),
            DebugCommand(
                category=CommandCategory.SPAWNING,
                name="Spawn Enemy Cluster",
                command="GameController.SpawnEnemyCluster(10)",
                description="Spawns a cluster of enemies",
                parameters={"count": 10, "radius": 2.0}
            ),
            DebugCommand(
                category=CommandCategory.SPAWNING,
                name="Test Spawn Pattern",
                command="SpawnPatternPlanner.TestPattern('alternating')",
                description="Tests a specific spawn pattern",
                parameters={"pattern": "alternating"}
            )
        ])
    
    def _generate_economy_commands(self):
        """Generate economy-related debug commands"""
        self.commands.extend([
            DebugCommand(
                category=CommandCategory.ECONOMY,
                name="Add Money",
                command="GameController.AddMoney(1000)",
                description="Adds 1000 currency to player",
                hotkey="M",
                parameters={"amount": 1000}
            ),
            DebugCommand(
                category=CommandCategory.ECONOMY,
                name="Set Money",
                command="GameController.SetMoney(5000)",
                description="Sets player money to specific amount",
                parameters={"amount": 5000}
            ),
            DebugCommand(
                category=CommandCategory.ECONOMY,
                name="Infinite Money",
                command="GameController.SetInfiniteMoney(true)",
                description="Enables infinite money mode",
                hotkey="Ctrl+M",
                parameters={"enabled": True}
            ),
            DebugCommand(
                category=CommandCategory.ECONOMY,
                name="Test Economy Balance",
                command="GameController.TestEconomyBalance()",
                description="Runs economy simulation for balance testing"
            ),
            DebugCommand(
                category=CommandCategory.ECONOMY,
                name="Double Rewards",
                command="GameController.SetRewardMultiplier(2.0)",
                description="Doubles all enemy kill rewards",
                parameters={"multiplier": 2.0}
            )
        ])
    
    def _generate_defender_commands(self):
        """Generate defender-related debug commands"""
        self.commands.extend([
            DebugCommand(
                category=CommandCategory.DEFENDERS,
                name="Spawn Basic Defender",
                command="GameController.SpawnDefender('Basic', MousePosition)",
                description="Spawns a basic defender at mouse position",
                hotkey="B"
            ),
            DebugCommand(
                category=CommandCategory.DEFENDERS,
                name="Spawn Long Range Defender",
                command="GameController.SpawnDefender('Long', MousePosition)",
                description="Spawns a long-range defender at mouse position",
                hotkey="L"
            ),
            DebugCommand(
                category=CommandCategory.DEFENDERS,
                name="Spawn Short Range Defender",
                command="GameController.SpawnDefender('Short', MousePosition)",
                description="Spawns a short-range defender at mouse position",
                hotkey="S"
            ),
            DebugCommand(
                category=CommandCategory.DEFENDERS,
                name="Heal All Defenders",
                command="GameController.HealAllDefenders()",
                description="Fully heals all defenders",
                hotkey="H"
            ),
            DebugCommand(
                category=CommandCategory.DEFENDERS,
                name="Damage All Defenders",
                command="GameController.DamageAllDefenders(50)",
                description="Damages all defenders by 50 HP",
                parameters={"damage": 50}
            ),
            DebugCommand(
                category=CommandCategory.DEFENDERS,
                name="God Mode Defenders",
                command="GameController.SetDefenderGodMode(true)",
                description="Makes all defenders invincible",
                hotkey="G",
                parameters={"enabled": True}
            ),
            DebugCommand(
                category=CommandCategory.DEFENDERS,
                name="Boost Attack Speed",
                command="GameController.SetDefenderAttackSpeedMultiplier(2.0)",
                description="Doubles defender attack speed",
                parameters={"multiplier": 2.0}
            )
        ])
    
    def _generate_performance_commands(self):
        """Generate performance testing commands"""
        self.commands.extend([
            DebugCommand(
                category=CommandCategory.PERFORMANCE,
                name="Stress Test Enemies",
                command="PerformanceTest.SpawnEnemies(100)",
                description="Spawns 100 enemies for stress testing",
                parameters={"count": 100}
            ),
            DebugCommand(
                category=CommandCategory.PERFORMANCE,
                name="Stress Test Projectiles",
                command="PerformanceTest.SpawnProjectiles(500)",
                description="Spawns 500 projectiles for stress testing",
                parameters={"count": 500}
            ),
            DebugCommand(
                category=CommandCategory.PERFORMANCE,
                name="Toggle FPS Display",
                command="Debug.ToggleFPSDisplay()",
                description="Shows/hides FPS counter",
                hotkey="F"
            ),
            DebugCommand(
                category=CommandCategory.PERFORMANCE,
                name="Profile Frame",
                command="Debug.ProfileCurrentFrame()",
                description="Captures profiling data for current frame",
                hotkey="Ctrl+P"
            ),
            DebugCommand(
                category=CommandCategory.PERFORMANCE,
                name="Memory Snapshot",
                command="Debug.TakeMemorySnapshot()",
                description="Takes a memory snapshot for analysis"
            ),
            DebugCommand(
                category=CommandCategory.PERFORMANCE,
                name="Toggle Pooling",
                command="GameController.SetObjectPooling(!enabled)",
                description="Toggles object pooling on/off"
            )
        ])
    
    def _generate_visual_commands(self):
        """Generate visual effect testing commands"""
        self.commands.extend([
            DebugCommand(
                category=CommandCategory.VISUAL,
                name="Test Upgrade VFX",
                command="VFXManager.PlayUpgradeEffect(MousePosition)",
                description="Plays upgrade particle effect at mouse",
                hotkey="V"
            ),
            DebugCommand(
                category=CommandCategory.VISUAL,
                name="Test Hit VFX",
                command="VFXManager.PlayHitEffect(MousePosition)",
                description="Plays hit particle effect at mouse"
            ),
            DebugCommand(
                category=CommandCategory.VISUAL,
                name="Test Screen Shake",
                command="CameraController.ScreenShake(1.0, 0.5)",
                description="Tests screen shake effect",
                parameters={"intensity": 1.0, "duration": 0.5}
            ),
            DebugCommand(
                category=CommandCategory.VISUAL,
                name="Test Post Process Pulse",
                command="PostProcessController.TriggerPulse()",
                description="Triggers post-processing pulse effect",
                hotkey="Ctrl+V"
            ),
            DebugCommand(
                category=CommandCategory.VISUAL,
                name="Cycle Shader Properties",
                command="ShaderController.CycleProperties()",
                description="Cycles through shader property variations"
            ),
            DebugCommand(
                category=CommandCategory.VISUAL,
                name="Toggle Wireframe Mode",
                command="RenderSettings.SetWireframe(!enabled)",
                description="Toggles wireframe rendering mode",
                hotkey="W"
            ),
            DebugCommand(
                category=CommandCategory.VISUAL,
                name="Test Color Variants",
                command="VariantAppearance.TestColorVariations()",
                description="Shows all enemy color variations"
            )
        ])
    
    def _generate_test_scenario_commands(self):
        """Generate complete test scenario commands"""
        self.commands.extend([
            DebugCommand(
                category=CommandCategory.TESTING,
                name="Test Early Game",
                command="TestScenario.RunEarlyGame()",
                description="Sets up early game scenario (waves 1-5)",
                parameters={"waves": "1-5", "money": 500, "upgrades": 0}
            ),
            DebugCommand(
                category=CommandCategory.TESTING,
                name="Test Mid Game",
                command="TestScenario.RunMidGame()",
                description="Sets up mid game scenario (waves 10-15)",
                parameters={"waves": "10-15", "money": 2000, "upgrades": 2}
            ),
            DebugCommand(
                category=CommandCategory.TESTING,
                name="Test Late Game",
                command="TestScenario.RunLateGame()",
                description="Sets up late game scenario (waves 20+)",
                parameters={"waves": "20+", "money": 5000, "upgrades": 4}
            ),
            DebugCommand(
                category=CommandCategory.TESTING,
                name="Test Boss Battle",
                command="TestScenario.RunBossBattle()",
                description="Sets up boss battle scenario"
            ),
            DebugCommand(
                category=CommandCategory.TESTING,
                name="Test Upgrade Progression",
                command="TestScenario.TestUpgradeProgression()",
                description="Tests all upgrade levels in sequence"
            ),
            DebugCommand(
                category=CommandCategory.TESTING,
                name="Test Defense Layout",
                command="TestScenario.TestOptimalLayout()",
                description="Places defenders in optimal test configuration"
            ),
            DebugCommand(
                category=CommandCategory.TESTING,
                name="Run All Tests",
                command="TestScenario.RunAllTests()",
                description="Runs complete test suite",
                hotkey="Ctrl+T"
            )
        ])
    
    def get_commands_by_category(self, category: CommandCategory) -> List[DebugCommand]:
        """Get all commands in a specific category"""
        return [cmd for cmd in self.commands if cmd.category == category]
    
    def generate_unity_script(self) -> str:
        """Generate a Unity C# script with all debug commands"""
        script = """using UnityEngine;
using System.Collections.Generic;

public class DebugCommandManager : MonoBehaviour
{
    private bool showDebugMenu = false;
    private Vector2 scrollPosition;
    private Dictionary<string, System.Action> commands;
    
    void Start()
    {
        InitializeCommands();
    }
    
    void InitializeCommands()
    {
        commands = new Dictionary<string, System.Action>();
"""
        
        # Add command registrations
        for cmd in self.commands:
            safe_name = cmd.name.replace(" ", "")
            script += f'        commands["{cmd.name}"] = () => {{ {cmd.command}; }};\n'
        
        script += """    }
    
    void Update()
    {
        // Toggle debug menu
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            showDebugMenu = !showDebugMenu;
        }
        
        // Handle hotkeys
"""
        
        # Add hotkey handlers
        hotkey_commands = [cmd for cmd in self.commands if cmd.hotkey]
        for cmd in hotkey_commands:
            key_parts = cmd.hotkey.split('+')
            conditions = []
            
            for part in key_parts:
                if part == "Ctrl":
                    conditions.append("Input.GetKey(KeyCode.LeftControl)")
                elif part == "Shift":
                    conditions.append("Input.GetKey(KeyCode.LeftShift)")
                else:
                    conditions.append(f"Input.GetKeyDown(KeyCode.{part})")
            
            condition_str = " && ".join(conditions)
            script += f'        if ({condition_str})\n'
            script += f'        {{\n'
            script += f'            commands["{cmd.name}"]?.Invoke();\n'
            script += f'            Debug.Log("Executed: {cmd.name}");\n'
            script += f'        }}\n'
        
        script += """    }
    
    void OnGUI()
    {
        if (!showDebugMenu) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 400, 600));
        GUILayout.Box("Debug Commands");
        
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
"""
        
        # Group commands by category
        for category in CommandCategory:
            category_commands = self.get_commands_by_category(category)
            if category_commands:
                script += f'        GUILayout.Label("{category.value}", GUI.skin.box);\n'
                for cmd in category_commands:
                    hotkey_text = f" [{cmd.hotkey}]" if cmd.hotkey else ""
                    script += f'        if (GUILayout.Button("{cmd.name}{hotkey_text}"))\n'
                    script += f'        {{\n'
                    script += f'            commands["{cmd.name}"]?.Invoke();\n'
                    script += f'            Debug.Log("Executed: {cmd.name}");\n'
                    script += f'        }}\n'
                script += '        GUILayout.Space(10);\n'
        
        script += """        
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }
}"""
        
        return script
    
    def generate_documentation(self) -> str:
        """Generate documentation for all debug commands"""
        doc = ["# TopDeck Debug Commands Documentation\n"]
        doc.append("## Overview")
        doc.append("This document lists all available debug commands for testing TopDeck.\n")
        
        # Table of contents
        doc.append("## Table of Contents")
        for category in CommandCategory:
            doc.append(f"- [{category.value}](#{category.value.lower().replace(' ', '-')})")
        doc.append("")
        
        # Commands by category
        for category in CommandCategory:
            commands = self.get_commands_by_category(category)
            if commands:
                doc.append(f"## {category.value}")
                doc.append("")
                
                # Create table
                doc.append("| Command | Description | Hotkey | Parameters |")
                doc.append("|---------|-------------|--------|------------|")
                
                for cmd in commands:
                    hotkey = cmd.hotkey if cmd.hotkey else "-"
                    params = json.dumps(cmd.parameters) if cmd.parameters else "-"
                    doc.append(f"| {cmd.name} | {cmd.description} | {hotkey} | {params} |")
                
                doc.append("")
                
                # Add code examples
                doc.append("### Code Examples")
                doc.append("```csharp")
                for cmd in commands[:3]:  # Show first 3 as examples
                    doc.append(f"// {cmd.description}")
                    doc.append(f"{cmd.command};")
                    doc.append("")
                doc.append("```")
                doc.append("")
        
        # Usage instructions
        doc.append("## Usage Instructions")
        doc.append("1. Press ` (backtick) to open the debug menu")
        doc.append("2. Click any button to execute the command")
        doc.append("3. Use hotkeys for frequently used commands")
        doc.append("4. Check the Unity console for command output")
        doc.append("")
        
        doc.append("## Integration")
        doc.append("1. Add the DebugCommandManager script to a GameObject in your scene")
        doc.append("2. Ensure all referenced managers and controllers are present")
        doc.append("3. Commands will automatically be available in play mode")
        
        return "\n".join(doc)
    
    def export_to_json(self) -> str:
        """Export all commands to JSON format"""
        commands_data = []
        
        for cmd in self.commands:
            commands_data.append({
                "category": cmd.category.value,
                "name": cmd.name,
                "command": cmd.command,
                "description": cmd.description,
                "parameters": cmd.parameters or {},
                "hotkey": cmd.hotkey
            })
        
        return json.dumps(commands_data, indent=2)


def main():
    """Main entry point"""
    generator = DebugCommandGenerator()
    
    print("TopDeck Debug Command Generator")
    print("=" * 50)
    print(f"Generated {len(generator.commands)} debug commands")
    print("")
    
    # Show command summary
    for category in CommandCategory:
        commands = generator.get_commands_by_category(category)
        print(f"{category.value}: {len(commands)} commands")
    
    print("\n" + "=" * 50)
    print("Export Options:")
    print("1. Generate Unity C# Script")
    print("2. Generate Documentation")
    print("3. Export to JSON")
    print("4. Show All Commands")
    
    # For automated use, generate all outputs
    print("\nGenerating all outputs...")
    
    # Save Unity script
    with open("Tools/python/output/DebugCommandManager.cs", "w") as f:
        f.write(generator.generate_unity_script())
    print("✓ Unity script saved to output/DebugCommandManager.cs")
    
    # Save documentation
    with open("Tools/python/output/debug_commands.md", "w") as f:
        f.write(generator.generate_documentation())
    print("✓ Documentation saved to output/debug_commands.md")
    
    # Save JSON
    with open("Tools/python/output/debug_commands.json", "w") as f:
        f.write(generator.export_to_json())
    print("✓ JSON data saved to output/debug_commands.json")
    
    print("\nDebug command generation complete!")
    return 0


if __name__ == "__main__":
    import os
    # Create output directory if it doesn't exist
    os.makedirs("Tools/python/output", exist_ok=True)
    main()
