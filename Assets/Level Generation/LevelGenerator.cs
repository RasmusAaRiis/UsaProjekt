using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.AI.Navigation;

public class LevelGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject doorPrefab;
    [SerializeField] GameObject starterWeaponPrefab;
    public GameObject officeWallPrefab;
    [SerializeField] GameObject[] enemyPrefabs;

    [Header("Variables")]
    public int maxLevelLength;
    public RoomScript currentActiveRoom;
    public NavigationBaker navBaker;
    public bool createNewRoom = false;
    public int minEnemyCount = 1;
    public int maxEnemyCount = 1;
    public int minEnemyCountIncrease = 1;
    public int maxEnemyCountIncrease = 1;
    public float fadeSpeed;

    [Header("Object Tracking")]
    public GameObject[] rooms;
    public List<GameObject> currentRooms;
    public List<GameObject> doors;
    public List<GameObject> deadEnemies;

    [Space()]
    [Header("Stat Tracking")]
    [Header("Data virker ikke, hvis scene reloades", order = 0)]
    [Space()]
    [Header("Levels")]
    public int levelsCleared = 0; //Done
    public float fastestLevelClearTime = 0; //Done
    public float slowestLevelClearTime = 0; //Done
    public float averageLevelClearTime = 0; //Done

    [Space()]
    [Header("Rooms")]
    public int roomsCleared = 0; //Done
    public float fastestRoomClearTime = 0; //Done
    public float slowestRoomClearTime = 0; //Done
    public float averageRoomClearTime = 0; //Done

    [Space()]
    [Header("Enemies")]
    public int enemiesKilled = 0; //Done
    public int chairsKilled = 0; //Done
    public int cabinetsKilled = 0; //Done

    [Space()]
    [Header("Other")]
    public int objectsThrown = 0;
    public int deaths = 0;

    bool spawnUp = true;
    RoomScript lastRoomScript;
    bool test = false;

    private void Start()
    {
        AudioManager.instance.sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
        AudioManager.instance.musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        CreateLevel();
    }

    void CreateLevel()
    {
        for (int i = 0; i < currentRooms.Count; i++)
        {
            Destroy(currentRooms[i]);
        }
        currentRooms.Clear();

        for (int i = 0; i < doors.Count; i++)
        {
            Destroy(doors[i]);
        }
        doors.Clear();

        for (int i = 0; i < deadEnemies.Count; i++)
        {
            if (deadEnemies[i].transform != player.GetComponent<CharacterController>().heldObject)
            {
                Destroy(deadEnemies[i]);
            }
        }
        deadEnemies.Clear();
        navBaker.surfaces.Clear();

        GameObject[] throwableObjects = GameObject.FindGameObjectsWithTag("Throwable");

        for (int i = 0; i < throwableObjects.Length; i++)
        {
            if (throwableObjects[i].transform != player.GetComponent<CharacterController>().heldObject)
            {
                Destroy(throwableObjects[i]);
            }
        }

        if (!player)
        {
            player = Instantiate(playerPrefab, Vector3.zero + Vector3.up, Quaternion.identity);
        }
        else
        {
            player.transform.position = Vector3.zero + Vector3.up;
        }

        GameObject elevatorRoomObject = Instantiate(rooms[0], Vector3.zero, Quaternion.identity);
        RoomScript elevatorRoomScript = elevatorRoomObject.GetComponent<RoomScript>();
        currentRooms.Add(elevatorRoomObject);
        elevatorRoomScript.rawBounds = GetBoundsRaw(elevatorRoomScript.gameObject);
        elevatorRoomScript.rawBounds.Expand(-1f);

        if (levelsCleared == 0)
        {
            elevatorRoomScript.southDoor.ActivateDoor();
        }

        GameObject firstRoomObject = Instantiate(rooms[Random.Range(2, rooms.Length)], Vector3.zero, Quaternion.identity);
        RoomScript firstRoomScript = firstRoomObject.GetComponent<RoomScript>();
        firstRoomScript.transform.position = elevatorRoomScript.transform.position;

        currentActiveRoom = firstRoomScript;

        Vector3 moveAmount = new Vector3(0, 0, elevatorRoomScript.height / 2);
        moveAmount += new Vector3(0, 0, firstRoomScript.height / 2);
        firstRoomScript.transform.position += moveAmount;

        firstRoomScript.rawBounds = GetBoundsRaw(firstRoomScript.gameObject);
        firstRoomScript.rawBounds.Expand(-1f);

        firstRoomScript.spawnUpValue = true;

        lastRoomScript = firstRoomScript;

        SpawnRoom();
    }

    GameObject newRoomObject;
    RoomScript newRoomScript;

    bool intersects = false;

    float startLevelTime;
    float startRoomTime;

    float totalLevelClearTime;
    float totalRoomClearTime;

    IEnumerator GameLoop()
    {
        startLevelTime = Time.time;
        startRoomTime = Time.time;

        yield return new WaitForEndOfFrame();
        BakeNavigation();
        SpawnEnemies();

        while (currentRooms[currentRooms.Count - 2].GetComponent<RoomScript>().currentlyAliveEnemies.Count > 0)
        {
            yield return new WaitForSeconds(0);
            intersects = false;

            if (GetBoundsRawSingle(player).Intersects(currentActiveRoom.rawBounds))
            {
                intersects = true;
            }
            else
            {
                AudioManager.instance.SetParameter("Situation", 0);
            }

            for (int i = 0; i < currentActiveRoom.currentlyAliveEnemies.Count; i++)
            {
                if (currentActiveRoom.currentlyAliveEnemies[i].GetComponent<EnemyMovement>().chaseTarget)
                {
                    AudioManager.instance.SetParameter("Situation", 1);
                }

                if (currentActiveRoom.currentlyAliveEnemies[i].GetComponent<EnemyMovement>().health <= 0)
                {
                    if (currentActiveRoom.currentlyAliveEnemies[i].GetComponent<EnemyShoot>())
                    {
                        cabinetsKilled++;
                    }
                    else
                    {
                        chairsKilled++;
                    }

                    enemiesKilled++;

                    deadEnemies.Add(currentActiveRoom.currentlyAliveEnemies[i]);
                    currentActiveRoom.currentlyAliveEnemies.Remove(currentActiveRoom.currentlyAliveEnemies[i]);
                }
                else if (intersects)
                {
                    currentActiveRoom.currentlyAliveEnemies[i].GetComponent<EnemyMovement>().chaseTarget = true;
                }
            }

            if (currentActiveRoom.currentlyAliveEnemies.Count <= 0)
            {
                AudioManager.instance.PlayOneShot(FMODEvents.instance.victory, player.transform.position);
                roomsCleared++;
                float newRoomClearTime = Time.time - startRoomTime;
                totalRoomClearTime += newRoomClearTime;
                averageRoomClearTime = totalRoomClearTime / roomsCleared;

                if (newRoomClearTime > slowestRoomClearTime)
                {
                    slowestRoomClearTime = newRoomClearTime;
                }

                if (fastestRoomClearTime == 0 || newRoomClearTime < fastestRoomClearTime)
                {
                    fastestRoomClearTime = newRoomClearTime;
                }

                startRoomTime = Time.time;

                currentActiveRoom.actualDoor.ActivateDoor();
                currentActiveRoom = currentRooms[currentRooms.IndexOf(currentActiveRoom.gameObject) + 1].GetComponent<RoomScript>();
            }
        }

        AudioManager.instance.SetParameter("Situation", 0);

        levelsCleared++;
        float newLevelClearTime = Time.time - startLevelTime;
        totalLevelClearTime += newLevelClearTime;
        averageLevelClearTime = totalLevelClearTime / levelsCleared;

        if (newLevelClearTime > slowestLevelClearTime)
        {
            slowestLevelClearTime = newLevelClearTime;
        }

        if (fastestLevelClearTime == 0 || newLevelClearTime < fastestLevelClearTime)
        {
            fastestLevelClearTime = newLevelClearTime;
        }
        test = false;
        while (!createNewRoom)
        {
            Bounds breakRoomBounds = GetBoundsRaw(currentRooms[currentRooms.Count - 1]);
            breakRoomBounds.Expand(-2f);

            if (!test && GetBoundsRawSingle(player).Intersects(breakRoomBounds))
            {
                test = true;
                AudioManager.instance.SetParameter("ElevatorLoad", 1f);
                AudioManager.instance.SetParameter("Elevator", 0);
                AudioManager.instance.SetParameter("Situation", 2);
            }

            yield return new WaitForSeconds(0);
        }

        createNewRoom = false;

        FadeToBlack(0.3f);
        FadeFromBlack(1f, 0.3f);
        yield return new WaitForSeconds(1f);
        minEnemyCount += minEnemyCountIncrease;
        maxEnemyCount += maxEnemyCountIncrease;
        Debug.Log("Fade");
        CreateLevel();

        AudioManager.instance.SetParameter("ElevatorLoad", 1f);
        AudioManager.instance.SetParameter("Elevator", 1);
        AudioManager.instance.SetParameter("Situation", 0);

        yield return new WaitForSeconds(3);

        AudioManager.instance.SetParameter("ElevatorLoad", 0f);

        currentRooms[0].GetComponent<RoomScript>().southDoor.ActivateDoor();
        AudioManager.instance.PlayOneShot(FMODEvents.instance.ding, this.transform.position);
    }

    public void FadeToBlack(float duration)
    {
        StartCoroutine(FadeToBlackOverTime(duration));
    }

    IEnumerator FadeToBlackOverTime(float duration)
    {
        Image fadeImage = player.GetComponent<CharacterController>().BlackFadeScreen;

        while (fadeImage.color.a < 1f)
        {
            Debug.Log(fadeImage.color.a + 1 / 25f);
            yield return new WaitForSeconds(duration / 25f);
            fadeImage.color = new Color(0f, 0f, 0f, fadeImage.color.a + 1 / 25f);
        }

        fadeImage.color = new Color(0f, 0f, 0f, 1f);
    }

    public void FadeFromBlack(float waitTime, float duration)
    {
        StartCoroutine(FadeFromBlackOverTime(waitTime, duration));
    }

    IEnumerator FadeFromBlackOverTime(float waitTime, float duration)
    {
        yield return new WaitForSeconds(waitTime);

        Image fadeImage = player.GetComponent<CharacterController>().BlackFadeScreen;

        while (fadeImage.color.a > 0f)
        {
            yield return new WaitForSeconds(duration / 25f);
            fadeImage.color = new Color(0f, 0f, 0f, fadeImage.color.a - 1 / 25f);
        }

        fadeImage.color = new Color(0f, 0f, 0f, 0f);
    }

    void SpawnRoom()
    {
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

            SetActualDoors();
            SpawnDoor(breakRoomScript.southDoor.gameObject);
            RemoveExcessDoors();

            doors[doors.Count - 1].transform.SetParent(currentRooms[currentRooms.Count - 1].GetComponent<RoomScript>().actualDoor.transform);
            currentRooms[0].GetComponent<RoomScript>().actualDoor.ActivateDoor();

            for (int i = 0; i < currentRooms.Count; i++)
            {
                ROSP[] rosps = currentRooms[i].GetComponentsInChildren<ROSP>();

                for (int ii = 0; ii < rosps.Length; ii++)
                {
                    rosps[ii].SpawnObject();
                }
            }

            StartCoroutine("GameLoop");
            return;
        }

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

        newRoomScript.rawBounds = GetBoundsRaw(newRoomScript.gameObject);
        newRoomScript.rawBounds.Expand(-1f);

        for (int i = 0; i < currentRooms.Count; i++)
        {
            if (newRoomScript.rawBounds.Intersects(currentRooms[i].GetComponent<RoomScript>().rawBounds))
            {
                Destroy(newRoomScript.gameObject);
                SpawnRoom();
                return;
            }
        }

        newRoomScript.spawnUpValue = spawnUp;
        newRoomScript.lastSpawnUpValue = lastRoomScript.spawnUpValue;

        currentRooms.Add(lastRoomScript.gameObject);
        lastRoomScript = newRoomScript;
        SpawnRoom();
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
            int spawnAmout = Random.Range(minEnemyCount, currentRooms[i].GetComponent<RoomScript>().enemySpawnPoints.Count);

            if (minEnemyCount > 3)
            {
                minEnemyCount = 3;
            }

            spawnAmout = Mathf.Clamp(spawnAmout, minEnemyCount, maxEnemyCount);

            for (int ii = 0; ii < spawnAmout; ii++)
            {
                int spawnPointIndex = Random.Range(0, currentRooms[i].GetComponent<RoomScript>().enemySpawnPoints.Count);
                Transform spawnPosition = currentRooms[i].GetComponent<RoomScript>().enemySpawnPoints[spawnPointIndex];
                GameObject newEnemy;
                if (levelsCleared == 0)
                {
                    newEnemy = Instantiate(enemyPrefabs[0], spawnPosition.position, Quaternion.identity);
                }
                else
                {
                    newEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], spawnPosition.position, Quaternion.identity);
                }
                newEnemy.GetComponent<EnemyMovement>().chaseTarget = false;
                newEnemy.GetComponent<EnemyMovement>().target = player.transform;
                currentRooms[i].GetComponent<RoomScript>().enemySpawnPoints.RemoveAt(spawnPointIndex);
                currentRooms[i].GetComponent<RoomScript>().currentlyAliveEnemies.Add(newEnemy);
            }
        }
    }

    void MoveDoorParents()
    {
        List<Door> acutalDoors = new List<Door>();

        for (int i = 1; i < currentRooms.Count; i++)
        {
            acutalDoors.Add(currentRooms[i].GetComponent<RoomScript>().actualDoor);
        }

        for (int i = 1; i < currentRooms.Count - 1; i++)
        {
            currentRooms[i].GetComponent<RoomScript>().actualDoor = acutalDoors[i];
        }

        for (int i = 0; i < doors.Count; i++)
        {
            doors[i].transform.SetParent(currentRooms[i].GetComponent<RoomScript>().actualDoor.transform);
        }
    }

    void BakeNavigation()
    {
        for (int i = 0; i < currentRooms.Count; i++)
        {
            if (currentRooms[i].GetComponentInChildren<NavMeshSurface>())
            {
                navBaker.surfaces.Add(currentRooms[i].GetComponentInChildren<NavMeshSurface>());
            }
        }

        navBaker.Build();
    }

    void SetActualDoors()
    {
        for (int i = 0; i < currentRooms.Count - 1; i++)
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
        MoveDoorParents();
    }

    void RemoveExcessDoors()
    {
        for (int i = 1; i < currentRooms.Count - 1; i++)
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

    public Bounds GetBoundsRaw(GameObject obj)
    {
        var renderers = obj.GetComponentsInChildren<Renderer>();
        Bounds bounds = renderers[0].bounds;
        for (var i = 1; i < renderers.Length; ++i)
            bounds.Encapsulate(renderers[i].bounds);
        return bounds;
    }

    public Bounds GetBoundsRawSingle(GameObject obj)
    {
        Bounds bounds = obj.GetComponent<Renderer>().bounds;
        return bounds;
    }

    private void OnDrawGizmos()
    {
        if (player)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(GetBoundsRawSingle(player).center, GetBoundsRawSingle(player).extents * 2);
        }

        for (int i = 0; i < currentRooms.Count; i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(currentRooms[i].GetComponent<RoomScript>().rawBounds.center, currentRooms[i].GetComponent<RoomScript>().rawBounds.extents * 2);
        }

        if (currentRooms.Count > 0)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(currentRooms[currentRooms.Count - 1].GetComponent<RoomScript>().rawBounds.center, currentRooms[currentRooms.Count - 1].GetComponent<RoomScript>().rawBounds.extents * 2);
        }
    }
}
