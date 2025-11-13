using System;

/// <summary>
/// Minimal contract for any entity that exposes mutable health values.
/// </summary>
public interface IHealthComponent
{
    /// <summary>
    /// Invoked whenever the current or max health changes.
    /// </summary>
    event Action<int, int> OnHealthChanged;

    /// <summary>
    /// Invoked when health reaches zero (fires once per death sequence).
    /// </summary>
    event Action OnDeath;

    /// <summary>
    /// Invoked when an entity transitions from dead to alive.
    /// </summary>
    event Action OnRevive;

    /// <summary>
    /// Maximum health for the entity.
    /// </summary>
    int MaxHealth { get; }

    /// <summary>
    /// Current health value (clamped between 0 and MaxHealth).
    /// </summary>
    int CurrentHealth { get; }

    /// <summary>
    /// True while health is above zero.
    /// </summary>
    bool IsAlive { get; }

    /// <summary>
    /// Initializes the component with a starting max health value.
    /// </summary>
    /// <param name="maxHealth">Desired maximum health.</param>
    void Initialize(int maxHealth);

    /// <summary>
    /// Updates the maximum health. Optionally heals to full.
    /// </summary>
    /// <param name="newMax">New max health.</param>
    /// <param name="healToFull">If true, current health becomes the new max.</param>
    void SetMaxHealth(int newMax, bool healToFull);

    /// <summary>
    /// Applies damage, reducing current health but never going below zero.
    /// </summary>
    /// <param name="amount">Damage amount (negative values ignored).</param>
    void TakeDamage(int amount);

    /// <summary>
    /// Restores health, clamped at the current max value.
    /// </summary>
    /// <param name="amount">Healing amount (negative values ignored).</param>
    void Heal(int amount);

    /// <summary>
    /// Forces health to zero and raises the death events if needed.
    /// </summary>
    void Kill();

    /// <summary>
    /// Restores the entity to life using the supplied health value.
    /// </summary>
    /// <param name="healthAmount">Health to restore with.</param>
    void Revive(int healthAmount);
}
