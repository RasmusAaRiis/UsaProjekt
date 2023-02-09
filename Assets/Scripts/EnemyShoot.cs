using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyShoot : MonoBehaviour
{
    [SerializeField] private GameObject spawnableObject;
    
    [SerializeField] private Transform spawnTransform;
    
    [SerializeField] private Vector2 spawnCooldown;

    [SerializeField] private bool shootTarget;

    [SerializeField] private float shootDistance;

    [SerializeField] private LayerMask raycastMask;

    private Transform target;

    private float timer;
    private float timerValue;

    private void Update()
    {
        target = this.GetComponent<EnemyMovement>().target;

        Debug.DrawRay(this.transform.position, target.position - this.transform.position);
        if (Physics.Raycast(this.transform.position, target.position - this.transform.position, out var hit, shootDistance, raycastMask))
        {

            if (hit.transform.gameObject.CompareTag("Player"))
            {
                var forward = transform.forward;
                
                Vector3 differensPositive = new Vector3(Quaternion.LookRotation(forward).x,
                    Quaternion.LookRotation(forward).eulerAngles.y + 30,
                    Quaternion.LookRotation(forward).z);
                
                Vector3 differensNegative = new Vector3(Quaternion.LookRotation(forward).x,
                    Quaternion.LookRotation(forward).eulerAngles.y - 30,
                    Quaternion.LookRotation(forward).z);
                
                if (Quaternion.LookRotation(target.position - this.transform.position).eulerAngles.y >= Quaternion.Euler(differensPositive).eulerAngles.y 
                    || Quaternion.LookRotation(target.position - this.transform.position).eulerAngles.y <= Quaternion.Euler(differensNegative).eulerAngles.y)
                {
                    shootTarget = false;
                }
                else
                {
                    shootTarget = true;
                }
            }
            else
            {
                shootTarget = true;
            }
        }
        else
        {
            shootTarget = true;
        }
        
        
        // if (shootTarget)
        // {
        //     timerValue += Time.deltaTime;
        //
        //     if (timer == 0)
        //     {
        //         timer = Random.Range(spawnCooldown.x, spawnCooldown.y);
        //     }
        //
        //     if (timerValue >= timer)
        //     {
        //         GameObject spawn = Instantiate(spawnableObject, spawnTransform);
        //         spawn.GetComponent<EnemyBullet>().target = target;
        //         
        //         timer = 0;
        //         timerValue = 0;
        //     }
        // }
    }
}
