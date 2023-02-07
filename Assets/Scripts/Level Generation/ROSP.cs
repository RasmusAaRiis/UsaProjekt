using UnityEngine;
using UnityEditor;

public class ROSP : MonoBehaviour
{
    //
    //ROSP = Random Object Spawn Position || ROSP* = det samme, men med mulighed for at være en dør
    //
    public GameObject[] ROSP_Objects;
    private Object doorObject;
    private Vector3 ROSP_Position;
    public bool doorPossibility;

    private void Awake()
    {
        ROSP_Position = transform.position;
        SetRandomObject();
        doorObject = AssetDatabase.LoadAssetAtPath("Assets/Scripts/Level Generation/Cube.prefab", typeof(GameObject));
    }

    public void SetAsDoor()
    {
        Instantiate(doorObject, ROSP_Position, Quaternion.identity);
    }

    private void SetRandomObject()
    {
        int rand = Random.Range(0, ROSP_Objects.Length);
        Instantiate(ROSP_Objects[rand], ROSP_Position, Quaternion.identity);
    }
}
