using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine : MonoBehaviour
{
    public int Cost = 25;
    [Space]
    public Camera VendingCamera;
    public GameObject[] Cans = new GameObject[3];
    public Transform ejectionPoint;

    [Space]
    public bool usingMachine = false;
    public bool cooldown = false;

    // Update is called once per frame
    void Update()
    {        
        VendingCamera.enabled = usingMachine;

        if (usingMachine && Input.GetKeyDown(KeyCode.E))
        {
            changeView(false);
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
            StartCoroutine(Cooldown());
            usingMachine = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (!cooldown)
        {
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
        Instantiate(Cans[canIndex], ejectionPoint.position, Quaternion.identity);
        cc.Money -= Cost;
        changeView(false);
    }
}
