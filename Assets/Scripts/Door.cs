using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // Animation Functions
    // ...
    // ...
    // ...

    public void ActivateDoor()
    {
        Debug.Log("TEST");

        transform.GetChild(0).transform.GetChild(0).tag = "Door";
    }
}
