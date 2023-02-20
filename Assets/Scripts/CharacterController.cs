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
using UnityEngine.UI;
using TMPro;
using FMODUnity;
using FMOD.Studio;

public class CharacterController : MonoBehaviour
{

    [Header("Basics")]
    public float speed = 10.0f;
    public float dashForce = 13f;
    public float dashCooldown = 0.1f;
    public float jumpForce = 10;
    bool isGrounded = true;
    bool dashing = false;
    bool canTakeDamage = true;
    public bool paused = false;

    [Header("Stats")]
    public int Health = 5;
    public int Money = 0;
    public float throwForce = 1f;
    public float damageModifier = 1f;
    public float rangedDamageModifier = 1f;
    public float rangedSpeedModifier = 1f;
    public float knockbackModifier = 1f;
    public int ammoModifier = 1;

    [HideInInspector]
    public Transform heldObject;
    GameObject lookedAtObject;
    Outline selectionOutline = new Outline();

    [Space]
    public GameObject PauseScreen;
    public Image CloudAmmoImage;
    public Image StapleAmmoImage;
    public Image rcButton;
    public Image eButton;
    public Image BlackFadeScreen;
    public Transform target;
    public Animator dashUI;
    public Animator damageAnimator;
    public Animator pickupAnimator;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI ammoText;
    public Rigidbody rb;
    public Transform Hand;
    private float translation;
    private float straffe;
    int currentFloor = 0;

    // Use this for initialization
    void Start()
    {
        PickupText($"Floor {currentFloor + 1}", 0, 0.3f);
        if (PlayerPrefs.HasKey("FOV"))
        {
            GetComponentInChildren<Camera>().fieldOfView = PlayerPrefs.GetInt("FOV");
        }
        // turn off the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Physics.gravity = new Vector3(Physics.gravity.x, -9.81f * 2f, Physics.gravity.z);
    }

    LevelGenerator lg = null;
    void Update()
    {
        if(lg == null)
        {
            if (FindObjectOfType<LevelGenerator>())
            {
                lg = FindObjectOfType<LevelGenerator>();
            }
        } else
        {
            if (currentFloor != lg.levelsCleared)
            {
                currentFloor = lg.levelsCleared;
            }
        }

        #region Basic Controls
        //Basic movements
        translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        straffe = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        rb.MovePosition(rb.position + transform.forward * translation + straffe * transform.right);

        //Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && !dashing && (translation != 0 || straffe != 0))
        {
            StartCoroutine(DashTimer());
        }

        //Står playeren på jorden?
        //Laver en sphere under spilleren og checker hvis den collider med mere end playeren selv
        Collider[] overlaps = Physics.OverlapSphere(transform.position + -Vector3.up * 1, 0.1f);
        if (overlaps.Length >= 2)
        {
            int overlapsL = overlaps.Length;
            for (int i = 0; i < overlaps.Length; i++)
            {
                if(overlaps[i].gameObject.layer == 2)
                {
                    overlapsL--;
                }
            }
            if(overlapsL >= 2)
            {
                isGrounded = true;
            } else
            {
                isGrounded = false;
            }
        }
        else
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

        #region Ammo
        //Ammo display
        Stapler stapler;
        FireExtinguisher fireExtinguisher;
        if (heldObject != null && heldObject.TryGetComponent<Stapler>(out stapler))
        {
            ammoText.transform.parent.gameObject.SetActive(true);
            StapleAmmoImage.gameObject.SetActive(true);
            ammoText.text = stapler.ammo.ToString();
        }
        else if (heldObject != null && heldObject.TryGetComponent<FireExtinguisher>(out fireExtinguisher))
        {
            ammoText.transform.parent.gameObject.SetActive(true);
            CloudAmmoImage.gameObject.SetActive(true);
            ammoText.text = fireExtinguisher.ammo.ToString();
        }
        else
        {
            ammoText.transform.parent.gameObject.SetActive(false);
            StapleAmmoImage.gameObject.SetActive(false);
            CloudAmmoImage.gameObject.SetActive(false);
        }
        #endregion

        #region Pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
            Pause(paused);
        }
        #endregion

        #region Throwable/weapon logic
        if (heldObject != null && Input.GetKeyDown(KeyCode.Mouse0) && !paused)
        {
            Weapon weapon_m;
            if (heldObject.TryGetComponent<Weapon>(out weapon_m))
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

        ShowControls("");

        //Hvis spilleren kigger på et objekt, og de ikke holder noget...
        RaycastHit hit;
        if (Physics.Raycast(Hand.parent.position, Hand.parent.forward, out hit, 4))
        {
            //Ok det her er lidt lorte kode men basically
            //gør det så selection outline virker bedre
            //Bare ikke pild
            //Jeg pillede💀
            //Åh nej, jeg har også pillede nu
            if (lookedAtObject != hit.transform.gameObject)
            {
                if (hit.transform.CompareTag("Throwable") ||
                    hit.transform.CompareTag("Door") ||
                    hit.transform.CompareTag("Can") ||
                    hit.transform.CompareTag("Vending") ||
                    hit.transform.CompareTag("EndLevelTemp") ||
                    hit.transform.CompareTag("Money") ||
                    hit.transform.CompareTag("Water") ||
                    hit.transform.CompareTag("GoToMenu") ||
                    hit.transform.CompareTag("Drawer"))
                {
                    SetObjectOutline(0);
                }
            }

            lookedAtObject = hit.transform.gameObject;

            if (!lookedAtObject.CompareTag("Throwable") &&
                !lookedAtObject.CompareTag("Door") &&
                !lookedAtObject.CompareTag("Can") &&
                !lookedAtObject.CompareTag("Vending") &&
                !lookedAtObject.CompareTag("EndLevelTemp") &&
                !lookedAtObject.CompareTag("Money") &&
                !lookedAtObject.CompareTag("Water") &&
                !lookedAtObject.CompareTag("GoToMenu") &&
                !lookedAtObject.CompareTag("Drawer"))
            {
                //Objektet spilleren kigger på er IKKE throwable eller weapon
                //... eller alt muligt andet nu også lol
                SetObjectOutline(0);
                return;
            }

            if (lookedAtObject.transform == heldObject)
            {
                return;
            }

            //Objektet spilleren kigger på ER throwable eller weapon
            SetObjectOutline(lookedAtObject, 10);
            ShowControls(lookedAtObject.transform.tag);

            if (Input.GetKeyDown(KeyCode.Mouse1) && lookedAtObject.CompareTag("Throwable"))
            {
                //Spileren kigger på et throwable object, de holder ikke noget, og de trykker højreklik
                //Denne kode får spilleren til at samle objektet op
                PickupObject(hit.transform);
            }

            if (Input.GetKeyDown(KeyCode.E) && lookedAtObject.CompareTag("Door"))
            {
                DoorBehavior db;
                if (lookedAtObject.transform.parent.TryGetComponent<DoorBehavior>(out db))
                {
                    db.OpenDoor();
                } else
                {
                    lookedAtObject.GetComponentInParent<Animator>().SetTrigger("Open");
                }
                lookedAtObject.tag = "Untagged";    
                AudioManager.instance.PlayOneShot(FMODEvents.instance.openDoor, this.transform.position);
            }

            //TEMP kode til at lave nye levels
            if (Input.GetKeyDown(KeyCode.E) && lookedAtObject.CompareTag("EndLevelTemp"))
            {
                PickupText($"Floor {currentFloor + 1}", 3, 0.3f);
                GameObject.Find("LevelGenerator").GetComponent<LevelGenerator>().createNewRoom = true;
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

            if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Mouse1)) && lookedAtObject.CompareTag("Can"))
            {
                AudioManager.instance.PlayOneShot(FMODEvents.instance.drink, this.transform.position);
                Can can = lookedAtObject.GetComponent<Can>();
                Upgrade(can.canIndex);
                Destroy(can.gameObject);
            }

            if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Mouse1)) && lookedAtObject.CompareTag("Water"))
            {
                AudioManager.instance.PlayOneShot(FMODEvents.instance.drink, this.transform.position);
                Health = 10;
                PickupText("+Health");
            }

            if (Input.GetKeyDown(KeyCode.E) && lookedAtObject.CompareTag("GoToMenu"))
            {
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene("MainMenu");
            }

            if (Input.GetKeyDown(KeyCode.E) && lookedAtObject.CompareTag("Drawer"))
            {
                lookedAtObject.GetComponent<Rigidbody>().AddForce(lookedAtObject.transform.right * 10, ForceMode.Impulse);
            }

            if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Mouse1)) && lookedAtObject.CompareTag("Money"))
            {
                AudioManager.instance.PlayOneShot(FMODEvents.instance.moneyPickup, this.transform.position);
                Money += 5;
                PickupText("+5 Dollars");
                lookedAtObject.SetActive(false);
                lookedAtObject = null;
            }

        }
        else
        {
            //Spilleren kigger ikke på et objekt
            if (lookedAtObject) { SetObjectOutline(0); }
            lookedAtObject = null;
        }
        #endregion

    }

    public void Pause(bool pause)
    {
        if (pause)
        {
            PauseScreen.SetActive(true);
            BlackFadeScreen.gameObject.SetActive(true);
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            BlackFadeScreen.color = new Color(0, 0, 0, 0.4f);
        } else
        {
            PauseScreen.SetActive(false);
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            BlackFadeScreen.color = new Color(0, 0, 0, 0f);
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

            damageAnimator.SetTrigger("Damage");
            Health--;
            Health = Mathf.Max(Health, 0);
            AudioManager.instance.PlayOneShot(FMODEvents.instance.playerHit, this.transform.position);
            if (Health <= 0)
            {
                Die();
            }
            StartCoroutine(TakeDamageCooldown());
        }
    }

    public void ShowControls(string hoverTag)
    {
        rcButton.enabled = false;
        eButton.enabled = false;
        switch (hoverTag)
        {
            case "Throwable":
                rcButton.enabled = true;
                break;
            case "Door":
                eButton.enabled = true;
                break;
            case "Can":
                eButton.enabled = true;
                break;
            case "Vending":
                eButton.enabled = true;
                break;
            case "EndLevelTemp":
                eButton.enabled = true;
                break;
            case "Money":
                eButton.enabled = true;
                break;
            case "Water":
                eButton.enabled = true;
                break;
            case "GoToMenu":
                eButton.enabled = true;
                break;
            case "Drawer":
                eButton.enabled = true;
                break;
            default:
                break;
        }
    }

    [HideInInspector]
    public int speedAm = 0;
    [HideInInspector]
    public int meleeAm = 0;
    [HideInInspector]
    public int throwingAm = 0;
    [HideInInspector]
    public int rangeAm = 0;
    public void Upgrade(int index)
    {
        //1) Speed
        //2) Melee
        //3) Throwing
        //4) Range

        switch (index)
        {
            case 0:
                PickupText("+Speed");
                speed *= 1.2f;
                dashForce *= 1.2f;
                dashCooldown *= 0.8f;
                break;
            case 1:
                PickupText("+Melee");
                damageModifier *= 1.2f;
                knockbackModifier *= 1.2f;
                break;
            case 2:
                PickupText("+Throwing");
                throwForce *= 1.4f;
                break;
            case 3:
                PickupText("+Range");
                rangedDamageModifier *= 1.2f;
                rangedSpeedModifier *= 1.2f;
                ammoModifier = Mathf.RoundToInt(ammoModifier * 1.5f);
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
        AudioManager.instance.PlayOneShot(FMODEvents.instance.dash, this.transform.position);

        dashUI.speed = 1f / dashCooldown;
        dashUI.SetTrigger("Dash");

        Vector3 originalVelocity = rb.velocity;
        originalVelocity.y = 0;
        rb.velocity = transform.forward * Input.GetAxis("Vertical") * dashForce;
        rb.velocity += transform.right * Input.GetAxis("Horizontal") * dashForce;

        yield return new WaitForSeconds(0.1f);
        rb.velocity = originalVelocity;

        yield return new WaitForSeconds(dashCooldown - 0.1f);
        dashing = false;
        AudioManager.instance.PlayOneShot(FMODEvents.instance.dashCooldown, this.transform.position);
    }

    void PickupObject(Transform objectToPickup)
    {
        heldObject = objectToPickup;

        heldObject.gameObject.layer = 7;
        for (int i = 0; i < heldObject.childCount; i++)
        {
            heldObject.GetChild(i).gameObject.layer = 7;
            for (int j = 0; j < heldObject.GetChild(i).childCount; j++)
            {
                heldObject.GetChild(i).GetChild(j).gameObject.layer = 7;
            }
        }

        heldObject.position = Hand.position;
        heldObject.parent = Hand;
        heldObject.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        heldObject.GetComponent<Rigidbody>().isKinematic = true;

        AudioManager.instance.PlayOneShot(FMODEvents.instance.pickup, this.transform.position);
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

        heldObject.GetComponent<Rigidbody>().velocity = (target.position - Hand.position).normalized * 15 * throwForce;
        if (heldObject.GetComponent<Collider>())
        {
            heldObject.GetComponent<Collider>().enabled = true;
        }
        Collider[] cols = heldObject.GetComponentsInChildren<Collider>();
        for (int i = 0; i < cols.Length; i++)
        {
            cols[i].enabled = true;
        }

        StartCoroutine(layerDelay(heldObject));

        heldObject = null;
    }

    IEnumerator layerDelay(Transform heldObject)
    {
        yield return new WaitForSeconds(0.1f);
        heldObject.gameObject.layer = 0;
        for (int i = 0; i < heldObject.childCount; i++)
        {
            heldObject.GetChild(i).gameObject.layer = 0;
            for (int j = 0; j < heldObject.GetChild(i).childCount; j++)
            {
                heldObject.GetChild(i).GetChild(j).gameObject.layer = 0;
            }
        }
    }

    public void LookAt()
    {
        MouseCamLook MouseCamLook = GetComponentInChildren<MouseCamLook>();
        MouseCamLook.mouseLook = new Vector2(MouseCamLook.mouseLook.x, -70);
    }

    void SetObjectOutline(GameObject Object, float width)
    {
        if (Object == null)
        {
            return;
        }
        //Få fat i objektets outline script
        if (!Object.TryGetComponent<Outline>(out selectionOutline))
        {
            selectionOutline = Object.AddComponent<Outline>();
        }
        if (width <= 0)
        {
            selectionOutline.enabled = false;
            return;
        }
        selectionOutline.enabled = true;
        selectionOutline.OutlineWidth = width;
    }

    void SetObjectOutline(float width)
    {
        if (selectionOutline == null)
        {
            return;
        }
        if (width <= 0)
        {
            selectionOutline.enabled = false;
            return;
        }
        selectionOutline.enabled = true;

        selectionOutline.OutlineWidth = width;
    }

    public void PickupText(string displayText)
    {
        TextMeshProUGUI Text = pickupAnimator.transform.GetComponentInChildren<TextMeshProUGUI>();
        Text.text = displayText;
        pickupAnimator.speed = 1;
        pickupAnimator.SetTrigger("Pickup");
    }

    public void PickupText(string displayText, float delay, float speed)
    {
        StartCoroutine(pickupDelay(displayText, delay, speed));
    }

    IEnumerator pickupDelay(string displayText, float delay, float speed)
    {
        yield return new WaitForSeconds(delay);
        TextMeshProUGUI Text = pickupAnimator.transform.GetComponentInChildren<TextMeshProUGUI>();
        Text.text = displayText;
        pickupAnimator.speed = speed;
        pickupAnimator.SetTrigger("Pickup");
    }

    //Jump sphere visualiser
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + -Vector3.up * 1, 0.1f);
    }


}