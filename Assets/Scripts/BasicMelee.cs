using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMelee : Weapon
{
    Animator animator;
    Vector3 originalPosition;
    bool cooldown = false;

    bool attacking = false;
    public override void Attack()
    {
        if (cooldown)
        {
            return;
        }

        if(!attacking)
        {
            attacking = true;
        }

        StartCoroutine(Cooldown());

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

    private void OnCollisionEnter(Collision collision)
    {
        if(!attacking)
        {
            return;
        }
        Rigidbody rb;
        if(collision.transform.TryGetComponent<Rigidbody>(out rb))
        {
            rb.AddForce(transform.forward * knockback, ForceMode.Impulse);
        }
    }

    IEnumerator Cooldown()
    {
        cooldown = true;
        yield return new WaitForSeconds(attackCooldown);
        cooldown = false;
    }
}
