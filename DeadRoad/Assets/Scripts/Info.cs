using System.Globalization;
using System;
using System.Collections.Generic;

namespace Main
{
    /// <summary>
    /// Класс светофора
    /// </summary>
    public class LightInfo
    {
        public string ID;
        public List<string> Lanes;
        public List<float> PosX;
        public List<float> PosY;
        public List<char> CurrentPhases;

        public LightInfo(string ID, List<string> Lanes, List<float> PosX, List<float> PosY, string CurrentPhases)
        {
            this.ID = ID;
            this.Lanes = Lanes;
            this.PosX = PosX;
            this.PosY = PosY;

            this.CurrentPhases = splitPhases(CurrentPhases);
        }

        public List<char> splitPhases(string phases)
        {
            List<char> split = new List<char>();
            foreach (char ch in phases)
            {
                split.Add(ch);
            }
            return split;
        }

    }

    /// <summary>
    /// Класс машины
    /// </summary>
    public class CarInfo
    {
        public string vehid;
        public float PosX;
        public float PosY;
        public float speed;
        public float heading;
        public float brakelight; //float
        public int sizeclass;
        public bool brakestate; //bool

        public CarInfo(string vehid, double posx, double posy, double speed, double heading, string sizeclass)
        {
            this.vehid = vehid;
            this.PosX = (float)posx;
            this.PosY = (float)posy;
            this.speed = (float)speed;
            this.heading = (float)heading;

            string type = sizeclass.Substring(0, sizeclass.IndexOf('_') + 1);

            if (type == "veh")
                this.sizeclass = 0; //Ауди Влада
            else if (type == "bus")
                this.sizeclass = 1; //Автобус
            else if (type == "truck")
                this.sizeclass = 2; //Грузовик
            else
                this.sizeclass = 3; //Ничего или пешеход
        }
    }
}