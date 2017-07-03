#!/usr/bin/python

# Raspberry Pi script which recieves and sends UDP packets from the PC

import socket
import json
import threading
import time

udpIpRead = "192.168.0.1"
udpIpSend = "192.168.0.10" #
udpPortRead = 5006
udpPortSend = 5005
exitFlag = False
sendDict = {"Time": 0}
sendDelay = 0.5 # 2 Hz by default

sockR = socket.socket(socket.AF_INET, socket.SOCK_DGRAM) # UDP
sockR.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
sockR.bind(('', udpPortRead))
sockS = socket.socket(socket.AF_INET, socket.SOCK_DGRAM) # UDP

def sendMsg():
    global exitFlag
    global sendDelay
    startTime = time.time()
    print("Write message thread started.")
    while not exitFlag:
        time.sleep(sendDelay)
        elapsedTime = time.time() - startTime
        sendDict["Time"] = elapsedTime
        print(json.dumps(sendDict))        
        sockS.sendto((json.dumps(sendDict)).encode('utf-8'), (udpIpSend, udpPortSend))
    print("Write message thread stopped.")

def readMsg():
    global exitFlag
    global udpIpSend
    global sendDelay
    global sendDict
    print("Read message thread started.")
    while not exitFlag:
        #print("r %d" %i)
        #time.sleep(2)
        readData, addr = sockR.recvfrom(1024) # buffer size is 1024 bytes
        print("R: %s" % readData)
        readDict = json.loads(str(readData,'utf-8'))
        for d in readDict:
            if("Exit" == d):
                exitFlag = True
            if("udpIpSend" == d):
                udpIpSend = readDict["udpIpSend"]
                print("New ip: %s" % (udpIpSend))
            if("sendDelay" == d):
                sendDelay = float(readDict["sendDelay"])
                print("New frequency: %.2f" % (1 / sendDelay))
            if("LapNumber" == d):
                if readDict["LapNumber"] == True:
                    sendDict["LapNumber"] = 0
                    print("Sending LapNumber too...")
                else:
                    try:
                        del sendDict["LapNumber"]
                        break
                    except:
                        print("Error deleting LapNumber")
                    print("Stopped sending LapNumber...")
        print("Read message thread stopped.")


# main

print("Listening... (waiting for exit message)")
s = threading.Thread(target=sendMsg)
r = threading.Thread(target=readMsg)
s.start()
r.start()
s.join() # block until all tasks are done
r.join() # block until all tasks are done      

print("Exit message recieved")
