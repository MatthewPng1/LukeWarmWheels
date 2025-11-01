using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Events : MonoBehaviour
{
    public void Menu()
    {
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
        SceneManager.LoadScene(sceneName);
    }
}
