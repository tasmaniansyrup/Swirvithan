using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionDetection : MonoBehaviour
{

    public Rigidbody enemyRB;
    public enemyAI enemyScript;
    public float knockback;
    public GameObject player;
    public AudioSource malletSmack;

    // Called when a collider is touching the trigger (weapon collider)
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Enemy")
        {
            // Gets rigidbody from collider
            enemyRB = other.attachedRigidbody;

            // Gets enemy script from collider
            enemyScript = enemyRB.gameObject.GetComponent<enemyAI>();

            if(enemyScript.isAlive)
            {
                // Removes rotational restraints
                enemyRB.constraints = RigidbodyConstraints.None;

                // Creates a vector pointing from the player to the enemy for the knockback direction
                Vector3 knockbackDir = player.transform.position - enemyRB.gameObject.transform.position;
                knockbackDir.y = knockbackDir.y + 1;

                // knocks enemy back
                enemyRB.AddForce(knockbackDir.normalized * -knockback, ForceMode.Impulse);
            
                enemyScript.isAlive = false;
                malletSmack.Play();
                

                if(other.gameObject.name.Contains("Arm")) {
                    Debug.Log("Arm - 15");
                }
                else if(other.gameObject.name.Contains("Leg")) {
                    Debug.Log("Leg - 20");
                }
                else if(other.gameObject.name.Contains("head")) {
                    Debug.Log("Head! - 35");
                }
                else if(other.gameObject.name.Contains("torso")) {
                    Debug.Log("Torso - 30");
                }
            }
        }
    }
}
