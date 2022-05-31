using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponBase : MonoBehaviour
{

    // TODO create a weapon for the enemy AI to use
    // based on the player weapon base, but modified for AI
    // experiment on it's workings

    [Header("Weapon basic info")]
    public string weaponName;
    public string caliber;
    public string fireMode;

    [Header("Weapon stats")]
    public float fireRate;
    public float recoil;
    public float reloadTime;
    public float weaponSpread;
    private float nextTimeToFire;

    [Header("Weapon related bools")]
    public bool isShooting;
    public bool isReloading;

    [Header("AI behavior related variables")]
    public EnemyAI enemy;
    public float burstBulletAmount;
    public float currentBurstFired;
    public float waitBetweenBursts;
    public float burstTimer;

    [Header("Variables for randomizing attack pattern")]
    public float minBurst;
    public float maxBurst;
    public float minWaitBetweenBursts;
    public float maxWaitBetweenBursts;

    public GameObject bullet;
    public Transform bulletSpawn;

    public AudioSource weaponSound;
    public AudioClip[] weaponSounds;

    public Rigidbody rigidBody;
    public MeshCollider w_collider;

    public GameObject muzzleEffects;

    public EnemyAmmoBase ammo;

    private MeshRenderer weaponRender;
    public float timeBeforeDelete;
    private float deleteTimer;



    public virtual void Start()
    {
        enemy = GetComponentInParent<EnemyAI>();
        ammo = GetComponent<EnemyAmmoBase>();
        rigidBody = GetComponent<Rigidbody>();
        weaponRender = GetComponent<MeshRenderer>();
        w_collider = GetComponent<MeshCollider>();
        weaponSound = GetComponentInChildren<AudioSource>();
    }

    public virtual void Update()
    {
        if (currentBurstFired == burstBulletAmount)
        {
            burstTimer += Time.deltaTime;

            if(burstTimer >= waitBetweenBursts)
            {
                currentBurstFired = 0f;
                burstTimer = 0f;

                RandomizeAttack();
            }
        }

        if (transform.parent == null)
            CleanUp();

    }

    void RandomizeAttack()
    {
        // randomize the attack pattern a little bit, for bigger variety after the initial burst
        float newBurstWait = Mathf.Round(Random.Range(minWaitBetweenBursts, maxWaitBetweenBursts));
        float newBurstAmount = Mathf.Round(Random.Range(minBurst, maxBurst));

        burstBulletAmount = newBurstAmount;
        waitBetweenBursts = newBurstWait;
    }

    public void AttemptShoot()
    {
        if (currentBurstFired < burstBulletAmount && ammo.currentMag > 0f && !isReloading && !isShooting && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            currentBurstFired++;
            Fire();
        }

        if (ammo.currentMag <= 0f && !isReloading && !isShooting)
        {
            StartCoroutine("Reload");
        }
    }

    public virtual void Fire()
    {
        isShooting = true;

        // instantiate an audiosource to play when the weapon fires
        // this will fix the issue of sounds being cut off after the death and removal of the AI weapon
        AudioSource singleShot = Instantiate(weaponSound, transform.position, transform.rotation);
        singleShot.PlayOneShot(weaponSounds[0]);
        singleShot.gameObject.AddComponent<CleanUp>().cleanUpTime = 2f;

        // muzzleflash gameobject that has the muzzleflash and smoke afterwards
        GameObject muzzle = Instantiate(muzzleEffects, bulletSpawn);

        GameObject bulletClone = Instantiate(bullet, bulletSpawn.position, SetRandomBulletAngle());
        BulletBase shotBullet = bulletClone.GetComponentInChildren<BulletBase>();
        shotBullet.source = transform;
        shotBullet.rigidBody.AddForce(bulletClone.transform.forward * shotBullet.bulletForce, ForceMode.Impulse);

        ammo.ReduceAmmo();

        isShooting = false;
    }

    Quaternion SetRandomBulletAngle()
    {
        // reusing old code for a random spread for weapons
        // get an new rotation for the bullet in a halfcone in front of the weapon, bullet wont go below the barrel

        Quaternion bulletDirection = bulletSpawn.transform.rotation;

        Quaternion randomSpread = Quaternion.identity
                                 * Quaternion.AngleAxis(Random.Range(-weaponSpread, weaponSpread), Vector3.up)
                                 * Quaternion.AngleAxis(Random.Range(-weaponSpread, weaponSpread), Vector3.right)
                                 * Quaternion.AngleAxis(Random.Range(-weaponSpread, weaponSpread), Vector3.forward);
        bulletDirection = Quaternion.Normalize(randomSpread * bulletDirection);

        return bulletDirection;
    }

    IEnumerator Reload()
    {
        isReloading = true;

        enemy.voice.PlayReloadVoice();

        yield return new WaitForSeconds(reloadTime);

        ammo.Reload();

        isReloading = false;
    }

    void CleanUp()
    {
        Destroy(gameObject);
    }

}
