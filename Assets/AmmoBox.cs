using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Stapler stapler;
        if (collision.transform.GetComponentInChildren<Stapler>())
        {
            stapler = collision.transform.GetComponentInChildren<Stapler>();
            stapler.ammo += 10;
            Destroy(gameObject);
        }
    }
}
