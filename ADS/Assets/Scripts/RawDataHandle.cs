using System.Linq;
using System.Collections.Generic;

namespace Main
{
    /// <summary>
    /// Класс методов для обработки полученных данных с сервера
    /// </summary>
    static class RawData
    {
        /// <summary>
        /// Создаёт список объектов в виде строки
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        static public List<string> SplitData(string message)  //Разбиваем входящую строку на транспортное средство
        {
            if (message is null || !message.Contains("@")) return new List<string>();    //@ - разделитель между транспортными средствами
            return message.Split('@').Distinct().ToList();
        }

        /// <summary>
        /// Создаёт список информации о машинах по сообщению с сервера
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        static public List<CarInfo> GetCarsFromData(string message)
        {
            var data = SplitData(message);
            return data.Select(x => new CarInfo(x)).ToList();
        }

        /// <summary>
        /// Создаёт список информации о светофорах по сообщению с сервера
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        static public List<LightInfo> GetLightsFromData(string message)
        {
            var data = SplitData(message);
            return data.Select(x => new LightInfo(x)).ToList();
        }

    }
    
}