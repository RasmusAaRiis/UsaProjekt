using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Instrumentation;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private bool debugMode;
    
    [SerializeField] private Rigidbody rb;

    public Transform target;
    
    public float health = 100;

    [Range(0f, 1f)][SerializeField] private float rotationStrength = 0.2f;
    [SerializeField] private float speed = 1;
    [SerializeField] private float jumpStrenght = 20;
    [SerializeField] private float targetLookOffset;
    [SerializeField] private Vector2 jumpCooldown = new Vector2(0.8f, 1f);

    [SerializeField] private bool resetVel;
    public bool chaseTarget;
    [SerializeField] private bool stabilize = true;

    [SerializeField] private bool onGround;
    
    private Quaternion startRot;
    private float timerValue;
    private float timer;

    private bool justDied = true;

    private void Start()
    {
        
        
        var tarIsMissing = !ReferenceEquals(target, null) && !target;
        if (tarIsMissing) { target = GameObject.FindWithTag("Player").transform; }

        rb = GetComponent<Rigidbody>();
        
        //var rbIsMissing = !ReferenceEquals(rb, null) && !rb;
        //if (rbIsMissing) { rb = GetComponent<Rigidbody>(); }
        
        startRot = transform.rotation;
        //print(startRot);

        jumpStrenght *= rb.mass;
        speed *= rb.mass;
    }

    private void Update()
    {
        if (health <= 0f)
        {
            chaseTarget = false;
            stabilize = false;
            if (justDied == true ) 
            { 
                AudioManager.instance.PlayOneShot(FMODEvents.instance.enemyDead, this.transform.position); 
                justDied = false;  
            }
            
        }
        
        var tarIsMissing = !ReferenceEquals(target, null) && (!target);
        
        if (chaseTarget && !tarIsMissing)
        {
            Vector3 direction = target!.position - this.transform.position;
        
            direction.x = Mathf.Clamp(direction.x, -5f, 5f);
            direction.y = Mathf.Clamp(direction.y, -5f, 5f);
            direction.z = Mathf.Clamp(direction.z, -5f, 5f);
        
            Vector3 force = new Vector3(direction.x * speed, direction.y + jumpStrenght, direction.z * speed);
        
            //Debug.Log(direction);
        
            timerValue += Time.deltaTime;
            
            if (timer == 0)
            {
                timer = Random.Range(jumpCooldown.x, jumpCooldown.y);
            }

            if (timerValue >= timer)
            {
                if (resetVel)
                {
                    rb.velocity = Vector3.zero;
                }
            
                rb.AddForce(force * 5);

                force = Vector3.zero;
                timerValue = 0;
                timer = 0;
            }
        }

        if (!onGround && stabilize)
        {
            Vector3 direction = new Vector3(target!.position.x, target!.position.y + targetLookOffset, target!.position.z) - this.transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            Quaternion newRot = Quaternion.Lerp(this.transform.rotation, lookRotation, rotationStrength);
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

    public void TakeDamage(float value)
    {
        health -= value;
    }

    private void OnDrawGizmosSelected()
    {
        if (debugMode)
        {
            var tarIsMissing = !ReferenceEquals(target, null) && (!target);

            var position = transform.position;
        
            if (!tarIsMissing)
            {
                Vector3 direction = target!.position - position;
                Vector3 lookDirection = new Vector3(target!.position.x, target!.position.y + targetLookOffset, target!.position.z) - this.transform.position;
                //Debug.Log(direction);
                Gizmos.color = Color.magenta;
                Gizmos.DrawRay(position, direction);
                Gizmos.color = Color.cyan;
                Gizmos.DrawRay(position, lookDirection);
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
}
