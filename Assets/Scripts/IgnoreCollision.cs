using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollision : MonoBehaviour
{
    [SerializeField] private Collider[] colliders1;
    
    [SerializeField] private Collider[] colliders2;
    
    void Start()
    {
        foreach (var t in colliders2)
        {
            foreach (var t1 in colliders1)
            {
                Physics.IgnoreCollision(t1, t);
            }
        }
    }
}
