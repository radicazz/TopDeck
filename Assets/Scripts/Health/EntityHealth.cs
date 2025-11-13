using System;
using UnityEngine;

/// <summary>
/// Concrete health implementation that exposes change/death events and clamps all math.
/// </summary>
[DisallowMultipleComponent]
public class EntityHealth : MonoBehaviour, IHealthComponent
{
    [SerializeField] private int _baseMaxHealth = 100;
    [SerializeField] private bool _initializeOnAwake = true;

    private int _maxHealth;
    private int _currentHealth;
    private bool _initialized;
    private bool _isDead;

    public event Action<int, int> OnHealthChanged;
    public event Action OnDeath;
    public event Action OnRevive;

    public int MaxHealth => Mathf.Max(1, _maxHealth);
    public int CurrentHealth => Mathf.Clamp(_currentHealth, 0, MaxHealth);
    public bool IsAlive => CurrentHealth > 0;

    void Awake()
    {
        if (_initializeOnAwake)
        {
            Initialize(_baseMaxHealth);
        }
    }

    /// <inheritdoc />
    public void Initialize(int maxHealth)
    {
        _maxHealth = Mathf.Max(1, maxHealth);
        _currentHealth = _maxHealth;
        _initialized = true;
        _isDead = false;
        RaiseHealthChanged();
    }

    /// <inheritdoc />
    public void SetMaxHealth(int newMax, bool healToFull)
    {
        EnsureInitialized();
        _maxHealth = Mathf.Max(1, newMax);

        if (healToFull)
        {
            _currentHealth = _maxHealth;
        }
        else
        {
            _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        }

        RaiseHealthChanged();
        UpdateLifeState();
    }

    /// <inheritdoc />
    public void TakeDamage(int amount)
    {
        EnsureInitialized();
        if (amount <= 0 || _isDead)
        {
            return;
        }

        _currentHealth = Mathf.Max(0, _currentHealth - amount);
        RaiseHealthChanged();
        UpdateLifeState();
    }

    /// <inheritdoc />
    public void Heal(int amount)
    {
        EnsureInitialized();
        if (amount <= 0)
        {
            return;
        }

        int previous = _currentHealth;
        _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);

        if (_currentHealth == previous)
        {
            return;
        }

        RaiseHealthChanged();
        UpdateLifeState();
    }

    /// <inheritdoc />
    public void Kill()
    {
        EnsureInitialized();
        if (_isDead)
        {
            return;
        }

        _currentHealth = 0;
        RaiseHealthChanged();
        UpdateLifeState();
    }

    /// <inheritdoc />
    public void Revive(int healthAmount)
    {
        EnsureInitialized();
        int revivedHealth = Mathf.Clamp(healthAmount, 1, _maxHealth);
        _currentHealth = revivedHealth;
        RaiseHealthChanged();
        UpdateLifeState();
    }

    private void EnsureInitialized()
    {
        if (_initialized)
        {
            return;
        }

        Initialize(_baseMaxHealth);
    }

    private void RaiseHealthChanged()
    {
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    private void UpdateLifeState()
    {
        bool currentlyDead = _currentHealth <= 0;
        if (currentlyDead && !_isDead)
        {
            _isDead = true;
            OnDeath?.Invoke();
        }
        else if (!currentlyDead && _isDead)
        {
            _isDead = false;
            OnRevive?.Invoke();
        }
    }
}
