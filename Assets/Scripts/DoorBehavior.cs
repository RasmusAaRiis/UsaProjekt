using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    [SerializeField] private ConfigurableJoint cj;

    private void Start()
    {
        cj = GetComponentInChildren<ConfigurableJoint>();
    }

    public void OpenDoor()
    {
        cj.angularYMotion = ConfigurableJointMotion.Limited;
    }
}
