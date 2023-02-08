using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    [HideInInspector] public float width; // The middle is centered, so xWidth 12 = boundry from -6 to 6
    [HideInInspector] public float height; // The same as above

    public bool lastSpawnUpValue;
    public bool spawnUpValue;

    [HideInInspector] public Door northDoor;
    [HideInInspector] public Door southDoor;
    [HideInInspector] public Door eastDoor;
    [HideInInspector] public Door westDoor;

    private void Awake()
    {
        if (GameObject.Find("North Door"))
        {
            northDoor = GameObject.Find("North Door").GetComponent<Door>();
        }
        if (GameObject.Find("South Door"))
        {
            southDoor = GameObject.Find("South Door").GetComponent<Door>();
        }
        if (GameObject.Find("East Door"))
        {
            eastDoor = GameObject.Find("East Door").GetComponent<Door>();
        }
        if (GameObject.Find("West Door"))
        {
            westDoor = GameObject.Find("West Door").GetComponent<Door>();
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
}
