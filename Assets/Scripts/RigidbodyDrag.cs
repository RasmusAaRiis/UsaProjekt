using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class RigidbodyDrag : MonoBehaviour
{
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

    // Start is called before the first frame update
    void Start()
    {
        dragObject = this.gameObject;
    }
    
    void Update()
    {
        isHolding = Input.GetKey(KeyCode.Mouse0);
        
        if (Physics.Raycast(transform.position, transform.forward, out var hit, reachDistane) && !isDragging && cc.heldObject == null)
        {
            if (hit.transform.GetComponent<Rigidbody>() != null)
            {
                if (hit.transform.GetComponent<Rigidbody>().isKinematic == false && Input.GetKeyDown(KeyCode.Mouse0) && hit.transform.GetComponent<EnemyMovement>() == null)
                {
                    //print(hit.transform.name);
                    isDragging = true;
                    dragObject = hit.transform.gameObject;
                    hitPos = hit.point;
                }
            }
        }

        if (dragObject && (!isHolding || Vector3.Distance(dragObject.transform.position, transform.forward * 2 + transform.position) >= reachDistane))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 center = new Vector3();
            
            if (dragObject.GetComponent<Renderer>() == null)
            {
                center = dragObject.GetComponentInChildren<Renderer>().bounds.center;
            }
            else
            {
                center = dragObject.GetComponent<Renderer>().bounds.center;
            }

            lineRenderer.SetPosition(0, center);
            lineRenderer.SetPosition(1, transform.forward * 2 + transform.position);
            float lineWidthEnd = lineWidth / Vector3.Distance(dragObject.transform.position, transform.forward * 2 + transform.position);
            lineWidthEnd = Mathf.Clamp(lineWidthEnd, 0.01f, lineWidth);
            lineRenderer.SetWidth(lineWidth, lineWidthEnd);

            forceStrengthDist = forceStrength / Vector3.Distance(dragObject.transform.position,
                transform.forward * 2 + transform.position);
            forceStrengthDist = Mathf.Clamp(forceStrengthDist, 0, forceStrength);
            dragObject.GetComponent<Rigidbody>().AddForce((transform.forward * 2 + transform.position - dragObject.transform.position) * forceStrengthDist * Time.deltaTime);
        }
        else
        {
            lineRenderer.SetWidth(0, 0);
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}
