/* 
 * author : jiankaiwang
 * description : The script provides you with basic operations of first personal control.
 * platform : Unity
 * date : 2017/12
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {

    public float speed = 10.0f;
    public float sprintSpeed = 13f;
    public float jumpForce = 10;
    bool isGrounded = true;

    Transform heldObject;
    GameObject lookedAtObject;
    Outline selectionOutline = new Outline();

    public Rigidbody rb;
    public Transform Hand;
    private float translation;
    private float straffe;

    // Use this for initialization
    void Start () {
        // turn off the cursor
        Cursor.lockState = CursorLockMode.Locked;		
        Physics.gravity = new Vector3(Physics.gravity.x, Physics.gravity.y * 2f, Physics.gravity.z);
	}

    void Update () {
        #region Basic Controls
        //Basic movements
        translation = Input.GetAxis("Vertical") * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : speed) * Time.deltaTime;
        straffe = Input.GetAxis("Horizontal") * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : speed) * Time.deltaTime;
        rb.MovePosition(rb.position + transform.forward * translation + straffe * transform.right);

        //Står playeren på jorden?
        //Laver en sphere under spilleren og checker hvis den collider med mere end playeren selv
        if(Physics.OverlapSphere(transform.position + -Vector3.up * 1, 0.1f).Length >= 2)
        {
            isGrounded = true;
        } else
        {
            isGrounded = false;
        }

        //Jump controls
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        #endregion

        if(heldObject != null && Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!heldObject.CompareTag("Weapon"))
            {
                return;
            }

            heldObject.GetComponent<Weapon>().Attack();
        }

        //Kast throwable item
        if (heldObject != null && Input.GetKeyDown(KeyCode.Mouse1))
        {
            ThrowObject();
            return;
        }

        //Hvis spilleren kigger på et objekt, og de ikke holder noget...
        RaycastHit hit;
        if(Physics.Raycast(Hand.parent.position, Hand.parent.forward, out hit, 4) &&
           heldObject == null)
        {
            //Ok det her er lidt lorte kode men basically
            //gør det så selection outline virker bedre
            //Bare ikke pild
            if (lookedAtObject != hit.transform.gameObject)
            {
                if (hit.transform.CompareTag("Throwable") || hit.transform.CompareTag("Weapon"))
                {
                    SetObjectOutline(0);
                }
            }

            lookedAtObject = hit.transform.gameObject;

            if (!hit.transform.CompareTag("Throwable") && !hit.transform.CompareTag("Weapon"))
            {
                //Objektet spilleren kigger på er IKKE throwable eller weapon
                SetObjectOutline(0);
                return;
            }

            //Objektet spilleren kigger på ER throwable eller weapon
            SetObjectOutline(lookedAtObject, 10);

            if (!Input.GetKeyDown(KeyCode.Mouse1))
            {
                return;
            }

            //Spileren kigger på et throwable object, de holder ikke noget, og de trykker højreklik
            //Denne kode får spilleren til at samle objektet op
            PickupObject(hit.transform);

        } else
        {
            //Spilleren kigger ikke på et objekt
            if(lookedAtObject) { SetObjectOutline(0); }
            lookedAtObject = null;
        }

        //Lav om på et tidspunkt btw
        if (Input.GetKeyDown(KeyCode.Escape)) {
            // turn on the cursor
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void PickupObject(Transform objectToPickup)
    {
        heldObject = objectToPickup;
        heldObject.position = Hand.position;
        heldObject.parent = Hand;
        heldObject.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        heldObject.GetComponent<Rigidbody>().isKinematic = true;
        //heldObject.GetComponent<Collider>().enabled = false;
    }

    void ThrowObject()
    {
        if(heldObject == null)
        {
            Debug.LogWarning("Cant throw object, since object is null");
            return;
        }

        heldObject.position = Hand.position;
        heldObject.parent = null;
        heldObject.GetComponent<Rigidbody>().isKinematic = false;
        heldObject.GetComponent<Rigidbody>().velocity = Hand.parent.forward * 15;
        heldObject.GetComponent<Collider>().enabled = true;
        heldObject = null;
    }

    void SetObjectOutline(GameObject Object, float width)
    {
        //Få fat i objektets outline script
        if (!Object.TryGetComponent<Outline>(out selectionOutline))
        {
            selectionOutline = Object.AddComponent<Outline>();
        }
        selectionOutline.OutlineWidth = width;
    }

    void SetObjectOutline(float width)
    {
        selectionOutline.OutlineWidth = width;
    }

    //Jump sphere visualiser
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + -Vector3.up * 1, 0.1f);
    }

}
