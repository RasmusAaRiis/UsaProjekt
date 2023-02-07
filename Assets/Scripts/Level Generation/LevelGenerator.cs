using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    private GameObject player; //ÆNDRES SENERE
    [SerializeField] private Room[] allRooms; // 0 = Elevator, 1 = Breakroom

    [SerializeField] private List<Room> currentRooms;

    public int maxLevelLength = 10;

    private void Start()
    {
        CreateLevel();
    }

    void CreateLevel()
    {
        Room elevator = allRooms[0];
        player = Instantiate(playerPrefab, Vector3.zero + Vector3.up, Quaternion.identity);
        Instantiate(elevator.roomPrefab, Vector3.zero, Quaternion.identity);
        elevator.SetExitFromChildren();
        currentRooms.Add(elevator);
        CreateNewRoom();
    }

    void CreateNewRoom()
    {
        if (currentRooms.Count >= maxLevelLength)
        {
            Debug.Log("Level Complete!");
            return;
        }

        int rand = Random.Range(1, allRooms.Length - 1); //ÆNDRE 1 TIL 2 SENERE
        Room newRoom = allRooms[rand];
        newRoom.SetEntranceFromChildren();
        GameObject newObj = Instantiate(newRoom.roomPrefab, currentRooms[currentRooms.Count - 1].exitPos.position, Quaternion.identity);

        MoveRoom(newRoom, newObj, currentRooms[currentRooms.Count - 1].exitPos);

        Door newExit = ChooseRandomDoor(newRoom.GetAllDoors(newObj));
        newRoom.SetExit(newExit.transform);
        

        //CreateNewRoom();
    }

    void MoveRoom(Room room, GameObject objectToMove, Transform endTransform)
    {
        Vector3 posDifference = room.entrancePos.transform.localPosition - endTransform.localPosition;
        Debug.Log(posDifference);
        Vector3 rotDifference = room.entrancePos.transform.rotation.eulerAngles + endTransform.rotation.eulerAngles;

        objectToMove.transform.position = room.entrancePos.transform.position + posDifference;

        objectToMove.transform.eulerAngles = rotDifference;
    }

    Door ChooseRandomDoor(Door[] doors)
    {
        Door door = doors[Random.Range(0, doors.Length)];
        return door;
    }
}
