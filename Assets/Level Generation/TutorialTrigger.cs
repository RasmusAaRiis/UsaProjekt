using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TutorialTrigger : MonoBehaviour
{
    CharacterController player;
    public Transform resetPosition;

    private void Awake()
    {
        player = FindObjectOfType<CharacterController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            player.transform.position = resetPosition.position;
            player.transform.rotation = resetPosition.rotation;
        }
    }
}
