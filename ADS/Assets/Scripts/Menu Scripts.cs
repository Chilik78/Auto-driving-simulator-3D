using UnityEngine;
using UnityEngine.UI;

public class MenuScripts : MonoBehaviour
{
    [Header("���������� ������ ����������")]
    public Dropdown variableTransport;// ���������� ������ ����������
    [Header("������ ������")]
    public GameObject camCar;// ������ ������
    [Header("������ ���������")]
    public GameObject firstTransport;// ������ ���������
    [Header("������ ���������")]
    public GameObject secondTransport;// ������ ���������
    [Header("������ ���������")]
    public GameObject thirdTransport;// ������ ���������

    [Header("���������� ������ ������ ����������")]
    public float posX, posY, posZ;// ���������� ������ ������ ����������

    private static Transform transformCar;// ������ �� ����� ����������
    private static GameObject camMenu;// ������ ����
    private void Start()
    {
        camMenu = gameObject;// ��������� ������ �� ������ ����
    }

    /// <summary>
    /// ������� �������� ������ ����������
    /// </summary>
    public void CreateCar()
    {
        string car = "";

        switch (variableTransport.value)// ������� ��������� ������������ ����������� ������
        {
            case 0: transformCar = Instantiate(firstTransport, new Vector3(posX, posY, posZ), new Quaternion(0,0,0,0)).transform; transformCar.name = "Player"; car = "veh0";  break;
            case 1: transformCar = Instantiate(secondTransport, new Vector3(posX, posY, posZ), new Quaternion(0, 0, 0, 0)).transform; transformCar.name = "Player"; car = "bus0"; break;
            case 2: transformCar = Instantiate(thirdTransport, new Vector3(posX, posY, posZ), new Quaternion(0, 0, 0, 0)).transform; transformCar.name = "Player"; car = "truck0"; break;
        }
        var MH = GameObject.Find("MyScripts").GetComponent<MainHandler>();
        MH.StartSim(car);

        camCar.SetActive(true);// �������� ������ ������
        camMenu.SetActive(false);// ��������� ������ ����
    }

    /// <summary>
    /// ������� ������ ������ �� ����� ���������
    /// </summary>
    /// <returns></returns>
    public static Transform GetTransformCar()
    {
        return transformCar;
    }

    /// <summary>
    /// ������� ��������� ������ ���� 
    /// </summary>
    public static void SwitchOnCameraMenu()
    {
        camMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }
}
