using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EndMenuController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI waveReachedText;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    void Start()
    {
        SetupUI();

        // Setup button events
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(PlayAgain);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    void SetupUI()
    {
        // Get final score from GameController if available
        // For now, use placeholder values
        int finalWave = PlayerPrefs.GetInt("LastWaveReached", 1);
        int finalMoney = PlayerPrefs.GetInt("FinalMoney", 0);

        if (finalScoreText != null)
            finalScoreText.text = "Final Money: $" + finalMoney;

        if (waveReachedText != null)
            waveReachedText.text = "Wave Reached: " + finalWave;
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("Game");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Start Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
