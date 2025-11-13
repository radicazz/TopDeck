using UnityEngine;
using System.Collections.Generic;

public class VfxManager : MonoBehaviour
{
    [SerializeField] GameObject _hitEffect;
    static VfxManager _instance;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    public static void SpawnHit(Vector3 pos)
    {
        if (_instance == null) return;
        if (_instance._hitEffect != null)
        {
            GameObject v = Instantiate(_instance._hitEffect, pos, Quaternion.identity);
            Destroy(v, 2f);
            return;
        }
        // Fallback particle burst
        var go = new GameObject("HitVFX"); go.transform.position = pos; var ps = go.AddComponent<ParticleSystem>(); var main = ps.main; main.startLifetime = 0.25f; main.startSpeed = 0.5f; main.startSize = 0.4f; main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f,0.5f,0.2f,0.9f)); var emission = ps.emission; emission.rateOverTime = 0f; emission.SetBursts(new[]{ new ParticleSystem.Burst(0f, 10)}); ps.Play(); GameObject.Destroy(go, 1f);
    }


public static void SpawnUpgradeFlash(Vector3 pos){ if(_instance==null) return; var go=new GameObject("UpgradeFlash"); go.transform.position=pos; var ps=go.AddComponent<ParticleSystem>(); var main=ps.main; main.startLifetime=0.6f; main.startSpeed=0.5f; main.startSize=1.2f; main.startColor=new ParticleSystem.MinMaxGradient(new Color(1f,0.8f,0.2f,0.9f)); var emission=ps.emission; emission.rateOverTime=0f; emission.SetBursts(new[]{ new ParticleSystem.Burst(0f,20)}); ps.Play(); Destroy(go,1.5f); }
}
