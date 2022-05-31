using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShotgun : EnemyWeaponBase
{
    [Header("Shotgun related variables")]
    public int shotgunShots;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Fire()
    {
        isShooting = true;

        // create the amount of projectiles as specified in shotgunShots variable
        for (int i = 0; i < shotgunShots; i++)
        {
            // create the bullet, shoot the bullet forwards
            GameObject bulletClone = Instantiate(bullet, bulletSpawn.position, SetRandomBulletAngle());
            BulletBase shotBullet = bulletClone.GetComponentInChildren<BulletBase>();
            shotBullet.source = transform;
            shotBullet.rigidBody.AddForce(bulletClone.transform.forward * shotBullet.bulletForce, ForceMode.Impulse);
        }

        // PlayOnesShot() lets the sound play in full and doesn't overlap
        // also plays a very short muzzle flash particle for about a frame
        // putting it here for shotguns, otherwise will have 8 sounds at the same time :D
        AudioSource singleShot = Instantiate(weaponSound, transform.position, transform.rotation);
        singleShot.PlayOneShot(weaponSounds[0]);
        singleShot.gameObject.AddComponent<CleanUp>().cleanUpTime = 2f;

        GameObject muzzle = Instantiate(muzzleEffects, bulletSpawn);

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
}
