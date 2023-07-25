using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Main;


public class MainHandler : MonoCache
{
    [Header("Общая папка")]
    public GameObject MyScripts;

    [Header("Настройки симуляции")]
    public string SumoPath;
    public string SimPath;
    public float SimLength;
    public int MaxNumVeh = 50;

    [Header("Настройка игрока")]
    public GameObject player;
    public string CarId;
    public List<string> CarIds;
    public Camera playerCamera;

    [Header("Типы объектов")]
    public GameObject[] SumoObjects;

    [Header("Светофор")]
    public int LightCount = 6;

    [Header("Одна модель для отладки")]
    public GameObject allVeh;
    public bool isDebug;


    private List<GameObject> carsUnity = new List<GameObject>(); //Все машины на карте
    private List<TLSCross> TLSCrosses = new List<TLSCross>(); //Все светофоры на карте
    private GameObject parentCars; //Папка для групировки

    private SUMO sumo; //Объект симуляции
    private List<string> ids;


    //Если хочется передвинуть машину в симуляции в какую-либо точку
    //И использовать sumo.TeleportSimBodyToStartPosition(id, position, rotation)
    [Header("Точка спавна машины в симуляции")]
    public Vector3 position;
    public Quaternion rotation;

    //Старт вызывается перед обновлением первого кадра
    /*void Start()
    {
        parentCars = new GameObject("Cars");
        sumo = new SUMO(4042, SimPath, SimLength);
        SUMO.StartSimulation(CarId, CarIds);
        if (player == null)
            player = GameObject.Find("player");

        sumo.TeleportSimBodyToStartPosition(CarId, position, rotation);
        // sumo.TeleportPlayerToStartPosition(player, CarId);

        ids = getTrafficLightsId(LightCount);
        foreach (string id in ids)
        {
            TLSCross cross = new TLSCross(id);
            cross.program = sumo.GetCountLaneToProgramById(id);
            TLSCrosses.Add(cross);
        }
        
        Debug.Log("Должно работать.");
    }*/



    //Обновление вызывается один раз за кадр
    public override void OnTick()
    {
        if (sumo == null || !sumo.connected) return;

        sumo.DoStep();
        Transform tr = player.transform;
        double x = transform.position.x;
        double z = transform.position.z;
        double y = transform.rotation.eulerAngles.y;
        sumo.UpdateClientCar(CarId, x, z, y);
        if (carsUnity.Count > 0) //Если машины есть
        {
            UpdateAllCars(); //Обновляем список машин (удаляем вышедшее, добавляем новые)
            UpdateAllCarsPositions(); //Обновляем позиции машин
        } 
        else 
        {
            CreateAllCars(sumo.GetNewCarInfos());
        }//Если нет машин, то создаём

        UpdateLights();
    }

    public void StartSim(string playerId)
    {
        parentCars = new GameObject("Cars");
        sumo = new SUMO(4042, SimPath, SumoPath, MaxNumVeh, SimLength);
        CarId = playerId;
        sumo.StartSimulation(CarId, CarIds);
        if (player == null)
            player = GameObject.FindWithTag("Player");

        sumo.TeleportSimBodyToStartPosition(CarId, position, rotation);
        // sumo.TeleportPlayerToStartPosition(player, CarId);

        ids = getTrafficLightsId(LightCount);
        foreach (string id in ids)
        {
            TLSCross cross = new TLSCross(id);
            cross.program = sumo.GetCountLaneToProgramById(id);
            TLSCrosses.Add(cross);
        }

        Debug.Log("Должно работать.");
    }

    public void ReturnToDefault()
    {
        sumo.KillSim();
        sumo = null;
        Destroy(parentCars);
   
        foreach(var car in carsUnity)
        {
            Destroy(car);
        }

        carsUnity.Clear();
        TLSCrosses.Clear();
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
    /// Функция создаёт все машины по списку информации
    /// </summary>
    public void CreateAllCars(List<CarInfo> CarsInfo)
    {

        if (CarsInfo.Count == 0) return;

        foreach (var carInfo in CarsInfo)
        {
            //Debug.LogWarning($"Car Info veh{carInfo.vehid}");
            if (carInfo == null || CarIds.Count(x => x == carInfo.vehid) > 0) continue;
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

            //GameObject toremove = GameObject.Find(leftcars[i].vehid);
            GameObject toremove = carsUnity.Where(x => x.name == leftcars[i].vehid).FirstOrDefault();

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
        if (CarsInfo == null) return;
        if (CarsInfo.Count < 1) return;

        for (int i = 0; i < carsUnity.Count; i++)
        {
            GameObject car = carsUnity[i];
            var carInfo = CarsInfo.Where(x => x.vehid == car.name).FirstOrDefault();
 
            Vector3 tempPos = car.transform.position;               //Получаем текущую позицию
            tempPos.x = carInfo.PosX; 
            tempPos.z = carInfo.PosY;
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
    /// Функция вовзращает id каждого светофора в игре
    /// </summary>
    /// <returns></returns>
    private List<string> getTrafficLightsId(int tlsCount)
    {
        List<string> ids = new List<string>();
        for (int i = 1; i <= tlsCount; i++)
        {
            GameObject obj = GameObject.Find($"TLC_{i}");
            if (obj == null) continue;
            ids.Add(obj.name);
        }
        return ids;
    }

    /// <summary>
    /// Обновляет состояния всех светофоров с помощью состояний
    /// </summary>
    public void UpdateLights()
    {
        List<string> phases = sumo.GetTLSPhases(ids);
        for (int i = 0; i < phases.Count; i++) TLSCrosses[i].UpdateCrossState(phases[i]);
        Debug.Log("Обновлены все состояния светофоров");
    }
    
    

   

    
}

