using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DefenseController : MonoBehaviour
{
    [Header("Tower Settings")]
    [SerializeField] private float range = 5f;
    [SerializeField] private float fireRate = 1f; // Shots per second
    [SerializeField] private int damage = 25;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private bool useInstantDamage = false; // Alternative to projectiles

    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint; // Where projectiles spawn from

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 90f; // Degrees per second
    [SerializeField] private bool rotateEntireTower = true; // New option for single-object towers
    private AttackerController currentTarget;
    private float lastFireTime;
    private List<AttackerController> enemiesInRange = new List<AttackerController>();
    private SphereCollider rangeCollider;

    void Start()
    {
        SetupRangeCollider();

        // If no fire point specified, use this transform
        if (firePoint == null)
            firePoint = transform;

        Debug.Log($"Tower {gameObject.name} initialized - Range: {range}, Fire Rate: {fireRate}, Damage: {damage}");
    }

    void SetupRangeCollider()
    {
        // Create a trigger collider for range detection
        rangeCollider = gameObject.GetComponent<SphereCollider>();
        if (rangeCollider == null)
        {
            rangeCollider = gameObject.AddComponent<SphereCollider>();
            Debug.Log($"Tower {gameObject.name} created new SphereCollider for range detection");
        }

        rangeCollider.isTrigger = true;
        rangeCollider.radius = range;

        Debug.Log($"Tower {gameObject.name} range collider setup: radius={range}, isTrigger={rangeCollider.isTrigger}");

        // Make sure the collider is on a layer that can interact with enemies
        Debug.Log($"Tower {gameObject.name} is on layer: {gameObject.layer} ({LayerMask.LayerToName(gameObject.layer)})");
    }

    void Update()
    {
        // Force check for enemies in range as backup to trigger detection
        if (Time.frameCount % 30 == 0) // Check every 30 frames
        {
            ForceCheckEnemiesInRange();
        }

        if (Time.frameCount % 60 == 0) // Reduce spam
        {
            Debug.Log($"Tower {gameObject.name} Update: Enemies in range: {enemiesInRange.Count}, Current target: {(currentTarget ? currentTarget.name : "null")}");
        }

        CleanupEnemyList();
        AcquireTarget();

        if (currentTarget != null)
        {
            RotateTowardsTarget();

            if (CanFire())
            {
                Fire();
            }
            else
            {
                float timeUntilFire = (lastFireTime + (1f / fireRate)) - Time.time;
                if (Time.frameCount % 60 == 0) // Log every 60 frames to reduce spam
                {
                    Debug.Log($"Tower {gameObject.name} cannot fire yet, {timeUntilFire:F1}s remaining");
                }
            }
        }
        else
        {
            if (Time.frameCount % 120 == 0) // Log every 120 frames
            {
                Debug.Log($"Tower {gameObject.name} has no target");
            }
        }
    }

    void CleanupEnemyList()
    {
        // Remove null or dead enemies from the list
        enemiesInRange.RemoveAll(enemy => enemy == null || !enemy.IsAlive());

        // Remove current target if it's dead or out of range
        if (currentTarget != null && (currentTarget == null || !currentTarget.IsAlive() ||
            !enemiesInRange.Contains(currentTarget)))
        {
            currentTarget = null;
        }
    }

    void ForceCheckEnemiesInRange()
    {
        // Backup method in case trigger detection isn't working
        AttackerController[] allEnemies = FindObjectsByType<AttackerController>(FindObjectsSortMode.None);

        if (Time.frameCount % 120 == 0) // Debug info every 2 seconds
        {
            Debug.Log($"Tower {gameObject.name} scanning {allEnemies.Length} total enemies");
        }

        foreach (AttackerController enemy in allEnemies)
        {
            if (enemy != null && enemy.IsAlive())
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                bool isInRange = distance <= range;
                bool wasInList = enemiesInRange.Contains(enemy);

                if (isInRange && !wasInList)
                {
                    enemiesInRange.Add(enemy);
                    Debug.Log($"Tower {gameObject.name} FORCE DETECTED enemy {enemy.name} at distance {distance:F1}");

                    // Debug enemy collider info
                    Collider enemyCollider = enemy.GetComponent<Collider>();
                    if (enemyCollider != null)
                    {
                        Debug.Log($"Enemy {enemy.name} has collider: {enemyCollider.GetType().Name}, IsTrigger: {enemyCollider.isTrigger}");
                    }
                    else
                    {
                        Debug.LogWarning($"Enemy {enemy.name} has NO COLLIDER - this will prevent trigger detection!");
                    }
                }
                else if (!isInRange && wasInList)
                {
                    enemiesInRange.Remove(enemy);
                    Debug.Log($"Tower {gameObject.name} FORCE REMOVED enemy {enemy.name} - out of range");
                }
            }
        }
    }

    void AcquireTarget()
    {
        if (currentTarget == null && enemiesInRange.Count > 0)
        {
            // Find the closest enemy or the one that's furthest along the path
            currentTarget = FindBestTarget();
            if (currentTarget != null)
            {
                Debug.Log($"Tower {gameObject.name} acquired new target: {currentTarget.name}");
            }
        }
    }

    AttackerController FindBestTarget()
    {
        AttackerController bestTarget = null;
        float bestScore = float.MinValue;

        foreach (AttackerController enemy in enemiesInRange)
        {
            if (enemy == null || !enemy.IsAlive()) continue;

            // Priority system: prefer enemies that are closer to the end
            float pathProgress = enemy.GetPathProgress();
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            // Higher score for enemies further along the path, lower score for distance
            float score = pathProgress * 100f - distance;

            if (score > bestScore)
            {
                bestScore = score;
                bestTarget = enemy;
            }
        }

        return bestTarget;
    }

    void RotateTowardsTarget()
    {
        if (currentTarget == null) return;

        Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
        direction.y = 0; // Keep rotation only on Y axis for isometric view

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            if (rotateEntireTower)
            {
                Quaternion oldRotation = transform.rotation;

                // Rotate the entire tower
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );

                // Debug rotation changes
                if (Quaternion.Angle(oldRotation, transform.rotation) > 0.1f)
                {
                    Debug.Log($"Tower {gameObject.name} rotating towards {currentTarget.name}. Angle difference: {Quaternion.Angle(transform.rotation, targetRotation):F1}°");
                }
            }
        }
    }

    bool CanFire()
    {
        return Time.time >= lastFireTime + (1f / fireRate);
    }

    void Fire()
    {
        if (currentTarget == null)
        {
            Debug.LogWarning($"Tower {gameObject.name} trying to fire but has no target!");
            return;
        }

        lastFireTime = Time.time;
        Debug.Log($"Tower {gameObject.name} firing at {currentTarget.gameObject.name}");

        // Option 1: Instant damage (no projectile)
        if (useInstantDamage || projectilePrefab == null)
        {
            Debug.Log($"Tower {gameObject.name} dealing instant damage: {damage} to {currentTarget.name}");
            currentTarget.TakeDamage(damage);
            return;
        }

        // Option 2: Projectile system
        Debug.Log($"Tower {gameObject.name} firing projectile from position {firePoint.position}");

        // Create projectile at fire point with tower's rotation
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, transform.rotation);

        // Make sure projectile is visible by setting a reasonable scale
        if (projectile.transform.localScale == Vector3.zero)
        {
            projectile.transform.localScale = Vector3.one * 0.1f; // Small sphere
        }

        Debug.Log($"Projectile created: {projectile.name} at {projectile.transform.position} with scale {projectile.transform.localScale}");

        ProjectileController projectileController = projectile.GetComponent<ProjectileController>();

        if (projectileController != null)
        {
            projectileController.Initialize(currentTarget, damage, projectileSpeed);
            Debug.Log($"Projectile initialized with target {currentTarget.name}, damage {damage}, speed {projectileSpeed}");
        }
        else
        {
            Debug.LogError($"Projectile prefab missing ProjectileController component! Using instant damage instead.");
            // Fallback to instant damage
            currentTarget.TakeDamage(damage);
            Destroy(projectile);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Tower {gameObject.name} OnTriggerEnter: {other.gameObject.name} (tag: {other.tag}, layer: {other.gameObject.layer})");

        AttackerController enemy = other.GetComponent<AttackerController>();
        if (enemy != null && enemy.IsAlive())
        {
            enemiesInRange.Add(enemy);
            Debug.Log($"Tower {gameObject.name} detected enemy {enemy.gameObject.name} in range - Total enemies: {enemiesInRange.Count}");
        }
        else
        {
            Debug.Log($"Tower {gameObject.name} ignored object {other.gameObject.name} - Not an enemy or not alive");
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log($"Tower {gameObject.name} OnTriggerExit: {other.gameObject.name}");

        AttackerController enemy = other.GetComponent<AttackerController>();
        if (enemy != null)
        {
            enemiesInRange.Remove(enemy);
            Debug.Log($"Tower {gameObject.name} lost enemy {enemy.gameObject.name} from range - Total enemies: {enemiesInRange.Count}");

            if (currentTarget == enemy)
            {
                currentTarget = null;
                Debug.Log($"Tower {gameObject.name} lost current target");
            }
        }
    }

    // Visualization in Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);

        if (currentTarget != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, currentTarget.transform.position);
        }
    }
}
