import traci

#Класс, описывающий светофоры
class TrafficLight(object):

    #Атрибут класса, словарь фаз для декодирования
    PhaseDict = {
        "o": 0,  # off
        "O": 0,  # off
        "g": 1,  # green
        "G": 1,  # green
        "y": 2,  # yellow
        "Y": 2,  # yellow
        "r": 3,  # red
        "R": 3,  # red
    }

    #Конструктор
    def __init__(self, ID, Lane, idx, LightPositionX, LightPositionY):

        self.ID = ID #Id светофора
        self.LaneID = Lane #Полоса движения
        self.Index = idx #Индекс, не используется(Полезная хрень, которая могла бы пригодиться)
        self.PosX = LightPositionX #X координата
        self.PosY = LightPositionY #Y координата
        #Получите долготу и широту положения каждой светофора.
        #self.__TransformGPS()

        self.CurrentPhase = 0 #Фаза (красный, жёлтый, зелёный)

    #Преобразует систему координат SUMO X-Y в GPS
    def __TransformGPS(self):
        self.Lon, self.Lat = traci.simulation.convertGeo(self.PosX, self.PosY)

    #Декодирует текущую фазу
    def DecodeTrafficPhase(self, CurrentPhaseRaw):
        self.CurrentPhase = self.PhaseDict[CurrentPhaseRaw]
