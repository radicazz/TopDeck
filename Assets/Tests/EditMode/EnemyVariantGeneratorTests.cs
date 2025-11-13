using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Edit mode tests for EnemyVariantGenerator to verify variant generation,
/// elite/mini-boss branching, and aggression hooks used by adaptive waves.
/// </summary>
public class EnemyVariantGeneratorTests
{
    [SetUp]
    public void Setup()
    {
        EnemyVariantGenerator.SetConfig(null);
        Random.InitState(1337);
    }

    [Test]
    public void CreateVariant_ReturnsValidVariant()
    {
        var baseType = BuildBaseType();
        var variant = EnemyVariantGenerator.CreateVariant(baseType, 1, 0);

        Assert.IsNotNull(variant.definition);
        Assert.Greater(variant.definition.BaseHealth, 0);
        Assert.Greater(variant.definition.MoveSpeed, 0);
        Assert.AreEqual(VariantCategory.Normal, variant.category);
    }

    [Test]
    public void CreateVariant_ScalesWithWave()
    {
        var baseType = BuildBaseType();
        var wave1 = EnemyVariantGenerator.CreateVariant(baseType, 1, 0);
        var wave6 = EnemyVariantGenerator.CreateVariant(baseType, 6, 0);

        Assert.Greater(wave6.definition.BaseHealth, wave1.definition.BaseHealth);
    }

    [Test]
    public void CreateVariant_ForcedEliteCategoryAppliesBoosts()
    {
        var baseType = BuildBaseType();
        var eliteVariant = EnemyVariantGenerator.CreateVariant(
            baseType,
            4,
            1,
            new VariantRequest { ForceElite = true });

        Assert.AreEqual(VariantCategory.Elite, eliteVariant.category);
        Assert.Greater(eliteVariant.healthMultiplier, 1f);
        Assert.Greater(eliteVariant.damageMultiplier, 1f);
    }

    [Test]
    public void CreateVariant_ForcedMiniBossCreatesDurableEnemy()
    {
        var baseType = BuildBaseType();
        var miniBoss = EnemyVariantGenerator.CreateVariant(
            baseType,
            5,
            2,
            new VariantRequest { ForceMiniBoss = true });

        Assert.AreEqual(VariantCategory.MiniBoss, miniBoss.category);
        Assert.Greater(miniBoss.healthMultiplier, 2f);
    }

    [Test]
    public void CreateVariant_AggressivePatternBoostsSpeed()
    {
        var baseType = BuildBaseType();
        var calm = EnemyVariantGenerator.CreateVariant(
            baseType,
            3,
            0,
            new VariantRequest { PatternAggression = 0f, DifficultyScalar = 0f, ForceElite = false });

        Random.InitState(1337); // reset for deterministic comparison

        var aggressive = EnemyVariantGenerator.CreateVariant(
            baseType,
            3,
            0,
            new VariantRequest { PatternAggression = 1f, DifficultyScalar = 0.8f, ForceElite = false });

        Assert.Greater(aggressive.speedMultiplier, calm.speedMultiplier);
        Assert.Greater(aggressive.damageMultiplier, calm.damageMultiplier);
    }

    static AttackerTypeDefinition BuildBaseType()
    {
        return new AttackerTypeDefinition(
            "TestEnemy",
            null,
            100,
            2f,
            10f,
            1.5f,
            1f,
            10,
            1f);
    }
}
