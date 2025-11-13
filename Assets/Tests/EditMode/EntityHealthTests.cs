using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Edit mode coverage for EntityHealth to guard core behaviours (damage, heal, events).
/// </summary>
public class EntityHealthTests
{
    private GameObject _entityObject;
    private EntityHealth _health;

    [SetUp]
    public void SetUp()
    {
        _entityObject = new GameObject("EntityHealth_Test");
        _health = _entityObject.AddComponent<EntityHealth>();
        _health.Initialize(100);
    }

    [TearDown]
    public void TearDown()
    {
        if (_entityObject != null)
        {
            Object.DestroyImmediate(_entityObject);
            _entityObject = null;
            _health = null;
        }
    }

    [Test]
    public void Initialize_SetsCurrentAndMax()
    {
        Assert.AreEqual(100, _health.MaxHealth);
        Assert.AreEqual(100, _health.CurrentHealth);
        Assert.IsTrue(_health.IsAlive);
    }

    [Test]
    public void TakeDamage_ClampsAtZero()
    {
        _health.TakeDamage(250);
        Assert.AreEqual(0, _health.CurrentHealth);
        Assert.IsFalse(_health.IsAlive);
    }

    [Test]
    public void Heal_ClampsAtMax()
    {
        _health.TakeDamage(40);
        _health.Heal(100);
        Assert.AreEqual(100, _health.CurrentHealth);
    }

    [Test]
    public void SetMaxHealth_RespectsHealFlag()
    {
        _health.TakeDamage(50);
        _health.SetMaxHealth(150, healToFull: false);
        Assert.AreEqual(50, _health.CurrentHealth);
        Assert.AreEqual(150, _health.MaxHealth);

        _health.SetMaxHealth(200, healToFull: true);
        Assert.AreEqual(200, _health.CurrentHealth);
    }

    [Test]
    public void Kill_RaisesDeathOnce()
    {
        int deathCallCount = 0;
        _health.OnDeath += () => deathCallCount++;

        _health.Kill();
        _health.Kill();

        Assert.AreEqual(1, deathCallCount);
    }

    [Test]
    public void Revive_RaisesEventWhenComingBackToLife()
    {
        int reviveCount = 0;
        _health.OnRevive += () => reviveCount++;
        _health.Kill();
        _health.Revive(50);

        Assert.AreEqual(1, reviveCount);
        Assert.IsTrue(_health.IsAlive);
        Assert.AreEqual(50, _health.CurrentHealth);
    }

    [Test]
    public void HealthChanged_TriggersOnDamage()
    {
        int changeEvents = 0;
        _health.OnHealthChanged += (current, max) => changeEvents++;
        _health.TakeDamage(10);

        Assert.AreEqual(1, changeEvents);
    }

    [Test]
    public void EnsureInitialized_SetsDefaults_WhenMethodsCalledBeforeInit()
    {
        var go = new GameObject("LazyInitHealth");
        var lazyHealth = go.AddComponent<EntityHealth>();
        lazyHealth.SetMaxHealth(10, healToFull: true);

        Assert.AreEqual(10, lazyHealth.MaxHealth);
        Assert.AreEqual(10, lazyHealth.CurrentHealth);
        Object.DestroyImmediate(go);
    }
}
