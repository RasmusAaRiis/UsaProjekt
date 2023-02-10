using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyShoot : MonoBehaviour
{
    [SerializeField] private GameObject spawnableObject;
    
    [SerializeField] private Transform spawnTransform;
    
    [SerializeField] private Vector2 spawnCooldown;

    [SerializeField] private bool shootTarget;

    [SerializeField] private float shootDistance;

    [SerializeField] private float shootForce = 100;

    [SerializeField] private float shootLifetime = 2;

    [SerializeField] private LayerMask raycastMask;

    [SerializeField] private float shootDifference = 30;
    
    [SerializeField] private float timer;
    [SerializeField] private float timerValue;
    
    private Transform target;

    private void Update()
    {
        target = this.GetComponent<EnemyMovement>().target;
        
        if (Physics.Raycast(this.transform.position, target.position - this.transform.position, out var hit, shootDistance, raycastMask))
        {

            if (hit.transform.gameObject.CompareTag("Player"))
            {
                var forward = transform.forward;
                
                Vector3 differencePositive = new Vector3(Quaternion.LookRotation(forward).x,
                    Quaternion.LookRotation(forward).eulerAngles.y + shootDifference,
                    Quaternion.LookRotation(forward).z);
                
                Vector3 differenceNegative = new Vector3(Quaternion.LookRotation(forward).x,
                    Quaternion.LookRotation(forward).eulerAngles.y - shootDifference,
                    Quaternion.LookRotation(forward).z);
                
                if (Quaternion.LookRotation(target.position - this.transform.position).eulerAngles.y >= Quaternion.Euler(differencePositive).eulerAngles.y 
                    || Quaternion.LookRotation(target.position - this.transform.position).eulerAngles.y <= Quaternion.Euler(differenceNegative).eulerAngles.y)
                {
                    shootTarget = false;
                    spawnTransform.localEulerAngles = Vector3.zero;
                }
                else
                {
                    shootTarget = true;
                    spawnTransform.LookAt(target);
                }
            }
            else
            {
                shootTarget = false;
                spawnTransform.localEulerAngles = Vector3.zero;
            }
        }
        else
        {
            shootTarget = false;
            spawnTransform.localEulerAngles = Vector3.zero;
        }
        
        
        if (shootTarget)
        {
            timerValue += Time.deltaTime;
        
            if (timer == 0)
            {
                timer = Random.Range(spawnCooldown.x, spawnCooldown.y);
            }
        
            if (timerValue >= timer)
            {
                GameObject spawn = Instantiate(spawnableObject, spawnTransform.position, spawnTransform.rotation);
                spawn.GetComponent<EnemyBullet>().target = target;
                spawn.GetComponent<EnemyBullet>().forceStrength = shootForce;
                spawn.GetComponent<EnemyBullet>().lifetime = shootLifetime;
                
                timer = 0;
                timerValue = 0;
            }
        }
    }
}
