using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Main;

public class scr_Main : MonoBehaviour
{
    [Header("Scripts gameobject")]
    public GameObject MyScripts;

    [Header("Типы объектов")]
    public GameObject[] SumoObjects;

    [Header("Светофор")]
    public GameObject LightObject;

    [Header("timer for motion vizualisation")]
    public float timer = 10.0f;

    [Header("Одна модель для отладки")]
    public GameObject allVeh;
    public bool isDebug;

    private List<GameObject> carsUnity = new List<GameObject>(); //Все машины на карте
    private List<GameObject> lightsUnity = new List<GameObject>(); //Все светофоры на карте
    private GameObject parentCars; //Папка для групировки
    private GameObject parentLights; //Папка для групировки

    //Старт вызывается перед обновлением первого кадра
    void Start()
    {
        Debug.Log("Должно работать.");
        Debug.Log($"Timer: {timer}");
        parentCars = new GameObject("Cars");
        parentLights = new GameObject("Lights");
    }

    //Обновление вызывается один раз за кадр
    void Update()
    {
        string Rx = MyScripts.GetComponent<scr_TCP>().RxMsg(); //Получаем входящую строку из TCP-скрипта
        if (Rx is null) return; //Избегаем null

        List<CarInfo> carsInfo = new List<CarInfo>(); //Список информации о машинах
        List<LightInfo> lightsInfo = new List<LightInfo>(); //Список информации о светофорах
        
        if (Rx.Contains("O1G"))
        {
            string poor = Rx.Substring(3); //Убираем начальную приписку
            poor = poor.Substring(0, poor.IndexOf('&')); //Оставляем текст до &
            carsInfo = RawData.GetCarsFromData(poor); //Извлекаем из строки информацию о машинах
        }
        else //Аналогично
        {
            string poor = Rx.Substring(3);
            poor = poor.Substring(0, poor.IndexOf('&'));
            lightsInfo = RawData.GetLightsFromData(poor);
        }

        if (carsUnity.Count > 0) //Если машины есть
        {
            bool isUpdate = UpdateAllCars(carsInfo); //Обновляем список машин (удаляем вышедшее, добавляем новые)
            if(isUpdate) UpdateAllCarsPositions(carsInfo); //Обновляем позиции машин
        } 
        else CreateAllCars(carsInfo); //Если нет машин, то создаём

        //Та же логика со светофорами
        if(lightsUnity.Count > 0) UpdateLights(lightsInfo);
        else CreateAllLights(lightsInfo);
    }

    /// <summary>
    /// Функция создаёт светофор по её информации
    /// </summary>
    /// <param name="lightInfo"></param>
    public void CreateLight(LightInfo lightInfo)
    {
        var light = Instantiate(LightObject, new Vector3(lightInfo.PosX, 0, lightInfo.PosY), LightObject.transform.rotation);
        light.transform.SetParent(parentLights.transform, false);
        light.name = lightInfo.LaneID;
        lightsUnity.Add(light);
    }

    /// <summary>
    /// Создаёт все светофоры по списку информации
    /// </summary>
    public void CreateAllLights(List<LightInfo> LightsInfo)
    {
        foreach (var lightInfo in LightsInfo)
        {
            CreateLight(lightInfo);
        }
    }

    /// <summary>
    /// Функция создаёт все машины по списку информации
    /// </summary>
    public void CreateAllCars(List<CarInfo> CarsInfo)
    {
        foreach (var carInfo in CarsInfo)
        {
            CreateCar(carInfo);
        }
    }

    /// <summary>
    /// Функция создаёт машину по её информации
    /// </summary>
    /// <param name="carInfo"></param>
    public void CreateCar(CarInfo carInfo)
    {
        GameObject obj;
        obj = isDebug ? allVeh : SumoObjects[carInfo.sizeclass];
        var car = Instantiate(obj, new Vector3(carInfo.posx, 0, carInfo.posy), obj.transform.rotation);
        car.transform.SetParent(parentCars.transform, false);
        car.name = carInfo.vehid;
        carsUnity.Add(car);
    }

    /// <summary>
    /// Функция обновляет - добавляет и удаляет лишниее машины<br></br>
    /// Нужно сравнить с прошлым притоком и добавить новые<br></br>
    /// А потом удалить уже те, которых не пришло
    /// </summary>
    /// <param name="CarsInfo"></param>
    /// <returns></returns>
    public bool UpdateAllCars(List<CarInfo> CarsInfo)
    {
        if(CarsInfo.Count == 0) return false;

        List<string> carNames = carsUnity.Select(x => x.name).ToList();

        foreach(var carInfo in CarsInfo)
        {
            if (!carNames.Contains(carInfo.vehid))
                CreateCar(carInfo);
        }

        carNames = carsUnity.Select(x => x.name).ToList();
        List<string> carsInfoNames = CarsInfo.Select(x => x.vehid).ToList();

        for(int i = 0; i < carNames.Count; i++)
        {
            if (carsInfoNames.Count(x => x == carNames[i]) == 0)
            {
                if (i <= carsUnity.Count)
                {
                    Destroy(carsUnity.ElementAt(i));
                    carsUnity.RemoveAt(i);
                    carNames.RemoveAt(i);
                }
                
                i = 0;
            }
        }

        return true;
    }

    /// <summary>
    /// Функция обновляет все позиции и поведение машины
    /// </summary>
    /// <param name="CarsInfo"></param>
    public void UpdateAllCarsPositions(List<CarInfo> CarsInfo)
    {
        for(int i = 0; i < carsUnity.Count; i++)
        {
            var car = carsUnity[i];
            CarInfo carInfo = CarsInfo[i];
            Vector3 tempPos = car.transform.position;               //Получаем текущую позицию
            tempPos.x = (float)(carInfo.posx); 
            tempPos.z = (float)(carInfo.posy);
            Quaternion tempRot = car.transform.rotation;            //Получаем текущую позицию
            Quaternion rot;
            Vector3 ydir = new Vector3(0, 1, 0);    //y направление вращения
            rot = Quaternion.AngleAxis((carInfo.heading), ydir);
            car.transform.SetPositionAndRotation(tempPos, rot);  //Устанавливаем положение и поворот
            var comp = car.GetComponent<scr_VehicleHandler>();
            if (comp != null)
            {
                comp.CalculateSteering(carInfo.heading, carInfo.speed, timer);
                comp.BrakeLightSwitch(carInfo.brakestate);
            }
        }
        Debug.Log("Обновлены все позиции машин");
    }

    /// <summary>
    /// Обновляет состояния всех светофоров по списку информации
    /// </summary>
    /// <param name="LightsInfo"></param>
    public void UpdateLights(List<LightInfo> LightsInfo)
    {
        var phases = LightsInfo.Select(x => x.CurrentPhase).ToList();
        var LaneIDs = LightsInfo.Select(x => x.LaneID).ToList();

        for(int i = 0; i < lightsUnity.Count; i++)
        {
            var light = lightsUnity[i];
            int index = LaneIDs.IndexOf(light.name);

            if (index < 0 || index > (phases.Count - 1)) continue;

            ChangeColorLight(light, phases[index]);
        }
    }
    
    /// <summary>
    /// Меняет светофора по фазе
    /// </summary>
    /// <param name="light"></param>
    /// <param name="phase"></param>
    public void ChangeColorLight(GameObject light, int phase)
    {
        var lightRenderer = light.GetComponent<Renderer>();
        Color color = Color.white;

        switch(phase)
        {
            case 1: color = Color.green; break;
            case 2: color = Color.yellow; break;
            case 3: color = Color.red; break;
        }

        lightRenderer.material.SetColor("_Color", color);
    }
}

