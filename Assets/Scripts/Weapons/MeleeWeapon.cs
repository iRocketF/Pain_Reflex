using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    public bool isPickedUp;
    public bool canPickUp;

    public string weaponName;

    [Header("Stats")]
    public float meleeDmg;
    public float meleeForce;
    public float meleeRange;
    public LayerMask meleeMask;
    public LayerMask propMask;

    public float swingSpeed;
    private float nextTimeToSwing;

    public float rePickUpTime;
    private float timer_rePickUpTime;

    public bool isSwinging;

    public Animator animator;
    public AudioSource weaponSound;
    public AudioClip[] weaponSounds;

    public Rigidbody rigidBody;
    public Collider weaponCollider;
    private Outline outline;

    public GameObject player;
    private Camera pCam;
    private PlayerInventory inventory;
    private CustomCharacterController movement;
    private PlayerHUD hud;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        hud = player.GetComponentInChildren<PlayerHUD>();
        pCam = player.GetComponentInChildren<Camera>();
        inventory = player.GetComponentInChildren<PlayerInventory>();
        movement = player.GetComponentInChildren<CustomCharacterController>();

        rigidBody = GetComponent<Rigidbody>();
        weaponCollider = GetComponent<BoxCollider>();
        weaponSound = GetComponent<AudioSource>();

        animator = GetComponent<Animator>();
        animator.enabled = false;

        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent == null)
            isPickedUp = false;
        else
        {
            isPickedUp = true;

            if (outline.enabled)
                outline.enabled = false;
        }

        if(!canPickUp)
        {
            timer_rePickUpTime += Time.deltaTime;

            if (timer_rePickUpTime >= rePickUpTime)
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), player.GetComponentInChildren<CapsuleCollider>(), false);
                canPickUp = true;
                timer_rePickUpTime = 0f;
            }
        }

        // two different swings depending on mouse input, have them do different things?
        if (isPickedUp && Input.GetButtonDown("Mouse1") && !isSwinging && Time.time >= nextTimeToSwing)
        {
            nextTimeToSwing = Time.time + 1f / swingSpeed;
            animator.SetTrigger("Swing");
            Melee();
        }
        else if (isPickedUp && Input.GetButtonDown("Mouse2") && !isSwinging && Time.time >= nextTimeToSwing)
        {
            nextTimeToSwing = Time.time + 1f / swingSpeed;
            animator.SetTrigger("Stab");
            Melee();
        }

        // walking animation
        if (isPickedUp)
        {
            if (movement.isWalking)
                animator.SetBool("isWalking", true);
            else
                animator.SetBool("isWalking", false);
        }

        // Drop the weapon the player is currently holding
        if (isPickedUp && Input.GetButton("Drop"))
        {
            Drop();
        }
    }

    void Melee()
    {
        weaponSound.PlayOneShot(weaponSounds[0]);
        //weaponSound.PlayOneShot();

        // using good old raycasts for a melee hit
        // melee is hitscan, if there is an enemy in front of the player, theyre getting smacked
        // much cleaner than the old trigger method!

        // this should also check if there are physics based gameobjects in front
        // so attacking them will apply force to them

        RaycastHit hit;

        // deal damage if enemy in front
        if (Physics.Raycast(pCam.transform.position, pCam.transform.TransformDirection(Vector3.forward), out hit, meleeRange, meleeMask))
        {
            if (hit.transform.gameObject.GetComponent<HealthBase>() != null)
                DealDamage(hit);
        }
        // knock back physics objects & smash glass
        else if (Physics.Raycast(pCam.transform.position, pCam.transform.TransformDirection(Vector3.forward), out hit, meleeRange, propMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.gameObject.GetComponent<Rigidbody>() != null)
                ApplyKnockback(hit);

            if (hit.transform.gameObject.GetComponent<BreakableGlassV2>() != null)
                hit.transform.gameObject.GetComponent<BreakableGlassV2>().Break();
        }
    }

    void DealDamage(RaycastHit hit)
    {
        HealthBase hitEnemy = hit.transform.gameObject.GetComponent<HealthBase>();
        weaponSound.PlayOneShot(weaponSounds[1]);

        if (hitEnemy.currentHealth > 0)
            hitEnemy.TakeDamage(meleeDmg, meleeForce, hit.point, transform, false);
        else
            hitEnemy.GetComponent<Rigidbody>().AddForce(pCam.transform.forward * meleeForce, ForceMode.Impulse);
    }

    void ApplyKnockback(RaycastHit hit)
    {
        if (hit.collider.gameObject.GetComponentInParent<Rigidbody>() != null)
            hit.collider.gameObject.GetComponentInParent<Rigidbody>().AddForce(pCam.transform.forward * meleeForce, ForceMode.Impulse);
        else if (hit.collider.gameObject.GetComponent<Rigidbody>() != null)
            hit.collider.gameObject.GetComponent<Rigidbody>().AddForce(pCam.transform.forward * meleeForce, ForceMode.Impulse);
    }

    public void Drop()
    {
        // Strip the weapon from any player specific parts when dropped
        // make the weapon physics based againn

        transform.parent = null;
        inventory.weaponInventory[0] = null;

        gameObject.layer = 9;
        if (transform.childCount != 0)
        {
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.layer = 9;
        }

        animator.SetTrigger("Discard");
        animator.enabled = false;
        rigidBody.isKinematic = false;
        weaponCollider.enabled = true;

        // make the weapon ignore collision with the player model, add some force to throw the weapon away
        Physics.IgnoreCollision(GetComponent<Collider>(), player.GetComponentInChildren<CapsuleCollider>(), true);
        //rigidBody.AddForce(transform.forward * 100f + transform.up * 10f);

        canPickUp = false;
    }
}

