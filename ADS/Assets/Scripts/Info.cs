using System.Globalization;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace Main
{
    /// <summary>
    /// Класс светофора
    /// </summary>
    public class TLS
    {
        public string name; //Название светофора
        public int headCount; //Количество голов на светофоре
        public Transform transform1; //Позиция части с красным кругом первой головы
        public Transform transform2; //Позиция части с красным кругом второй головы 
        public List<GameObject> circles1; //Светящиется кругляшки
        public List<GameObject> circles2; //Светящиется кругляшки
        public string headName1;
        public string headName2;
        public TLS(string name, Transform transform1, Transform transform2, List<GameObject> circles1, List<GameObject> circles2, string headName1, string headName2)
        {
            this.name = name;
            this.transform1 = transform1;
            this.transform2 = transform2;
            this.circles1 = circles1;
            this.circles2 = circles2;
            headCount = 2;
            this.headName1 = headName1;
            this.headName2 = headName2;
        }

        public TLS(string name, Transform transform1, List<GameObject> circles1, string headName1)
        {
            this.name = name;
            this.transform1 = transform1;
            this.circles1 = circles1;
            this.headName1 = headName1;
            headCount = 1;
        }
    }

    /// <summary>
    /// Класс перекрёстка со светофорами
    /// </summary>
    public class TLSCross
    {
        public string name; //Название перекрёстка
        public List<TLS> tlss = new List<TLS>(); //Светофоры

        public TLSCross(string name)
        {
            this.name = name;
            List<string> tlss_str = getTrafficLightsChildId(name);
            foreach(string id in tlss_str)
            {
                TLS tls = getTLSFromName(id);
                if (tls != null)
                    tlss.Add(tls);
            }
        }

        /// <summary>
        /// Возвращает имена всех детей
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private List<string> getTrafficLightsChildId(string id)
        {
            List<string> ids = new List<string>();
            Transform obj = GameObject.Find(id).transform;
            for (int i = 0; i < obj.childCount; i++) ids.Add(obj.GetChild(i).name);
            return ids;
        }

        /// <summary>
        /// Возвращает объект светофора по названию
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private TLS getTLSFromName(string name)
        {
            if (name.Contains("Big"))
            {
                var head1 = GameObject.Find(name).transform.GetChild(0);
                var head2 = GameObject.Find(name).transform.GetChild(1);

              
                var first = searchChild(head1, "Зелёный фонарь");
                var second = searchChild(head2, "Зелёный фонарь", true);
                return new TLS(name, first, second, getThreeCircles(head1), getThreeCircles(head2, true), head1.name, head2.name);

            }

            var head = GameObject.Find(name).transform.GetChild(0);
            var one = searchChild(head, "Зелёный фонарь");

            if (one == null) return null; 

            return new TLS(name, one, getThreeCircles(head), head.name);
        }

        /// <summary>
        /// Ищет ребёнка слева и права
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childName"></param>
        /// <param name="fromRight"></param>
        /// <returns></returns>
        private Transform searchChild(Transform parent, string childName, bool fromRight = false)
        {
            if(fromRight)
                for(int i = parent.childCount-1; i > 0; i--)
                {
                    var child = parent.GetChild(i);
                    if (child.name == childName) return child;
                }
            for(int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                if (child.name == childName) return child;
            }
            return null;
        }

        private List<GameObject> getThreeCircles(Transform Parent, bool fromRight = false)
        {
            Debug.Log($"p:{Parent.name}");
            if (fromRight)
            {
                GameObject green = GameObject.Find(searchChild(Parent, "Зелёный фонарь", fromRight).name);
                GameObject yellow = GameObject.Find(searchChild(Parent, "Жёлтый фонарь", fromRight).name);
                GameObject red = GameObject.Find(searchChild(Parent, "Красный фонарь", fromRight).name);
                return new List<GameObject>() { green, yellow, red };
            }

            GameObject green1 = GameObject.Find(searchChild(Parent, "Зелёный фонарь").name);
            GameObject yellow1 = GameObject.Find(searchChild(Parent, "Жёлтый фонарь").name);
            GameObject red1 = GameObject.Find(searchChild(Parent, "Красный фонарь").name);
            return new List<GameObject>() { green1, yellow1, red1 };
        }

       
        public List<MeshRenderer[]> GetAllRenders()
        {
            List<MeshRenderer[]> list = new List<MeshRenderer[]>();
           
            foreach (TLS tls in tlss)
            {
                if (tls.headCount > 1)
                {
                    list.Add(GameObject.Find(tls.headName1).GetComponentsInChildren<MeshRenderer>());
                    list.Add(GameObject.Find(tls.headName2).GetComponentsInChildren<MeshRenderer>());
                }
                else
                {
                    list.Add(GameObject.Find(tls.headName1).GetComponentsInChildren<MeshRenderer>());
                }
            }

            return list;
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