using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VendingMachine : MonoBehaviour
{
    public int Cost = 25;
    [Space]
    public TextMeshProUGUI MoneyText;
    public Camera VendingCamera;
    public GameObject[] Cans = new GameObject[3];
    public Transform ejectionPoint;
    CharacterController cc = null;

    [Space]
    public bool usingMachine = false;
    public bool cooldown = false;

    // Update is called once per frame
    void Update()
    {   
        if(cc == null)
        {
            cc = FindObjectOfType<CharacterController>();
        } else
        {
            MoneyText.text = $"Money: {cc.Money}";
        }

        VendingCamera.enabled = usingMachine;

        if (usingMachine && Input.GetKeyDown(KeyCode.E))
        {
            cc.BlackFadeScreen.gameObject.SetActive(false);
            changeView(false);
        }

        if(cc.speedAm >= 5)
        {
            transform.GetChild(1).GetChild(2).GetChild(0).gameObject.SetActive(false);
        }
        if (cc.meleeAm >= 5)
        {
            transform.GetChild(1).GetChild(2).GetChild(1).gameObject.SetActive(false);
        }
        if (cc.throwingAm >= 5)
        {
            transform.GetChild(1).GetChild(2).GetChild(2).gameObject.SetActive(false);
        }
        if (cc.rangeAm >= 5)
        {
            transform.GetChild(1).GetChild(2).GetChild(3).gameObject.SetActive(false);
        }
    }

    IEnumerator Cooldown()
    {
        cooldown = true;
        yield return new WaitForSeconds(0.1f);
        Time.timeScale = 0;
        cooldown = false;
    }

    public void changeView(bool locked)
    {
        if (locked)
        {
            if(usingMachine)
            {
                cc.BlackFadeScreen.gameObject.SetActive(false);
                changeView(false);
                return;
            }
            Cursor.visible = true;
            cc.BlackFadeScreen.gameObject.SetActive(false);
            StartCoroutine(Cooldown());
            usingMachine = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (!cooldown)
        {
            cc.BlackFadeScreen.gameObject.SetActive(true);
            usingMachine = false;
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void Eject(int canIndex)
    {
        CharacterController cc = FindObjectOfType<CharacterController>();
        if (cc.Money < Cost)
        {
            //Ikke nok penge
            return;
        }
        cc.BlackFadeScreen.gameObject.SetActive(true);
        var obj = Instantiate(Cans[canIndex], ejectionPoint.position, Quaternion.identity);
        cc.LookAt();
        cc.Money -= Cost;
        changeView(false);

        switch (canIndex)
        {
            case 0:
                cc.speedAm++;
                break;
            case 1:
                cc.meleeAm++;
                break;
            case 2:
                cc.throwingAm++;
                break;
            case 3:
                cc.rangeAm++;
                break;
            default:
                break;
        }
    }
}
