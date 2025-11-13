using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Edit mode tests for UpgradeManager to verify stat modifier calculations.
/// Protects against regressions when tuning upgrade coefficients.
/// </summary>
public class UpgradeManagerTests
{
    private GameObject managerObject;
    private UpgradeManager upgradeManager;

    [SetUp]
    public void SetUp()
    {
        managerObject = new GameObject("UpgradeManager_Test");
        upgradeManager = managerObject.AddComponent<UpgradeManager>();
    }

    [TearDown]
    public void TearDown()
    {
        if (managerObject != null)
        {
            Object.DestroyImmediate(managerObject);
            managerObject = null;
            upgradeManager = null;
        }
    }

    [Test]
    public void DefenderHealthBonus_IncreasesWithUpgrades()
    {
        Assert.AreEqual(0, upgradeManager.GetDefenderHealthBonus(), "Level 0 should have no health bonus");

        upgradeManager.UpgradeDefenders();
        int level1Bonus = upgradeManager.GetDefenderHealthBonus();
        Assert.Greater(level1Bonus, 0, "Level 1 should increase health");

        upgradeManager.UpgradeDefenders();
        int level2Bonus = upgradeManager.GetDefenderHealthBonus();
        Assert.Greater(level2Bonus, level1Bonus, "Higher level should provide larger bonus");
    }

    [Test]
    public void TowerHealthBonus_IncreasesWithUpgrades()
    {
        Assert.AreEqual(0, upgradeManager.GetTowerHealthBonus(), "Level 0 should have no tower bonus");

        upgradeManager.UpgradeTower();
        int level1Bonus = upgradeManager.GetTowerHealthBonus();
        Assert.Greater(level1Bonus, 0, "Level 1 should increase tower health bonus");
    }

    [Test]
    public void FireRateMultiplier_ReturnsValidValues()
    {
        Assert.AreEqual(1.0f, upgradeManager.GetDefenderFireRateMultiplier(), 0.01f, "Level 0 should have base fire rate");

        upgradeManager.UpgradeDefenders();
        float level1 = upgradeManager.GetDefenderFireRateMultiplier();
        Assert.Greater(level1, 1.0f, "Level 1 should increase fire rate");
    }

    [Test]
    public void DamageMultiplier_ReturnsValidValues()
    {
        Assert.AreEqual(1.0f, upgradeManager.GetDefenderDamageMultiplier(), 0.01f, "Level 0 should have base damage");

        upgradeManager.UpgradeDefenders();
        float level1 = upgradeManager.GetDefenderDamageMultiplier();
        Assert.Greater(level1, 1.0f, "Level 1 should increase damage");
    }
}
