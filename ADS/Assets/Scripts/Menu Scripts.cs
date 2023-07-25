using UnityEngine;
using UnityEngine.UI;

public class MenuScripts : MonoBehaviour
{
    [Header("Выпадающий список транспорта")]
    public Dropdown variableTransport;// Выпадающий список транспорта
    [Header("Камера машины")]
    public GameObject camCar;// Камера машины
    [Header("Первый транспорт")]
    public GameObject firstTransport;// Первый транспорт
    [Header("Второй транспорт")]
    public GameObject secondTransport;// Второй транспорт
    [Header("Третий транспорт")]
    public GameObject thirdTransport;// Третий транспорт

    [Header("Координаты спавна нового транспорта")]
    public float posX, posY, posZ;// Координаты спавна нового транспорта

    private static Transform transformCar;// Ссылка на новый транспорта
    private static GameObject camMenu;// Камера меню
    private void Start()
    {
        camMenu = gameObject;// Указываем ссылку на камеру меню
    }

    /// <summary>
    /// Функция создания нового транспорта
    /// </summary>
    public void CreateCar()
    {
        string car = "";

        switch (variableTransport.value)// Спавним транспорт относительно выпадающего списка
        {
            case 0: transformCar = Instantiate(firstTransport, new Vector3(posX, posY, posZ), new Quaternion(0,0,0,0)).transform; transformCar.name = "Player"; car = "veh0";  break;
            case 1: transformCar = Instantiate(secondTransport, new Vector3(posX, posY, posZ), new Quaternion(0, 0, 0, 0)).transform; transformCar.name = "Player"; car = "bus0"; break;
            case 2: transformCar = Instantiate(thirdTransport, new Vector3(posX, posY, posZ), new Quaternion(0, 0, 0, 0)).transform; transformCar.name = "Player"; car = "truck0"; break;
        }
        var MH = GameObject.Find("MyScripts").GetComponent<MainHandler>();
        MH.StartSim(car);

        camCar.SetActive(true);// Включаем камеру машины
        camMenu.SetActive(false);// Выключаем камеру меню
    }

    /// <summary>
    /// Функция выдачи ссылки на новый транспорт
    /// </summary>
    /// <returns></returns>
    public static Transform GetTransformCar()
    {
        return transformCar;
    }

    /// <summary>
    /// Функция включения камеры меню 
    /// </summary>
    public static void SwitchOnCameraMenu()
    {
        camMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }
}
