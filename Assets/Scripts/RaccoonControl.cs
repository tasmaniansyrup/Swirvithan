using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RaccoonControl : MonoBehaviour
{

    public bool isInRange;
    public Transform player;
    public Rigidbody playerRB;
    public UnityEngine.AI.NavMeshAgent clyde;
    public Vector3 newWalkPoint;
    public Animator animator;
    public float distanceToPlayer;
    public PlayerMovement pm;
    public float lerpVal;
    public LayerMask enemy;
    public MultiAimConstraint ac;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = player.position;
        Vector3 raccoonPosition = gameObject.transform.position;

        playerPosition.y = 0f;
        raccoonPosition.y = 0f;

        distanceToPlayer = Vector3.Distance(playerPosition, raccoonPosition);

        if(distanceToPlayer < 3.5f) {
            if(lerpVal < 1) {
                lerpVal += .1f;
            }
            clyde.speed = 0f;
        }

        followPlayer();
        animatorUpdater();
        
        if(Physics.CheckSphere(transform.position, 15, enemy)) {
            
        }

    }

    private void followPlayer() {
        
        clyde.SetDestination(player.position);

        Quaternion target = Quaternion.LookRotation(player.position - transform.position);

            target.x = 0f;
            target.z = 0f;

        transform.rotation = Quaternion.Slerp(transform.rotation, target, 5 * Time.deltaTime);

        if(distanceToPlayer > 3f) {
            clyde.speed = distanceToPlayer;
        }
        

    }
    private void animatorUpdater() {
        if(clyde.speed > 5) {
            animator.SetInteger("Speed", 2);
            if(ac.weight > 0) {
                ac.weight -= 0.025f;
            }
        }
        else if(clyde.speed > 0.1f) {
            animator.SetInteger("Speed", 1);
            if(ac.weight < 0.5f) {
                ac.weight += .025f;
            }
        }
        else {
            animator.SetInteger("Speed", 0);
        }

        animator.speed = distanceToPlayer/5;
    }
}
