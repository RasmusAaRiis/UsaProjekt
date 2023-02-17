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
        foreach (var t in surfaces)
        {
            t.BuildNavMesh ();
        }
    }
}