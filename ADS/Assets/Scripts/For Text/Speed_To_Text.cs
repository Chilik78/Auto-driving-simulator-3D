using UnityEngine;
using UnityEngine.UI;
using System;

public class Speed_To_Text : MonoBehaviour
{
    void Update()
    {
        Text textObj = GetComponent<Text>();
        textObj.text = "������� ��������: " + Math.Round(MainCarController.GetSpeed(), 1) + " ��/�";
    }
}
