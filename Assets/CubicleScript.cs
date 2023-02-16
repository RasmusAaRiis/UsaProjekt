using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubicleScript : MonoBehaviour
{
    public GameObject[] drawers;
    public ROSP[] drawerROSPS;

    [Header("Debug")]
    public bool unlockAfterLoad;

    private void Start()
    {
        if (unlockAfterLoad)
        {
            Invoke("UnlockDrawers", 3);
        }
    }

    public void UnlockDrawers()
    {
        Debug.Log("Test");
        for (int i = 0; i < drawers.Length; i++)
        {
            drawers[i].GetComponent<ConfigurableJoint>().xMotion = ConfigurableJointMotion.Limited;
            drawers[i].GetComponent<ConfigurableJoint>().breakForce = 500;
        }
    }

    public void ActivateROSPS()
    {
        for (int i = 0; i < drawerROSPS.Length; i++)
        {
            drawerROSPS[i].SpawnObject();
        }
    }
}
