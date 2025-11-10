using UnityEngine;

/// <summary>
/// Spawns particle/VFX prefabs when upgrades are purchased. Hooked via UpgradeShop events.
/// </summary>
public class UpgradeVfxSpawner : MonoBehaviour
{
    [SerializeField] private ParticleSystem defenderUpgradeEffect;
    [SerializeField] private ParticleSystem towerUpgradeEffect;
    [SerializeField] private Transform defenderEffectAnchor;
    [SerializeField] private Transform towerEffectAnchor;
    [SerializeField] private bool parentToAnchor = true;
    [SerializeField] private bool autoDestroyInstances = true;
    [SerializeField] private float destroyPaddingSeconds = 0.25f;

    public void PlayDefenderUpgrade()
    {
        SpawnEffect(defenderUpgradeEffect, defenderEffectAnchor);
    }

    public void PlayTowerUpgrade()
    {
        SpawnEffect(towerUpgradeEffect, towerEffectAnchor);
    }

    void SpawnEffect(ParticleSystem prefab, Transform anchor)
    {
        if (prefab == null)
        {
            return;
        }

        Vector3 spawnPos = anchor != null ? anchor.position : transform.position;
        ParticleSystem instance = Instantiate(prefab, spawnPos, Quaternion.identity);

        if (parentToAnchor && anchor != null)
        {
            instance.transform.SetParent(anchor, worldPositionStays: true);
        }

        if (autoDestroyInstances)
        {
            var mainModule = instance.main;
            float lifetime = mainModule.duration;
            if (mainModule.startLifetime.mode != ParticleSystemCurveMode.Constant)
            {
                lifetime += mainModule.startLifetime.constantMax;
            }
            else
            {
                lifetime += mainModule.startLifetime.constant;
            }

            Destroy(instance.gameObject, Mathf.Max(0.1f, lifetime + destroyPaddingSeconds));
        }
    }
}
