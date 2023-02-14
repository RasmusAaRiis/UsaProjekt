using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stapler : Weapon
{
    public int ammo = 10;
    float rangedDamage = 1;
    float rangedSpeed = 1;
    
    public GameObject Projectile;

    public override void Start()
    {
        base.Start();

        rangedDamage = orgDamage;
        rangedSpeed = attackSpeed;
    }

    public override void Attack()
    {
        base.Attack();
        CharacterController cc = GetComponentInParent<CharacterController>();
        damage = rangedDamage * cc.rangedDamageModifier;
        attackSpeed = rangedSpeed * cc.rangedSpeedModifier;

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
        proj_m.GetComponent<Rigidbody>().velocity = (cc.target.position - cc.Hand.position).normalized * attackSpeed;
        Projectile proj = proj_m.GetComponent<Projectile>();
        proj.origin = this;
        proj.Activate();
        Destroy(proj_m, 5);
    }
}
