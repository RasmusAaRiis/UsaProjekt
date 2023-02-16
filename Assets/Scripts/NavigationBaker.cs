using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class NavigationBaker : MonoBehaviour
{
    public NavMeshSurface[] surfaces;

    public void Build()
    {
        for (int i = 0; i < surfaces.Length; i++) 
        {
            surfaces [i].BuildNavMesh ();    
        }  
    }
}