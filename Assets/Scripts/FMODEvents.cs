using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FMODEvents : MonoBehaviour
{

    [field: Header("Music")]
    [field: SerializeField] public EventReference music { get; private set; }

    [field: Header("Effects")]
    [field: SerializeField] public EventReference ding { get; private set; }
    [field: SerializeField] public EventReference staples { get; private set; }
    [field: SerializeField] public EventReference pickup { get; private set; }
    [field: SerializeField] public EventReference genericHit { get; private set; }
    [field: SerializeField] public EventReference playerHit { get; private set; }
    [field: SerializeField] public EventReference enemyDead { get; private set; }
    [field: SerializeField] public EventReference openDoor { get; private set; }
    [field: SerializeField] public EventReference dash { get; private set; }
    [field: SerializeField] public EventReference moneyPickup { get; private set; }
    [field: SerializeField] public EventReference dashCooldown { get; private set; }
    [field: SerializeField] public EventReference drink { get; private set; }
    [field: SerializeField] public EventReference victory { get; private set; }
    [field: SerializeField] public EventReference spray { get; private set; }
    [field: SerializeField] public EventReference buttonButton { get; private set; }


    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Hov du har mere end 1 fmod events >:(");
        }
        instance = this;
    }
}
