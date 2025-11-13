using UnityEngine;

/// <summary>
/// Coordinates wave spawning with difficulty balancing.
/// Simplified version - removed WaveSpawner dependency
/// </summary>
public class WaveDirector : MonoBehaviour
{
    [SerializeField] private float _baseEnemyHealth = 10f;
    [SerializeField] private float _healthScalingPerLevel = 0.15f;

    void OnEnable()
    {
        if (UpgradeSystem.Instance != null)
        {
            UpgradeSystem.Instance.OnTowerUpgrade += OnTowerUpgraded;
        }
    }

    void OnDisable()
    {
        if (UpgradeSystem.Instance != null)
        {
            UpgradeSystem.Instance.OnTowerUpgrade -= OnTowerUpgraded;
        }
    }

    void OnTowerUpgraded(int level)
    {
        // Placeholder for dynamic difficulty adjustment
        float scaling = 1f + _healthScalingPerLevel * (level - 1);
        Debug.Log($"[WaveDirector] Tower upgraded to level {level}, enemy scaling: {scaling:F2}x");
    }

    public float GetEnemyHealthScaling()
    {
        int towerLevel = UpgradeSystem.Instance != null ? UpgradeSystem.Instance.TowerLevel : 1;
        return 1f + _healthScalingPerLevel * (towerLevel - 1);
    }
}
