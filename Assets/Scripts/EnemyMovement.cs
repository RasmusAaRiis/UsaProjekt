using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Instrumentation;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;

public enum EnemyType
{
    Melee,
    Ranged
}

public class EnemyMovement : MonoBehaviour
{
    public EnemyType enemyType = EnemyType.Melee;
    
    [SerializeField] private bool debugMode;
    
    [SerializeField] private Rigidbody rb;

    public Transform target;
    
    public float health = 100;

    [Range(0f, 1f)][SerializeField] private float rotationStrength = 0.2f;
    [SerializeField] private float speed = 1;
    [SerializeField] private float jumpStrenght = 20;
    [SerializeField] private float targetLookOffset;
    [SerializeField] private Vector2 jumpCooldown = new Vector2(0.8f, 1f);
    
    [Tooltip("Only applicable if the enemy is of ranged type")]
    [SerializeField] private float chaseRadius = 3;

    [SerializeField] private bool resetVel;
    public bool chaseTarget;
    [SerializeField] private bool stabilize = true;

    [SerializeField] private bool onGround;

    [SerializeField] private TextMeshProUGUI debugText;

    [SerializeField] private DamagePopup damagePopup;
    private Transform targetCamera = null;

    private Quaternion startRot;
    private float timerValue;
    private float timer;

    private bool justDied = true;

    private void Start()
    {
        var tarIsMissing = !ReferenceEquals(target, null) && !target;
        if (tarIsMissing) { target = FindObjectOfType<CharacterController>().transform; }

        rb = GetComponent<Rigidbody>();

        TryGetComponent<DamagePopup>(out damagePopup);

        startRot = transform.rotation;

        jumpStrenght *= rb.mass;
        speed *= rb.mass;
    }

    private void Update()
    {
        var tarIsMissing = !ReferenceEquals(target, null) && !target;
        if (tarIsMissing) { target = FindObjectOfType<CharacterController>().transform; }

        debugText.enabled = debugMode;
        
        if (debugMode)
        {
            debugText.text = target.name;
        }

        if (damagePopup)
        {
            if (!targetCamera)
            {
                targetCamera = target.GetComponentInChildren<Camera>().transform;
            }
            damagePopup.anim.transform.LookAt(targetCamera);
        }

        if (health <= 0f)
        {
            chaseTarget = false;
            stabilize = false;
            debugText.text = "";
            if (justDied == true ) 
            { 
                AudioManager.instance.PlayOneShot(FMODEvents.instance.enemyDead, this.transform.position); 
                justDied = false;  
            }

            switch (enemyType)
            {
                case EnemyType.Melee:
                    Destroy(gameObject.GetComponent<EnemyMovement>());
                    break;
                case EnemyType.Ranged:
                    Destroy(gameObject.GetComponent<EnemyShoot>());
                    Destroy(gameObject.GetComponent<EnemyMovement>());
                    break;
            }
        }

        Vector3 chasePosition = new Vector3();
        
        switch (enemyType)
        {
            case EnemyType.Melee:
                chasePosition = target.position;
                break;
            case EnemyType.Ranged:
                if (target.gameObject.CompareTag("Player"))
                {
                    Ray r = new Ray(this.transform.position, target.position - this.transform.position);
                    var dist = Vector3.Distance(transform.position, target.position);
                    chasePosition = r.GetPoint(dist - chaseRadius);
                }
                else
                {
                    chasePosition = target.position;
                }
                break;
                
        }

        if (chaseTarget && !tarIsMissing)
        {
            Vector3 direction = chasePosition - this.transform.position;
        
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

        if (this.transform.position.y < -1) { health = 0;  }
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
        if (damagePopup)
        {
            damagePopup.DisplayDamage(value.ToString());
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (debugMode)
        {
            var tarIsMissing = !ReferenceEquals(target, null) && (!target);

            var position = transform.position;
        
           
            
            if (!tarIsMissing)
            {
                switch (enemyType)
                {
                    case EnemyType.Melee:
                        break;
                    case EnemyType.Ranged:
                        Ray r = new Ray(this.transform.position, target.position - this.transform.position);
                        Gizmos.DrawRay(r);
                        var dist = Vector3.Distance(transform.position, target.position);
                        Gizmos.color = Color.magenta;
                        Gizmos.DrawSphere(r.GetPoint(dist - chaseRadius), 0.1f);
                        break;
                }
                
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
