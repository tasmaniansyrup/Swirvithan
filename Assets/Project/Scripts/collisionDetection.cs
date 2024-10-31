using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionDetection : MonoBehaviour
{

    public Rigidbody enemyRB;
    public enemyAI enemyScript;
    public doorController dc;
    public GameObject player;
    public AudioSource malletSmack;
    public ParticleSystem bloodVFX;
    public int armDmg;
    public int legDmg;
    public int headDmg;
    public int torsoDmg;

    // Called when a collider is touching the trigger (weapon collider)
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Enemy")
        {
            // Gets rigidbody from collider
            enemyRB = other.attachedRigidbody;

            // Gets enemy script from collider
            enemyScript = enemyRB.gameObject.GetComponent<enemyAI>();

            if(enemyScript.isAlive && !enemyScript.gotHit)
            {
                enemyScript.gotHit = true;

                malletSmack.Play();
                bloodVFX.Play();

                if(other.gameObject.name.Contains("Arm")) {
                    Debug.Log("Arm - 15");
                    enemyScript.enemyHealth -= armDmg;
                }
                else if(other.gameObject.name.Contains("Leg")) {
                    Debug.Log("Leg - 20");
                    enemyScript.enemyHealth -= legDmg;
                }
                else if(other.gameObject.name.Contains("head")) {
                    Debug.Log("Head! - 35");
                    enemyScript.enemyHealth -= headDmg;
                }
                else if(other.gameObject.name.Contains("torso")) {
                    Debug.Log("Torso - 30");
                    enemyScript.enemyHealth -= torsoDmg;
                }
            }
        }
        else if(other.tag == "Door") {
            dc = other.gameObject.GetComponent<doorController>();
            dc.bustOpenDoor(player.transform);
        }
    }

}