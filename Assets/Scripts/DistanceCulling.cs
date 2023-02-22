using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum CullingType
{
    Rigidbody,
    Light,
    Collider,
    GameObject,
    GameObjectChild
}

    

[DisallowMultipleComponent]
public class DistanceCulling : MonoBehaviour
{
    public CullingType cullingType = CullingType.Rigidbody;
    
    public static float maxDistance = 20;

    private float distance;
    
    private Transform camera;

    private Rigidbody rb;

    private float rbMassCopy;

    private Light l;

    private Collider c;

    private Transform rbTrans;

    private GameObject go;
    
    private void Start()
    {
        switch (cullingType)
        {
            case CullingType.Rigidbody:
                rb = GetComponent<Rigidbody>();
                rbTrans = rb.transform;
                rbMassCopy = rb.mass;
                break;
            case CullingType.Light:
                l = GetComponent<Light>();
                break;
            case CullingType.Collider:
                c = GetComponent<Collider>();
                break;
            case CullingType.GameObject:
                go = gameObject;
                break;
            case CullingType.GameObjectChild:
                go = transform.GetChild(0).gameObject;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        if (Camera.main != null) camera = Camera.main.transform;
    }

    private void Update()
    {
        distance = Vector3.Distance(transform.position, camera.transform.position);
        
        switch (cullingType)
        {
            case CullingType.Rigidbody:
                if (distance <= maxDistance && !rbTrans.GetComponent<Rigidbody>())
                {
                    Rigidbody addedRb = rbTrans.AddComponent<Rigidbody>();
                    addedRb.mass = rbMassCopy;
                    rb = addedRb;
                }
                else if(distance > maxDistance&& rbTrans.GetComponent<Rigidbody>())
                {
                    Destroy(rbTrans.GetComponent<Rigidbody>());
                }
                break;
            
            case CullingType.Light:
                l.enabled = !(distance >= maxDistance);
                break;
            
            case CullingType.Collider:
                c.enabled = !(distance >= maxDistance);
                break;
            
            case CullingType.GameObject:
                go.SetActive(!(distance >= maxDistance));
                break;
            
            case CullingType.GameObjectChild:
                go.SetActive(!(distance >= maxDistance));
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
