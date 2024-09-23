using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{

    public GameObject Axe;
    public bool CanAttack = true;
    public float AttackCooldown;
    public Animator animator;
    public AudioClip AxeAttackSound;
    public bool isAttacking = false;
    public bool attackAgain = false;
    public bool willAttack = false;
    public float playerHealth = 100;
    public Image healthBar;
    public GameObject deathScene;

    public ParticleSystem ability1_Left;
    public ParticleSystem ability1_Right;
    public Transform playerCameraTransform;

    public bool canSpecialAttack;
    public bool canUseSpecialAttack;
    public bool isSpecialAttacking;
    public float specialAttackPercentage;
    public Image specialAttackBar;
    public ParticleToPoint ptp;
    public Material slMaterial;
    public Material glMaterial;
    public Renderer rend;
    public float shinyNumber;
    public Shader particleShader;


    // Update is called once per frame cool!
    void Update()
    {
        Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward*30, Color.red);
        
        if(Input.GetMouseButtonDown(0))
        {
            if(CanAttack && !attackAgain)
            {
                AxeAttack();
            }
            else if(attackAgain)
            {
                willAttack = true;
            }
        }
        
        if(specialAttackPercentage >= 100) {
            canSpecialAttack = true;
            if(Input.GetKey(KeyCode.Q)) {
                ability1_Left.Play();
                ability1_Right.Play();

                IEnumerator ability1chargetime = Ability1ChargeTime();
                IEnumerator ability1timeinuse = Ability1TimeInUse();

                StartCoroutine(ability1chargetime);
                StartCoroutine(ability1timeinuse);
                isSpecialAttacking = true;
            }
        }

        if(canSpecialAttack && shinyNumber < 1f) {
            shinyNumber += 0.05f;
            slMaterial.SetFloat("Shiny Number", shinyNumber);
        }
        int _shinyNumber;
        _shinyNumber = Shader.PropertyToID("ShinyNumber");
        shinyNumber += 0.05f;
            slMaterial.SetFloat(_shinyNumber, shinyNumber);

        if(isSpecialAttacking && Input.GetMouseButtonDown(0)) {
            ptp.enabled = true;
            ptp.pSystem = ability1_Left;
        }


        // Accurate Health Checker
        healthBar.fillAmount = playerHealth/100f;

        checkPlayerHealth();
    }

    public IEnumerator Ability1ChargeTime()
    {
        yield return new WaitForSeconds(2f);
        canUseSpecialAttack = true;
    }

    public IEnumerator Ability1TimeInUse()
    {
        yield return new WaitForSeconds(6f);
        canUseSpecialAttack = false;
    }

    public void AxeAttack()
    {
        StartCoroutine(StartAttackPhase());
        StartCoroutine(ResetAttackBool());
        CanAttack = false;
        attackAgain = false;
        willAttack = false;
        animator.SetInteger("Attacks", 1);
        //AudioSource ac = GetComponent<AudioSource>();
        //ac.PlayOneShot(AxeAttackSound);
        StartCoroutine(ResetAttackCooldown());
    }

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(AttackCooldown);
        CanAttack = true;
        attackAgain = false;
        if(willAttack) {
            //Debug.Log("OOg");
            AxeAttack();
        }
    }

    IEnumerator ResetAttackBool()
    {
        yield return new WaitForSeconds(.5f);
        isAttacking = false;
        attackAgain = true;
        animator.SetInteger("Attacks", 0);
    }

    IEnumerator StartAttackPhase()
    {
        // Accounts for wind up time and ensures that the enemy is hit after you start swinging
        yield return new WaitForSeconds(.3f);
        isAttacking = true;
    }

    void checkPlayerHealth()
    {
        if(playerHealth <= 0)
        {
            Time.timeScale = 0f;
            deathScene.SetActive(true);
        }
    }
}
