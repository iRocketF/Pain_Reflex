using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableButton : MonoBehaviour
{
    private MeshRenderer rend;

    public Material neutral;
    public Material used;

    public bool wasUsed;
    public float timeUntilNeutral;
    public float timer;
    public string customString;

    [Header("Button/Keycard related variables")]
    public bool requireKeycard;
    public bool disableAfterUse;
    public bool isDisabled;

    public List<Color> keycardColor = new List<Color>();

    [Header("0 = Green, 1 = Blue, 2 = Yellow, 3 = Red, 4 = Black")]
    public int colorInt;
    public Color requiredColor;
    private string colorName;
    private Light keyLight;

    // button sounds
    // 0. positive sound for access
    // 1. negative sound for rejection
    private AudioSource buttonSound;
    public AudioClip[] buttonSoundClips;

    private Outline outline;

    private void Start()
    {
        rend = GetComponent<MeshRenderer>();
        rend.material = neutral;

        if (requireKeycard)
            rend.material.color = keycardColor[colorInt];

        requiredColor = keycardColor[colorInt];

        switch(colorInt)
        {
            case 0:
                colorName = "Green";
                break;
            case 1:
                colorName = "Blue";
                break;
            case 2:
                colorName = "Yellow";
                break;
            case 3:
                colorName = "Red";
                break;
        }

        if(GetComponentInChildren<Light>() != null)
        {
            keyLight = GetComponentInChildren<Light>();
            keyLight.color = requiredColor;
        }

        buttonSound = GetComponent<AudioSource>();

        outline = GetComponent<Outline>();

    }

    private void Update()
    {
        if (wasUsed)
        {
            timer += Time.deltaTime;

            if (timer >= timeUntilNeutral)
            {
                rend.material = neutral;
                wasUsed = false;
                timer = 0f;
            }
        }
    }

    public string ReturnInteractText(PlayerInventory inventory)
    {
        outline.enabled = true;

        if (!requireKeycard && !isDisabled && customString.Length == 0)
            return "Use button";

        if (requireKeycard && !isDisabled && inventory.CheckKeyCards(requiredColor) == requiredColor && customString.Length == 0)
            return "Use keycard";

        if (requireKeycard && !isDisabled && inventory.CheckKeyCards(requiredColor) != requiredColor && customString.Length == 0)
            return colorName + " keycard required";

        if (isDisabled && customString != null)
            return "Button locked";

        if (customString.Length > 0 && !requireKeycard && !isDisabled)
            return customString;



        return null;
    }

    public void ButtonInteract(PlayerInventory inventory)
    {
        if (requireKeycard)
        {
            if (inventory.CheckKeyCards(requiredColor) == requiredColor)
            {
                if(!isDisabled)
                {
                    Button button = gameObject.GetComponent<Button>();
                    ButtonAccess();
                    button.onClick.Invoke();
                    requireKeycard = false;

                    if (GetComponentInParent<DoubleDoor>() != null)
                        GetComponentInParent<DoubleDoor>().Unlock();
                }
                else
                    ButtonReject();


                if (disableAfterUse && !isDisabled)
                    isDisabled = true;
            }

            else if (inventory.CheckKeyCards(requiredColor) != requiredColor)
            {
                ButtonReject();
            }
}
        else if (!requireKeycard)
        {
            if (!isDisabled)
            {
                Button button = gameObject.GetComponent<Button>();
                ButtonAccess();
                button.onClick.Invoke();
            }
            else
                ButtonReject();

            if (disableAfterUse && !isDisabled)
                isDisabled = true;
        }

    }

    void ButtonAccess()
    {
        buttonSound.PlayOneShot(buttonSoundClips[0]);

        if(used != null)
            rend.material = used;

        wasUsed = true;
    }

    void ButtonReject()
    {
        buttonSound.PlayOneShot(buttonSoundClips[1]);
        rend.material.color = keycardColor[colorInt];
    }

    public void UnlockButton()
    {
        isDisabled = false;
    }
}
