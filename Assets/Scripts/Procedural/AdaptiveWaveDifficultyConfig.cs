using UnityEngine;

/// <summary>
/// ScriptableObject wrapper for adaptive wave difficulty settings so designers
/// can tune budgets/curves without editing serialized MonoBehaviour fields.
/// </summary>
[CreateAssetMenu(fileName = "AdaptiveWaveDifficultyConfig", menuName = "TopDeck/Adaptive Wave Difficulty Config")]
public class AdaptiveWaveDifficultyConfig : ScriptableObject
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

    public AnimationCurve EnemyCountCurve => enemyCountCurve;
    public float ReferenceWaveCount => referenceWaveCount;
    public AnimationCurve SpawnDelayCurve => spawnDelayCurve;
    public Vector2 SpawnDelayRange => spawnDelayRange;
    public AnimationCurve EliteBudgetCurve => eliteBudgetCurve;
    public AnimationCurve MiniBossBudgetCurve => miniBossBudgetCurve;
    public float HealthPenaltyWeight => healthPenaltyWeight;
    public float DurationPenaltyWeight => durationPenaltyWeight;
    public float UpgradeBoostWeight => upgradeBoostWeight;
    public float TargetCombatDuration => targetCombatDuration;
    public int HistoryWindow => Mathf.Max(1, historyWindow);
    public int ReferenceUpgradeCap => Mathf.Max(1, referenceUpgradeCap);
}
