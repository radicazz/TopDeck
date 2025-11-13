using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Cleaned up UI Toolkit upgrade panel with proper show/hide and ESC support.
/// Right-side panel, narrower, more creative design.
/// </summary>
public class UpgradePanelUIDocument : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset _uxml;
    [SerializeField] private StyleSheet _uss;
    [SerializeField] private KeyCode closeKey = KeyCode.Escape;
    
    private UIDocument _doc;
    private VisualElement _root, _upgradeRoot, _panelContainer;
    private Label _towerTitle, _towerLevel, _towerDesc, _statusText;
    private Button _towerBtn, _closeBtn, _dockToggle;
    private ProgressBar _towerProg;
    private VisualElement _emptyState, _towerContent;
    private bool _isVisible;
    private bool _isDocked = false;
    private bool _hasTowerSelected = false;
    
    // Reference to InfoHud to hide it
    private InfoHudUIDocument _infoHud;

    void Awake()
    {
        _doc = GetComponent<UIDocument>();
        if (_doc == null) _doc = gameObject.AddComponent<UIDocument>();
        
        _infoHud = FindFirstObjectByType<InfoHudUIDocument>();
    }

    void OnEnable()
    {
        if (_doc == null) _doc = GetComponent<UIDocument>();
        
        // Find panel settings asset
        if (_doc.panelSettings == null)
        {
            var panelSettings = Resources.FindObjectsOfTypeAll<PanelSettings>();
            if (panelSettings.Length > 0)
            {
                _doc.panelSettings = panelSettings[0];
            }
            else
            {
                var ps = ScriptableObject.CreateInstance<PanelSettings>();
                ps.scaleMode = PanelScaleMode.ScaleWithScreenSize;
                ps.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
                ps.referenceResolution = new Vector2Int(1920, 1080);
                _doc.panelSettings = ps;
            }
        }
        
        if (_uxml != null) _doc.visualTreeAsset = _uxml;
        
        _root = _doc.rootVisualElement;
        if (_root != null && _uss != null && !_root.styleSheets.Contains(_uss))
        {
            _root.styleSheets.Add(_uss);
        }
        
        // Get upgrade-root for docking
        _upgradeRoot = _root.Q<VisualElement>("upgrade-root");
        _panelContainer = _root.Q<VisualElement>("upgrade-panel");
        
        CacheElements(_root);
        InitTexts();
        HookButtons();
        SetupDockToggle();
        ApplyTheme();
        
        // Start hidden
        HidePanel();
    }

    void SetupDockToggle()
    {
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
        
        if (_upgradeRoot != null)
        {
            if (_isDocked)
            {
                _upgradeRoot.AddToClassList("docked");
            }
            else
            {
                _upgradeRoot.RemoveFromClassList("docked");
            }
        }
        
        UpdateDockButton();
    }

    void UpdateDockButton()
    {
        if (_dockToggle != null)
        {
            _dockToggle.text = _isDocked ? "<" : ">";
        }
    }

    void Update()
    {
        // ESC to close
        if (_isVisible && Input.GetKeyDown(closeKey))
        {
            HidePanel();
        }
        
        // Auto-update levels and tower selection status
        if (_isVisible && Time.frameCount % 30 == 0)
        {
            var gc = GameController.Instance;
            bool currentlySelected = gc != null && gc.IsTowerSelected();
            
            if (currentlySelected != _hasTowerSelected)
            {
                _hasTowerSelected = currentlySelected;
                UpdateContentVisibility();
            }
            
            if (_hasTowerSelected)
            {
                UpdateLevels();
            }
        }
    }

    void CacheElements(VisualElement root)
    {
        if (root == null) return;
        
        _towerTitle = root.Q<Label>("tower-title");
        _towerLevel = root.Q<Label>("tower-level");
        _towerDesc = root.Q<Label>("tower-desc");
        _towerBtn = root.Q<Button>("tower-upgrade");
        _closeBtn = root.Q<Button>("close-btn");
        _towerProg = root.Q<ProgressBar>("tower-progress");
        _statusText = root.Q<Label>("status-text");
        
        // Cache empty state and tower content containers
        _emptyState = root.Q<VisualElement>("empty-state");
        _towerContent = root.Q<VisualElement>("tower-content");
    }

    void InitTexts()
    {
        if (_towerTitle != null) _towerTitle.text = "TOWER DEFENSE";
        if (_towerDesc != null) _towerDesc.text = "Fortify your defenses";
        if (_statusText != null) _statusText.text = "Ready to upgrade";
    }

    void HookButtons()
    {
        if (_towerBtn != null)
        {
            _towerBtn.clicked += OnUpgradeClicked;
        }
        
        if (_closeBtn != null)
        {
            _closeBtn.clicked += () => HidePanel();
        }
    }

    void OnUpgradeClicked()
    {
        var shop = FindFirstObjectByType<UpgradeShop>();
        bool ok = false;
        
        if (shop != null)
        {
            ok = shop.TryPurchaseTowerUpgrade();
        }
        else if (UpgradeManager.Instance != null)
        {
            if (UpgradeManager.Instance.CanUpgradeTower() && 
                GameController.Instance != null && 
                GameController.Instance.TrySpendMoney(300))
            {
                UpgradeManager.Instance.UpgradeTower();
                ok = true;
            }
        }
        
        UpdateLevels();
        
        if (_statusText != null)
        {
            _statusText.text = ok ? "✓ Upgrade successful!" : "✗ Insufficient funds";
        }
    }

    void UpdateLevels()
    {
        int tl = 0, tmax = 1;
        
        if (UpgradeManager.Instance != null)
        {
            tl = UpgradeManager.Instance.GetTowerLevel();
            tmax = UpgradeManager.Instance.GetTowerMaxLevel();
        }
        
        if (_towerLevel != null)
        {
            _towerLevel.text = $"LV {tl}";
        }
        
        if (_towerProg != null)
        {
            _towerProg.highValue = tmax;
            _towerProg.value = tl;
        }
        
        bool tMaxed = tmax > 0 && tl >= tmax;
        
        if (_towerBtn != null)
        {
            var shop = FindFirstObjectByType<UpgradeShop>();
            
            if (tMaxed)
            {
                _towerBtn.text = "⭐ MAX LEVEL";
                _towerBtn.SetEnabled(false);
                if (_towerDesc != null)
                {
                    _towerDesc.text = "Maximum fortification achieved!";
                }
            }
            else
            {
                int cost = shop != null ? shop.TowerUpgradeCost : 300;
                _towerBtn.text = $"⬆ UPGRADE (${cost})";
                
                bool canAfford = GameController.Instance != null && 
                                 GameController.Instance.money >= cost;
                _towerBtn.SetEnabled(canAfford);
                
                if (_towerDesc != null)
                {
                    _towerDesc.text = $"Next level grants +{(shop != null ? 50 : 50)} HP";
                }
            }
        }
    }

    void ApplyTheme()
    {
        var tm = FindFirstObjectByType<ThemeManager>();
        var pal = tm != null ? tm.ActivePalette : null;
        if (tm == null || pal == null)
        {
            return;
        }

        if (_panelContainer != null)
        {
            _panelContainer.style.backgroundColor = pal.PanelBackground;
        }

        if (_towerBtn != null)
        {
            _towerBtn.style.backgroundColor = pal.ButtonNormal;
            _towerBtn.style.color = pal.TextPrimary;
        }

        if (_closeBtn != null)
        {
            _closeBtn.style.backgroundColor = pal.ButtonPressed;
            _closeBtn.style.color = pal.TextPrimary;
        }

        if (_statusText != null)
        {
            _statusText.style.color = pal.Info;
        }
    }

    public void ShowPanel()
    {
        if (_panelContainer != null)
        {
            _panelContainer.style.display = DisplayStyle.Flex;
            _isVisible = true;

            var gc = GameController.Instance;
            _hasTowerSelected = gc != null && gc.IsTowerSelected();

            UpdateContentVisibility();

            if (_hasTowerSelected)
            {
                UpdateLevels();
            }

            Debug.Log("[UpgradePanelUIDocument] Panel shown");
        }
    }

    void UpdateContentVisibility()
    {
        if (_emptyState != null && _towerContent != null)
        {
            if (_hasTowerSelected)
            {
                _emptyState.style.display = DisplayStyle.None;
                _towerContent.style.display = DisplayStyle.Flex;
            }
            else
            {
                _emptyState.style.display = DisplayStyle.Flex;
                _towerContent.style.display = DisplayStyle.None;
            }
        }
    }

    public void HidePanel()
    {
        if (_panelContainer != null)
        {
            _panelContainer.style.display = DisplayStyle.None;
            _isVisible = false;

            // Auto-deselect tower when panel closes
            var gc = GameController.Instance;
            if (gc != null)
            {
                var method = typeof(GameController).GetMethod("ClearTowerSelection",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (method != null)
                {
                    method.Invoke(gc, null);
                }
            }

            Debug.Log("[UpgradePanelUIDocument] Panel hidden, tower deselected");
        }
    }

    public void TogglePanel()
    {
        if (_isVisible)
        {
            HidePanel();
        }
        else
        {
            ShowPanel();
        }
    }

    public bool IsVisible => _isVisible;
}
