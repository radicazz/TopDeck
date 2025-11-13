using UnityEngine;

/// <summary>
/// Configuration asset for tuning enemy variant procedural generation.
/// Exposes AnimationCurves and ranges so designers can shape difficulty
/// without code edits. Referenced by GameController/EnemyVariantGenerator.
/// </summary>
[CreateAssetMenu(fileName = "ProceduralVariantConfig", menuName = "TopDeck/Procedural Variant Config")]
public class ProceduralVariantConfig : ScriptableObject
{
    [Header("Health Scaling")]
    [SerializeField] private AnimationCurve healthCurve = AnimationCurve.Linear(0f, 1f, 1f, 1.2f);
    [SerializeField] private Vector2 healthRange = new Vector2(0.9f, 1.2f);

    [Header("Speed Scaling")]
    [SerializeField] private AnimationCurve speedCurve = AnimationCurve.Linear(0f, 1f, 1f, 1.15f);
    [SerializeField] private Vector2 speedRange = new Vector2(0.9f, 1.15f);

    [Header("Damage Scaling")]
    [SerializeField] private AnimationCurve damageCurve = AnimationCurve.Linear(0f, 1f, 1f, 1.15f);
    [SerializeField] private Vector2 damageRange = new Vector2(0.9f, 1.15f);

    [Header("Wave & Upgrade Factors")]
    [SerializeField] private float waveScaleFactor = 0.05f;
    [SerializeField] private float defenderCounterFactor = 0.03f;

    [Header("Tint Configuration")]
    [SerializeField] private Gradient variantTintGradient = new Gradient();
    [SerializeField] private float extremeThreshold = 0.85f;

    [Header("Elite Enemies")]
    [SerializeField] private AnimationCurve eliteChanceCurve = AnimationCurve.Linear(0f, 0.05f, 1f, 0.35f);
    [SerializeField] private float eliteHealthMultiplier = 1.75f;
    [SerializeField] private float eliteSpeedMultiplier = 1.05f;
    [SerializeField] private float eliteDamageMultiplier = 1.4f;
    [SerializeField] private Gradient eliteTintGradient = new Gradient();

    [Header("Mini-Boss Enemies")]
    [SerializeField] private AnimationCurve miniBossChanceCurve = AnimationCurve.Linear(0f, 0f, 1f, 0.15f);
    [SerializeField] private float miniBossHealthMultiplier = 3f;
    [SerializeField] private float miniBossSpeedMultiplier = 0.8f;
    [SerializeField] private float miniBossDamageMultiplier = 2f;
    [SerializeField] private Gradient miniBossTintGradient = new Gradient();

    public float EvaluateHealth(float normalizedWave) => 
        Mathf.Clamp(healthCurve.Evaluate(normalizedWave), healthRange.x, healthRange.y);

    public float EvaluateSpeed(float normalizedWave) => 
        Mathf.Clamp(speedCurve.Evaluate(normalizedWave), speedRange.x, speedRange.y);

    public float EvaluateDamage(float normalizedWave) => 
        Mathf.Clamp(damageCurve.Evaluate(normalizedWave), damageRange.x, damageRange.y);

    public float EvaluateEliteChance(float normalizedWave) => 
        Mathf.Clamp01(eliteChanceCurve.Evaluate(normalizedWave));

    public float EvaluateMiniBossChance(float normalizedWave) =>
        Mathf.Clamp01(miniBossChanceCurve.Evaluate(normalizedWave));

    public float WaveScaleFactor => waveScaleFactor;
    public float DefenderCounterFactor => defenderCounterFactor;
    public Gradient TintGradient => variantTintGradient;
    public float ExtremeThreshold => extremeThreshold;
    public Gradient EliteTintGradient => eliteTintGradient != null ? eliteTintGradient : variantTintGradient;
    public Gradient MiniBossTintGradient => miniBossTintGradient != null ? miniBossTintGradient : variantTintGradient;
    public float EliteHealthMultiplier => Mathf.Max(1f, eliteHealthMultiplier);
    public float EliteSpeedMultiplier => Mathf.Max(0.01f, eliteSpeedMultiplier);
    public float EliteDamageMultiplier => Mathf.Max(0.01f, eliteDamageMultiplier);
    public float MiniBossHealthMultiplier => Mathf.Max(1f, miniBossHealthMultiplier);
    public float MiniBossSpeedMultiplier => Mathf.Max(0.01f, miniBossSpeedMultiplier);
    public float MiniBossDamageMultiplier => Mathf.Max(0.01f, miniBossDamageMultiplier);
}
