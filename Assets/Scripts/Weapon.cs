using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage = 10f;
    public float attackRange = 1f;
    public float attackSpeed = 1f;
    public float knockback = 1f;
    public float attackCooldown = 1f;
    [HideInInspector]
    public bool cooldown = false;
    [HideInInspector]
    public bool attacking = false;

    virtual public void Attack()
    {

    }

    public bool Damage(Collision collision, bool ceiling)
    {
        if (!attacking)
        {
            return false;
        }

        Rigidbody rb;
        if (collision.transform.TryGetComponent<Rigidbody>(out rb))
        {
            rb.AddForce(transform.forward * knockback, ForceMode.Impulse);
        }

        EnemyMovement em;
        float velocity = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z);
        if (collision.transform.TryGetComponent<EnemyMovement>(out em))
        {
            float appliedDamage = Mathf.RoundToInt(velocity * damage);
            if(ceiling)
            {
                appliedDamage = Mathf.Min(appliedDamage, damage);
            } else
            {
                appliedDamage = Mathf.Max(appliedDamage, damage);
            }
            em.TakeDamage(appliedDamage);
            Debug.Log(appliedDamage);
            attacking = false;
            return true;
        }

        return false;
    }

    public IEnumerator Cooldown()
    {
        cooldown = true;
        yield return new WaitForSeconds(attackCooldown);
        cooldown = false;
    }
}
