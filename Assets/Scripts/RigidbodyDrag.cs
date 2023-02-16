using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public enum GrabType
{
    WhenEquipped,
    WhenUnequipped,
    Both
    
}

public class RigidbodyDrag : MonoBehaviour
{
    [SerializeField] private GrabType grabType;
    
    [SerializeField] private KeyCode input;

    [SerializeField] private CharacterController cc;
    
    [SerializeField] private float reachDistane = 5;

    [SerializeField] private bool isDragging;

    [SerializeField] private bool isHolding;

    [SerializeField] private GameObject dragObject;

    [SerializeField] private Vector3 hitPos;

    [SerializeField] private LineRenderer lineRenderer;

    [Range(0f, 1f)] [SerializeField] private float lineWidth = 0.5f;

    [SerializeField] private float forceStrength = 5;

    [SerializeField] private float forceStrengthDist;

    [SerializeField] private GameObject dragObjPoint;

    // Start is called before the first frame update
    void Start()
    {
        dragObject = this.gameObject;
    }
    
    void Update()
    {
        isHolding = Input.GetKey(input);
        
        if (Physics.Raycast(transform.position, transform.forward, out var hit, reachDistane) && !isDragging)
        {
            switch (grabType)
            {
                case GrabType.Both:
                    
                    break;
                
                case GrabType.WhenEquipped:
                    
                    if (cc.heldObject != null)
                    {
                        break;
                    }
                    else
                    {
                        return;
                    }
                
                case GrabType.WhenUnequipped:
                    
                    if (cc.heldObject == null)
                    {
                        break;
                    }
                    else
                    {
                        return;
                    }
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            
            if (hit.transform.GetComponent<Rigidbody>() != null)
            {
                if (hit.transform.GetComponent<Rigidbody>().isKinematic == false && Input.GetKeyDown(input) && hit.transform.GetComponent<EnemyMovement>() == null)
                {
                    //print(hit.transform.name);
                    isDragging = true;
                    dragObject = hit.transform.gameObject;
                    
                    dragObjPoint = new GameObject
                    {
                        transform =
                        {
                            position = hit.point,
                            rotation = quaternion.identity,
                            parent = hit.transform
                        },
                        name = "DragObjPoint " + hit.transform.name
                    };
                    
                    hitPos = hit.point;
                }
            }
        }

        if (dragObject && (!isHolding || Vector3.Distance(dragObject.transform.position, transform.forward * 2 + transform.position) >= reachDistane))
        {
            isDragging = false;
            
            if (dragObjPoint != null)
            {
                Destroy(dragObjPoint.gameObject);
                dragObjPoint = null;
            }
        }

        if (isDragging)
        {
            // Vector3 center = new Vector3();
            //
            // if (dragObject.GetComponent<Renderer>() == null)
            // {
            //     center = dragObject.GetComponentInChildren<Renderer>().bounds.center;
            // }
            // else
            // {
            //     center = dragObject.GetComponent<Renderer>().bounds.center;
            // }
            var forward = transform.forward;
            var thisPos = transform.position;
            var dragPos = dragObject.transform.position;
            lineRenderer.SetPosition(0, dragObjPoint.transform.position);
            lineRenderer.SetPosition(1, forward * 2 + thisPos);
            
            float lineWidthEnd = lineWidth / Vector3.Distance(dragPos, forward * 2 + thisPos);
            lineWidthEnd = Mathf.Clamp(lineWidthEnd, 0.01f, lineWidth);
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidthEnd;

            forceStrengthDist = forceStrength / Vector3.Distance(dragPos,
                forward * 2 + thisPos);
            forceStrengthDist = Mathf.Clamp(forceStrengthDist, 0, forceStrength);
            dragObject.GetComponent<Rigidbody>().AddForce((forward * 2 + thisPos - dragPos) * forceStrengthDist * Time.deltaTime);
        }
        else
        {
            lineRenderer.startWidth = 0;
            lineRenderer.endWidth = 0;
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, transform.forward * reachDistane);
    }
}
