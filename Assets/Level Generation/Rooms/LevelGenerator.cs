using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject doorPrefab;

    public int maxLevelLength;
    public GameObject[] rooms;
    public List<GameObject> currentRooms;
    public List<GameObject> doors;


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

        StartCoroutine("SpawnRoom");
    }

    GameObject newRoomObject;
    RoomScript newRoomScript;

    float waitTime = 0.01f;

    IEnumerator SpawnRoom()
    {
        yield return new WaitForSeconds(waitTime);
        if (currentRooms.Count > maxLevelLength) //Count being over the max lenght due to the fact that the first room is always the starter room
        {
            GameObject finalRoomObject = Instantiate(rooms[1], Vector3.zero, Quaternion.identity);
            RoomScript finalRoomScript = finalRoomObject.GetComponent<RoomScript>();
            finalRoomScript.transform.position = lastRoomScript.transform.position;
            Vector3 moveAmount = new Vector3(0, 0, lastRoomScript.height / 2);
            moveAmount += new Vector3(0, 0, finalRoomScript.height / 2);
            finalRoomScript.transform.position += moveAmount;
            finalRoomScript.spawnUpValue = true;
            if (GetBoundsRaw(finalRoomObject).Intersects(GetBoundsRaw(lastRoomObject)))
            {
                Destroy(finalRoomScript.gameObject);
                StartCoroutine("SpawnRoom");
                yield return new WaitForSeconds(9999999999);
            }
            currentRooms.Add(newRoomScript.gameObject);
            currentRooms.Add(finalRoomObject);

            RemoveExcessDoors();
            Debug.Log("Level Complete");
            yield return new WaitForSeconds(9999999999);
        }

        lastSpawnUp = spawnUp;
        spawnUp = (Random.value > 0.5f);

        newRoomObject = rooms[Random.Range(2, rooms.Length)];
        newRoomScript = Instantiate(newRoomObject, Vector3.zero, Quaternion.identity).GetComponent<RoomScript>();
        yield return new WaitForSeconds(waitTime);
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
        yield return new WaitForSeconds(waitTime);
        if (GetBoundsRaw(newRoomScript.gameObject).Intersects(GetBoundsRaw(lastRoomObject)))
        {
            Destroy(newRoomScript.gameObject);
            StartCoroutine("SpawnRoom");
            yield return new WaitForSeconds(9999999999);
        }
        yield return new WaitForSeconds(waitTime);
        newRoomScript.spawnUpValue = spawnUp;
        newRoomScript.lastSpawnUpValue = lastRoomScript.spawnUpValue;

        //newRoomScript.gameObject.SetActive(false);

        currentRooms.Add(lastRoomScript.gameObject);
        lastRoomObject = newRoomObject;
        lastRoomScript = newRoomScript;
        StartCoroutine("SpawnRoom");
    }

    void SpawnDoor(Transform doorScript)
    {
        Vector3 rot = doorScript.rotation.eulerAngles + doorPrefab.transform.rotation.eulerAngles;

        GameObject newDoor = Instantiate(doorPrefab, doorScript.position, Quaternion.Euler(rot));
        doors.Add(newDoor);
    }

    void RemoveExcessDoors()
    {
        for (int i = 2; i < currentRooms.Count - 1; i++)
        {
            if (currentRooms[i].GetComponent<RoomScript>() && currentRooms[i].transform.Find("Doors"))
            {
                if (currentRooms[i + 1].GetComponent<RoomScript>().spawnUpValue && currentRooms[i].GetComponent<RoomScript>().spawnUpValue)
                {
                    if (currentRooms[i].GetComponent<RoomScript>().southDoor)
                    {
                        SpawnDoor(currentRooms[i].GetComponent<RoomScript>().southDoor.transform);
                    }

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
                    if (currentRooms[i].GetComponent<RoomScript>().southDoor)
                    {
                        SpawnDoor(currentRooms[i].GetComponent<RoomScript>().southDoor.transform);
                    }

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
                    if (currentRooms[i].GetComponent<RoomScript>().westDoor)
                    {
                        SpawnDoor(currentRooms[i].GetComponent<RoomScript>().westDoor.transform);
                    }

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
                    if (currentRooms[i].GetComponent<RoomScript>().westDoor)
                    {
                        SpawnDoor(currentRooms[i].GetComponent<RoomScript>().westDoor.transform);
                    }

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

    public Bounds GetBoundsRaw(GameObject obj)
    {
        var renderers = obj.GetComponentsInChildren<Renderer>();
        Bounds bounds = renderers[0].bounds;
        for (var i = 1; i < renderers.Length; ++i)
            bounds.Encapsulate(renderers[i].bounds);
        return bounds;
    }
}
