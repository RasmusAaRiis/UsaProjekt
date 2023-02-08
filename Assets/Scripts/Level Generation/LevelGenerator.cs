using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    private GameObject player; //ÆNDRES SENERE
    [SerializeField] private Room[] allRooms; // 0 = Elevator, 1 = Breakroom

    [SerializeField] private List<Room> currentRooms;

    public int maxLevelLength = 100;

    private void Start()
    {
        CreateLevel();
    }

    Room lastRoom;
    Room currentRoom;

    void CreateLevel()
    {
        Room elevator = allRooms[0];
        player = Instantiate(playerPrefab, elevator.roomPrefab.transform.position + Vector3.up, Quaternion.identity);
        Instantiate(elevator.roomPrefab, Vector3.zero, Quaternion.identity);
        elevator.exitDoor = elevator.get
        currentRooms.Add(elevator);
        CreateNewRoom();
    }

    int index = 0;
    void CreateNewRoom()
    {
        //Checks if the max amout of rooms is reached
        //It is > maxLevelLength because the first room is the elevator
        if (currentRooms.Count > maxLevelLength)
        {
            Debug.Log("Level Complete!");
            //Should add breakroom now
            return;
        }

        //Creating a random room
        int rand = Random.Range(1, allRooms.Length - 1); //ÆNDRE 1 TIL 2 SENERE
        Room newRoom = allRooms[rand];
        lastRoom = currentRoom;
        currentRoom = newRoom;
        GameObject newObj = Instantiate(newRoom.roomPrefab, Vector3.zero, Quaternion.identity);
        newObj.name = "Room - " + index;
        index++;

        //Sets the new entrance of the room
        newRoom.SetEntranceFromChildren();

        //Moves the new room into position
        MoveRoom(newRoom.width, newRoom.height, newRoom.GetEntranceDoor(), currentRooms[currentRooms.Count - 1].GetExitDoor(), newObj);

        //Finds and sets the exit of the new room
        //Door newExit = ChooseRandomDoor(GetAllDoors(newObj), newRoom.GetEntranceDoor());
        Door newExit = ChooseSpecificDoor(GetAllDoors(newObj), "Door (3)");
        newRoom.SetExit(newExit);

        //Adds the room to the current list
        currentRooms.Add(newRoom);

        //Recursive loop until the level is complete
        CreateNewRoom();
    }

    public Door[] GetAllDoors(GameObject objectToCheck)
    {
        Door[] allDoors = objectToCheck.GetComponentsInChildren<Door>();

        return allDoors;
    }

    void MoveRoom(float width, float height, Door lastExitDoor, Door newEntranceDoor, GameObject objectToMove)
    {
        float moveX = newEntranceDoor.transform.position.x - lastExitDoor.transform.position.x;
        moveX += width / 2;

        float moveZ = newEntranceDoor.transform.position.z - lastExitDoor.transform.position.z;
        moveZ += width / 2;

        objectToMove.transform.position = new Vector3(moveX, 0, moveZ);
    }

    Door ChooseRandomDoor(Door[] doors)
    {
        Door door = doors[Random.Range(0, doors.Length)];
        return door;
    }

    Door ChooseRandomDoor(Door[] doors, Door exclude)
    {
        Door door = doors[Random.Range(0, doors.Length)];
        if (door == exclude)
        {
            ChooseRandomDoor(doors, exclude);
        }
        return door;
    }

    Door ChooseSpecificDoor(Door[] doors, string doorName)
    {
        for (int i = 0; i < doors.Length; i++)
        {
            if (doors[i].transform.name == doorName)
            {
                return doors[i];
            }
        }
        return null;
    }
}
