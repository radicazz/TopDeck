using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Edit mode tests for UpgradeManager to verify stat modifier calculations.
/// Protects against regressions when tuning upgrade coefficients.
/// </summary>
public class UpgradeManagerTests
{
    [Test]
    public void DefenderHealthModifier_ReturnsCorrectMultiplier()
    {
        var config = ScriptableObject.CreateInstance<UpgradeManager>();
        
        float level0 = config.GetDefenderHealthModifier(0);
        Assert.AreEqual(1.0f, level0, 0.01f, "Level 0 should have no modifier");

        float level1 = config.GetDefenderHealthModifier(1);
        Assert.Greater(level1, 1.0f, "Level 1 should increase health");

        float level2 = config.GetDefenderHealthModifier(2);
        Assert.Greater(level2, level1, "Level 2 should be greater than Level 1");
    }

    [Test]
    public void TowerHealthModifier_ReturnsCorrectMultiplier()
    {
        var config = ScriptableObject.CreateInstance<UpgradeManager>();
        
        float level0 = config.GetTowerHealthModifier(0);
        Assert.AreEqual(1.0f, level0, 0.01f, "Level 0 should have no modifier");

        float level1 = config.GetTowerHealthModifier(1);
        Assert.Greater(level1, 1.0f, "Level 1 should increase tower health");
    }

    [Test]
    public void FireRateModifier_ReturnsValidValues()
    {
        var config = ScriptableObject.CreateInstance<UpgradeManager>();
        
        float level0 = config.GetDefenderFireRateModifier(0);
        Assert.AreEqual(1.0f, level0, 0.01f, "Level 0 should have base fire rate");

        float level2 = config.GetDefenderFireRateModifier(2);
        Assert.Greater(level2, 1.0f, "Higher level should increase fire rate");
    }

    [Test]
    public void DamageModifier_ReturnsValidValues()
    {
        var config = ScriptableObject.CreateInstance<UpgradeManager>();
        
        float level0 = config.GetDefenderDamageModifier(0);
        Assert.AreEqual(1.0f, level0, 0.01f, "Level 0 should have base damage");

        float level2 = config.GetDefenderDamageModifier(2);
        Assert.Greater(level2, 1.0f, "Higher level should increase damage");
    }
}
