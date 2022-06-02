using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    public bool isPickedUp;
    public bool canPickUp;

    [Header("Weapon basic info")]
    public string weaponName;
    public string caliber;
    public string fireMode;

    [Header("Weapon stats")]
    public float fireRate;
    public float weaponSpread;
    public float spreadAimModifier;
    protected float originalSpread;
    public float reloadTime;
    private float nextTimeToFire;
    public float recoil;
    public float adsSpeed;

    public float rePickUpTime;
    private float timer_rePickUpTime;

    [Header("Quickmelee stats")]
    public float meleeDmg;
    public float meleeForce;
    public float meleeRange;
    public LayerMask meleeMask;
    public LayerMask propMask;

    [Header("Weapon scales for world and FPS view")]
    public Vector3 worldScale;
    public Vector3 povScale;
    private MeshFilter mesh;
    public Mesh[] models; // lods

    [Header("Weapon related bools")]
    public bool isShooting;
    //public bool isReloading;
    public bool isAiming;

    [Header("Components for functionality")]
    public WeaponRecoil weaponRecoil;
    public Animator animator;
    public AmmoBase ammo;
    public GameObject bullet;
    public Transform bulletSpawn;
    public GameObject muzzleEffects;
    public Rigidbody rigidBody;
    public Collider weaponCollider;
    public MeshRenderer arms;
    private Outline outline;

    [Header("Audio source and sounds")]
    public AudioSource weaponSound;
    public AudioClip[] firingSounds;
    public AudioClip[] weaponSounds;

    // Player related stuff
    [Header("Player related components")]
    public Vector3 aimSpot;
    public GameObject player;
    protected PlayerCamera pCam;
    private PlayerInventory inventory;
    private CustomCharacterController movement;
    private PlayerHUD hud;

    protected GameManager manager;


    public virtual void Start()
    {
        player = GameObject.FindWithTag("Player");
        hud = player.GetComponentInChildren<PlayerHUD>();
        pCam = player.GetComponentInChildren<PlayerCamera>();
        inventory = player.GetComponentInChildren<PlayerInventory>();
        movement = player.GetComponentInChildren<CustomCharacterController>();
        worldScale = transform.localScale;

        animator = GetComponent<Animator>();
        animator.enabled = false;

        outline = GetComponent<Outline>();
        outline.enabled = false;

        ammo = GetComponent<AmmoBase>();
        rigidBody = GetComponent<Rigidbody>();
        weaponCollider = GetComponent<Collider>();
        weaponSound = GetComponentInChildren<AudioSource>();
        weaponRecoil = GetComponent<WeaponRecoil>();

        manager = FindObjectOfType<GameManager>();

        if (arms != null)
            arms.enabled = false;

        originalSpread = weaponSpread;

        isShooting = false;
        ammo.isReloading = false;
        isAiming = false;

        mesh = GetComponent<MeshFilter>();
        SetMesh();
}

    public void SetAimSpot()
    {
        // this does not work if weapon has animations?
        Vector3 aimSpot = pCam.transform.position;
        aimSpot += pCam.transform.forward * 100f;
        transform.parent.LookAt(aimSpot);
    }

    public void SetMesh()
    {
        if(isPickedUp)
        {
            if (models.Length > 0f)
                mesh.mesh = models[0];
        }
        else
        {
            if (models.Length > 0f)
                mesh.mesh = models[1];
        }

    }

    public virtual void Update()
    {
        // check if the weapon is currently on the player
        if (transform.parent == null)
            isPickedUp = false;
        else
        {
            isPickedUp = true;

            if (outline.enabled)
                outline.enabled = false;
        }

        // a timer to stop the player from instantly picking the gun back up again after throwing it away
        if (!canPickUp)
        {
            outline.enabled = false;

            timer_rePickUpTime += Time.deltaTime;

            if (timer_rePickUpTime >= rePickUpTime)
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), player.GetComponentInChildren<CapsuleCollider>(), false);
                canPickUp = true;
                timer_rePickUpTime = 0f;
            }
        }

        // the main method for the weapon to shoot
        // TODO: tidy this up, it's pretty ugly and looks convoluted right now
        if (fireMode == "Automatic")
        {
            if (isPickedUp && Input.GetButton("Mouse1") && ammo.currentMag > 0f && !ammo.isReloading && !isShooting && !manager.isPaused && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Fire();
            }
        }
        else if (fireMode == "Single")
        {
            if (isPickedUp && Input.GetButtonDown("Mouse1") && ammo.currentMag > 0f && !ammo.isReloading && !isShooting && !manager.isPaused && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Fire();
            }
        }
        
        // quickmelee attack! 
        // add proper bools so it cant be spammed
        if(isPickedUp && !ammo.isReloading && !isShooting && !isAiming && Input.GetButtonDown("QuickMelee"))
            QuickMelee();
        
        // play an alternate empty chamber sound if the player has no ammo and mouse1 is pressed
        // TODO: clean this up like the upper one
        else if (isPickedUp && Input.GetButtonDown("Mouse1") && ammo.currentMag == 0f && !ammo.isReloading && !isShooting && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            weaponSound.PlayOneShot(weaponSounds[1]);
        }

        // start a coroutine reload
        // EXPERIMENT: look into other possibilies on how to do a reload, maybe using timers?
        // EXPERIMENT: implement reload in the AmmoBase script?
        if (isPickedUp && Input.GetButtonDown("Reload") && ammo.currentMag < ammo.maxMag && inventory.currentAmmo[ammo.ammoInt] > 0f && !isShooting && !ammo.isReloading)
        {
            WeaponReload();
            animator.SetTrigger("reload");
            ammo.isReloading = true;
        }

        // Drop the weapon the player is currently holding
        if (isPickedUp && Input.GetButtonDown("Drop"))
            Drop();

        // set a bool for the ADS method to work
        if(movement.toggleAim)
        {
            if (isPickedUp && Input.GetButtonDown("Mouse2"))
                Aim();
        }
        else
        {
            if(isPickedUp && Input.GetButton("Mouse2") || isAiming)
                Aim();
        }



        // walking animation
        if (isPickedUp)
        {
            if (movement.isWalking)
                animator.SetBool("isWalking", true);
            else
                animator.SetBool("isWalking", false);

            MatrixAimModifier();
        }

        // set 0 weapon spread when in slow motion

    }

    public void Aim()
    {
        // methdod for toggle aim
        if(movement.toggleAim)
        {
            if (!isAiming)
            {
                animator.SetBool("isAiming", true);

                if (!manager.matrixMode)
                    weaponSpread = weaponSpread / spreadAimModifier;

                isAiming = true;
            }
            else if (isAiming)
            {
                animator.SetBool("isAiming", false);

                if (!manager.matrixMode)
                    weaponSpread = originalSpread;

                isAiming = false;
            }
        }
        // method for hold aim
        else
        {
            if(Input.GetButton("Mouse2"))
            {
                animator.SetBool("isAiming", true);

                if (!manager.matrixMode)
                    weaponSpread = weaponSpread / spreadAimModifier;

                isAiming = true;
            }
            else
            {
                animator.SetBool("isAiming", false);

                if (!manager.matrixMode)
                    weaponSpread = originalSpread;

                isAiming = false;
            }

        }
       
    }

    public virtual void MatrixAimModifier()
    {
        if (isPickedUp && manager.matrixMode)
            weaponSpread = 0f;
        else if (isPickedUp && !manager.matrixMode)
        {
            if (!isAiming)
                weaponSpread = originalSpread;
            else
                weaponSpread = originalSpread / spreadAimModifier;
        }
    }

    public virtual void Fire()
    {
        isShooting = true;

        // FOR FUTURE ANIMATIONS

        if (!isAiming)
             animator.SetTrigger("shoot");
        if (isAiming)
            animator.SetTrigger("shoot_ads");

        // PlayOnesShot() lets the sound play in full and doesn't overlap
        // choose an firing sound from an array

        int i = Random.Range(0, firingSounds.Length);
        weaponSound.PlayOneShot(firingSounds[i]);

        // muzzleflash gameobject that has the muzzleflash and smoke afterwards
        // set the layers to equippedweapon so the smoke aligns properly with barrel
        GameObject muzzle = Instantiate(muzzleEffects, bulletSpawn);

        muzzle.layer = 12;

        for (int j = 0; j < muzzle.transform.childCount; j++)
            muzzle.transform.GetChild(j).gameObject.layer = 12;

        // create the bullet, shoot the bullet forwards
        GameObject bulletClone = Instantiate(bullet, pCam.transform.position, SetRandomBulletAngle());
        BulletBase shotBullet = bulletClone.GetComponentInChildren<BulletBase>();
        shotBullet.source = transform;
        shotBullet.rigidBody.AddForce(bulletClone.transform.forward * shotBullet.bulletForce, ForceMode.Impulse);

        weaponRecoil.AddRecoil();
        ammo.ReduceAmmo();

        isShooting = false;
    }

    Quaternion SetRandomBulletAngle()
    {
        // reusing old code for a random spread for weapons
        // get an new rotation for the bullet in a halfcone in front of the weapon, bullet wont go below the barrel

        Quaternion bulletDirection = pCam.transform.rotation;

        Quaternion randomSpread = Quaternion.identity
                                 * Quaternion.AngleAxis(Random.Range(-weaponSpread, weaponSpread), Vector3.up)
                                 * Quaternion.AngleAxis(Random.Range(-weaponSpread, weaponSpread), Vector3.right)
                                 * Quaternion.AngleAxis(Random.Range(0f, weaponSpread), Vector3.forward);
        if(!isAiming)
            bulletDirection = Quaternion.Normalize(randomSpread * bulletDirection);
        if(isAiming)
            bulletDirection = randomSpread * bulletDirection;

        return bulletDirection;
    }

    void QuickMelee()
    {
        weaponSound.PlayOneShot(weaponSounds[3]);
        animator.SetTrigger("melee");

        // using good old raycasts for a melee hit
        // melee is hitscan, if there is an enemy in front of the player, theyre getting smacked
        // much cleaner than the old trigger method!

        // this should also check if there are physics based gameobjects in front
        // so attacking them will apply force to them

        RaycastHit hit;

        if (Physics.Raycast(pCam.transform.position, pCam.transform.TransformDirection(Vector3.forward), out hit, meleeRange, meleeMask))
        {
            if (hit.transform.gameObject.GetComponent<HealthBase>() != null)
                DealMeleeDamage(hit);
        }
        else if (Physics.Raycast(pCam.transform.position, pCam.transform.TransformDirection(Vector3.forward), out hit, meleeRange, propMask))
        {
            if (hit.transform.gameObject.GetComponent<Rigidbody>() != null)
                ApplyKnockback(hit);

            if (hit.transform.gameObject.GetComponent<BreakableGlassV2>() != null)
                hit.transform.gameObject.GetComponent<BreakableGlassV2>().Break();

        }
    }

    public virtual void WeaponReload()
    {
        StartCoroutine("Reload");
    }

    // do this in AmmoBase instead?
    IEnumerator Reload()
    {
        weaponSound.PlayOneShot(weaponSounds[2]);
        ammo.isReloading = true;
        isAiming = false;

        yield return new WaitForSeconds(reloadTime);

        ammo.Reload();
    }

    void DealMeleeDamage(RaycastHit hit)
    {
        HealthBase hitEnemy = hit.transform.gameObject.GetComponent<HealthBase>();
        weaponSound.PlayOneShot(weaponSounds[4]);

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
        // make the weapon physics based again

        if(isAiming)
            isAiming = false;

        StopCoroutine(Reload());

        transform.parent = null;
        inventory.weaponInventory[0] = null;

        gameObject.layer = 9;
        if(transform.childCount != 0)
        {
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.layer = 9;
        }
            
        animator.SetTrigger("Discard");
        animator.enabled = false;
        rigidBody.isKinematic = false;
        weaponCollider.enabled = true;

        if(arms != null)
            arms.enabled = false;

        transform.localScale = worldScale;

        // make the weapon ignore collision with the player model, add some force to throw the weapon away
        Physics.IgnoreCollision(GetComponent<Collider>(), player.GetComponentInChildren<CapsuleCollider>(), true);
        rigidBody.AddForce(transform.forward * 100f + transform.up * 50f);

        isPickedUp = false;
                canPickUp = false;
        SetMesh();


    }

}
