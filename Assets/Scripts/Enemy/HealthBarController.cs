using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls a health bar UI element that tracks and displays an enemy's health.
/// Automatically updates colors and fill amount based on health percentage.
/// </summary>
public class HealthBarController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image _fillImage;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private CanvasGroup _canvasGroup;

    [Header("Configuration")]
    [SerializeField] private bool _useThemeColors = true;
    [SerializeField] private bool _fadeWhenFull = true;
    [SerializeField] private float _fadedAlpha = 0.3f;
    [SerializeField] private float _visibleAlpha = 1f;

    [Header("Custom Colors (if not using theme)")]
    [SerializeField] private Color _healthGoodColor = new Color(0.2f, 0.8f, 0.2f, 1f);
    [SerializeField] private Color _healthMidColor = new Color(1f, 0.8f, 0.2f, 1f);
    [SerializeField] private Color _healthLowColor = new Color(0.9f, 0.2f, 0.2f, 1f);
    [SerializeField] private Color _backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);

    [Header("Billboard Settings")]
    [SerializeField] private bool _billboardToCamera = true;
    [SerializeField] private Vector3 _offset = Vector3.up * 2f;

    private Camera _mainCamera;
    private Transform _targetTransform;
    private float _currentHealthPercent = 1f;

    void Awake()
    {
        _mainCamera = Camera.main;

        // Auto-find UI components if not assigned
        if (_fillImage == null)
        {
            _fillImage = transform.Find("Fill")?.GetComponent<Image>();
        }

        if (_backgroundImage == null)
        {
            _backgroundImage = GetComponent<Image>();
        }

        if (_canvasGroup == null)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        // Initialize background color
        if (_backgroundImage != null && !_useThemeColors)
        {
            _backgroundImage.color = _backgroundColor;
        }
    }

    void Start()
    {
        UpdateHealthBar(1f);
    }

    void LateUpdate()
    {
        if (_billboardToCamera && _mainCamera != null)
        {
            // Make health bar face camera
            transform.rotation = _mainCamera.transform.rotation;
        }

        // Update position to follow target
        if (_targetTransform != null)
        {
            transform.position = _targetTransform.position + _offset;
        }
    }

    /// <summary>
    /// Updates the health bar display based on current and max health.
    /// </summary>
    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        if (maxHealth <= 0) maxHealth = 1;

        float healthPercent = Mathf.Clamp01((float)currentHealth / maxHealth);
        UpdateHealthBar(healthPercent);
    }

    /// <summary>
    /// Updates the health bar display based on a 0-1 percentage.
    /// </summary>
    public void UpdateHealthBar(float healthPercent)
    {
        _currentHealthPercent = Mathf.Clamp01(healthPercent);

        if (_fillImage != null)
        {
            _fillImage.fillAmount = _currentHealthPercent;
            _fillImage.color = GetHealthColor(_currentHealthPercent);
        }

        // Fade out when at full health
        if (_canvasGroup != null && _fadeWhenFull)
        {
            _canvasGroup.alpha = _currentHealthPercent >= 0.99f ? _fadedAlpha : _visibleAlpha;
        }
    }

    /// <summary>
    /// Sets the transform that this health bar should follow.
    /// </summary>
    public void SetTarget(Transform target)
    {
        _targetTransform = target;
    }

    /// <summary>
    /// Sets the offset from the target transform.
    /// </summary>
    public void SetOffset(Vector3 offset)
    {
        _offset = offset;
    }

    /// <summary>
    /// Shows or hides the health bar.
    /// </summary>
    public void SetVisible(bool visible)
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = visible ? _visibleAlpha : 0f;
        }
        else
        {
            gameObject.SetActive(visible);
        }
    }

    /// <summary>
    /// Gets the appropriate color for the current health percentage.
    /// </summary>
    Color GetHealthColor(float healthPercent)
    {
        if (_useThemeColors && ThemeManager.Instance != null)
        {
            return ThemeManager.Instance.GetHealthColor(healthPercent);
        }

        // Use custom colors
        if (healthPercent > 0.6f)
        {
            return _healthGoodColor;
        }
        else if (healthPercent > 0.3f)
        {
            return Color.Lerp(_healthLowColor, _healthMidColor, (healthPercent - 0.3f) / 0.3f);
        }
        else
        {
            return _healthLowColor;
        }
    }

    /// <summary>
    /// Creates a simple health bar UI hierarchy.
    /// </summary>
    public static GameObject CreateHealthBarPrefab()
    {
        // Create root canvas
        GameObject healthBarObj = new GameObject("HealthBar");
        Canvas canvas = healthBarObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        RectTransform canvasRect = healthBarObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(1f, 0.15f);

        // Add background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(healthBarObj.transform, false);
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // Add fill
        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(healthBarObj.transform, false);
        Image fillImage = fillObj.AddComponent<Image>();
        fillImage.color = new Color(0.2f, 0.8f, 0.2f, 1f);
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillAmount = 1f;
        
        RectTransform fillRect = fillObj.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;

        // Add controller component
        HealthBarController controller = healthBarObj.AddComponent<HealthBarController>();
        controller._fillImage = fillImage;
        controller._backgroundImage = bgImage;

        return healthBarObj;
    }

    void OnValidate()
    {
        if (Application.isPlaying && _fillImage != null)
        {
            UpdateHealthBar(_currentHealthPercent);
        }
    }
}
