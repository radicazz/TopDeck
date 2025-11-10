using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor utility to create default ProceduralVariantConfig assets.
/// Menu: Assets/Create/TopDeck/Create Default Variant Config
/// </summary>
public static class ProceduralVariantConfigCreator
{
    [MenuItem("Assets/Create/TopDeck/Create Default Variant Config")]
    public static void CreateDefaultConfig()
    {
        var config = ScriptableObject.CreateInstance<ProceduralVariantConfig>();
        
        string path = EditorUtility.SaveFilePanelInProject(
            "Save Procedural Variant Config",
            "DefaultVariantConfig",
            "asset",
            "Choose location for the config asset"
        );

        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(config, path);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = config;
            Debug.Log($"Created ProceduralVariantConfig at {path}");
        }
    }
}
