using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine : MonoBehaviour
{
    public Camera VendingCamera;
    public GameObject[] Cans = new GameObject[3];
    public Transform ejectionPoint;

    bool usingMachine = false;

    // Update is called once per frame
    void Update()
    {
        foreach (var item in Physics.OverlapSphere(transform.position, 2))
        {
            if (item.transform.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
            {
                changeView();
            }
        }
        
        VendingCamera.enabled = usingMachine;
    }

    void changeView()
    {
        usingMachine = !usingMachine;
        if (usingMachine)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void Eject(int canIndex)
    {
        Instantiate(Cans[canIndex], ejectionPoint.position, Quaternion.identity);
        changeView();
    }
}
