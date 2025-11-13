using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Tests for DifficultyScaling system.
/// Validates the 1.10^(wave-1) formula and proper capping at wave 15.
/// </summary>
public class DifficultyScalingTests
{
    [Test]
    public void GetDifficultyMultiplier_Wave1_Returns1()
    {
        float result = DifficultyScaling.GetDifficultyMultiplier(1);
        Assert.AreEqual(1.0f, result, 0.001f, "Wave 1 should have 1.0x multiplier");
    }

    [Test]
    public void GetDifficultyMultiplier_Wave2_Returns110Percent()
    {
        float result = DifficultyScaling.GetDifficultyMultiplier(2);
        Assert.AreEqual(1.10f, result, 0.001f, "Wave 2 should have 1.10x multiplier");
    }

    [Test]
    public void GetDifficultyMultiplier_Wave5_ReturnsCorrectValue()
    {
        float expected = Mathf.Pow(1.10f, 4); // 1.4641
        float result = DifficultyScaling.GetDifficultyMultiplier(5);
        Assert.AreEqual(expected, result, 0.001f, "Wave 5 should follow formula 1.10^4");
    }

    [Test]
    public void GetDifficultyMultiplier_Wave10_ReturnsCorrectValue()
    {
        float expected = Mathf.Pow(1.10f, 9); // ~2.358
        float result = DifficultyScaling.GetDifficultyMultiplier(10);
        Assert.AreEqual(expected, result, 0.001f, "Wave 10 should follow formula 1.10^9");
    }

    [Test]
    public void GetDifficultyMultiplier_Wave15_ReturnsCappedValue()
    {
        float expected = Mathf.Pow(1.10f, 14); // ~3.797
        float result = DifficultyScaling.GetDifficultyMultiplier(15);
        Assert.AreEqual(expected, result, 0.001f, "Wave 15 should be the cap");
    }

    [Test]
    public void GetDifficultyMultiplier_Wave20_IsCappedAt15()
    {
        float wave15 = DifficultyScaling.GetDifficultyMultiplier(15);
        float wave20 = DifficultyScaling.GetDifficultyMultiplier(20);
        Assert.AreEqual(wave15, wave20, 0.001f, "Wave 20+ should be capped at wave 15 values");
    }

    [Test]
    public void GetDifficultyMultiplier_Wave100_IsCappedAt15()
    {
        float wave15 = DifficultyScaling.GetDifficultyMultiplier(15);
        float wave100 = DifficultyScaling.GetDifficultyMultiplier(100);
        Assert.AreEqual(wave15, wave100, 0.001f, "Wave 100 should be capped at wave 15 values");
    }

    [Test]
    public void GetDifficultyMultiplier_NegativeWave_Returns1()
    {
        float result = DifficultyScaling.GetDifficultyMultiplier(-5);
        Assert.AreEqual(1.0f, result, 0.001f, "Negative waves should be clamped to wave 1");
    }

    [Test]
    public void GetDifficultyMultiplier_Wave0_Returns1()
    {
        float result = DifficultyScaling.GetDifficultyMultiplier(0);
        Assert.AreEqual(1.0f, result, 0.001f, "Wave 0 should be clamped to wave 1");
    }

    [Test]
    public void ScaleHealth_Wave1_ReturnsBaseHealth()
    {
        int baseHealth = 100;
        int result = DifficultyScaling.ScaleHealth(baseHealth, 1);
        Assert.AreEqual(100, result, "Wave 1 should not scale health");
    }

    [Test]
    public void ScaleHealth_Wave5_ScalesCorrectly()
    {
        int baseHealth = 100;
        float multiplier = Mathf.Pow(1.10f, 4); // ~1.4641
        int expected = Mathf.RoundToInt(100 * multiplier); // 146
        int result = DifficultyScaling.ScaleHealth(baseHealth, 5);
        Assert.AreEqual(expected, result, "Wave 5 health should scale with multiplier");
    }

    [Test]
    public void ScaleHealth_Wave15_ReturnsMaxScaledValue()
    {
        int baseHealth = 100;
        float multiplier = Mathf.Pow(1.10f, 14); // ~3.797
        int expected = Mathf.RoundToInt(100 * multiplier); // 380
        int result = DifficultyScaling.ScaleHealth(baseHealth, 15);
        Assert.AreEqual(expected, result, "Wave 15 health should be at max scaling");
    }

    [Test]
    public void ScaleDamage_Wave1_ReturnsBaseDamage()
    {
        int baseDamage = 10;
        int result = DifficultyScaling.ScaleDamage(baseDamage, 1);
        Assert.AreEqual(10, result, "Wave 1 should not scale damage");
    }

    [Test]
    public void ScaleDamage_Wave10_ScalesCorrectly()
    {
        int baseDamage = 10;
        float multiplier = Mathf.Pow(1.10f, 9);
        int expected = Mathf.RoundToInt(10 * multiplier);
        int result = DifficultyScaling.ScaleDamage(baseDamage, 10);
        Assert.AreEqual(expected, result, "Wave 10 damage should scale with multiplier");
    }

    [Test]
    public void ScaleSpeed_Wave1_ReturnsBaseSpeed()
    {
        float baseSpeed = 3.0f;
        float result = DifficultyScaling.ScaleSpeed(baseSpeed, 1);
        Assert.AreEqual(3.0f, result, 0.001f, "Wave 1 should not scale speed");
    }

    [Test]
    public void ScaleSpeed_Wave10_ScalesLessThanHealth()
    {
        float baseSpeed = 3.0f;
        float speedResult = DifficultyScaling.ScaleSpeed(baseSpeed, 10);
        float healthMultiplier = DifficultyScaling.GetDifficultyMultiplier(10);
        
        // Speed should scale less aggressively than health
        float speedIncrease = speedResult - baseSpeed;
        float healthIncrease = (baseSpeed * healthMultiplier) - baseSpeed;
        
        Assert.Less(speedIncrease, healthIncrease, "Speed should scale less than health");
    }

    [Test]
    public void ScaleSize_Wave1_ReturnsBaseSize()
    {
        float baseSize = 1.0f;
        float result = DifficultyScaling.ScaleSize(baseSize, 1);
        Assert.AreEqual(1.0f, result, 0.001f, "Wave 1 should not scale size");
    }

    [Test]
    public void ScaleSize_AlwaysClamped_Between08And14()
    {
        for (int wave = 1; wave <= 20; wave++)
        {
            float result = DifficultyScaling.ScaleSize(1.0f, wave);
            Assert.GreaterOrEqual(result, 0.8f, $"Wave {wave} size should be >= 0.8");
            Assert.LessOrEqual(result, 1.4f, $"Wave {wave} size should be <= 1.4");
        }
    }

    [Test]
    public void GetDifficultyTint_Wave1_ReturnsWhite()
    {
        Color result = DifficultyScaling.GetDifficultyTint(1);
        Assert.AreEqual(Color.white, result, "Wave 1 should return white tint");
    }

    [Test]
    public void GetDifficultyTint_Wave15_ReturnsReddish()
    {
        Color result = DifficultyScaling.GetDifficultyTint(15);
        Color expected = new Color(1f, 0.3f, 0.3f);
        
        Assert.AreEqual(expected.r, result.r, 0.01f, "Red channel should match");
        Assert.AreEqual(expected.g, result.g, 0.01f, "Green channel should match");
        Assert.AreEqual(expected.b, result.b, 0.01f, "Blue channel should match");
    }

    [Test]
    public void GetDifficultyTint_ProgressesFromWhiteToRed()
    {
        Color wave1 = DifficultyScaling.GetDifficultyTint(1);
        Color wave8 = DifficultyScaling.GetDifficultyTint(8);
        Color wave15 = DifficultyScaling.GetDifficultyTint(15);

        // Green channel should decrease from wave 1 to 15
        Assert.Greater(wave1.g, wave8.g, "Green should decrease from wave 1 to 8");
        Assert.Greater(wave8.g, wave15.g, "Green should decrease from wave 8 to 15");

        // Red channel should stay at 1.0
        Assert.AreEqual(1.0f, wave1.r, 0.01f);
        Assert.AreEqual(1.0f, wave15.r, 0.01f);
    }

    [Test]
    public void GetDebugInfo_ReturnsValidString()
    {
        string result = DifficultyScaling.GetDebugInfo(5);
        
        Assert.IsNotNull(result, "Debug info should not be null");
        Assert.IsNotEmpty(result, "Debug info should not be empty");
        Assert.IsTrue(result.Contains("Wave 5"), "Debug info should contain wave number");
        Assert.IsTrue(result.Contains("Multiplier"), "Debug info should contain multiplier info");
    }

    [Test]
    public void ProgressiveScaling_EachWaveIsHarderThanPrevious()
    {
        for (int wave = 1; wave < 15; wave++)
        {
            float current = DifficultyScaling.GetDifficultyMultiplier(wave);
            float next = DifficultyScaling.GetDifficultyMultiplier(wave + 1);
            
            Assert.Greater(next, current, $"Wave {wave + 1} should be harder than wave {wave}");
        }
    }

    [Test]
    public void Wave15Multiplier_IsApproximately3Point8()
    {
        float result = DifficultyScaling.GetDifficultyMultiplier(15);
        Assert.AreEqual(3.797f, result, 0.01f, "Wave 15 should have ~3.8x multiplier");
    }
}
