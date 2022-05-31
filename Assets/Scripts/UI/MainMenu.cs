using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameManager manager;

    void Start()
    {
        manager = FindObjectOfType<GameManager>();
    }

    public void Play()
    {
        manager.LoadScene("level_1_a");
    }

    public void LevelSelect()
    {
        manager.LoadScene("sceneselection");
    }

    public void Settings()
    {
        manager.LoadScene("settings");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
