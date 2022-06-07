using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    private GameManager manager;
    private ScoreManager scoreManager;

    [Header("The UI canvas in use")]
    public Canvas hudCanvas;
    public Canvas pauseCanvas;
    public Canvas pauseSettings;
    [SerializeField]
    private PauseMenu pauseMenu;

    [Header("UI icons and bars")]
    [Header("Health")]
    public Image healthBar;
    public Image healthBarGlow;
    public Color healthBarColorFull;
    [Header("Armor")]
    public Image armorBar;
    public Image crossHair;
    public Image matrixBar;
    public Image hurtSplash;
    public Image healSplash;
    [Header("Keycards")]
    public Image keycardIcon;
    public List<Sprite> icons;
    [Header("Level finish")]
    public Image blackScreen;
    public Image completion;

    [Header("UI text")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI ammoCountCurrent;
    public TextMeshProUGUI ammoCountReserve;
    public TextMeshProUGUI interactText;
    public TextMeshProUGUI deathText;
    public TextMeshProUGUI finishText;
    //public TextMeshProUGUI scoreText;

    [Header("UI related bools")]
    public bool isInteractableInFront;
    public bool isHurt;
    public bool isHealed;

    [Header("UI related variables")]
    [SerializeField] private float interactRange; // how far the player can interact with stuff
    [SerializeField] private LayerMask interactMask; // what objects the raycast checks for
    [SerializeField] private float hurtSplashTime; // how long the red splash last upon taking damage
    private float hurtTimer; // timer for functionality
    [SerializeField] private float healSplashTime; // how long does the red splash last upon taking damage
    private float healTimer; // timer for functionality
    public float keycardXOffset;

    public PlayerInventory pInventory;
    public CustomCharacterController player;

    public AudioSource hpIndicator;
    public AudioSource uiSound;
    public AudioClip[] sounds;
    public HealthBase pHealth;
    private AmmoBase ammo;
    private LayerMask playerMask;

    void Start()
    {
        manager = FindObjectOfType<GameManager>();
        scoreManager = manager.GetComponent<ScoreManager>();

        pauseMenu = transform.parent.GetComponentInChildren<PauseMenu>();

        //player = FindObjectOfType<CustomCharacterController>();
        //pInventory = player.GetComponent<PlayerInventory>();
        pHealth = player.GetComponent<HealthBase>();

        // uiSound = GetComponent<AudioSource>();

        playerMask = LayerMask.GetMask("Player");
    }

    // Update is called one per frame
    void Update()
    {
        if(manager == null)
            manager = FindObjectOfType<GameManager>();

        HudLogic();

        if (!player.isDead)
        {
            CrossHairLogic();
            CheckForInteract();

            if (isHurt)
                HurtSplashScreen();

            if (isHealed)
                HealSplashScreen();

            LowHealthEffects();
        }
        else
            crossHair.enabled = false;

        UpdateText();
    }

    public void UpdateHealthBar()
    {
        healthBar.fillAmount = pHealth.currentHealth / pHealth.maxHealth;
        healthBarGlow.fillAmount = healthBar.fillAmount;

        healthBar.color = Color.Lerp(Color.red, healthBarColorFull, healthBar.fillAmount);
        //healthBarGlow.color = Color.Lerp(Color.red, healthBarColorFull, healthBar.fillAmount);
    }

    public void UpdateArmorBar()
    {
        armorBar.fillAmount = pHealth.currentArmor / pHealth.maxArmor;
    }

    public void UpdateAmmoText()
    {
        if (pInventory.weaponInventory[0] != null && pInventory.weaponInventory[0].GetComponent<WeaponBase>() != null)
        {
            ammo = pInventory.weaponInventory[0].GetComponent<AmmoBase>();
            ammoCountCurrent.text = ammo.currentMag.ToString();
            ammoCountReserve.text = " / " + pInventory.currentAmmo[ammo.ammoInt].ToString();
        }
        else if (pInventory.weaponInventory[0] != null && pInventory.weaponInventory[0].GetComponent<MeleeWeapon>() != null)
        {
            MeleeWeapon knife = pInventory.weaponInventory[0].GetComponent<MeleeWeapon>();
            ammoCountCurrent.text = "1";
            ammoCountReserve.text = " + " + pInventory.currentAmmo[knife.ammoInt].ToString();
        }
        else
        {
            ammoCountCurrent.text = null;
            ammoCountReserve.text = null;
        }
    }

    void UpdateText()
    {
        timerText.text = Mathf.Round(manager.timer).ToString();
   
        if (isInteractableInFront)
            interactText.enabled = true;
        else
            interactText.enabled = false;

        if (!player.isDead)
            deathText.enabled = false;
        else
        {
            deathText.enabled = true;
            hurtSplash.enabled = true;
        }

        //scoreText.text = scoreManager.score.ToString();

        if (manager.ongoingTransition)
        {
            if(blackScreen.color.a < 1f)
            {
                var tempColor = blackScreen.color;
                tempColor.a += Time.deltaTime;
                blackScreen.color = tempColor;
            }
            else if (blackScreen.color.a >= 1f)
            {
                finishText.enabled = true;
                completion.enabled = true;
            }

        }
        else
        {
            finishText.enabled = false;
            completion.enabled = false;

            if (blackScreen.color.a > 0f)
            {
                var tempColor = blackScreen.color;
                tempColor.a -= Time.deltaTime;
                blackScreen.color = tempColor;
            }
        }
    }

    void HurtSplashScreen()
    {
        if (hurtTimer < hurtSplashTime)
        {
            hurtSplash.enabled = true;
            hurtSplash.color = new Color(hurtSplash.color.r, hurtSplash.color.g, hurtSplash.color.b, 1f);
            hurtTimer += Time.deltaTime;
        }
            
        if(hurtTimer >= hurtSplashTime)
        {
            isHurt = false;
            hurtSplash.enabled = false;
            hurtTimer = 0f;
        }
    }

    void HealSplashScreen()
    {
        if(!healSplash.enabled)
            healSplash.enabled = true;

        // make the health splash screen less transparent first and then reduce it to 0 for a pulse effect;
        if (healTimer < healSplashTime)
        {
            healTimer += Time.deltaTime;

            var tempColor = healSplash.color;

            if (healTimer < (healSplashTime / 3f))
            {
                tempColor.a += Time.deltaTime / (healSplashTime / 3);
                healSplash.color = tempColor;
            }
            else
            {
                tempColor.a -= Time.deltaTime / (healSplashTime / 3);
                healSplash.color = tempColor;
            }

            if(healTimer >= healSplashTime)
            {
                isHealed = false;
                healSplash.enabled = false;
                tempColor.a = 0f;
                healSplash.color = tempColor;
                healTimer = 0f;
            }
        }
    }

    // this code below checks for interactable(pickup) objects in front of the player
    // floating contextual text shows up when the player is looking at an object at close enough range
    // text gets disabled when not looking at an interactable object

    // CLEAN UP COMPLETE

    void CheckForInteract()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward) * interactRange;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, interactRange, interactMask))
        {
            if (hit.transform.gameObject.GetComponent<InteractablePickUp>() != null && hit.transform.parent == null)
            {
                InteractablePickUp interactObject = hit.transform.gameObject.GetComponent<InteractablePickUp>();

                isInteractableInFront = true;

                interactText.text = interactObject.ReturnInteractText(pInventory, pHealth);

                if (Input.GetButtonDown("Interact"))
                    interactObject.AttemptPickUp(pInventory, pHealth, this);
                
            }
            else if (hit.transform.gameObject.GetComponent<InteractableButton>() != null)
            {
                InteractableButton interactObject = hit.transform.gameObject.GetComponent<InteractableButton>();

                isInteractableInFront = true;

                interactText.text = interactObject.ReturnInteractText(pInventory);

                if(Input.GetButtonDown("Interact"))
                    interactObject.ButtonInteract(pInventory);
                    
            }
            else if (hit.transform.gameObject.GetComponent<InteractablePickUp>() == null ||
                hit.transform.gameObject.GetComponent<InteractableButton>() == null)
                isInteractableInFront = false;
        }
        else
            isInteractableInFront = false;
    }

    void CrossHairLogic()
    {
        WeaponBase weapon;

        if (pInventory.weaponInventory[0] != null &&
            pInventory.weaponInventory[0].GetComponent<WeaponBase>() != null)
        {
            weapon = pInventory.weaponInventory[0].GetComponent<WeaponBase>();
            crossHair.enabled = true;
        }
        /*else if (isInteractableInFront)
            crossHair.enabled = false;
        else
            crossHair.enabled = true; */

        if (manager.vipKilled)
            crossHair.enabled = false;
                 
    }

    void HudLogic()
    {
        if (!manager.inMenus)
        {
            if (Input.GetButtonDown("Escape"))
            {
                if (pauseMenu.inSettings)
                    pauseMenu.inSettings = false;
            }
        }

        if (manager.isPaused && !pauseMenu.inSettings)
        {
            hudCanvas.enabled = false;
            pauseCanvas.enabled = true;
            pauseSettings.enabled = false;
        }
        else if (manager.isPaused && pauseMenu.inSettings)
        {
            hudCanvas.enabled = false;
            pauseCanvas.enabled = false;
            pauseSettings.enabled = true;
        }
        else if (!manager.isPaused)
        {
            hudCanvas.enabled = true;
            pauseCanvas.enabled = false;
            pauseSettings.enabled = false;
        }


    }

    void LowHealthEffects()
    {

        if(pHealth.currentHealth <= pHealth.maxHealth / 2f)
        {         
            if (!hpIndicator.isPlaying)
                hpIndicator.Play();      
            if(!hurtSplash.enabled && !isHurt)
            {
                hurtSplash.enabled = true;
            }
            if(hurtSplash.enabled && !isHurt)
            {
                var tempColor = hurtSplash.color;
                tempColor.a = Mathf.Lerp(1, 0, pHealth.currentHealth / pHealth.maxHealth);
                hurtSplash.color = tempColor;
            }

        }
        else
        {
            if (hpIndicator.isPlaying)
                hpIndicator.Stop();
            if (hurtSplash.enabled && !isHurt)
                hurtSplash.enabled = false;
        }

        hpIndicator.volume = Mathf.Lerp(1, 0, pHealth.currentHealth / pHealth.maxHealth);

    }


}
