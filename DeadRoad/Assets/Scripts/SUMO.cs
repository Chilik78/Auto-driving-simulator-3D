using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using CodingConnected.TraCI.NET;
using UnityEngine;

namespace Main
{

    public class SUMO
    {
        private TraCIClient client;
        private int port;
        private string host;
        private Thread Stepping;
        public bool connected;

        /// <summary>
        /// Конструктор SUMO
        /// </summary>
        /// <param name="port"></param>
        /// <param name="host"></param>
        /// <param name="step_length"></param>
        public SUMO(int port, string SimPath, float step_length = 0.02f, string host = "localhost")
        {
            string sumoBinary = System.Environment.GetEnvironmentVariable("SUMO_HOME") + @"\bin\sumo-gui";
            string FolderPath = @"Assets\SUMO_Networks\";

            string map = SimPath.Contains(".sumocfg") ? SimPath : SimPath + ".sumocfg";
            map = map.Replace('/', '\\');

            string[] commands = new string[10] { "/C", sumoBinary, "-c", FolderPath + map, "--start", "--quit-on-end", "--step-length", step_length.ToString().Replace(',', '.'), "--remote-port", port.ToString() };
            string strCmdText = string.Join(' ', commands);
            Debug.Log(strCmdText);

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = strCmdText;
            process.StartInfo = startInfo;
            process.Start();

            this.port = port;
            this.host = host;
            client = new TraCIClient();
        }

        /// <summary>
        /// Подключение к traci 
        /// </summary>
        public void StartSimulation(string playerId, List<string> ids)
        {
            this.connected = client.Connect(host, port);
            Debug.Log(connected);

            while(!connected) client.Connect(host, port);

            if (connected)
            {
                client.Control.SimStep();
                client.Control.SimStep();

                TrackCar(playerId);
                ChangeZoom(8000);

                foreach(string id in ids)
                {
                    client.Vehicle.SetSpeed(id, 0);
                }
                //1717.212 5066.528
            }
        }

        /// <summary>
        /// Изменение зума по виду
        /// </summary>
        /// <param name="degree"></param>
        public void ChangeZoom(int degree, string view = "View #0")
        {
            client.Gui.SetZoom(view, degree);
        }

        /// <summary>
        /// Прикрепление трекинга в сумо
        /// </summary>
        /// <param name="carid"></param>
        /// <param name="view"></param>
        public void TrackCar(string carid, string view = "View #0")
        {
            client.Gui.TrackVehicle(view, carid);
        }

        /// <summary>
        /// Телепортирование машины в симуляции на начальную точку
        /// </summary>
        /// <param name="carid"></param>
        public void TeleportSimBodyToStartPosition(string carid, Vector3 position, Quaternion rotation)
        {
            GameObject obj = new GameObject();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            UpdateClientCar(carid, obj.transform);
        }

        /// <summary>
        /// Телепортирование модели игрока на позиции, как в симуляции
        /// </summary>
        /// <param name="player"></param>
        /// <param name="carid"></param>
        public void TeleportPlayerToStartPosition(GameObject player, string carid)
        {
            var shape = client.Vehicle.GetPosition(carid).Content;
            var angle = client.Vehicle.GetAngle(carid).Content;
            player.transform.position = new Vector3((float)shape.X, 1.33f, (float)shape.Y);
            player.transform.rotation = Quaternion.Euler(0, (float)angle, 0);
        }

        /// <summary>
        /// Обновление данных в симуляции по движению модели в игре
        /// </summary>
        /// <param name="id"></param>
        /// <param name="carTransform"></param>
        public void UpdateClientCar(string id, Transform carTransform)
        {
            var road = client.Vehicle.GetRoadID(id).Content;
            var lane = client.Vehicle.GetLaneIndex(id).Content;
            client.Vehicle.MoveToXY(id, road, lane, 
                (double)carTransform.position.x,
                (double)carTransform.transform.position.z, 
                (double)carTransform.transform.eulerAngles.y,
                 2);
        }

        /// <summary>
        /// Шаг симуляции
        /// </summary>
        public void DoStep()
        {
            client.Control.SimStep();
        }

        /// <summary>
        /// Функция возвращает список информации о новых машинах
        /// </summary>
        /// <returns></returns>
        public List<CarInfo> GetNewCarInfos()
        {
            List<CarInfo> newcars = new List<CarInfo>();
            var newvehicles = client.Simulation.GetDepartedIDList("0").Content;
            Debug.Log($"Прибыло: {newvehicles.Count}");

            foreach (string id in newvehicles)
            {
                newcars.Add(withIdGetCarInfo(id));
            }
            return newcars;
        }

        /// <summary>
        /// Функция возвращает список информации о выбывших машинах
        /// </summary>
        /// <returns></returns>
        public List<CarInfo> GetLeftCarInfos()
        {
            List<CarInfo> leftcars = new List<CarInfo>();
            var vehiclesleft = client.Simulation.GetArrivedIDList("0").Content;
            Debug.Log($"Убыло: {vehiclesleft.Count}");

            foreach (string id in vehiclesleft)
            {
                leftcars.Add(withIdGetCarInfo(id));
            }
            return leftcars;
        }

        /// <summary>
        /// Функция возвращает два списка информации о новых машинах и выбывших машинах
        /// </summary>
        /// <returns></returns>
        public (List<CarInfo>, List<CarInfo>) GetNewAndLeftCarInfos()
        {
            return (GetNewCarInfos(), GetLeftCarInfos());
        }

        /// <summary>
        /// Функция возвращает два списка информации о нынишных машинах и машинах на удаление
        /// </summary>
        public List<CarInfo> GetCarInfos()
        {
            List<CarInfo> cars = new List<CarInfo>();
            foreach (string id in client.Vehicle.GetIdList().Content)
            {
                cars.Add(withIdGetCarInfo(id));
            }
            return cars;
        }

        /// <summary>
        /// Функция возвращает объект информации о машине
        /// </summary>
        /// <param name="id"></param>
        private CarInfo withIdGetCarInfo(string id)
        {
            var car = client.Vehicle;
            var pos = car.GetPosition(id).Content;
            var speed = car.GetSpeed(id).Content;
            var heading = car.GetAngle(id).Content;
            var sizeclass = car.GetTypeID(id).Content;

            if (heading == 0) return null;

            return new CarInfo(id, pos.X, pos.Y, speed, heading, sizeclass);
        }

        /// <summary>
        /// Функция возвращает список информации о светофорах
        /// </summary>
        public List<LightInfo> GetLigthInfos()
        {
            List<LightInfo> TrafficLights = new List<LightInfo>();
            var TlsIDs = client.TrafficLight.GetIdList().Content;
            foreach (var id in TlsIDs)
            {
                List<string> Lanes = client.TrafficLight.GetControlledLanes(id).Content;
                List<float> LanesX = new List<float>();
                List<float> LanesY = new List<float>();

                foreach(var l in Lanes)
                {
                    var pos = client.Lane.GetShape(l).Content;
                    
                    LanesX.Add((float)pos.Points.ElementAt(pos.Points.Count - 1).X);
                    LanesY.Add((float)pos.Points.ElementAt(pos.Points.Count - 1).Y);

                    
                }

                string states = client.TrafficLight.GetState(id).Content;

                TrafficLights.Add(new LightInfo(id, Lanes, LanesX, LanesY, states));

                
            }
            return TrafficLights;
        }

        /// <summary>
        /// Получает фазы на перекрёсток в формате строки
        /// </summary>
        public List<string> GetTLSPhases()
        {
            var TlsIDs = client.TrafficLight.GetIdList().Content;
            List<string> phases = new List<string>();

            foreach(string id in TlsIDs)
            {
                phases.Add(client.TrafficLight.GetState(id).Content);
            }
            return phases;
        }

        /// <summary>
        /// Возвращает фазы, скленные в одну строку
        /// </summary>
        /// <returns></returns>
        public string GetTLSPhasesStr()
        {
            var TlsIDs = client.TrafficLight.GetIdList().Content;
            StringBuilder sb = new StringBuilder();
            foreach (string id in TlsIDs)
            {
                sb.Append(client.TrafficLight.GetState(id).Content);
            }
            return sb.ToString();
        }
    }
}