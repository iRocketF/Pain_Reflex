using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class ReflexMode : MonoBehaviour
{
    private CustomCharacterController controller;
    private Rigidbody rb_player;
    private PlayerHUD hud;
    private GameManager manager;
    [SerializeField] private AudioSource playerSound;

    [Header("Reflex Mode variables")]
    [SerializeField] private float slowMoScale;
    [SerializeField] private float maxStamina;
    [SerializeField] private float currentStamina;
    [SerializeField] private float drainOnToggle;
    [SerializeField] private float drainOverTime;
    [SerializeField] private float regenOverTime;
    [SerializeField] private float rechargeCoolDown;
    private float rechargeTimer;
    [SerializeField] private AudioClip[] slowMoSounds;

    [Header("")]
    private float returnLerp;
    [SerializeField] private float returnTime;
    private float returnTimer;
    [SerializeField] private bool returningToRealTime;

    [Header("Reflex Mode buffs")]
    public float damageReduction;
    public float regenOnHit;
    public float bulletForceMultiplier;

    [Header("Reflex mode Post-processing")]
    [SerializeField] private PostProcessVolume volume;
    [SerializeField] private Vignette vignette;
    [SerializeField] private MotionBlur blur;

    private void Start()
    {
        controller = GetComponent<CustomCharacterController>();
        //playerSound = GetComponent<AudioSource>();
        hud = GetComponentInChildren<PlayerHUD>();
        manager = FindObjectOfType<GameManager>();

        currentStamina = maxStamina;
        hud.matrixBar.fillAmount = currentStamina / maxStamina;
      
        volume = FindObjectOfType<PostProcessVolume>();
        volume.profile.TryGetSettings(out vignette);
        volume.profile.TryGetSettings(out blur);

        PlayerHealth.onDamageTaken += AddReflexResourceOnHit;
        //SceneManager.sceneLoaded += RefreshPostProcessing;
    }

    private void Update()
    {
        if (!controller.isDead && currentStamina != maxStamina)
            ReflexModeResourceHandler();

        if (returningToRealTime)
            LerpToRealTime();
    }

    void RefreshPostProcessing(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("refreshed vignette");
        //volume = FindObjectOfType<PostProcessVolume>();
        volume.profile.TryGetSettings(out vignette);
        Debug.Log(volume);
    }

    public void ReflexModeToggle()
    {
        if (!manager.matrixMode && controller.isWalking && controller.isGrounded && currentStamina > drainOnToggle)
        {
            manager.MatrixMode();

            vignette.enabled.Override(true);
            blur.enabled.Override(true);
            vignette.intensity.Override(0.25f);

            currentStamina = currentStamina - drainOnToggle;
            rechargeTimer = 0f;

            //rb_player.velocity = new Vector3(rb_player.velocity.x, 0f, rb_player.velocity.z);
            //rb_player.AddForce(transform.up * controller.jumpForce, ForceMode.Impulse);

            //rb_player.AddForce(direction.normalized * controller.jumpForce, ForceMode.Impulse);

            playerSound.PlayOneShot(slowMoSounds[0]);
        }
        else if (!manager.matrixMode && controller.isWalking && !controller.isGrounded && currentStamina > drainOnToggle)
        {
            manager.MatrixMode();

            blur.enabled.Override(true);
            vignette.enabled.Override(true);
            vignette.intensity.Override(0.25f);

            currentStamina = currentStamina - drainOnToggle;
            rechargeTimer = 0f;

            playerSound.PlayOneShot(slowMoSounds[0]);
            manager.master.SetFloat("sfxPitch", slowMoScale);
            manager.master.SetFloat("musicLowPass", 1000f);
        }
        else if (!manager.matrixMode && !controller.isWalking && currentStamina > drainOnToggle)
        {
            manager.MatrixMode();

            blur.enabled.Override(true);
            vignette.enabled.Override(true);
            vignette.intensity.Override(0.25f);

            currentStamina = currentStamina - drainOnToggle;
            rechargeTimer = 0f;

            playerSound.PlayOneShot(slowMoSounds[0]);
            manager.master.SetFloat("sfxPitch", slowMoScale);
            manager.master.SetFloat("musicLowPass", 1000f);
        }
        else if (manager.matrixMode)
        {
            manager.MatrixMode();
    
            returningToRealTime = true;

            //playerSound.PlayOneShot(slowMoSounds[1]);
        }
    }

    public void LerpToRealTime()
    {
        returnTimer += Time.unscaledDeltaTime;

        returnLerp = returnTimer / returnTime;
        returnLerp = returnLerp * returnLerp;

        Time.timeScale = Mathf.Lerp(slowMoScale, 1f, returnLerp);
        manager.master.SetFloat("sfxPitch", Mathf.Lerp(slowMoScale, 1f, returnLerp));
        manager.master.SetFloat("musicLowPass", Mathf.Lerp(1000, 5000, returnLerp));

        vignette.intensity.Override(Mathf.Lerp(0.25f, 0f, returnLerp));

        if(returnLerp >= 1)
        {
            returnTimer = 0f;
            returningToRealTime = false;
            vignette.enabled.Override(false);
            blur.enabled.Override(false);

            Time.timeScale = 1f;
            manager.master.SetFloat("sfxPitch", 1f);
            manager.master.SetFloat("musicLowPass", 5000f);
        }
    }

    public void ReflexModeResourceHandler()
    {
        // handles logic for slow mo resource so it cant be spammed

        if (manager.matrixMode)
        {
            // stop stamina drain when paused
            if (manager.isPaused)
                currentStamina = currentStamina - drainOverTime * Time.deltaTime;
            else
                currentStamina = currentStamina - drainOverTime * Time.unscaledDeltaTime;

            if (currentStamina <= 0f)
            {
                ReflexModeToggle(); // pop this to put things into preslowmo
                rechargeTimer = 0f;
            }
        }
        else if (!manager.matrixMode && currentStamina < maxStamina)
        {
            rechargeTimer += Time.deltaTime;

            if (rechargeTimer >= rechargeCoolDown)
            {
                currentStamina += regenOverTime * Time.deltaTime;

                if (currentStamina >= maxStamina)
                {
                    currentStamina = maxStamina;
                    rechargeTimer = 0f;
                }
            }
        }

        hud.matrixBar.fillAmount = currentStamina / maxStamina;
    }

    public void AddReflexResourceOnHit()
    {
        currentStamina += regenOnHit;

        if (currentStamina >= maxStamina)
            currentStamina = maxStamina;
    }
}
