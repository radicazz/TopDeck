using UnityEngine;

/// <summary>
/// Applies upgrade-derived stat changes to a defender (DefenseController) and optional health component.
/// </summary>
[RequireComponent(typeof(DefenseController))]
public class DefenderUpgrade : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Renderer rendererRef;
    [SerializeField] private Material upgradedMaterial;

    [Header("Health (optional)")]
    [SerializeField] private int baseHealth = 100;

    private DefenseController defense;
    private int appliedLevel = -1;
    private bool hasBaseStats = false;
    private int baseDamage = 1;
    private float baseFireRate = 1f;

    void Awake()
    {
        defense = GetComponent<DefenseController>();
        EnsureRendererReference();
    }

    void Start()
    {
        ApplyUpgradesIfNeeded();
    }

    void OnValidate()
    {
        EnsureRendererReference();
    }

    void Update()
    {
        // In case upgrades change at runtime
        ApplyUpgradesIfNeeded();
    }

    void ApplyUpgradesIfNeeded()
    {
        if (UpgradeManager.Instance == null)
        {
            return;
        }

        int level = UpgradeManager.Instance.GetDefenderLevel();
        if (level == appliedLevel)
        {
            return;
        }

        appliedLevel = level;

        // Apply stat multipliers
        float dmgMul = UpgradeManager.Instance.GetDefenderDamageMultiplier();
        float rateMul = UpgradeManager.Instance.GetDefenderFireRateMultiplier();

        // Modify DefenseController exposed stats
        // Note: DefenseController uses serialized private fields; expose via helper where needed
        ApplyToDefenseController(dmgMul, rateMul);

        // Apply health bonus if a simple Health component is present
        var health = GetComponent<SimpleHealth>();
        if (health == null)
        {
            health = gameObject.AddComponent<SimpleHealth>();
            health.Initialize(baseHealth);
        }

        int bonus = UpgradeManager.Instance.GetDefenderHealthBonus();
        health.SetMaxHealth(baseHealth + bonus, true);

        // Update visuals
        if (rendererRef != null && upgradedMaterial != null && level > 0)
        {
            rendererRef.sharedMaterial = upgradedMaterial;
        }
    }

    void EnsureRendererReference()
    {
        if (rendererRef != null)
        {
            return;
        }

        rendererRef = GetComponentInChildren<Renderer>(includeInactive: true);
    }

    void ApplyToDefenseController(float damageMultiplier, float fireRateMultiplier)
    {
        if (defense == null)
        {
            return;
        }

        // Use reflection to adjust serialized private fields without changing DefenseController's API
        var dmgField = typeof(DefenseController).GetField("damage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var rateField = typeof(DefenseController).GetField("fireRate", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (!hasBaseStats)
        {
            if (dmgField != null)
            {
                baseDamage = Mathf.Max(1, (int)dmgField.GetValue(defense));
            }
            if (rateField != null)
            {
                baseFireRate = Mathf.Max(0.01f, (float)rateField.GetValue(defense));
            }
            hasBaseStats = true;
        }

        if (dmgField != null)
        {
            int newDamage = Mathf.Max(1, Mathf.RoundToInt(baseDamage * damageMultiplier));
            dmgField.SetValue(defense, newDamage);
        }

        if (rateField != null)
        {
            float newRate = Mathf.Max(0.01f, baseFireRate * fireRateMultiplier);
            rateField.SetValue(defense, newRate);
        }
    }
}
