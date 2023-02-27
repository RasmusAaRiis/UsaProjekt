using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class QuantityCulling : MonoBehaviour
{
    [SerializeField] private bool calculate;
    
    public int maxObjects;

    [SerializeField] private List<DistanceCulling> cullings;

    private void Update()
    {
        cullings = SortByDistanceLinq(cullings);

        for (int i = 0; i < cullings.Count; i++)
        {
            cullings[i].active = i < maxObjects;
        }
    }

    private void OnValidate()
    {
        if (calculate)
        {
            RecalculateList();
            calculate = false;
        }
    }

    private List<DistanceCulling> SortByDistanceLinq(List<DistanceCulling> dc)
    {
        if (dc.Count < 0)
        {
            throw new ArgumentNullException();
        }

        var orderByDescending = dc.OrderBy(x => x.distance);
        return orderByDescending.ToList();
    }
    
    public void RecalculateList()
    {
        Debug.Log("List calculated");
        cullings.Clear();
        DistanceCulling[] dcList = FindObjectsOfType<DistanceCulling>();
        for (int i = 0; i < dcList.Length; i++)
        {
            DistanceCulling dc = dcList[i];
            if (dc.cullingType == CullingType.GameObjectChilds)
            {
                cullings.Add(dc);
            }
        }
        
    }
}
