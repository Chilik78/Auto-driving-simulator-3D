import socket

def client():
    addr = 'localhost'
    port = 4042

    s = socket.create_connection((addr, port))
    s.setblocking(True)

    res = s.send(bytes('U3D00'.encode('utf8')))
    
    srt = s.recv(16384*2) 
    print(str(srt)[:5])
    print("b\'O1G")
    print(str(srt))
    while True:
        print(s.recv(16384))
        # if str(srt)[:5] == "b\'O1G":
        #     vehs = str(srt).split('@')
        #     for veh in vehs:
        #         sp = veh.split(';')
        #         print(f'id: {sp[0]} | class: {sp[-1]}')

client()