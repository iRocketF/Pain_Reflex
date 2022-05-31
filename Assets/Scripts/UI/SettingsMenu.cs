using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public GameManager manager;

    [Header("Mouse Sensitivity")]
    public Slider sensitivitySlider;
    public Slider sensModifierSlider;
    public TextMeshProUGUI sensitivityValue;
    public TextMeshProUGUI modifierValue;

    [Header("Sound sliders")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    // public Slider bloodSlider;

    [Header("Buttons & text")]
    public Button toggleAimOn;
    public TextMeshProUGUI aimOnText;
    public Button toggleAimOff;
    public TextMeshProUGUI aimOffText;
    public Button toggleCrouchOn;
    public TextMeshProUGUI crouchOnText;
    public Button toggleCrouchOff;
    public TextMeshProUGUI crouchOffText;

    void Start()
    {
        manager = FindObjectOfType<GameManager>();

        // set setting sliders to manager settings
        sensitivitySlider.value = manager.sensitivity;

        SetInitialVolume(manager.musicVolume, manager.sfxVolume, manager.masterVolume);

        Debug.Log(sfxVolumeSlider.value);

        if (!manager.toggleAim)
        {
            aimOffText.alpha = 1f;
            aimOnText.alpha = 0.5f;
        }

        if (!manager.toggleCrouch)
        {
            crouchOffText.alpha = 1f;
            crouchOnText.alpha = 0.5f;
        }
    }

    void SetInitialVolume(float musicVolume, float sfxVolume, float masterVolume)
    {
        masterVolumeSlider.value = masterVolume;
        musicVolumeSlider.value = musicVolume;
        sfxVolumeSlider.value = sfxVolume;
    }

    public void SetVolume()
    {
        manager.masterVolume = masterVolumeSlider.value;
        manager.master.SetFloat("masterVolume", Mathf.Log10(masterVolumeSlider.value) * 20);
        manager.musicVolume = musicVolumeSlider.value;
        manager.master.SetFloat("musicVolume", Mathf.Log10(musicVolumeSlider.value) * 20);
        manager.sfxVolume = sfxVolumeSlider.value;
        manager.master.SetFloat("sfxVolume", Mathf.Log10(sfxVolumeSlider.value) * 20);
    }

    public void SetSensitivity()
    {
        manager.sensitivity = sensitivitySlider.value;

        sensitivityValue.text = (Mathf.Round(sensitivitySlider.value * 100f) / 100f).ToString();
        modifierValue.text = (Mathf.Round(sensModifierSlider.value * 100f) / 100f).ToString();

        sensitivityValue.text = sensitivityValue.text.Replace(",", ".");
        modifierValue.text = modifierValue.text.Replace(",", ".");

        if (sensitivityValue.text.Length == 3)
            sensitivityValue.text = sensitivityValue.text + "0";

        if (modifierValue.text.Length == 3)
            modifierValue.text = modifierValue.text + "0";
    }

    public void SetToggleAim()
    {
        if (manager.toggleAim)
        {
            manager.toggleAim = false;
            aimOffText.alpha = 1f;
            aimOnText.alpha = 0.5f;
        }

        else
        {
            manager.toggleAim = true;
            aimOffText.alpha = 0.5f;
            aimOnText.alpha = 1f;

        }
    }

    public void SetToggleCrouch()
    {
        if (manager.toggleCrouch)
        {
            manager.toggleCrouch = false;
            crouchOffText.alpha = 1f;
            crouchOnText.alpha = 0.5f;
        }
        else
        {
            manager.toggleCrouch = true;
            crouchOffText.alpha = 0.5f;
            crouchOnText.alpha = 1f;

        }
    }

    public void Return()
    {
        manager.MainMenu();
    }
}
