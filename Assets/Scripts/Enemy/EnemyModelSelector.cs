using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;

/// <summary>
/// Selects appropriate enemy models from 20+ variants based on enemy type and upgrade level.
/// Supports Basic, Advanced, Long, Short, and Sniper defender models with 4 upgrade tiers each.
/// </summary>
public static class EnemyModelSelector
{
    public enum DefenderType
    {
        Basic,
        Advanced,
        Long,
        Short,
        Sniper
    }

    public enum UpgradeTier
    {
        Base,
        HealthUpgrade,
        MiscUpgrade,
        FullUpgrade
    }

    private static Dictionary<string, GameObject> _modelCache = new Dictionary<string, GameObject>();
    private static bool _cacheInitialized = false;

    /// <summary>
    /// Selects a model based on difficulty tier and enemy category.
    /// </summary>
    public static GameObject SelectModel(VariantCategory category, int wave, float difficultyMultiplier)
    {
        DefenderType type = SelectDefenderType(category, wave);
        UpgradeTier tier = SelectUpgradeTier(difficultyMultiplier, wave);
        
        return GetModel(type, tier);
    }

    /// <summary>
    /// Applies a selected model to the provided enemy instance.
    /// </summary>
    public static void SelectModel(GameObject enemy, int wave, VariantCategory category)
    {
        if (enemy == null)
        {
            return;
        }

        float difficultyMultiplier = DifficultyScaling.GetDifficultyMultiplier(wave);
        GameObject modelPrefab = SelectModel(category, wave, difficultyMultiplier);

        if (modelPrefab == null)
        {
            return;
        }

        Transform visualRoot = enemy.transform.Find("ModelRoot");
        if (visualRoot == null)
        {
            visualRoot = new GameObject("ModelRoot").transform;
            visualRoot.SetParent(enemy.transform, false);
            visualRoot.localPosition = Vector3.zero;
            visualRoot.localRotation = Quaternion.identity;
            visualRoot.localScale = Vector3.one;
        }

        for (int i = visualRoot.childCount - 1; i >= 0; i--)
        {
            Object.Destroy(visualRoot.GetChild(i).gameObject);
        }

        GameObject instance = Object.Instantiate(modelPrefab, visualRoot);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Gets a specific model by type and upgrade tier.
    /// </summary>
    public static GameObject GetModel(DefenderType type, UpgradeTier tier)
    {
        string modelPath = GetModelPath(type, tier);
        
        if (!_cacheInitialized)
        {
            InitializeCache();
        }

        if (_modelCache.TryGetValue(modelPath, out GameObject cachedModel))
        {
            return cachedModel;
        }

        // Load from Resources
        GameObject model = Resources.Load<GameObject>(modelPath);
        
        if (model != null)
        {
            _modelCache[modelPath] = model;
        }
        else
        {
            Debug.LogWarning($"Could not load model at path: {modelPath}");
        }

        return model;
    }

    /// <summary>
    /// Preloads all models into cache for faster access.
    /// </summary>
    public static void PreloadModels()
    {
        InitializeCache();
    }

    private static void InitializeCache()
    {
        if (_cacheInitialized) return;

        // Preload commonly used models
        foreach (DefenderType type in System.Enum.GetValues(typeof(DefenderType)))
        {
            foreach (UpgradeTier tier in System.Enum.GetValues(typeof(UpgradeTier)))
            {
                string modelPath = GetModelPath(type, tier);
                GameObject model = Resources.Load<GameObject>(modelPath);
                
                if (model != null)
                {
                    _modelCache[modelPath] = model;
                }
            }
        }

        _cacheInitialized = true;
        Debug.Log($"EnemyModelSelector: Preloaded {_modelCache.Count} enemy models");
    }

    private static DefenderType SelectDefenderType(VariantCategory category, int wave)
    {
        switch (category)
        {
            case VariantCategory.MiniBoss:
                // Mini-bosses use Advanced or Sniper models
                return wave % 2 == 0 ? DefenderType.Advanced : DefenderType.Sniper;
            
            case VariantCategory.Elite:
                // Elites use Long or Short models
                return Random.value > 0.5f ? DefenderType.Long : DefenderType.Short;
            
            case VariantCategory.Normal:
            default:
                // Normal enemies use Basic model primarily, with variety
                int choice = Mathf.FloorToInt(Random.value * 100);
                
                if (choice < 60)
                    return DefenderType.Basic;
                else if (choice < 75)
                    return DefenderType.Long;
                else if (choice < 90)
                    return DefenderType.Short;
                else
                    return DefenderType.Advanced;
        }
    }

    private static UpgradeTier SelectUpgradeTier(float difficultyMultiplier, int wave)
    {
        // Map difficulty multiplier to upgrade tiers
        // Wave 1: 1.0x → Base
        // Wave 5: ~1.46x → HealthUpgrade
        // Wave 10: ~2.36x → MiscUpgrade
        // Wave 15: ~3.80x → FullUpgrade
        
        if (difficultyMultiplier < 1.3f)
            return UpgradeTier.Base;
        else if (difficultyMultiplier < 2.0f)
            return UpgradeTier.HealthUpgrade;
        else if (difficultyMultiplier < 3.0f)
            return UpgradeTier.MiscUpgrade;
        else
            return UpgradeTier.FullUpgrade;
    }

    private static string GetModelPath(DefenderType type, UpgradeTier tier)
    {
        string baseName = $"Models/Defender/Model_Defender_{type}";
        
        switch (tier)
        {
            case UpgradeTier.Base:
                return baseName;
            case UpgradeTier.HealthUpgrade:
                return $"{baseName}_HealthUpgrade";
            case UpgradeTier.MiscUpgrade:
                return $"{baseName}_MiscUpgrade";
            case UpgradeTier.FullUpgrade:
                return $"{baseName}_FullUpgrade";
            default:
                return baseName;
        }
    }

    /// <summary>
    /// Gets a random model for variety.
    /// </summary>
    public static GameObject GetRandomModel()
    {
        DefenderType randomType = (DefenderType)Random.Range(0, System.Enum.GetValues(typeof(DefenderType)).Length);
        UpgradeTier randomTier = (UpgradeTier)Random.Range(0, System.Enum.GetValues(typeof(UpgradeTier)).Length);
        
        return GetModel(randomType, randomTier);
    }

    /// <summary>
    /// Clears the model cache. Useful for memory management.
    /// </summary>
    public static void ClearCache()
    {
        _modelCache.Clear();
        _cacheInitialized = false;
    }
}
