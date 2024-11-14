using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class enemyAI : MonoBehaviour
{
    [Header("Attack Timing")]

    public float attackHitboxDuration;
    public float attackHitboxDelay;
    public float attackAnimationDuration = 1.049f;
    public bool isAttacking = false;
    public Collider attackHitbox;
    
    public GameObject player;
    public float targetDistance;
    public float attackDistance;
    public float playerDistance;
    public bool isAlive;
    public float enemySpeed;
    public Rigidbody enemyRB;
    public int enemyHealth;
    public float knockback;
    public bool gotHit;
    public bool canBeHit;
    public Animator enemyAnimator;
    public GameObject rig;
    public GameObject enemyColliders;
    public Transform spine;
    public DecalProjector bloodpool;


    // Makes enemy alive when enemy first spawns
    void Awake()
    {
        isAlive = true;
        canBeHit = true;
        setRigidbodyState(true);
        setColliderState(false);

        bloodpool.size = new Vector3(3f, 3f, 1f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Only runs if enemy is alive
        if(isAlive)
        {
            // distance from player
            playerDistance = Vector3.Distance(player.transform.position, gameObject.transform.position);

            Vector3 enemyLookAt = new Vector3(player.transform.position.x, gameObject.transform.position.y, player.transform.position.z);

            // If player is in enemy range and is not in attack range
            if(playerDistance < targetDistance && playerDistance > attackDistance && canBeHit) 
            {

                enemyAnimator.SetInteger("Speed", 1);
                enemyAnimator.SetInteger("Attack", 1);

                Vector3 flatVel = new Vector3(enemyRB.velocity.x, 0f, enemyRB.velocity.z);

                // Rotates object to look at player

                gameObject.transform.LookAt(enemyLookAt);

                // Adds force to enemy in direction of player
                enemyRB.AddForce(gameObject.transform.forward.normalized * enemySpeed * 10f, ForceMode.Force);
                
                Debug.DrawRay(gameObject.transform.position, gameObject.transform.forward, Color.red);


                // Prevents enemy from going over set speed
                if(flatVel.magnitude > enemySpeed && canBeHit)
                {
                    Vector3 limitedVel = flatVel.normalized * enemySpeed;
                    enemyRB.velocity = new Vector3(limitedVel.x, enemyRB.velocity.y, limitedVel.z);
                }

            } // Stops enemy if close to player
            else if(playerDistance <= attackDistance && canBeHit && !isAttacking)
            {
                gameObject.transform.LookAt(enemyLookAt);
                enemyRB.velocity = Vector3.zero;
                enemyAnimator.SetInteger("Speed", 0);
                enemyAnimator.SetInteger("Attack", 2);

                isAttacking = true;

                IEnumerator attackDuraction = scientistAttackDuration();
                StartCoroutine(attackDuraction);

                IEnumerator attackDelay = scientistHitboxDelayer();
                StartCoroutine(attackDelay);
            }

            if(enemyHealth <= 0)
            {
                die();
            }

            if(gotHit)
            {
                canBeHit = false;
                IEnumerator rgh = resetGotHit(.1f);
                enemyAnimator.SetBool("Hit", true);
                StartCoroutine(rgh);
            }
        }
    }

    public void die() 
    {
        enemyHealth = 0;

        enemyAnimator.enabled = false;

        setRigidbodyState(false);
        setColliderState(true);

        Collider[] colliders = enemyColliders.GetComponentsInChildren<Collider>();

        foreach(Collider collider in colliders)
        {
            collider.enabled = false;
        }

        // Removes rotational restraints
        enemyRB.constraints = RigidbodyConstraints.None;

        // Creates a vector pointing from the player to the enemy for the knockback direction
        Vector3 knockbackDir = player.transform.position - gameObject.transform.position;
        // knockbackDir.y = knockbackDir.y + 1;

        // knocks enemy back

        Debug.Log("He dead!");

        GameManager.Instance.enemiesKilled += 1;
    
        isAlive = false;

    }

    void setRigidbodyState(bool state) {

        Rigidbody[] rigidbodies = rig.GetComponentsInChildren<Rigidbody>();

        foreach(Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = state;
        }

        GetComponent<Rigidbody>().isKinematic = !state;
    }

    void setColliderState(bool state) {

        Collider[] colliders = rig.GetComponentsInChildren<Collider>();

        foreach(Collider collider in colliders)
        {
            collider.enabled = state;
        }

        GetComponent<Collider>().enabled = !state;
    }

    public IEnumerator resetGotHit(float timeHit) 
    {
        yield return new WaitForSeconds(timeHit);
        enemyAnimator.SetBool("Hit", false);
        canBeHit = true;
        gotHit = false;
    }

    public IEnumerator scientistAttackDuration()
    {
        yield return new WaitForSeconds(attackAnimationDuration);
        isAttacking = false;
    }

    public IEnumerator scientistHitboxDelayer()
    {
        yield return new WaitForSeconds(1.042f * attackHitboxDelay);
        attackHitbox.enabled = true;

        IEnumerator attackDuration = scientistDurationTurnOff();
        StartCoroutine(attackDuration);
    }

    public IEnumerator scientistDurationTurnOff()
    {
        yield return new WaitForSeconds(attackHitboxDuration);
        attackHitbox.enabled = false;
    }
}
