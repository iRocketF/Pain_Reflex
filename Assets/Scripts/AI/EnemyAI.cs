using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent agent;
    [HideInInspector]
    public Transform player;
    public Transform playerAimSpot;
    public Transform eyes;
    public Transform body;

    // public LayerMask Ground, Player;
    [SerializeField]
    protected LayerMask playerMask;
    [SerializeField]
    private LayerMask obstacleMask;

    [HideInInspector]
    public EnemyWeaponBase weapon;
    [HideInInspector]
    public EnemyWeaponMelee meleeWeapon;

    [HideInInspector]
    public HealthBase health;

    // TODO: experiment with AI and possible separate scripts for patrol & combat

    [Header("AI patrol variables")]
    public Transform waypoint;
    private Vector3 walkPoint;
    public bool walkPointSet;
    public float walkPointRange;
    public float idleTime;
    public float idleTimer;
    public bool isStatic;
    public bool isPursuer;
    public bool randomPatrol;

    [Header("AI reaction speed")]
    public float reactionTime;
    private float reactionTimer;
    private bool hasAimed;

    [Header("AI aim variables")]
    // aim deviation variables
    [SerializeField][Tooltip("how much AI aim deviates from target")]
    private float aimDeviation; // how much AI aim deviates from target
    [SerializeField][Tooltip("how much aim can deviate at it's maximum")]
    private float maxAimDeviation; // how much aim can deviate at it's maximum
    [SerializeField][Tooltip("how much AI aim deviates from target")]
    private float minAimDeviation; // how much aim can deviate at it's minimum
    [SerializeField][Tooltip("how much AI aim deviates from target")]
    private float maxPlayerDistance; // how far the player can be so deviation is 1;
    private float playerDistance; // current player distance
    public float aimSpeed;

    // ai aim helpers
    private float targetX;
    private float targetY;
    private float targetZ;

    [Header("AI states & related variables")]
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    public float maxAttackRange;
    public float minAttackRange;
    public float awarenessRange;

    public bool playerInAttackRange;
    public bool playerInMinAttackRange;
    public bool playerInAwarenessRange;
    public bool hasLoS;
    public bool sawPlayer;
    public bool react;

    [Header("Enemy sounds")]
    public EnemyVoice voice;
    public EnemyFootsteps steps;

    [Header("Enemy bools")]
    public bool isDead;
    public bool waitingForDoor;
    public bool isWalking;

    [Header("Weapon drop")]
    public GameObject weaponToDrop;

    private Transform target;

    [SerializeField]
    private GameObject headshotEffect;


    public virtual void Start()
    {
        if (GetComponentInChildren<EnemyWeaponBase>() != null)
            weapon = GetComponentInChildren<EnemyWeaponBase>();
        if (GetComponentInChildren<EnemyWeaponMelee>() != null)
            meleeWeapon = GetComponentInChildren<EnemyWeaponMelee>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        for(int i = 0; i < player.childCount; i++)
        {
            if(player.GetChild(i).name == "EnemyAimSpot")
            {
                playerAimSpot = player.GetChild(i);
                break;
            }
        }

        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<HealthBase>();
        voice = GetComponent<EnemyVoice>();
        steps = GetComponent<EnemyFootsteps>();

        idleTimer = 0f;

        if (waypoint == null)
            randomPatrol = true;
    }

    public virtual void Update()
    {
        // Check for sight & attack in a radius around the enemy object
        // In addition, check for Line of Sight before attacking
        // enemy needs initial Line of Sight to do anything

        if (!isDead)
        {
            playerInAttackRange = Physics.CheckSphere(transform.position, maxAttackRange, playerMask);
            playerInMinAttackRange = Physics.CheckSphere(transform.position, minAttackRange, playerMask);
            playerInAwarenessRange = Physics.CheckSphere(transform.position, awarenessRange, playerMask);

            if(agent.isOnOffMeshLink)
            {
                idleTimer += Time.deltaTime;

                if (idleTimer >= idleTime)
                    agent.CompleteOffMeshLink();
            }

            FindPlayerInRange();
            AIState();

            if (hasLoS)
                SetAimDeviation();
        }

    }

    private void AIState()
    {
        // these need a logical system for the AI to seem "smart" and behave logically
        // add timers for state changes
        // idle patrol if not in combat
        // when in combat, close distance to attack, chase player behind corners and shoot
        // added reaction time for when the enemy sees player to not instantly shoot, but to add a slight delay
        // modify reaction time depending on AI state

        if (reactionTimer >= reactionTime && !react)
        {
            voice.PlaySpotVoice();
            react = true;
        }

        if (!isStatic)
        {
            if (playerInAwarenessRange && !hasLoS) Approach();
            if (!playerInAttackRange && !hasLoS && !sawPlayer) Patrol(); // player nowhere close, idle patrol
            if (playerInAttackRange && !hasLoS && !sawPlayer) Patrol(); // player in attack range, but no LoS yet
            if (playerInAttackRange && !hasLoS && sawPlayer) Chase(); // player close but no sight yet, idle patrol
            if (!playerInAttackRange && hasLoS && sawPlayer) Chase(); // player inside sight radius and there is LoS, close distance to attack
            if (playerInAttackRange && react && !hasLoS) Chase(); // player close, will chase player even with no LoS


            if (health.currentHealth < health.maxHealth) Chase(); // act upon taking damage, start chasing the player

            if (playerInAttackRange && !react && hasLoS) TurnTowards(player);
            if (playerInAttackRange && !playerInMinAttackRange && react && hasLoS) AttackMove(); // attack player when close enough
            if (playerInMinAttackRange && react && hasLoS) Attack(); // attack player when close enough
        }
        else if (isStatic)
        {
            if (playerInAwarenessRange && !hasLoS) TurnTowards(player);
            if (playerInAttackRange && health.currentHealth < health.maxHealth && !hasLoS) TurnTowards(player); // act upon taking damage, turn towards the player
            if (playerInAttackRange && !react && hasLoS) TurnTowards(player); // this function is mostly for snipers
            if (playerInAttackRange && react && hasLoS) Attack(); // attack player when close enough && reaction time is up
        }

        if (isStatic && isPursuer && hasLoS)
            isStatic = false;

        if (agent.velocity.x != 0 || agent.velocity.z != 0)
            isWalking = true;
        else
            isWalking = false;


    }

    private void Patrol()
    {
        //isPassive = true;
        //isAggressive = false;

        if (!walkPointSet && idleTimer >= idleTime)
            SearchWalkPoint();
        else if (!walkPointSet && idleTimer < idleTime)
            idleTimer = idleTimer + Time.deltaTime;
        else
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (walkPointSet && agent.velocity == Vector3.zero && idleTimer < idleTime)
            idleTimer = idleTimer + Time.deltaTime;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;

        else if (idleTimer >= idleTime)
            walkPointSet = false;
    }

    void FindPlayerInRange()
    {
        Collider[] playerInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, playerMask);

        for (int i = 0; i < playerInViewRadius.Length; i++)
        {
            Transform p_Target = playerInViewRadius[i].transform;

            Vector3 dirToTarget = (p_Target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, p_Target.position);

                if (!Physics.Raycast(eyes.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    if (reactionTimer < reactionTime)
                        reactionTimer += Time.deltaTime;

                    hasLoS = true;

                    if (!sawPlayer)
                        sawPlayer = true;
                }

                else if (Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                    hasLoS = false;
            }
        }

    }

    private void SearchWalkPoint()
    {
        if (randomPatrol)
        {
            // calculate a random point in range to patrol to
            float randomZ = Random.Range(-walkPointRange, walkPointRange);
            float randomX = Random.Range(-walkPointRange, walkPointRange);

            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        }
        else
        {
            Vector3 distanceToWalkPoint = transform.position - walkPoint;

            if (distanceToWalkPoint.magnitude < 1f)
            {
                if (waypoint.GetComponent<Waypoint>().isFinalWaypoint)
                {
                    isStatic = true;
                    isPursuer = true;
                }
                else
                    waypoint = waypoint.GetComponent<Waypoint>().nextWaypoint;
            }
            walkPoint = waypoint.position;
        }

        idleTimer = 0f;
        walkPointSet = true;
    }

    private void Approach()
    {
        agent.speed = 2f;

        walkPointSet = false;

        //isPassive = false;
        //isAggressive = false;
        //isAlert = true;

        agent.SetDestination(player.position);
    }

    private void Chase()
    {
        agent.speed = 5f;

        walkPointSet = false;

        //isPassive = false;
        //isAggressive = false;
        //isAlert = true;

        agent.SetDestination(player.position);
    }

    private void SetAimDeviation()
    {
        // linear interpolation of enemy aim deviation depending on distance
        // 0 distance = 0 deviation, maxPlayerDistance = 1 deviation;
        // pseudocode aimDeviation = Mathf.Lerp(0, 1, playerDistance / maxPlayerDistance)
        playerDistance = Vector3.Distance(transform.position, player.position);

        aimDeviation = Mathf.Lerp(0, 1, playerDistance / maxPlayerDistance);

        if (aimDeviation < minAimDeviation)
            aimDeviation = minAimDeviation;
        else if (aimDeviation > maxAimDeviation)
            aimDeviation = maxAimDeviation;
    }

    public virtual void Attack()
    {
        //isPassive = false;
        //isAggressive = true;

        agent.SetDestination(transform.position);

        // to make the enemy not an aimbot, I want to check a random spot in a sphere around the player for
        // the AI to aim at, it can be at the player or around the player to make the bot miss as well
        // experiment with values here!

        if (weapon.currentBurstFired == 0f && !hasAimed)
        {
            targetX = Random.Range(-aimDeviation, aimDeviation);
            targetY = Random.Range(-aimDeviation, aimDeviation);
            targetZ = Random.Range(-aimDeviation, aimDeviation);

            hasAimed = true;
        }

        Vector3 target = new Vector3(playerAimSpot.position.x + targetX, playerAimSpot.position.y + targetY, playerAimSpot.position.z + targetZ);

        TurnTowards(player);

        Quaternion w_ogRotation = weapon.transform.rotation;
        weapon.transform.LookAt(target);
        Quaternion w_newRotation = weapon.transform.rotation;
        weapon.transform.rotation = w_ogRotation;
        weapon.transform.rotation = Quaternion.Lerp(weapon.transform.rotation, w_newRotation, aimSpeed * Time.deltaTime);

        weapon.AttemptShoot();

        if (weapon.currentBurstFired >= weapon.burstBulletAmount)
            hasAimed = false;
    }

    public virtual void AttackMove()
    {
        agent.speed = 4f;

        agent.SetDestination(player.position);

        if (weapon.currentBurstFired == 0f && !hasAimed)
        {
            targetX = Random.Range(-aimDeviation, aimDeviation);
            targetY = Random.Range(-aimDeviation, aimDeviation);
            targetZ = Random.Range(-aimDeviation, aimDeviation);

            hasAimed = true;
        }

        Vector3 target = new Vector3(playerAimSpot.position.x + targetX, playerAimSpot.position.y + targetY, playerAimSpot.position.z + targetZ);

        TurnTowards(player);

        Quaternion w_ogRotation = weapon.transform.rotation;
        weapon.transform.LookAt(target);
        Quaternion w_newRotation = weapon.transform.rotation;
        weapon.transform.rotation = w_ogRotation;
        weapon.transform.rotation = Quaternion.Lerp(weapon.transform.rotation, w_newRotation, aimSpeed * Time.deltaTime);

        weapon.AttemptShoot();

        if (weapon.currentBurstFired >= weapon.burstBulletAmount)
            hasAimed = false;
    }

    void TurnTowards(Transform lookTarget)
    {
        Quaternion ogRotation = transform.rotation;
        transform.LookAt(lookTarget);
        Quaternion newRotation = transform.rotation;
        transform.rotation = ogRotation;
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, aimSpeed * Time.deltaTime);
    }

    void BodyTurnTowards(Transform lookTarget)
    {
        Quaternion ogRotation = body.transform.rotation;
        transform.LookAt(lookTarget);
        Quaternion newRotation = body.transform.rotation;
        transform.rotation = ogRotation;
        transform.rotation = Quaternion.Lerp(body.transform.rotation, newRotation, aimSpeed * Time.deltaTime);
    }

    // This is used for FieldOfView
    // Tutorial used: https://www.youtube.com/watch?v=rQG9aUWarwE&list=WL&index=1&t
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public virtual void Die(float deathForce, Transform source, bool headshot)
    {
        isDead = true;

        // disable pathfinding and enable physics
        Collider[] colliders = GetComponentsInChildren<Collider>();

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponent<Rigidbody>() == null)
            {
                Rigidbody rb = colliders[i].gameObject.AddComponent<Rigidbody>();
                rb.gameObject.AddComponent<CleanUpGib>();
                rb.mass = 20f;
                rb.AddForce(deathForce * (transform.position - source.position).normalized, ForceMode.Impulse);
                rb.gameObject.tag = "Untagged";

                if (headshot)
                {
                    if (rb.gameObject.name == "Head")
                    {
                        if(headshotEffect != null)
                        {
                            GameObject headBloodSpray = Instantiate(headshotEffect, rb.transform.position, Quaternion.identity);
                            Destroy(rb.gameObject);
                        }
                        else
                            rb.AddForce(deathForce * transform.up, ForceMode.Impulse);
                    }
                }

            }
        }

        Rigidbody rigidBody = GetComponent<Rigidbody>();
        rigidBody.isKinematic = false;

        agent.enabled = false;
        Destroy(agent);

        // instantiate a copy of the AI's weapon that the player can use
        // set the ammo amount to be the same as the enemy weapon

        if (weapon != null)
        {
            GameObject newWeapon = Instantiate(weaponToDrop, weapon.transform.position, weapon.transform.rotation);

            newWeapon.GetComponent<AmmoBase>().isDrop = true;
            newWeapon.GetComponent<AmmoBase>().currentMag = weapon.GetComponent<EnemyAmmoBase>().currentMag;

            weapon.transform.parent = null;
        }
        else if (meleeWeapon != null)
        {
            GameObject newWeapon = Instantiate(weaponToDrop, meleeWeapon.transform.position, meleeWeapon.transform.rotation);
            Destroy(meleeWeapon);
        }


        // "ragdoll" & ignore collision so player can move over body
        // get a reliable way to add force to the body related to the caliber of the gun shot with
        Physics.IgnoreLayerCollision(6, 7);
        rigidBody.AddForce((transform.position - player.position).normalized * deathForce, ForceMode.Impulse);

        // disable AI
        enabled = false;

        //add to player score
        //GetComponent<EnemyScoring>().AddScore();
    }

}


