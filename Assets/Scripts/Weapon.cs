using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float attackRange = 1f;
    public float attackSpeed = 1f;
    public float knockback = 1f;
    public float attackCooldown = 1f;

    virtual public void Attack()
    {

    }
}
