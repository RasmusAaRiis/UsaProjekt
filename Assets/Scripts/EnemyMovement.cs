using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    [SerializeField] private Transform target;

    [SerializeField] private Transform groundCheck;

    [SerializeField] private float speed = 1;
    [SerializeField] private float jumpStrenght = 20;

    [SerializeField] private bool resetVel;
    [SerializeField] private bool chaseTarget;
    

    private float timer;

    private void Start()
    {
        var tarIsMissing = !ReferenceEquals(target, null) && !target;
        if (tarIsMissing) { target = GameObject.FindWithTag("Player").transform; }

        var rbIsMissing = !ReferenceEquals(rb, null) && !rb;
        if (rbIsMissing) { rb = GetComponent<Rigidbody>(); }
    }

    private void Update()
    {
        Vector3 stabilizer = new Vector3();
        
        
        
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
                timer = 0;
            }
        }
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
            Gizmos.color = Color.red;
            print(-target.up);
            Gizmos.DrawRay(position, -transform.up);
        }
        Gizmos.color = Color.white;
        //Gizmos.DrawRay(position, Vector3.down);
        Gizmos.DrawRay(position, Vector3.left);
    }
}
