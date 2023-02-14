using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public void ActivateDoor()
    {
        int childCount = transform.GetChild(0).transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            transform.GetChild(0).transform.GetChild(i).tag = "Door";
        }
    }
}
