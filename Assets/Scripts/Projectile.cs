using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Weapon origin;
    Rigidbody rb;

    bool active = false;
    public void Activate()
    {
        rb = GetComponent<Rigidbody>();
        origin.attacking = true;
        active = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!active)
        {
            return;
        }
        origin.Damage(collision, true);
        //AudioManager.instance.PlayOneShot(FMODEvents.instance.genericHit, this.transform.position);
        active = false;
    }
}
