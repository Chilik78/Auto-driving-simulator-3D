import math
import traci

#Класс, описывающий двигающиеся объекты
class SumoObject(object):

    #Конструктор
    def __init__(self, SumoID):
        self.ID = str(SumoID)  #Идентификатор транспортного средства
        try:
            self.ObjType = traci.vehicle.getTypeID(self.ID) #Тип объекта
            self.Route = traci.vehicle.getRouteID(self.ID) #Направление
            self.Edge = traci.vehicle.getRoadID(self.ID) #Id дороги
            self.Length = traci.vehicle.getLength(self.ID) #Длина
            self.Width = traci.vehicle.getWidth(self.ID) #Ширина

            if traci.vehicle.getSignals(self.ID) & 8 == 8: #Bitmask - 8 for brake light
                self.StBrakePedal = True
            else:
                self.StBrakePedal = False

            tmp_pos = traci.vehicle.getPosition(self.ID)  # position: x,y
            self.PosX_FrontBumper = tmp_pos[0] # X position (front bumper, meters)
            self.PosY_FrontBumper = tmp_pos[1] # Y position (front bumper, meters)
            self.Velocity = traci.vehicle.getSpeed(self.ID) #Скорость
            self.Heading = traci.vehicle.getAngle(self.ID) #Угол

            self.__CalculateSizeClass() #self.SizeClass
            self.__CalculateCenter() #self.PosX_Center, self.PosY_Center (center, meters)

        except:
            print("Error creating container for SUMO vehicle: ", self.ID)

    #Обновить состояние объекта
    def UpdateVehicle(self):

        try:
            if traci.vehicle.getSignals(self.ID) & 8 == 8: #Bitmask - 8 для стоп-сигнала
                self.StBrakePedal = True
            else:
                self.StBrakePedal = False

            tmp_pos = traci.vehicle.getPosition(self.ID)  # position: x,y
            self.PosX_FrontBumper = tmp_pos[0]  # X position (front bumper, meters)
            self.PosY_FrontBumper = tmp_pos[1]  # Y position (front bumper, meters)
            self.Velocity = traci.vehicle.getSpeed(self.ID) #Скорость 
            self.Heading = traci.vehicle.getAngle(self.ID) #Угол
            self.Edge = traci.vehicle.getRoadID(self.ID) #Id дороги

            self.__CalculateCenter()  # self.PosX_Center, self.PosY_Center (center, meters)

        except:
            print("Error updating SUMO vehicle: ", self.ID)
            
            #Попытка вернуть его
            self.ReinsertVehicle()

    #Перезасосывает машины в симулцию
    def ReinsertVehicle(self):

        LaneIndex = 1  # дурачок
        KeepRouteMode = 1  #KeepRoute: 2 = свободное перемещение.

        try:
            traci.vehicle.add(self.ID, self.Route)  #Попытка вернуть его
        except:
            pass #Если уже есть, ничего не делать
        try:
            traci.vehicle.moveToXY(self.ID, self.Edge, LaneIndex, self.PosX_FrontBumper, self.PosY_FrontBumper, self.Heading, KeepRouteMode)
            traci.vehicle.setSpeed(self.ID, self.Velocity)
        except:
            print("Error reinserting SUMO vehicle: ", self.ID)

    #Преобразует систему координат SUMO X-Y в GPS
    def __TransformGPS(self, PosX, PosY):
        Lon, Lat = traci.simulation.convertGeo(PosX, PosY)
        return Lon, Lat

    #Получает координаты центра относительно переднего бампера
    def __CalculateCenter(self):
        self.PosX_Center = self.PosX_FrontBumper - (math.sin(math.radians(self.Heading)) * (self.Length / 2))
        self.PosY_Center = self.PosY_FrontBumper - (math.cos(math.radians(self.Heading)) * (self.Length / 2))

    #Определяет категорию транспортного средства в зависимости от его размера
    def __CalculateSizeClass(self):

        type = self.ObjType[:self.ObjType.index('_')]
        
        if type == 'bike':
            self.SizeClass = 1 #Велик
        elif type == 'motorcycle':
            self.SizeClass = 2 #Моцик
        elif type == 'veh':
            self.SizeClass = 3 #Ауди Влада
        elif type == 'bus': 
            self.SizeClass = 4 #Автобус
        elif type == 'truck':
            self.SizeClass = 5 #Грузовик
        else:
            self.SizeClass = 0 #Ничего или пешеход


