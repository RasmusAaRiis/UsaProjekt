using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum ActionType
{
    ResetPosition,
    TriggerText
}

[RequireComponent(typeof(BoxCollider))]
public class TutorialTrigger : MonoBehaviour
{
    CharacterController player;
    public Animator anim;
    public ActionType type;
    [TextArea()]
    public string directionsString;
    public float showTime;
    public TextMeshProUGUI directionsText;
    public Transform resetPosition;

    private void Awake()
    {
        player = FindObjectOfType<CharacterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (type == ActionType.ResetPosition && other.transform.CompareTag("Player"))
        {
            player.transform.position = resetPosition.position;
            player.transform.rotation = resetPosition.rotation;
        }
        else if (type == ActionType.TriggerText && other.CompareTag("Player"))
        {
            HideText();
            Invoke("ShowText", 0.15f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (type == ActionType.TriggerText && other.CompareTag("Player"))
        {
            HideText();
        }
    }

    public void ShowText()
    {
        directionsText.SetText(directionsString);
        anim.SetTrigger("Show");
    }

    public void HideText()
    {
        anim.SetTrigger("Hide");
    }
}
