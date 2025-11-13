using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class AdaptiveWaveDifficultyTests
{
    [Test]
    public void Evaluate_ReturnsPositiveEnemyBudget()
    {
        var director = new AdaptiveWaveDifficulty();
        WaveTuning tuning = director.Evaluate(1, 0, 0);

        Assert.GreaterOrEqual(tuning.EnemyCount, 1);
        Assert.Greater(tuning.SpawnDelay, 0f);
    }

    [Test]
    public void RecordWaveResult_ReducesDifficultyWhenPlayerStruggles()
    {
        var director = new AdaptiveWaveDifficulty();
        WaveTuning baseline = director.Evaluate(6, 0, 0);

        director.RecordWaveResult(new WaveResult
        {
            WaveIndex = 5,
            StartingHealth = 100,
            HealthLost = 90,
            CombatDuration = 60f,
            EnemiesSpawned = 12
        });

        WaveTuning adjusted = director.Evaluate(6, 0, 0);

        Assert.LessOrEqual(adjusted.DifficultyScore, baseline.DifficultyScore);
        Assert.GreaterOrEqual(adjusted.SpawnDelay, baseline.SpawnDelay);
    }

    [Test]
    public void SpawnPatternPlanner_RespectsBudgets()
    {
        var plan = ProceduralSpawnPatternPlanner.BuildPattern(
            SpawnPatternType.Surround,
            12,
            0.75f,
            3,
            3,
            1);

        Assert.AreEqual(12, plan.Count);
        Assert.GreaterOrEqual(plan.Count(instr => instr.ForceElite), 1);
        Assert.AreEqual(1, plan.Count(instr => instr.ForceMiniBoss));
    }

    [Test]
    public void ApplyConfig_OverridesDirectorSettings()
    {
        var director = new AdaptiveWaveDifficulty();
        var config = ScriptableObject.CreateInstance<AdaptiveWaveDifficultyConfig>();

        OverrideConfigField(config, "enemyCountCurve", AnimationCurve.Linear(0f, 10f, 1f, 10f));
        OverrideConfigField(config, "spawnDelayCurve", AnimationCurve.Linear(0f, 1f, 1f, 1f));
        OverrideConfigField(config, "eliteBudgetCurve", AnimationCurve.Linear(0f, 2f, 1f, 2f));
        OverrideConfigField(config, "miniBossBudgetCurve", AnimationCurve.Linear(0f, 1f, 1f, 1f));
        OverrideConfigField(config, "spawnDelayRange", new Vector2(0.1f, 0.25f));
        OverrideConfigField(config, "referenceWaveCount", 5f);
        OverrideConfigField(config, "referenceUpgradeCap", 2);

        director.ApplyConfig(config);

        WaveTuning tuning = director.Evaluate(3, 2, 0);

        Assert.AreEqual(10, tuning.EnemyCount);
        Assert.That(tuning.SpawnDelay, Is.LessThanOrEqualTo(0.25f));
        Assert.GreaterOrEqual(tuning.EliteBudget, 1);
        Assert.GreaterOrEqual(tuning.MiniBossBudget, 1);

        Object.DestroyImmediate(config);
    }

    static void OverrideConfigField<T>(AdaptiveWaveDifficultyConfig config, string fieldName, T value)
    {
        FieldInfo field = typeof(AdaptiveWaveDifficultyConfig).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field, $"Field '{fieldName}' not found on AdaptiveWaveDifficultyConfig.");
        field.SetValue(config, value);
    }
}
