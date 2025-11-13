using UnityEngine;

/// <summary>
/// Auto-wires missing references in the Game scene on Awake.
/// Implements all requirements from FINAL_WIRING_GUIDE.md
/// </summary>
public class SceneAutoWirer : MonoBehaviour
{
    [Header("Auto-Wire Settings")]
    [SerializeField] private bool runOnAwake = true;
    [SerializeField] private bool verbose = true;

    void Awake()
    {
        if (runOnAwake)
        {
            WireScene();
        }
    }

    [ContextMenu("Wire Scene Now")]
    public void WireScene()
    {
        if (verbose) Debug.Log("[SceneAutoWirer] Starting comprehensive auto-wire...");

        WireGameController();
        WireUpgradeShop();
        WireVariantTelemetry();
        WireVfxSpawner();
        WireShaderDrivers();
        WireMaterials();
        WireUIToolkit();
        DisableOldCanvasUI();
        
        if (verbose) Debug.Log("[SceneAutoWirer] Auto-wire complete!");
    }

    void WireGameController()
    {
        GameController gc = FindFirstObjectByType<GameController>();
        if (gc == null)
        {
            if (verbose) Debug.LogWarning("[SceneAutoWirer] GameController not found!");
            return;
        }

        // Wire UpgradePanelUIDocument
        var panel = FindFirstObjectByType<UpgradePanelUIDocument>();
        if (panel != null)
        {
            SetField(gc, "upgradePanel", panel);
            if (verbose) Debug.Log("[SceneAutoWirer] Wired GameController.upgradePanel");
        }

        // Wire variant config
        var variantConfig = Resources.Load<ProceduralVariantConfig>("DefaultVariantConfig");
        if (variantConfig != null)
        {
            SetField(gc, "variantConfig", variantConfig);
            if (verbose) Debug.Log("[SceneAutoWirer] Wired GameController.variantConfig");
        }

        // Wire adaptive difficulty config
        var difficultyConfig = Resources.Load<AdaptiveWaveDifficultyConfig>("DefaultAdaptiveDifficultyConfig");
        if (difficultyConfig != null)
        {
            SetField(gc, "adaptiveDifficultyConfig", difficultyConfig);
            if (verbose) Debug.Log("[SceneAutoWirer] Wired GameController.adaptiveDifficultyConfig");
        }
    }

    void WireUpgradeShop()
    {
        // UpgradeShop is already on GameController - nothing to wire
    }

    void WireVariantTelemetry()
    {
        // VariantTelemetryPresenter moved to _Deprecated - no longer needed
    }

    void WireVfxSpawner()
    {
        var vfxSpawner = FindFirstObjectByType<UpgradeVfxSpawner>();
        if (vfxSpawner == null)
        {
            if (verbose) Debug.LogWarning("[SceneAutoWirer] UpgradeVfxSpawner not found!");
            return;
        }

        // Wire particle prefabs from Resources
        var defenderEffect = Resources.Load<ParticleSystem>("Prefab_VFX_BuildBurst");
        if (defenderEffect != null)
        {
            SetField(vfxSpawner, "defenderUpgradeEffect", defenderEffect);
            SetField(vfxSpawner, "towerUpgradeEffect", defenderEffect);
            if (verbose) Debug.Log("[SceneAutoWirer] Wired VFX particle effects");
        }

        // Wire anchors - find or use vfxSpawner's transform
        var defenderAnchor = GameObject.Find("DefenderVfxAnchor");
        var towerAnchor = GameObject.Find("TowerVfxAnchor");
        
        if (defenderAnchor != null)
        {
            SetField(vfxSpawner, "defenderEffectAnchor", defenderAnchor.transform);
        }
        else
        {
            SetField(vfxSpawner, "defenderEffectAnchor", vfxSpawner.transform);
        }

        if (towerAnchor != null)
        {
            SetField(vfxSpawner, "towerEffectAnchor", towerAnchor.transform);
        }
        else
        {
            SetField(vfxSpawner, "towerEffectAnchor", vfxSpawner.transform);
        }

        if (verbose) Debug.Log("[SceneAutoWirer] Wired UpgradeVfxSpawner anchors");
    }

    void WireShaderDrivers()
    {
        var drivers = FindObjectsByType<UpgradeVisualShaderDriver>(FindObjectsSortMode.None);
        if (drivers == null || drivers.Length == 0)
        {
            if (verbose) Debug.Log("[SceneAutoWirer] No UpgradeVisualShaderDrivers found");
            return;
        }

        foreach (var driver in drivers)
        {
            // Try to find renderer on same GameObject or children
            var renderer = driver.GetComponent<Renderer>();
            if (renderer == null)
            {
                renderer = driver.GetComponentInChildren<Renderer>();
            }

            if (renderer != null)
            {
                SetField(driver, "targetRenderer", renderer);
                if (verbose) Debug.Log($"[SceneAutoWirer] Wired renderer for {driver.gameObject.name}");
            }

            // Try to find model swapper
            var swapper = driver.GetComponent<UpgradeModelSwapper>();
            if (swapper != null)
            {
                SetField(driver, "modelSwapper", swapper);
                if (verbose) Debug.Log($"[SceneAutoWirer] Wired model swapper for {driver.gameObject.name}");
            }
        }
    }

    void WireMaterials()
    {
        // Auto-assign custom shader materials to defenders
        var defenders = FindObjectsByType<DefenderUpgrade>(FindObjectsSortMode.None);
        if (defenders == null || defenders.Length == 0)
        {
            if (verbose) Debug.Log("[SceneAutoWirer] No DefenderUpgrade components found for material assignment");
            return;
        }

        // Try to load materials from Resources
        Material vertexMat = Resources.Load<Material>("UpgradeVertexMaterial");
        Material colorMat = Resources.Load<Material>("UpgradeColorMaterial");

        if (vertexMat == null && colorMat == null)
        {
            if (verbose) Debug.Log("[SceneAutoWirer] No upgrade materials found in Resources");
            return;
        }

        foreach (var defender in defenders)
        {
            var renderer = defender.GetComponent<Renderer>();
            if (renderer == null)
            {
                renderer = defender.GetComponentInChildren<Renderer>();
            }

            if (renderer != null && renderer.sharedMaterial != null)
            {
                // Add materials if not already present
                var materials = new System.Collections.Generic.List<Material>(renderer.sharedMaterials);
                bool modified = false;

                if (vertexMat != null && !materials.Contains(vertexMat))
                {
                    materials.Add(vertexMat);
                    modified = true;
                }

                if (colorMat != null && !materials.Contains(colorMat))
                {
                    materials.Add(colorMat);
                    modified = true;
                }

                if (modified)
                {
                    renderer.sharedMaterials = materials.ToArray();
                    if (verbose) Debug.Log($"[SceneAutoWirer] Added upgrade materials to {defender.gameObject.name}");
                }
            }
        }
    }

    void WireUIToolkit()
    {
        // Wire TopBanner
        var topBannerGO = GameObject.Find("TopBanner");
        if (topBannerGO != null)
        {
            var bannerDoc = topBannerGO.GetComponent<UnityEngine.UIElements.UIDocument>();
            var bannerScript = topBannerGO.GetComponent<TopBannerUIDocument>();
            
            if (bannerDoc != null && bannerScript != null)
            {
                #if UNITY_EDITOR
                var bannerUxml = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.UIElements.VisualTreeAsset>("Assets/UI/TopBanner.uxml");
                var bannerUss = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.UIElements.StyleSheet>("Assets/UI/TopBanner.uss");
                
                if (bannerUxml != null)
                {
                    bannerDoc.visualTreeAsset = bannerUxml;
                    SetField(bannerScript, "_uxml", bannerUxml);
                }
                if (bannerUss != null)
                {
                    SetField(bannerScript, "_uss", bannerUss);
                }
                
                if (verbose && bannerUxml != null) Debug.Log("[SceneAutoWirer] Assigned TopBanner UXML/USS");
                #endif
            }
        }
        
        // Wire InfoHud UI Toolkit
        var infoHudGO = GameObject.Find("InfoHud");
        if (infoHudGO == null)
        {
            if (verbose) Debug.LogWarning("[SceneAutoWirer] InfoHud GameObject not found!");
            return;
        }

        var uiDoc = infoHudGO.GetComponent<UnityEngine.UIElements.UIDocument>();
        var infoHudScript = infoHudGO.GetComponent<InfoHudUIDocument>();

        if (uiDoc == null)
        {
            if (verbose) Debug.LogWarning("[SceneAutoWirer] InfoHud missing UIDocument component!");
            return;
        }

        // Load UXML and USS
        var uxml = UnityEngine.Resources.Load<UnityEngine.UIElements.VisualTreeAsset>("UI/InfoHud");
        if (uxml == null)
        {
            // Try loading from Assets/UI directly
            #if UNITY_EDITOR
            uxml = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.UIElements.VisualTreeAsset>("Assets/UI/InfoHud.uxml");
            #endif
        }

        var uss = UnityEngine.Resources.Load<UnityEngine.UIElements.StyleSheet>("UI/InfoHud");
        if (uss == null)
        {
            #if UNITY_EDITOR
            uss = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.UIElements.StyleSheet>("Assets/UI/InfoHud.uss");
            #endif
        }

        // Assign to UIDocument
        if (uxml != null)
        {
            uiDoc.visualTreeAsset = uxml;
            if (verbose) Debug.Log("[SceneAutoWirer] Assigned InfoHud.uxml to UIDocument");
        }
        else
        {
            if (verbose) Debug.LogWarning("[SceneAutoWirer] Could not find InfoHud.uxml!");
        }

        // Find or create PanelSettings
        if (uiDoc.panelSettings == null)
        {
            var panelSettings = Resources.FindObjectsOfTypeAll<UnityEngine.UIElements.PanelSettings>();
            if (panelSettings.Length > 0)
            {
                uiDoc.panelSettings = panelSettings[0];
                if (verbose) Debug.Log("[SceneAutoWirer] Assigned existing PanelSettings to InfoHud");
            }
            else
            {
                if (verbose) Debug.LogWarning("[SceneAutoWirer] No PanelSettings found - InfoHud may not display correctly");
            }
        }

        // Assign to InfoHudUIDocument script
        if (infoHudScript != null)
        {
            if (uxml != null) SetField(infoHudScript, "_uxml", uxml);
            if (uss != null) SetField(infoHudScript, "_uss", uss);
            if (verbose) Debug.Log("[SceneAutoWirer] Wired InfoHudUIDocument fields");
        }
        
        // Wire UpgradePanel UI Toolkit
        var upgradePanelGO = GameObject.Find("UpgradePanel");
        if (upgradePanelGO != null)
        {
            var panelDoc = upgradePanelGO.GetComponent<UnityEngine.UIElements.UIDocument>();
            var panelScript = upgradePanelGO.GetComponent<UpgradePanelUIDocument>();
            
            if (panelDoc != null && panelScript != null)
            {
                // Load UXML and USS
                var panelUxml = UnityEngine.Resources.Load<UnityEngine.UIElements.VisualTreeAsset>("UI/UpgradePanel");
                if (panelUxml == null)
                {
                    #if UNITY_EDITOR
                    panelUxml = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.UIElements.VisualTreeAsset>("Assets/UI/UpgradePanel.uxml");
                    #endif
                }
                
                var panelUss = UnityEngine.Resources.Load<UnityEngine.UIElements.StyleSheet>("UI/UpgradePanel");
                if (panelUss == null)
                {
                    #if UNITY_EDITOR
                    panelUss = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.UIElements.StyleSheet>("Assets/UI/UpgradePanel.uss");
                    #endif
                }
                
                // Assign to UIDocument
                if (panelUxml != null)
                {
                    panelDoc.visualTreeAsset = panelUxml;
                    if (verbose) Debug.Log("[SceneAutoWirer] Assigned UpgradePanel.uxml to UIDocument");
                }
                
                // Assign PanelSettings
                if (panelDoc.panelSettings == null)
                {
                    var panelSettings = Resources.FindObjectsOfTypeAll<UnityEngine.UIElements.PanelSettings>();
                    if (panelSettings.Length > 0)
                    {
                        panelDoc.panelSettings = panelSettings[0];
                        if (verbose) Debug.Log("[SceneAutoWirer] Assigned PanelSettings to UpgradePanel");
                    }
                }
                
                // Assign to UpgradePanelUIDocument script
                if (panelUxml != null) SetField(panelScript, "_uxml", panelUxml);
                if (panelUss != null) SetField(panelScript, "_uss", panelUss);
                if (verbose) Debug.Log("[SceneAutoWirer] Wired UpgradePanelUIDocument fields");
            }
        }
    }

    void DisableOldCanvasUI()
    {
        // Disable old Canvas Screen GameObject if it exists
        var canvasScreen = GameObject.Find("Canvas Screen");
        if (canvasScreen != null)
        {
            canvasScreen.SetActive(false);
            if (verbose) Debug.Log("[SceneAutoWirer] Disabled old 'Canvas Screen' GameObject");
        }

        // Check for incorrectly configured health bars
        var healthBars = FindObjectsByType<HealthBarController>(FindObjectsSortMode.None);
        foreach (var hb in healthBars)
        {
            var canvas = hb.GetComponentInParent<Canvas>();
            if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                if (verbose) Debug.LogWarning($"[SceneAutoWirer] HealthBar on {hb.gameObject.name} is in ScreenSpace mode - should be WorldSpace!");
            }
        }
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
            Debug.LogWarning($"[SceneAutoWirer] Field '{fieldName}' not found on {target.GetType().Name}");
        }
    }
}
