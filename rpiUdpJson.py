#!/usr/bin/python

# -*- coding: utf-8 -*-
"""
Created on Thu Jul  6 08:52:28 2017
@author: horverno (github.com/horverno)
Python version: 3.4.2
"""

# Raspberry Pi script which recieves UDP and sends UDP + TCP packets from the PC
# API reference: https://github.com/horverno/deep-pilot/wiki/API-reference

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
idMotDrive = 12 # ID of the wheel (drive) motor
axMot = Ax12()


sockR = socket.socket(socket.AF_INET, socket.SOCK_DGRAM) # UDP
sockR.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1) # UDP
sockR.bind(('', udpPortRead)) # UDP
sockS = socket.socket(socket.AF_INET, socket.SOCK_DGRAM) # UDP


def sendMsg():
    global exitFlag, sendDelay, sendDict, axMot, idMotSteering, idMotDrive
    startTime = time.time()
    #print(" >> Write message thread started.")
    while not exitFlag:
        time.sleep(sendDelay)
        elapsedTime = time.time() - startTime
        sendDict["Time"] = elapsedTime
        #print(json.dumps(sendDict))
        if "MotorPositionDrive" in sendDict:
            sendDict["MotorPositionDrive"] = axMot.readPosition(idMotDrive)
        sockS.sendto((json.dumps(sendDict)).encode('utf-8'), (udpIpSend, udpPortSend))
    #print(" >> Write message thread stopped.")

def readMsg():
    global exitFlag, udpIpSend, sendDelay, sendDict, axMot, idMotSteering, idMotDrive
    #print("Read message thread started.")
    while not exitFlag:
        #print("r %d" %i)
        #time.sleep(2)
        readData, addr = sockR.recvfrom(1024) # buffer size is 1024 bytes
        print("R: %s" % readData)
        try:
            readDict = json.loads(str(readData,'utf-8'))
            for d in readDict:
                    if("Exit" == d):
                            exitFlag = True
                    if("SetUdpIpSend" == d):
                            udpIpSend = readDict["SetUdpIpSend"]
                            print("New ip: %s" % (udpIpSend))
                    if("SetDelay" == d):
                            sendDelay = float(readDict["SetDelay"])
                            print(" >> New frequency: %.2f" % (1 / sendDelay))
                    if("SetSpeedSignalReferenceDrive" == d):
                            axMot.setMovingSpeed(idMotDrive, readDict["SetSpeedSignalReferenceDrive"])
                            print(" >> New wehicle speed: %.1f" % (readDict["SetSpeedSignalReferenceDrive"]))
                    if("SetAngleReferenceSteering" == d):
                            axMot.move(idMotSteering, readDict["SetAngleReferenceSteering"])
                            print(" >> New steering angle: %.1f" % (readDict["SetAngleReferenceSteering"]))            
                    if("SendMotorPosition" == d):
                            if readDict["SendMotorPosition"] == True:
                                    sendDict["MotorPositionDrive"] = 0
                                    print(" >> Sending motor position too...")
                            else:
                                    try:
                                            del sendDict["MotorPositionDrive"]
                                            break
                                    except:
                                            print(" >> Error deleting MotorPositionDrive")
                                    print(" >> Stopped sending motor position...")            
                    if("SendLapNumber" == d):
                            if readDict["SendLapNumber"] == True:
                                    sendDict["LapNumber"] = 0
                                    print(" >> Sending LapNumber too...")
                            else:
                                    try:
                                            del sendDict["LapNumber"]
                                            break
                                    except:
                                            print(" >> Error deleting LapNumber")
                                    print(" >> Stopped sending LapNumber...")
                    if("SendCamera1" == d):
                            if readDict["SendCamera1"] == True:
                                    #snap1 = snapShotImage()
                                    #sendDict["Camera1"] = str(pygame.image.tostring(snap1, "RGBA", True))
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
        except:
            print(" >> Not valid JSON sting recieved.")
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
axMot.setAngleLimit(idMotDrive, 0, 0)
axMot.setAngleLimit(idMotSteering, 0, 1023)
s = threading.Thread(target=sendMsg)
r = threading.Thread(target=readMsg)
s.start()
r.start()
s.join() # block until all tasks are done
r.join() # block until all tasks are done      
axMot.setMovingSpeed(idMotDrive, 0) # stop the motor
print("Exit message recieved")
