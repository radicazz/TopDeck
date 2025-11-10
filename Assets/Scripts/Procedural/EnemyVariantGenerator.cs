using UnityEngine;

/// <summary>
/// Creates procedural enemy variants by adjusting stats per spawn and applying a visual tint.
/// Optionally uses ProceduralVariantConfig for designer-tunable curves.
/// </summary>
public static class EnemyVariantGenerator
{
    public struct Variant
    {
        public AttackerTypeDefinition definition;
        public Color tint;
        public float healthMultiplier;
        public float speedMultiplier;
        public float damageMultiplier;
    }

    private static ProceduralVariantConfig _config;

    public static void SetConfig(ProceduralVariantConfig config)
    {
        _config = config;
    }

    public static Variant CreateVariant(AttackerTypeDefinition baseType, int wave, int defenderUpgradeLevel)
    {
        float waveFactor, counterFactor;
        
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
        
        float scale = waveFactor * counterFactor;

        int baseHealth = baseType != null ? baseType.BaseHealth : 100;
        float moveSpeed = baseType != null ? baseType.MoveSpeed : 3f;
        float towerDamage = baseType != null ? baseType.TowerDamage : 20f;
        float attackRange = baseType != null ? baseType.AttackRange : 1.5f;
        float attackRate = baseType != null ? baseType.AttackRate : 1f;
        int damageToPlayer = baseType != null ? baseType.DamageToPlayer : 10;

        float healthMult, speedMult, damageMult;
        
        if (_config != null)
        {
            float normalizedWave = Mathf.Clamp01((wave - 1f) / 10f);
            healthMult = _config.EvaluateHealth(normalizedWave) * scale;
            speedMult = _config.EvaluateSpeed(normalizedWave);
            damageMult = _config.EvaluateDamage(normalizedWave) * scale;
        }
        else
        {
            healthMult = scale * Random.Range(0.9f, 1.2f);
            speedMult = Random.Range(0.9f, 1.15f);
            damageMult = Random.Range(0.9f, 1.15f);
        }

        int healthV = Mathf.RoundToInt(baseHealth * healthMult);
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

        Color tint;
        if (_config != null && _config.TintGradient != null)
        {
            float gradientPos = Mathf.Clamp01((scale - 1f) / 0.5f);
            tint = _config.TintGradient.Evaluate(gradientPos);
        }
        else
        {
            float hue = Mathf.Repeat(scale * 0.15f, 1f);
            tint = Color.HSVToRGB(hue, 0.4f, 1f);
        }

        return new Variant 
        { 
            definition = variantType, 
            tint = tint,
            healthMultiplier = healthMult,
            speedMultiplier = speedMult,
            damageMultiplier = damageMult
        };
    }
}
