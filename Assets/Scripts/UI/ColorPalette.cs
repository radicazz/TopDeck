using UnityEngine;

/// <summary>
/// ScriptableObject that centralizes UI colors so themes can be swapped without code changes.
/// Matches the serialized layout expected by existing palette assets.
/// </summary>
[CreateAssetMenu(menuName = "TopDeck/UI/Color Palette", fileName = "ColorPalette")]
public class ColorPalette : ScriptableObject
{
    [Header("Backgrounds")]
    [SerializeField] private Color _background = new Color(0.1f, 0.1f, 0.18f, 1f);
    [SerializeField] private Color _backgroundLight = new Color(0.15f, 0.15f, 0.25f, 1f);
    [SerializeField] private Color _backgroundDark = new Color(0.05f, 0.05f, 0.1f, 1f);

    [Header("Accents")]
    [SerializeField] private Color _accent = new Color(0.086f, 0.753f, 0.875f, 1f);
    [SerializeField] private Color _accentHover = new Color(0.1f, 0.85f, 1f, 1f);
    [SerializeField] private Color _accentPressed = new Color(0.05f, 0.65f, 0.75f, 1f);

    [Header("Status Colors")]
    [SerializeField] private Color _warning = new Color(0.953f, 0.612f, 0.071f, 1f);
    [SerializeField] private Color _success = new Color(0.2f, 0.8f, 0.2f, 1f);
    [SerializeField] private Color _error = new Color(0.9f, 0.2f, 0.2f, 1f);
    [SerializeField] private Color _info = new Color(0.3f, 0.6f, 1f, 1f);

    [Header("Typography")]
    [SerializeField] private Color _textPrimary = Color.white;
    [SerializeField] private Color _textSecondary = new Color(0.8f, 0.8f, 0.8f, 1f);
    [SerializeField] private Color _textDisabled = new Color(0.5f, 0.5f, 0.5f, 1f);
    [SerializeField] private Color _textHighlight = new Color(0.086f, 0.753f, 0.875f, 1f);

    [Header("Health/Money")]
    [SerializeField] private Color _healthGood = new Color(0.2f, 0.8f, 0.2f, 1f);
    [SerializeField] private Color _healthMid = new Color(1f, 0.8f, 0.2f, 1f);
    [SerializeField] private Color _healthLow = new Color(0.9f, 0.2f, 0.2f, 1f);
    [SerializeField] private Color _moneyColor = new Color(0.953f, 0.612f, 0.071f, 1f);

    [Header("Panels & Buttons")]
    [SerializeField] private Color _panelBackground = new Color(0.1f, 0.1f, 0.18f, 0.95f);
    [SerializeField] private Color _buttonNormal = new Color(0.2f, 0.2f, 0.3f, 1f);
    [SerializeField] private Color _buttonHighlight = new Color(0.086f, 0.753f, 0.875f, 1f);
    [SerializeField] private Color _buttonPressed = new Color(0.05f, 0.05f, 0.1f, 1f);
    [SerializeField] private Color _buttonDisabled = new Color(0.15f, 0.15f, 0.2f, 0.5f);

    public Color Background => _background;
    public Color BackgroundLight => _backgroundLight;
    public Color BackgroundDark => _backgroundDark;
    public Color Accent => _accent;
    public Color AccentHover => _accentHover;
    public Color AccentPressed => _accentPressed;
    public Color Warning => _warning;
    public Color Success => _success;
    public Color Error => _error;
    public Color Info => _info;
    public Color TextPrimary => _textPrimary;
    public Color TextSecondary => _textSecondary;
    public Color TextDisabled => _textDisabled;
    public Color TextHighlight => _textHighlight;
    public Color MoneyColor => _moneyColor;
    public Color PanelBackground => _panelBackground;
    public Color ButtonNormal => _buttonNormal;
    public Color ButtonHighlight => _buttonHighlight;
    public Color ButtonPressed => _buttonPressed;
    public Color ButtonDisabled => _buttonDisabled;

    /// <summary>
    /// Returns a color based on the supplied health ratio (0-1).
    /// </summary>
    public Color GetHealthColor(float normalizedHealth)
    {
        if (normalizedHealth > 0.6f)
        {
            return _healthGood;
        }

        if (normalizedHealth > 0.3f)
        {
            float t = Mathf.InverseLerp(0.3f, 0.6f, normalizedHealth);
            return Color.Lerp(_healthMid, _healthGood, t);
        }

        float blend = Mathf.InverseLerp(0f, 0.3f, normalizedHealth);
        return Color.Lerp(_healthLow, _healthMid, blend);
    }
}
