using UnityEngine;

/// <summary>
/// Central palette service for UI Toolkit, health bars, and other color-driven systems.
/// Loads a ColorPalette asset on startup so designers can tweak colors without touching code.
/// </summary>
public class ThemeManager : MonoBehaviour
{
    public static ThemeManager Instance { get; private set; }

    [Header("Palette Settings")]
    [SerializeField] private ColorPalette _activePalette;
    [SerializeField] private string _defaultPalettePath = "DefaultColorPalette";
    [SerializeField] private bool _dontDestroyOnLoad = true;
    [SerializeField] private bool _debugLogs = false;

    [Header("Fallback Colors (used if palette missing)")]
    [SerializeField] private Color _healthyColor = Color.green;
    [SerializeField] private Color _damagedColor = Color.yellow;
    [SerializeField] private Color _criticalColor = Color.red;

    public ColorPalette ActivePalette => _activePalette;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (_dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }

        EnsurePaletteLoaded();
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void OnValidate()
    {
        EnsurePaletteLoaded();
    }

    /// <summary>
    /// Returns a health color from the active palette (or fallbacks).
    /// </summary>
    public Color GetHealthColor(float healthPercent)
    {
        float clamped = Mathf.Clamp01(healthPercent);
        if (_activePalette != null)
        {
            return _activePalette.GetHealthColor(clamped);
        }

        if (clamped > 0.6f)
        {
            return _healthyColor;
        }

        if (clamped > 0.3f)
        {
            return Color.Lerp(_damagedColor, _healthyColor, Mathf.InverseLerp(0.3f, 0.6f, clamped));
        }

        return _criticalColor;
    }

    public Color GetAccentColor() => _activePalette != null ? _activePalette.Accent : _healthyColor;
    public Color GetPanelColor() => _activePalette != null ? _activePalette.PanelBackground : _backgroundFallback;
    public Color GetMoneyColor() => _activePalette != null ? _activePalette.MoneyColor : _warningFallback;
    public Color GetTextColor(bool secondary = false) => _activePalette != null
        ? (secondary ? _activePalette.TextSecondary : _activePalette.TextPrimary)
        : (secondary ? Color.gray : Color.white);

    public void ApplyPalette(ColorPalette palette)
    {
        if (palette == null)
        {
            if (_debugLogs)
            {
                Debug.LogWarning("[ThemeManager] Attempted to apply a null palette.");
            }
            return;
        }

        _activePalette = palette;
    }

    void EnsurePaletteLoaded()
    {
        if (_activePalette != null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_defaultPalettePath))
        {
            return;
        }

        _activePalette = Resources.Load<ColorPalette>(_defaultPalettePath);
        if (_activePalette == null && _debugLogs)
        {
            Debug.LogWarning($"[ThemeManager] Could not locate palette at Resources/{_defaultPalettePath}.");
        }
    }

    // Cached fallback shades for panel/money when palette missing
    static readonly Color _backgroundFallback = new Color(0.1f, 0.1f, 0.18f, 0.95f);
    static readonly Color _warningFallback = new Color(0.953f, 0.612f, 0.071f, 1f);
}
