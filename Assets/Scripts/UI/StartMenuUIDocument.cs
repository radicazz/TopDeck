using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class StartMenuUIDocument : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset _uxml;
    [SerializeField] private StyleSheet _uss;
    
    private UIDocument _doc;
    private VisualElement _root;
    private Button _startButton, _quitButton;

    void Awake()
    {
        _doc = GetComponent<UIDocument>();
        if (_doc == null) _doc = gameObject.AddComponent<UIDocument>();
    }

    void Start()
    {
        // Delay setup to ensure AutoWirer has configured everything
        SetupUI();
    }

    void SetupUI()
    {
        if (_doc == null) _doc = GetComponent<UIDocument>();
        
        // Get root from UIDocument (AutoWirer already set visualTreeAsset)
        _root = _doc.rootVisualElement;
        
        if (_root == null)
        {
            Debug.LogError("[StartMenuUIDocument] Root visual element is null!");
            return;
        }
        
        // Add USS if needed
        if (_uss != null && !_root.styleSheets.Contains(_uss))
        {
            _root.styleSheets.Add(_uss);
        }
        
        CacheElements();
        SetupButtons();
    }

    void CacheElements()
    {
        if (_root == null) return;
        
        _startButton = _root.Q<Button>("start-button");
        _quitButton = _root.Q<Button>("quit-button");
        
        if (_startButton == null)
        {
            Debug.LogWarning("[StartMenuUIDocument] start-button not found in UXML!");
        }
        if (_quitButton == null)
        {
            Debug.LogWarning("[StartMenuUIDocument] quit-button not found in UXML!");
        }
    }

    void SetupButtons()
    {
        if (_startButton != null)
        {
            _startButton.clicked += StartGame;
            Debug.Log("[StartMenuUIDocument] Start button wired");
        }
        
        if (_quitButton != null)
        {
            _quitButton.clicked += QuitGame;
            Debug.Log("[StartMenuUIDocument] Quit button wired");
        }
    }

    void StartGame()
    {
        Debug.Log("[StartMenuUIDocument] Loading Game Scene...");
        SceneManager.LoadSceneAsync("Game");
    }

    void QuitGame()
    {
        Debug.Log("[StartMenuUIDocument] Quitting Game...");
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    void OnDestroy()
    {
        if (_startButton != null)
        {
            _startButton.clicked -= StartGame;
        }
        
        if (_quitButton != null)
        {
            _quitButton.clicked -= QuitGame;
        }
    }
}
