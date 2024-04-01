using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public WeaponController wc;
    public ParticleSystem hit_Scratches;
    private Enemy_Beatle eb;
    public Transform AxeHead;
    public Transform Camera;
    public bool canFlinch;
    
    private void    OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy" && wc.isAttacking)
        {
            //Debug.Log(AxeHead.position);
            eb = other.GetComponentInParent<Enemy_Beatle>();
            eb.isHit = true;
            if(eb.isAlive)
            {
                if(canFlinch){
                    //canFlinch = false;
                    StartCoroutine(inflictFlinch());
                    eb.canAttack = false;
                    eb.flinched = true;
                    StartCoroutine(flinchTime());
                    wc.isAttacking = false;
                    eb.anim.SetInteger("Bug_Control",5);
                    eb.rb.isKinematic = false;
                    eb.knockbackCounter = .3f;
                }
                eb.beetleHealth -= 50;
                
            }
        }
    }

    IEnumerator flinchTime(){
        yield return new WaitForSeconds(.6f);
        eb.flinched = false;
        eb.canAttack = true;
        eb.alreadyAttacked = false;
        
    } // ? ballsack somewhere

    IEnumerator inflictFlinch() {
        yield return new WaitForSeconds(2f);
        canFlinch = true;
    }
}
