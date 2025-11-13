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
    [SerializeField] private UpgradeModelSwapper modelSwapper;

    [Header("Health (optional)")]
    [SerializeField] private int baseHealth = 100;

    [Header("Selection")]
    [SerializeField] private string displayName = "Tower";
    [SerializeField] private float selectionRingRadius = 1.5f;
    [SerializeField] private float selectionRingHeight = 0.075f;
    [SerializeField] private Color selectionRingColor = new Color(0.2f, 0.85f, 1f, 0.85f);

    private DefenseController defense;
    private IHealthComponent healthComponent;
    private int appliedLevel = -1;
    private bool hasBaseStats = false;
    private int baseDamage = 1;
    private float baseFireRate = 1f;
    private GameObject selectionIndicatorRoot;
    private LineRenderer selectionIndicatorRenderer;

void Awake()
    {
        defense = GetComponent<DefenseController>();
        healthComponent = GetComponent<IHealthComponent>();
        if (modelSwapper == null)
        {
            modelSwapper = GetComponent<UpgradeModelSwapper>();
        }
        EnsureRendererReference();
        EnsureCollider();
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

        RefreshRendererReference();

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

        // Apply health bonus via shared health abstraction
        var health = EnsureHealthComponent();
        if (health != null)
        {
            int bonus = UpgradeManager.Instance.GetDefenderHealthBonus();
            health.SetMaxHealth(baseHealth + bonus, true);
        }

        // Update visuals
        if (rendererRef != null && upgradedMaterial != null && level > 0)
        {
            rendererRef.sharedMaterial = upgradedMaterial;
        }
    }

    public string DisplayName
    {
        get { return string.IsNullOrEmpty(displayName) ? gameObject.name : displayName; }
    }

    public int GetCurrentUpgradeLevel()
    {
        return Mathf.Max(0, appliedLevel);
    }

    public int GetMaxUpgradeLevel()
    {
        return UpgradeManager.Instance != null ? UpgradeManager.Instance.GetDefenderMaxLevel() : 1;
    }

    public void RefreshUpgradeState()
    {
        ApplyUpgradesIfNeeded();
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            EnsureSelectionIndicator();
        }

        if (selectionIndicatorRoot != null)
        {
            selectionIndicatorRoot.SetActive(selected);
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

    void RefreshRendererReference()
    {
        if (modelSwapper == null)
        {
            modelSwapper = GetComponent<UpgradeModelSwapper>();
        }

        if (modelSwapper != null)
        {
            var candidate = modelSwapper.GetActiveRenderer();
            if (candidate != null)
            {
                rendererRef = candidate;
                return;
            }
        }

        if (rendererRef == null || !rendererRef.gameObject.activeInHierarchy)
        {
            var fallback = GetComponentInChildren<Renderer>(includeInactive: false);
            if (fallback != null)
            {
                rendererRef = fallback;
            }
        }
    }

    void EnsureSelectionIndicator()
    {
        if (selectionIndicatorRoot != null)
        {
            return;
        }

        selectionIndicatorRoot = new GameObject("SelectionIndicator");
        selectionIndicatorRoot.transform.SetParent(transform, false);
        selectionIndicatorRoot.transform.localPosition = Vector3.zero;

        selectionIndicatorRenderer = selectionIndicatorRoot.AddComponent<LineRenderer>();
        selectionIndicatorRenderer.useWorldSpace = false;
        selectionIndicatorRenderer.loop = true;
        selectionIndicatorRenderer.widthMultiplier = 0.065f;
        selectionIndicatorRenderer.numCornerVertices = 4;
        selectionIndicatorRenderer.numCapVertices = 4;
        Shader spriteShader = Shader.Find("Sprites/Default");
        Material lineMaterial = spriteShader != null ? new Material(spriteShader) : new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended"));
        lineMaterial.hideFlags = HideFlags.DontSave;
        selectionIndicatorRenderer.material = lineMaterial;
        selectionIndicatorRenderer.startColor = selectionRingColor;
        selectionIndicatorRenderer.endColor = selectionRingColor;

        const int segments = 48;
        selectionIndicatorRenderer.positionCount = segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2f;
            float x = Mathf.Cos(angle) * selectionRingRadius;
            float z = Mathf.Sin(angle) * selectionRingRadius;
            selectionIndicatorRenderer.SetPosition(i, new Vector3(x, selectionRingHeight, z));
        }

        selectionIndicatorRoot.SetActive(false);
    }

void EnsureCollider()
    {
        if (GetComponent<Collider>() == null)
        {
            BoxCollider col = gameObject.AddComponent<BoxCollider>();
            col.center = new Vector3(0, 0.5f, 0);
            col.size = new Vector3(1, 1, 1);
        }
    }


    void ApplyToDefenseController(float damageMultiplier, float fireRateMultiplier)
    {
        if (defense == null)
        {
            return;
        }

        if (!hasBaseStats)
        {
            DefenseController.DefenseStatsSnapshot snapshot = defense.GetCurrentStats();
            baseDamage = Mathf.Max(1, snapshot.Damage);
            baseFireRate = Mathf.Max(0.01f, snapshot.FireRate);
            defense.SetBaseStats(baseDamage, baseFireRate);
            hasBaseStats = true;
        }

        defense.ApplyMultipliers(damageMultiplier, fireRateMultiplier);
    }

    IHealthComponent EnsureHealthComponent()
    {
        if (healthComponent != null)
        {
            return healthComponent;
        }

        healthComponent = GetComponent<IHealthComponent>();
        if (healthComponent != null)
        {
            return healthComponent;
        }

        EntityHealth entityHealth = gameObject.AddComponent<EntityHealth>();
        entityHealth.Initialize(Mathf.Max(1, baseHealth));
        healthComponent = entityHealth;
        return healthComponent;
    }
}
