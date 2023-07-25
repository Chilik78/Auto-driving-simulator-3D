using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffLights : MonoBehaviour
{
    private GameObject Lights;
    private Light a;
    // Start is called before the first frame update
    void Start()
    {
        Lights = gameObject;
        a = Lights.GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.L) && a.enabled == false)
        {
            a.enabled = true;
        }
        else if(Input.GetKeyDown(KeyCode.L) && a.enabled == true)
        {
            a.enabled = false;
        }

    }
}
