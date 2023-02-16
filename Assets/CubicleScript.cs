using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubicleScript : MonoBehaviour
{
    public GameObject[] drawers;
    public ROSP[] drawerROSPS;

    [Header("Debug")]
    public bool unlockAfterLoad;

    private void Awake()
    {
        ActivateROSPS();
    }

    private void Start()
    {
        if (unlockAfterLoad)
        {
            Invoke("UnlockDrawers", 3);
        }
    }

    private void Update()
    {
        for (int i = 0; i < drawers.Length; i++)
        {
            if (!drawers[i].GetComponent<ConfigurableJoint>())
            {
                Debug.Log("Test");
                drawers[i].layer = 7;
                drawers[i].AddComponent<BasicMelee>();
                BasicMelee newMelee = drawers[i].GetComponent<BasicMelee>();
                newMelee.attackRange = 2;
            }
        }
    }

    public void UnlockDrawers()
    {
        for (int i = 0; i < drawers.Length; i++)
        {
            if (drawers[i].GetComponent<ConfigurableJoint>())
            {
                drawers[i].GetComponent<ConfigurableJoint>().xMotion = ConfigurableJointMotion.Limited;
                drawers[i].GetComponent<ConfigurableJoint>().breakForce = 500;
            }
        }
    }

    public void ActivateROSPS()
    {
        for (int i = 0; i < drawerROSPS.Length; i++)
        {
            Debug.Log("Test");
            drawerROSPS[i].SpawnObject();
        }
    }
}
