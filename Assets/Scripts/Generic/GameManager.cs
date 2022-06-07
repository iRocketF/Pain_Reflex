using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool gameLaunched;

    [Header("Player initialization & tracking")]
    public GameObject playerToSpawn;
    public GameObject player;
    public Transform spawn;

    [HideInInspector]
    public Vector3 transitionPosition;
    [HideInInspector]
    public Quaternion transitionRotation;

    public float timer;

    [Header("Game Status")]
    public bool inMenus;
    public bool isPaused;
    public bool ongoingTransition;
    public bool vipKilled;
    public bool matrixMode;
    public float slowMoScale;

    // items & ammo that the player should receive after a death during a level
    public List<GameObject> itemsToReturn;
    public List<float> ammoToReturn;

    [Header("Player settings")]
    public float sensitivity;
    public float aimSensitivity;
    public bool toggleAim;
    public bool toggleCrouch;

    [Header("Audio settings")]
    public AudioMixer master;
    [HideInInspector]
    public AudioSource gameMusic;
    public AudioClip[] tracks;
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;

    public Scene currentScene;

    private ScoreManager scoreManager;
    private ParticleDecalPool decalPool;

    void Awake()
    {
        gameLaunched = PlayerPrefs.GetInt("gameLaunched") == 1;

        if (gameLaunched)
            LoadSettings();
        else
            PlayerPrefs.SetInt("gameLaunched", 1);


        decalPool = GetComponentInChildren<ParticleDecalPool>();
        gameMusic = GetComponent<AudioSource>();
        scoreManager = GetComponent<ScoreManager>();

        SceneManager.sceneLoaded += OnSceneLoaded;

        DontDestroyOnLoad(gameObject);

        GameObject[] managers = GameObject.FindGameObjectsWithTag("GameController");

        if (managers.Length > 1)
            Destroy(gameObject);
    }

    private void Start()
    {
        SetInitialVolume();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckGameStatus();

        if (!inMenus && !ongoingTransition)
            SetupPlayer();

        // swap the level music
        switch (scene.name)
        {
            case "level_1_a_v2":
                gameMusic.clip = tracks[0];
                gameMusic.Play();
                break;

            case "level_2_a_v2":
                gameMusic.clip = tracks[1];
                gameMusic.Play();
                break;

            case "level_3_a_v2":
                gameMusic.clip = tracks[2];
                gameMusic.Play();
                break;
        }
    }

    void Update()
    {

        CheckGameStatus();
        CheckTimeScale();

        if (player == null)
            player = GameObject.FindWithTag("Player");

        if(player != null)
            if (player.GetComponent<CustomCharacterController>().isDead)
                if (Input.GetButtonDown("Reload"))
                    ResetScene();

        if (player != null)
            if (!player.GetComponent<CustomCharacterController>().isDead && vipKilled && !inMenus)
                if (Input.GetButtonDown("Submit"))
                    Continue();

        if(currentScene != SceneManager.GetActiveScene())
            currentScene = SceneManager.GetActiveScene();

        if(!inMenus)
        {
            Timer();

            if (Input.GetButtonDown("Escape"))
                Pause();
        }       
    }

    void CheckGameStatus()
    {
        if (SceneManager.GetActiveScene().name == "title_screen"
            || SceneManager.GetActiveScene().name == "main_menu"
            || SceneManager.GetActiveScene().name == "settings"
            || SceneManager.GetActiveScene().name == "sceneselection")
            inMenus = true;
        else
            inMenus = false;

    }

    void CheckTimeScale()
    {
        if (Time.timeScale == 0f)
            isPaused = true;
        else
            isPaused = false;
    }

    void Timer()
    {
        timer += Time.deltaTime;
    }

    public void Pause()
    {
        if (!isPaused)
        {
            Time.timeScale = 0f;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }    
        else if (isPaused)
        {
            Time.timeScale = 1f;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);

        if (isPaused)
            Pause();

        if (inMenus)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    public void Continue()
    {
        Time.timeScale = 1f;

        ClearParticles();

        master.SetFloat("sfxPitch", 1f);
        master.SetFloat("musicLowPass", 5000);

        vipKilled = false;
        timer = 0f;

        SceneManager.LoadScene(currentScene.buildIndex + 1); 
    }

    public void MainMenu()
    {
        if(!inMenus)
        {
            gameMusic.clip = tracks[0];
            gameMusic.Play();
        }

        ClearParticles();
        itemsToReturn.Clear();
        ammoToReturn.Clear();

        SceneManager.LoadScene("main_menu");

        master.SetFloat("sfxPitch", 1f);
        master.SetFloat("musicLowPass", 5000);

        vipKilled = false;
        timer = 0f;

        if (isPaused)
            Pause();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ResetScene()
    {
        vipKilled = false;

        ClearParticles();

        // clone items that are getting destroyed
        if (itemsToReturn.Count > 0f)
        {
            for (int i = 0; i < itemsToReturn.Count; i++)
                itemsToReturn[i] = Instantiate(itemsToReturn[i], transform);
        }

        LoadScene(currentScene.name);

        scoreManager.score = 0f;
        timer = 0f;
    }

    void ClearParticles()
    {
        decalPool.decalParticleSystem.Clear();
    }

    public void Quit()
    {
        Application.Quit();
    }

    void SetupPlayer()
    {
        Debug.Log("Player setup");

        player = GameObject.FindWithTag("Player");
        spawn = FindObjectOfType<PlayerSpawnPoint>().transform;

        if (spawn == null)
            spawn = FindObjectOfType<PlayerSpawnPoint>().transform;

        if (player == null)
            player = Instantiate(playerToSpawn, spawn.position, spawn.rotation);

        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        inventory.Start();

        // give the player items they are missing in case of a respawn
        if (itemsToReturn.Count > 0f)
        {
            for (int i = 0; i < itemsToReturn.Count; i++)
            {

                //if (itemsToReturn[i].GetComponent<Keycard>() == true)
                    //inventory.AddKeyCard(itemsToReturn[i]);

                if (itemsToReturn[i].GetComponent<WeaponBase>() == true)
                {
                    WeaponBase weapon = itemsToReturn[i].GetComponent<WeaponBase>();
                    weapon.Start();
                    weapon.GetComponent<AmmoBase>().Start();
                    weapon.GetComponent<WeaponRecoil>().Start();
                    inventory.SetWeapon(itemsToReturn[i]);
                }
            }
        }

        // give the player the ammo they had at a checkpoint in case of respawn
        for (int i = 0; i < ammoToReturn.Count; i++)
            inventory.currentAmmo[i] = ammoToReturn[i];
    }

    public void VictorySlowMo()
    {
        Time.timeScale = slowMoScale;
        master.SetFloat("sfxPitch", slowMoScale);
    }

    public void MatrixMode()
    {
        if (!matrixMode)
        {
            matrixMode = true;

            Time.timeScale = slowMoScale;

            master.SetFloat("sfxPitch", slowMoScale);
            master.SetFloat("musicLowPass", 2000f);
        }
        else
        {
            matrixMode = false;

            Time.timeScale = 1f;

            master.SetFloat("sfxPitch", 1f);
            master.SetFloat("musicLowPass", 5000f);
        }
    }

    void SetInitialVolume()
    {
        master.SetFloat("masterVolume", Mathf.Log10(masterVolume) * 20);
        master.SetFloat("musicVolume", Mathf.Log10(musicVolume) * 20);
        master.SetFloat("sfxVolume", Mathf.Log10(sfxVolume) * 20);
    }

    public void SaveSettings()
    {
        // save sound settings
        PlayerPrefs.SetFloat("masterVolume", masterVolume);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);

        // save sensitivity
        PlayerPrefs.SetFloat("sensitivity", sensitivity);
        PlayerPrefs.SetFloat("aimSensitivity", aimSensitivity);
        //Debug.Log(PlayerPrefs.GetFloat("aimSensitivity"));

        // save toggles
        PlayerPrefs.SetInt("toggleAim", toggleAim ? 1 : 0);
        PlayerPrefs.SetInt("toggleCrouch", toggleCrouch ? 1 : 0);

        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        // load sound settings
        masterVolume = PlayerPrefs.GetFloat("masterVolume");
        musicVolume = PlayerPrefs.GetFloat("musicVolume");
        sfxVolume = PlayerPrefs.GetFloat("sfxVolume");

        // load sensitivity
        sensitivity = PlayerPrefs.GetFloat("sensitivity");
        aimSensitivity = PlayerPrefs.GetFloat("aimSensitivity");

        // load toggles
        toggleAim = PlayerPrefs.GetInt("toggleAim") == 1;
        toggleCrouch = PlayerPrefs.GetInt("toggleCrouch") == 1;
    }
}
