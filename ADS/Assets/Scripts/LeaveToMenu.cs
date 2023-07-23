using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveToMenu : MonoBehaviour
{
    [Header(" нопка выхода в меню")]
    public KeyCode leaveKey;
    void Update()
    {
        if(Input.GetKeyDown(leaveKey))
        {
            MainCarController.DestroyCarWithMenu();
            Destroy(GameObject.FindWithTag("Player"));
        }
    }
}
