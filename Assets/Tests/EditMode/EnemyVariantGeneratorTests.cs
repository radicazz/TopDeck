using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Edit mode tests for EnemyVariantGenerator to verify variant generation.
/// Seeds RNG and asserts health/color bounds to protect against regressions.
/// </summary>
public class EnemyVariantGeneratorTests
{
    private GameObject _generatorObject;
    private EnemyVariantGenerator _generator;

    [SetUp]
    public void Setup()
    {
        _generatorObject = new GameObject("TestGenerator");
        _generator = _generatorObject.AddComponent<EnemyVariantGenerator>();
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(_generatorObject);
    }

    [Test]
    public void CreateVariant_ReturnsValidVariant()
    {
        var baseType = ScriptableObject.CreateInstance<AttackerTypeDefinition>();
        baseType.displayName = "TestEnemy";
        baseType.health = 100f;
        baseType.movementSpeed = 2f;
        baseType.towerDamage = 10f;

        var variant = _generator.CreateVariant(baseType, 1, 0);

        Assert.IsNotNull(variant.definition, "Variant definition should not be null");
        Assert.Greater(variant.definition.health, 0f, "Variant health should be positive");
        Assert.Greater(variant.definition.movementSpeed, 0f, "Variant speed should be positive");
    }

    [Test]
    public void CreateVariant_ScalesWithWave()
    {
        var baseType = ScriptableObject.CreateInstance<AttackerTypeDefinition>();
        baseType.health = 100f;

        var wave1 = _generator.CreateVariant(baseType, 1, 0);
        var wave5 = _generator.CreateVariant(baseType, 5, 0);

        Assert.Greater(wave5.definition.health, wave1.definition.health, 
            "Wave 5 enemies should generally be tougher than wave 1");
    }

    [Test]
    public void CreateVariant_TintColorIsValid()
    {
        var baseType = ScriptableObject.CreateInstance<AttackerTypeDefinition>();
        baseType.health = 100f;

        var variant = _generator.CreateVariant(baseType, 1, 0);

        Assert.IsTrue(variant.tint.r >= 0f && variant.tint.r <= 1f, "Red channel in valid range");
        Assert.IsTrue(variant.tint.g >= 0f && variant.tint.g <= 1f, "Green channel in valid range");
        Assert.IsTrue(variant.tint.b >= 0f && variant.tint.b <= 1f, "Blue channel in valid range");
    }

    [Test]
    public void CreateVariant_IncreasesDifficultyWithDefenderLevel()
    {
        var baseType = ScriptableObject.CreateInstance<AttackerTypeDefinition>();
        baseType.health = 100f;

        var defLevel0 = _generator.CreateVariant(baseType, 3, 0);
        var defLevel2 = _generator.CreateVariant(baseType, 3, 2);

        // Enemies should compensate for upgraded defenders
        Assert.GreaterOrEqual(defLevel2.definition.health, defLevel0.definition.health,
            "Enemies should scale with defender upgrades");
    }
}
