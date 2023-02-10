using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomScript : MonoBehaviour
{
    [HideInInspector] public float width; // The middle is centered, so xWidth 12 = boundry from -6 to 6
    [HideInInspector] public float height; // The same as above

    [HideInInspector] public bool lastSpawnUpValue;
    [HideInInspector] public bool spawnUpValue;

    [HideInInspector] public Transform doorParent;

    [HideInInspector] public List<Transform> enemySpawnPoints;
    public List<GameObject> currentlyAliveEnemies;

    public Door actualDoor;

    [HideInInspector] public Door northDoor;
    [HideInInspector] public Door southDoor;
    [HideInInspector] public Door eastDoor;
    [HideInInspector] public Door westDoor;

    private void Awake()
    {
        Transform[] children = transform.GetComponentsInChildren<Transform>();

        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].tag == "SpawnPoint")
            {
                enemySpawnPoints.Add(children[i]);
            }
        }

        doorParent = transform.Find("Doors");

        if (doorParent.transform.Find("North Door"))
        {
            northDoor = doorParent.transform.Find("North Door").GetComponent<Door>();
        }
        if (doorParent.transform.Find("South Door"))
        {
            southDoor = doorParent.transform.Find("South Door").GetComponent<Door>();
        }
        if (doorParent.transform.Find("East Door"))
        {
            eastDoor = doorParent.transform.Find("East Door").GetComponent<Door>();
        }
        if (doorParent.transform.Find("West Door"))
        {
            westDoor = doorParent.transform.Find("West Door").GetComponent<Door>();
        }   

        Vector3 bound = GetBounds(gameObject);

        width = bound.x;
        height = bound.z;
    }

    public Vector3 GetBounds(GameObject obj)
    {
        var renderers = obj.GetComponentsInChildren<Renderer>();
        Bounds bounds = renderers[0].bounds;
        for (var i = 1; i < renderers.Length; ++i)
            bounds.Encapsulate(renderers[i].bounds);
        return bounds.extents * 2;
    }

    public void DestroyDoor(Door doorToDestroy)
    {
        Object wall = Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Models/OfficeWall.fbx", typeof(Object)), doorToDestroy.transform.position, doorToDestroy.transform.rotation);
        GameObject wallGameObject = (GameObject)wall;
        wallGameObject.transform.SetParent(doorToDestroy.transform.parent);

        Destroy(doorToDestroy.gameObject);
    }
}
