using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform orientation;
    private Camera pCamera;
    private CustomCharacterController player;
    private PlayerInventory inventory;
    private WeaponBase weapon;
    private GameManager manager;

    [Header("Camera movement related variables")]
    public float mouseSens;
    public float normalSens;
    [Range(0,1)]
    public float aimSensModifier;
    public float aimSens;
    [HideInInspector]
    public float yRotation;
    [HideInInspector]
    public float xRotation;

    public float maxClamp;
    public float minClamp;

    [Header("Camera variables related to movement")]
    public float startHeight;
    public float currentHeight;
    public float crouchHeight;

    [Header("Camera zoom related variables")]
    public float zoomSpeed;
    private float originalFOV;
    private float zoomFOV;
    private float zoomLerp;

    public bool isAiming;

    void Start()
    {
        pCamera = GetComponent<Camera>();
        inventory = FindObjectOfType<PlayerInventory>();
        player = FindObjectOfType<CustomCharacterController>();
        manager = FindObjectOfType<GameManager>();
        originalFOV = pCamera.fieldOfView;
        zoomFOV = pCamera.fieldOfView / 2f;

        normalSens = manager.sensitivity;
        aimSens = normalSens * aimSensModifier;

        mouseSens = normalSens;

        startHeight = transform.localPosition.y;
        crouchHeight = transform.localPosition.y / 2f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        CheckForWeapon();

        if (!player.isDead)
        {
            // use the ADS/Zoom method
            if (isAiming)
            {
                Zoom();
                mouseSens = aimSens;
            }
            else if (!isAiming)
            {
                Zoom();
                mouseSens = normalSens;
            }

            if (!manager.isPaused)
                CameraMovement();
        }
        else if (player.isDead)
        {
            Zoom();
        }

    }

    void CameraMovement()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSens;// * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens;// * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, minClamp, maxClamp);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void ChangeSensitivity(float newSens)
    {
        normalSens = newSens;
        aimSens = newSens * aimSensModifier;
    }

    public void ChangeSensModifier(float newModifier)
    {
        aimSensModifier = newModifier;
        aimSens = normalSens * aimSensModifier;
    }

    void CheckForWeapon()
    {
        if (inventory.weaponInventory[0] != null)
        {
            if (inventory.weaponInventory[0].GetComponent<WeaponBase>() != null)
            {
                weapon = inventory.weaponInventory[0].GetComponent<WeaponBase>();
                zoomSpeed = weapon.adsSpeed;
                isAiming = weapon.isAiming;
            }
        }
        else
            isAiming = false;
    }

    void Zoom()
    {
        if (isAiming && zoomLerp < 1)
        {
            zoomLerp += Time.deltaTime * zoomSpeed;

            pCamera.fieldOfView = Mathf.Lerp(originalFOV, zoomFOV, zoomLerp);

            if (zoomLerp > 1)
                zoomLerp = 1f;

        }
        else if (!isAiming && zoomLerp > 0)
        {
            zoomLerp -= Time.deltaTime * zoomSpeed;

            pCamera.fieldOfView = Mathf.Lerp(originalFOV, zoomFOV, zoomLerp);

            if (zoomLerp < 0)
                zoomLerp = 0f;

        }
    }
}
