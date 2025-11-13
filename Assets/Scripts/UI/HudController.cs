using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0414

/// <summary>
/// Legacy Canvas HUD placeholder kept so existing scenes stop logging missing-script errors.
/// The actual in-game HUD is powered by UI Toolkit (InfoHudUIDocument + TopBannerUIDocument),
/// so this component simply ensures the old Canvas stays hidden at runtime.
/// </summary>
[DisallowMultipleComponent]
public sealed class HudController : MonoBehaviour
{
    [Header("Legacy References")]
    [SerializeField] private Object _stateText;
    [SerializeField] private Object _moneyText;
    [SerializeField] private Object _timerText;
    [SerializeField] private Object _healthText;
    [SerializeField] private Object _waveText;
    [SerializeField] private Image _moneyIcon;
    [SerializeField] private Image _healthIcon;
    [SerializeField] private GameObject _mainHud;
    [SerializeField] private GameObject _upgradePanel;

    [Header("Behaviour Flags")]
    [SerializeField] private bool _useTheme = true;
    [SerializeField] private bool _autoRefresh = true;
    [SerializeField, Min(0f)] private float _refreshRate = 0.1f;

    void Awake()
    {
        // Ensure the legacy canvas never shows on top of the Toolkit HUD
        HideLegacyCanvas();
    }

    void OnEnable()
    {
        HideLegacyCanvas();
    }

    void HideLegacyCanvas()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        var legacyCanvas = GetComponentInParent<Canvas>();
        if (legacyCanvas != null && legacyCanvas.gameObject.activeSelf)
        {
            legacyCanvas.gameObject.SetActive(false);
        }
    }
#pragma warning restore 0414
}
