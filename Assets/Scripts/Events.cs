using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Events : MonoBehaviour
{
    public void Menu()
    {
        // Reset all persistent managers (coins, time, meters, score) before returning to menu
        GameResetter.ResetAll();
        SceneManager.LoadScene(0);
    }
    public void Level1()
    {
        SceneManager.LoadScene(1);
    }
        public void Level2()
    {
        SceneManager.LoadScene(2);
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void LoadSceneByName(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
