using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    [SerializeField] private Transform target;

    [SerializeField] private float speed = 1;
    [SerializeField] private float jumpStrenght = 20;

    private float timer;
    

    private void Update()
    {
        Vector3 direction = target.position - this.transform.position;
        Vector3 force = new Vector3(direction.x * speed, direction.y + jumpStrenght, direction.z * speed);
        
        timer += Time.deltaTime;
        
        if (timer >= 1)
        {
            rb.AddForce(force * 5);
            timer = 0;
        }
    }


    private void OnDrawGizmosSelected()
    {
        Vector3 direction = target.position - this.transform.position;
        Debug.Log(direction);
        Gizmos.DrawRay(transform.position, direction);
    }
}
