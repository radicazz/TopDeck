using UnityEngine;

/// <summary>
/// Centralized upgrade state for defenders and the central tower.
/// Keeps simple level-based modifiers and exposes helper accessors.
/// </summary>
public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Header("Defender Upgrades")]
    [SerializeField] private int defenderLevel = 0; // 0..2
    [SerializeField] private int defenderMaxLevel = 2;
    [SerializeField] private int defenderHealthBonusPerLevel = 25;
    [SerializeField] private float defenderDamageMultiplierPerLevel = 0.1f; // +10%/lvl
    [SerializeField] private float defenderFireRateMultiplierPerLevel = 0.1f; // +10%/lvl

    [Header("Tower Upgrades")]
    [SerializeField] private int towerLevel = 0; // 0..2
    [SerializeField] private int towerMaxLevel = 2;
    [SerializeField] private int towerHealthBonusPerLevel = 50;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Public API
    public int GetDefenderLevel()
    {
        return Mathf.Clamp(defenderLevel, 0, defenderMaxLevel);
    }

    public int GetDefenderMaxLevel()
    {
        return Mathf.Max(1, defenderMaxLevel);
    }

    public bool CanUpgradeDefender()
    {
        return defenderLevel < defenderMaxLevel;
    }

    public void UpgradeDefenders()
    {
        if (CanUpgradeDefender())
        {
            defenderLevel++;
        }
    }

    public int GetTowerLevel()
    {
        return Mathf.Clamp(towerLevel, 0, towerMaxLevel);
    }

    public int GetTowerMaxLevel()
    {
        return Mathf.Max(1, towerMaxLevel);
    }

    public bool CanUpgradeTower()
    {
        return towerLevel < towerMaxLevel;
    }

    public void UpgradeTower()
    {
        if (CanUpgradeTower())
        {
            towerLevel++;
        }
    }

    // Modifiers
    public int GetDefenderHealthBonus()
    {
        return GetDefenderLevel() * Mathf.Max(0, defenderHealthBonusPerLevel);
    }

    public float GetDefenderDamageMultiplier()
    {
        return 1f + GetDefenderLevel() * Mathf.Max(0f, defenderDamageMultiplierPerLevel);
    }

    public float GetDefenderFireRateMultiplier()
    {
        return 1f + GetDefenderLevel() * Mathf.Max(0f, defenderFireRateMultiplierPerLevel);
    }

    public int GetTowerHealthBonus()
    {
        return GetTowerLevel() * Mathf.Max(0, towerHealthBonusPerLevel);
    }
}
