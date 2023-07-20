import threading
import socket
import time

#Запуск отправки на клиент
def StartUnity(Client, UnityQueue):
    #Начало потока
    UnityClientErr = threading.Event() #Флаг остановки для событий
    
    #Создаётся и запускается поток для функции SendMessage с параметрами Client, UnityClientErr и UnityQueue
    UnityThread = threading.Thread(target=SendMessage, args=(Client, UnityClientErr, UnityQueue)) 
    UnityThread.start()

    #Функция возвращается  Событие потока и сам поток
    return UnityClientErr, UnityThread

#Функция отправки сообщения, которая будет запущена в отдельном потоке
def SendMessage(Client:socket.socket, UnityClientErr, UnityQueue):
    
    #Периодически отправляет сообщения в Unity
    while True:

        #Пробует получить данные из очереди
        if (UnityQueue.empty()):
            time.sleep(0.005)
        else:
            cars = UnityQueue.get()
            tlights = UnityQueue.get()
            # Префикс каждого сообщения длиной 4 байта (сетевой порядок байтов)
            try:
                Client.send(cars.encode()) #Отправляем машины
                Client.send(tlights.encode()) #Отпраляем светофоры
            except socket.error as e: #если соединение с клиентом потеряно, отображает ошибку
                print(e)
                #Установить состояние потока на ошибку, ибо клиент разорвал соединение
                UnityClientErr.set()
                break

#Создаёт сообщение, которое будет отправлено в Unity
def ToUnity(Vehicles, TrafficLights, UnityQueue):

    DataToUnity = "O1G" #Рандомный код для определения, что это машины 

    #Другие транспортные средства в симуляции
    for veh in Vehicles:
        DataToUnity += veh.ID + ";" + "{0:.3f}".format(veh.PosX_Center) + ";" + "{0:.3f}".format(veh.PosY_Center) + ";" + "{0:.2f}".format(veh.Velocity) + ";"  + "{0:.2f}".format(veh.Heading) + ";" + str(int(veh.StBrakePedal)) + ";" + str(veh.SizeClass) + "@"

    #Разрыв строки
    DataToUnity = DataToUnity[:-1] + "&" 
    
    #Светофоры
    DataToUnityL = "G1O" #Рандомный код для определения, что это светофоры 

    #Светофоры
    for tls in TrafficLights:
        DataToUnityL += str(tls.ID) + ";" + str(tls.LaneID) + ";" + "{0:.3f}".format(tls.PosX) + ";" + "{0:.3f}".format(tls.PosY) + ";" + str(tls.CurrentPhase) + "@"

    #Разрыв строки
    DataToUnityL = DataToUnityL[:-1] + "&" 

    with UnityQueue.mutex: #Mutex - описывается как объект синхронизации для защиты доступа к общему ресурсу
        UnityQueue.queue.clear() #В любом случаем отчищаем очередь, если случайно там что-то есть

    UnityQueue.put(DataToUnity)  #Ставим в очередь обновлённые данные машин
    UnityQueue.put(DataToUnityL) #Ставим в очередь обновлённые данные светофоров