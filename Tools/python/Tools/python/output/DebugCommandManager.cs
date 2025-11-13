using UnityEngine;
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
        commands["Upgrade All Defenders"] = () => { UpgradeManager.UpgradeAllDefenders(); };
        commands["Max All Upgrades"] = () => { UpgradeManager.MaxAllUpgrades(); };
        commands["Reset All Upgrades"] = () => { UpgradeManager.ResetAllUpgrades(); };
        commands["Upgrade Tower"] = () => { UpgradeManager.UpgradeTower(); };
        commands["Test Upgrade Visuals"] = () => { UpgradeManager.TestVisualProgression(); };
        commands["Apply Random Upgrades"] = () => { UpgradeManager.ApplyRandomUpgrades(3); };
        commands["Set All Upgrades Level 1"] = () => { UpgradeManager.SetAllUpgradeLevel(1); };
        commands["Set All Upgrades Level 2"] = () => { UpgradeManager.SetAllUpgradeLevel(2); };
        commands["Set All Upgrades Level 3"] = () => { UpgradeManager.SetAllUpgradeLevel(3); };
        commands["Set All Upgrades Level 4"] = () => { UpgradeManager.SetAllUpgradeLevel(4); };
        commands["Set All Upgrades Level 5"] = () => { UpgradeManager.SetAllUpgradeLevel(5); };
        commands["Skip to Wave"] = () => { GameController.SkipToWave(10); };
        commands["Force Next Wave"] = () => { GameController.ForceNextWave(); };
        commands["End Current Wave"] = () => { GameController.EndCurrentWave(); };
        commands["Pause Wave Spawning"] = () => { GameController.PauseWaveSpawning(); };
        commands["Test Boss Wave"] = () => { GameController.SpawnBossWave(); };
        commands["Test Elite Wave"] = () => { GameController.SpawnEliteWave(); };
        commands["Jump to Wave 5"] = () => { GameController.SkipToWave(5); };
        commands["Jump to Wave 10"] = () => { GameController.SkipToWave(10); };
        commands["Jump to Wave 20"] = () => { GameController.SkipToWave(20); };
        commands["Jump to Wave 50"] = () => { GameController.SkipToWave(50); };
        commands["Jump to Wave 100"] = () => { GameController.SkipToWave(100); };
        commands["Spawn Enemy Type 1"] = () => { GameController.SpawnEnemy(AttackerType.Type1, SpawnPoint.Random); };
        commands["Spawn Enemy Type 2"] = () => { GameController.SpawnEnemy(AttackerType.Type2, SpawnPoint.Random); };
        commands["Spawn Enemy Type 3"] = () => { GameController.SpawnEnemy(AttackerType.Type3, SpawnPoint.Random); };
        commands["Spawn Elite Enemy"] = () => { GameController.SpawnEliteEnemy(); };
        commands["Spawn Mini Boss"] = () => { GameController.SpawnMiniBoss(); };
        commands["Spawn Enemy Cluster"] = () => { GameController.SpawnEnemyCluster(10); };
        commands["Test Spawn Pattern"] = () => { SpawnPatternPlanner.TestPattern('alternating'); };
        commands["Add Money"] = () => { GameController.AddMoney(1000); };
        commands["Set Money"] = () => { GameController.SetMoney(5000); };
        commands["Infinite Money"] = () => { GameController.SetInfiniteMoney(true); };
        commands["Test Economy Balance"] = () => { GameController.TestEconomyBalance(); };
        commands["Double Rewards"] = () => { GameController.SetRewardMultiplier(2.0); };
        commands["Spawn Basic Defender"] = () => { GameController.SpawnDefender('Basic', MousePosition); };
        commands["Spawn Long Range Defender"] = () => { GameController.SpawnDefender('Long', MousePosition); };
        commands["Spawn Short Range Defender"] = () => { GameController.SpawnDefender('Short', MousePosition); };
        commands["Heal All Defenders"] = () => { GameController.HealAllDefenders(); };
        commands["Damage All Defenders"] = () => { GameController.DamageAllDefenders(50); };
        commands["God Mode Defenders"] = () => { GameController.SetDefenderGodMode(true); };
        commands["Boost Attack Speed"] = () => { GameController.SetDefenderAttackSpeedMultiplier(2.0); };
        commands["Stress Test Enemies"] = () => { PerformanceTest.SpawnEnemies(100); };
        commands["Stress Test Projectiles"] = () => { PerformanceTest.SpawnProjectiles(500); };
        commands["Toggle FPS Display"] = () => { Debug.ToggleFPSDisplay(); };
        commands["Profile Frame"] = () => { Debug.ProfileCurrentFrame(); };
        commands["Memory Snapshot"] = () => { Debug.TakeMemorySnapshot(); };
        commands["Toggle Pooling"] = () => { GameController.SetObjectPooling(!enabled); };
        commands["Test Upgrade VFX"] = () => { VFXManager.PlayUpgradeEffect(MousePosition); };
        commands["Test Hit VFX"] = () => { VFXManager.PlayHitEffect(MousePosition); };
        commands["Test Screen Shake"] = () => { CameraController.ScreenShake(1.0, 0.5); };
        commands["Test Post Process Pulse"] = () => { PostProcessController.TriggerPulse(); };
        commands["Cycle Shader Properties"] = () => { ShaderController.CycleProperties(); };
        commands["Toggle Wireframe Mode"] = () => { RenderSettings.SetWireframe(!enabled); };
        commands["Test Color Variants"] = () => { VariantAppearance.TestColorVariations(); };
        commands["Test Early Game"] = () => { TestScenario.RunEarlyGame(); };
        commands["Test Mid Game"] = () => { TestScenario.RunMidGame(); };
        commands["Test Late Game"] = () => { TestScenario.RunLateGame(); };
        commands["Test Boss Battle"] = () => { TestScenario.RunBossBattle(); };
        commands["Test Upgrade Progression"] = () => { TestScenario.TestUpgradeProgression(); };
        commands["Test Defense Layout"] = () => { TestScenario.TestOptimalLayout(); };
        commands["Run All Tests"] = () => { TestScenario.RunAllTests(); };
    }
    
    void Update()
    {
        // Toggle debug menu
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            showDebugMenu = !showDebugMenu;
        }
        
        // Handle hotkeys
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.U))
        {
            commands["Upgrade All Defenders"]?.Invoke();
            Debug.Log("Executed: Upgrade All Defenders");
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.U))
        {
            commands["Max All Upgrades"]?.Invoke();
            Debug.Log("Executed: Max All Upgrades");
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
        {
            commands["Reset All Upgrades"]?.Invoke();
            Debug.Log("Executed: Reset All Upgrades");
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            commands["Upgrade Tower"]?.Invoke();
            Debug.Log("Executed: Upgrade Tower");
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.W))
        {
            commands["Skip to Wave"]?.Invoke();
            Debug.Log("Executed: Skip to Wave");
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            commands["Force Next Wave"]?.Invoke();
            Debug.Log("Executed: Force Next Wave");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            commands["End Current Wave"]?.Invoke();
            Debug.Log("Executed: End Current Wave");
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            commands["Pause Wave Spawning"]?.Invoke();
            Debug.Log("Executed: Pause Wave Spawning");
        }
        if (Input.GetKeyDown(KeyCode.1))
        {
            commands["Spawn Enemy Type 1"]?.Invoke();
            Debug.Log("Executed: Spawn Enemy Type 1");
        }
        if (Input.GetKeyDown(KeyCode.2))
        {
            commands["Spawn Enemy Type 2"]?.Invoke();
            Debug.Log("Executed: Spawn Enemy Type 2");
        }
        if (Input.GetKeyDown(KeyCode.3))
        {
            commands["Spawn Enemy Type 3"]?.Invoke();
            Debug.Log("Executed: Spawn Enemy Type 3");
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            commands["Add Money"]?.Invoke();
            Debug.Log("Executed: Add Money");
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.M))
        {
            commands["Infinite Money"]?.Invoke();
            Debug.Log("Executed: Infinite Money");
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            commands["Spawn Basic Defender"]?.Invoke();
            Debug.Log("Executed: Spawn Basic Defender");
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            commands["Spawn Long Range Defender"]?.Invoke();
            Debug.Log("Executed: Spawn Long Range Defender");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            commands["Spawn Short Range Defender"]?.Invoke();
            Debug.Log("Executed: Spawn Short Range Defender");
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            commands["Heal All Defenders"]?.Invoke();
            Debug.Log("Executed: Heal All Defenders");
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            commands["God Mode Defenders"]?.Invoke();
            Debug.Log("Executed: God Mode Defenders");
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            commands["Toggle FPS Display"]?.Invoke();
            Debug.Log("Executed: Toggle FPS Display");
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.P))
        {
            commands["Profile Frame"]?.Invoke();
            Debug.Log("Executed: Profile Frame");
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            commands["Test Upgrade VFX"]?.Invoke();
            Debug.Log("Executed: Test Upgrade VFX");
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V))
        {
            commands["Test Post Process Pulse"]?.Invoke();
            Debug.Log("Executed: Test Post Process Pulse");
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            commands["Toggle Wireframe Mode"]?.Invoke();
            Debug.Log("Executed: Toggle Wireframe Mode");
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.T))
        {
            commands["Run All Tests"]?.Invoke();
            Debug.Log("Executed: Run All Tests");
        }
    }
    
    void OnGUI()
    {
        if (!showDebugMenu) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 400, 600));
        GUILayout.Box("Debug Commands");
        
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        GUILayout.Label("Upgrades", GUI.skin.box);
        if (GUILayout.Button("Upgrade All Defenders [Shift+U]"))
        {
            commands["Upgrade All Defenders"]?.Invoke();
            Debug.Log("Executed: Upgrade All Defenders");
        }
        if (GUILayout.Button("Max All Upgrades [Ctrl+Shift+U]"))
        {
            commands["Max All Upgrades"]?.Invoke();
            Debug.Log("Executed: Max All Upgrades");
        }
        if (GUILayout.Button("Reset All Upgrades [Ctrl+R]"))
        {
            commands["Reset All Upgrades"]?.Invoke();
            Debug.Log("Executed: Reset All Upgrades");
        }
        if (GUILayout.Button("Upgrade Tower [T]"))
        {
            commands["Upgrade Tower"]?.Invoke();
            Debug.Log("Executed: Upgrade Tower");
        }
        if (GUILayout.Button("Test Upgrade Visuals"))
        {
            commands["Test Upgrade Visuals"]?.Invoke();
            Debug.Log("Executed: Test Upgrade Visuals");
        }
        if (GUILayout.Button("Apply Random Upgrades"))
        {
            commands["Apply Random Upgrades"]?.Invoke();
            Debug.Log("Executed: Apply Random Upgrades");
        }
        if (GUILayout.Button("Set All Upgrades Level 1"))
        {
            commands["Set All Upgrades Level 1"]?.Invoke();
            Debug.Log("Executed: Set All Upgrades Level 1");
        }
        if (GUILayout.Button("Set All Upgrades Level 2"))
        {
            commands["Set All Upgrades Level 2"]?.Invoke();
            Debug.Log("Executed: Set All Upgrades Level 2");
        }
        if (GUILayout.Button("Set All Upgrades Level 3"))
        {
            commands["Set All Upgrades Level 3"]?.Invoke();
            Debug.Log("Executed: Set All Upgrades Level 3");
        }
        if (GUILayout.Button("Set All Upgrades Level 4"))
        {
            commands["Set All Upgrades Level 4"]?.Invoke();
            Debug.Log("Executed: Set All Upgrades Level 4");
        }
        if (GUILayout.Button("Set All Upgrades Level 5"))
        {
            commands["Set All Upgrades Level 5"]?.Invoke();
            Debug.Log("Executed: Set All Upgrades Level 5");
        }
        GUILayout.Space(10);
        GUILayout.Label("Waves", GUI.skin.box);
        if (GUILayout.Button("Skip to Wave [Ctrl+W]"))
        {
            commands["Skip to Wave"]?.Invoke();
            Debug.Log("Executed: Skip to Wave");
        }
        if (GUILayout.Button("Force Next Wave [N]"))
        {
            commands["Force Next Wave"]?.Invoke();
            Debug.Log("Executed: Force Next Wave");
        }
        if (GUILayout.Button("End Current Wave [E]"))
        {
            commands["End Current Wave"]?.Invoke();
            Debug.Log("Executed: End Current Wave");
        }
        if (GUILayout.Button("Pause Wave Spawning [P]"))
        {
            commands["Pause Wave Spawning"]?.Invoke();
            Debug.Log("Executed: Pause Wave Spawning");
        }
        if (GUILayout.Button("Test Boss Wave"))
        {
            commands["Test Boss Wave"]?.Invoke();
            Debug.Log("Executed: Test Boss Wave");
        }
        if (GUILayout.Button("Test Elite Wave"))
        {
            commands["Test Elite Wave"]?.Invoke();
            Debug.Log("Executed: Test Elite Wave");
        }
        if (GUILayout.Button("Jump to Wave 5"))
        {
            commands["Jump to Wave 5"]?.Invoke();
            Debug.Log("Executed: Jump to Wave 5");
        }
        if (GUILayout.Button("Jump to Wave 10"))
        {
            commands["Jump to Wave 10"]?.Invoke();
            Debug.Log("Executed: Jump to Wave 10");
        }
        if (GUILayout.Button("Jump to Wave 20"))
        {
            commands["Jump to Wave 20"]?.Invoke();
            Debug.Log("Executed: Jump to Wave 20");
        }
        if (GUILayout.Button("Jump to Wave 50"))
        {
            commands["Jump to Wave 50"]?.Invoke();
            Debug.Log("Executed: Jump to Wave 50");
        }
        if (GUILayout.Button("Jump to Wave 100"))
        {
            commands["Jump to Wave 100"]?.Invoke();
            Debug.Log("Executed: Jump to Wave 100");
        }
        GUILayout.Space(10);
        GUILayout.Label("Spawning", GUI.skin.box);
        if (GUILayout.Button("Spawn Enemy Type 1 [1]"))
        {
            commands["Spawn Enemy Type 1"]?.Invoke();
            Debug.Log("Executed: Spawn Enemy Type 1");
        }
        if (GUILayout.Button("Spawn Enemy Type 2 [2]"))
        {
            commands["Spawn Enemy Type 2"]?.Invoke();
            Debug.Log("Executed: Spawn Enemy Type 2");
        }
        if (GUILayout.Button("Spawn Enemy Type 3 [3]"))
        {
            commands["Spawn Enemy Type 3"]?.Invoke();
            Debug.Log("Executed: Spawn Enemy Type 3");
        }
        if (GUILayout.Button("Spawn Elite Enemy"))
        {
            commands["Spawn Elite Enemy"]?.Invoke();
            Debug.Log("Executed: Spawn Elite Enemy");
        }
        if (GUILayout.Button("Spawn Mini Boss"))
        {
            commands["Spawn Mini Boss"]?.Invoke();
            Debug.Log("Executed: Spawn Mini Boss");
        }
        if (GUILayout.Button("Spawn Enemy Cluster"))
        {
            commands["Spawn Enemy Cluster"]?.Invoke();
            Debug.Log("Executed: Spawn Enemy Cluster");
        }
        if (GUILayout.Button("Test Spawn Pattern"))
        {
            commands["Test Spawn Pattern"]?.Invoke();
            Debug.Log("Executed: Test Spawn Pattern");
        }
        GUILayout.Space(10);
        GUILayout.Label("Economy", GUI.skin.box);
        if (GUILayout.Button("Add Money [M]"))
        {
            commands["Add Money"]?.Invoke();
            Debug.Log("Executed: Add Money");
        }
        if (GUILayout.Button("Set Money"))
        {
            commands["Set Money"]?.Invoke();
            Debug.Log("Executed: Set Money");
        }
        if (GUILayout.Button("Infinite Money [Ctrl+M]"))
        {
            commands["Infinite Money"]?.Invoke();
            Debug.Log("Executed: Infinite Money");
        }
        if (GUILayout.Button("Test Economy Balance"))
        {
            commands["Test Economy Balance"]?.Invoke();
            Debug.Log("Executed: Test Economy Balance");
        }
        if (GUILayout.Button("Double Rewards"))
        {
            commands["Double Rewards"]?.Invoke();
            Debug.Log("Executed: Double Rewards");
        }
        GUILayout.Space(10);
        GUILayout.Label("Defenders", GUI.skin.box);
        if (GUILayout.Button("Spawn Basic Defender [B]"))
        {
            commands["Spawn Basic Defender"]?.Invoke();
            Debug.Log("Executed: Spawn Basic Defender");
        }
        if (GUILayout.Button("Spawn Long Range Defender [L]"))
        {
            commands["Spawn Long Range Defender"]?.Invoke();
            Debug.Log("Executed: Spawn Long Range Defender");
        }
        if (GUILayout.Button("Spawn Short Range Defender [S]"))
        {
            commands["Spawn Short Range Defender"]?.Invoke();
            Debug.Log("Executed: Spawn Short Range Defender");
        }
        if (GUILayout.Button("Heal All Defenders [H]"))
        {
            commands["Heal All Defenders"]?.Invoke();
            Debug.Log("Executed: Heal All Defenders");
        }
        if (GUILayout.Button("Damage All Defenders"))
        {
            commands["Damage All Defenders"]?.Invoke();
            Debug.Log("Executed: Damage All Defenders");
        }
        if (GUILayout.Button("God Mode Defenders [G]"))
        {
            commands["God Mode Defenders"]?.Invoke();
            Debug.Log("Executed: God Mode Defenders");
        }
        if (GUILayout.Button("Boost Attack Speed"))
        {
            commands["Boost Attack Speed"]?.Invoke();
            Debug.Log("Executed: Boost Attack Speed");
        }
        GUILayout.Space(10);
        GUILayout.Label("Performance", GUI.skin.box);
        if (GUILayout.Button("Stress Test Enemies"))
        {
            commands["Stress Test Enemies"]?.Invoke();
            Debug.Log("Executed: Stress Test Enemies");
        }
        if (GUILayout.Button("Stress Test Projectiles"))
        {
            commands["Stress Test Projectiles"]?.Invoke();
            Debug.Log("Executed: Stress Test Projectiles");
        }
        if (GUILayout.Button("Toggle FPS Display [F]"))
        {
            commands["Toggle FPS Display"]?.Invoke();
            Debug.Log("Executed: Toggle FPS Display");
        }
        if (GUILayout.Button("Profile Frame [Ctrl+P]"))
        {
            commands["Profile Frame"]?.Invoke();
            Debug.Log("Executed: Profile Frame");
        }
        if (GUILayout.Button("Memory Snapshot"))
        {
            commands["Memory Snapshot"]?.Invoke();
            Debug.Log("Executed: Memory Snapshot");
        }
        if (GUILayout.Button("Toggle Pooling"))
        {
            commands["Toggle Pooling"]?.Invoke();
            Debug.Log("Executed: Toggle Pooling");
        }
        GUILayout.Space(10);
        GUILayout.Label("Visual", GUI.skin.box);
        if (GUILayout.Button("Test Upgrade VFX [V]"))
        {
            commands["Test Upgrade VFX"]?.Invoke();
            Debug.Log("Executed: Test Upgrade VFX");
        }
        if (GUILayout.Button("Test Hit VFX"))
        {
            commands["Test Hit VFX"]?.Invoke();
            Debug.Log("Executed: Test Hit VFX");
        }
        if (GUILayout.Button("Test Screen Shake"))
        {
            commands["Test Screen Shake"]?.Invoke();
            Debug.Log("Executed: Test Screen Shake");
        }
        if (GUILayout.Button("Test Post Process Pulse [Ctrl+V]"))
        {
            commands["Test Post Process Pulse"]?.Invoke();
            Debug.Log("Executed: Test Post Process Pulse");
        }
        if (GUILayout.Button("Cycle Shader Properties"))
        {
            commands["Cycle Shader Properties"]?.Invoke();
            Debug.Log("Executed: Cycle Shader Properties");
        }
        if (GUILayout.Button("Toggle Wireframe Mode [W]"))
        {
            commands["Toggle Wireframe Mode"]?.Invoke();
            Debug.Log("Executed: Toggle Wireframe Mode");
        }
        if (GUILayout.Button("Test Color Variants"))
        {
            commands["Test Color Variants"]?.Invoke();
            Debug.Log("Executed: Test Color Variants");
        }
        GUILayout.Space(10);
        GUILayout.Label("Testing", GUI.skin.box);
        if (GUILayout.Button("Test Early Game"))
        {
            commands["Test Early Game"]?.Invoke();
            Debug.Log("Executed: Test Early Game");
        }
        if (GUILayout.Button("Test Mid Game"))
        {
            commands["Test Mid Game"]?.Invoke();
            Debug.Log("Executed: Test Mid Game");
        }
        if (GUILayout.Button("Test Late Game"))
        {
            commands["Test Late Game"]?.Invoke();
            Debug.Log("Executed: Test Late Game");
        }
        if (GUILayout.Button("Test Boss Battle"))
        {
            commands["Test Boss Battle"]?.Invoke();
            Debug.Log("Executed: Test Boss Battle");
        }
        if (GUILayout.Button("Test Upgrade Progression"))
        {
            commands["Test Upgrade Progression"]?.Invoke();
            Debug.Log("Executed: Test Upgrade Progression");
        }
        if (GUILayout.Button("Test Defense Layout"))
        {
            commands["Test Defense Layout"]?.Invoke();
            Debug.Log("Executed: Test Defense Layout");
        }
        if (GUILayout.Button("Run All Tests [Ctrl+T]"))
        {
            commands["Run All Tests"]?.Invoke();
            Debug.Log("Executed: Run All Tests");
        }
        GUILayout.Space(10);
        
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }
}