using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public int maxLevelLength;
    public GameObject[] rooms;
    public List<RoomScript> currentRooms;

    bool spawnUp = true;
    bool lastSpawnUp = true;
    GameObject lastRoomObject;
    RoomScript lastRoomScript;

    private void Start()
    {
        CreateLevel();
    }

    void CreateLevel()
    {
        GameObject elevatorRoomObject = Instantiate(rooms[0], Vector3.zero, Quaternion.identity);
        RoomScript elevatorRoomScript = elevatorRoomObject.GetComponent<RoomScript>();
        currentRooms.Add(elevatorRoomScript);

        GameObject firstRoomObject = Instantiate(rooms[Random.Range(2, rooms.Length)], Vector3.zero, Quaternion.identity);
        RoomScript firstRoomScript = firstRoomObject.GetComponent<RoomScript>();
        firstRoomScript.transform.position = elevatorRoomScript.transform.position;

        Vector3 moveAmount = new Vector3(0, 0, elevatorRoomScript.height / 2);
        moveAmount += new Vector3(0, 0, firstRoomScript.height / 2);
        firstRoomScript.transform.position += moveAmount;
        currentRooms.Add(elevatorRoomScript);

        lastRoomObject = firstRoomObject;
        lastRoomScript = firstRoomScript;

        SpawnRoom();
    }

    GameObject newRoomObject;
    RoomScript newRoomScript;

    void SpawnRoom()
    {
        if (currentRooms.Count > maxLevelLength) //Count being over the max lenght due to the fact that the first room is always the starter room
        {
            GameObject finalRoomObject = Instantiate(rooms[1], Vector3.zero, Quaternion.identity);
            RoomScript finalRoomScript = finalRoomObject.GetComponent<RoomScript>();
            finalRoomScript.transform.position = lastRoomScript.transform.position;
            Vector3 moveAmount = new Vector3(0, 0, lastRoomScript.height / 2);
            moveAmount += new Vector3(0, 0, finalRoomScript.height / 2);
            finalRoomScript.transform.position += moveAmount;
            if (finalRoomScript.GetBoundsRaw(finalRoomObject).Intersects(lastRoomScript.GetBoundsRaw(lastRoomObject)))
            {
                Destroy(finalRoomScript.gameObject);
                SpawnRoom();
                return;
            }
            currentRooms.Add(finalRoomScript);

            RemoveExcessDoors();

            return;
        }

        if (newRoomScript)
        {
            newRoomScript.lastSpawnUpValue = lastSpawnUp;
            newRoomScript.spawnUpValue = spawnUp;
        }

        lastSpawnUp = spawnUp;
        spawnUp = (Random.value > 0.5f);

        newRoomObject = rooms[Random.Range(2, rooms.Length)];
        newRoomScript = Instantiate(newRoomObject, Vector3.zero, Quaternion.identity).GetComponent<RoomScript>();

        if (spawnUp)
        {
            newRoomScript.transform.position = lastRoomScript.transform.position;
            Vector3 moveAmount = new Vector3(0, 0, lastRoomScript.height / 2);
            moveAmount += new Vector3(0, 0, newRoomScript.height / 2);
            newRoomScript.transform.position += moveAmount;
        }
        else
        {
            newRoomScript.transform.position = lastRoomScript.transform.position;
            Vector3 moveAmount = new Vector3(lastRoomScript.width / 2, 0, 0);
            moveAmount += new Vector3(newRoomScript.width / 2, 0, 0);
            newRoomScript.transform.position += moveAmount;
        }

        if (newRoomScript.GetBoundsRaw(newRoomObject).Intersects(lastRoomScript.GetBoundsRaw(lastRoomObject)))
        {
            Destroy(newRoomScript.gameObject);
            SpawnRoom();
            return;
        }

        currentRooms.Add(lastRoomScript);
        lastRoomObject = newRoomObject;
        lastRoomScript = newRoomScript;

        SpawnRoom();
    }

    void RemoveExcessDoors()
    {
        for (int i = 1; i < currentRooms.Count - 1; i++)
        {
            if (currentRooms[i + 1].lastSpawnUpValue && currentRooms[i + 1].spawnUpValue)
            {
                if (currentRooms[i].eastDoor)
                {
                    currentRooms[i].DestroyDoor(currentRooms[i].eastDoor);
                }
                if (currentRooms[i].westDoor)
                {
                    currentRooms[i].DestroyDoor(currentRooms[i].westDoor);
                }
            }
            else if (!currentRooms[i + 1].lastSpawnUpValue && currentRooms[i + 1].spawnUpValue)
            {
                if (currentRooms[i].eastDoor)
                {
                    currentRooms[i].DestroyDoor(currentRooms[i].eastDoor);
                }
                if (currentRooms[i].southDoor)
                {
                    currentRooms[i].DestroyDoor(currentRooms[i].southDoor);
                }
            }
            else if (currentRooms[i + 1].lastSpawnUpValue && !currentRooms[i + 1].spawnUpValue)
            {
                if (currentRooms[i].northDoor)
                {
                    currentRooms[i].DestroyDoor(currentRooms[i].northDoor);
                }
                if (currentRooms[i].westDoor)
                {
                    currentRooms[i].DestroyDoor(currentRooms[i].westDoor);
                }
            }
            else if (!currentRooms[i + 1].lastSpawnUpValue && !currentRooms[i + 1].spawnUpValue)
            {
                if (currentRooms[i].northDoor)
                {
                    currentRooms[i].DestroyDoor(currentRooms[i].northDoor);
                }
                if (currentRooms[i].southDoor)
                {
                    currentRooms[i].DestroyDoor(currentRooms[i].southDoor);
                }
            }
        }
    }
}
