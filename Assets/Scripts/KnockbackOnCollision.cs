using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackOnCollision : MonoBehaviour
{
    public float knockbackStrength;

    private void OnTriggerEnter(Collider collider)
    {
        Rigidbody rb = collider.GetComponent<Rigidbody>();

        if(rb != null)
        {
            Vector3 direction = collider.transform.position - transform.position;
            direction.y = 0;

            //Debug.Log(rb);

            rb.AddForce(direction.normalized * knockbackStrength, ForceMode.Impulse);
        }
    }
}
