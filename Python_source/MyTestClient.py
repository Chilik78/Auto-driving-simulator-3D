import socket
from TrafficLight import TrafficLight

def client():
    addr = 'localhost'
    port = 4042

    s = socket.create_connection((addr, port))
    s.setblocking(True)

    res = s.send(bytes('U3D00'.encode('utf8')))
    
    print(res)
    

    srt = s.recv(16384)

    srt = s.recv(16384)
    string = str(srt)
    string = string[3:-2]
    print(string)
    strings = string.split('@')
    TrafficLights = []
    for sr in strings:
        fields = sr.split(';')
        tf = TrafficLight(fields[0], fields[1], 0, float(fields[2]), float(fields[3]))
        tf.CurrentPhase = int(fields[4])
        TrafficLights.append(tf)
    
    for tf in TrafficLights:
        print(f'Id: {tf.ID} Lane:{tf.LaneID} Ph: {tf.CurrentPhase}')
    
client()