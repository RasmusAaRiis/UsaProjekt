using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubicleScript : MonoBehaviour
{
    public GameObject[] drawers;
    public ROSP[] drawerROSPS;

    private void Awake()
    {
        ActivateROSPS();
        for (int i = 0; i < drawers.Length; i++)
        {
            drawers[i].GetComponent<ConfigurableJoint>().anchor = drawers[i].transform.position;
        }
        Invoke("UnlockDrawers", 1f);
    }

    private void Update()
    {
        if (drawers.Length <= 0)
        {
            Destroy(this);
        }

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
            drawers[i].GetComponent<ConfigurableJoint>().anchor = drawers[i].transform.position;
            drawers[i].GetComponent<Rigidbody>().isKinematic = false;
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
