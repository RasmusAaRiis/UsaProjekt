using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stapler : Weapon
{
    public int ammo = 10;
    float rangedDamage = 1;
    float rangedSpeed = 1;
    
    public GameObject Projectile;

    private void Start()
    {
        rangedDamage = damage;
        rangedSpeed = attackSpeed;
    }

    public override void Attack()
    {
        base.Attack();

        damage = rangedDamage * GetComponentInParent<CharacterController>().rangedDamageModifier;
        attackSpeed = rangedSpeed * GetComponentInParent<CharacterController>().rangedSpeedModifier;

        if (cooldown || ammo <= 0)
        {
            return;
        }

        StartCoroutine(Cooldown());
        
        ammo--;

        Animator animator = transform.parent.GetComponent<Animator>();
        animator.SetTrigger("Shoot");
        AudioManager.instance.PlayOneShot(FMODEvents.instance.staples, this.transform.position);
        GameObject proj_m = Instantiate(Projectile, transform.position + (transform.forward * 0.2f) + (-transform.right * 0.2f) + (transform.up * 0.2f), Quaternion.LookRotation(transform.forward));
        proj_m.GetComponent<Rigidbody>().velocity = transform.forward * attackSpeed;
        Projectile proj = proj_m.GetComponent<Projectile>();
        proj.origin = this;
        proj.Activate();
        Destroy(proj_m, 5);
    }
}
