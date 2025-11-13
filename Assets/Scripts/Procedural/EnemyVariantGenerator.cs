using UnityEngine;

/// <summary>
/// Creates procedural enemy variants by adjusting stats per spawn and applying a visual tint.
/// Optionally uses ProceduralVariantConfig for designer-tunable curves.
/// Adds elite and mini-boss categories plus hooks for adaptive wave planners.
/// </summary>
public enum VariantCategory
{
    Normal,
    Elite,
    MiniBoss
}

public struct VariantRequest
{
    public float DifficultyScalar;
    public float EliteBudgetRatio;
    public float MiniBossBudgetRatio;
    public float PatternAggression;
    public bool ForceElite;
    public bool ForceMiniBoss;
}

public static class EnemyVariantGenerator
{
    public struct Variant
    {
        public AttackerTypeDefinition definition;
        public Color tint;
        public float healthMultiplier;
        public float speedMultiplier;
        public float damageMultiplier;
        public float sizeMultiplier;
        public VariantCategory category;
        public float threatScore;
    }

    private const float WaveNormalization = 12f;

    private static ProceduralVariantConfig _config;

    public static void SetConfig(ProceduralVariantConfig config)
    {
        _config = config;
    }

    public static Variant CreateVariant(
        AttackerTypeDefinition baseType,
        int wave,
        int defenderUpgradeLevel,
        VariantRequest request = default)
    {
        float normalizedWave = Mathf.Clamp01((wave - 1f) / WaveNormalization);
        float waveFactor;
        float counterFactor;

        if (_config != null)
        {
            waveFactor = 1f + _config.WaveScaleFactor * Mathf.Max(0, wave - 1);
            counterFactor = 1f + _config.DefenderCounterFactor * Mathf.Max(0, defenderUpgradeLevel);
        }
        else
        {
            waveFactor = 1f + 0.05f * Mathf.Max(0, wave - 1);
            counterFactor = 1f + 0.03f * Mathf.Max(0, defenderUpgradeLevel);
        }

        float difficultyScalar = Mathf.Clamp01(request.DifficultyScalar);
        float scale = waveFactor * counterFactor * Mathf.Lerp(0.95f, 1.2f, difficultyScalar);
        float aggression = Mathf.Clamp01(request.PatternAggression);

        bool spawnMiniBoss = request.ForceMiniBoss;
        bool spawnElite = request.ForceElite;

        if (!spawnMiniBoss)
        {
            spawnMiniBoss = ShouldSpawnMiniBoss(normalizedWave, request.MiniBossBudgetRatio, difficultyScalar);
        }

        if (!spawnElite && !spawnMiniBoss)
        {
            spawnElite = ShouldSpawnElite(normalizedWave, request.EliteBudgetRatio, difficultyScalar);
        }

        VariantCategory category = spawnMiniBoss
            ? VariantCategory.MiniBoss
            : spawnElite ? VariantCategory.Elite : VariantCategory.Normal;

        int baseHealth = baseType != null ? baseType.BaseHealth : 100;
        float moveSpeed = baseType != null ? baseType.MoveSpeed : 3f;
        float towerDamage = baseType != null ? baseType.TowerDamage : 20f;
        float attackRange = baseType != null ? baseType.AttackRange : 1.5f;
        float attackRate = baseType != null ? baseType.AttackRate : 1f;
        int damageToPlayer = baseType != null ? baseType.DamageToPlayer : 10;

        float healthMult;
        float speedMult;
        float damageMult;

        if (_config != null)
        {
            healthMult = _config.EvaluateHealth(normalizedWave) * scale;
            speedMult = _config.EvaluateSpeed(normalizedWave) * Mathf.Lerp(0.9f, 1.25f, aggression);
            damageMult = _config.EvaluateDamage(normalizedWave) * scale * Mathf.Lerp(0.9f, 1.3f, aggression);
        }
        else
        {
            healthMult = scale * Random.Range(0.9f, 1.2f);
            speedMult = Random.Range(0.9f, 1.15f) * Mathf.Lerp(0.9f, 1.25f, aggression);
            damageMult = Random.Range(0.9f, 1.15f) * scale * Mathf.Lerp(0.9f, 1.3f, aggression);
        }

        ApplyCategoryBoosts(category, ref healthMult, ref speedMult, ref damageMult);

        int healthV = Mathf.Max(1, Mathf.RoundToInt(baseHealth * healthMult));
        float speedV = Mathf.Max(0.5f, moveSpeed * speedMult);
        float towerDmgV = Mathf.Max(0f, towerDamage * damageMult);
        float rangeV = Mathf.Max(0.5f, attackRange * Random.Range(0.95f, 1.1f));
        float rateV = Mathf.Max(0.1f, attackRate * Random.Range(0.95f, 1.1f));
        int dmgPlayerV = Mathf.Max(1, Mathf.RoundToInt(damageToPlayer * damageMult));

        var variantType = new AttackerTypeDefinition(
            baseType != null ? baseType.Id + "_Var" : "Variant",
            baseType != null ? baseType.Prefab : null,
            healthV,
            speedV,
            towerDmgV,
            rangeV,
            rateV,
            dmgPlayerV,
            baseType != null ? baseType.SpawnWeight : 1f
        );

        Color tint = EvaluateTint(scale, normalizedWave, category);
        float threatScore = ComputeThreatScore(healthMult, speedMult, damageMult, category);
        float sizeMultiplier = EvaluateSizeMultiplier(healthMult, category);

        return new Variant
        {
            definition = variantType,
            tint = tint,
            healthMultiplier = healthMult,
            speedMultiplier = speedMult,
            damageMultiplier = damageMult,
            sizeMultiplier = sizeMultiplier,
            category = category,
            threatScore = threatScore
        };
    }

    static bool ShouldSpawnElite(float normalizedWave, float eliteBudgetRatio, float difficultyScalar)
    {
        float baseChance = _config != null
            ? _config.EvaluateEliteChance(normalizedWave)
            : Mathf.Lerp(0.05f, 0.25f, normalizedWave);

        baseChance += Mathf.Lerp(-0.05f, 0.15f, difficultyScalar);
        baseChance += Mathf.Clamp01(eliteBudgetRatio) * 0.35f;

        return Random.value < Mathf.Clamp01(baseChance);
    }

    static bool ShouldSpawnMiniBoss(float normalizedWave, float miniBossBudgetRatio, float difficultyScalar)
    {
        float baseChance = _config != null
            ? _config.EvaluateMiniBossChance(normalizedWave)
            : Mathf.Lerp(0f, 0.12f, normalizedWave);

        baseChance += Mathf.Lerp(-0.03f, 0.1f, difficultyScalar);
        baseChance += Mathf.Clamp01(miniBossBudgetRatio) * 0.5f;

        return Random.value < Mathf.Clamp01(baseChance);
    }

    static void ApplyCategoryBoosts(VariantCategory category, ref float health, ref float speed, ref float damage)
    {
        if (category == VariantCategory.Normal)
        {
            return;
        }

        if (_config != null)
        {
            if (category == VariantCategory.Elite)
            {
                health *= _config.EliteHealthMultiplier;
                speed *= _config.EliteSpeedMultiplier;
                damage *= _config.EliteDamageMultiplier;
            }
            else
            {
                health *= _config.MiniBossHealthMultiplier;
                speed *= _config.MiniBossSpeedMultiplier;
                damage *= _config.MiniBossDamageMultiplier;
            }
            return;
        }

        if (category == VariantCategory.Elite)
        {
            health *= 1.6f;
            speed *= 1.1f;
            damage *= 1.4f;
        }
        else if (category == VariantCategory.MiniBoss)
        {
            health *= 3f;
            speed *= 0.85f;
            damage *= 2f;
        }
    }

    static Color EvaluateTint(float scale, float normalizedWave, VariantCategory category)
    {
        Gradient gradient = null;

        if (_config != null)
        {
            switch (category)
            {
                case VariantCategory.Elite:
                    gradient = _config.EliteTintGradient;
                    break;
                case VariantCategory.MiniBoss:
                    gradient = _config.MiniBossTintGradient;
                    break;
                default:
                    gradient = _config.TintGradient;
                    break;
            }
        }

        if (gradient != null)
        {
            float sample = category == VariantCategory.Normal
                ? Mathf.Clamp01((scale - 1f) / 0.5f)
                : Mathf.Clamp01(normalizedWave);
            return gradient.Evaluate(sample);
        }

        if (category == VariantCategory.MiniBoss)
        {
            return new Color(0.85f, 0.25f, 0.85f);
        }

        if (category == VariantCategory.Elite)
        {
            return Color.Lerp(Color.yellow, Color.red, normalizedWave);
        }

        float hue = Mathf.Repeat(scale * 0.15f, 1f);
        return Color.HSVToRGB(hue, 0.4f, 1f);
    }

    static float ComputeThreatScore(float healthMult, float speedMult, float damageMult, VariantCategory category)
    {
        float weighted = healthMult * 0.5f + damageMult * 0.3f + speedMult * 0.2f;
        if (category == VariantCategory.Elite)
        {
            weighted *= 1.2f;
        }
        else if (category == VariantCategory.MiniBoss)
        {
            weighted *= 1.5f;
        }
        return Mathf.Max(0.1f, weighted);
    }

    static float EvaluateSizeMultiplier(float healthMult, VariantCategory category)
    {
        float size = Mathf.Clamp(healthMult, 0.75f, 1.35f);

        if (category == VariantCategory.Elite)
        {
            size *= 1.2f;
        }
        else if (category == VariantCategory.MiniBoss)
        {
            size *= 1.5f;
        }

        return Mathf.Clamp(size, 0.75f, 1.8f);
    }

    /// <summary>
    /// Enhanced variant creation using DifficultyScaling system.
    /// Combines procedural variety with progressive difficulty.
    /// </summary>
    public static Variant CreateVariantWithDifficulty(
        AttackerTypeDefinition baseType,
        int wave,
        int defenderUpgradeLevel,
        VariantRequest request = default)
    {
        // Apply base difficulty scaling from DifficultyScaling system
        float difficultyMultiplier = DifficultyScaling.GetDifficultyMultiplier(wave);
        
        // Create base variant
        Variant variant = CreateVariant(baseType, wave, defenderUpgradeLevel, request);
        
        // Apply additional DifficultyScaling enhancements
        float sizeFromDifficulty = DifficultyScaling.ScaleSize(1.0f, wave);
        variant.sizeMultiplier = Mathf.Clamp(variant.sizeMultiplier * sizeFromDifficulty, 0.8f, 1.4f);
        
        // Blend DifficultyScaling tint with procedural tint
        Color difficultyTint = DifficultyScaling.GetDifficultyTint(wave);
        variant.tint = Color.Lerp(variant.tint, difficultyTint, 0.3f);
        
        return variant;
    }
}
