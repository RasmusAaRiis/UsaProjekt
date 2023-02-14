using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Stapler stapler;
        if (other.transform.GetComponentInChildren<Stapler>())
        {
            stapler = other.transform.GetComponentInChildren<Stapler>();
            stapler.ammo += 10 * other.transform.GetComponent<CharacterController>().ammoModifier;
            Destroy(gameObject);
        }
    }
}
