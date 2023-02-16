using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireExtinguisher : Weapon
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

        AudioManager.instance.PlayOneShot(FMODEvents.instance.spray, this.transform.position);
        for (int i = 0; i < 8; i++)
        {
            GameObject proj_m = Instantiate(Projectile, transform.position + (transform.forward * 0.4f), Quaternion.LookRotation(transform.forward));
            proj_m.GetComponent<Rigidbody>().velocity = (transform.right * Random.Range(-5f, 3f)) + (transform.up * Random.Range(-2f, 7f)) + (transform.forward * 2) * attackSpeed;
            Projectile proj = proj_m.GetComponent<Projectile>();
            proj.origin = this;
            proj.Activate();
            Destroy(proj_m, 5);
        }


    }
}
