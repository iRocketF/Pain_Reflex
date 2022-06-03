using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShotgun : WeaponBase
{
    [Header("Shotgun related variables")]
    public int shotgunShots;
    public float pumpDelay;
    public float ejectionForce;
    public GameObject shell;
    public Transform ejectPort;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();

        if(ammo.isReloading && (Input.GetButtonDown("Mouse1") && ammo.currentMag != ammo.maxMag))
        {
            ammo.isReloading = false;
            animator.ResetTrigger("reloadNext");
            animator.SetTrigger("reloadFinish");
        }
    }

    public override void MatrixAimModifier()
    {
        if (isPickedUp && manager.matrixMode)
            weaponSpread = originalSpread / spreadAimModifier;
        else if (isPickedUp && !manager.matrixMode)
        {
            if (!isAiming)
                weaponSpread = originalSpread;
            else
                weaponSpread = originalSpread / spreadAimModifier;
        }
    }

    public override void Fire()
    {
        isShooting = true;

        // create the amount of projectiles as specified in shotgunShots variable
        for(int i = 0; i < shotgunShots; i++)
        {
            // create the bullet, shoot the bullet forwards
            GameObject bulletClone = Instantiate(bullet, pCam.transform.position, SetRandomBulletAngle());
            BulletBase shotBullet = bulletClone.GetComponentInChildren<BulletBase>();
            shotBullet.source = transform;
            shotBullet.rigidBody.AddForce(bulletClone.transform.forward * shotBullet.bulletForce, ForceMode.Impulse);
        }

        // PlayOnesShot() lets the sound play in full and doesn't overlap
        // also plays a very short muzzle flash particle for about a frame
        // putting it here for shotguns, otherwise will have 8 sounds at the same time :D
        weaponSound.PlayOneShot(weaponSounds[0]);
        StartCoroutine(Pump());

        GameObject muzzle = Instantiate(muzzleEffects, bulletSpawn.position, bulletSpawn.rotation, bulletSpawn.parent.parent);

        muzzle.layer = 12;

        for (int j = 0; j < muzzle.transform.childCount; j++)
            muzzle.transform.GetChild(j).gameObject.layer = 12;

        weaponRecoil.AddRecoil();
        ammo.ReduceAmmo();

        if (!isAiming)
            animator.SetTrigger("shoot");
        if (isAiming)
            animator.SetTrigger("shoot_ads");

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
        bulletDirection = Quaternion.Normalize(randomSpread * bulletDirection);

        return bulletDirection;
    }

    public override void WeaponReload()
    {
        StartCoroutine("ShotgunReload");

        isAiming = false;
    }

    // separate reload method in the ammo script for shotgun
    // reusing old code here
    // logic goes that it loops the Coroutine if there is still ammo to load
    // want to make this cancellable
    IEnumerator ShotgunReload()
    {
        yield return new WaitForSeconds(reloadTime);

        ammo.ShotgunReload();

    }

    IEnumerator Pump()
    {
        yield return new WaitForSeconds(pumpDelay);
        weaponSound.PlayOneShot(weaponSounds[6]);

        yield return new WaitForSeconds(pumpDelay);
        GameObject newShell = Instantiate(shell, ejectPort.position, ejectPort.rotation);
        BoxCollider shellCollider = newShell.GetComponent<BoxCollider>();
        Rigidbody newShellrb = newShell.GetComponent<Rigidbody>();

        Physics.IgnoreCollision(shellCollider, player.GetComponentInChildren<CapsuleCollider>(), true);

        ejectionForce = Random.Range(1f, 1.5f);
        float torque = Random.Range(900f, 1100f);

        newShellrb.AddForce(newShell.transform.right * ejectionForce, ForceMode.Impulse);
        newShellrb.AddForce(newShell.transform.up * ejectionForce, ForceMode.Impulse);
        newShellrb.AddTorque(newShell.transform.forward * torque, ForceMode.Impulse);
    }
}
