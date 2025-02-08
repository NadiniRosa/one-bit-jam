using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CheckGameOver(bool isPlayerDead)
    {
        if (isPlayerDead)
        {
            LoadScene("Defeat");
        }
        else
        {
            LoadScene("Victory");
        }
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void TryAgain()
    {
        LoadScene("Game");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
