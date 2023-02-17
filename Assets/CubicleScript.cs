using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubicleScript : MonoBehaviour
{
    public List<GameObject> drawers;
    public ROSP[] drawerROSPS;

    private void Awake()
    {
        ActivateROSPS();
        for (int i = 0; i < drawers.Count; i++)
        {
            drawers[i].GetComponent<ConfigurableJoint>().anchor = drawers[i].transform.position;
        }
        Invoke("UnlockDrawers", 1f);
    }

    private void Update()
    {
        if (drawers.Count <= 0)
        {
            Destroy(this);
        }

        for (int i = 0; i < drawers.Count; i++)
        {
            if (!drawers[i].GetComponent<ConfigurableJoint>())
            {
                drawers[i].layer = 7;
                drawers[i].transform.GetChild(0).gameObject.layer = 7;
                drawers[i].AddComponent<BasicMelee>();
                BasicMelee newMelee = drawers[i].GetComponent<BasicMelee>();
                newMelee.attackRange = 2;
                drawers.RemoveAt(i);
            }
        }
    }

    public void UnlockDrawers()
    {
        for (int i = 0; i < drawers.Count; i++)
        {
            drawers[i].GetComponent<ConfigurableJoint>().anchor = drawers[i].transform.position;
            drawers[i].GetComponent<Rigidbody>().isKinematic = false;
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
