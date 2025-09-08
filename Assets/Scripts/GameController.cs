using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private int startingMoney = 1000;
    [SerializeField] private int startingHealth = 100;
    [SerializeField] private int towerCost = 500;
    [SerializeField] private int killReward = 5;
    [SerializeField] private float prepTime = 30f;
    [SerializeField] private int healthLossPerEnemy = 10;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Prefabs")]
    [SerializeField] private GameObject defenderPrefab;
    [SerializeField] private GameObject attackerPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private int baseEnemiesPerWave = 5;
    [SerializeField] private float enemySpawnDelay = 1f;
    [SerializeField] private float waveHealthMultiplier = 1.2f;

    [Header("Debug")]
    [SerializeField] private bool debugMode = true;

    // Game State
    public enum GamePhase { Preparation, Combat, GameOver }
    public GamePhase currentPhase;

    // Game Variables
    public static GameController Instance;
    public int money;
    public int health;
    public int currentWave = 1;
    public int enemiesRemaining;
    private float timer;
    private float lastTowerPlacementTime = 0f; // Cooldown for tower placement

    // Map References
    private MapController mapController;
    private List<Vector3> spawnPoints = new List<Vector3>();
    private List<Vector3> pathPoints = new List<Vector3>();

    // Tower Placement
    private Camera mainCamera;
    private GameObject highlightedTile;
    private Color originalTileColor;
    private LayerMask tileLayerMask = 1; // Ground layer
    private LayerMask towerLayerMask = -1; // All layers initially

    // Enemy Management
    private List<AttackerController> activeEnemies = new List<AttackerController>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Debug.Log("GameController Start() called");
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
        }

        mapController = FindFirstObjectByType<MapController>();
        if (mapController == null)
        {
            Debug.LogError("MapController not found!");
        }

        InitializeGame();

        // Add a small delay to ensure map is generated
        StartCoroutine(DelayedInitialization());
    }

    IEnumerator DelayedInitialization()
    {
        // Wait a frame for map generation to complete
        yield return new WaitForEndOfFrame();

        FindSpawnPoints();
        StartPreparationPhase();
    }

    void InitializeGame()
    {
        money = startingMoney;
        health = startingHealth;
        currentPhase = GamePhase.Preparation;
        timer = prepTime;

        Debug.Log($"Game initialized - Money: ${money}, Health: {health}, Wave: {currentWave}");
        UpdateUI();
    }

    void FindSpawnPoints()
    {
        if (mapController == null)
        {
            Debug.LogError("MapController not found! Cannot find spawn points.");
            return;
        }

        // Clear previous points
        spawnPoints.Clear();
        pathPoints.Clear();

        // Get path start points directly from MapController
        List<Vector3> pathStartPoints = mapController.GetPathStartPoints();
        spawnPoints.AddRange(pathStartPoints);

        // Get all black positions for pathfinding
        pathPoints.AddRange(mapController.blackPositions);

        Debug.Log($"Found {spawnPoints.Count} path start points for spawning");
        Debug.Log($"Found {pathPoints.Count} total black tiles for pathfinding");

        foreach (Vector3 spawn in spawnPoints)
        {
            Debug.Log($"Spawn point available at: {spawn}");
        }

        // If no spawn points found, fallback to edge detection
        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("No path start points found, falling back to edge detection");
            FindSpawnPointsFallback();
        }
    }

    void FindSpawnPointsFallback()
    {
        // Fallback method - find black tiles by tag/name
        GameObject[] blackTiles = GameObject.FindGameObjectsWithTag("BlackTile");
        Debug.Log($"Fallback: Found {blackTiles.Length} black tiles by tag");

        // If no tiles found by tag, try finding by name or component
        if (blackTiles.Length == 0)
        {
            Debug.LogWarning("No BlackTile tagged objects found, searching by name...");

            // Search for objects with "black" in the name
            GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            List<GameObject> blackObjects = new List<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                if (obj.name.ToLower().Contains("black") || obj.name.ToLower().Contains("cube"))
                {
                    // Check if it's likely a black tile by checking renderer color or material
                    Renderer renderer = obj.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        if (renderer.material.color == Color.black ||
                            renderer.material.name.ToLower().Contains("black"))
                        {
                            blackObjects.Add(obj);
                        }
                    }
                }
            }

            blackTiles = blackObjects.ToArray();
            Debug.Log($"Found {blackTiles.Length} potential black tiles by name/material");
        }

        foreach (GameObject tile in blackTiles)
        {
            Vector3 pos = tile.transform.position;
            pathPoints.Add(pos);

            // Check if this is a spawn point (edge of map)
            if (IsEdgeTile(pos))
            {
                spawnPoints.Add(pos);
                Debug.Log($"Added edge spawn point at: {pos}");
            }
        }

        Debug.Log($"Fallback total spawn points: {spawnPoints.Count}, Total path points: {pathPoints.Count}");
    }

    bool IsEdgeTile(Vector3 position)
    {
        if (mapController == null) return false;

        float cellSize = mapController.cellSize;
        float mapWidth = mapController.width * cellSize;
        float mapHeight = mapController.height * cellSize;

        // Consider tiles on the edges as spawn points
        bool isOnLeftEdge = position.x <= cellSize * 0.5f;
        bool isOnRightEdge = position.x >= mapWidth - (cellSize * 0.5f);
        bool isOnBottomEdge = position.z <= cellSize * 0.5f;
        bool isOnTopEdge = position.z >= mapHeight - (cellSize * 0.5f);

        bool isEdge = isOnLeftEdge || isOnRightEdge || isOnBottomEdge || isOnTopEdge;

        if (isEdge)
        {
            Debug.Log($"Edge tile detected at {position} - Left: {isOnLeftEdge}, Right: {isOnRightEdge}, Bottom: {isOnBottomEdge}, Top: {isOnTopEdge}");
        }

        return isEdge;
    }

    void Update()
    {
        // Debug: Confirm Update is running and show current phase
        if (Input.GetMouseButtonDown(0) && debugMode)
        {
            Debug.Log($"[Update] Mouse clicked! Current phase: {currentPhase}, Timer: {timer:F1}");
        }

        switch (currentPhase)
        {
            case GamePhase.Preparation:
                HandlePreparationPhase();
                break;
            case GamePhase.Combat:
                HandleCombatPhase();
                break;
        }

        UpdateUI();
    }

    void HandlePreparationPhase()
    {
        timer -= Time.deltaTime;

        // Debug: Show we're in preparation phase
        if (Time.frameCount % 120 == 0 && debugMode) // Every 2 seconds
        {
            Debug.Log($"[Preparation] Timer: {timer:F1}s, Money: ${money}, Phase: {currentPhase}");
        }

        // Handle tower placement
        HandleTowerPlacement();

        // Auto-start combat if money reaches $0
        if (money < towerCost)
        {
            Debug.Log($"Not enough money for towers (${money} < ${towerCost}), starting combat phase early");
            StartCombatPhase();
            return;
        }

        if (timer <= 0)
        {
            Debug.Log("Preparation time ended, starting combat phase");
            StartCombatPhase();
        }
    }

    void HandleCombatPhase()
    {
        // Check if all enemies are dead
        CleanupDeadEnemies();

        if (activeEnemies.Count == 0 && enemiesRemaining <= 0 && currentPhase == GamePhase.Combat)
        {
            // Wave complete, start preparation for next wave
            Debug.Log($"Wave {currentWave} completed! Starting preparation for wave {currentWave + 1}.");
            currentWave++;
            // DON'T reset health - let it carry over between waves for difficulty
            // health = startingHealth; // Remove this line to make waves more challenging
            StartPreparationPhase();
        }

        // Check game over condition
        if (health <= 0)
        {
            Debug.Log("Player health reached 0, game over!");
            GameOver();
        }
    }

    void HandleTowerPlacement()
    {
        // ALWAYS show this debug when mouse is clicked (ignore debugMode setting)
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log($"[TOWER DEBUG] CLICK! Phase:{currentPhase} Money:${money} DebugMode:{debugMode} Timer:{timer:F1}");
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // Strategy 1: Try to find tiles using RaycastAll (continue through towers)
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        if (debugMode && Input.GetMouseButtonDown(0)) Debug.Log($"[TowerPlacement] Total hits: {hits.Length}");

        GameObject hitTile = null;
        Vector3 hitPoint = Vector3.zero;
        bool foundTower = false;

        // Look for the ground tile in all hits, prioritizing tiles over towers
        foreach (RaycastHit hit in hits)
        {
            GameObject hitObject = hit.collider.gameObject;

            if (debugMode) Debug.Log($"[TowerPlacement] Hit: {hitObject.name}, Tag: {hitObject.tag}, Position: {hit.point}");

            // Check if this is a tower/defender
            if (hitObject.name.Contains("Defender") || hitObject.GetComponent<DefenseController>() != null)
            {
                if (debugMode) Debug.Log($"[TowerPlacement] Hit tower object: {hitObject.name}");
                foundTower = true;
                continue; // Keep looking for tiles
            }

            // More robust tile detection - look for objects that could be tiles
            bool isTile = false;

            // Check by name patterns
            if (hitObject.name.ToLower().Contains("cube") ||
                hitObject.name.ToLower().Contains("tile") ||
                hitObject.name.ToLower().Contains("ground"))
            {
                isTile = true;
            }

            // Check by tags
            if (hitObject.CompareTag("WhiteTile") || hitObject.CompareTag("BlackTile"))
            {
                isTile = true;
            }

            // Check by renderer and reasonable height (more flexible than before)
            if (!isTile && hitObject.GetComponent<Renderer>() != null && hit.point.y <= 2f)
            {
                isTile = true;
            }

            if (isTile)
            {
                hitTile = hitObject;
                hitPoint = hit.point;
                if (debugMode) Debug.Log($"[TowerPlacement] Found valid tile: {hitObject.name} at {hit.point}");
                break; // Found a tile, stop searching
            }
        }

        // Strategy 2: If no tile found and we hit a tower, try a different approach
        if (hitTile == null && foundTower)
        {
            if (debugMode) Debug.Log("[TowerPlacement] No tile found behind tower, trying alternative detection...");

            // Try raycasting with a layer mask to ignore towers
            int layerMask = ~LayerMask.GetMask("Default"); // Ignore default layer if towers are on it
            RaycastHit altHit;

            if (Physics.Raycast(ray, out altHit, Mathf.Infinity, layerMask))
            {
                GameObject altObject = altHit.collider.gameObject;
                if (debugMode) Debug.Log($"[TowerPlacement] Alternative raycast hit: {altObject.name}");

                // Check if this alternative hit is a tile
                if (altObject.CompareTag("WhiteTile") || altObject.CompareTag("BlackTile") ||
                    altObject.name.ToLower().Contains("cube") || altObject.name.ToLower().Contains("tile"))
                {
                    hitTile = altObject;
                    hitPoint = altHit.point;
                    if (debugMode) Debug.Log($"[TowerPlacement] Alternative method found tile: {altObject.name}");
                }
            }
        }

        // Strategy 3: If still no tile, try finding the nearest tile to the mouse position
        if (hitTile == null && foundTower)
        {
            if (debugMode) Debug.Log("[TowerPlacement] Trying nearest tile detection...");
            hitTile = FindNearestTileToMouse();
            if (hitTile != null)
            {
                // Project the tile position for hit point
                hitPoint = hitTile.transform.position;
                hitPoint.y = 0f; // Ground level
                if (debugMode) Debug.Log($"[TowerPlacement] Found nearest tile: {hitTile.name} at {hitPoint}");
            }
        }

        // If we found a tile
        if (hitTile != null)
        {
            // Only highlight and allow placement on non-black tiles
            if (!hitTile.CompareTag("BlackTile"))
            {
                if (debugMode) Debug.Log($"[TowerPlacement] Highlighting tile: {hitTile.name}");
                HighlightTile(hitTile);

                // Place tower on click (with money and cooldown checks)
                if (Input.GetMouseButtonDown(0))
                {
                    PlaceTower(hitPoint, hitTile);

                }
            }
            else
            {
                if (debugMode) Debug.Log($"[TowerPlacement] Cannot place on black tile: {hitTile.name}");
                ClearHighlight();
            }
        }
        else
        {
            if (foundTower)
            {
                if (debugMode) Debug.Log("[TowerPlacement] Tile search blocked by tower - clearing highlight");
            }
            else
            {
                if (debugMode) Debug.Log("[TowerPlacement] No valid tile found");
            }
            ClearHighlight();
        }
    }

    GameObject FindNearestTileToMouse()
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));

        // Find all objects that could be tiles
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        GameObject nearestTile = null;
        float nearestDistance = float.MaxValue;

        foreach (GameObject obj in allObjects)
        {
            // Check if this looks like a tile
            if (obj.CompareTag("WhiteTile") || obj.CompareTag("BlackTile") ||
                obj.name.ToLower().Contains("cube") || obj.name.ToLower().Contains("tile"))
            {
                float distance = Vector3.Distance(mouseWorldPos, obj.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestTile = obj;
                }
            }
        }

        return nearestTile;
    }

    void HighlightTile(GameObject tile)
    {
        if (debugMode) Debug.Log($"[HighlightTile] Called for tile: {tile.name}");

        if (highlightedTile != tile)
        {
            ClearHighlight();
            highlightedTile = tile;

            // Add highlight effect (change material or add outline)
            Renderer renderer = tile.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Create a copy of the material to avoid affecting all instances
                if (renderer.material != null)
                {
                    // Store original color before changing it
                    originalTileColor = renderer.material.color;

                    // Create a material instance to avoid affecting other tiles
                    Material highlightMaterial = new Material(renderer.material);
                    highlightMaterial.color = Color.yellow;
                    renderer.material = highlightMaterial;

                    if (debugMode) Debug.Log($"[HighlightTile] Applied highlight to {tile.name}, Original color: {originalTileColor}");
                }
            }
            else
            {
                if (debugMode) Debug.LogWarning($"[HighlightTile] No renderer found on tile: {tile.name}");
            }
        }
    }

    void ClearHighlight()
    {
        if (debugMode) Debug.Log($"[ClearHighlight] Called, current highlightedTile: {(highlightedTile ? highlightedTile.name : "null")}");

        if (highlightedTile != null)
        {
            // Reset material color to original
            Renderer renderer = highlightedTile.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Create a new material with the original color to restore
                Material originalMaterial = new Material(renderer.material);
                originalMaterial.color = originalTileColor;
                renderer.material = originalMaterial;

                if (debugMode) Debug.Log($"[ClearHighlight] Restored color for tile: {highlightedTile.name} to {originalTileColor}");
            }
            highlightedTile = null;
        }
    }

    void PlaceTower(Vector3 position, GameObject targetTile = null)
    {
        if (debugMode) Debug.Log($"[PlaceTower] Called with position: {position}, targetTile: {(targetTile ? targetTile.name : "null")}");

        if (money >= towerCost)
        {
            // Snap to grid at ground level (ignore the hit point's Y coordinate)
            Vector3 snapPosition = new Vector3(
                Mathf.Round(position.x / mapController.cellSize) * mapController.cellSize,
                0.5f, // Use fixed ground level instead of position.y
                Mathf.Round(position.z / mapController.cellSize) * mapController.cellSize
            );

            if (debugMode) Debug.Log($"[PlaceTower] Snap position calculated: {snapPosition} (from original: {position}) with cellSize: {mapController.cellSize}");

            money -= towerCost;
            GameObject tower = Instantiate(defenderPrefab, snapPosition, Quaternion.identity);
            Debug.Log($"Tower placed at {snapPosition}, Money remaining: ${money}");

            if (tower == null)
            {
                Debug.LogError("Failed to instantiate tower! Check if defenderPrefab is assigned.");
            }
            else
            {
                if (debugMode) Debug.Log($"[PlaceTower] Tower instantiated successfully: {tower.name} at {tower.transform.position}");

                // Tag the tower and set layer to avoid interfering with tile detection
                if (!tower.CompareTag("Tower"))
                {
                    tower.tag = "Tower";
                    if (debugMode) Debug.Log($"[PlaceTower] Tagged tower as 'Tower'");
                }

                if (tower.GetComponent<Collider>())
                {
                    tower.layer = LayerMask.NameToLayer("Default"); // Or create a "Tower" layer
                    if (debugMode) Debug.Log($"[PlaceTower] Set tower layer to avoid tile detection interference");
                }
            }

            ClearHighlight();
            UpdateUI();
        }
        else
        {
            Debug.Log($"Not enough money to place tower! Need: ${towerCost}, Have: ${money}");
        }
    }

    void StartPreparationPhase()
    {
        currentPhase = GamePhase.Preparation;
        timer = prepTime;
        ClearHighlight();
        Debug.Log($"Started preparation phase for wave {currentWave}");
    }

    void StartCombatPhase()
    {
        currentPhase = GamePhase.Combat;
        enemiesRemaining = GetEnemiesForWave();
        ClearHighlight();

        Debug.Log($"Started combat phase - Wave {currentWave}, Enemies to spawn: {enemiesRemaining}");
        StartCoroutine(SpawnEnemies());
    }

    int GetEnemiesForWave()
    {
        return baseEnemiesPerWave + (currentWave - 1) * 2; // Increase enemies each wave
    }

    IEnumerator SpawnEnemies()
    {
        int enemiesToSpawn = enemiesRemaining;
        Debug.Log($"Spawning {enemiesToSpawn} enemies at {spawnPoints.Count} spawn points");

        // Only proceed if we're still in combat phase
        if (currentPhase != GamePhase.Combat)
        {
            Debug.LogWarning("Spawn enemies called but not in combat phase!");
            yield break;
        }

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            if (spawnPoints.Count > 0)
            {
                // Select a random path start point
                int pathIndex = Random.Range(0, spawnPoints.Count);
                Vector3 spawnPoint = spawnPoints[pathIndex];

                // Ensure enemy spawns exactly on the black tile
                Vector3 exactSpawnPoint = new Vector3(spawnPoint.x, spawnPoint.y + 0.5f, spawnPoint.z);

                Debug.Log($"Spawning enemy {i + 1}/{enemiesToSpawn} at path {pathIndex} position: {exactSpawnPoint}");

                GameObject enemy = Instantiate(attackerPrefab, exactSpawnPoint, Quaternion.identity);

                if (enemy == null)
                {
                    Debug.LogError("Failed to instantiate enemy! Check if attackerPrefab is assigned.");
                    continue;
                }

                AttackerController attacker = enemy.GetComponent<AttackerController>();
                if (attacker != null)
                {
                    int enemyHealth = GetEnemyHealthForWave();

                    // Get the specific path for this enemy to follow
                    List<Vector3> enemyPath = mapController.GetPathByIndex(pathIndex);

                    if (enemyPath.Count > 0)
                    {
                        attacker.Initialize(enemyHealth, enemyPath, exactSpawnPoint);
                        activeEnemies.Add(attacker);
                        Debug.Log($"Enemy initialized with {enemyHealth} health, following path {pathIndex} with {enemyPath.Count} waypoints");
                    }
                    else
                    {
                        Debug.LogError($"No path found for index {pathIndex}, using all black tiles");
                        attacker.Initialize(enemyHealth, pathPoints, exactSpawnPoint);
                        activeEnemies.Add(attacker);
                    }
                }
                else
                {
                    Debug.LogError("Enemy prefab missing AttackerController component!");
                    Destroy(enemy); // Clean up failed enemy
                }
            }
            else
            {
                Debug.LogError("No spawn points available!");
                break;
            }

            yield return new WaitForSeconds(enemySpawnDelay);
        }

        // Reset enemiesRemaining since they're all spawned now
        enemiesRemaining = 0;
        Debug.Log($"Finished spawning enemies. Active enemies: {activeEnemies.Count}");
    }

    int GetEnemyHealthForWave()
    {
        return Mathf.RoundToInt(100 * Mathf.Pow(waveHealthMultiplier, currentWave - 1));
    }

    void CleanupDeadEnemies()
    {
        activeEnemies.RemoveAll(enemy => enemy == null);
    }

    public void OnEnemyKilled(AttackerController enemy)
    {
        money += killReward;
        activeEnemies.Remove(enemy);
        Debug.Log($"Enemy killed! Money earned: ${killReward}, Total money: ${money}, Enemies remaining: {activeEnemies.Count}");

        // Check if wave is complete
        if (activeEnemies.Count == 0 && enemiesRemaining <= 0 && currentPhase == GamePhase.Combat)
        {
            Debug.Log("Wave completed via enemy killed - transitioning to preparation");
        }
    }

    public void OnEnemyReachedEnd(AttackerController enemy)
    {
        health -= healthLossPerEnemy;
        activeEnemies.Remove(enemy);
        Debug.Log($"Enemy reached end! Health lost: {healthLossPerEnemy}, Remaining health: {health}, Enemies remaining: {activeEnemies.Count}");

        // Check if wave is complete
        if (activeEnemies.Count == 0 && enemiesRemaining <= 0 && currentPhase == GamePhase.Combat)
        {
            Debug.Log("Wave completed via enemy reached end - transitioning to preparation");
        }
    }

    void GameOver()
    {
        currentPhase = GamePhase.GameOver;
        SceneManager.LoadScene("End Menu");
    }

    void UpdateUI()
    {
        if (stateText != null)
        {
            stateText.text = currentPhase == GamePhase.Preparation ? "PREPARATION" :
                            currentPhase == GamePhase.Combat ? "COMBAT" : "GAME OVER";
        }

        if (moneyText != null)
        {
            moneyText.text = "$" + money.ToString();
        }

        if (timerText != null)
        {
            if (currentPhase == GamePhase.Preparation)
            {
                timerText.text = "Time: " + Mathf.Ceil(timer).ToString();
            }
            else if (currentPhase == GamePhase.Combat)
            {
                timerText.text = "Wave " + currentWave + " | Enemies: " + activeEnemies.Count;
            }
        }
    }
}
