using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Ensures all scenes have proper UI setup when loaded
/// </summary>
[DefaultExecutionOrder(-200)]
public class UIAutoSetup : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[UIAutoSetup] Scene loaded: {scene.name}");
        
        if (scene.name == "Start Menu")
        {
            SetupStartMenu(scene);
        }
        else if (scene.name == "Game")
        {
            SetupGameScene(scene);
        }
    }
    
    static void SetupStartMenu(Scene scene)
    {
        Debug.Log("[UIAutoSetup] Setting up Start Menu UI...");
        
        // Check if auto-wirer already exists
        StartMenuAutoWirer existingWirer = Object.FindFirstObjectByType<StartMenuAutoWirer>();
        if (existingWirer != null)
        {
            Debug.Log("[UIAutoSetup] StartMenuAutoWirer already exists");
            return;
        }
        
        // Create auto-wirer GameObject
        GameObject wirerGO = new GameObject("_StartMenuAutoWirer");
        wirerGO.AddComponent<StartMenuAutoWirer>();
        
        Debug.Log("[UIAutoSetup] Added StartMenuAutoWirer to scene");
    }
    
    static void SetupGameScene(Scene scene)
    {
        Debug.Log("[UIAutoSetup] Setting up Game scene UI...");
        
        // Check if auto-wirer already exists
        SceneAutoWirer existingWirer = Object.FindFirstObjectByType<SceneAutoWirer>();
        if (existingWirer == null)
        {
            GameObject wirerGO = new GameObject("_SceneAutoWirer");
            wirerGO.AddComponent<SceneAutoWirer>();
            Debug.Log("[UIAutoSetup] Added SceneAutoWirer to Game scene");
        }
        
        // Setup InfoHud if needed
        SetupInfoHud();
        
        // Setup UpgradePanel if needed
        SetupUpgradePanel();
    }
    
    static void SetupInfoHud()
    {
        InfoHudUIDocument existingHud = Object.FindFirstObjectByType<InfoHudUIDocument>();
        if (existingHud != null) return;
        
        GameObject hudGO = new GameObject("InfoHud");
        var uiDoc = hudGO.AddComponent<UnityEngine.UIElements.UIDocument>();
        
        // Load resources
        var panelSettings = Resources.Load<UnityEngine.UIElements.PanelSettings>("UI Toolkit/PanelSettings");
        if (panelSettings == null)
        {
            panelSettings = Resources.FindObjectsOfTypeAll<UnityEngine.UIElements.PanelSettings>()[0];
        }
        uiDoc.panelSettings = panelSettings;
        
        var uxml = Resources.Load<UnityEngine.UIElements.VisualTreeAsset>("UI/InfoHud");
        if (uxml != null) uiDoc.visualTreeAsset = uxml;
        
        hudGO.AddComponent<InfoHudUIDocument>();
        
        Debug.Log("[UIAutoSetup] Created InfoHud UI");
    }
    
    static void SetupUpgradePanel()
    {
        UpgradePanelUIDocument existingPanel = Object.FindFirstObjectByType<UpgradePanelUIDocument>();
        if (existingPanel != null) return;
        
        GameObject panelGO = new GameObject("UpgradePanel");
        var uiDoc = panelGO.AddComponent<UnityEngine.UIElements.UIDocument>();
        
        // Load resources
        var panelSettings = Resources.Load<UnityEngine.UIElements.PanelSettings>("UI Toolkit/PanelSettings");
        if (panelSettings == null)
        {
            panelSettings = Resources.FindObjectsOfTypeAll<UnityEngine.UIElements.PanelSettings>()[0];
        }
        uiDoc.panelSettings = panelSettings;
        
        var uxml = Resources.Load<UnityEngine.UIElements.VisualTreeAsset>("UI/UpgradePanel");
        if (uxml != null) uiDoc.visualTreeAsset = uxml;
        
        panelGO.AddComponent<UpgradePanelUIDocument>();
        panelGO.SetActive(false); // Start hidden
        
        Debug.Log("[UIAutoSetup] Created UpgradePanel UI");
    }
}
