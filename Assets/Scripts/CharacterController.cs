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

    [Header("Basics")]
    public float speed = 10.0f;
    public float dashForce = 13f;
    public float dashCooldown = 0.1f;
    public float jumpForce = 10;
    bool isGrounded = true;
    bool dashing = false;

    [Header("Stats")]
    public float throwForce = 1f;

    Transform heldObject;
    GameObject lookedAtObject;
    Outline selectionOutline = new Outline();

    [Space]
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
        translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        straffe = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        rb.MovePosition(rb.position + transform.forward * translation + straffe * transform.right);

        //Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && !dashing)
        {
            StartCoroutine(DashTimer());
        }

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

        #region Throwable/weapon logic
        if (heldObject != null && Input.GetKeyDown(KeyCode.Mouse0))
        {
            Weapon weapon_m;
            if(heldObject.TryGetComponent<Weapon>(out weapon_m))
            {
                weapon_m.Attack();
            }
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
            //Jeg pillede💀
            if (lookedAtObject != hit.transform.gameObject)
            {
                if (hit.transform.CompareTag("Throwable") || hit.transform.CompareTag("Door"))
                {
                    SetObjectOutline(0);
                }
            }

            lookedAtObject = hit.transform.gameObject;

            if (!hit.transform.CompareTag("Throwable") && !hit.transform.CompareTag("Door"))
            {
                //Objektet spilleren kigger på er IKKE throwable eller weapon
                SetObjectOutline(0);
                return;
            }

            //Objektet spilleren kigger på ER throwable eller weapon
            SetObjectOutline(lookedAtObject, 10);

            if (Input.GetKeyDown(KeyCode.Mouse1) && lookedAtObject.CompareTag("Throwable"))
            {
                //Spileren kigger på et throwable object, de holder ikke noget, og de trykker højreklik
                //Denne kode får spilleren til at samle objektet op
                PickupObject(hit.transform);
            }

            if (Input.GetKeyDown(KeyCode.E) && lookedAtObject.CompareTag("Door"))
            {
                lookedAtObject.GetComponentInParent<Animator>().SetTrigger("Open");
            }

        } else
        {
            //Spilleren kigger ikke på et objekt
            if(lookedAtObject) { SetObjectOutline(0); }
            lookedAtObject = null;
        }
        #endregion

        //Lav om på et tidspunkt btw
        if (Input.GetKeyDown(KeyCode.Escape) && Cursor.lockState == CursorLockMode.Locked) {
            // turn on the cursor
            Cursor.lockState = CursorLockMode.None;
        }
        if (Input.GetKeyDown(KeyCode.Escape) && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    IEnumerator DashTimer()
    {
        dashing = true;
        Vector3 originalVelocity = rb.velocity;
        originalVelocity.y = 0;
        rb.velocity = transform.forward * Input.GetAxis("Vertical") * dashForce;
        rb.velocity += transform.right * Input.GetAxis("Horizontal") * dashForce;
        yield return new WaitForSeconds(0.1f);
        rb.velocity = originalVelocity;
        yield return new WaitForSeconds(dashCooldown - 0.1f);
        dashing = false;
    }

    void PickupObject(Transform objectToPickup)
    {
        heldObject = objectToPickup;
        heldObject.position = Hand.position;
        heldObject.parent = Hand;
        heldObject.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        heldObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    void ThrowObject()
    {
        if (heldObject == null)
        {
            Debug.LogWarning("Cant throw object, since object is null");
            return;
        }

        Weapon weapon;
        if (heldObject.TryGetComponent<Weapon>(out weapon))
        {
            weapon.Throw(throwForce);
        }

        heldObject.position = Hand.position;
        heldObject.parent = null;
        heldObject.GetComponent<Rigidbody>().isKinematic = false;
        heldObject.GetComponent<Rigidbody>().velocity = Hand.parent.forward * 15 * throwForce;
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
