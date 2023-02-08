using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum Direction
{
    North,
    South,
    East,
    West
}

public class Door : MonoBehaviour
{
    public Direction direction;
    // Animation Functions
    // ...
    // ...
    // ...

    public void DestroyDoor()
    {
        Object wall = Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Models/OfficeWall.fbx", typeof(Object)), transform.position, transform.rotation);
        GameObject wallGameObject = (GameObject)wall;
        wallGameObject.transform.SetParent(transform.parent);

        Destroy(gameObject);
    }
}
