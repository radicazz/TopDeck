using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Creates default config asset instances for Part 3 systems.
/// Run via: Tools → TopDeck → Create Default Configs
/// </summary>
public static class ConfigAssetCreator
{
    [MenuItem("Tools/TopDeck/Create Default Configs")]
    public static void CreateAllDefaultConfigs()
    {
        CreateProceduralVariantConfig();
        CreateAdaptiveWaveDifficultyConfig();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✓ Created all default config assets");
    }

    static void CreateProceduralVariantConfig()
    {
        string path = "Assets/Resources/DefaultVariantConfig.asset";
        
        if (AssetDatabase.LoadAssetAtPath<ProceduralVariantConfig>(path) != null)
        {
            Debug.Log($"ProceduralVariantConfig already exists at {path}");
            return;
        }

        var config = ScriptableObject.CreateInstance<ProceduralVariantConfig>();
        
        // Set up default gradient (blue to red for difficulty)
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(new Color(0.2f, 0.6f, 1f), 0f), // Blue (easier)
                new GradientColorKey(new Color(1f, 1f, 0.3f), 0.5f),  // Yellow (medium)
                new GradientColorKey(new Color(1f, 0.3f, 0.2f), 1f)   // Red (harder)
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(1f, 0f), 
                new GradientAlphaKey(1f, 1f) 
            }
        );

        AssetDatabase.CreateAsset(config, path);
        Debug.Log($"✓ Created ProceduralVariantConfig at {path}");
    }

    static void CreateAdaptiveWaveDifficultyConfig()
    {
        string path = "Assets/Resources/DefaultAdaptiveDifficultyConfig.asset";
        
        if (AssetDatabase.LoadAssetAtPath<AdaptiveWaveDifficultyConfig>(path) != null)
        {
            Debug.Log($"AdaptiveWaveDifficultyConfig already exists at {path}");
            return;
        }

        var config = ScriptableObject.CreateInstance<AdaptiveWaveDifficultyConfig>();
        AssetDatabase.CreateAsset(config, path);
        Debug.Log($"✓ Created AdaptiveWaveDifficultyConfig at {path}");
    }
}
