using UnityEngine;
using System.Collections.Generic;

public class AttackerController : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float towerDamage = 20f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackRate = 1f; // Attacks per second

    [Header("Visual")]
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private Transform healthBarParent;

    // Health and combat
    private int currentHealth;
    private bool isAlive = true;
    private float lastAttackTime;

    // Pathfinding
    private List<Vector3> availablePaths;
    private List<Vector3> currentPath;
    private int currentPathIndex = 0;
    private float pathProgress = 0f; // 0 to 1, how far along the entire path

    // Movement
    private Vector3 targetPosition;
    private bool hasReachedEnd = false;

    // Tower attacking
    private DefenseController targetTower;
    private bool isAttackingTower = false;

    // UI
    private GameObject healthBarInstance;

    void Start()
    {
        currentHealth = maxHealth;
        CreateHealthBar();
        Debug.Log($"Enemy {gameObject.name} Start() called at {transform.position}");
    }

    void OnDestroy()
    {
        Debug.Log($"Enemy {gameObject.name} is being destroyed at {transform.position}");
    }

    public void Initialize(int health, List<Vector3> pathPoints)
    {
        Initialize(health, pathPoints, transform.position);
    }

    public void Initialize(int health, List<Vector3> pathPoints, Vector3 startPosition)
    {
        maxHealth = health;
        currentHealth = health;
        availablePaths = pathPoints;

        // Ensure enemy starts exactly on a black tile
        transform.position = startPosition;

        Debug.Log($"Enemy {gameObject.name} initialized with {health} health at {startPosition}, {pathPoints.Count} path points available");

        // Check if we have a complete path or just scattered points
        if (pathPoints.Count > 1 && ArePointsConnected(pathPoints))
        {
            // Use the provided path directly
            CreateDirectPath();
        }
        else
        {
            // Fallback to pathfinding
            FindPathToEnd();
        }

        if (currentPath != null && currentPath.Count > 0)
        {
            targetPosition = currentPath[0];
            Debug.Log($"Enemy path created with {currentPath.Count} waypoints, first target: {targetPosition}");
        }
        else
        {
            Debug.LogError($"Enemy {gameObject.name} failed to create path! Will be destroyed.");
            // Don't destroy immediately, let it try to move
            currentPath = new List<Vector3> { startPosition };
            targetPosition = startPosition;
        }
    }

    bool ArePointsConnected(List<Vector3> points)
    {
        if (points.Count < 2) return false;

        // Check if points form a connected sequence
        for (int i = 0; i < points.Count - 1; i++)
        {
            float distance = Vector3.Distance(points[i], points[i + 1]);
            if (distance > 2f) // If gap is too large, they're not connected
            {
                return false;
            }
        }
        return true;
    }

    void CreateDirectPath()
    {
        currentPath = new List<Vector3>();

        foreach (Vector3 point in availablePaths)
        {
            // Adjust height for movement
            currentPath.Add(new Vector3(point.x, point.y + 0.5f, point.z));
        }

        currentPathIndex = 0;
        Debug.Log($"Enemy {gameObject.name} using direct path with {currentPath.Count} waypoints");
    }

    void FindPathToEnd()
    {
        if (availablePaths == null || availablePaths.Count == 0)
        {
            Debug.LogError($"Enemy {gameObject.name}: No available paths provided!");
            return;
        }

        currentPath = new List<Vector3>();
        Vector3 currentPos = transform.position;

        // Create a path that follows black tiles from start to an end point
        List<Vector3> pathSequence = CreatePathOnBlackTiles(currentPos);
        currentPath.AddRange(pathSequence);

        if (currentPath.Count > 0)
        {
            currentPathIndex = 0;
            targetPosition = currentPath[0];
            Debug.Log($"Enemy {gameObject.name} path created with {currentPath.Count} waypoints, starting from {currentPos}");
        }
        else
        {
            Debug.LogError($"Enemy {gameObject.name} failed to create path from available points!");
        }
    }

    List<Vector3> CreatePathOnBlackTiles(Vector3 startPos)
    {
        List<Vector3> path = new List<Vector3>();

        // Find the nearest black tile to start position (should be very close since we spawn on one)
        Vector3 currentTile = FindNearestBlackTile(startPos);
        path.Add(new Vector3(currentTile.x, currentTile.y + 0.5f, currentTile.z));

        List<Vector3> visited = new List<Vector3>();
        visited.Add(currentTile);

        // Create a path by following connected black tiles
        for (int steps = 0; steps < 15 && availablePaths.Count > visited.Count; steps++)
        {
            Vector3 nextTile = FindNextBlackTile(currentTile, visited);

            if (nextTile != Vector3.zero)
            {
                Vector3 pathPoint = new Vector3(nextTile.x, nextTile.y + 0.5f, nextTile.z);
                path.Add(pathPoint);
                visited.Add(nextTile);
                currentTile = nextTile;

                // Check if we've reached an edge (potential end point)
                if (IsAtMapEdge(nextTile))
                {
                    Debug.Log($"Enemy {gameObject.name} reached map edge at {nextTile}, ending path");
                    break;
                }
            }
            else
            {
                // No more connected tiles, find furthest unvisited tile
                Vector3 furthestTile = FindFurthestUnvisitedTile(currentTile, visited);
                if (furthestTile != Vector3.zero)
                {
                    Vector3 pathPoint = new Vector3(furthestTile.x, furthestTile.y + 0.5f, furthestTile.z);
                    path.Add(pathPoint);
                    Debug.Log($"Enemy {gameObject.name} jumping to furthest tile {furthestTile}");
                    break;
                }
                else
                {
                    break;
                }
            }
        }

        Debug.Log($"Enemy {gameObject.name} created path with {path.Count} black tile waypoints");
        return path;
    }

    Vector3 FindNearestBlackTile(Vector3 position)
    {
        Vector3 nearest = availablePaths[0];
        float shortestDistance = Vector3.Distance(position, nearest);

        foreach (Vector3 tile in availablePaths)
        {
            float distance = Vector3.Distance(position, tile);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearest = tile;
            }
        }

        return nearest;
    }

    Vector3 FindNextBlackTile(Vector3 currentTile, List<Vector3> visited)
    {
        float cellSize = 1f; // Assuming 1 unit cell size, adjust if needed
        Vector3 bestNext = Vector3.zero;
        float shortestDistance = float.MaxValue;

        foreach (Vector3 tile in availablePaths)
        {
            if (visited.Contains(tile)) continue;

            // Check if this tile is adjacent (within reasonable distance)
            float distance = Vector3.Distance(currentTile, tile);
            if (distance <= cellSize * 1.5f && distance < shortestDistance)
            {
                shortestDistance = distance;
                bestNext = tile;
            }
        }

        return bestNext;
    }

    Vector3 FindFurthestUnvisitedTile(Vector3 currentPos, List<Vector3> visited)
    {
        Vector3 furthest = Vector3.zero;
        float maxDistance = 0f;

        foreach (Vector3 tile in availablePaths)
        {
            if (visited.Contains(tile)) continue;

            float distance = Vector3.Distance(currentPos, tile);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                furthest = tile;
            }
        }

        return furthest;
    }

    bool IsAtMapEdge(Vector3 position)
    {
        // This is a simplified check - you might need to adjust based on your map size
        // Assuming map is centered around origin with reasonable bounds
        float edgeThreshold = 8f; // Adjust based on your map size

        return Mathf.Abs(position.x) > edgeThreshold || Mathf.Abs(position.z) > edgeThreshold;
    }

    void Update()
    {
        if (!isAlive || hasReachedEnd) return;

        // Safety check - if enemy has been alive for a few frames but has no path, create emergency path
        if ((currentPath == null || currentPath.Count == 0) && Time.time > 2f)
        {
            Debug.LogWarning($"Enemy {gameObject.name} creating emergency path");
            CreateEmergencyPath();
        }

        // Check for nearby towers to attack (but don't stop moving)
        CheckForTowersToAttack();

        // Always keep moving along the path - never stop
        MoveAlongPath();

        // Attack towers while moving (if in range)
        if (isAttackingTower)
        {
            AttackTower();
        }

        UpdateHealthBar();
    }

    void CreateEmergencyPath()
    {
        if (availablePaths == null || availablePaths.Count == 0)
        {
            // Create a simple forward path
            currentPath = new List<Vector3>();
            Vector3 start = transform.position;
            currentPath.Add(start);
            currentPath.Add(start + Vector3.forward * 5f);
            currentPath.Add(start + Vector3.forward * 10f);
            Debug.Log($"Enemy {gameObject.name} created emergency forward path");
        }
        else
        {
            FindPathToEnd();
        }

        if (currentPath != null && currentPath.Count > 0)
        {
            currentPathIndex = 0;
            targetPosition = currentPath[0];
        }
    }

    void CheckForTowersToAttack()
    {
        // If already attacking a tower and it's still in range, continue
        if (isAttackingTower && targetTower != null)
        {
            float distanceToTower = Vector3.Distance(transform.position, targetTower.transform.position);
            if (distanceToTower <= attackRange * 1.2f) // Add some hysteresis to prevent flickering
            {
                return; // Continue attacking current tower
            }
            else
            {
                // Tower is out of range, stop attacking
                isAttackingTower = false;
                targetTower = null;
                Debug.Log($"{gameObject.name} lost tower target - out of range");
            }
        }

        // Only look for new towers if not currently attacking one
        if (!isAttackingTower)
        {
            // Find towers in attack range
            Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange);

            foreach (Collider col in colliders)
            {
                DefenseController tower = col.GetComponent<DefenseController>();
                if (tower != null)
                {
                    targetTower = tower;
                    isAttackingTower = true;
                    Debug.Log($"{gameObject.name} found tower to attack: {tower.gameObject.name}");
                    break;
                }
            }
        }
    }

    void AttackTower()
    {
        if (targetTower == null)
        {
            isAttackingTower = false;
            return;
        }

        // Attack at specified rate while moving (no facing or stopping)
        if (Time.time >= lastAttackTime + (1f / attackRate))
        {
            lastAttackTime = Time.time;

            // Deal damage to tower invisibly - no visual effects, just damage
            Debug.Log($"{gameObject.name} deals {towerDamage} damage to tower {targetTower.gameObject.name} while passing by");

            // You could add tower health system here later
            // Example: targetTower.TakeDamage(towerDamage);
        }
    }

    void MoveAlongPath()
    {
        if (currentPath == null || currentPath.Count == 0)
        {
            Debug.LogWarning($"Enemy {gameObject.name} has no path, will be destroyed");
            ReachedEnd();
            return;
        }

        if (currentPathIndex >= currentPath.Count)
        {
            Debug.Log($"Enemy {gameObject.name} completed path");
            ReachedEnd();
            return;
        }

        // Move towards current target position
        Vector3 direction = (targetPosition - transform.position).normalized;
        float distanceToMove = moveSpeed * Time.deltaTime;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (distanceToMove >= distanceToTarget)
        {
            // Reached current waypoint
            transform.position = targetPosition;
            currentPathIndex++;

            Debug.Log($"Enemy {gameObject.name} reached waypoint {currentPathIndex - 1}/{currentPath.Count} at {targetPosition}");

            if (currentPathIndex >= currentPath.Count)
            {
                // Reached end of path
                ReachedEnd();
            }
            else
            {
                targetPosition = currentPath[currentPathIndex];
                Debug.Log($"Enemy {gameObject.name} moving to next waypoint: {targetPosition}");
            }
        }
        else
        {
            // Move towards target
            Vector3 nextPosition = transform.position + direction * distanceToMove;
            transform.position = nextPosition;

            // Less strict validation - just log warnings instead of blocking movement
            if (!IsValidMove(nextPosition))
            {
                Debug.LogWarning($"Enemy {gameObject.name} moved away from path at {nextPosition}");
            }
        }

        // Update path progress
        UpdatePathProgress();

        // Rotate to face movement direction
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    bool IsValidMove(Vector3 newPosition)
    {
        // Check if the new position is reasonably close to any black tile
        float maxDistance = 1.5f; // Allow some tolerance

        foreach (Vector3 blackTile in availablePaths)
        {
            float distance = Vector3.Distance(new Vector3(newPosition.x, blackTile.y, newPosition.z), blackTile);
            if (distance <= maxDistance)
            {
                return true;
            }
        }

        return false; // Not close enough to any black tile
    }

    void UpdatePathProgress()
    {
        if (currentPath == null || currentPath.Count == 0) return;

        pathProgress = (float)currentPathIndex / (float)currentPath.Count;
    }

    void ReachedEnd()
    {
        hasReachedEnd = true;
        Debug.Log($"Enemy {gameObject.name} reached the end of the path!");

        // Notify game controller
        if (GameController.Instance != null)
        {
            GameController.Instance.OnEnemyReachedEnd(this);
        }

        // Destroy this enemy
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        if (!isAlive) return;

        currentHealth -= damage;
        Debug.Log($"Enemy {gameObject.name} took {damage} damage, health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isAlive = false;
        Debug.Log($"Enemy {gameObject.name} died!");

        // Notify game controller
        if (GameController.Instance != null)
        {
            GameController.Instance.OnEnemyKilled(this);
        }

        // Add death effects here if desired

        // Destroy the enemy
        Destroy(gameObject);
    }

    void CreateHealthBar()
    {
        if (healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, transform);
            healthBarInstance.transform.localPosition = Vector3.up * 2f; // Above the enemy
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarInstance != null)
        {
            // Update health bar UI - you'll need to implement this based on your health bar prefab
            // Example: healthBarInstance.GetComponent<Slider>().value = (float)currentHealth / maxHealth;
        }
    }

    // Public methods for other scripts
    public bool IsAlive() => isAlive;
    public float GetPathProgress() => pathProgress;
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;

    // Visualization in Scene view
    void OnDrawGizmosSelected()
    {
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Draw path
        if (currentPath != null && currentPath.Count > 1)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < currentPath.Count - 1; i++)
            {
                Gizmos.DrawLine(currentPath[i], currentPath[i + 1]);
            }
        }

        // Draw current target
        if (targetTower != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targetTower.transform.position);
        }
    }
}
