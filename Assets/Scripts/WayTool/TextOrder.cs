using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextOrder : MonoBehaviour
{
    public TextMeshPro text;
    
    public void SetText(string txt, bool isShow)
    {
        text.text = txt;
        text.enabled = isShow;
    }

    void LateUpdate()
    {
        // Xoay text để luôn nhìn về camera
        transform.forward = Camera.main.transform.forward;
    }
}
