using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public void LoadNextScene()
    {
        Debug.Log("Load next scene");
        //int currentScene = SceneManager.GetActiveScene().buildIndex;
        //SceneManager.LoadScene(currentScene + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadGameOverScene()
    {

    }

    public void LoadGameScene()
    {

    }

    IEnumerator PlayerWins()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("Player Wins! Congratulations");
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("Game Over");
    }
}
