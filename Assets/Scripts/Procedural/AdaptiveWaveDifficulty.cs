using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WaveResult
{
    public int WaveIndex;
    public int StartingHealth;
    public int HealthLost;
    public float CombatDuration;
    public int EnemiesSpawned;
    public int ElitesSpawned;
    public int MiniBossesSpawned;
}

[System.Serializable]
public struct WaveTuning
{
    public int EnemyCount;
    public float SpawnDelay;
    public SpawnPatternType Pattern;
    public int EliteBudget;
    public int MiniBossBudget;
    public float DifficultyScore;
    public float PatternAggression;
}

/// <summary>
/// Adaptive difficulty helper that scales enemy counts, tempos, and elite budgets
/// based on wave progression plus recent player performance.
/// </summary>
[System.Serializable]
public class AdaptiveWaveDifficulty
{
    [Header("Enemy Budget")]
    [SerializeField] private AnimationCurve enemyCountCurve = AnimationCurve.Linear(0f, 6f, 1f, 28f);
    [SerializeField] private float referenceWaveCount = 15f;

    [Header("Spawn Tempo")]
    [SerializeField] private AnimationCurve spawnDelayCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] private Vector2 spawnDelayRange = new Vector2(0.35f, 1.5f);

    [Header("Elite & Mini-Boss Budgets")]
    [SerializeField] private AnimationCurve eliteBudgetCurve = AnimationCurve.Linear(0f, 0f, 1f, 3f);
    [SerializeField] private AnimationCurve miniBossBudgetCurve = AnimationCurve.Linear(0f, 0f, 1f, 1.5f);

    [Header("Performance Weights")]
    [SerializeField, Range(0f, 1f)] private float healthPenaltyWeight = 0.45f;
    [SerializeField, Range(0f, 1f)] private float durationPenaltyWeight = 0.35f;
    [SerializeField, Range(0f, 1f)] private float upgradeBoostWeight = 0.2f;
    [SerializeField] private float targetCombatDuration = 35f;
    [SerializeField, Range(1, 6)] private int historyWindow = 3;
    [SerializeField] private int referenceUpgradeCap = 4;

    private readonly Queue<WaveResult> _history = new Queue<WaveResult>();

    public WaveTuning Evaluate(int waveIndex, int defenderLevel, int towerLevel)
    {
        float normalizedReferenceWave = Mathf.Max(1f, referenceWaveCount);
        float waveProgress = Mathf.Clamp01((waveIndex - 1f) / normalizedReferenceWave);
        float performancePenalty = ComputeStress();
        float upgradeBoost = (defenderLevel + towerLevel) / Mathf.Max(1f, referenceUpgradeCap);

        float difficultyScore = Mathf.Clamp01(waveProgress + upgradeBoost * upgradeBoostWeight - performancePenalty);

        int enemyCount = Mathf.Max(1, Mathf.RoundToInt(enemyCountCurve.Evaluate(difficultyScore)));
        float spawnLerp = Mathf.Clamp01(spawnDelayCurve.Evaluate(difficultyScore));
        float spawnDelay = Mathf.Lerp(spawnDelayRange.y, spawnDelayRange.x, spawnLerp);

        int eliteBudget = Mathf.Max(0, Mathf.RoundToInt(eliteBudgetCurve.Evaluate(difficultyScore)));
        int miniBossBudget = Mathf.Max(0, Mathf.RoundToInt(miniBossBudgetCurve.Evaluate(difficultyScore)));

        SpawnPatternType pattern = SelectPattern(difficultyScore);
        float patternAggression = Mathf.Clamp01(difficultyScore + PatternAggressionOffset(pattern));

        return new WaveTuning
        {
            EnemyCount = enemyCount,
            SpawnDelay = Mathf.Clamp(spawnDelay, spawnDelayRange.x, spawnDelayRange.y),
            Pattern = pattern,
            EliteBudget = eliteBudget,
            MiniBossBudget = miniBossBudget,
            DifficultyScore = difficultyScore,
            PatternAggression = patternAggression
        };
    }

    public void RecordWaveResult(WaveResult result)
    {
        if (result.WaveIndex <= 0)
        {
            return;
        }

        _history.Enqueue(result);

        while (_history.Count > historyWindow)
        {
            _history.Dequeue();
        }
    }

    float ComputeStress()
    {
        if (_history.Count == 0)
        {
            return 0f;
        }

        float healthSum = 0f;
        float durationSum = 0f;

        foreach (WaveResult result in _history)
        {
            float healthLossPct = result.StartingHealth > 0
                ? Mathf.Clamp01((float)result.HealthLost / result.StartingHealth)
                : 0f;

            float durationPct = targetCombatDuration > 0f
                ? Mathf.Clamp01(result.CombatDuration / targetCombatDuration)
                : 0f;

            healthSum += healthLossPct;
            durationSum += durationPct;
        }

        float normalizedHealth = healthSum / _history.Count;
        float normalizedDuration = durationSum / _history.Count;

        return Mathf.Clamp01(normalizedHealth * healthPenaltyWeight + normalizedDuration * durationPenaltyWeight);
    }

    public void ApplyConfig(AdaptiveWaveDifficultyConfig config)
    {
        if (config == null)
        {
            return;
        }

        enemyCountCurve = CloneCurve(config.EnemyCountCurve, enemyCountCurve);
        referenceWaveCount = config.ReferenceWaveCount;
        spawnDelayCurve = CloneCurve(config.SpawnDelayCurve, spawnDelayCurve);
        spawnDelayRange = config.SpawnDelayRange;
        eliteBudgetCurve = CloneCurve(config.EliteBudgetCurve, eliteBudgetCurve);
        miniBossBudgetCurve = CloneCurve(config.MiniBossBudgetCurve, miniBossBudgetCurve);
        healthPenaltyWeight = config.HealthPenaltyWeight;
        durationPenaltyWeight = config.DurationPenaltyWeight;
        upgradeBoostWeight = config.UpgradeBoostWeight;
        targetCombatDuration = config.TargetCombatDuration;
        historyWindow = config.HistoryWindow;
        referenceUpgradeCap = config.ReferenceUpgradeCap;
    }

    static AnimationCurve CloneCurve(AnimationCurve source, AnimationCurve fallback)
    {
        if (source == null)
        {
            return fallback ?? new AnimationCurve();
        }

        return new AnimationCurve(source.keys);
    }

    static SpawnPatternType SelectPattern(float difficultyScore)
    {
        if (difficultyScore < 0.25f)
        {
            return SpawnPatternType.Alternating;
        }

        if (difficultyScore < 0.45f)
        {
            return Random.value > 0.5f ? SpawnPatternType.Alternating : SpawnPatternType.Burst;
        }

        if (difficultyScore < 0.65f)
        {
            return SpawnPatternType.Focused;
        }

        if (difficultyScore < 0.85f)
        {
            return SpawnPatternType.Surround;
        }

        return SpawnPatternType.Escort;
    }

    static float PatternAggressionOffset(SpawnPatternType pattern)
    {
        switch (pattern)
        {
            case SpawnPatternType.Burst:
                return 0.2f;
            case SpawnPatternType.Surround:
                return 0.3f;
            case SpawnPatternType.Escort:
                return 0.4f;
            case SpawnPatternType.Focused:
                return 0.15f;
            default:
                return 0f;
        }
    }
}
