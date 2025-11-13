using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Auto-wires Start Menu UI on Awake, matching pattern from SceneAutoWirer
/// </summary>
public class StartMenuAutoWirer : MonoBehaviour
{
    [SerializeField] private bool verbose = true;
    
    void Awake()
    {
        SetupStartMenuUI();
    }
    
    void SetupStartMenuUI()
    {
        if (verbose) Debug.Log("[StartMenuAutoWirer] Setting up Start Menu UI...");
        
        GameObject uiDocGO = GameObject.Find("StartMenuUI");
        if (uiDocGO == null)
        {
            if (verbose) Debug.LogError("[StartMenuAutoWirer] StartMenuUI GameObject not found!");
            return;
        }
        
        UIDocument uiDoc = uiDocGO.GetComponent<UIDocument>();
        if (uiDoc == null)
        {
            uiDoc = uiDocGO.AddComponent<UIDocument>();
            if (verbose) Debug.Log("[StartMenuAutoWirer] Added UIDocument component");
        }
        
        // Load UXML
        VisualTreeAsset uxml = Resources.Load<VisualTreeAsset>("UI/StartMenu");
        if (uxml == null)
        {
            #if UNITY_EDITOR
            uxml = UnityEditor.AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/StartMenu.uxml");
            if (uxml == null)
            {
                uxml = UnityEditor.AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Resources/UI/StartMenu.uxml");
            }
            #endif
        }
        
        if (uxml != null)
        {
            uiDoc.visualTreeAsset = uxml;
            if (verbose) Debug.Log("[StartMenuAutoWirer] Assigned StartMenu.uxml to UIDocument");
        }
        else
        {
            Debug.LogError("[StartMenuAutoWirer] Could not find StartMenu.uxml!");
        }
        
        // Load USS
        StyleSheet uss = Resources.Load<StyleSheet>("UI/StartMenu");
        if (uss == null)
        {
            #if UNITY_EDITOR
            uss = UnityEditor.AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI/StartMenu.uss");
            if (uss == null)
            {
                uss = UnityEditor.AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Resources/UI/StartMenu.uss");
            }
            #endif
        }
        
        // Assign PanelSettings
        if (uiDoc.panelSettings == null)
        {
            PanelSettings panelSettings = null;
            #if UNITY_EDITOR
            panelSettings = UnityEditor.AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI Toolkit/PanelSettings.asset");
            if (panelSettings != null)
            {
                uiDoc.panelSettings = panelSettings;
                if (verbose) Debug.Log("[StartMenuAutoWirer] Assigned PanelSettings from Assets/UI Toolkit");
            }
            else
            #endif
            {
                var panelSettingsArray = Resources.FindObjectsOfTypeAll<PanelSettings>();
                if (panelSettingsArray.Length > 0)
                {
                    uiDoc.panelSettings = panelSettingsArray[0];
                    if (verbose) Debug.Log("[StartMenuAutoWirer] Assigned existing PanelSettings");
                }
                else
                {
                    Debug.LogWarning("[StartMenuAutoWirer] No PanelSettings found");
                }
            }
        }
        
        // Wire StartMenuUIDocument component
        StartMenuUIDocument menuDoc = uiDocGO.GetComponent<StartMenuUIDocument>();
        if (menuDoc == null)
        {
            menuDoc = uiDocGO.AddComponent<StartMenuUIDocument>();
            if (verbose) Debug.Log("[StartMenuAutoWirer] Added StartMenuUIDocument component");
        }
        
        if (menuDoc != null)
        {
            if (uxml != null) SetField(menuDoc, "_uxml", uxml);
            if (uss != null) SetField(menuDoc, "_uss", uss);
            if (verbose) Debug.Log("[StartMenuAutoWirer] Wired StartMenuUIDocument fields");
        }
        
        if (verbose) Debug.Log("[StartMenuAutoWirer] Start Menu UI setup complete!");
    }
    
    void SetField(object target, string fieldName, object value)
    {
        var field = target.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Public | 
            System.Reflection.BindingFlags.Instance);
        
        if (field != null)
        {
            field.SetValue(target, value);
        }
        else if (verbose)
        {
            Debug.LogWarning($"[StartMenuAutoWirer] Field '{fieldName}' not found on {target.GetType().Name}");
        }
    }
}
