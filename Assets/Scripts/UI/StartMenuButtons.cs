using UnityEngine;

public class StartMenuButtons : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("Loading Game Scene...");
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Game");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
