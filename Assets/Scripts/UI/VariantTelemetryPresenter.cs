using UnityEngine;
using TMPro;

/// <summary>
/// Displays procedural variant statistics to the player.
/// Subscribes to GameController variant events and updates UI labels
/// showing multipliers (HP, speed, damage) for the current wave.
/// </summary>
public class VariantTelemetryPresenter : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI telemetryLabel;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Formatting")]
    [SerializeField] private string formatString = "Wave {0} Mutators: {1:+0}% HP, {2:+0}% SPD, {3:+0}% DMG";
    [SerializeField] private float displayDuration = 3f;

    private float _displayTimer;
    private bool _isVisible;

    void Awake()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    void Update()
    {
        if (_isVisible && _displayTimer > 0f)
        {
            _displayTimer -= Time.deltaTime;
            if (_displayTimer <= 0f)
            {
                HideTelemetry();
            }
        }
    }

    public void ShowVariantStats(int wave, float healthMult, float speedMult, float damageMult)
    {
        if (telemetryLabel == null) return;

        int hpPercent = Mathf.RoundToInt((healthMult - 1f) * 100f);
        int spdPercent = Mathf.RoundToInt((speedMult - 1f) * 100f);
        int dmgPercent = Mathf.RoundToInt((damageMult - 1f) * 100f);

        telemetryLabel.text = string.Format(formatString, wave, hpPercent, spdPercent, dmgPercent);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }

        _isVisible = true;
        _displayTimer = displayDuration;
    }

    void HideTelemetry()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
        _isVisible = false;
    }
}
