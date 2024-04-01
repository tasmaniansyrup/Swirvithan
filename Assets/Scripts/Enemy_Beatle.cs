using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MilkShake;

public class Enemy_Beatle : MonoBehaviour
{

    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer, whatIsEnemy, bottomOfPlayer;
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public float timeBetweenAttacks;
    public bool alreadyAttacked;
    public float sightRange, attackRange, saveAttackRange;
    public bool playerInSightRange, playerInAttackRange;

    public WeaponController wc;
    public CollisionDetection cd;
    public RaccoonControl rc;
    public int beetleHealth = 500;
    public float knockbackCounter;
    public Animator anim;
    
    public bool isAlive = true;
    public ParticleSystem deathParticle;
    public ParticleSystem hit_Scratches;
    public Material aliveAntenna;
    public Material deadAntenna;
    public GameObject renderedBody;

    public Rigidbody rb;
    public Rigidbody player_rb;
    public Transform groundCheck;
    public Transform headbuttCheck;
    public bool isOnTop = false;
    public bool isAttackingBeatle = false;
    public bool playerKnockbackTimer;
    public Vector3 boxShape;
    public Transform topCheck;
    public int dmgMult = 1;
    public float roamSpeed = 2, chaseSpeed = 4;
    public int knockbackMult = 1;

    public Shaker EnemyShaker;
    public ShakePreset EnemyShakePreset;
    public bool isHit;
    public bool inArea = false;
    public GameObject Location;
    public LayerMask locLayer;
    public bool isChasing;
    public bool flinched;
    public bool canAttack;

    public GameObject spiritCollider;

    private void Start()
    {
        var emission = deathParticle.emission;
        emission.enabled = false;
        saveAttackRange = attackRange;
        canAttack = true;
        //spiritCollider.SetActive(false);
    }

    private void Awake()
    {
        player = GameObject.Find("FP_Character").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(Location.transform.position);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawCube(topCheck.position, boxShape);
    }

    private void Update()
    {
        if(isHit) {
            playHitEffect();
        }

        if(Physics.CheckSphere(headbuttCheck.position, 1, locLayer)) {
            //Debug.Log(Location.layer);
            inArea = true;
        }

        if(isAlive && inArea)
        {
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if(knockbackCounter <= 0)
            {
                rb.isKinematic = true;

                isOnTop = Physics.CheckBox(topCheck.position, boxShape, gameObject.transform.rotation, bottomOfPlayer);

                if(!playerInSightRange && !playerInAttackRange)
                {
                    Roaming();
                    anim.SetInteger("Bug_Control",0);
                    agent.speed = roamSpeed;
                    agent.acceleration = 10;
                }
                if(playerInSightRange && !playerInAttackRange)
                {
                    ChasePlayer();
                    if(agent.speed == roamSpeed)
                    {
                    knockbackCounter = .7f;
                    anim.SetInteger("Bug_Control",1);
                    }
                }
                if(playerInAttackRange && playerInSightRange)
                {
                    AttackPlayer();
                }
                if(isOnTop)
                {
                    anim.SetInteger("Bug_Control",7);
                    StartCoroutine(ThrowPlayer());
                    //Debug.Log("Top dome");
                    knockbackCounter = .8f;
                }
            }
            else if( knockbackCounter > 0)
            {
                knockbackCounter -= Time.deltaTime;
                agent.SetDestination(transform.position);
            }

            checkBeetleHealth();

            if(playerKnockbackTimer)
            {
                Vector3 playerKnockback = player_rb.transform.position - transform.position;
                playerKnockback.y = 0f;
                player_rb.AddForce(playerKnockback.normalized * 3 * knockbackMult, ForceMode.Impulse);
                playerKnockback.z -= Time.deltaTime;
                playerKnockback.x -= Time.deltaTime;
            }

        }
        else if(!isAlive){
            spiritCollider.transform.Translate(Vector3.up * Time.deltaTime * 1, Space.World);
        }
    }

    private void Roaming()
    {
        StartCoroutine(quitChasing());
        if(!walkPointSet)
        {
            SearchWalkPoint();
        }

        if(walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if(distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    IEnumerator quitChasing() {
        yield return new WaitForSeconds(.1f);
        
        isChasing = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.CheckSphere(transform.position, 8, locLayer)) {
            if(walkPoint.z > transform.position.z) {
                walkPoint.z = transform.position.z - walkPoint.z;
            }
        }

        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        if(!isChasing) {
            StartCoroutine(beatleShocked());
        }
        else if(isChasing) {
            anim.SetInteger("Bug_Control",1);
            agent.speed = chaseSpeed;
            agent.acceleration = 15;
            agent.SetDestination(player.position);
        }
    }

    IEnumerator beatleShocked() {
        yield return new WaitForSeconds(.3f);
        isChasing = true;
    }

    public void AttackPlayer()
    {
        IEnumerator resetAttack = ResetAttack();
        IEnumerator HitTimeStart = hitTimeStart();
        IEnumerator HitTimeEnd = hitTimeEnd();
        IEnumerator PlayerKnockBackTime = playerKnockbackTime();

        if(flinched) {
            //Debug.Log("Flinched");
            StopCoroutine(resetAttack);
            StopCoroutine(HitTimeStart);
            StopCoroutine(HitTimeEnd);
            StopCoroutine(PlayerKnockBackTime);
            isAttackingBeatle = false;
            alreadyAttacked = true;
        }

        if(!flinched) {
            agent.SetDestination(transform.position);

            Vector3 targetPos = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);


            Quaternion target = Quaternion.LookRotation(targetPos - transform.position);

            target.x = 0f;
            target.z = 0f;

            transform.rotation = Quaternion.Slerp(transform.rotation, target, 5 * Time.deltaTime);

            if(!alreadyAttacked)
            {
                // Try Coroutine here
                //Invoke(nameof(ResetAttack), timeBetweenAttacks);
                StartCoroutine(resetAttack);
                //Debug.Log("attack ifstat");
                alreadyAttacked = true;
                anim.SetInteger("Bug_Control",3);

                StartCoroutine(HitTimeStart);
                StartCoroutine(HitTimeEnd);
            }
            else{
                //anim.SetInteger("Bug_Control", 1);
            }

            if(isAttackingBeatle && Physics.CheckSphere(headbuttCheck.position, attackRange, whatIsPlayer) && alreadyAttacked && !flinched && canAttack)
            {
                //Debug.Log("print once");
                isAttackingBeatle = false;
                wc.playerHealth -= 20 * dmgMult;
                playerKnockbackTimer = true;
                EnemyShaker.Shake(EnemyShakePreset);
                StartCoroutine(PlayerKnockBackTime);
            }
        }
    }

    public IEnumerator playerKnockbackTime()
    {
        yield return new WaitForSeconds(.15f);
        playerKnockbackTimer = false;
    }

    public IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(1f);
        alreadyAttacked = false;
    }

    public IEnumerator hitTimeStart()
    {
        yield return new WaitForSeconds(.45f);
        //Debug.Log("Aaaaaaaafaf");
        isAttackingBeatle = true;
        attackRange += 1;
    }

    public IEnumerator hitTimeEnd()
    {
        yield return new WaitForSeconds(.65f);
        isAttackingBeatle = false;
        attackRange = saveAttackRange;
    }

    private void checkBeetleHealth()
    {
        if(beetleHealth <= 0)
        {
            StartCoroutine(DeathTime());
            isAlive = false;
            gameObject.layer = LayerMask.NameToLayer("Dead_Beatle");
            spiritCollider.SetActive(true);
        }
    }

    private void changeMaterial()
    {
        Material[] materials = renderedBody.GetComponent<Renderer>().materials;
        materials[1] = deadAntenna;
        renderedBody.GetComponent<Renderer>().materials = materials;
    }

    IEnumerator DeathTime()
    {
        yield return new WaitForSeconds(.25f);
        isAlive = false;
        anim.SetInteger("Bug_Control",2);
        rb.isKinematic = true;
        var emission = deathParticle.emission;
        emission.enabled = true;
        StartCoroutine(stopParticles());
    }

    private void playHitEffect() {
        //Debug.Log("HIT");
        hit_Scratches.transform.position = cd.AxeHead.position;
        Vector3 targetPos = new Vector3(cd.Camera.position.x, cd.Camera.position.y, cd.Camera.position.z);
        Quaternion target = Quaternion.LookRotation(cd.AxeHead.transform.position - rb.transform.position);
        hit_Scratches.transform.rotation = target;

        hit_Scratches.Play();
        isHit = false;
    }

    IEnumerator stopParticles()
    {
        yield return new WaitForSeconds(4);
        var emission = deathParticle.emission;
        emission.enabled = false;
        changeMaterial();
    }

    IEnumerator ThrowPlayer()
    {
        if(Physics.CheckBox(topCheck.position, boxShape, topCheck.rotation, bottomOfPlayer))
        {
            yield return new WaitForSeconds(.3f);
            Vector3 throwDirection = player_rb.transform.position - transform.position;
            // Controls direction of force
            if(Mathf.Abs(throwDirection.y/throwDirection.x) < 2|| Mathf.Abs(throwDirection.y/throwDirection.z) < 2)
            {
                throwDirection.y += 1;
            }
            if(Mathf.Abs(throwDirection.y/throwDirection.x) > 8|| Mathf.Abs(throwDirection.y/throwDirection.z) > 8)
            {
                throwDirection.y -= 1;
            }

            player_rb.AddForce(throwDirection.normalized * 10, ForceMode.Impulse);
            //Debug.Log(throwDirection);
            isOnTop = false;
        }
    }

}
