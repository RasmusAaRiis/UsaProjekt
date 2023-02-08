using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    [SerializeField] private Transform target;

    [SerializeField] private LayerMask groundMask;

    [Range(0f, 1f)][SerializeField] private float rotationStrength;
    [SerializeField] private float speed = 1;
    [SerializeField] private float jumpStrenght = 20;

    [SerializeField] private bool resetVel;
    [SerializeField] private bool chaseTarget;
    [SerializeField] private bool stabilize;

    [SerializeField] private bool onGround;
    
    private Quaternion startRot;
    private float timer;

    private void Start()
    {
        var tarIsMissing = !ReferenceEquals(target, null) && !target;
        if (tarIsMissing) { target = GameObject.FindWithTag("Player").transform; }

        var rbIsMissing = !ReferenceEquals(rb, null) && !rb;
        if (rbIsMissing) { rb = GetComponent<Rigidbody>(); }
        
        startRot = Quaternion.identity;
    }

    private void Update()
    {
        
        
        var tarIsMissing = !ReferenceEquals(target, null) && (!target);
        
        if (chaseTarget && !tarIsMissing)
        {
            Vector3 direction = target!.position - this.transform.position;
        
            direction.x = Mathf.Clamp(direction.x, -5f, 5f);
            direction.y = Mathf.Clamp(direction.y, -5f, 5f);
            direction.z = Mathf.Clamp(direction.z, -5f, 5f);
        
            Vector3 force = new Vector3(direction.x * speed, direction.y + jumpStrenght, direction.z * speed);
        
            //Debug.Log(direction);
        
            timer += Time.deltaTime;
        
            float value = 0;
            value = Random.Range(0.8f, 1f);
        
            if (timer >= value)
            {
                if (resetVel)
                {
                    rb.velocity = Vector3.zero;
                }
            
                rb.AddForce(force * 5);
                
                if (stabilize)
                {
                    
                }
                
                force = Vector3.zero;
                timer = 0;
            }
        }

        if (!onGround && stabilize)
        {
            Quaternion newRot = Quaternion.Lerp(this.transform.rotation, startRot, rotationStrength);
            rb.MoveRotation(newRot);
        }
    }

    private void OnCollisionStay(Collision collisionInfo)
    {
        onGround = collisionInfo.gameObject.layer == LayerMask.NameToLayer("Ground");
    }

    private void OnCollisionExit(Collision other)
    {
        onGround = false;
    }

    private void OnDrawGizmosSelected()
    {
        var tarIsMissing = !ReferenceEquals(target, null) && (!target);

        var position = transform.position;
        
        if (!tarIsMissing)
        {
            Vector3 direction = target!.position - position;
            //Debug.Log(direction);
            Gizmos.DrawRay(position, direction);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(position, -transform.up);
            Gizmos.DrawRay(position, transform.up);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(position, -transform.right);
            //Gizmos.DrawRay(position, transform.right);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(position, -transform.forward);
            Gizmos.DrawRay(position, transform.forward);
        }
        Gizmos.color = Color.white;
        Gizmos.DrawRay(position, Vector3.down);
        //Gizmos.DrawRay(position, Vector3.left);
    }
}
