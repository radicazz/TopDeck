using UnityEngine;
using UnityEditor;

/// <summary>
/// Automatically creates default config assets on first import.
/// Unity will run this when the Editor compiles scripts.
/// </summary>
[InitializeOnLoad]
public static class AutoCreateDefaultConfigs
{
    static AutoCreateDefaultConfigs()
    {
        EditorApplication.delayCall += CreateConfigsIfNeeded;
    }

    static void CreateConfigsIfNeeded()
    {
        bool created = false;

        // Create ProceduralVariantConfig if missing
        string variantPath = "Assets/Resources/DefaultVariantConfig.asset";
        if (!AssetDatabase.LoadAssetAtPath<ProceduralVariantConfig>(variantPath))
        {
            var config = ScriptableObject.CreateInstance<ProceduralVariantConfig>();
            AssetDatabase.CreateAsset(config, variantPath);
            created = true;
            Debug.Log($"[AutoCreate] Created ProceduralVariantConfig at {variantPath}");
        }

        // Create AdaptiveWaveDifficultyConfig if missing
        string adaptivePath = "Assets/Resources/DefaultAdaptiveDifficultyConfig.asset";
        if (!AssetDatabase.LoadAssetAtPath<AdaptiveWaveDifficultyConfig>(adaptivePath))
        {
            var config = ScriptableObject.CreateInstance<AdaptiveWaveDifficultyConfig>();
            AssetDatabase.CreateAsset(config, adaptivePath);
            created = true;
            Debug.Log($"[AutoCreate] Created AdaptiveWaveDifficultyConfig at {adaptivePath}");
        }

        if (created)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[AutoCreate] ✓ Config assets created successfully. Assign them to GameController.");
        }
    }
}
