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
            CharacterController cc = other.transform.GetComponent<CharacterController>();
            int appliedAmmo = 20 * cc.ammoModifier;
            stapler.ammo += appliedAmmo;
            cc.PickupText("+" + appliedAmmo + " Ammo");
            Destroy(gameObject);
        }
    }
}
