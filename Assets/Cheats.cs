using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheats : MonoBehaviour
{
    CharacterController cc;
    char input = 'A';
    bool search = false;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey)
        {
            
            if(e.keyCode.ToString() == "None")
            {
                return;
            }
            if(e.keyCode.ToString().Length > 1 && e.keyCode.ToString()[1] == 'l')
            {
                input = e.keyCode.ToString()[5];
            } else
            {
                input = e.keyCode.ToString()[0];
            }
        }
    }

    private void Update()
    {
        if(input == 'M')
        {
            input = 'W';
            search = true;
        }

        if (search)
        {
            if(input == 'W')
            {
                return;
            }
            search = false;
            switch (input)
            {
                case '1':
                    cc.PickupText("Health Cheat Activated", 0, 0.5f);
                    cc.Health = 99;
                    break;
                case '2':
                    cc.PickupText("Damage Cheat Activated", 0, 0.5f);
                    cc.damageModifier = 99;
                    cc.rangedDamageModifier = 99;
                    break;
                case '3':
                    cc.PickupText("Money Cheat Activated", 0, 0.5f);
                    cc.Money = 9999;
                    break;
                case '4':
                    if(cc.gameObject.GetComponent<Collider>().enabled == false)
                    {
                        cc.PickupText("Noclip Cheat Deactivated", 0, 0.5f);
                        cc.gameObject.GetComponent<Collider>().enabled = true;
                        cc.gameObject.GetComponent<Rigidbody>().useGravity = true;
                    } else
                    {
                        cc.PickupText("Noclip Cheat Activated", 0, 0.5f);
                        cc.gameObject.GetComponent<Collider>().enabled = false;
                        cc.gameObject.GetComponent<Rigidbody>().useGravity = false;
                    }
                    break;
                case 'W':
                    search = true;
                    break;
                default:
                    search = false;
                    break;
            }
        }
    }
}
