using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stapler : Weapon
{
    public GameObject Projectile;

    public override void Attack()
    {
        //base.Attack();

        GameObject proj_m = Instantiate(Projectile, transform.position, Quaternion.identity);
        proj_m.GetComponent<Rigidbody>().velocity = transform.forward * 10;
    }
}
