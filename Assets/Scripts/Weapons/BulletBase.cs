using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : MonoBehaviour
{
    public bool debug;

    public LayerMask targetMask;
    public float raycastOverhead;

    public float bulletForce; 
    public float bulletDamage;

    public Rigidbody rigidBody;
    //public GameObject parent;
    private BoxCollider bulletCollider;

    //impact particles
    public ParticleSystem impactParticles;
    public List<ParticleSystem> l_impactParticles;
    [SerializeField] LayerMask environmentMask;
   
    // trail & visuals
    private TrailRenderer trail;
    private MeshRenderer render;
    [SerializeField] float trailDelay;
    [SerializeField] GameObject bullethole;

    public Transform source;

    private PlayerHUD hud;

    void Start()
    {
        bulletCollider = GetComponentInChildren<BoxCollider>();
        Physics.IgnoreLayerCollision(8, 9);
        //parent = transform.parent.gameObject;

        // bullet logic

        // in progress

        // first the code checks if the the enemy or the player shot the bullet
        // this is because depending on the shooter, the bullet is either a trigger or collider
        // enemies shoot triggers to not cause the player to go flying, because the player is not a kinematic rigidbody
        // player shoots normal colliders, because enemies are kinematic and wont fly around erratically when hit
        
        // NEW: raycasts to enhance enemy bullet accuracy
        if(source != null)
        {
            if (source.parent.gameObject.layer == 6)
            {
                bulletCollider.isTrigger = false;

                // bullets wont clip the player itself
                CustomCharacterController player = FindObjectOfType<CustomCharacterController>();
                CapsuleCollider playerCollision = player.gameObject.GetComponentInChildren<CapsuleCollider>();
                Physics.IgnoreCollision(GetComponentInChildren<BoxCollider>(), playerCollision, true);

                hud = player.GetComponentInChildren<PlayerHUD>();

                Invoke("EnableVisuals", trailDelay);
            }
            else if (source.parent.gameObject.layer == 7)
            {
                bulletCollider.isTrigger = true;
                gameObject.layer = 14;

                // create a new collider to collide with environment
                BoxCollider newCollider = gameObject.AddComponent<BoxCollider>();
                newCollider.size = bulletCollider.size / 2f;

                Collider[] enemyCollision = source.parent.gameObject.GetComponentsInChildren<Collider>();
                GameObject player = GameObject.FindWithTag("Player");
                CapsuleCollider playerCollision = player.gameObject.GetComponentInChildren<CapsuleCollider>();

                for(int i = 0; i < enemyCollision.Length; i++)
                {
                    // bullets wont clip with any enemy parts
                    Physics.IgnoreCollision(bulletCollider, enemyCollision[i], true);
                    Physics.IgnoreCollision(newCollider, enemyCollision[i], true);
                }
                // new collider wont hit player
                Physics.IgnoreCollision(newCollider, playerCollision, true);

                Invoke("EnableVisuals", 0f);
            }
        }

        trail = GetComponentInChildren<TrailRenderer>();
        render = GetComponentInChildren<MeshRenderer>();
        render.enabled = false;
    }

    private void Update()
    {
        // raycast to improve enemy hit detection on player
        // this is only ran if the bullet layer is EnemyBullet
        if(gameObject.layer == 14)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, raycastOverhead, targetMask))
            {
                Debug.Log("Hit player with raycast");

                HealthBase targetHealth = hit.transform.gameObject.GetComponent<HealthBase>();

                targetHealth.TakeDamage(bulletDamage, bulletForce, hit.point, source, false);

                Destroy(gameObject);
            }
        }

        //Debug.DrawRay(transform.position, transform.forward * raycastOverhead, Color.red);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector3 contactPoint = collision.GetContact(0).point;
            
            if(collision.gameObject.GetComponent<HealthBase>() != null)
            {
                HealthBase targetHealth = collision.gameObject.GetComponent<HealthBase>();

                if (collision.collider == targetHealth.critBox)
                {
                    //Debug.Log("Headshot!");
                    targetHealth.TakeDamage(bulletDamage * 2, bulletForce, contactPoint, source, true);

                    // play headshot sound
                    //if (hud != null)
                        //hud.uiSound.PlayOneShot(hud.sounds[5]);
                }
                else
                {
                    targetHealth.TakeDamage(bulletDamage, bulletForce, contactPoint, source, false);

                    // play normal hitmark sound
                    //if (hud != null)
                        //hud.uiSound.PlayOneShot(hud.sounds[5]);
                }
                    
            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 contactPoint = collision.GetContact(0).point;

            if (collision.gameObject.GetComponent<HealthBase>() != null)
            {
                HealthBase targetHealth = collision.gameObject.GetComponent<HealthBase>();

                if (collision.collider == targetHealth.critBox)
                {
                    //Debug.Log("Headshot!");
                    targetHealth.TakeDamage(bulletDamage * 2, bulletForce, contactPoint, source, true);

                    // play headshot sound
                    //if (hud != null)
                    //hud.uiSound.PlayOneShot(hud.sounds[5]);
                }
                else
                {
                    targetHealth.TakeDamage(bulletDamage, bulletForce, contactPoint, source, false);

                    // play normal hitmark sound
                    //if (hud != null)
                    //hud.uiSound.PlayOneShot(hud.sounds[5]);
                }

            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == 10)
        {
            Vector3 contactPoint = collision.GetContact(0).point;
            Vector3 normal = collision.GetContact(0).normal;

            if(!collision.gameObject.CompareTag("Footsteps/GLASS"))
            {
                GameObject bhole = Instantiate(bullethole, contactPoint, Quaternion.LookRotation(normal));
                bhole.transform.parent = collision.transform;
                //Debug.Log("bullethole created?");
                bhole.transform.position += bhole.transform.forward / 1000;
            }

            CreateImpactParticles(collision.gameObject.tag, contactPoint);

            if(debug)
            {
                CreateDebugSphere(contactPoint, collision.transform);
            }

            Destroy(gameObject);
        }
        else
            Destroy(gameObject);
    }

    // this is used to make the player not take knockback from bullets
    private void OnTriggerEnter(Collider other)
    {  
        if (other.gameObject.transform.parent != null)
        {
            if (other.gameObject.transform.parent.CompareTag("Player"))
            {
                Debug.Log("Hit player with trigger");

                Vector3 contactPoint = other.ClosestPoint(transform.position);
                HealthBase targetHealth = other.gameObject.GetComponentInParent<HealthBase>();
                targetHealth.TakeDamage(bulletDamage, bulletForce, contactPoint, source, false);

                Destroy(gameObject);
            }
        }
    }

    void EnableVisuals()
    {
        render.enabled = true;
        trail.enabled = true;
    }

    void CreateImpactParticles(string tag, Vector3 contactPoint)
    {
        ParticleSystem impact;
        switch(tag)
        {
            case "Footsteps/CONCRETE":
                impact = Instantiate(l_impactParticles[0], contactPoint, Quaternion.identity);
                impact.transform.LookAt(source);
                break;
            case "Footsteps/WOOD":
                impact = Instantiate(l_impactParticles[1], contactPoint, Quaternion.identity);
                impact.transform.LookAt(source);
                break;
            case "Footsteps/METAL":
                impact = Instantiate(l_impactParticles[2], contactPoint, Quaternion.identity);
                impact.transform.LookAt(source);
                break;
            default:
                impact = Instantiate(l_impactParticles[0], contactPoint, Quaternion.identity);
                impact.transform.LookAt(source);
                break;
        }
    }

    private void CreateDebugSphere(Vector3 contactPoint, Transform newParent)
    {
        GameObject debugSphere = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere));
        debugSphere.transform.position = contactPoint;
        debugSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        debugSphere.transform.SetParent(newParent);
        debugSphere.GetComponent<Collider>().enabled = false;
        debugSphere.GetComponent<MeshRenderer>().material.color = Color.red;
        debugSphere.tag = "Temp";
    }

}
