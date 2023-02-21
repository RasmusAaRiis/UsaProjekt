using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Instrumentation;
using DitzelGames.FastIK;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.AI;

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

    [SerializeField] private NavMeshAgent navAgent;

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

    [SerializeField] private Renderer goopRenderer;

    [SerializeField] private Material goopMaterial;

    [SerializeField] private DamagePopup damagePopup;
    private Transform targetCamera = null;

    private Quaternion startRot;
    private float timerValue;
    private float timer;

    private Vector3 navMeshTarget;

    private bool justDied = true;

    [Range(0f,1f)][SerializeField] private float tentacleSpeed = 0.2f;

    [SerializeField] private List<FastIKFabric> ikTentas;
    [SerializeField] private List<Transform> ikTargets;
    [SerializeField] private List<Transform> ikTargetsGoto;

    private static readonly int MoveSpeed = Shader.PropertyToID("_MoveSpeed");
    private static readonly int Saturation = Shader.PropertyToID("_Saturation");
    private static readonly int Size = Shader.PropertyToID("_Size");

    private void Start()
    {
        ikTargets.Clear();
        ikTargetsGoto.Clear();
        
        for (int i = 0; i < ikTentas.Count; i++)
        {
            ikTargets.Add(ikTentas[i].Target);
            
            GameObject go = new GameObject
            {
                transform =
                {
                    position = ikTargets[i].position,
                    //parent = ikTargets[i].parent,
                    name = "go" + i+1
                }
            };

            ikTargetsGoto.Add(go.transform);
            ikTentas[i].Target = ikTargetsGoto[i].transform;
        }
        
        goopMaterial = goopRenderer.material;
        
        var tarIsMissing = !ReferenceEquals(target, null) && !target;
        if (tarIsMissing) { target = FindObjectOfType<CharacterController>().transform; }

        rb = GetComponent<Rigidbody>();

        TryGetComponent<DamagePopup>(out damagePopup);

        startRot = transform.rotation;

        jumpStrenght *= rb.mass;
        speed *= rb.mass;
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < ikTentas.Count; i++)
        {
            GoTowards(ikTargetsGoto[i], ikTargets[i], tentacleSpeed);
        }
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
            goopMaterial.SetFloat(MoveSpeed, Mathf.Lerp(goopMaterial.GetFloat(MoveSpeed), 0, 0.03f));
            goopMaterial.SetFloat(Saturation,Mathf.Lerp(goopMaterial.GetFloat(Saturation), 0.3f, 0.03f));
            goopMaterial.SetFloat(Size,Mathf.Lerp(goopMaterial.GetFloat(Size), 0, 0.03f));
            
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
                    Destroy(gameObject.GetComponent<EnemyMovement>(), 2f);
                    break;
                case EnemyType.Ranged:
                    Destroy(gameObject.GetComponent<EnemyShoot>(), 2f);
                    Destroy(gameObject.GetComponent<EnemyMovement>(), 2f);
                    break;
            }
        }

        Vector3 chasePosition = new Vector3();
        
        switch (enemyType)
        {
            case EnemyType.Melee:
                navAgent.destination = target.position;
                navAgent.transform.localPosition = Vector3.zero;
                
                //print(navAgent.path.corners.Length + " navmesh corners");
                
                if (navAgent.path.corners.Length >= 2)
                {
                    chasePosition = navAgent.path.corners[1];
                }
                
                //print(Vector3.Distance(chasePosition, this.transform.position));
                
                //Debug.DrawLine(chasePosition, transform.position, Color.yellow);
                
                if (Vector3.Distance(chasePosition, this.transform.position) <= 1 && navAgent.path.corners.Length >= 3)
                {
                    chasePosition = navAgent.path.corners[2];
                }
                
                if (navAgent.path.corners.Length < 3)
                {
                    chasePosition = target.position;
                }
                
                navMeshTarget = chasePosition;
                
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

            default:
                throw new ArgumentOutOfRangeException();
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


    private void GoTowards(Transform mover, Transform target, float speed)
    {
        mover.position = Vector3.Lerp(mover.position, target.position, speed);
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
        AudioManager.instance.PlayOneShot(FMODEvents.instance.genericHit, this.transform.position);
        if (damagePopup)
        {
            damagePopup.DisplayDamage(value.ToString());
        }
        if(health > 0)
        {
            return;
        }

        for (int i = 0; i < ikTentas.Count; i++)
        {
            ikTargets[i].position = ikTargetsGoto[i].position; 
            ikTentas[i].Target = ikTargets[i];
        }
        
        if (rb.mass < 12)
        {
            BasicMelee bm = gameObject.AddComponent<BasicMelee>();
            bm.attackRange = 3;
            bm.damage = 15;
            bm.attackCooldown = 0.4f;
            bm.knockback = 5;
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

                for (int i = 0; i < navAgent.path.corners.Length; i++)
                {
                    if (i ==  0)
                    {
                        Gizmos.DrawRay(position, navAgent.path.corners[i] - position);
                    }
                    else
                    {
                        Gizmos.DrawRay(navAgent.path.corners[i-1], navAgent.path.corners[i] - navAgent.path.corners[i-1]);
                    }
                    
                }

                Gizmos.DrawSphere(navMeshTarget, 0.2f);

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
