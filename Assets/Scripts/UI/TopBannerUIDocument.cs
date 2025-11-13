using UnityEngine;
using UnityEngine.UIElements;

public class TopBannerUIDocument : MonoBehaviour
{
    [SerializeField] VisualTreeAsset _uxml;
    [SerializeField] StyleSheet _uss;
    
    UIDocument _doc;
    VisualElement _root;
    Label _moneyValue, _waveValue, _phaseLabel, _timerValue, _healthValue;
    
    void Awake()
    {
        _doc = GetComponent<UIDocument>();
        if (_doc == null) _doc = gameObject.AddComponent<UIDocument>();
    }

    void OnEnable()
    {
        if (_doc == null) _doc = GetComponent<UIDocument>();
        
        if (_doc.panelSettings == null)
        {
            var panelSettings = Resources.FindObjectsOfTypeAll<PanelSettings>();
            if (panelSettings.Length > 0)
            {
                _doc.panelSettings = panelSettings[0];
            }
        }
        
        if (_uxml != null) _doc.visualTreeAsset = _uxml;
        _root = _doc.rootVisualElement;
        
        if (_root != null && _uss != null && !_root.styleSheets.Contains(_uss))
        {
            _root.styleSheets.Add(_uss);
        }
        
        CacheElements();
        UpdateBanner();
    }

    void CacheElements()
    {
        if (_root == null) return;
        
        _moneyValue = _root.Q<Label>("banner-money-value");
        _waveValue = _root.Q<Label>("banner-wave-value");
        _phaseLabel = _root.Q<Label>("banner-phase");
        _timerValue = _root.Q<Label>("banner-timer-value");
        _healthValue = _root.Q<Label>("banner-health-value");
    }

    void Update()
    {
        if (Time.frameCount % 15 == 0)
        {
            UpdateBanner();
        }
    }

    void UpdateBanner()
    {
        var gc = GameController.Instance;
        if (gc == null) return;
        
        // Update money
        if (_moneyValue != null)
        {
            _moneyValue.text = "$" + gc.money;
        }
        
        // Update wave
        if (_waveValue != null)
        {
            _waveValue.text = gc.currentWave.ToString();
        }
        
        // Update phase
        if (_phaseLabel != null)
        {
            _phaseLabel.text = gc.currentPhase.ToString().ToUpper();
        }
        
        // Update timer
        if (_timerValue != null)
        {
            if (gc.currentPhase == GameController.GamePhase.Preparation)
            {
                float t = gc.GetPreparationTimer();
                _timerValue.text = FormatTime(t);
            }
            else if (gc.currentPhase == GameController.GamePhase.Combat)
            {
                int enemies = gc.GetEnemiesAlive();
                _timerValue.text = $"{enemies} LEFT";
            }
            else
            {
                _timerValue.text = "--:--";
            }
        }
        
        // Update health
        if (_healthValue != null)
        {
            _healthValue.text = gc.health.ToString();
            
            // Color based on health
            float healthPercent = (float)gc.health / gc.GetTowerMaxHealth();
            if (healthPercent > 0.6f)
            {
                _healthValue.style.color = new Color(0.3f, 0.9f, 0.3f); // Green
            }
            else if (healthPercent > 0.3f)
            {
                _healthValue.style.color = new Color(0.9f, 0.7f, 0.2f); // Yellow
            }
            else
            {
                _healthValue.style.color = new Color(0.9f, 0.3f, 0.3f); // Red
            }
        }
    }

    string FormatTime(float sec)
    {
        int s = Mathf.Max(0, Mathf.FloorToInt(sec));
        int m = s / 60;
        s = s % 60;
        return m.ToString("00") + ":" + s.ToString("00");
    }
}
