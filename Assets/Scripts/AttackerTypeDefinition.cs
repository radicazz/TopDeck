using System;
using UnityEngine;

[Serializable]
public class AttackerTypeDefinition
{
    [SerializeField] private string id = "Default";
    [SerializeField] private GameObject prefab;
    [SerializeField, Range(0.01f, 10f)] private float spawnWeight = 1f;
    [Header("Combat")]
    [SerializeField] private int baseHealth = 100;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float towerDamage = 20f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackRate = 1f;
    [SerializeField] private int damageToPlayer = 10;

    public AttackerTypeDefinition() { }

    public AttackerTypeDefinition(
        string id,
        GameObject prefab,
        int baseHealth,
        float moveSpeed,
        float towerDamage,
        float attackRange,
        float attackRate,
        int damageToPlayer,
        float spawnWeight = 1f)
    {
        this.id = id;
        this.prefab = prefab;
        this.baseHealth = baseHealth;
        this.moveSpeed = moveSpeed;
        this.towerDamage = towerDamage;
        this.attackRange = attackRange;
        this.attackRate = attackRate;
        this.damageToPlayer = damageToPlayer;
        this.spawnWeight = spawnWeight;
    }

    public string Id => string.IsNullOrEmpty(id) ? prefab != null ? prefab.name : "Attacker" : id;
    public GameObject Prefab => prefab;
    public float SpawnWeight => Mathf.Max(0.01f, spawnWeight);
    public int BaseHealth => Mathf.Max(1, baseHealth);
    public float MoveSpeed => Mathf.Max(0.01f, moveSpeed);
    public float TowerDamage => Mathf.Max(0f, towerDamage);
    public float AttackRange => Mathf.Max(0.1f, attackRange);
    public float AttackRate => Mathf.Max(0.01f, attackRate);
    public int DamageToPlayer => Mathf.Max(0, damageToPlayer);
}
