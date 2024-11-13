using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBarHandler : MonoBehaviour
{
    public Image healthBar;
    public Image staminaBar;
    public Image gasBar;


    void Start()
    {
        
    }

    void Update()
    {
        // If the user is using the chainsaw slowly reduce amount of gas
        if (Input.GetMouseButtonDown(1))
        {

        }
    }
}
