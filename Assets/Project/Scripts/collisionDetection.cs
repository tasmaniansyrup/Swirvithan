using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionDetection : MonoBehaviour
{
    public Rigidbody enemyRB;
    public Rigidbody playerRB;
    public enemyAI enemyScript;
    public doorController dc;
    public GameObject player;
    public AudioSource malletSmack;
    public ParticleSystem bloodVFX;
    public int armDmg;
    public int legDmg;
    public int headDmg;
    public int torsoDmg;

    public string targetTag;

    public UIBarHandler UIBarHandler;

    // Called when a collider is touching the trigger (weapon collider)
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Enemy" && gameObject.tag == "Player")
        {
            // Gets rigidbody from collider
            enemyRB = other.attachedRigidbody;

            // Gets enemy script from collider
            enemyScript = enemyRB.gameObject.GetComponent<enemyAI>();

            bool isChainsaw = gameObject.name.Contains("chain");

            if(enemyScript.isAlive && !enemyScript.gotHit)
            {
                enemyScript.gotHit = true;

                // Spill blood accordingly
                GameManager.Instance.gallonsSpilled += UnityEngine.Random.Range(0.5f, 2.5f);


                malletSmack.Play();
                bloodVFX.Play();

                
                if(!isChainsaw)
                {
                    Vector3 knockbackDir = player.transform.position - gameObject.transform.position;
                    knockbackDir.y = knockbackDir.y + 1;

                    enemyRB.AddForce(knockbackDir.normalized * -10, ForceMode.Impulse);
                }

                if(other.gameObject.name.Contains("Arm")) {
                    Debug.Log("Arm - 1");
                    enemyScript.enemyHealth -= armDmg;
                }
                else if(other.gameObject.name.Contains("Leg")) {
                    Debug.Log("Leg - 1");
                    enemyScript.enemyHealth -= legDmg;
                }
                else if(other.gameObject.name.Contains("head")) {
                    Debug.Log("Head! - 3");
                    enemyScript.enemyHealth -= headDmg;
                }
                else if(other.gameObject.name.Contains("torso")) {
                    Debug.Log("Torso - 2");
                    enemyScript.enemyHealth -= torsoDmg;
                }
            }
        }
        else if (other.tag == "Player" && gameObject.tag == "Enemy")
        {
            Debug.Log("I'm hit!");
            gameObject.GetComponent<Collider>().enabled = false;

            UIBarHandler.UpdateHealth(-5f);
            playerRB = other.attachedRigidbody;

            Vector3 knockbackDir = player.transform.position - gameObject.transform.position;

            playerRB.AddForce(knockbackDir.normalized * 10, ForceMode.Impulse);
        }
        else if (other.tag == "Door" && gameObject.tag == "Player") {
            dc = other.gameObject.GetComponent<doorController>();
            dc.bustOpenDoor(player.transform);
        }
    }

}