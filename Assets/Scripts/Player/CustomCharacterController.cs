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
    [SerializeField] float groundCheckRadius;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    [SerializeField] private bool exitingSlope;

    public Transform orientation;

    float horizontal;
    float vertical;
    Vector3 direction;

    // jump jump jump everybody jump
    [Header("Jumping related variables")]
    public float jumpHeight;
    private float jumpForce;
    public float jumpCooldown;
    [SerializeField] private bool jumpRequest;
    public float matrixForce;
    public float airMultiplier;
    public float fallMultiplier;
    private bool readyToJump;
    private bool playedLandingNoise;
    public LayerMask ground;
    [SerializeField] AudioClip[] jumpSounds;

    // crouching related variables
    [Header("Crouching variables")]
    public float startHeight;
    public float crouchHeight;
    private float currentHeight;
    public float crouchTime;
    private float crouchLerpTime;

    // experimental: trying to keep the players head not going downwards when crouching midair
    // public float crouchDifference;

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

    [Header("Functionality stuff")]
    [SerializeField] ReflexMode reflex;
    public AudioSource playerSound;
    [SerializeField] PhysicMaterial noFriction;
    [SerializeField] CameraBreathe breathe;
    [SerializeField] Footsteps steps;

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

        jumpForce = Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y);
        readyToJump = true;

        reflex = GetComponent<ReflexMode>();
        breathe = GetComponentInChildren<CameraBreathe>();
        manager = FindObjectOfType<GameManager>();

        stepRayUpper.transform.position = new Vector3(stepRayUpper.transform.position.x, stepRayLower.transform.position.y + stepHeight, stepRayUpper.transform.position.z);

        toggleCrouch = manager.toggleCrouch;
        toggleAim = manager.toggleAim;

        playedLandingNoise = true;
    }

    void Update()
    {
        if(!playedLandingNoise && isGrounded)
        {
            steps.PlayLandingNoise();
            playedLandingNoise = true;
        }

        RaycastHit hit;

        isGrounded = Physics.SphereCast(transform.position, groundCheckRadius, Vector3.down, out hit, startHeight * 0.5f, ground);
        


        if (!isDead)
        {
            PlayerInput();
            SpeedControl();

            if(toggleCrouch)
            {
                Crouch();
                pCam.CameraCrouch(isCrouching, isGrounded, readyToJump, crouchLerpTime);
            }
        }

        if (isGrounded)
        {
            rb_player.drag = groundDrag;
        }
        else
        {
            rb_player.drag = 0f;
            playedLandingNoise = false;
        }
    }

    private void FixedUpdate()
    {
        Movement();

        if (jumpRequest)
            Jump();
    }

    private void PlayerInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump") && readyToJump && isGrounded)
        {
            readyToJump = false;

            jumpRequest = true;

            Invoke("ResetJump", jumpCooldown);
        }

        if (!manager.matrixMode && Input.GetButtonDown("L_Shift"))
            reflex.ReflexModeToggle();
        else if (manager.matrixMode && Input.GetButtonDown("L_Shift"))
            reflex.ReflexModeToggle();

        if (toggleCrouch)
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
                if (!isCrouching)
                    crouchLerpTime = 0f;

                isCrouching = true;

                Crouch();
                pCam.CameraCrouch(isCrouching, isGrounded, readyToJump, crouchLerpTime);
            }
            else if (CanUncrouch())
            {
                if(isCrouching)
                    crouchLerpTime = 0f;

                isCrouching = false;

                Crouch();
                pCam.CameraCrouch(isCrouching, isGrounded, readyToJump, crouchLerpTime);
            }
        }


    }

    private void SpeedControl()
    {

        if (OnSlope())
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

            if (!isCrouching)
            {
                if (flatVelocity.magnitude > walkSpeed)
                {
                    Vector3 limitedVelocity = flatVelocity.normalized * walkSpeed;
                    rb_player.velocity = new Vector3(limitedVelocity.x, rb_player.velocity.y, limitedVelocity.z);
                }
            }
            else if (isCrouching && isGrounded)
            {
                if (flatVelocity.magnitude > crouchSpeed)
                {
                    Vector3 limitedVelocity = flatVelocity.normalized * crouchSpeed;
                    rb_player.velocity = new Vector3(limitedVelocity.x, rb_player.velocity.y, limitedVelocity.z);
                }
            }
            else if (isCrouching)
            {
                if (flatVelocity.magnitude > walkSpeed)
                {
                    Vector3 limitedVelocity = flatVelocity.normalized * walkSpeed;
                    rb_player.velocity = new Vector3(limitedVelocity.x, rb_player.velocity.y, limitedVelocity.z);
                }
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

        // if there is input, walking is true / also set physicsmaterial to nofriction when moving and remove physics material when standing still
        if (direction.magnitude > 0f)
        {
            isWalking = true;

            if (playerCollider.sharedMaterial != noFriction)
                playerCollider.sharedMaterial = noFriction;
        }
        else
        {
            isWalking = false;

            if (playerCollider.sharedMaterial != null)
                playerCollider.sharedMaterial = null;
        }

        if (isGrounded)
        {
            rb_player.AddForce(direction.normalized * currentSpeed * 10f, ForceMode.Force);
        }
        else if (!isGrounded)
            rb_player.AddForce(direction.normalized * currentSpeed * 10f * airMultiplier, ForceMode.Force);

        // turn gravity off when the player is on slope
        rb_player.useGravity = !OnSlope();

        // only do step climb when not on a slope
        if (!OnSlope())
            StepClimb(direction);

        // jump fallmultiplier applied when falling

        if (rb_player.velocity.y < 0 && !manager.matrixMode)
        {
            rb_player.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

    }

    private void Jump()
    {
        exitingSlope = true;
        jumpRequest = false;

        rb_player.velocity = new Vector3(rb_player.velocity.x, 0, rb_player.velocity.z);

        rb_player.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        playerSound.PlayOneShot(jumpSounds[0]);
    }

    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private void Crouch()
    {
        currentHeight = playerCollider.height;

        if (isCrouching)
        {
            float lerpPercent = 0f;

            if (crouchHeight < currentHeight)
            {
                crouchLerpTime += Time.deltaTime;
                if (crouchLerpTime > crouchTime)
                    crouchLerpTime = crouchTime;

                lerpPercent = crouchLerpTime / crouchTime;
                playerCollider.height = Mathf.Lerp(currentHeight, crouchHeight, lerpPercent);
                breathe.LerpPosition(lerpPercent, isCrouching);
            }

        }
        else if (!isCrouching)
        {
            float lerpPercent = 0f;

            if (startHeight > currentHeight)
            {
                crouchLerpTime += Time.deltaTime;
                if (crouchLerpTime > crouchTime)
                    crouchLerpTime = crouchTime;

                lerpPercent = crouchLerpTime / crouchTime;
                playerCollider.height = Mathf.Lerp(currentHeight, startHeight, lerpPercent);
                breathe.LerpPosition(lerpPercent, isCrouching);
            }
        }

    }

    private bool CanUncrouch()
    {
        RaycastHit hit;

        Debug.DrawRay(transform.position, Vector3.up * 0.75f);

        if (Physics.Raycast(transform.position, Vector3.up, out hit, 1.5f))
            return false;
        else
            return true;


    }

    private bool OnSlope()
    {
        // checking if the player is on a slope
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
