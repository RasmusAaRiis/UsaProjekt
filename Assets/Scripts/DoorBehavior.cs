using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    [SerializeField] private ConfigurableJoint cj;

    [SerializeField] private Rigidbody rb;

    private void Start()
    {
        cj = GetComponentInChildren<ConfigurableJoint>();
        rb = GetComponentInChildren<Rigidbody>();
    }

    public void OpenDoor()
    {
        rb.isKinematic = false;
        cj.angularYMotion = ConfigurableJointMotion.Limited;
        rb.AddForce(-transform.right * 5000, ForceMode.Impulse);
    }
}
