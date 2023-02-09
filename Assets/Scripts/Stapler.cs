using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stapler : Weapon
{
    public int ammo = 10;
    public GameObject Projectile;

    public override void Attack()
    {
        if (cooldown || ammo <= 0)
        {
            return;
        }

        StartCoroutine(Cooldown());
        
        ammo--;

        Animator animator = transform.parent.GetComponent<Animator>();
        animator.SetTrigger("Shoot");

        GameObject proj_m = Instantiate(Projectile, transform.position + (transform.forward * 0.2f) + (-transform.right * 0.2f) + (transform.up * 0.2f), Quaternion.LookRotation(transform.forward));
        proj_m.GetComponent<Rigidbody>().velocity = transform.forward * attackSpeed;
        Projectile proj = proj_m.GetComponent<Projectile>();
        proj.origin = this;
        proj.Activate();
        Destroy(proj_m, 5);
    }
}
