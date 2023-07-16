import traci
import os
import sys
import SUMO_vehicle
from TrafficLight import TrafficLight
import dotenv

dotenv.load_dotenv()

class TrafficSimulator(object):
    def __init__(self, NetworkName, delay):

        self.NetworkName = NetworkName #Название симуляции
        self.StartSumo(delay) #Запуск старта симуляции
        self.ParseNetwork() #Парсинг симуляции

    #Запускает сумо
    def StartSumo(self, delay):
        
        #Получает папку с СУМО
        if 'SUMO_HOME' in os.environ:
            tools = os.path.join(os.environ['SUMO_HOME'], 'tools')
            sys.path.append(tools)
        else:
            sys.exit("please declare environment variable 'SUMO_HOME'")

        #sumo-gui для запуска интерфейса
        sumoBinary = os.environ['SUMO_HOME'].replace('\\', '/')+"/bin/sumo-gui"  # "C:/Sumo/bin/sumo-gui"
        #Папка с картами
        FolderPath = "../SUMO_Networks/"
        #Команда для запуска
        sumoCmd = [sumoBinary, "-c",
                   FolderPath + self.NetworkName, "--start", "--step-length", str(0.1), "--quit-on-end", "-d", str(delay)]
        
        #Сам запуск
        traci.start(sumoCmd)

        print("Sumo is running")

    def ParseNetwork(self):
        #Получить идентификаторы линий
        self.Edges = traci.lane.getIDList()

        #Получить формы краев
        self.LinkShapes = []
        for e in self.Edges:
            self.LinkShapes.append(traci.lane.getShape(e))

    def RestartSumo(self, SumoObjects):

        #Перезапускаем программу
        self.StartSumo()
        self.ParseNetwork()

        #Поместите объекты обратно в симулятор
        for Obj in SumoObjects:
            Obj.ReinsertVehicle()

    def StepSumo(self, SumoObjects, TrafficLights):

        try:
            traci.simulationStep()  #Шаг симулятор
        except:
            print("Restarting SUMO")
            self.RestartSumo(SumoObjects)

        SumoObjectsRaw0 = traci.vehicle.getIDList()  #Получить идентификатор каждого автомобиля
        SumoObjectNames = list(set(SumoObjectsRaw0)) #Сделать их уникальными

        #Удалять объекты SUMO из списка, если они покинули сеть
        for Obj in SumoObjects:
            if(not(any(ObjName == Obj.ID for ObjName in SumoObjectNames))):
                SumoObjects.remove(Obj)

        #Добавлять новые объекты и обновлять существующие.
        for VehID in SumoObjectNames:
            if(not(any(Obj.ID == VehID for Obj in SumoObjects))):
                NewlyArrived = SUMO_vehicle.SumoObject(VehID)
                SumoObjects.append(NewlyArrived)

        #Обновление объектов транспортных средств сумо
        for Obj in SumoObjects:
            Obj.UpdateVehicle()

        #Обновить фазы светофора
        TrafficLights = self.UpdateSignalPhases(TrafficLights)

        return SumoObjects, TrafficLights

    def ParseTrafficLights(self):

        TrafficLights = []

        #Получить Id светофоров
        self.LightIDs = traci.trafficlight.getIDList()

        #Пройтись по всем сигнальным перекресткам
        for ID in self.LightIDs:
            LightLaneList = traci.trafficlight.getControlledLanes(ID)

            #Прокручиваем все сигнальные головки на перекрестке
            idx = 0
            for Lane in LightLaneList:
                
                #Координаты, описывающие 2D плоскость
                Pos = traci.lane.getShape(Lane)

                LightPositionX = Pos[len(Pos) - 1][0]
                LightPositionY = Pos[len(Pos) - 1][1]

                #Создаем объект светофора в симуляторе
                TrafficLightObject = TrafficLight(ID, Lane, idx, LightPositionX, LightPositionY)
                TrafficLights.append(TrafficLightObject)

                idx = idx + 1
      
        return TrafficLights

    def UpdateSignalPhases(self, TrafficLights):
        
        i = 0
        #Проходим через каждый сигнальный перекресток
        for ID in self.LightIDs:
            LightPhases = traci.trafficlight.getRedYellowGreenState(ID) #string

            # Просматриваем строку, содержащую состояние каждого сигнального контроллера, и обновляем объекты светофора
            for State in LightPhases:
                TrafficLights[i].DecodeTrafficPhase(State)
                i = i+1

        return TrafficLights
