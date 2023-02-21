using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ROSP_Object
{
    public Transform parent;
    public GameObject obj;
    [Range(0f, 1f)]
    public float spawnChance;
}

public class ROSP : MonoBehaviour
{
    public ROSP_Object[] rospObjects;
    GameObject obj;
    public GameObject SpawnObject()
    {
        float chance = Random.Range(0f, 1f);
        ROSP_Object currenObjectToSpawn = rospObjects[0];
        for (int i = 0; i < rospObjects.Length; i++)
        {
            if (rospObjects[i].spawnChance >= chance && rospObjects[i].spawnChance < currenObjectToSpawn.spawnChance)
            {
                currenObjectToSpawn = rospObjects[i];
            }
        }

        if (currenObjectToSpawn.obj)
        {
            obj = Instantiate(currenObjectToSpawn.obj, transform.position, currenObjectToSpawn.parent.rotation);
            obj.transform.SetParent(gameObject.transform);
            return obj;
        }

        return null;
    }
}
