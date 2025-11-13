using UnityEngine;

/// <summary>
/// Feeds upgrade/tower state into a renderer via MaterialPropertyBlock for shader-driven visuals.
/// </summary>
[ExecuteAlways]
public class UpgradeVisualShaderDriver : MonoBehaviour
{
    private enum ValueSource
    {
        DefenderLevel,
        TowerLevel
    }

    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private UpgradeModelSwapper modelSwapper;
    [SerializeField] private ValueSource valueSource = ValueSource.DefenderLevel;
    [SerializeField] private string floatProperty = "_UpgradeLevel";
    [SerializeField] private string colorProperty = "_UpgradeColor";
    [SerializeField] private Gradient colorByLevel = new Gradient();

    private MaterialPropertyBlock propertyBlock;

    void Awake()
    {
        EnsureRenderer();
        EnsurePropertyBlock();
    }

    void OnEnable()
    {
        EnsurePropertyBlock();
        ApplyProperties();
    }

    void OnValidate()
    {
        EnsureRenderer();
    }

    void Update()
    {
        ApplyProperties();
    }

    void EnsureRenderer()
    {
        if (targetRenderer != null && targetRenderer.gameObject.activeInHierarchy)
        {
            return;
        }

        if (modelSwapper == null)
        {
            modelSwapper = GetComponent<UpgradeModelSwapper>();
        }

        if (modelSwapper != null)
        {
            var candidate = modelSwapper.GetActiveRenderer();
            if (candidate != null)
            {
                targetRenderer = candidate;
                return;
            }
        }

        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }

        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<Renderer>(includeInactive: true);
        }
    }

    void EnsurePropertyBlock()
    {
        if (propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }
    }

    void ApplyProperties()
    {
        EnsureRenderer();

        if (targetRenderer == null || UpgradeManager.Instance == null)
        {
            return;
        }

        EnsurePropertyBlock();
        targetRenderer.GetPropertyBlock(propertyBlock);

        float normalized = 0f;
        switch (valueSource)
        {
            case ValueSource.TowerLevel:
                normalized = UpgradeManager.Instance.GetTowerMaxLevel() > 0
                    ? UpgradeManager.Instance.GetTowerLevel() / (float)UpgradeManager.Instance.GetTowerMaxLevel()
                    : 0f;
                break;
            default:
                normalized = UpgradeManager.Instance.GetDefenderMaxLevel() > 0
                    ? UpgradeManager.Instance.GetDefenderLevel() / (float)UpgradeManager.Instance.GetDefenderMaxLevel()
                    : 0f;
                break;
        }

        if (!string.IsNullOrEmpty(floatProperty))
        {
            propertyBlock.SetFloat(floatProperty, normalized);
        }

        if (!string.IsNullOrEmpty(colorProperty))
        {
            Color color = colorByLevel.Evaluate(Mathf.Clamp01(normalized));
            propertyBlock.SetColor(colorProperty, color);
        }

        targetRenderer.SetPropertyBlock(propertyBlock);
    }
}
