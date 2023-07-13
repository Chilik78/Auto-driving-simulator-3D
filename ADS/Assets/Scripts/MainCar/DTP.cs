using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DTP : MonoBehaviour
{
    public GameObject scores;


    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Other cars")
        {

            /*Component[] components = scores.GetComponents(typeof(Component));
            foreach (Component component in components)
            {
                Debug.Log(component.ToString());
            }*/


            scores.GetComponent<TextMeshProUGUI>().text = Convert.ToString(Convert.ToInt32(scores.GetComponent<TextMeshProUGUI>().text) - 10);
        }
        
    }
}
