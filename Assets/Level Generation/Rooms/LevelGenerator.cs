using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject doorPrefab;
    [SerializeField] GameObject enemyPrefab;

    public int maxLevelLength;
    public GameObject[] rooms;
    public List<GameObject> currentRooms;
    public List<GameObject> doors;

    public RoomScript currentActiveRoom;

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
        player = Instantiate(playerPrefab, Vector3.zero + Vector3.up, Quaternion.identity);

        GameObject elevatorRoomObject = Instantiate(rooms[0], Vector3.zero, Quaternion.identity);
        RoomScript elevatorRoomScript = elevatorRoomObject.GetComponent<RoomScript>();
        currentRooms.Add(elevatorRoomObject);
        elevatorRoomScript.rawBounds = GetBoundsRaw(elevatorRoomScript.gameObject);

        GameObject firstRoomObject = Instantiate(rooms[Random.Range(2, rooms.Length)], Vector3.zero, Quaternion.identity);
        RoomScript firstRoomScript = firstRoomObject.GetComponent<RoomScript>();
        firstRoomScript.transform.position = elevatorRoomScript.transform.position;

        firstRoomScript.rawBounds = GetBoundsRaw(firstRoomScript.gameObject);
        currentActiveRoom = firstRoomScript;

        Vector3 moveAmount = new Vector3(0, 0, elevatorRoomScript.height / 2);
        moveAmount += new Vector3(0, 0, firstRoomScript.height / 2);
        firstRoomScript.transform.position += moveAmount;

        firstRoomScript.spawnUpValue = true;

        lastRoomObject = firstRoomObject;
        lastRoomScript = firstRoomScript;

        StartCoroutine("SpawnRoom");
    }

    GameObject newRoomObject;
    RoomScript newRoomScript;

    float waitTime = 0f;

    IEnumerator SpawnRoom()
    {
        yield return new WaitForSeconds(waitTime);
        if (currentRooms.Count >= maxLevelLength) //Count being over the max lenght due to the fact that the first room is always the starter room
        {
            GameObject breakRoomObject = Instantiate(rooms[1], Vector3.zero, Quaternion.identity);
            RoomScript breakRoomScript = breakRoomObject.GetComponent<RoomScript>();
            breakRoomScript.transform.position = lastRoomScript.transform.position;
            Vector3 moveAmount = new Vector3(0, 0, lastRoomScript.height / 2);
            moveAmount += new Vector3(0, 0, breakRoomScript.height / 2);
            breakRoomScript.transform.position += moveAmount;
            breakRoomScript.spawnUpValue = true;
            
            currentRooms.Add(newRoomScript.gameObject);
            currentRooms.Add(breakRoomObject);

            SetAllDoors();
            SpawnDoor(breakRoomScript.southDoor.gameObject);
            RemoveExcessDoors();
            SpawnEnemies();

            doors[0].transform.SetParent(currentRooms[0].GetComponent<RoomScript>().actualDoor.transform);
            doors[doors.Count - 1].transform.SetParent(currentRooms[currentRooms.Count - 1].GetComponent<RoomScript>().actualDoor.transform);

            currentRooms[0].GetComponent<RoomScript>().actualDoor.ActivateDoor();

            yield return new WaitForSeconds(waitTime);
            while (currentRooms[currentRooms.Count - 2].GetComponent<RoomScript>().currentlyAliveEnemies.Count > 0)
            {
                for (int i = 0; i < currentActiveRoom.currentlyAliveEnemies.Count; i++)
                {
                    if (currentActiveRoom.currentlyAliveEnemies[i].GetComponent<EnemyMovement>().health <= 0)
                    {
                        currentActiveRoom.currentlyAliveEnemies.Remove(currentActiveRoom.currentlyAliveEnemies[i]);
                    }
                    else
                    {
                        currentActiveRoom.currentlyAliveEnemies[i].GetComponent<EnemyMovement>().chaseTarget = true;
                    }
                }

                yield return new WaitForSeconds(waitTime);

                int count = 0;

                for (int i = 1; i < currentRooms.IndexOf(currentActiveRoom.gameObject); i++)
                {
                    if (GetBoundsRaw(player).Intersects(currentRooms[i].GetComponent<RoomScript>().rawBounds))
                    {
                        count++;
                    }
                }

                if (count > 0)
                {
                    RuntimeManager.StudioSystem.setParameterByName("Situation", 1);
                }

                if (currentActiveRoom.currentlyAliveEnemies.Count <= 0)
                {
                    yield return new WaitForSeconds(waitTime);
                    currentActiveRoom.actualDoor.ActivateDoor();
                    currentActiveRoom = currentRooms[currentRooms.IndexOf(currentActiveRoom.gameObject) + 1].GetComponent<RoomScript>();
                }
            }

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

        newRoomScript.rawBounds = GetBoundsRaw(newRoomScript.gameObject);

        for (int i = 0; i < currentRooms.Count; i++)
        {
            if (newRoomScript.rawBounds.Intersects(currentRooms[i].GetComponent<RoomScript>().rawBounds))
            {
                Destroy(newRoomScript.gameObject);
                StartCoroutine("SpawnRoom");
                yield return new WaitForSeconds(9999999999);
            }
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

    GameObject SpawnDoor(GameObject door)
    {
        Vector3 rot = door.transform.rotation.eulerAngles + doorPrefab.transform.rotation.eulerAngles;

        GameObject newDoor = Instantiate(doorPrefab, door.transform.position, Quaternion.Euler(rot));
        doors.Add(newDoor);
        return newDoor;
    }

    void SpawnEnemies()
    {
        for (int i = 1; i < currentRooms.Count - 1; i++)
        {
            int spawnAmout = Random.Range(1, currentRooms[i].GetComponent<RoomScript>().enemySpawnPoints.Count);
            spawnAmout = Mathf.Clamp(spawnAmout, 1, 7);

            for (int ii = 0; ii < spawnAmout; ii++)
            {
                Transform spawnPosition = currentRooms[i].GetComponent<RoomScript>().enemySpawnPoints[Random.Range(0, currentRooms[i].GetComponent<RoomScript>().enemySpawnPoints.Count)];
                GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition.position, Quaternion.identity);
                newEnemy.GetComponent<EnemyMovement>().chaseTarget = false;
                currentRooms[i].GetComponent<RoomScript>().currentlyAliveEnemies.Add(newEnemy);
            }
        }
    }

    void SetAllDoors()
    {
        for (int i = 1; i < currentRooms.Count - 1; i++)
        {
            if (currentRooms[i].GetComponent<RoomScript>() && currentRooms[i].transform.Find("Doors"))
            {
                if (currentRooms[i + 1].GetComponent<RoomScript>().spawnUpValue && currentRooms[i].GetComponent<RoomScript>().spawnUpValue)
                {
                    if (currentRooms[i].GetComponent<RoomScript>().southDoor)
                    {
                        SpawnDoor(currentRooms[i].GetComponent<RoomScript>().southDoor.gameObject);
                        currentRooms[i].GetComponent<RoomScript>().actualDoor = currentRooms[i].GetComponent<RoomScript>().southDoor;
                    }
                }
                if (!currentRooms[i + 1].GetComponent<RoomScript>().spawnUpValue && currentRooms[i].GetComponent<RoomScript>().spawnUpValue)
                {
                    if (currentRooms[i].GetComponent<RoomScript>().southDoor)
                    {
                        SpawnDoor(currentRooms[i].GetComponent<RoomScript>().southDoor.gameObject);
                        currentRooms[i].GetComponent<RoomScript>().actualDoor = currentRooms[i].GetComponent<RoomScript>().southDoor;
                    }
                }
                if (currentRooms[i + 1].GetComponent<RoomScript>().spawnUpValue && !currentRooms[i].GetComponent<RoomScript>().spawnUpValue)
                {
                    if (currentRooms[i].GetComponent<RoomScript>().westDoor)
                    {
                        SpawnDoor(currentRooms[i].GetComponent<RoomScript>().westDoor.gameObject);
                        currentRooms[i].GetComponent<RoomScript>().actualDoor = currentRooms[i].GetComponent<RoomScript>().westDoor;
                    }
                }
                if (!currentRooms[i + 1].GetComponent<RoomScript>().spawnUpValue && !currentRooms[i].GetComponent<RoomScript>().spawnUpValue)
                {
                    if (currentRooms[i].GetComponent<RoomScript>().westDoor)
                    {
                        SpawnDoor(currentRooms[i].GetComponent<RoomScript>().westDoor.gameObject);
                        currentRooms[i].GetComponent<RoomScript>().actualDoor = currentRooms[i].GetComponent<RoomScript>().westDoor;
                    }
                }
            }
        }

        currentRooms[0].GetComponent<RoomScript>().actualDoor = currentRooms[1].GetComponent<RoomScript>().southDoor;
        currentRooms[currentRooms.Count - 1].GetComponent<RoomScript>().actualDoor = currentRooms[currentRooms.Count - 1].GetComponent<RoomScript>().southDoor;
    }

    void RemoveExcessDoors()
    {
        for (int i = 1; i < currentRooms.Count - 1; i++)
        {
            if (currentRooms[i].GetComponent<RoomScript>() && currentRooms[i].transform.Find("Doors"))
            {
                if (currentRooms[i + 1].GetComponent<RoomScript>().spawnUpValue && currentRooms[i].GetComponent<RoomScript>().spawnUpValue)
                {
                    if (doors.Count > i + 1 && currentRooms[i].GetComponent<RoomScript>().southDoor)
                    {
                        currentRooms[i].GetComponent<RoomScript>().actualDoor = currentRooms[i + 1].GetComponent<RoomScript>().actualDoor;
                        doors[i + 1].transform.SetParent(currentRooms[i].GetComponent<RoomScript>().actualDoor.transform);
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
                        currentRooms[i].GetComponent<RoomScript>().actualDoor = currentRooms[i + 1].GetComponent<RoomScript>().actualDoor;
                        doors[i].transform.SetParent(currentRooms[i].GetComponent<RoomScript>().actualDoor.transform);
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
                        currentRooms[i].GetComponent<RoomScript>().actualDoor = currentRooms[i + 1].GetComponent<RoomScript>().actualDoor;
                        doors[i].transform.SetParent(currentRooms[i].GetComponent<RoomScript>().actualDoor.transform);
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
                        currentRooms[i].GetComponent<RoomScript>().actualDoor = currentRooms[i + 1].GetComponent<RoomScript>().actualDoor;
                        doors[i].transform.SetParent(currentRooms[i].GetComponent<RoomScript>().actualDoor.transform);
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
