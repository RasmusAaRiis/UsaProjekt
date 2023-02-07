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
    public float jumpForce = 10;
    bool isGrounded = true;
    public Rigidbody rb;
    private float translation;
    private float straffe;

    // Use this for initialization
    void Start () {
        // turn off the cursor
        Cursor.lockState = CursorLockMode.Locked;		
        Physics.gravity = new Vector3(Physics.gravity.x, Physics.gravity.y * 2f, Physics.gravity.z);
	}

    void Update () {
        //Basic movements
        translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        straffe = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
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

        //???
        if (Input.GetKeyDown(KeyCode.Escape)) {
            // turn on the cursor
            Cursor.lockState = CursorLockMode.None;
        }
    }

    //Jump sphere visualiser
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + -Vector3.up * 1, 0.1f);
    }

}
