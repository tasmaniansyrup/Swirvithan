using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
    public Rigidbody spine;
    public DecalProjector bloodpool;

    public float desiredDuration;
    public float elapsedTime;
    public LayerMask whatIsGround;
    


    // Makes enemy alive when enemy first spawns
    void Awake()
    {
        isAlive = true;
        canBeHit = true;
        setRigidbodyState(true);
        setColliderState(false);
        

        bloodpool.size = new Vector3(0f, 0f, 1f);
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


            if(gotHit)
            {
                canBeHit = false;
                IEnumerator rgh = resetGotHit(.1f);
                enemyAnimator.SetBool("Hit", true);
                StartCoroutine(rgh);
            }
        }
        if(enemyHealth <= 0)
        {
            die();
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

        if(spine.velocity.magnitude <= 0.1f)
        {
            Vector3 startSize = new Vector3(0f, 0f, 1f);
            Vector3 spilledSize = new Vector3(2f, 2f, 1f);

            elapsedTime += Time.deltaTime;
            

            float percentComplete = elapsedTime / desiredDuration;

            bloodpool.size = Vector3.Lerp(startSize, spilledSize, Mathf.SmoothStep(0, 1, percentComplete));

            bloodpool.transform.position = new Vector3(spine.transform.position.x, bloodpool.transform.position.y, spine.transform.position.z);

            if(Physics.Raycast(spine.transform.position, Vector3.down, out RaycastHit groundHit, 2f, whatIsGround)) {
                bloodpool.transform.position = groundHit.transform.position;
            }
        }

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
}
