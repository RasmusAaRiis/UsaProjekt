using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class RigidbodyDrag : MonoBehaviour
{
    [SerializeField] private float reachDistane = 5;

    [SerializeField] private bool isDragging;

    [SerializeField] private bool isHolding;

    [SerializeField] private Transform dragObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    void Update()
    {
        isHolding = Input.GetKey(KeyCode.Mouse0);
        
        if (Physics.Raycast(transform.position, transform.forward, out var hit, reachDistane))
        {
            if (!ReferenceEquals(hit.transform.GetComponent<Rigidbody>(), null))
            {
                if (hit.transform.GetComponent<Rigidbody>().isKinematic == false && Input.GetKeyDown(KeyCode.Mouse0))
                {
                    print(hit.transform.name);
                    isDragging = true;
                    dragObject = hit.transform;
                }
            }
        }

        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}
