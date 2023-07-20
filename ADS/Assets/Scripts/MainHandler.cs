using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Main;
using System;
using System.Threading;

public class MainHandler : MonoBehaviour
{
    [Header("Общая папка")]
    public GameObject MyScripts;

    [Header("Настройки симуляции")]
    public string SimPath;
    public float SimLength;

    [Header("Настройка игрока")]
    public GameObject player;
    public string CarId;
    public List<string> CarIds;
    public Camera playerCamera;

    [Header("Типы объектов")]
    public GameObject[] SumoObjects;

    [Header("Светофор")]
    public GameObject LightObject;
    public float createPosY;

    [Header("Одна модель для отладки")]
    public GameObject allVeh;
    public bool isDebug;

    private List<GameObject> carsUnity = new List<GameObject>(); //Все машины на карте
    private List<GameObject> lightsUnity = new List<GameObject>(); //Все светофоры на карте
    private GameObject parentCars; //Папка для групировки
    private GameObject parentLights; //Папка для групировки

    private SUMO sumo; //Объект симуляции

    //Если хочется передвинуть машину в симуляции в какую-либо точку
    //И использовать sumo.TeleportSimBodyToStartPosition(id, position, rotation)
    [Header("Точка спавна машины в симуляции")]
    public Vector3 position;
    public Quaternion rotation;

    //Старт вызывается перед обновлением первого кадра
    void Start()
    {
        parentCars = new GameObject("Cars");
        parentLights = new GameObject("Lights");
        sumo = new SUMO(4042, SimPath, SimLength);
        sumo.StartSimulation(CarId, CarIds);
        if (player == null)
            player = GameObject.Find("player");

        sumo.TeleportSimBodyToStartPosition(CarId, position, rotation);
       // sumo.TeleportPlayerToStartPosition(player, CarId);

        Debug.Log("Должно работать.");
        Debug.Log(CarId);
        Debug.Log(CarIds.First()); 
    }

    //Обновление вызывается один раз за кадр
    void Update()
    {
        if (!sumo.connected) return;

        try
        {
            sumo.DoStep();
        }
        catch (Exception er)
        {
            Debug.Log(er);
            return;
        }
        
        sumo.UpdateClientCar(CarId, player.transform);
        if (carsUnity.Count > 0) //Если машины есть
        {
            UpdateAllCars(); //Обновляем список машин (удаляем вышедшее, добавляем новые)
            UpdateAllCarsPositions(); //Обновляем позиции машин
        } 
        else 
        {
            CreateAllCars(sumo.GetNewCarInfos());
        }; //Если нет машин, то создаём

        //Та же логика со светофорами
        if(lightsUnity.Count > 0) UpdateLights();
        else CreateAllLights(sumo.GetLigthInfos());
    }

    /// <summary>
    /// Функция меняет трекинг и переводит слежку за другой машиной<br></br>
    /// И прикрепляет его на определённый объект на сцене
    /// </summary>
    /// <param name="CarId"></param>
    /// <param name="ModelName"></param>
    public void ChangeMainCar(string CarId, string ModelName)
    {
        player = GameObject.Find(ModelName);
        playerCamera.transform.SetParent(player.transform);
        playerCamera.transform.position.Set(playerCamera.transform.position.x, playerCamera.transform.position.y + 20f, playerCamera.transform.position.z);
        this.CarId = CarId;
        sumo.TrackCar(CarId);
        sumo.TeleportPlayerToStartPosition(player, CarId);
        Debug.Log($"Игрок изменён на {ModelName} - {CarId}");
    }

    /// <summary>
    /// Функция создаёт светофор по её информации
    /// </summary>
    /// <param name="lightInfo"></param>
    public void CreateLight(LightInfo lightInfo)
    {
        for(int i = 0; i < lightInfo.Lanes.Count; i++)
        {
            var light = Instantiate(LightObject, new Vector3(lightInfo.PosX[i], createPosY, lightInfo.PosY[i]), LightObject.transform.rotation);
            light.transform.SetParent(parentLights.transform, false);
            light.name = lightInfo.Lanes[i];
            lightsUnity.Add(light);
        }
        
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

        Debug.Log("Все светофороы созданы");
    }

    /// <summary>
    /// Функция создаёт все машины по списку информации
    /// </summary>
    public void CreateAllCars(List<CarInfo> CarsInfo)
    {
        foreach (var carInfo in CarsInfo)
        {
            if (CarIds.Count(x => x == carInfo.vehid) > 0) continue;
            CreateCar(carInfo);
        }

        Debug.Log("Все машины созданы");
    }

    /// <summary>
    /// Функция создаёт машину по её информации
    /// </summary>
    /// <param name="carInfo"></param>
    public void CreateCar(CarInfo carInfo)
    {
        GameObject obj;
        obj = isDebug ? allVeh : SumoObjects[carInfo.sizeclass];
        var car = Instantiate(obj, new Vector3(carInfo.PosX, 0, carInfo.PosY), obj.transform.rotation);
        car.transform.SetParent(parentCars.transform, false);
        car.name = carInfo.vehid;
        car.tag = "Other cars";
        carsUnity.Add(car);
    }

    /// <summary>
    /// Функция обновляет - добавляет и удаляет лишниее машины<br></br>
    /// Нужно сравнить с прошлым притоком и добавить новые<br></br>
    /// А потом удалить уже те, которых не пришло
    /// </summary>
    public void UpdateAllCars()
    {
        List<CarInfo> newcars, leftcars;
        (newcars, leftcars) = sumo.GetNewAndLeftCarInfos();


        for (int i = 0; i < leftcars.Count; i++)
        {
            if (leftcars[i] is null)
            {
                i++;
                continue;
            }

            GameObject toremove = GameObject.Find(leftcars[i].vehid);

            if (toremove)
            {
                carsUnity.Remove(toremove);
                Destroy(toremove);
            }
        }

        foreach (CarInfo carInfo in newcars)
        {
            CreateCar(carInfo);
        }
    }

    /// <summary>
    /// Функция обновляет все позиции и поведение машины
    /// </summary>
    /// <param name="CarsInfo"></param>
    public void UpdateAllCarsPositions()
    {
        List<CarInfo> CarsInfo = sumo.GetCarInfos();

        for (int i = 0; i < carsUnity.Count; i++)
        {
            var car = carsUnity[i];
            CarInfo carInfo = CarsInfo.Where(x => x.vehid == car.name).First();

            if (carInfo is null) continue;
 
            Vector3 tempPos = car.transform.position;               //Получаем текущую позицию
            tempPos.x = (float)(carInfo.PosX); 
            tempPos.z = (float)(carInfo.PosY);
            Quaternion tempRot = car.transform.rotation;            //Получаем текущую позицию
            Quaternion rot;
            Vector3 ydir = new Vector3(0, 1, 0);    //y направление вращения
            rot = Quaternion.AngleAxis((carInfo.heading), ydir);
            car.transform.SetPositionAndRotation(tempPos, rot);  //Устанавливаем положение и поворот
            var comp = car.GetComponent<VehicleHandler>();
            if (comp != null)
            {
                comp.CalculateSteering(carInfo.heading, carInfo.speed);
                comp.BrakeLightSwitch(carInfo.brakestate);
            }


        }
        Debug.Log("Обновлены все позиции машин");
    }

    /// <summary>
    /// Обновляет состояния всех светофоров по списку информации
    /// </summary>
    /// <param name="LightsInfo"></param>
    public void UpdateLights()
    {
        string phases = sumo.GetTLSPhasesStr();

        for(int i = 0; i < lightsUnity.Count; i++)
        {
            var light = lightsUnity[i];

            ChangeColorLight(light, phases[i]);
        }
        Debug.Log("Обновлены все состояния светофоров");
    }
    
    /// <summary>
    /// Меняет светофора по фазе
    /// </summary>
    /// <param name="light"></param>
    /// <param name="phase"></param>
    public void ChangeColorLight(GameObject light, char phase)
    {
        var lightRenderer = light.GetComponent<Renderer>();
        Color color = Color.white;
        int code = 0;

        switch (char.ToLower(phase))
        {
            case 'g': code = 1; break;
            case 'y': code = 2; break;
            case 'r': code = 3; break;
        }

        switch(code)
        {
            case 1: color = Color.green; break;
            case 2: color = Color.yellow; break;
            case 3: color = Color.red; break;
        }

        lightRenderer.material.SetColor("_Color", color);
    }
}

