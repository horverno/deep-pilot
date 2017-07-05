#!/usr/bin/python

# Raspberry Pi script which recieves UDP and sends UDP + TCP packets from the PC

import socket
import json
import threading
import time
import pygame, sys
import pygame.camera
from pygame.locals import *
from ax12 import Ax12

udpIpRead = "192.168.0.1"
udpIpSend = "192.168.0.10" #"255.255.255.255" # broadcast
tcpIpSend = "192.168.0.10" #
udpPortRead = 5006
udpPortSend = 5005
tcpPortSend = 5008
tcpBufSize = 1024
exitFlag = False
sendDict = {"Time": 0}
sendDelay = 0.5 # 2 Hz by default
width1 = 384
height1 = 288
idMotSteering = 2 # ID of the steering wheel motor
idMotWheel = 12 # ID of the wheel motor
axMot = Ax12()


sockR = socket.socket(socket.AF_INET, socket.SOCK_DGRAM) # UDP
sockR.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1) # UDP
sockR.bind(('', udpPortRead)) # UDP
sockS = socket.socket(socket.AF_INET, socket.SOCK_DGRAM) # UDP


def sendMsg():
    global exitFlag, sendDelay, sendDict, axMot, idMotSteering, idMotWheel
    startTime = time.time()
    #print(" >> Write message thread started.")
    while not exitFlag:
        time.sleep(sendDelay)
        elapsedTime = time.time() - startTime
        sendDict["Time"] = elapsedTime
        #print(json.dumps(sendDict))
        if "Motor_Position" in sendDict:
            sendDict["Motor_Position"] = axMot.readPosition(idMotWheel)
        sockS.sendto((json.dumps(sendDict)).encode('utf-8'), (udpIpSend, udpPortSend))
    #print(" >> Write message thread stopped.")

def readMsg():
    global exitFlag, udpIpSend, sendDelay, sendDict, axMot, idMotSteering, idMotWheel
    #print("Read message thread started.")
    while not exitFlag:
        #print("r %d" %i)
        #time.sleep(2)
        readData, addr = sockR.recvfrom(1024) # buffer size is 1024 bytes
        print("R: %s" % readData)
        #try:
        readDict = json.loads(str(readData,'utf-8'))
        for d in readDict:
            if("Exit" == d):
                exitFlag = True
            if("udpIpSend" == d):
                udpIpSend = readDict["udpIpSend"]
                print("New ip: %s" % (udpIpSend))
            if("sendDelay" == d):
                sendDelay = float(readDict["sendDelay"])
                print(" >> New frequency: %.2f" % (1 / sendDelay))
            if("Speed_Signal_Reference" == d):
                axMot.setMovingSpeed(idMotWheel, readDict["Speed_Signal_Reference"])
                print(" >> New wehicle speed: %.1f" % (readDict["Speed_Signal_Reference"]))
            if("Steering_Wheel_Angle_Reference" == d):
                axMot.move(idMotSteering, readDict["Steering_Wheel_Angle_Reference"])
                print(" >> New steering angle: %.1f" % (readDict["Steering_Wheel_Angle_Reference"]))            
            if("Motor_Position" == d):
                if readDict["Motor_Position"] == True:
                    sendDict["Motor_Position"] = 0
                    print(" >> Sending motor position too...")
                else:
                    try:
                        del sendDict["Motor_Position"]
                        break
                    except:
                        print(" >> Error deleting Motor_Position")
                    print(" >> Stopped sending motor position...")            
            if("LapNumber" == d):
                if readDict["LapNumber"] == True:
                    sendDict["LapNumber"] = 0
                    print(" >> Sending LapNumber too...")
                else:
                    try:
                        del sendDict["LapNumber"]
                        break
                    except:
                        print(" >> Error deleting LapNumber")
                    print(" >> Stopped sending LapNumber...")
            if("Camera1" == d):
                if readDict["Camera1"] == True:
                    snap1 = snapShotImage()
                    sendDict["Camera1"] = str(pygame.image.tostring(snap1, "RGBA", True))
                    #sendDict["Camera1"] = "ff"
                    #print(pygame.image.tostring(snap1, "RGBA", True))
                    print(" >> Sending cam image[1] too via TCP...")
                else:
                    try:
                        del sendDict["Camera1"]
                        break
                    except:
                        print(" >> Error deleting Camera1 entry")
                    print(" >> Stopped sending cam image[1]...")        
            break
        #except:
            #print(" >> Not valid JSON sting recieved.")
        #print(" >> Read message thread stopped.")


def snapShotImage():
    global width1, height1
    cam1 = pygame.camera.Camera("/dev/video2",(width1,height1))
    cam1.start()
    image = cam1.get_image()
    return image

# main

print("Listening... (waiting for exit message)")
pygame.init() # camera
pygame.camera.init() # camera
# Motor endless turn - If both values for the CW Angle Limit and the CCW Angle Limit are set to 0, an Endless Turn mode can be implemented by setting the Moving Speed
axMot.setAngleLimit(idMotWheel, 0, 0)
axMot.setAngleLimit(idMotSteering, 0, 1023)
s = threading.Thread(target=sendMsg)
r = threading.Thread(target=readMsg)
s.start()
r.start()
s.join() # block until all tasks are done
r.join() # block until all tasks are done      
axMot.setMovingSpeed(idMotWheel, 0) # stop the motor
print("Exit message recieved")
