using UnityEngine;

/// <summary>
/// Creates procedural enemy variants by adjusting stats per spawn and applying a visual tint.
/// </summary>
public static class EnemyVariantGenerator
{
    public struct Variant
    {
        public AttackerTypeDefinition type;
        public Color tint;
    }

    public static Variant CreateVariant(AttackerTypeDefinition baseType, int wave, int defenderUpgradeLevel)
    {
        // Scale factors: slightly harder variants at higher waves and if player has upgrades
        float waveFactor = 1f + 0.05f * Mathf.Max(0, wave - 1);
        float counterFactor = 1f + 0.03f * Mathf.Max(0, defenderUpgradeLevel);
        float scale = waveFactor * counterFactor;

        int baseHealth = baseType != null ? baseType.BaseHealth : 100;
        float moveSpeed = baseType != null ? baseType.MoveSpeed : 3f;
        float towerDamage = baseType != null ? baseType.TowerDamage : 20f;
        float attackRange = baseType != null ? baseType.AttackRange : 1.5f;
        float attackRate = baseType != null ? baseType.AttackRate : 1f;
        int damageToPlayer = baseType != null ? baseType.DamageToPlayer : 10;

        // Randomize within bands
        int healthV = Mathf.RoundToInt(baseHealth * scale * Random.Range(0.9f, 1.2f));
        float speedV = Mathf.Max(0.5f, moveSpeed * Random.Range(0.9f, 1.15f));
        float towerDmgV = Mathf.Max(0f, towerDamage * Random.Range(0.9f, 1.15f));
        float rangeV = Mathf.Max(0.5f, attackRange * Random.Range(0.95f, 1.1f));
        float rateV = Mathf.Max(0.1f, attackRate * Random.Range(0.95f, 1.1f));
        int dmgPlayerV = Mathf.Max(1, Mathf.RoundToInt(damageToPlayer * Random.Range(0.9f, 1.1f)));

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

        // Visual tint based on scale
        float hue = Mathf.Repeat(scale * 0.15f, 1f);
        Color tint = Color.HSVToRGB(hue, 0.4f, 1f);

        return new Variant { type = variantType, tint = tint };
    }
}

