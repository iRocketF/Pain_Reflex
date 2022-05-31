using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCharacterController : MonoBehaviour
{
    public PlayerCamera pCam;
    public Rigidbody rb_player;
    public CapsuleCollider playerCollider;

    // basic movement modifiers and variables
    [Header("Basic movement variables")]
    public float walkSpeed;
    public float crouchSpeed;
    private float currentSpeed;
    public float groundDrag;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public Transform orientation;

    float horizontal;
    float vertical;
    Vector3 direction;

    // jump jump jump everybody jump
    [Header("Jumping related variables")]
    public float jumpForce;
    public float matrixForce;
    public float airMultiplier;
    public float fallMultiplier;
    private bool readyToJump;
    public LayerMask ground;
    [SerializeField] AudioClip[] jumpSounds;

    // crouching related variables
    [Header("Crouching variables")]
    public float startHeight;
    public float crouchHeight;
    private float currentHeight;
    public float crouchTime;
    private float crouchLerpTime;

    [Header("Player step climbing")]
    [SerializeField] GameObject stepRayUpper;
    [SerializeField] GameObject stepRayLower;
    [SerializeField] float stepHeight;
    [SerializeField] float stepSmooth;

    // player status bools
    [Header("Player status bools")]
    public bool isWalking;
    public bool isCrouching;
    public bool isGrounded;
    //public bool reflexMode;
    public bool isDead;

    //player control bools
    public bool toggleCrouch;
    public bool toggleAim;

    [Header("Max Payne Jump Variables")]
    public float slowMoScale;
    public float maxMatrixStamina;
    public float currentStamina;
    public float matrixDrainChunk;
    public float matrixDrainOverTime;
    private float matrixRegainOverTime;
    public float matrixRechargeCooldown;
    private float matrixRechargeTimer;
    public AudioClip[] slowMoSounds;

    [Header("Functionality stuff")]
    [SerializeField] AudioSource playerSound;

    // the big boss
    private GameManager manager;

    void Start()
    {
        pCam = GetComponentInChildren<PlayerCamera>();

        rb_player = GetComponent<Rigidbody>();
        rb_player.freezeRotation = true;

        playerCollider = GetComponentInChildren<CapsuleCollider>();
        startHeight = playerCollider.height;
        crouchHeight = startHeight / 2f;

        currentStamina = maxMatrixStamina;
        matrixRegainOverTime = matrixDrainOverTime / 2f;

        manager = FindObjectOfType<GameManager>();

        stepRayUpper.transform.position = new Vector3(stepRayUpper.transform.position.x, stepRayLower.transform.position.y + stepHeight, stepRayUpper.transform.position.z);

        toggleCrouch = manager.toggleCrouch;
        toggleAim = manager.toggleAim;
    }

    void Update()
    {
        //isGrounded = Physics.Raycast(transform.position, Vector3.down, startHeight * 0.5f + 0.2f, ground);

        RaycastHit hit;

        isGrounded = Physics.SphereCast(transform.position, 0.4f, Vector3.down, out hit, startHeight * 0.5f, ground);
            
        if (!isDead)
        {
            PlayerInput();
            SpeedControl();

            if(toggleCrouch)
                Crouch();

            PayneJumpResource();
        }

        if (isGrounded)
            rb_player.drag = groundDrag;
        else
            rb_player.drag = 0f;
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void PlayerInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump") && isGrounded)
            Jump();

        if (!manager.matrixMode && Input.GetButtonDown("L_Shift") && currentStamina > matrixDrainChunk)
            PayneJump();
        else if (manager.matrixMode && Input.GetButtonDown("L_Shift"))
            PayneJump();

        if(toggleCrouch)
        {
            if (Input.GetButtonDown("Ctrl"))
            {
                if (isCrouching && CanUncrouch())
                {
                    isCrouching = false;
                    crouchLerpTime = 0f;
                }
                else
                {
                    isCrouching = true;
                    crouchLerpTime = 0f;
                }
            }
        }
        else
        {
            if (Input.GetButton("Ctrl"))
            {
                isCrouching = true;
                //crouchLerpTime = 0f;

                Crouch();
            }
            else if (CanUncrouch())
            {
                isCrouching = false;
                //crouchLerpTime = 0f;

                Crouch();
            }
        }


    }

    private void SpeedControl()
    {

        if(OnSlope())
        {
            if (!isCrouching)
            {
                if (rb_player.velocity.magnitude > walkSpeed)
                    rb_player.velocity = rb_player.velocity.normalized * walkSpeed;
            }
            else
            {
                if (rb_player.velocity.magnitude > crouchSpeed)
                    rb_player.velocity = rb_player.velocity.normalized * crouchSpeed;
            }

            
        }
        // this limits the player velocity so it doesn't exceed the speed in player variables
        else 
        {
            Vector3 flatVelocity = new Vector3(rb_player.velocity.x, 0f, rb_player.velocity.z);

            if (flatVelocity.magnitude > walkSpeed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * walkSpeed;
                rb_player.velocity = new Vector3(limitedVelocity.x, rb_player.velocity.y, limitedVelocity.z);
            }
        }

    }

    private void Movement()
    {
        // set player speed
        if (!isCrouching && isGrounded)
            currentSpeed = walkSpeed;
        if (isCrouching && isGrounded)
            currentSpeed = crouchSpeed;

        direction = orientation.forward * vertical + orientation.right * horizontal;

        // limit player velocity when on slopes
        if(OnSlope() && !exitingSlope)
        {
            rb_player.AddForce(GetSlopeMoveDirection() * walkSpeed * 20f, ForceMode.Force);

            if (rb_player.velocity.y > 0)
                rb_player.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        if (direction.magnitude > 0f)
            isWalking = true;
        /*else if (direction.magnitude == 0f && isGrounded)
        {
            isWalking = false;
            rb_player.velocity = Vector3.zero;
        }*/
        else
            isWalking = false;

        if (isGrounded)
        {
            rb_player.AddForce(direction.normalized * currentSpeed * 10f, ForceMode.Force);
            exitingSlope = false;
        }
        else if (!isGrounded)
            rb_player.AddForce(direction.normalized * currentSpeed * 10f * airMultiplier, ForceMode.Force);

        StepClimb(direction);

        // turn gravity off when the player is on slope
        rb_player.useGravity = !OnSlope();

        // jump fallmultiplier applied when falling

        if(rb_player.velocity.y < 0 && !manager.matrixMode)
        {
            rb_player.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

    }

    private void Jump()
    {
        exitingSlope = true;

        rb_player.velocity = new Vector3(rb_player.velocity.x, 0f, rb_player.velocity.z);

        rb_player.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        playerSound.PlayOneShot(jumpSounds[0]);
    }

    private void PayneJump()
    {
        if (!manager.matrixMode && isWalking && isGrounded)
        {
            manager.MatrixMode();

            currentStamina = currentStamina - matrixDrainChunk;
            matrixRechargeTimer = 0f;

            isCrouching = true;
            crouchLerpTime = 0f;

            rb_player.velocity = new Vector3(rb_player.velocity.x, 0f, rb_player.velocity.z);
            rb_player.AddForce(transform.up * jumpForce, ForceMode.Impulse);

            rb_player.AddForce(direction.normalized * jumpForce, ForceMode.Impulse);

            playerSound.PlayOneShot(slowMoSounds[0]);
        }
        else if (!manager.matrixMode && isWalking && !isGrounded)
        {
            manager.MatrixMode();

            currentStamina = currentStamina - matrixDrainChunk;
            matrixRechargeTimer = 0f;

            playerSound.PlayOneShot(slowMoSounds[0]);
            manager.master.SetFloat("sfxPitch", slowMoScale);
            manager.master.SetFloat("musicLowPass", 1000f);
        }
        else if (!manager.matrixMode && !isWalking)
        {
            manager.MatrixMode();

            currentStamina = currentStamina - matrixDrainChunk;
            matrixRechargeTimer = 0f;

            playerSound.PlayOneShot(slowMoSounds[0]);
            manager.master.SetFloat("sfxPitch", slowMoScale);
            manager.master.SetFloat("musicLowPass", 1000f);
        }
        else if(manager.matrixMode)
        {
            manager.MatrixMode();

            isCrouching = false;
            Time.timeScale = 1f;
            // slowMoSound.PlayOneShot(slowMoSounds[1]);
            manager.master.SetFloat("sfxPitch", 1f);
            manager.master.SetFloat("musicLowPass", 5000f);
        }
    }

    private void PayneJumpResource()
    {
        // handles logic for slow mo resource so it cant be spammed

        if (manager.matrixMode)
        {
            // stop stamina drain when paused
            if(manager.isPaused)
                currentStamina = currentStamina - matrixDrainOverTime * Time.deltaTime;
            else
                currentStamina = currentStamina - matrixDrainOverTime * Time.unscaledDeltaTime;

            if (currentStamina <= 0f)
            {
                PayneJump(); // pop this to put things into preslowmo
                matrixRechargeTimer = 0f;
            }
        }
        else if(!manager.matrixMode && currentStamina < maxMatrixStamina)
        {
            matrixRechargeTimer += Time.deltaTime;

            if(matrixRechargeTimer >= matrixRechargeCooldown)
            {
                currentStamina += matrixDrainOverTime * Time.deltaTime;

                if (currentStamina >= maxMatrixStamina)
                {
                    currentStamina = maxMatrixStamina;
                    matrixRechargeTimer = 0f;
                }
            }
        }
    }

    private void Crouch()
    {
        currentHeight = playerCollider.height;
        pCam.currentHeight = pCam.transform.localPosition.y;

        if (isCrouching)
        {
            float lerpPercent = 0f;

            if (lerpPercent <= 1f && crouchHeight < currentHeight)
            {
                crouchLerpTime += Time.deltaTime;
                if (crouchLerpTime > crouchTime)
                    crouchLerpTime = crouchTime;

                lerpPercent = crouchLerpTime / crouchTime;
                pCam.transform.localPosition = new Vector3(0f, (Mathf.Lerp(pCam.currentHeight, pCam.crouchHeight, lerpPercent)), 0f);
                playerCollider.height = Mathf.Lerp(currentHeight, crouchHeight, lerpPercent);
            }
        }
        else if (!isCrouching)
        {
            float lerpPercent = 0f;

            if (lerpPercent <= 1f && startHeight > currentHeight)
            {
                crouchLerpTime += Time.deltaTime;
                if (crouchLerpTime > crouchTime)
                    crouchLerpTime = crouchTime;

                lerpPercent = crouchLerpTime / crouchTime;
                pCam.transform.localPosition = new Vector3(0f, (Mathf.Lerp(pCam.currentHeight, pCam.startHeight, lerpPercent)), 0f);
                playerCollider.height = Mathf.Lerp(currentHeight, startHeight, lerpPercent);
            }
        }

    }

    private bool CanUncrouch()
    {
        RaycastHit hit;

        Debug.DrawRay(transform.position, Vector3.up * 0.75f);

        if (Physics.Raycast(transform.position, Vector3.up, out hit, 1f))
            return false;
        else
            return true;


    }

    private bool OnSlope()
    {
        // making player movement on slopes the same as walking on 
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, startHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    private void StepClimb(Vector3 direction)
    {
        RaycastHit hitLower;

        Debug.DrawRay(stepRayLower.transform.position, direction, Color.green);

        if (Physics.Raycast(stepRayLower.transform.position, direction, out hitLower, 0.5f, ground))
        {
            //Debug.Log("hitLower");
            RaycastHit hitUpper;

            if(!Physics.Raycast(stepRayUpper.transform.position, direction, out hitUpper, 0.5f, ground))
            {
                rb_player.position -= new Vector3(0f, -stepSmooth, 0f);
                //Debug.Log("hitUpper");
            }
        }
    }

}
