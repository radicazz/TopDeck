using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class EndMenuUIDocument : MonoBehaviour
{
    [SerializeField] VisualTreeAsset _uxml;
    [SerializeField] StyleSheet _uss;

    UIDocument _doc;
    Label _waveLabel, _moneyLabel;
    Button _playAgainButton, _mainMenuButton, _quitButton;

    void Awake()
    {
        _doc = GetComponent<UIDocument>();
        if (_doc == null) _doc = gameObject.AddComponent<UIDocument>();
    }

    void OnEnable()
    {
        if (_doc.panelSettings == null)
        {
            var ps = Resources.FindObjectsOfTypeAll<PanelSettings>();
            if (ps.Length > 0) _doc.panelSettings = ps[0];
        }

        if (_uxml != null) _doc.visualTreeAsset = _uxml;
        var root = _doc.rootVisualElement;
        if (root == null) return;
        if (_uss != null && !root.styleSheets.Contains(_uss)) root.styleSheets.Add(_uss);

        _waveLabel = root.Q<Label>("wave-reached");
        _moneyLabel = root.Q<Label>("final-money");
        _playAgainButton = root.Q<Button>("play-again-button");
        _mainMenuButton = root.Q<Button>("main-menu-button");
        _quitButton = root.Q<Button>("quit-button");

        UpdateStats();
        WireButtons();
    }

    void OnDisable()
    {
        if (_playAgainButton != null) _playAgainButton.clicked -= PlayAgain;
        if (_mainMenuButton != null) _mainMenuButton.clicked -= MainMenu;
        if (_quitButton != null) _quitButton.clicked -= QuitGame;
    }

    void UpdateStats()
    {
        int wave = PlayerPrefs.GetInt("LastWaveReached", 1);
        int money = PlayerPrefs.GetInt("FinalMoney", 0);
        if (_waveLabel != null) _waveLabel.text = "Wave: " + wave;
        if (_moneyLabel != null) _moneyLabel.text = "Money: $" + money;
    }

    void WireButtons()
    {
        if (_playAgainButton != null) _playAgainButton.clicked += PlayAgain;
        if (_mainMenuButton != null) _mainMenuButton.clicked += MainMenu;
        if (_quitButton != null) _quitButton.clicked += QuitGame;
    }

    void PlayAgain() { SceneManager.LoadScene("Game"); }
    void MainMenu() { SceneManager.LoadScene("Start Menu"); }
    void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
