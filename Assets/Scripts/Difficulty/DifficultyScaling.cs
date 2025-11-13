using UnityEngine;

/// <summary>
/// Static difficulty scaling utilities for wave-based progression
/// </summary>
public static class DifficultyScaling
{
    private const float BaseMultiplier = 1.10f;
    private const float SizeScalePerWave = 0.02f;
    private const float SpeedScalePerWave = 0.03f;
    
    public static float GetDifficultyMultiplier(int wave)
    {
        return Mathf.Pow(BaseMultiplier, wave - 1);
    }
    
    public static int ScaleHealth(int baseHealth, int wave)
    {
        float multiplier = GetDifficultyMultiplier(wave);
        return Mathf.RoundToInt(baseHealth * multiplier);
    }
    
    public static int ScaleDamage(int baseDamage, int wave)
    {
        float multiplier = GetDifficultyMultiplier(wave);
        return Mathf.RoundToInt(baseDamage * multiplier);
    }
    
    public static float ScaleSpeed(float baseSpeed, int wave)
    {
        float multiplier = 1.0f + (wave - 1) * SpeedScalePerWave;
        return baseSpeed * multiplier;
    }
    
    public static float ScaleSize(float baseSize, int wave)
    {
        float multiplier = 1.0f + (wave - 1) * SizeScalePerWave;
        return baseSize * multiplier;
    }
    
    public static Color GetDifficultyTint(int wave)
    {
        float intensity = Mathf.Clamp01((wave - 1) / 20.0f);
        return Color.Lerp(Color.white, new Color(1.0f, 0.5f, 0.5f), intensity);
    }
    
    public static string GetDebugInfo(int wave)
    {
        return $"Wave {wave}: Multiplier={GetDifficultyMultiplier(wave):F2}, Health=x{GetDifficultyMultiplier(wave):F2}, Speed=x{1.0f + (wave - 1) * SpeedScalePerWave:F2}";
    }
}
