using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Weapon origin;
    Rigidbody rb;

    public void Activate()
    {
        rb = GetComponent<Rigidbody>();
        origin.attacking = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        origin.Damage(collision, true);
    }
}
