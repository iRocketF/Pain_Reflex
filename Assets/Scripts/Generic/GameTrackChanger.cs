using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTrackChanger : MonoBehaviour
{
    public GameManager manager;

    public int trackToPlay;

    void Start()
    {
        manager = FindObjectOfType<GameManager>();
    }

    public void ChangeTrack()
    {
        manager.gameMusic.clip = manager.tracks[trackToPlay];
        manager.gameMusic.Play();
    }

}
