using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMelee : Weapon
{
    Animator animator;
    Vector3 originalPosition;
    
    public override void Attack()
    {
        base.Attack();

        if (cooldown)
        {
            return;
        }

        StartCoroutine(Cooldown());

        if (!attacking)
        {
            attacking = true;
        }

        originalPosition = transform.localPosition;
        animator = transform.parent.GetComponent<Animator>();
        animator.speed = attackSpeed;
        animator.SetTrigger("Attack");
    }

    float i = 0;
    private void FixedUpdate()
    {
        if(!attacking)
        {
            return;
        }
        i += 0.1f * attackSpeed;
        transform.localPosition = Vector3.Slerp((i <= 0.5f ? originalPosition : originalPosition + Vector3.forward * attackRange), (i <= 0.5f ? originalPosition + Vector3.forward * attackRange : originalPosition), i);
        
        if(i >= 1)
        {
            transform.localPosition = originalPosition;
            attacking = false;
            i = 0;
        }
    }

    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        AudioManager.instance.PlayOneShot(FMODEvents.instance.genericHit, this.transform.position);

        if (!attacking)
        {
            return;
        }

        if (!Damage(collision, false))
        {
            return;
        }

        transform.localPosition = originalPosition;
        i = 0;
    }
}
