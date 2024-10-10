using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{

    [Header("Objects")]
    public GameObject chainsaw;
    public GameObject malletHitbox;
    public Animator playerAnimator;

    [Header("Keybinds")]

    public KeyCode chainSawButton = KeyCode.Mouse1;
    public KeyCode malletButton = KeyCode.Mouse0;

    [Header("Attack Timing")]

    public float malletHitboxDuration;
    public float malletHitboxDelay;
    public float malletTimeDuration = 0.34f;

    [Header("Player Status")]

    public bool isAttacking = false;

    // Update is called once per frame cool!
    void Update()
    {
        if(!isAttacking)
        {
            if(Input.GetKey(chainSawButton))
            {
                playerAnimator.SetInteger("Attack", 2);

                //isAttacking = true;

            }
            else if(Input.GetKey(malletButton))
            {
                playerAnimator.SetInteger("Attack", 1);

                isAttacking = true;

                IEnumerator malletDelay = malletHitboxDelayer();
                StartCoroutine(malletDelay);

                IEnumerator malletDuration = malletAttackDuration();
                StartCoroutine(malletDuration);
            }
        }
        else
        {
            playerAnimator.SetInteger("Attack", 0);
        }
    } 


    public IEnumerator malletAttackDuration()
    {
        yield return new WaitForSeconds(malletTimeDuration);
        isAttacking = false;
    }







    public IEnumerator malletHitboxDelayer()
    {
        yield return new WaitForSeconds(.333f * malletHitboxDelay);
        malletHitbox.SetActive(true);

        IEnumerator malletDuration = malletDurationTurnOff();
        StartCoroutine(malletDuration);
    }

    public IEnumerator malletDurationTurnOff()
    {
        yield return new WaitForSeconds(malletHitboxDuration);
        malletHitbox.SetActive(false);
    }
}
