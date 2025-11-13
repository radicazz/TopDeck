using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Binds a health component to a UI Slider for health bar display
/// </summary>
[DisallowMultipleComponent]
public class HealthBarBinder : MonoBehaviour
{
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Vector3 _worldOffset = Vector3.up * 2f;
    [SerializeField] private bool _hideOnDeath = true;

    private IHealthComponent _boundHealth;

    public void Bind(IHealthComponent healthComponent)
    {
        if (healthComponent == _boundHealth)
        {
            SyncImmediate();
            return;
        }

        UnbindInternal();

        if (healthComponent == null)
        {
            if (_healthSlider != null)
            {
                _healthSlider.gameObject.SetActive(false);
            }
            return;
        }

        _boundHealth = healthComponent;
        _boundHealth.OnHealthChanged += OnHealthChanged;
        _boundHealth.OnDeath += OnDeath;

        if (_healthSlider != null)
        {
            _healthSlider.gameObject.SetActive(true);
        }

        SyncImmediate();
    }

    void OnHealthChanged(int current, int max)
    {
        if (_healthSlider != null)
        {
            _healthSlider.maxValue = max;
            _healthSlider.value = current;
        }
    }

    void OnDeath()
    {
        if (_hideOnDeath && _healthSlider != null)
        {
            _healthSlider.gameObject.SetActive(false);
        }
    }

    void SyncImmediate()
    {
        if (_boundHealth != null)
        {
            OnHealthChanged(_boundHealth.CurrentHealth, _boundHealth.MaxHealth);
        }
    }

    void UnbindInternal()
    {
        if (_boundHealth != null)
        {
            _boundHealth.OnHealthChanged -= OnHealthChanged;
            _boundHealth.OnDeath -= OnDeath;
            _boundHealth = null;
        }
    }

    void OnDestroy()
    {
        UnbindInternal();
    }
}
