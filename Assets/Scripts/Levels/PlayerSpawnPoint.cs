using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    public GameObject player;
    public GameManager manager;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        manager = FindObjectOfType<GameManager>();

        if(player.transform.parent == null && !manager.ongoingTransition)
        {
            player.transform.position = transform.position;
            player.transform.rotation = transform.rotation;
        } 
    }

    public void MovePlayerToSpawn()
    {
        player.transform.position = transform.position;
        player.transform.rotation = transform.rotation;
    }

}
