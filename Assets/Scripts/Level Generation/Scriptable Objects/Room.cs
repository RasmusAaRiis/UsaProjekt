using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Room", menuName = "Room")]
public class Room : ScriptableObject
{
    public GameObject roomPrefab;
    public ROSP[] rosps;
    public Transform entrancePos;
    public Transform exitPos;

    public Door[] GetAllDoors(GameObject objectToCheck)
    {
        Door[] allDoors = objectToCheck.GetComponentsInChildren<Door>();

        return allDoors;
    }

    #region Set Entrance
    public virtual void SetEntrance(Transform transform)
    {
        entrancePos = transform;
    }
    public virtual void SetEntranceFromChildren(string entranceName)
    {
        entrancePos = roomPrefab.transform.Find(entranceName);
    }
    public virtual void SetEntranceFromChildren()
    {
        entrancePos = roomPrefab.transform.Find("EntrancePos");
    }
    #endregion
    #region Set Exit
    public virtual void SetExit(Transform transform)
    {
        exitPos = transform;
    }
    public virtual void SetExitFromChildren(string exitName)
    {
        exitPos = roomPrefab.transform.Find(exitName);
    }
    public virtual void SetExitFromChildren()
    {
        exitPos = roomPrefab.transform.Find("ExitPos");
    }
    #endregion
}
