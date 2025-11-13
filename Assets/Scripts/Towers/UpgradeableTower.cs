using UnityEngine;

/// <summary>
/// Applies visual and stat upgrades to tower based on UpgradeSystem level.
/// Simplified version - removed SimpleTower dependency
/// </summary>
public class UpgradeableTower : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _visual;
    [SerializeField] private Color[] _levelColors;
    [SerializeField] private float _healthMultiplierPerLevel = 0.2f;
    [SerializeField] private float _scaleIncreasePerLevel = 0.05f;

    private int _currentLevel = 1;

    void OnEnable()
    {
        if (UpgradeSystem.Instance != null)
        {
            UpgradeSystem.Instance.OnTowerUpgrade += ApplyUpgrade;
        }
    }

    void OnDisable()
    {
        if (UpgradeSystem.Instance != null)
        {
            UpgradeSystem.Instance.OnTowerUpgrade -= ApplyUpgrade;
        }
    }

    void Start()
    {
        int level = UpgradeSystem.Instance != null ? UpgradeSystem.Instance.TowerLevel : 1;
        ApplyUpgrade(level);
    }

    void ApplyUpgrade(int level)
    {
        _currentLevel = level;
        
        // Apply visual changes
        if (_visual != null && _levelColors != null && _levelColors.Length > 0)
        {
            int colorIndex = Mathf.Clamp(level - 1, 0, _levelColors.Length - 1);
            _visual.color = _levelColors[colorIndex];
            
            float scale = 1f + _scaleIncreasePerLevel * (level - 1);
            transform.localScale = Vector3.one * scale;
        }
        
        Debug.Log($"[UpgradeableTower] Upgraded to level {level}");
    }

    public float GetHealthMultiplier()
    {
        return 1f + _healthMultiplierPerLevel * (_currentLevel - 1);
    }
}
