using UnityEngine;

/// <summary>
/// Manages theme colors for UI elements
/// </summary>
public class ThemeManager : MonoBehaviour
{
    public static ThemeManager Instance { get; private set; }
    
    [SerializeField] private Color _healthyColor = Color.green;
    [SerializeField] private Color _damagedColor = Color.yellow;
    [SerializeField] private Color _criticalColor = Color.red;
    [SerializeField] private ThemePalette _activePalette;
    
    public ThemePalette ActivePalette => _activePalette;
    
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
    
    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    public Color GetHealthColor(float healthPercent)
    {
        if (healthPercent > 0.6f)
        {
            return _healthyColor;
        }
        else if (healthPercent > 0.3f)
        {
            return Color.Lerp(_criticalColor, _damagedColor, (healthPercent - 0.3f) / 0.3f);
        }
        else
        {
            return _criticalColor;
        }
    }
}

[System.Serializable]
public class ThemePalette
{
    public Color primaryColor = Color.white;
    public Color secondaryColor = Color.gray;
    public Color accentColor = Color.blue;
}
