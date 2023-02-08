using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomScript : MonoBehaviour
{
    [HideInInspector] public float width; // The middle is centered, so xWidth 12 = boundry from -6 to 6
    [HideInInspector] public float height; // The same as above

    public bool lastSpawnUpValue;
    public bool spawnUpValue;

    private Transform doorParent;

    public Door northDoor;
    public Door southDoor;
    public Door eastDoor;
    public Door westDoor;

    private void Awake()
    {
        if (transform.Find("Doors"))
        {
            doorParent = transform.Find("Doors");

            if (doorParent.Find("North Door"))
            {
                northDoor = doorParent.Find("North Door").GetComponent<Door>();
            }
            if (doorParent.Find("South Door"))
            {
                southDoor = doorParent.Find("South Door").GetComponent<Door>();
            }
            if (doorParent.Find("East Door"))
            {
                eastDoor = doorParent.Find("East Door").GetComponent<Door>();
            }
            if (doorParent.Find("West Door"))
            {
                westDoor = doorParent.Find("West Door").GetComponent<Door>();
            }
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

    public Bounds GetBoundsRaw(GameObject obj)
    {
        var renderers = obj.GetComponentsInChildren<Renderer>();
        Bounds bounds = renderers[0].bounds;
        for (var i = 1; i < renderers.Length; ++i)
            bounds.Encapsulate(renderers[i].bounds);
        return bounds;
    }

    public void DestroyDoor(Door doorToDestroy)
    {
        Object wall = Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Models/OfficeWall.fbx", typeof(Object)), doorToDestroy.transform.position, doorToDestroy.transform.rotation);
        GameObject wallGameObject = (GameObject)wall;
        wallGameObject.transform.SetParent(doorToDestroy.transform.parent);

        Destroy(doorToDestroy.gameObject);
    }
}
