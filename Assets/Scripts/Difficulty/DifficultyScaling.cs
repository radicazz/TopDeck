using UnityEngine;

/// <summary>
/// Static difficulty scaling utilities for wave-based progression
/// </summary>
public static class DifficultyScaling
{
    private const float BaseMultiplier = 1.10f;      // Core exponential base
    private const float SizeScalePerWave = 0.02f;    // Mild size growth
    private const float SpeedScalePerWave = 0.03f;   // Linear speed growth (kept lower than health exponential)

    /// <summary>
    /// Returns exponential multiplier capped at wave 15, clamping invalid wave inputs to 1.
    /// Formula: 1.10^(wave-1) with wave clamped to [1,15].
    /// </summary>
    public static float GetDifficultyMultiplier(int wave)
    {
        wave = Mathf.Clamp(wave, 1, 15);
        return Mathf.Pow(BaseMultiplier, wave - 1);
    }

    /// <summary>
    /// Scales health using capped exponential multiplier.
    /// </summary>
    public static int ScaleHealth(int baseHealth, int wave)
    {
        float multiplier = GetDifficultyMultiplier(wave);
        return Mathf.RoundToInt(baseHealth * multiplier);
    }

    /// <summary>
    /// Scales damage identically to health (capped exponential).
    /// </summary>
    public static int ScaleDamage(int baseDamage, int wave)
    {
        float multiplier = GetDifficultyMultiplier(wave);
        return Mathf.RoundToInt(baseDamage * multiplier);
    }

    /// <summary>
    /// Scales speed linearly and gently so its increase is less than health increase.
    /// Wave clamped to >=1 for consistency.
    /// </summary>
    public static float ScaleSpeed(float baseSpeed, int wave)
    {
        wave = Mathf.Max(1, wave);
        float multiplier = 1.0f + (wave - 1) * SpeedScalePerWave;
        return baseSpeed * multiplier;
    }

    /// <summary>
    /// Scales size mildly then clamps to [0.8, 1.4].
    /// </summary>
    public static float ScaleSize(float baseSize, int wave)
    {
        wave = Mathf.Max(1, wave);
        float multiplier = 1.0f + (wave - 1) * SizeScalePerWave;
        float scaled = baseSize * multiplier;
        return Mathf.Clamp(scaled, 0.8f * baseSize, 1.4f * baseSize);
    }

    /// <summary>
    /// Returns a tint progressing from white (wave 1) to (1,0.3,0.3) at wave 15, then capped.
    /// </summary>
    public static Color GetDifficultyTint(int wave)
    {
        wave = Mathf.Clamp(wave, 1, 15);
        float t = (wave - 1) / 14f; // 0 at wave1, 1 at wave15
        float greenBlue = 1f - t * 0.7f; // 1 -> 0.3
        return new Color(1f, greenBlue, greenBlue, 1f);
    }

    /// <summary>
    /// Debug string summarizing core multipliers.
    /// </summary>
    public static string GetDebugInfo(int wave)
    {
        return $"Wave {wave}: Multiplier={GetDifficultyMultiplier(wave):F2}, Health=x{GetDifficultyMultiplier(wave):F2}, Speed=x{ScaleSpeed(1f, wave):F2}";
    }
}
