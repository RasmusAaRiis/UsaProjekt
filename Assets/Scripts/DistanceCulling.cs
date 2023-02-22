using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CullingType
{
    Rigidbody,
    Light
}

    

[DisallowMultipleComponent]
public class DistanceCulling : MonoBehaviour
{
    public CullingType cullingType = CullingType.Rigidbody;
    
    public static float maxDistance = 20;
    
    private Transform camera;

    private Rigidbody rb;

    private Light l;
    
    private void Start()
    {
        switch (cullingType)
        {
            case CullingType.Rigidbody:
                rb = GetComponent<Rigidbody>();
                break;
            case CullingType.Light:
                l = GetComponent<Light>();
                break;
        }
        
        if (Camera.main != null) camera = Camera.main.transform;
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, camera.transform.position);
        
        switch (cullingType)
        {
            case CullingType.Rigidbody:
                rb.isKinematic = !(distance <= maxDistance);
                break;
            case CullingType.Light:
                l.enabled = !(distance <= maxDistance);
                break;
        }
        
        
    }
}
