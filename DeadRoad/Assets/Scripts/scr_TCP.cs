using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class scr_TCP : MonoBehaviour
{
    private TcpClient socketConnection; //Подключение клиента
    private Thread clientReceiveThread; //Поток
    private int ipPort = 4042;                      //TCP/IP PORT
    private string IpAd_loc = "localhost";          //TCP/IP ADRESS
    public int NumOfConnection = 1;  //Кол-во подклчений
    private int TCPtimeout = 3; //Тайм-аут для остановки переподключения, при попытке переподключения редактор запускается после остановленной симуляции. Сначала нужно было остановить сервер
    private string message2send; //Сообщение для отправки
    public Queue<string> TCP_recv_queue = new Queue<string>(); //Очередь для хранения полученных сообщений
    private string Rx; //Переменная для передачи данных в Main


    ///Старт вызывается перед обновлением первого кадра
    void Start()
    {
        Debug.Log("Соединился.");
        ConnectToTcpServer(); //Соединияемся
        
    }

    ///Обновление вызывается один раз за кадр
    void Update()
    {
        if (TCP_recv_queue.Count > 0) //Если есть какое-то сообщение
        {
            string msg = TCP_recv_queue.Dequeue();
            if ((msg.Contains("O1G") || msg.Contains("G1O")) && msg.Contains("&"))
            {
                Rx = msg;
            }
        }
        else
        {
            Debug.Log("Que is empty");
        }
    }

    /// <summary>
    /// Функция, которая возвращает сообщения из TCP.cs
    /// </summary>
    /// <returns></returns>
    public string RxMsg() 
    {
        return Rx;
    }

    /// <summary>
    /// Функция запуска TCP клиента<br></br>
    /// Так же запускается в отдельном потоке и работает на фоне
    /// </summary>
    private void ConnectToTcpServer()
    {
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(Con));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }

    /// <summary>
    /// Функция подключения к серверу, вызываемая из потока
    /// </summary>
    private void Con()
    {
        bool connect = true;
        while (connect)
        {
            NumOfConnection += 1;
            if (NumOfConnection < TCPtimeout)
            {
                int i = 1;
                try
                {
                    socketConnection = new TcpClient(IpAd_loc, ipPort); //Создаём подключение
                    message2send = "U3D00"; //Сообщение, которое отправляем на сервер, для старта потока данных
                    SendMessage(); //Отправляем сообщение
                    ListenForData(); //
                    connect = false;
                    i++;
                }
                catch
                {

                }
            }
            else
            {
                clientReceiveThread.Abort();
                Debug.Log("Exit the game");
                Application.Quit();
            }
        }
    }

    /// <summary>
    /// Функция пересоеденения к серверу
    /// </summary>
    private void ReCon()
    {
        NumOfConnection += 1;
        if (NumOfConnection < TCPtimeout)
        {
            try
            {
                socketConnection = new TcpClient(IpAd_loc, ipPort);
                message2send = "U3D00";
                SendMessage();
                ListenForData();
            }
            catch
            {

            }
        }
        else
        {
            clientReceiveThread.Abort();
            Debug.Log("Exit the game");
            Application.Quit();
        }

    }

    /// <summary>
    /// Функция получает данные с сервера и кладёт в очередь
    /// </summary>
    private void ListenForData()
    {
        try
        {
            NumOfConnection = 1;
            Byte[] bytes = new Byte[16384];
            while (true)
            {
                //Получаем объект потока для чтения 		
                try
                {
                    Debug.Log("connected");
                    using (NetworkStream stream = socketConnection.GetStream())
                    {
                        int length;
                        TCP_recv_queue.Clear();

                        //Считатываем входящий поток в массив байтов 
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incommingData = new byte[length];
                            Array.Copy(bytes, 0, incommingData, 0, length);

                            //Преобразование массива байтов в строковое сообщение						
                            string serverMessage = Encoding.ASCII.GetString(incommingData);
                            TCP_recv_queue.Enqueue(serverMessage);
                        }
                    }
                }
                catch
                {
                    Debug.Log("lost connection");
                    ReCon();
                }
            }
        }
        catch
        {
            ReCon();
        }
    }


    /// <summary>
    /// Отправляет сообщение, хранящиеся в message2send, на сервер
    /// </summary>
    private void SendMessage()
    {
        if (socketConnection == null)
        {
            Debug.Log("NO SERVER AVAILABLE");
        }
        try
        {
            //Получить объект потока для записи 			
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                string clientMessage = message2send;
                //Преобразование строкового сообщения в массив байтов                
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
                //Записываем массив байтов в поток socketConnection.                
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("Выключаюсь");
        clientReceiveThread.Abort();
    }
}
