using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Controls a screen-space post-process Volume based on tower health ratio.
/// </summary>
public class PostProcessPulseController : MonoBehaviour
{
    [SerializeField] private Volume volume;
    [SerializeField] private VolumeProfile fallbackProfile;
    [SerializeField] private AnimationCurve intensityByHealth = AnimationCurve.Linear(0f, 1f, 1f, 0f);
    [SerializeField] private float pulseSpeed = 2f;

    void Awake()
    {
        EnsureVolumeReference();
    }

    void Update()
    {
        EnsureVolumeReference();

        if (volume == null || GameController.Instance == null)
        {
            return;
        }

        float maxHealth = Mathf.Max(1f, GameController.Instance.GetTowerMaxHealth());
        float ratio = Mathf.Clamp01(GameController.Instance.health / maxHealth);
        float baseIntensity = intensityByHealth.Evaluate(1f - ratio);
        float pulse = 0.5f + 0.5f * Mathf.Sin(Time.time * pulseSpeed);
        volume.weight = Mathf.Clamp01(baseIntensity * pulse);
    }

    void EnsureVolumeReference()
    {
        if (volume == null)
        {
            volume = GetComponent<Volume>();
        }

        if (volume == null)
        {
            volume = gameObject.AddComponent<Volume>();
        }

        if (volume != null)
        {
            volume.isGlobal = true;
            if (volume.sharedProfile == null && volume.profile == null && fallbackProfile != null)
            {
                volume.profile = fallbackProfile;
            }
        }
    }
}
