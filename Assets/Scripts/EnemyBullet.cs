using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public Transform target;
    
    public float forceStrength;
    [Range(0f, 1f)] public float rotationStrength;

    [SerializeField] private Rigidbody rb;
    
    [SerializeField] private bool followTarget;
    
    [SerializeField] private float rotationCalculated;

    [SerializeField] private float lifetime = 1;
    private float timerValue;
    
    private void Start()
    {
        forceStrength *= rb.mass;
    }

    private void Update()
    {
        timerValue += Time.deltaTime;
        if (timerValue >= lifetime)
        {
            followTarget = false;
            rb.useGravity = true;
        }
        
        if (followTarget)
        {
            Vector3 direction = target!.position - this.transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            rotationCalculated = rotationStrength * (Vector3.Distance(this.transform.position, target.position) / 2);
            Quaternion newRot = Quaternion.Lerp(this.transform.rotation, lookRotation, rotationCalculated);
        
            rb.MoveRotation(newRot);
            rb.velocity = Vector3.zero;
            rb.AddForce(transform.forward * forceStrength);
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        followTarget = false;
        rb.useGravity = true;
    }
}
