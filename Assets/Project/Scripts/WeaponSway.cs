using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public float amount;
    public float maxAmount;
    public float smoothAmount;
    public Vector3 vel = Vector3.one;

    private Vector3 iniitialPosition;
    // Start is called before the first frame update
    void Start()
    {
        iniitialPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        float movementX = -Input.GetAxis("Mouse X") * amount;
        float movementY = -Input.GetAxis("Mouse Y") * amount;
        movementX = Mathf.Clamp(movementX, -maxAmount, maxAmount);
        movementY = Mathf.Clamp(movementY, -maxAmount, maxAmount);

        Vector3 finalPosition = new Vector3(movementX, movementY, 0f);
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition + iniitialPosition, ref vel, Time.deltaTime * smoothAmount);
    }
}
