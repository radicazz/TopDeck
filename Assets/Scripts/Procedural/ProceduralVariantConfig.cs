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

    public float EvaluateHealth(float normalizedWave) => 
        Mathf.Clamp(healthCurve.Evaluate(normalizedWave), healthRange.x, healthRange.y);

    public float EvaluateSpeed(float normalizedWave) => 
        Mathf.Clamp(speedCurve.Evaluate(normalizedWave), speedRange.x, speedRange.y);

    public float EvaluateDamage(float normalizedWave) => 
        Mathf.Clamp(damageCurve.Evaluate(normalizedWave), damageRange.x, damageRange.y);

    public float WaveScaleFactor => waveScaleFactor;
    public float DefenderCounterFactor => defenderCounterFactor;
    public Gradient TintGradient => variantTintGradient;
    public float ExtremeThreshold => extremeThreshold;
}
