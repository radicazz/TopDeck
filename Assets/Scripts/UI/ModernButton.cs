using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0414

/// <summary>
/// Lightweight replacement for the legacy ModernButton behaviour.
/// It keeps the serialized fields intact so old Canvas prefabs do not throw
/// missing-script errors, and it forwards basic theme colors to the button visuals.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
public sealed class ModernButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Graphic _label;
    [SerializeField] private Image _iconImage;
    [SerializeField] private int _style;
    [SerializeField] private bool _useTheme = true;
    [SerializeField] private float _hoverScale = 1.05f;
    [SerializeField] private float _pressedScale = 0.95f;
    [SerializeField] private float _animationSpeed = 10f;
    [SerializeField] private bool _enableHoverGlow = true;
    [SerializeField] private bool _enablePressEffect = true;
    [SerializeField] private bool _enableSoundEffects = false;

    void Reset()
    {
        CacheReferences();
    }

    void Awake()
    {
        CacheReferences();
        ApplyTheme();
    }

    void CacheReferences()
    {
        if (_button == null)
        {
            _button = GetComponent<Button>();
        }

        if (_backgroundImage == null)
        {
            _backgroundImage = GetComponent<Image>();
        }
    }

    void ApplyTheme()
    {
        if (!_useTheme || ThemeManager.Instance == null)
        {
            return;
        }

        var palette = ThemeManager.Instance.ActivePalette;
        if (palette == null)
        {
            return;
        }

        if (_button != null)
        {
            var colors = _button.colors;
            colors.normalColor = palette.ButtonNormal;
            colors.highlightedColor = palette.ButtonHighlight;
            colors.pressedColor = palette.ButtonPressed;
            colors.disabledColor = palette.ButtonDisabled;
            _button.colors = colors;
        }

        if (_backgroundImage != null)
        {
            _backgroundImage.color = palette.ButtonNormal;
        }

        if (_label != null)
        {
            _label.color = palette.TextPrimary;
        }

        if (_iconImage != null)
        {
            _iconImage.color = palette.TextPrimary;
        }
    }
#pragma warning restore 0414
}
