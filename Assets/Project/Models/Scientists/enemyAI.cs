using System;
using System.Collections;
using System.Collections.Generic;
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


    // Makes enemy alive when enemy first spawns
    void Awake()
    {
        isAlive = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Only runs if enemy is alive
        if(isAlive)
        {
            // distance from player
            playerDistance = Vector3.Distance(player.transform.position, gameObject.transform.position);

            // If player is in enemy range and is not in attack range
            if(playerDistance < targetDistance && playerDistance > attackDistance) 
            {
                // Rotates object to look at player
                gameObject.transform.LookAt(player.transform);

                // Adds force to enemy in direction of player
                enemyRB.AddForce(gameObject.transform.forward.normalized * enemySpeed * 10f, ForceMode.Force);

                // Prevents enemy from going over set speed
                if(enemyRB.velocity.magnitude > enemySpeed)
                {
                    enemyRB.velocity = enemyRB.velocity.normalized * enemySpeed;
                }

            } // Stops enemy if close to player
            else if(playerDistance <= attackDistance)
            {
                gameObject.transform.LookAt(player.transform);
                enemyRB.velocity = Vector3.zero;
            }
        }
    }
}
