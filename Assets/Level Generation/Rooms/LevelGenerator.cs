using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    public int maxLevelLength;
    public GameObject[] rooms;
    public List<GameObject> currentRooms;


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
        Instantiate(playerPrefab, Vector3.zero + Vector3.up, Quaternion.identity);

        GameObject elevatorRoomObject = Instantiate(rooms[0], Vector3.zero, Quaternion.identity);
        RoomScript elevatorRoomScript = elevatorRoomObject.GetComponent<RoomScript>();
        currentRooms.Add(elevatorRoomObject);

        GameObject firstRoomObject = Instantiate(rooms[Random.Range(2, rooms.Length)], Vector3.zero, Quaternion.identity);
        RoomScript firstRoomScript = firstRoomObject.GetComponent<RoomScript>();
        firstRoomScript.transform.position = elevatorRoomScript.transform.position;

        Vector3 moveAmount = new Vector3(0, 0, elevatorRoomScript.height / 2);
        moveAmount += new Vector3(0, 0, firstRoomScript.height / 2);
        firstRoomScript.transform.position += moveAmount;
        currentRooms.Add(firstRoomObject);

        firstRoomScript.spawnUpValue = true;

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
            finalRoomScript.spawnUpValue = true;
            if (finalRoomScript.GetBoundsRaw(finalRoomObject).Intersects(lastRoomScript.GetBoundsRaw(lastRoomObject)))
            {
                Destroy(finalRoomScript.gameObject);
                SpawnRoom();
                return;
            }
            currentRooms.Add(newRoomScript.gameObject);
            currentRooms.Add(finalRoomObject);

            RemoveExcessDoors();

            return;
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

        newRoomScript.spawnUpValue = spawnUp;
        newRoomScript.lastSpawnUpValue = lastRoomScript.spawnUpValue;

        //newRoomScript.gameObject.SetActive(false);

        currentRooms.Add(lastRoomScript.gameObject);
        lastRoomObject = newRoomObject;
        lastRoomScript = newRoomScript;
        SpawnRoom();
    }

    void RemoveExcessDoors()
    {
        for (int i = 2; i < currentRooms.Count - 1; i++)
        {
            if (currentRooms[i].GetComponent<RoomScript>() && currentRooms[i].transform.Find("Doors"))
            {
                if (currentRooms[i + 1].GetComponent<RoomScript>().spawnUpValue && currentRooms[i].GetComponent<RoomScript>().spawnUpValue)
                {
                    if (currentRooms[i].GetComponent<RoomScript>().eastDoor)
                    {
                        currentRooms[i].GetComponent<RoomScript>().DestroyDoor(currentRooms[i].GetComponent<RoomScript>().eastDoor);
                    }
                    if (currentRooms[i].GetComponent<RoomScript>().westDoor)
                    {
                        currentRooms[i].GetComponent<RoomScript>().DestroyDoor(currentRooms[i].GetComponent<RoomScript>().westDoor);
                    }
                }
                if (!currentRooms[i + 1].GetComponent<RoomScript>().spawnUpValue && currentRooms[i].GetComponent<RoomScript>().spawnUpValue)
                {
                    if (currentRooms[i].GetComponent<RoomScript>().northDoor)
                    {
                        currentRooms[i].GetComponent<RoomScript>().DestroyDoor(currentRooms[i].GetComponent<RoomScript>().northDoor);
                    }
                    if (currentRooms[i].GetComponent<RoomScript>().westDoor)
                    {
                        currentRooms[i].GetComponent<RoomScript>().DestroyDoor(currentRooms[i].GetComponent<RoomScript>().westDoor);
                    }
                }
                if (currentRooms[i + 1].GetComponent<RoomScript>().spawnUpValue && !currentRooms[i].GetComponent<RoomScript>().spawnUpValue)
                {
                    if (currentRooms[i].GetComponent<RoomScript>().southDoor)
                    {
                        currentRooms[i].GetComponent<RoomScript>().DestroyDoor(currentRooms[i].GetComponent<RoomScript>().southDoor);
                    }
                    if (currentRooms[i].GetComponent<RoomScript>().eastDoor)
                    {
                        currentRooms[i].GetComponent<RoomScript>().DestroyDoor(currentRooms[i].GetComponent<RoomScript>().eastDoor);
                    }
                }
                if (!currentRooms[i + 1].GetComponent<RoomScript>().spawnUpValue && !currentRooms[i].GetComponent<RoomScript>().spawnUpValue)
                {
                    if (currentRooms[i].GetComponent<RoomScript>().southDoor)
                    {
                        currentRooms[i].GetComponent<RoomScript>().DestroyDoor(currentRooms[i].GetComponent<RoomScript>().southDoor);
                    }
                    if (currentRooms[i].GetComponent<RoomScript>().northDoor)
                    {
                        currentRooms[i].GetComponent<RoomScript>().DestroyDoor(currentRooms[i].GetComponent<RoomScript>().northDoor);
                    }
                }
            }
        }

        if (currentRooms[currentRooms.Count - 1].GetComponent<RoomScript>().spawnUpValue)
        {
            if (currentRooms[currentRooms.Count - 1].GetComponent<RoomScript>().eastDoor)
            {
                currentRooms[currentRooms.Count - 1].GetComponent<RoomScript>().DestroyDoor(currentRooms[currentRooms.Count - 1].GetComponent<RoomScript>().eastDoor);
            }
            if (currentRooms[currentRooms.Count - 1].GetComponent<RoomScript>().westDoor)
            {
                currentRooms[currentRooms.Count - 1].GetComponent<RoomScript>().DestroyDoor(currentRooms[currentRooms.Count - 1].GetComponent<RoomScript>().westDoor);
            }
        }
        else
        {
            if (currentRooms[currentRooms.Count - 1].GetComponent<RoomScript>().eastDoor)
            {
                currentRooms[currentRooms.Count - 1].GetComponent<RoomScript>().DestroyDoor(currentRooms[currentRooms.Count - 1].GetComponent<RoomScript>().eastDoor);
            }
            if (currentRooms[currentRooms.Count - 1].GetComponent<RoomScript>().southDoor)
            {
                currentRooms[currentRooms.Count - 1].GetComponent<RoomScript>().DestroyDoor(currentRooms[currentRooms.Count - 1].GetComponent<RoomScript>().southDoor);
            }
        }
    }
}
