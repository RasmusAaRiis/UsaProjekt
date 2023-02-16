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

    public void SpawnObject()
    {
        float chance = Random.Range(0f, 1f);
        ROSP_Object currenObjectToSpawn = rospObjects[0];
        Debug.Log("Chance to spawn: " + chance);
        for (int i = 0; i < rospObjects.Length; i++)
        {
            if (rospObjects[i].spawnChance >= chance && rospObjects[i].spawnChance < currenObjectToSpawn.spawnChance)
            {
                currenObjectToSpawn = rospObjects[i];
            }
        }

        if (currenObjectToSpawn.obj)
        {
            GameObject newObj = Instantiate(currenObjectToSpawn.obj, transform.position, currenObjectToSpawn.parent.rotation);
            newObj.transform.SetParent(gameObject.transform);
            /*
            Vector3 rot = transform.parent.rotation.eulerAngles + currenObjectToSpawn.obj.transform.rotation.eulerAngles + currenObjectToSpawn.rotationOffset;
            GameObject newObj = Instantiate(currenObjectToSpawn.obj, transform.position, Quaternion.Euler(rot));
            newObj.transform.position += currenObjectToSpawn.positionOffset;*/
        }
    }
}
