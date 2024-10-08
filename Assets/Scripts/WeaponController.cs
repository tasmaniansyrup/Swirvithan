using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{

    [Header("Objects")]
    public GameObject chainsaw;
    public GameObject mallet;
    public Animator playerAnimator;

    [Header("Keybinds")]

    public KeyCode chainSawButton = KeyCode.Mouse0;
    public KeyCode malletButton = KeyCode.Mouse1;

    // Update is called once per frame cool!
    void Update()
    {
        
        if(Input.GetKey(chainSawButton))
        {
            playerAnimator.SetInteger("Attack", 1);
        }
        else if(Input.GetKey(malletButton))
        {
            playerAnimator.SetInteger("Attack", 2);
        }
        else
        {
            playerAnimator.SetInteger("Attack", 0);
        }
    } 
}
