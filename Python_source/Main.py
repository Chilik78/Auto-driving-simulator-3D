import time
from queue import Queue
import TrafficSimulator
import TCP_server
import Unity
import os, dotenv

#Класс, соединяющий симуляцию и сервер
class SumoUnity(object):
    def __init__(self, IP, Port, SumoNetwork, step_length):
        
        self.NetworkName = SumoNetwork #Название симуляции
        
        #Определяем очереди для связи
        self.UnityQueue = Queue(maxsize=2)

        #Запуск СУМО
        self.TrafficSim = TrafficSimulator.TrafficSimulator(self.NetworkName, step_length)
        self.TrafficLights = self.TrafficSim.ParseTrafficLights()
        self.SumoObjects = []
        
        #Запуск TCP-сервер
        self.ServerIP = IP
        self.ServerPort = Port

        self.Server = TCP_server.TCP_Server(self.ServerIP, self.ServerPort)
        self.Server.StartServer(self.UnityQueue)

    #Главная функция, которая постоянно обновляет данные
    def main(self):

        deltaT = 0.02

        while True:

            #Получить метку времени
            TiStamp1 = time.time()

            #Контролировать TCP-соединение - теперь это работает как отключение и нормальное закрытие программы после разрыва соединения
            if self.Server.ReopenSocket(self.UnityQueue): break
            
            #Обновить SUMO (машины, светофоры)
            self.SumoObjects, self.TrafficLights = self.TrafficSim.StepSumo(self.SumoObjects, self.TrafficLights)

            #Обновить Unity (скомпилировать данные и добавить их в очередь)
            Unity.ToUnity(self.SumoObjects, self.TrafficLights, self.UnityQueue)
            
            #Синхронизировать время
            TiStamp2 = time.time() - TiStamp1
            if TiStamp2 > deltaT:
                pass
            else:
                time.sleep(deltaT-TiStamp2)
            
            
if __name__ == '__main__':
    IP = 'localhost'
    port = 4042
    step_length = 0.02

    #Имя открываемой сети, но по сути путь к конфигу
    dotenv.load_dotenv()
    SumoNetwork = os.environ["Sim_Test"] 

    #Инициалиазция симуляции
    Simulation = SumoUnity(IP, port, SumoNetwork, step_length)

    #Запуск активности
    Simulation.main()
    
    