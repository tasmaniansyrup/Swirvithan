using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class doorController : MonoBehaviour
{

    public HingeJoint hinge;
    public Rigidbody hingeRB;
    public float knockback;
    public float openForce;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    public void openDoor()
    {
        if(hinge != null)
        {
            if(hinge.angle < 90)
            {
                var motor = hinge.motor;
                motor.force = 100;
                motor.targetVelocity = 90;

                hinge.motor = motor;
                hinge.useMotor = true;
                
                IEnumerator stopMotor = stopHingeMotor();
                StartCoroutine(stopMotor);
            } else if(hinge.angle >= 90)
            {

                var motor = hinge.motor;
                motor.force = 100;
                motor.targetVelocity = -90;

                hinge.motor = motor;

                hinge.useMotor = true;
                IEnumerator stopMotor = stopHingeMotor();
                StartCoroutine(stopMotor);
            }
        }
    }

    public void bustOpenDoor(Transform player) 
    {
        if(hinge != null)
        {
            hinge.breakForce = 0f;
            hinge.breakTorque = 0f;

            Vector3 knockbackDir = player.transform.position - gameObject.transform.position;
            knockbackDir.y = knockbackDir.y + 1;

            hingeRB.AddForce(knockbackDir.normalized * -knockback, ForceMode.Impulse);
        }
    }

    public IEnumerator stopHingeMotor() {
        yield return new WaitForSeconds(.1f);
        hinge.useMotor = false;
    }
}