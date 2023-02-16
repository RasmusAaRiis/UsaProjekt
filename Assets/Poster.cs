using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poster : MonoBehaviour
{
    public List<Texture> textures = new List<Texture>();
    public MeshRenderer mr;
    void Start()
    {
        mr.material.SetTexture("_MainTex", textures[Random.Range(0, textures.Count)]);
    }
}
