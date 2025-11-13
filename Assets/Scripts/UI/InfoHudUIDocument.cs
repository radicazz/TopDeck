using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class InfoHudUIDocument : MonoBehaviour
{
    [SerializeField] VisualTreeAsset _uxml;
    [SerializeField] StyleSheet _uss;
    UIDocument _doc;
    Label _healthValue, _enemiesValue, _defendersValue, _statusMessage, _moneyValue, _waveValue;
    ProgressBar _healthBar;
    VisualElement _root, _hudRoot;
    Button _dockToggle;
    bool _isVisible = true;
    bool _isDocked = false;
    bool _contentInitialized;

    void Awake()
    {
        _doc = GetComponent<UIDocument>();
        if (_doc == null)
        {
            _doc = gameObject.AddComponent<UIDocument>();
        }
    }

    void OnEnable()
    {
        if (_doc == null)
        {
            _doc = GetComponent<UIDocument>();
        }

        _contentInitialized = false;
        EnsurePanelSettings();
        LoadAssets();
        RegisterRootCallback();
    }

    void OnDisable()
    {
        TearDownDockToggle();
        UnregisterRootCallback();
        _contentInitialized = false;
        _root = null;
        _hudRoot = null;
        _healthValue = null;
        _enemiesValue = null;
        _defendersValue = null;
        _statusMessage = null;
        _healthBar = null;
    }

    void SetupDockToggle()
    {
        if (_root == null)
        {
            return;
        }

        _dockToggle = _root.Q<Button>("dock-toggle");
        if (_dockToggle != null)
        {
            _dockToggle.clicked += ToggleDock;
            UpdateDockButton();
        }
    }

    void ToggleDock()
    {
        _isDocked = !_isDocked;
        
        if (_hudRoot != null)
        {
            if (_isDocked)
            {
                _hudRoot.AddToClassList("docked");
            }
            else
            {
                _hudRoot.RemoveFromClassList("docked");
            }
        }
        
        UpdateDockButton();
    }

    void UpdateDockButton()
    {
        if (_dockToggle != null)
        {
            _dockToggle.text = _isDocked ? ">" : "<";
        }
    }

    void CacheElements()
    {
        if (_root == null) return;
        _healthValue = _root.Q<Label>("health-value");
        _enemiesValue = _root.Q<Label>("enemies-value");
        _defendersValue = _root.Q<Label>("defenders-value");
        _moneyValue = _root.Q<Label>("money-value");
        _waveValue = _root.Q<Label>("wave-value");
        _statusMessage = _root.Q<Label>("status-message");
        _healthBar = _root.Q<ProgressBar>("health-bar");
    }

    float _nextTick;
    void Update()
    {
        if (Time.unscaledTime >= _nextTick)
        {
            _nextTick = Time.unscaledTime + 0.25f;
            UpdateHud();
        }
    }

    void UpdateHud()
    {
        var gc = GameController.Instance;
        if (gc == null) return;
        
        // Update health
        if (_healthValue != null && _healthBar != null)
        {
            int maxHealth = gc.GetTowerMaxHealth();
            _healthValue.text = $"{gc.health} / {maxHealth}";
            _healthBar.highValue = maxHealth;
            _healthBar.value = gc.health;
            
            // Color health bar
            float healthPercent = (float)gc.health / maxHealth;
            Color healthColor = healthPercent > 0.6f ? new Color(0.3f, 0.69f, 0.31f) :
                               healthPercent > 0.3f ? new Color(1f, 0.76f, 0.03f) :
                               new Color(0.96f, 0.26f, 0.21f);
            _healthBar.Q(className: "unity-progress-bar__progress").style.backgroundColor = healthColor;
        }
        
        // Update enemies count
        if (_enemiesValue != null)
        {
            int enemies = gc.GetEnemiesAlive();
            _enemiesValue.text = enemies.ToString();
        }

        // Update money and wave
        if (_moneyValue != null)
        {
            _moneyValue.text = $"${gc.money}";
        }
        if (_waveValue != null)
        {
            _waveValue.text = gc.currentWave.ToString();
        }
        
        // Update defenders count (placeholder - implement when defender tracking is available)
        if (_defendersValue != null)
        {
            int defenders = gc.GetActiveDefenderCount();
            int limit = gc.GetDefenderLimit();
            _defendersValue.text = limit > 0 ? $"{defenders} / {limit}" : defenders.ToString();
        }
        
        // Update status message
        if (_statusMessage != null)
        {
            string message = gc.currentPhase switch
            {
                GameController.GamePhase.Preparation => "Build your defenses!",
                GameController.GamePhase.Combat => "Defend the tower!",
                GameController.GamePhase.GameOver => "Game Over",
                _ => "Ready"
            };
            _statusMessage.text = message;
        }
    }

    void EnsurePanelSettings()
    {
        if (_doc == null || _doc.panelSettings != null)
        {
            return;
        }

        var panelSettings = Resources.FindObjectsOfTypeAll<PanelSettings>();
        if (panelSettings.Length > 0)
        {
            _doc.panelSettings = panelSettings[0];
            return;
        }

        var ps = ScriptableObject.CreateInstance<PanelSettings>();
        ps.scaleMode = PanelScaleMode.ScaleWithScreenSize;
        ps.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
        ps.referenceResolution = new Vector2Int(1920, 1080);
        _doc.panelSettings = ps;
    }

    void LoadAssets()
    {
        if (_uxml == null)
        {
            _uxml = Resources.Load<VisualTreeAsset>("UI/InfoHud");
        }

        #if UNITY_EDITOR
        if (_uxml == null)
        {
            _uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/InfoHud.uxml");
        }
        #endif

        if (_uss == null)
        {
            _uss = Resources.Load<StyleSheet>("UI/InfoHud");
        }

        #if UNITY_EDITOR
        if (_uss == null)
        {
            _uss = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI/InfoHud.uss");
        }
        #endif

        if (_uxml == null)
        {
            Debug.LogWarning("[InfoHudUIDocument] InfoHud.uxml could not be loaded; HUD will stay empty.");
        }

        if (_uss == null)
        {
            Debug.LogWarning("[InfoHudUIDocument] InfoHud.uss could not be loaded; styling may be missing.");
        }

        if (_uxml != null && _doc != null)
        {
            _doc.visualTreeAsset = _uxml;
        }
    }

    void RegisterRootCallback()
    {
        if (_doc == null)
        {
            return;
        }

        var root = _doc.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("[InfoHudUIDocument] UIDocument root visual element is null!");
            return;
        }

        root.RegisterCallback<AttachToPanelEvent>(OnRootAttached);
        if (root.panel != null)
        {
            InitializeHud();
        }
    }

    void UnregisterRootCallback()
    {
        if (_doc == null)
        {
            return;
        }

        var root = _doc.rootVisualElement;
        if (root != null)
        {
            root.UnregisterCallback<AttachToPanelEvent>(OnRootAttached);
        }
    }

    void OnRootAttached(AttachToPanelEvent evt)
    {
        InitializeHud();
    }

    void InitializeHud()
    {
        if (_contentInitialized || _doc == null)
        {
            return;
        }

        _root = _doc.rootVisualElement;
        if (_root == null)
        {
            Debug.LogError("[InfoHudUIDocument] Root visual element is null!");
            return;
        }

        if (_uss != null && !_root.styleSheets.Contains(_uss))
        {
            _root.styleSheets.Add(_uss);
        }

        _hudRoot = _root.Q<VisualElement>("hud-root");
        if (_hudRoot != null)
        {
            _hudRoot.style.display = DisplayStyle.Flex;
            _isVisible = true;
        }

        CacheElements();
        SetupDockToggle();
        UpdateHud();
        _contentInitialized = true;
        UnregisterRootCallback();
    }

    void TearDownDockToggle()
    {
        if (_dockToggle != null)
        {
            _dockToggle.clicked -= ToggleDock;
        }
        _dockToggle = null;
    }
}
