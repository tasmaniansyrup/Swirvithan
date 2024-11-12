using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class enemyAI : MonoBehaviour
{
    
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
    public Rigidbody ragdoll;


    // Makes enemy alive when enemy first spawns
    void Awake()
    {
        isAlive = true;
        canBeHit = true;
        rig.SetActive(false);
        enemyColliders.SetActive(true);
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
            else if(playerDistance <= attackDistance && canBeHit)
            {
                gameObject.transform.LookAt(enemyLookAt);
                enemyRB.velocity = Vector3.zero;
                enemyAnimator.SetInteger("Speed", 0);
                enemyAnimator.SetInteger("Attack", 2);
            }

            checkHealth();

            if(gotHit)
            {
                canBeHit = false;
                IEnumerator rgh = resetGotHit(.1f);
                enemyAnimator.SetBool("Hit", true);
                StartCoroutine(rgh);
            }
        }
    }

    public void checkHealth() 
    {
        if(enemyHealth <= 0)
        {
            enemyHealth = 0;

            enemyAnimator.enabled = false;
            enemyRB.isKinematic = true;
            enemyColliders.SetActive(false);
            rig.SetActive(true);

            // Removes rotational restraints
            enemyRB.constraints = RigidbodyConstraints.None;

            // Creates a vector pointing from the player to the enemy for the knockback direction
            Vector3 knockbackDir = player.transform.position - gameObject.transform.position;
            // knockbackDir.y = knockbackDir.y + 1;

            // knocks enemy back
            ragdoll.AddForce(knockbackDir.normalized * -knockback, ForceMode.Impulse);

            Debug.Log("He dead!");

            GameManager.Instance.enemiesKilled += 1;
        
            isAlive = false;
        }
    }

    public IEnumerator resetGotHit(float timeHit) 
    {
        yield return new WaitForSeconds(timeHit);
        enemyAnimator.SetBool("Hit", false);
        canBeHit = true;
        gotHit = false;
    }
}
