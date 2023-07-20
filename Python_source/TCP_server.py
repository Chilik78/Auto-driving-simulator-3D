import socket
import time
import Unity


class TCP_Server(object):
    def __init__(self, IP, port):
        self.IP = IP
        self.port = port
        self.Num_Listener = 1 #Кол-вол слушателей, то есть клиентов

        #Создать объект сокета
        self.ServerSocket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.ServerSocket.setblocking(True)
        s_adr = (self.IP, self.port)
        self.ServerSocket.bind(s_adr)
        #self.ServerSocket.settimeout(120)

        #Определить клиентов
        self.UnityClient = []
        self.UnityAddress = []
        self.UnityRunning = False
        self.UnityThread = []

        print("Starting TCP server at ",  self.IP, ":", self.port)
    
    def StartServer(self, UnityQueue):

        self.ServerSocket.listen(self.Num_Listener) #Прослушиваем

        i = 0
        #Ждем всех клиентов
        while i < self.Num_Listener:
            tmpClient, tmpAddress = self.ServerSocket.accept() #Принимаем соединение
            ClientName = '00000' #Начальное имя
            print("Клиент: ", ClientName) 
            while ClientName == '00000': #Пока не получим код от клиента
                try: #Пытаемся получить данные и присвоить их клиенту, иначе пауза в 0.01
                    ClientName = tmpClient.recv(5) 
                    print("Клиент: ", ClientName)
                except:
                    time.sleep(0.01)
                    
            #Заказ клиентов
            if ClientName == 'U3D00'.encode('utf8'): #Если клиент отправил запрос на получение данных
                print("Connection from: Unity 3D")
                self.UnityClient = tmpClient #Передаём в переменную сокет, по которому можно передавать данные на клиент
                self.UnityAddress = tmpAddress #Просто так сохраняем адрес, он никак не нужен
                self.UnityRunning = True #Указываем, что клиент готов получать данные
                i = i + 1 #Счётчик клиентов, отправивших запрос
                time.sleep(1)

                #Начать передачу в поток Unity здесь!
                self.UnityError, self.UnityThread = Unity.StartUnity(self.UnityClient, UnityQueue)
            else:
                tmpClient.close() #Закрываем соединение
                print("ERROR! Check the clients and retry!")
                # sys.exit(1)

    def ReopenSocket(self, UnityQueue):

        if self.UnityError.isSet(): #Если поток закрылся, то возвращаем True и убиваем сервер в Main
            
            return True
            #Убейте поток, если есть ошибка, сбросьте ошибку
            self.UnityThread.join()
            self.UnityError.clear()
            self.UnityRunning = False

            #Открыть 1 соединение и дождаться соответствующего клиентаt
            self.ServerSocket.listen(1)
            tmpClient, tmpAddress = self.ServerSocket.accept()
            ClientName = '00000'

            while ClientName == '00000':
                try:
                    ClientName = tmpClient.recv(5)
                    print(ClientName)
                except:
                    time.sleep(0.01)

            if ClientName == 'U3D00'.encode('utf8'):
                print("Connection from: Unity 3D")
                self.UnityClient = tmpClient
                self.UnityAddress = tmpAddress
                time.sleep(1)

                #Начать передачу в поток Unity здесь!
                self.UnityError, self.UnityThread = Unity.StartUnity(self.UnityClient, UnityQueue)
                self.UnityRunning = True
            else:
                
                #Брось соединение и жди нужного
                tmpClient.close()
                
        
        return False

    def CloseSocket(self):
        self.ServerSocket.close()