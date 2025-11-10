using UnityEngine;

/// <summary>
/// Minimal health component for defenders or other objects.
/// </summary>
public class SimpleHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

    public void Initialize(int baseHealth)
    {
        maxHealth = Mathf.Max(1, baseHealth);
        currentHealth = maxHealth;
    }

    public void SetMaxHealth(int newMax, bool healToFull)
    {
        maxHealth = Mathf.Max(1, newMax);
        if (healToFull)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - Mathf.Max(0, amount));
        if (currentHealth == 0)
        {
            Destroy(gameObject);
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + Mathf.Max(0, amount));
    }
}

