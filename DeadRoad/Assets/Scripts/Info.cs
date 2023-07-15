using System.Globalization;
using System;

namespace Main
{
    /// <summary>
    /// Класс светофора
    /// </summary>
    public class LightInfo
    {
        public string ID;
        public string LaneID;
        public float PosX;
        public float PosY;
        public int CurrentPhase;

        public LightInfo(string txt)
        {
            if (txt.Contains(';'))
            {
                string[] b = txt.Split(';');
                if(b.Length >=5)
                {
                    ID = b[0];
                    LaneID = b[1]; 
                    PosX = (float)Convert.ToDouble(b[2], new CultureInfo("en-US"));
                    PosY = (float)Convert.ToDouble(b[3], new CultureInfo("en-US"));
                    CurrentPhase = int.Parse(b[4]);
                }
            }
        }

    }

    /// <summary>
    /// Класс машины
    /// </summary>
    public class CarInfo
    {
        public string vehid;
        public float posx;
        public float posy;
        public float speed;
        public float heading;
        public float brakelight; //float
        public int sizeclass;
        public bool brakestate; //bool

        public CarInfo(string txt)
        {
            if (txt.Contains(";"))
            {
                string[] a = txt.Split(';'); //Разделить данные о транспортном средстве, порядок данных: vehid, posx, posy, speed, heading, stoplight state, sizeclass
                if (a.Length >= 7)
                {
                    vehid = a[0];
                    posx = (float)Convert.ToDouble(a[1], new CultureInfo("en-US"));
                    posy = (float)Convert.ToDouble(a[2], new CultureInfo("en-US"));
                    speed = (float)Convert.ToDouble(a[3], new CultureInfo("en-US"));
                    heading = (float)Convert.ToDouble(a[4], new CultureInfo("en-US"));
                    brakelight = (float)Convert.ToDouble(a[5], new CultureInfo("en-US"));
                    sizeclass = (int)Convert.ToDouble(a[6], new CultureInfo("en-US"));      //Не используется в этом проекте

                    if (brakelight == 1)
                        brakestate = true;
                    else
                        brakestate = false;
                }
            }
        }
    }
}