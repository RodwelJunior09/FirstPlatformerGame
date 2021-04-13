using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void LoadNextScene()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadGameMenu()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void LoadFirstLevel()
    {
        SceneManager.LoadScene("IntroductionToTheCave");
        FindObjectOfType<LifeStatus>().ResetGame();
    }

    public void LoadWinLevel()
    {
        StartCoroutine(PlayerWins());
    }

    public void LoadGameOver()
    {
        StartCoroutine(GameOver());
    }

    public void LoadCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    IEnumerator PlayerWins()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("WinScreen");
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("LoseScreen");
    }
}
