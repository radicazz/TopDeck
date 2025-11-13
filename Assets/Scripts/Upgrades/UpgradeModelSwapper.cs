using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Toggles between predefined model variants based on defender/tower upgrade levels.
/// </summary>
[DisallowMultipleComponent]
public class UpgradeModelSwapper : MonoBehaviour
{
    private enum UpgradeTrack
    {
        Defender,
        Tower
    }

    [SerializeField] private UpgradeTrack track = UpgradeTrack.Defender;
    [SerializeField] private List<Transform> variantRoots = new List<Transform>();
    [SerializeField] private bool disableInactiveVariants = true;

    private int appliedLevel = -1;
    private Renderer activeRenderer;

    public Renderer GetActiveRenderer()
    {
        return activeRenderer;
    }

    void Awake()
    {
        RefreshActiveVariant(force: true);
    }

    void OnEnable()
    {
        RefreshActiveVariant(force: true);
    }

    void Update()
    {
        RefreshActiveVariant(force: false);
    }

    void RefreshActiveVariant(bool force)
    {
        if (variantRoots == null || variantRoots.Count == 0)
        {
            if (activeRenderer == null)
            {
                activeRenderer = GetComponentInChildren<Renderer>(includeInactive: false);
            }
            return;
        }

        int level = Mathf.Clamp(GetCurrentLevel(), 0, variantRoots.Count - 1);
        if (!force && level == appliedLevel)
        {
            return;
        }

        appliedLevel = level;

        for (int i = 0; i < variantRoots.Count; i++)
        {
            Transform variant = variantRoots[i];
            if (variant == null)
            {
                continue;
            }

            bool shouldEnable = i == level;
            GameObject variantObject = variant.gameObject;

            if (shouldEnable && !variantObject.activeSelf)
            {
                variantObject.SetActive(true);
            }
            else if (!shouldEnable && disableInactiveVariants && variantObject.activeSelf)
            {
                variantObject.SetActive(false);
            }

            if (shouldEnable)
            {
                activeRenderer = variant.GetComponentInChildren<Renderer>(includeInactive: true);
            }
        }
    }

    int GetCurrentLevel()
    {
        if (UpgradeManager.Instance == null)
        {
            return 0;
        }

        return track == UpgradeTrack.Tower
            ? UpgradeManager.Instance.GetTowerLevel()
            : UpgradeManager.Instance.GetDefenderLevel();
    }
}
