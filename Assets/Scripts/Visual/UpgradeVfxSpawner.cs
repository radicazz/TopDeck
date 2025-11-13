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
    [SerializeField] private bool monitorUpgradeLevels = true;
    [SerializeField] private float pollInterval = 0.25f;

    private int lastDefenderLevel = -1;
    private int lastTowerLevel = -1;
    private float nextPollTime;

    public void PlayDefenderUpgrade()
    {
        SpawnEffect(defenderUpgradeEffect, defenderEffectAnchor);
    }

    public void PlayTowerUpgrade()
    {
        SpawnEffect(towerUpgradeEffect, towerEffectAnchor);
    }

    void OnEnable()
    {
        CacheUpgradeLevels();
        nextPollTime = Time.time;
    }

    void Update()
    {
        if (!Application.isPlaying || !monitorUpgradeLevels)
        {
            return;
        }

        if (UpgradeManager.Instance == null)
        {
            return;
        }

        if (Time.time < nextPollTime)
        {
            return;
        }

        nextPollTime = Time.time + Mathf.Max(0.05f, pollInterval);

        int currentDefender = UpgradeManager.Instance.GetDefenderLevel();
        int currentTower = UpgradeManager.Instance.GetTowerLevel();

        if (lastDefenderLevel >= 0 && currentDefender > lastDefenderLevel)
        {
            PlayDefenderUpgrade();
        }

        if (lastTowerLevel >= 0 && currentTower > lastTowerLevel)
        {
            PlayTowerUpgrade();
        }

        lastDefenderLevel = currentDefender;
        lastTowerLevel = currentTower;
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

    void CacheUpgradeLevels()
    {
        if (UpgradeManager.Instance == null)
        {
            lastDefenderLevel = -1;
            lastTowerLevel = -1;
            return;
        }

        lastDefenderLevel = UpgradeManager.Instance.GetDefenderLevel();
        lastTowerLevel = UpgradeManager.Instance.GetTowerLevel();
    }
}
