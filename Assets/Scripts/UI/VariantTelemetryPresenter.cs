using UnityEngine;
using TMPro;

/// <summary>
/// Minimal legacy telemetry presenter used only to keep the old Canvas UI from breaking.
/// New telemetry is surfaced through the Toolkit HUD, but this still formats text so
/// existing designers' data remains visible if the canvas is enabled for debugging.
/// </summary>
public sealed class VariantTelemetryPresenter : MonoBehaviour
{
    [SerializeField] private TMP_Text telemetryLabel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private string formatString = "Wave {0} [Pattern {4}] Mutators: {1:+0}% HP, {2:+0}% SPD, {3:+0}% DMG";
    [SerializeField, Min(0f)] private float displayDuration = 3f;

    float _hideTime;

    void Awake()
    {
        HideImmediate();
    }

    void Update()
    {
        if (canvasGroup != null && canvasGroup.alpha > 0f && displayDuration > 0f && Time.unscaledTime >= _hideTime)
        {
            HideImmediate();
        }
    }

    public void Present(int waveIndex, float healthBonus, float speedBonus, float damageBonus, string patternLabel)
    {
        string message = string.Format(formatString ?? string.Empty, waveIndex, healthBonus, speedBonus, damageBonus, patternLabel);
        Show(message);
    }

    public void Show(string message)
    {
        if (telemetryLabel != null)
        {
            telemetryLabel.text = message;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }

        _hideTime = displayDuration > 0f ? Time.unscaledTime + displayDuration : float.NegativeInfinity;
    }

    public void HideImmediate()
    {
        if (telemetryLabel != null)
        {
            telemetryLabel.text = string.Empty;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        _hideTime = float.NegativeInfinity;
    }
}
