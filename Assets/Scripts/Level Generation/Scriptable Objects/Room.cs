using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Room", menuName = "Room")]
public class Room : ScriptableObject
{
    public GameObject roomPrefab;
    private Door entranceDoor;
    private Door exitDoor;

    public Door GetEntranceDoor()
    {
        return entranceDoor;
    }

    public Door GetExitDoor()
    {
        return exitDoor;
    }
}
