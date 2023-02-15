using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Stapler stapler;
        FireExtinguisher fireExtinguisher;
        if (other.transform.GetComponentInChildren<Stapler>())
        {
            Debug.Log("Go");
            stapler = other.transform.GetComponentInChildren<Stapler>();
            CharacterController cc = other.transform.GetComponent<CharacterController>();
            int appliedAmmo = 20 * cc.ammoModifier;
            stapler.ammo += appliedAmmo;
            cc.PickupText("+" + appliedAmmo + " Ammo");
            Destroy(gameObject);
            Debug.Log("Go");
        }
        if (other.transform.GetComponentInChildren<FireExtinguisher>())
        {
            fireExtinguisher = other.transform.GetComponentInChildren<FireExtinguisher>();
            CharacterController cc = other.transform.GetComponent<CharacterController>();
            int appliedAmmo = 10 * cc.ammoModifier;
            fireExtinguisher.ammo += appliedAmmo;
            cc.PickupText("+" + appliedAmmo + " Ammo");
            Destroy(gameObject);
        }
    }
}
