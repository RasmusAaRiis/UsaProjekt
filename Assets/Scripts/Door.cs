using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public void ActivateDoor()
    {
        transform.GetChild(0).transform.GetChild(0).tag = "Door";
    }
}
