using UnityEngine;
using UnityEngine.UI;
using System;

public class Speed_To_Text : MonoBehaviour
{
    void Update()
    {
        Text textObj = GetComponent<Text>();
        textObj.text = "Текущая скорость: " + Math.Round(MainCarController.GetSpeed(), 1) + " км/ч";
    }
}
