using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    private GameManager manager;

    void Start()
    {
        manager = FindObjectOfType<GameManager>();
    }

    public void LoadFirstLevel()
    {
        manager.LoadScene("level_1_a_v2");
    }

    public void LoadSecondLevel()
    {
        manager.LoadScene("level_2_a");
    }

    public void LoadThirdLevel()
    {
        manager.LoadScene("level_3_a");
    }

    public void LoadFourthLevel()
    {
        manager.LoadScene("level_4_a");
    }
    
    public void LoadEndlessMode()
    {
        manager.LoadScene("level_arena_nightclub");
    }


    public void Return()
    {
        manager.MainMenu();
    }
}
