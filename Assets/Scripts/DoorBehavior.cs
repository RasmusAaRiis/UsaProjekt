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
        cj.angularYMotion = ConfigurableJointMotion.Limited;
        rb.AddForce(-transform.right * 10000, ForceMode.Impulse);
    }
}
