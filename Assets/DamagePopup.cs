using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public Animator anim;
    
    public void DisplayDamage(string displayText)
    {
        anim.GetComponent<TextMeshProUGUI>().text = displayText;
        anim.SetTrigger("Display");
    }
}
