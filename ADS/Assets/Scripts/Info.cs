using System.Collections.Generic;
using UnityEngine;


namespace Main
{
    /// <summary>
    /// Класс светофора
    /// </summary>
    public class TLS
    {
        public string name; //Название светофора
        public int headCount; //Количество голов на светофоре
        public string headName1;
        public string headName2;
  
        public TLS(string name, string headName1, string headName2)
        {
            this.name = name;
            headCount = 2;
            this.headName1 = headName1;
            this.headName2 = headName2;
        }

        public TLS(string name ,string headName1)
        {
            this.name = name;
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
        public int[] program;
        public List<MeshRenderer[]> MeshRendereres;
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
            MeshRendereres = GetAllRenders();
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
            Transform tr = GameObject.Find(name).transform;
            if (name.Contains("Big"))
            {
                var head1 = tr.GetChild(0);
                var head2 = tr.GetChild(1);
                return new TLS(name, head1.name, head2.name);
            }
            var head = tr.GetChild(0);
            return new TLS(name, head.name);
        }
        
        /// <summary>
        /// Получить все массивы рендереров - массив на голову
        /// </summary>
        /// <returns></returns>
        public List<MeshRenderer[]> GetAllRenders()
        {
            List<MeshRenderer[]> list = new List<MeshRenderer[]>();
            foreach (TLS tls in tlss)
            {
                var head1 = GameObject.Find(tls.headName1);
                if (tls.headCount > 1)
                {
                    list.Add(head1.GetComponentsInChildren<MeshRenderer>());
                    list.Add(GameObject.Find(tls.headName2).GetComponentsInChildren<MeshRenderer>());
                }
                else
                {
                    list.Add(head1.GetComponentsInChildren<MeshRenderer>());
                }
            }
            return list;
        }

        /// <summary>
        /// Функция принимает фазы и обновляет состояние перекрёстка
        /// </summary>
        /// <param name="phases"></param>
        public void UpdateCrossState(string phases)
        {
            string p = phases;

            //Обрезаем строку по длина программы и сразу обновляем состояние
            int i = 0;
            foreach (int l in program)
            {
                MeshRenderer[] mr = MeshRendereres[i];
                string state = p.Substring(0, l);
                foreach (char ch in state) ChangeColorLight(mr, ch);
                p = p.Remove(0, l);
                i++;
            }
        }

        /// <summary>
        /// Меняет светофора по фазе
        /// </summary>
        /// <param name="phase"></param>
        public void ChangeColorLight(MeshRenderer[] render, char phase)
        {

            Color color = Color.white;
            int code = 0;
            switch (char.ToLower(phase))
            {
                case 'g': color = Color.green; code = 1; break;
                case 'y': color = Color.yellow; break;
                case 'r': color = Color.red; code = 2; break;
            }

            for (int i = 0; i < 3; i++)
            {
                if (i != code)
                {
                    render[i].material.DisableKeyword("_EMISSION");
                    render[i].material.color = new Color(0.179f, 0.171f, 0.171f, 0.0039f);
                }
                else
                {
                    render[i].material.EnableKeyword("_EMISSION");
                    render[i].material.SetColor("_EmissionColor", color);
                    render[i].material.color = color;
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

            string type = sizeclass.Substring(0, sizeclass.IndexOf('_'));

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