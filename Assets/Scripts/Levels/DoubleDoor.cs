using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoubleDoor : MonoBehaviour
{
    public Transform leftDoor;
    public Transform rightDoor;
    public AudioSource doorSound;
    public AudioClip[] doorSounds;
    [SerializeField]
    private OffMeshLink link;

    // save positions for closed and open states of the door
    private Vector3 openPosition_left;
    private Vector3 openPosition_right;
    private Vector3 closedPosition_left;
    private Vector3 closedPosition_right;

    [Range(0,1)]
    public float doorSpeed;
    private float doorLerp;
    public float invokeDelay;

    [Header("Does this door start open?")]
    public bool startOpen;
    public bool lockAfterClosing;
    public bool openWithProximity;

    private InteractableButton button;
    private bool requireKeyCard;

    [Header("Door states")]
    public bool isOpening;
    private bool isOpen;
    public bool isClosing;
    private bool isClosed;
    public bool isLocked;
    [SerializeField]
    private bool autoClose;
    public float timeUntilClose;
    private float closeTimer;
    
    void Start()
    {
        doorSound = GetComponent<AudioSource>();

        openPosition_left = leftDoor.localPosition;
        openPosition_right = rightDoor.localPosition;

        // use of magic number, could make this not depend on that? figure that one out later
        closedPosition_left = new Vector3(leftDoor.localPosition.x, leftDoor.localPosition.y, (leftDoor.localPosition.z - 0.75f));
        closedPosition_right= new Vector3(rightDoor.localPosition.x, rightDoor.localPosition.y, (rightDoor.localPosition.z + 0.75f));

        if (!startOpen)
        {
            leftDoor.localPosition = closedPosition_left;
            rightDoor.localPosition = closedPosition_right;
            isClosed = true;
        }
        else
            isOpen = true;

        if(GetComponentInChildren<InteractableButton>() != null)
        {
            button = GetComponentInChildren<InteractableButton>();
            if (button.requireKeycard)
                requireKeyCard = true;
        }

        link = GetComponentInChildren<OffMeshLink>();

        if (requireKeyCard)
            link.gameObject.SetActive(false);

        //for (int i = 0; i < links.Length; i++)
            //links[i].GetComponent<MeshRenderer>().enabled = false;
    }

    void Update()
    {
        if (isOpening && !isOpen && !isLocked)
            Open();
        if (isClosing && !isClosed && !isLocked)
            Close();

        if (autoClose && isOpen)
        {
            closeTimer += Time.deltaTime;

            if (closeTimer >= timeUntilClose)
                Operate();
        }
    }

    public void Operate()
    {
        if (!isOpening && isClosed && !isLocked)
        {
            doorSound.PlayOneShot(doorSounds[0]);
            ToggleLinks();
            isOpening = true;
        }
        if (!isClosing && isOpen)
        {
            doorSound.PlayOneShot(doorSounds[0]);
            ToggleLinks();
            isClosing = true;
        }
    }

    void Open()
    {
        if(isClosed)
            isClosed = false;
        
        doorLerp += doorSpeed * Time.deltaTime;

        // basic lerp stuff for door opening and closing

        leftDoor.localPosition = Vector3.Lerp(closedPosition_left, openPosition_left, doorLerp);
        rightDoor.localPosition = Vector3.Lerp(closedPosition_right, openPosition_right, doorLerp);

        if (leftDoor.localPosition == openPosition_left || rightDoor.localPosition == openPosition_right)
        {
            doorSound.PlayOneShot(doorSounds[1]);

            isOpening = false;
            isOpen = true;
            closeTimer = 0f;
            doorLerp = 0f;
        }

    }

    void Close()
    {
        if(isOpen)
            isOpen = false;

        doorLerp += doorSpeed * Time.deltaTime;

        // basic lerp stuff for door opening and closing

        leftDoor.localPosition = Vector3.Lerp(openPosition_left, closedPosition_left, doorLerp);
        rightDoor.localPosition = Vector3.Lerp(openPosition_right, closedPosition_right, doorLerp);

        if (leftDoor.localPosition == closedPosition_left || rightDoor.localPosition == closedPosition_right)
        {
            doorSound.PlayOneShot(doorSounds[1]);

            isClosing = false;
            isClosed = true;
            closeTimer = 0f;
            doorLerp = 0f;

            if (lockAfterClosing)
                isLocked = true;
        }
    }

    public void InvokeDoor()
    {
        Invoke("Operate", invokeDelay);
    }

    public void Unlock()
    {
        requireKeyCard = false;
        link.gameObject.SetActive(true);
    }

    void ToggleLinks()
    {
        if (link.enabled)
            link.enabled = false;
        else
            link.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && isClosed && !requireKeyCard)
            Operate();

        if (openWithProximity)
            if (other.CompareTag("Player") && isClosed && !requireKeyCard)
                Operate();
    }
}
