using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float maxLifetime = 5f;
    [SerializeField] private bool usePhysics = false;
    [SerializeField] private GameObject explosionEffect;

    // Target and damage info
    private AttackerController target;
    private int damage;
    private float speed;
    private Vector3 lastKnownTargetPosition;
    private bool isInitialized = false;

    // Physics components
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Debug.Log($"Projectile {gameObject.name} created at {transform.position} with scale {transform.localScale}");

        // Make sure the projectile has a renderer for visibility
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogWarning($"Projectile {gameObject.name} has no Renderer component - it will be invisible!");
        }
        else
        {
            Debug.Log($"Projectile {gameObject.name} has renderer with material: {renderer.material?.name ?? "null"}");
        }

        // Destroy projectile after max lifetime to prevent accumulation
        Destroy(gameObject, maxLifetime);
    }

    public void Initialize(AttackerController targetEnemy, int projectileDamage, float projectileSpeed)
    {
        target = targetEnemy;
        damage = projectileDamage;
        speed = projectileSpeed;
        isInitialized = true;

        if (target != null)
        {
            lastKnownTargetPosition = target.transform.position;
            Debug.Log($"Projectile {gameObject.name} initialized - Target: {target.name}, Damage: {damage}, Speed: {speed}");
            Debug.Log($"Target position: {lastKnownTargetPosition}");
        }
        else
        {
            Debug.LogError($"Projectile {gameObject.name} initialized with null target!");
        }
    }

    void Update()
    {
        if (!isInitialized) return;

        MoveProjectile();
    }

    void MoveProjectile()
    {
        Vector3 targetPosition;

        // If target still exists and is alive, track it
        if (target != null && target.IsAlive())
        {
            targetPosition = target.transform.position;
            lastKnownTargetPosition = targetPosition;
        }
        else
        {
            // Continue to last known position
            targetPosition = lastKnownTargetPosition;
            Debug.Log($"Projectile {gameObject.name} lost target, moving to last known position: {targetPosition}");
        }

        // Calculate direction to target
        Vector3 direction = (targetPosition - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (usePhysics && rb != null)
        {
            // Physics-based movement
            rb.linearVelocity = direction * speed;
        }
        else
        {
            // Simple transform movement
            float distanceToMove = speed * Time.deltaTime;

            if (distanceToMove >= distanceToTarget)
            {
                // Reached target
                Debug.Log($"Projectile {gameObject.name} reached target at {targetPosition}");
                transform.position = targetPosition;
                HitTarget();
            }
            else
            {
                // Move towards target
                Vector3 newPosition = transform.position + direction * distanceToMove;
                transform.position = newPosition;

                // Debug every few frames to avoid spam
                if (Time.frameCount % 30 == 0)
                {
                    Debug.Log($"Projectile {gameObject.name} moving: {transform.position} -> {targetPosition} (distance: {distanceToTarget:F2})");
                }
            }
        }

        // Rotate projectile to face movement direction
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Projectile {gameObject.name} trigger hit: {other.gameObject.name} (tag: {other.tag})");

        // Check if we hit our target or any enemy
        AttackerController hitEnemy = other.GetComponent<AttackerController>();

        if (hitEnemy != null && hitEnemy.IsAlive())
        {
            Debug.Log($"Projectile hit enemy {hitEnemy.gameObject.name} for {damage} damage");
            // Deal damage to the enemy
            hitEnemy.TakeDamage(damage);
            HitTarget();
        }
        // Also check for ground/obstacles if you want projectiles to be blocked
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log($"Projectile hit ground: {other.gameObject.name}");
            HitGround();
        }
    }

    void HitTarget()
    {
        Debug.Log($"Projectile {gameObject.name} hit target!");

        // Create explosion effect if available
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Debug.Log($"Created explosion effect at {transform.position}");
        }
        else
        {
            VfxManager.SpawnHit(transform.position);
        }

        // Destroy the projectile
        Destroy(gameObject);
    }

    void HitGround()
    {
        // Projectile hit the ground or an obstacle
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
        else
        {
            VfxManager.SpawnHit(transform.position);
        }

        Destroy(gameObject);
    }

    // For physics-based projectiles
    void OnCollisionEnter(Collision collision)
    {
        AttackerController hitEnemy = collision.gameObject.GetComponent<AttackerController>();

        if (hitEnemy != null && hitEnemy.IsAlive())
        {
            hitEnemy.TakeDamage(damage);
            HitTarget();
        }
        else
        {
            // Hit something else (ground, wall, etc.)
            HitGround();
        }
    }
}
