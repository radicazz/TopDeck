using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Component that allows an enemy to deal AoE (Area of Effect) damage to nearby towers.
/// Continuously damages all towers within a specified radius.
/// </summary>
[RequireComponent(typeof(AttackerController))]
public class AoEDamageController : MonoBehaviour
{
    [Header("AoE Configuration")]
    [SerializeField] private float _aoeRadius = 2f;
    [SerializeField] private float _damagePerSecond = 5f;
    [SerializeField] private float _damageTickRate = 0.5f; // How often to apply damage
    [SerializeField] private bool _onlyDamageWhileAlive = true;

    [Header("Visual Feedback")]
    [SerializeField] private bool _showDebugRadius = true;
    [SerializeField] private Color _debugRadiusColor = new Color(1f, 0.5f, 0f, 0.3f);

    [Header("Performance")]
    [SerializeField] private int _maxTargetsPerTick = 5;

    private AttackerController _attacker;
    private float _lastDamageTime;
    private List<DefenseController> _cachedTargets = new List<DefenseController>();
    private bool _isActive = true;

    void Awake()
    {
        _attacker = GetComponent<AttackerController>();
        if (_attacker == null)
        {
            Debug.LogError($"AoEDamageController on {gameObject.name} requires AttackerController component!");
            enabled = false;
        }
    }

    void Start()
    {
        _lastDamageTime = Time.time;
    }

    void Update()
    {
        if (!_isActive) return;

        if (_onlyDamageWhileAlive && (_attacker == null || !_attacker.IsAlive()))
        {
            return;
        }

        if (Time.time >= _lastDamageTime + _damageTickRate)
        {
            ApplyAoEDamage();
            _lastDamageTime = Time.time;
        }
    }

    void ApplyAoEDamage()
    {
        // Find all towers within radius
        FindTowersInRadius();

        if (_cachedTargets.Count == 0) return;

        // Calculate damage for this tick
        float damageThisTick = _damagePerSecond * _damageTickRate;

        // Apply damage to all targets (up to max)
        int damageCount = Mathf.Min(_cachedTargets.Count, _maxTargetsPerTick);
        for (int i = 0; i < damageCount; i++)
        {
            DefenseController tower = _cachedTargets[i];
            if (tower != null && tower.gameObject.activeInHierarchy)
            {
                // TODO: When DefenseController gets health system, call tower.TakeDamage(damageThisTick)
                // For now, just log the damage
                Debug.Log($"{gameObject.name} deals {damageThisTick:F1} AoE damage to tower {tower.gameObject.name}");
            }
        }
    }

    void FindTowersInRadius()
    {
        _cachedTargets.Clear();

        Collider[] colliders = Physics.OverlapSphere(transform.position, _aoeRadius);

        foreach (Collider col in colliders)
        {
            DefenseController tower = col.GetComponent<DefenseController>();
            if (tower != null && !_cachedTargets.Contains(tower))
            {
                _cachedTargets.Add(tower);
            }
        }
    }

    /// <summary>
    /// Sets the AoE damage parameters dynamically.
    /// </summary>
    public void SetAoEParameters(float radius, float damagePerSecond, float tickRate = 0.5f)
    {
        _aoeRadius = Mathf.Max(0.5f, radius);
        _damagePerSecond = Mathf.Max(0f, damagePerSecond);
        _damageTickRate = Mathf.Max(0.1f, tickRate);
    }

    /// <summary>
    /// Enables or disables AoE damage.
    /// </summary>
    public void SetActive(bool active)
    {
        _isActive = active;
    }

    /// <summary>
    /// Gets the current AoE radius.
    /// </summary>
    public float GetAoERadius() => _aoeRadius;

    /// <summary>
    /// Gets the current damage per second.
    /// </summary>
    public float GetDamagePerSecond() => _damagePerSecond;

    /// <summary>
    /// Gets the number of towers currently in range.
    /// </summary>
    public int GetTargetCount()
    {
        FindTowersInRadius();
        return _cachedTargets.Count;
    }

    void OnDrawGizmosSelected()
    {
        if (!_showDebugRadius) return;

        // Draw AoE radius
        Gizmos.color = _debugRadiusColor;
        Gizmos.DrawWireSphere(transform.position, _aoeRadius);

        // Draw filled sphere for better visibility
        Color fillColor = _debugRadiusColor;
        fillColor.a = 0.1f;
        Gizmos.color = fillColor;
        Gizmos.DrawSphere(transform.position, _aoeRadius);

        // Draw lines to all towers in range
        if (Application.isPlaying && _cachedTargets != null)
        {
            Gizmos.color = Color.red;
            foreach (var tower in _cachedTargets)
            {
                if (tower != null)
                {
                    Gizmos.DrawLine(transform.position, tower.transform.position);
                }
            }
        }
    }

    void OnDestroy()
    {
        _cachedTargets.Clear();
    }
}
