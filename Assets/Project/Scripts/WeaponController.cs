using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{

    [Header("Objects")]
    public Collider chainsawHitbox;
    public Collider malletHitbox;
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

    public void Awake() {
        malletHitbox.enabled = false;
        chainsawHitbox.enabled = false;
    }
    void Update()
    {
        // Block input if paused
        if (GameManager.Instance.State == GameState.Paused)
        {
            return;
        }

        if(!isAttacking && playerAnimator != null)
        {
            if(Input.GetKeyDown(chainSawButton))
            {
                GameManager.Instance.currGas -= 1;

                playerAnimator.SetInteger("Attack", 2);

                IEnumerator chainsawDelay = chainsawHitboxDelayer();
                StartCoroutine(chainsawDelay);

                isAttacking = true;

            }
            else if(Input.GetKey(malletButton))
            {
                playerAnimator.SetInteger("Attack", 1);
                playerAnimator.speed = 1f;

                isAttacking = true;

                IEnumerator malletDelay = malletHitboxDelayer();
                StartCoroutine(malletDelay);

                IEnumerator malletDuration = malletAttackDuration();
                StartCoroutine(malletDuration);
            }
        }
        else if (playerAnimator.GetInteger("Attack") == 2)
        {
            if(Input.GetKeyUp(chainSawButton))
            {
                playerAnimator.SetInteger("Attack", 0);

                chainsawHitbox.enabled = false;
                
                isAttacking = false;
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

    public IEnumerator chainsawHitboxDelayer()
    {
        yield return new WaitForSeconds(.267f);
        chainsawHitbox.enabled = true;
    }


    public IEnumerator malletHitboxDelayer()
    {
        yield return new WaitForSeconds(.333f * malletHitboxDelay);
        malletHitbox.enabled = true;

        IEnumerator malletDuration = malletDurationTurnOff();
        StartCoroutine(malletDuration);
    }

    public IEnumerator malletDurationTurnOff()
    {
        yield return new WaitForSeconds(malletHitboxDuration);
        malletHitbox.enabled = false;
    }
}
