using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameController : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private int startingMoney = 2000;  // Increased to allow multiple towers
    [SerializeField] private int startingHealth = 100;
    [SerializeField] private int towerCost = 500;
    [SerializeField] private int killReward = 5;
    [SerializeField] private float prepTime = 10f;
    [SerializeField] private int healthLossPerEnemy = 10;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI playerHealthText;
    [SerializeField] private RectTransform[] pointerBlockingRects;

    [Header("Prefabs")]
    [SerializeField] private GameObject defenderPrefab;
    [SerializeField] private GameObject normalAttackerPrefabOverride;
    [SerializeField] private GameObject eliteAttackerPrefabOverride;
    [SerializeField] private GameObject miniBossAttackerPrefabOverride;

    [Header("Attacker Types")]
    [SerializeField] private List<AttackerTypeDefinition> attackerTypes = new List<AttackerTypeDefinition>();

    [Header("Spawn Settings")]
    [SerializeField] private int baseEnemiesPerWave = 5;
    [SerializeField] private float enemySpawnDelay = 1f;
    [SerializeField] private float waveHealthMultiplier = 1.2f;

    [Header("Debug")]
    [SerializeField] private bool debugMode = true;

    [Header("Time Control")]
    [SerializeField] private List<float> timeScaleOptions = new List<float> { 1f, 2f };
    [SerializeField] private KeyCode timeScaleToggleKey = KeyCode.Space;

    [Header("Procedural System")]
    [SerializeField] private ProceduralVariantConfig variantConfig;

    [Header("Adaptive Difficulty")]
    [SerializeField] private AdaptiveWaveDifficulty adaptiveDifficulty = new AdaptiveWaveDifficulty();
    [SerializeField] private AdaptiveWaveDifficultyConfig adaptiveDifficultyConfig;

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
    private int enemiesAlive = 0;
    private int currentTimeScaleIndex = 0;
    private WaveTuning currentWaveTuning;
    private List<SpawnInstruction> currentSpawnPlan = new List<SpawnInstruction>();
    private float combatPhaseStartTime;
    private int waveStartHealth;
    private int enemiesSpawnedThisWave;
    private int elitesSpawnedThisWave;
    private int miniBossesSpawnedThisWave;
    private DefenderUpgrade selectedDefender;
    private bool hasActiveSelection;

    // Map References
    private MapController mapController;
    private List<Vector3> spawnPoints = new List<Vector3>();
    private List<Vector3> pathPoints = new List<Vector3>();

    // Tower Placement
    [Header("Tower Limits")]
    [SerializeField] private int maxDefenders = 10;
    private Camera mainCamera;
    private GameObject highlightedTile;
    private Color originalTileColor;
    private LayerMask tileLayerMask = 1; // Ground layer
    private LayerMask towerLayerMask = -1; // All layers initially
    private readonly List<DefenderUpgrade> placedDefenders = new List<DefenderUpgrade>();

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

        if (pointerBlockingRects != null)
        {
            var filtered = new List<RectTransform>();
            foreach (var rect in pointerBlockingRects)
            {
                if (rect != null)
                {
                    filtered.Add(rect);
                }
            }

            pointerBlockingRects = filtered.Count > 0 ? filtered.ToArray() : null;
        }
    }

    public float GetPreparationTimer()
    {
        return timer;
    }

    public int GetEnemiesAlive()
    {
        return Mathf.Max(0, enemiesAlive);
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

        if (variantConfig != null)
        {
            EnemyVariantGenerator.SetConfig(variantConfig);
        }

        if (adaptiveDifficultyConfig != null && adaptiveDifficulty != null)
        {
            adaptiveDifficulty.ApplyConfig(adaptiveDifficultyConfig);
        }

        InitializeGame();
        EnsureAttackerTypesConfigured();

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
        // Apply tower health bonus from upgrades, if present
        int towerBonus = 0;
        if (UpgradeManager.Instance != null)
        {
            towerBonus = UpgradeManager.Instance.GetTowerHealthBonus();
        }
        health = startingHealth + Mathf.Max(0, towerBonus);
        currentPhase = GamePhase.Preparation;
        timer = prepTime;

        if (timeScaleOptions == null || timeScaleOptions.Count == 0)
        {
            timeScaleOptions = new List<float> { 1f };
        }

        currentTimeScaleIndex = 0;
        ApplyCurrentTimeScale();

        Debug.Log($"Game initialized - Money: ${money}, Health: {health}, Wave: {currentWave}");
        UpdateUI();
        RefreshDefenderRoster();
    }

    void EnsureAttackerTypesConfigured()
    {
        if (attackerTypes == null)
        {
            attackerTypes = new List<AttackerTypeDefinition>();
        }

        attackerTypes.RemoveAll(type => type == null);

        bool hasValidType = attackerTypes.Any(type => type != null && type.Prefab != null);

        if (!hasValidType)
        {
            Debug.LogWarning("No attacker types configured. Please assign at least one attacker type with a valid prefab in the inspector.");
        }
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
        HandleTimeScaleToggle();

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

        if (hasActiveSelection && selectedDefender == null)
        {
            ClearTowerSelection();
        }

        UpdateUI();
    }

    void HandleTimeScaleToggle()
    {
        if (timeScaleOptions == null || timeScaleOptions.Count == 0)
        {
            return;
        }

        if (Input.GetKeyDown(timeScaleToggleKey))
        {
            currentTimeScaleIndex = (currentTimeScaleIndex + 1) % timeScaleOptions.Count;
            ApplyCurrentTimeScale();
        }
    }

    void ApplyCurrentTimeScale()
    {
        if (timeScaleOptions == null || timeScaleOptions.Count == 0)
        {
            return;
        }

        currentTimeScaleIndex = Mathf.Clamp(currentTimeScaleIndex, 0, timeScaleOptions.Count - 1);

        float targetScale = timeScaleOptions[currentTimeScaleIndex];
        if (targetScale < 0f)
        {
            targetScale = 0f;
        }

        if (!Mathf.Approximately(Time.timeScale, targetScale))
        {
            Time.timeScale = targetScale;

            if (debugMode)
            {
                Debug.Log($"Time scale changed to {targetScale}x via {timeScaleToggleKey}");
            }
        }
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
        if (timer <= 0f) { StartCombatPhase(); }
    }

    void HandleCombatPhase()
    {
        // Check if all enemies are dead
        CleanupDeadEnemies();

        CheckGameOverCondition("combat phase");
        if (currentPhase == GamePhase.GameOver)
        {
            return;
        }

        CheckWaveCompletion("combat phase");
    }

void HandleTowerPlacement()
    {
        if (debugMode && Input.GetMouseButtonDown(0))
        {
            Debug.Log("[HandleTowerPlacement] Left click detected");
        }
        
        // Only block if actually clicking ON the upgrade panel, not just when it exists
        if (IsPointerOverBlockingUI())
        {
            if (debugMode)
            {
                Debug.Log("[HandleTowerPlacement] Click blocked by UI");
            }
            return;
        }

        if (currentPhase != GamePhase.Preparation)
        {
            ClearHighlight();
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        if (Input.GetMouseButtonDown(1))
        {
            ClearTowerSelection();
            return;
        }

        bool clicked = Input.GetMouseButtonDown(0);
        bool attemptedPlacementThisClick = false;
        DefenderUpgrade clickedDefender = null;
        bool foundTower = false;
        GameObject hitTile = null;
        Vector3 hitPoint = Vector3.zero;

        foreach (var hit in hits)
        {
            if (hit.collider == null)
            {
                continue;
            }

            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
            {
                continue;
            }

            DefenderUpgrade defender = hitObject.GetComponentInParent<DefenderUpgrade>();
            if (defender != null && defender.isActiveAndEnabled)
            {
                foundTower = true;
                if (clicked && clickedDefender == null)
                {
                    clickedDefender = defender;
                    SelectTower(clickedDefender);
                    ClearHighlight();
                    return;
                }
                continue;
            }

            bool isTile = false;
            if (hitObject.name.ToLower().Contains("cube") ||
                hitObject.name.ToLower().Contains("tile") ||
                hitObject.name.ToLower().Contains("ground"))
            {
                isTile = true;
            }

            if (hitObject.CompareTag("WhiteTile") || hitObject.CompareTag("BlackTile"))
            {
                isTile = true;
            }

            if (!isTile && hitObject.GetComponent<Renderer>() != null && hit.point.y <= 2f)
            {
                isTile = true;
            }

            if (isTile)
            {
                hitTile = hitObject;
                hitPoint = hitTile.transform.position;
                hitPoint.y = 0f;
                break;
            }
        }

        if (hitTile == null && foundTower)
        {
            if (debugMode) Debug.Log("[TowerPlacement] No tile found behind tower, trying alternative detection...");
            int layerMask = ~LayerMask.GetMask("Default");
            RaycastHit altHit;

            if (Physics.Raycast(ray, out altHit, Mathf.Infinity, layerMask))
            {
                GameObject altObject = altHit.collider.gameObject;
                if (altObject.CompareTag("WhiteTile") || altObject.CompareTag("BlackTile") ||
                    altObject.name.ToLower().Contains("cube") || altObject.name.ToLower().Contains("tile"))
                {
                    hitTile = altObject;
                    hitPoint = hitTile.transform.position;
                    hitPoint.y = 0f;
                }
            }
        }

        if (hitTile == null && foundTower)
        {
            hitTile = FindNearestTileToMouse();
            if (hitTile != null)
            {
                hitPoint = hitTile.transform.position;
                hitPoint.y = 0f;
            }
        }

        if (hitTile != null)
        {
            if (!hitTile.CompareTag("BlackTile"))
            {
                HighlightTile(hitTile);

                // Allow placement if we clicked on a tile and didn't click on a tower
                if (clicked && clickedDefender == null)
                {
                    attemptedPlacementThisClick = true;
                    PlaceTower(hitPoint, hitTile);
                }
            }
            else
            {
                ClearHighlight();
            }
        }
        else
        {
            ClearHighlight();
        }
    }

    bool IsPointerOverBlockingUI()
    {
        if (UIToolkitPointerBlocker.IsPointerBlocking())
        {
            if (debugMode)
            {
                Debug.Log("[IsPointerOverBlockingUI] Pointer over UI Toolkit panel");
            }
            return true;
        }

        // First check: Is pointer over ANY UI element?
        if (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject())
        {
            return false;
        }

        // Second check: Is it over a BLOCKING UI element (not just any UI)?
        if (pointerBlockingRects == null || pointerBlockingRects.Length == 0)
        {
            return false;
        }

        Vector2 screenPoint = Input.mousePosition;

        foreach (var rect in pointerBlockingRects)
        {
            if (rect == null)
            {
                continue;
            }
            
            // Skip if the rect is not active (panel hidden)
            if (!rect.gameObject.activeInHierarchy)
            {
                continue;
            }

            Canvas canvas = rect.GetComponentInParent<Canvas>();
            Camera uiCamera = null;
            if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                uiCamera = canvas.worldCamera != null ? canvas.worldCamera : mainCamera;
            }

            if (RectTransformUtility.RectangleContainsScreenPoint(rect, screenPoint, uiCamera))
            {
                if (debugMode)
                {
                    Debug.Log($"[IsPointerOverBlockingUI] Pointer over {rect.gameObject.name}");
                }
                return true;
            }
        }

        return false;
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
        if (highlightedTile == tile)
        {
            return; // Already highlighted, skip
        }

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

                // Red if can't afford, cyan if can afford
                Color highlightColor = (money >= towerCost) ? Color.cyan : Color.red;

                // Create a material instance to avoid affecting other tiles
                Material highlightMaterial = new Material(renderer.material);
                highlightMaterial.color = highlightColor;
                renderer.material = highlightMaterial;
            }
        }
    }

    void ClearHighlight()
    {
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
            }
            highlightedTile = null;
        }
    }

void PlaceTower(Vector3 position, GameObject targetTile = null)
    {
        Debug.Log($"[PlaceTower] Called with position: {position}, targetTile: {(targetTile ? targetTile.name : "null")}");

        if (IsDefenderLimitReached())
        {
            Debug.Log("[PlaceTower] Cannot place tower: defender limit reached.");
            return;
        }

        Vector3 snapPosition = new Vector3(
            Mathf.Round(position.x / mapController.cellSize) * mapController.cellSize,
            0.5f,
            Mathf.Round(position.z / mapController.cellSize) * mapController.cellSize
        );

        Debug.Log($"[PlaceTower] Snap position: {snapPosition}, cellSize: {mapController.cellSize}");

        if (IsTileOccupied(snapPosition))
        {
            Debug.Log("[PlaceTower] Cannot place tower: tile already occupied.");
            return;
        }

        if (!TrySpendMoney(towerCost))
        {
            Debug.Log($"[PlaceTower] Not enough money! Need: ${towerCost}, Have: ${money}");
            return;
        }

        GameObject tower = Instantiate(defenderPrefab, snapPosition, Quaternion.identity);
        Debug.Log($"[PlaceTower] Tower placed at {snapPosition}, Money remaining: ${money}");

        if (tower == null)
        {
            Debug.LogError("[PlaceTower] Failed to instantiate tower!");
        }
        else
        {
            Debug.Log($"[PlaceTower] Tower instantiated: {tower.name}");

            if (!tower.CompareTag("Tower"))
            {
                tower.tag = "Tower";
            }

            if (tower.GetComponent<Collider>())
            {
                tower.layer = LayerMask.NameToLayer("Default");
            }

        var defenderUpgrade = tower.GetComponent<DefenderUpgrade>();
        if (defenderUpgrade != null)
        {
            RegisterDefender(defenderUpgrade);
            Debug.Log($"[PlaceTower] Auto-selecting placed tower");
            SelectTower(defenderUpgrade);
        }
        }

        ClearHighlight();
    }

    void RefreshDefenderRoster()
    {
        placedDefenders.Clear();
        var defenders = FindObjectsByType<DefenderUpgrade>(FindObjectsSortMode.None);
        foreach (var defender in defenders)
        {
            RegisterDefender(defender);
        }
    }

    void CleanupDefenderRoster()
    {
        placedDefenders.RemoveAll(defender => defender == null);
    }

    void RegisterDefender(DefenderUpgrade defender)
    {
        if (defender == null)
        {
            return;
        }

        CleanupDefenderRoster();
        if (!placedDefenders.Contains(defender))
        {
            placedDefenders.Add(defender);
        }
    }

    bool IsDefenderLimitReached()
    {
        if (maxDefenders <= 0)
        {
            return false;
        }

        CleanupDefenderRoster();
        return placedDefenders.Count >= maxDefenders;
    }

    public int GetActiveDefenderCount()
    {
        CleanupDefenderRoster();
        return placedDefenders.Count;
    }

    public int GetDefenderLimit()
    {
        return Mathf.Max(0, maxDefenders);
    }

    bool IsTileOccupied(Vector3 tileCenter)
    {
        float radius = mapController != null ? Mathf.Max(0.45f, mapController.cellSize * 0.45f) : 0.5f;
        Vector3 center = tileCenter + Vector3.up * 0.5f;
        Collider[] overlaps = Physics.OverlapSphere(center, radius);
        foreach (var col in overlaps)
        {
            if (col == null)
            {
                continue;
            }

            if (col.GetComponentInParent<DefenseController>() != null)
            {
                return true;
            }
        }

        return false;
    }

    void StartPreparationPhase()
    {
        currentPhase = GamePhase.Preparation;
        timer = prepTime;
        CleanupDeadEnemies();
        enemiesAlive = Mathf.Max(0, enemiesAlive);
        enemiesRemaining = 0;
        currentSpawnPlan.Clear();
        combatPhaseStartTime = 0f;
        enemiesSpawnedThisWave = 0;
        elitesSpawnedThisWave = 0;
        miniBossesSpawnedThisWave = 0;
        ClearHighlight();
        ClearTowerSelection();
        Debug.Log($"Started preparation phase for wave {currentWave}");
    }

    void StartCombatPhase()
    {
        currentPhase = GamePhase.Combat;
        CleanupDeadEnemies();
        PrepareWaveTuning();
        enemiesRemaining = Mathf.Max(0, currentWaveTuning.EnemyCount);
        enemiesAlive = activeEnemies.Count;
        ClearHighlight();

        currentSpawnPlan = ProceduralSpawnPatternPlanner.BuildPattern(
            currentWaveTuning.Pattern,
            enemiesRemaining,
            Mathf.Max(0.05f, currentWaveTuning.SpawnDelay),
            spawnPoints.Count,
            currentWaveTuning.EliteBudget,
            currentWaveTuning.MiniBossBudget);

        waveStartHealth = health;
        combatPhaseStartTime = Time.time;
        enemiesSpawnedThisWave = 0;
        elitesSpawnedThisWave = 0;
        miniBossesSpawnedThisWave = 0;
        ClearTowerSelection();

        Debug.Log($"Started combat phase - Wave {currentWave}, Pattern: {currentWaveTuning.Pattern}, Difficulty: {currentWaveTuning.DifficultyScore:F2}, Enemies to spawn: {enemiesRemaining}, Elite budget: {currentWaveTuning.EliteBudget}, MiniBoss budget: {currentWaveTuning.MiniBossBudget}");
        StartCoroutine(SpawnEnemies());
    }

void SelectTower(DefenderUpgrade defender)
    {
        if (defender == null)
        {
            Debug.Log($"[SelectTower] Cannot select: defender={defender}, phase={currentPhase}");
            return;
        }

        if (selectedDefender == defender)
        {
            Debug.Log("[SelectTower] Already selected this tower");
            return;
        }

        if (selectedDefender != null)
        {
            selectedDefender.SetSelected(false);
        }

        selectedDefender = defender;
        selectedDefender.SetSelected(true);
        hasActiveSelection = true;
        
        Debug.Log($"[SelectTower] Selected {defender.DisplayName}");
        
        // Show upgrade panel if available
        var upgradePanel = FindFirstObjectByType<UpgradePanelUIDocument>();
        if (upgradePanel != null)
        {
            upgradePanel.ShowPanel();
        }
    }

    void ClearTowerSelection()
    {
        if (!hasActiveSelection && selectedDefender == null)
        {
            return;
        }

        if (selectedDefender != null)
        {
            selectedDefender.SetSelected(false);
        }

        selectedDefender = null;
        hasActiveSelection = false;
        
        // Hide upgrade panel
        var upgradePanel = FindFirstObjectByType<UpgradePanelUIDocument>();
        if (upgradePanel != null)
        {
            upgradePanel.HidePanel();
        }
    }

    public bool IsTowerSelected()
    {
        return selectedDefender != null && hasActiveSelection;
    }

    void PrepareWaveTuning()
    {
        int defenderLevel = UpgradeManager.Instance != null ? UpgradeManager.Instance.GetDefenderLevel() : 0;
        int towerLevel = UpgradeManager.Instance != null ? UpgradeManager.Instance.GetTowerLevel() : 0;

        if (adaptiveDifficulty != null)
        {
            currentWaveTuning = adaptiveDifficulty.Evaluate(currentWave, defenderLevel, towerLevel);
        }
        else
        {
            currentWaveTuning = new WaveTuning
            {
                EnemyCount = GetEnemiesForWave(),
                SpawnDelay = enemySpawnDelay,
                Pattern = SpawnPatternType.Alternating,
                EliteBudget = 0,
                MiniBossBudget = 0,
                DifficultyScore = Mathf.Clamp01((currentWave - 1f) / 10f),
                PatternAggression = 0.3f
            };
        }

        if (currentWaveTuning.EnemyCount <= 0)
        {
            currentWaveTuning.EnemyCount = GetEnemiesForWave();
        }

        if (currentWaveTuning.SpawnDelay <= 0f)
        {
            currentWaveTuning.SpawnDelay = enemySpawnDelay;
        }

        if (currentWaveTuning.PatternAggression <= 0f)
        {
            currentWaveTuning.PatternAggression = Mathf.Clamp01((float)currentWave / 10f);
        }

        enemySpawnDelay = currentWaveTuning.SpawnDelay;
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

        List<AttackerTypeDefinition> spawnQueue = BuildAttackerSpawnQueue(enemiesToSpawn);

        if (spawnQueue.Count == 0)
        {
            Debug.LogError("Spawn queue was empty. Ensure attacker types are configured with valid prefabs.");
            yield break;
        }

        List<SpawnInstruction> spawnPlan = currentSpawnPlan;
        if (spawnPlan == null || spawnPlan.Count == 0)
        {
            spawnPlan = ProceduralSpawnPatternPlanner.BuildPattern(
                currentWaveTuning.Pattern,
                spawnQueue.Count,
                Mathf.Max(0.05f, enemySpawnDelay),
                spawnPoints.Count,
                currentWaveTuning.EliteBudget,
                currentWaveTuning.MiniBossBudget);
        }

        int spawnedCount = 0;
        float totalHealthMult = 0f;
        float totalSpeedMult = 0f;
        float totalDamageMult = 0f;

        for (int i = 0; i < spawnQueue.Count; i++)
        {
            SpawnInstruction instruction = i < spawnPlan.Count
                ? spawnPlan[i]
                : new SpawnInstruction
                {
                    SpawnPointIndex = spawnPoints.Count > 0 ? i % Mathf.Max(1, spawnPoints.Count) : 0,
                    Delay = enemySpawnDelay
                };

            if (spawnPoints.Count > 0)
            {
                AttackerTypeDefinition attackerType = spawnQueue[i];
                if (attackerType == null)
                {
                    Debug.LogError($"Unable to spawn enemy {i + 1}/{spawnQueue.Count} - attacker type entry is missing.");
                    continue;
                }

                // Select a random path start point
                int pathIndex = Mathf.Clamp(instruction.SpawnPointIndex, 0, spawnPoints.Count - 1);
                Vector3 spawnPoint = spawnPoints[pathIndex];

                // Ensure enemy spawns exactly on the black tile
                Vector3 exactSpawnPoint = new Vector3(spawnPoint.x, spawnPoint.y + 0.5f, spawnPoint.z);

                string typeLabel = attackerType.Id;
                Debug.Log($"Spawning enemy {i + 1}/{enemiesToSpawn} ({typeLabel}) at path {pathIndex} position: {exactSpawnPoint}");

                // Create a procedural variant of this attacker type (stats + tint)
                int defLevel = UpgradeManager.Instance != null ? UpgradeManager.Instance.GetDefenderLevel() : 0;
                int spawnsRemaining = Mathf.Max(1, spawnQueue.Count - i);
                int elitesBudgetRemaining = Mathf.Max(0, currentWaveTuning.EliteBudget - elitesSpawnedThisWave);
                int miniBossBudgetRemaining = Mathf.Max(0, currentWaveTuning.MiniBossBudget - miniBossesSpawnedThisWave);

                VariantRequest variantRequest = new VariantRequest
                {
                    DifficultyScalar = currentWaveTuning.DifficultyScore,
                    PatternAggression = currentWaveTuning.PatternAggression,
                    EliteBudgetRatio = (float)elitesBudgetRemaining / spawnsRemaining,
                    MiniBossBudgetRatio = (float)miniBossBudgetRemaining / spawnsRemaining,
                    ForceElite = instruction.ForceElite,
                    ForceMiniBoss = instruction.ForceMiniBoss
                };

                var variant = EnemyVariantGenerator.CreateVariant(attackerType, currentWave, defLevel, variantRequest);

                GameObject prefabToSpawn = ResolvePrefabForVariant(attackerType, variant.category);
                if (prefabToSpawn == null)
                {
                    Debug.LogError("No attacker prefab available for variant; skipping spawn.");
                    continue;
                }

                totalHealthMult += variant.healthMultiplier;
                totalSpeedMult += variant.speedMultiplier;
                totalDamageMult += variant.damageMultiplier;
                enemiesSpawnedThisWave++;

                if (variant.category == VariantCategory.Elite)
                {
                    elitesSpawnedThisWave++;
                }
                else if (variant.category == VariantCategory.MiniBoss)
                {
                    miniBossesSpawnedThisWave++;
                }

                GameObject enemy = Instantiate(prefabToSpawn, exactSpawnPoint, Quaternion.identity);

                if (enemy == null)
                {
                    Debug.LogError("Failed to instantiate enemy! Check attacker type prefab assignments.");
                    continue;
                }

                if (!Mathf.Approximately(variant.sizeMultiplier, 1f))
                {
                    enemy.transform.localScale = enemy.transform.localScale * variant.sizeMultiplier;
                }

                AttackerController attacker = enemy.GetComponent<AttackerController>();
                if (attacker != null)
                {
                    int enemyHealth = GetEnemyHealthForWave(variant.definition);

                    // Apply model selection based on wave and variant category
                    EnemyModelSelector.SelectModel(enemy, currentWave, variant.category);

                    // Get the specific path for this enemy to follow
                    List<Vector3> enemyPath = mapController.GetPathByIndex(pathIndex);

                    if (enemyPath.Count > 0)
                    {
                        attacker.Initialize(enemyHealth, enemyPath, exactSpawnPoint, variant.definition);
                        activeEnemies.Add(attacker);
                        enemiesAlive = activeEnemies.Count;
                        spawnedCount++;
                        Debug.Log($"Enemy ({typeLabel}) initialized with {enemyHealth} health, following path {pathIndex} with {enemyPath.Count} waypoints");
                    }
                    else
                    {
                        Debug.LogError($"No path found for index {pathIndex}, using all black tiles");
                        attacker.Initialize(enemyHealth, pathPoints, exactSpawnPoint, variant.definition);
                        activeEnemies.Add(attacker);
                        enemiesAlive = activeEnemies.Count;
                        spawnedCount++;
                    }

                    // Apply visual variant tint
                    var appearance = enemy.GetComponent<EnemyVariantAppearance>();
                    if (appearance == null)
                    {
                        appearance = enemy.AddComponent<EnemyVariantAppearance>();
                    }
                    appearance.Initialize(variant.tint);
                }
                else
                {
                    Debug.LogError("Enemy prefab missing AttackerController component!");
                    Destroy(enemy); // Clean up failed enemy
                }

                enemiesRemaining = Mathf.Max(0, enemiesRemaining - 1);
            }
            else
            {
                Debug.LogError("No spawn points available!");
                enemiesRemaining = 0;
                break;
            }

            yield return new WaitForSeconds(Mathf.Max(0f, instruction.Delay));
        }

        Debug.Log($"Finished spawning enemies. Spawned: {spawnedCount}, To spawn remaining: {enemiesRemaining}, Active enemies: {activeEnemies.Count}");

        if (spawnedCount > 0)
        {
            float avgHealth = totalHealthMult / spawnedCount;
            float avgSpeed = totalSpeedMult / spawnedCount;
            float avgDamage = totalDamageMult / spawnedCount;
            
            Debug.Log($"Wave {currentWave} stats - Avg Health: {avgHealth:F2}x, Speed: {avgSpeed:F2}x, Damage: {avgDamage:F2}x");
        }

        CheckWaveCompletion("spawn complete");
    }

    List<AttackerTypeDefinition> BuildAttackerSpawnQueue(int totalSpawns)
    {
        List<AttackerTypeDefinition> validTypes = GetValidAttackerTypes();

        if (validTypes == null || validTypes.Count == 0)
        {
            Debug.LogError("No valid attacker types available for spawning.");
            return new List<AttackerTypeDefinition>();
        }

        if (totalSpawns <= 0)
        {
            return new List<AttackerTypeDefinition>();
        }

        List<AttackerTypeDefinition> queue = new List<AttackerTypeDefinition>(totalSpawns);

        // Guarantee each type appears at least once when there are enough spawn slots
        if (totalSpawns >= validTypes.Count)
        {
            List<AttackerTypeDefinition> sortedByWeight = validTypes
                .OrderByDescending(type => type.SpawnWeight)
                .ToList();

            for (int i = 0; i < validTypes.Count && queue.Count < totalSpawns; i++)
            {
                queue.Add(sortedByWeight[i]);
            }
        }

        while (queue.Count < totalSpawns)
        {
            AttackerTypeDefinition nextType = SelectWeightedType(validTypes);
            if (nextType == null)
            {
                break;
            }

            queue.Add(nextType);
        }

        ShuffleList(queue);
        return queue;
    }

    List<AttackerTypeDefinition> GetValidAttackerTypes()
    {
        if (attackerTypes == null)
        {
            return new List<AttackerTypeDefinition>();
        }

        return attackerTypes
            .Where(type => type != null && type.Prefab != null)
            .ToList();
    }

    AttackerTypeDefinition SelectWeightedType(List<AttackerTypeDefinition> candidates)
    {
        if (candidates == null || candidates.Count == 0)
        {
            return null;
        }

        if (candidates.Count == 1)
        {
            return candidates[0];
        }

        float totalWeight = candidates.Sum(type => type.SpawnWeight);
        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (AttackerTypeDefinition type in candidates)
        {
            cumulative += type.SpawnWeight;
            if (roll <= cumulative)
            {
                return type;
            }
        }

        return candidates[candidates.Count - 1];
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int swapIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[swapIndex];
            list[swapIndex] = temp;
        }
    }

    int GetEnemyHealthForWave(AttackerTypeDefinition attackerType)
    {
        int baseHealth = attackerType != null ? attackerType.BaseHealth : 100;
        int scaled = DifficultyScaling.ScaleHealth(baseHealth, currentWave);
        return Mathf.RoundToInt(scaled * Mathf.Max(0.01f, waveHealthMultiplier));
    }

    GameObject ResolvePrefabForVariant(AttackerTypeDefinition baseType, VariantCategory category)
    {
        if (category == VariantCategory.MiniBoss && miniBossAttackerPrefabOverride != null)
        {
            return miniBossAttackerPrefabOverride;
        }

        if (category == VariantCategory.Elite && eliteAttackerPrefabOverride != null)
        {
            return eliteAttackerPrefabOverride;
        }

        if (normalAttackerPrefabOverride != null)
        {
            return normalAttackerPrefabOverride;
        }

        if (baseType != null && baseType.Prefab != null)
        {
            return baseType.Prefab;
        }

        return null;
    }

    void CleanupDeadEnemies()
    {
        activeEnemies.RemoveAll(enemy => enemy == null);
        enemiesAlive = activeEnemies.Count;
    }

    public void OnEnemyKilled(AttackerController enemy)
    {
        money += killReward;
        activeEnemies.Remove(enemy);
        CleanupDeadEnemies();
        UpdateUI();
        Debug.Log($"Enemy killed! +${killReward}, Money: ${money}, Alive: {enemiesAlive}, ToSpawn: {enemiesRemaining}");

        CheckWaveCompletion("enemy killed");
    }

    public void OnEnemyReachedEnd(AttackerController enemy)
    {
        int damageToPlayer = enemy != null ? Mathf.Max(0, enemy.DamageToPlayer) : healthLossPerEnemy;
        health -= damageToPlayer;
        activeEnemies.Remove(enemy);
        CleanupDeadEnemies();
        UpdateUI();
        Debug.Log($"Enemy reached end! Health lost: {damageToPlayer}, Health: {health}, Alive: {enemiesAlive}, ToSpawn: {enemiesRemaining}");

        CheckGameOverCondition("enemy reached end");
        if (currentPhase == GamePhase.GameOver)
        {
            return;
        }

        CheckWaveCompletion("enemy reached end");
    }

    public bool TrySpendMoney(int amount)
    {
        if (amount <= 0)
        {
            return true;
        }

        if (money < amount)
        {
            return false;
        }

        money -= amount;
        UpdateUI();
        return true;
    }

    public void AddMoney(int amount)
    {
        if (amount == 0)
        {
            return;
        }

        money = Mathf.Max(0, money + amount);
        UpdateUI();
    }

    public int GetTowerMaxHealth()
    {
        int bonus = UpgradeManager.Instance != null ? UpgradeManager.Instance.GetTowerHealthBonus() : 0;
        return startingHealth + Mathf.Max(0, bonus);
    }

    void CheckGameOverCondition(string source)
    {
        if (currentPhase == GamePhase.GameOver || health > 0)
        {
            return;
        }

        if (debugMode)
        {
            Debug.Log($"Player health reached 0 during {source}, triggering game over.");
        }

        GameOver();
    }

    void RecordWaveResult()
    {
        if (adaptiveDifficulty == null)
        {
            return;
        }

        WaveResult result = new WaveResult
        {
            WaveIndex = currentWave,
            StartingHealth = waveStartHealth,
            HealthLost = Mathf.Max(0, waveStartHealth - health),
            CombatDuration = combatPhaseStartTime > 0f ? Time.time - combatPhaseStartTime : 0f,
            EnemiesSpawned = enemiesSpawnedThisWave,
            ElitesSpawned = elitesSpawnedThisWave,
            MiniBossesSpawned = miniBossesSpawnedThisWave
        };

        adaptiveDifficulty.RecordWaveResult(result);
    }

    void CheckWaveCompletion(string source)
    {
        if (currentPhase == GamePhase.GameOver)
        {
            return;
        }

        if (currentPhase != GamePhase.Combat)
        {
            return;
        }

        // Ensure any null references are cleaned up before counting
        CleanupDeadEnemies();

        if (enemiesAlive > 0 || enemiesRemaining > 0)
        {
            if (debugMode)
            {
                Debug.Log($"[CheckWaveCompletion] Waiting ({source}) - Alive: {enemiesAlive}, ToSpawn: {enemiesRemaining}");
            }
            return;
        }

        Debug.Log($"Wave {currentWave} completed via {source}. Starting preparation for wave {currentWave + 1}.");

        RecordWaveResult();
        combatPhaseStartTime = 0f;

        currentWave++;
        StartPreparationPhase();
    }

    void GameOver()
    {
        if (currentPhase == GamePhase.GameOver)
        {
            return;
        }

        currentPhase = GamePhase.GameOver;
        StopAllCoroutines();
        activeEnemies.Clear();
        enemiesAlive = 0;
        enemiesRemaining = 0;
        combatPhaseStartTime = 0f;

        // Persist final stats for End Menu UI Toolkit
        PlayerPrefs.SetInt("LastWaveReached", currentWave);
        PlayerPrefs.SetInt("FinalMoney", money);
        PlayerPrefs.Save();

        // Reset time scale before transitioning to the end scene
        float defaultTimeScale = (timeScaleOptions != null && timeScaleOptions.Count > 0)
            ? Mathf.Max(0f, timeScaleOptions[0])
            : 1f;
        Time.timeScale = defaultTimeScale > 0f ? defaultTimeScale : 1f;

        const string endSceneName = "End Menu";

        if (debugMode)
        {
            Debug.Log($"Loading '{endSceneName}' asynchronously...");
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(endSceneName, LoadSceneMode.Single);
        if (asyncLoad == null)
        {
            Debug.LogError($"Failed to initiate async load for '{endSceneName}'. Ensure the scene is added to build settings.");
        }
    }

    void UpdateUI()
    {
        // UI Toolkit handles the HUD now - this method is kept for compatibility
        // InfoHudUIDocument updates the UI automatically
    }
}
