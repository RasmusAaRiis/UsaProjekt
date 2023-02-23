using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum CullingType
{
    Rigidbody,
    Light,
    Collider,
    GameObject,
    GameObjectChilds,
    ReflectionProbe
}

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

    [Tooltip("Don't touch")][SerializeField] private List<GameObject> go;

    private ReflectionProbe rp;

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
                go.Add(gameObject);
                break;
            
            case CullingType.GameObjectChilds:
                for (int i = 0; i < transform.childCount; i++)
                {
                    GameObject item = transform.GetChild(i).gameObject;
                    go.Add(item);
                }
                print(go.Count);
                break;
            
            case CullingType.ReflectionProbe:
                rp = GetComponent<ReflectionProbe>();
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (Camera.main != null) camera = Camera.main.transform;
    }

    private void Update()
    {
        if (Camera.main != null && !camera) camera = Camera.main.transform;

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
                go[0].SetActive(!(distance >= maxDistance));
                break;
            
            case CullingType.GameObjectChilds:
                foreach (var t in go)
                {
                    t.SetActive(!(distance >= maxDistance));
                }
                break;
            
            case CullingType.ReflectionProbe:
                rp.enabled = !(distance >= maxDistance);
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        
    }
}

