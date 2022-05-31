using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitionBox : MonoBehaviour
{
    public GameManager manager;
    public GameObject player;
    public Transform oldTransitionPoint;

    // this bool defines if the point is used to transition to another level
    // if false, point is simply non-functional other than to use as a receiving port
    public bool isTransitionPoint;

    public float levelTransitionTime;
    private float transitionTimer;

    private bool isTransitioning;

    // the idea here is to make a transition point between scenes where the player position stays consistent
    // upon entering the transition area, it's set as the player's parent to get the localPosition of the player
    // relative to the transition zone

    // upon loading the next scene, assign the new transition zone as the player's parent for a short while
    // teleport player to the localPosition relative to the transition zone, remove parent, continue? 
    // testing required

    // A LEVEL TRANSITION POINT IS NEEDED ON THE PREVIOUS AND THE NEXT SCENE, MAKE SURE TO CHECK
    // isTransitionPoint CORRECTLY!!!!!!

    void Start()
    {
        manager = FindObjectOfType<GameManager>();
        player = manager.player;

        if (!manager.ongoingTransition && !isTransitionPoint)
            gameObject.SetActive(false);
    }


    void Update()
    {
        if (isTransitionPoint && isTransitioning)
        {
            LevelTransition();
        }

        if (!isTransitionPoint && !isTransitioning && manager.ongoingTransition)
        {
            // this probably could be done a lot easier, but this is a method that works
            // so we'll go with it

            TransitionLogic();
        }
    }

    void TransitionLogic()
    {
        // here's what happens, step by step
        // the transition point transform the player used is taken
        // isTransitionPoint to false to prevent looping level transitions
        player = GameObject.FindWithTag("Player"); // find the player
        player.GetComponent<CustomCharacterController>().enabled = false;
        oldTransitionPoint = player.transform.parent;
        oldTransitionPoint.position = transform.position;

        // save the items the player has during this transition period
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        manager.itemsToReturn = inventory.SaveItems();
        manager.ammoToReturn = inventory.SaveAmmo();

        // unparent the player from the old transition point
        player.transform.SetParent(null, true);

        // destroy the old transition point, good riddance
        Destroy(oldTransitionPoint.gameObject);
        oldTransitionPoint = null;

        // the new transition point is now the player's parent!
        // now that there is a new parent, we can set the players localPosition from using the player's location from last scene! 
        // INGENIOUS if I say so myself
        player.transform.SetParent(transform, true);
        player.transform.localPosition = manager.transitionPosition;
        player.transform.localRotation = manager.transitionRotation;
        //Debug.Log("Player should be transformed INSIDE the elevator");

        // finally, set the player free from the transitioned point, should be fine and clean 
        player.transform.SetParent(null, true);
        manager.ongoingTransition = false;
        player.GetComponent<CustomCharacterController>().enabled = true;

        gameObject.SetActive(false);
    }


    void LevelTransition()
    {
        // start the timer
        transitionTimer += Time.deltaTime;
        // save player position every frame in the GameManager for after transition happens
        manager.transitionPosition = player.transform.localPosition;
        manager.transitionRotation = player.transform.localRotation;
        
        if(transitionTimer >= levelTransitionTime)
        {
            if (Input.GetButtonDown("Submit"))
            {
                transitionTimer = 0f;
                isTransitioning = false;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

                // TransitionLogic();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(isTransitionPoint)
        {
            if(other.gameObject.transform.parent.CompareTag("Player"))
            {
                player = other.gameObject.transform.parent.gameObject;
                player.transform.SetParent(transform, true);

                DontDestroyOnLoad(transform);

                manager.ongoingTransition = true;
                isTransitioning = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isTransitioning = false;
        transitionTimer = 0f;
    }

}
