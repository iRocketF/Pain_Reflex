using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController pController;
    public PlayerCamera pCam;

    // basic movement modifiers and variables
    [Header("Basic movement variables")]
    public float walkSpeed;
    //public float runSpeed;
    public float crouchSpeed;
    private float currentSpeed;
    public float gravity;
    public float jumpHeight;
    private Vector3 velocity;

    // crouching related variables
    [Header("Crouching variables")]
    public float startHeight;
    public float currentHeight;
    public float crouchHeight;
    public float crouchTime;
    public float currentLerpTime;

    // stamina related variables
    /*[Header("Stamina variables")]
    public float maxStamina;
    public float currentStamina;
    public float staminaDrain;
    public float jumpStaminaDrain;
    public float staminaRegen;
    public float staminaRegenTime;
    private float t_staminaRegenTimer*/

    // bools regarding player's current status
    public bool isWalking;
    //public bool isRunning;
    public bool isCrouching;
    //public bool isLeaning;
    public bool matrixMode;
    public bool isDead;

    private float matrixLerp;

    void Start()
    {
        pController = GetComponent<CharacterController>();
        pCam = GetComponentInChildren<PlayerCamera>();

        //currentStamina = maxStamina;
        startHeight = pController.height;
        crouchHeight = pController.height / 2f;
    }

    void Update()
    {
        if (!isDead)
        {
            Movement();
            Crouch();
            //Stamina();
        }
    }

    void Movement()
    {
        if (pController.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = (transform.right * horizontal + transform.forward * vertical);

        if (Input.GetButtonDown("Jump") && CollisionFlags.Below != 0 && pController.isGrounded)
        {
            Jump();
        }

        if (!pController.isGrounded && (pController.collisionFlags & CollisionFlags.Above) != 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;

        // set isWalking bool only when there is horizontal/vertical movement
        if (horizontal != 0 || vertical != 0)
            isWalking = true;
        else
            isWalking = false;

        // change the movement speed according to sprint command
        /* if (Input.GetAxis("L_Shift") > 0f && horizontal + vertical != 0 && currentStamina > 0f && !isCrouching)
        {
            currentSpeed = runSpeed;
            isWalking = false;
            isRunning = true;
        }*/

        if (!isCrouching && pController.isGrounded)
        {
            currentSpeed = walkSpeed;
            //isRunning = false;
        }
        else if (isCrouching && pController.isGrounded)
            currentSpeed = crouchSpeed;

        if ((horizontal + vertical) >= 1f || (horizontal + vertical) <= -1f || (horizontal + vertical) == 0f)
            move.Normalize();

        pController.Move(((move * currentSpeed) * Time.deltaTime) + velocity * Time.deltaTime);
    }

    void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        //currentStamina -= jumpStaminaDrain;
        //t_staminaRegenTimer = 0f;
    }

    void PayneJump()
    {
        /* PROBLEM - you cant rotate the normal charactercontroller capsule, so for this feature to work
         * you really gotta make a custom one... brainstorm this one more
         * 
         * The Payne jump! Concept: Max Payne slowmo jump, but in first person! Implementation
         * 1st do a small hop, rotate player a maximum of 90 into every direction
         * 2
         */

        matrixLerp = matrixLerp + Time.deltaTime;
        transform.localRotation = Quaternion.Slerp(transform.rotation, new Quaternion(transform.rotation.w, transform.rotation.x, transform.rotation.y, 90), matrixLerp);
    }

    void Crouch()
    {
        if (Input.GetButtonDown("Ctrl"))
            if (isCrouching)
            {
                isCrouching = false;
                currentLerpTime = 0f;
            }
            else
            {
                isCrouching = true;
                currentLerpTime = 0f;
            }

        currentHeight = pController.height;
        pCam.currentHeight = pCam.transform.localPosition.y;
                
        if (isCrouching)
        {
            float lerpPercent = 0f;

            if(lerpPercent <= 1f && crouchHeight < currentHeight)
            {
                currentLerpTime += Time.deltaTime;
                if (currentLerpTime > crouchTime)
                    currentLerpTime = crouchTime;

                lerpPercent = currentLerpTime / crouchTime;
                pCam.transform.localPosition = new Vector3(0f, (Mathf.Lerp(pCam.currentHeight, pCam.crouchHeight, lerpPercent)), 0f);
                pController.height = Mathf.Lerp(currentHeight, crouchHeight, lerpPercent);
            }
        }
        else if(!isCrouching)
        {
            float lerpPercent = 0f;

            if(lerpPercent <= 1f && startHeight > currentHeight)
            {
                currentLerpTime += Time.deltaTime;
                if (currentLerpTime > crouchTime)
                    currentLerpTime = crouchTime;

                lerpPercent = currentLerpTime / crouchTime;
                pCam.transform.localPosition = new Vector3(0f, (Mathf.Lerp(pCam.currentHeight, pCam.startHeight, lerpPercent)), 0f);
                pController.height = Mathf.Lerp(currentHeight, startHeight, lerpPercent);
            }
        }
            
    }

    /*void Stamina()
    {
        if (isRunning && currentStamina > 0f)
        {
            currentStamina -= staminaDrain * Time.deltaTime;
            t_staminaRegenTimer = 0f;
        }

        if (!isRunning && currentStamina < maxStamina)
        {
            t_staminaRegenTimer += Time.deltaTime;

            if (t_staminaRegenTimer >= staminaRegenTime)
            {
                t_staminaRegenTimer = staminaRegenTime;
                currentStamina += staminaRegen * Time.deltaTime;
            }
        }
    }*/
}
