using UnityEngine;

public class LeaveToMenu : MonoBehaviour
{
    [Header(" нопка выхода в меню")]
    public KeyCode leaveKey;
    void Update()
    {
        if(Input.GetKeyDown(leaveKey))
        {
            GoToMenu();
        }
    }

    public void GoToMenu()
    {
        MainCarController.DestroyCarWithMenu();
        Destroy(GameObject.FindWithTag("Player"));
        var MH = GameObject.Find("MyScripts").GetComponent<MainHandler>();
        MH.ReturnToDefault();
    }
}
