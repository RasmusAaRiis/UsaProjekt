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
    [HideInInspector]
    public bool throwing = false;

    private void Start()
    {
        transform.tag = "Throwable";
    }

    virtual public void Attack()
    {

    }

    float throwForce = 1;
    public void Throw(float throwForce)
    {
        this.throwForce = throwForce;
        throwing = true;
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if(!throwing)
        {
            throwForce = 1;
            return;
        }

        Damage(collision, false);
        throwForce = 1;
        throwing = false;
    }

    public bool Damage(Collision collision, bool ceiling)
    {
        Rigidbody rb;
        if (collision.transform.TryGetComponent<Rigidbody>(out rb))
        {
            rb.AddForce(transform.forward * knockback * throwForce, ForceMode.Impulse);
        } else
        {
            return false;
        }

        EnemyMovement em = null;

        if (collision.transform.GetComponentInParent<EnemyMovement>())
        {
            em = collision.transform.GetComponentInParent<EnemyMovement>();
        }

        float velocity = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z);
        if (em != null || collision.transform.TryGetComponent<EnemyMovement>(out em))
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
