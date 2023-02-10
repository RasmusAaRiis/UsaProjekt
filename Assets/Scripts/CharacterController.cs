/* 
 * author : jiankaiwang
 * description : The script provides you with basic operations of first personal control.
 * platform : Unity
 * date : 2017/12
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using FMODUnity;
using FMOD.Studio;

public class CharacterController : MonoBehaviour {

    [Header("Basics")]
    public float speed = 10.0f;
    public float dashForce = 13f;
    public float dashCooldown = 0.1f;
    public float jumpForce = 10;
    bool isGrounded = true;
    bool dashing = false;
    bool canTakeDamage = true;

    [Header("Stats")]
    public int Health = 5;
    public float throwForce = 1f;
    public float damageModifier = 1f;

    Transform heldObject;
    GameObject lookedAtObject;
    Outline selectionOutline = new Outline();

    [Space]
    public TextMeshProUGUI healthText;
    public Rigidbody rb;
    public Transform Hand;
    private float translation;
    private float straffe;

    // Use this for initialization
    void Start () {
        // turn off the cursor
        Cursor.lockState = CursorLockMode.Locked;		
        Physics.gravity = new Vector3(Physics.gravity.x, -9.81f * 2f, Physics.gravity.z);
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

        healthText.text = Health.ToString();

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
        if(Physics.Raycast(Hand.parent.position, Hand.parent.forward, out hit, 4))
        {
            //Ok det her er lidt lorte kode men basically
            //gør det så selection outline virker bedre
            //Bare ikke pild
            //Jeg pillede💀
            if (lookedAtObject != hit.transform.gameObject)
            {
                if (hit.transform.CompareTag("Throwable") ||
                    hit.transform.CompareTag("Door") ||
                    hit.transform.CompareTag("Can") ||
                    hit.transform.CompareTag("Vending"))
                {
                    SetObjectOutline(0);
                }
            }

            lookedAtObject = hit.transform.gameObject;

            if (!lookedAtObject.CompareTag("Throwable") &&
                !lookedAtObject.CompareTag("Door") &&
                !lookedAtObject.CompareTag("Can") &&
                !lookedAtObject.CompareTag("Vending"))
            {
                //Objektet spilleren kigger på er IKKE throwable eller weapon
                SetObjectOutline(0);
                return;
            }

            if(lookedAtObject.transform == heldObject)
            {
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

            if (Input.GetKeyDown(KeyCode.E) && lookedAtObject.CompareTag("Vending"))
            {
                VendingMachine vending = lookedAtObject.GetComponent<VendingMachine>();
                if (vending.usingMachine)
                {
                    return;
                }
                vending.changeView(true);
            }

            if (Input.GetKeyDown(KeyCode.Mouse1) && lookedAtObject.CompareTag("Can"))
            {
                Can can = lookedAtObject.GetComponent<Can>();
                Upgrade(can.canIndex);
                Destroy(can.gameObject);
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

    private void OnCollisionEnter(Collision collision)
    {
        EnemyMovement em = null;
        EnemyBullet eb = null;
        if (collision.transform.TryGetComponent<EnemyMovement>(out em) || collision.transform.TryGetComponent<EnemyBullet>(out eb))
        {

            if (em != null && em.health <= 0)
            {
                return;
            }

            if (!canTakeDamage)
            {
                return;
            }

            Health--;
            Health = Mathf.Max(Health, 0);
            if(Health <= 0)
            {
                Die();
            }
            StartCoroutine(TakeDamageCooldown());
        }
    }

    public void Upgrade(int index)
    {
        //1) Speed
        //2) Melee
        //3) Throwing
        //4) Range

        switch (index)
        {
            case 0:
                speed *= 1.2f;
                dashForce *= 1.2f;
                break;
            case 1:
                damageModifier *= 1.2f;
                break;
            case 2:
                throwForce *= 1.2f;
                break;
            case 3:
                break;
            default:
                break;
        }
    }

    public void Die()
    {
        RuntimeManager.StudioSystem.setParameterByName("Situation", 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator TakeDamageCooldown()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(1);
        canTakeDamage = true;
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
