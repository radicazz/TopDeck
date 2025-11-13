using UnityEngine;

/// <summary>
/// Manages tower placement and tracking
/// </summary>
public class TowerManager : MonoBehaviour
{
    [SerializeField] private GameObject _towerPrefab;
    
    public bool TryPlaceTower(Vector3 position)
    {
        if (_towerPrefab == null)
        {
            Debug.LogWarning("[TowerManager] No tower prefab assigned");
            return false;
        }
        
        GameObject tower = Instantiate(_towerPrefab, position, Quaternion.identity);
        if (tower != null)
        {
            Debug.Log($"[TowerManager] Placed tower at {position}");
            return true;
        }
        
        return false;
    }
}
