using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public enum DetectionType
{
    Individual,
    Layer
}
public class IgnoreCollision : MonoBehaviour
{
    [SerializeField] private DetectionType detectionType;

    [SerializeField] private string[] ignoreMask;

    [SerializeField] private List<Collider> affectedColliders;
    
    [SerializeField] private List<Collider> otherColliders;

    private void Start()
    {
        //IgnoreColliders();
    }

    public void IgnoreColliders()
    {
        otherColliders.Clear();
        
        switch (detectionType)
        {
            case DetectionType.Individual:
                
                foreach (var t in otherColliders)
                {
                    foreach (var t1 in affectedColliders)
                    {
                        Physics.IgnoreCollision(t1, t);
                    }
                }
                return;
            
            case DetectionType.Layer:
                
                Collider[] colliders = Physics.OverlapBox(transform.position, new Vector3(3,3,3));
                foreach (var t in colliders)
                {
                    foreach (var t1 in ignoreMask)
                    {
                        if (t.gameObject.layer == LayerMask.NameToLayer(t1))
                        {
                            otherColliders.Add(t); 
                        }
                    }
                }

                if (otherColliders.Count > 0)
                {
                    foreach (var t in otherColliders)
                    {
                        foreach (var t1 in affectedColliders)
                        {
                            Physics.IgnoreCollision(t1, t);
                        }
                    }
                }
                return;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(3,3,3));
    }
}
