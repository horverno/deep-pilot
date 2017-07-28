#!/usr/bin/python

# -*- coding: utf-8 -*-
"""
Created on Fri Jul 28 08:08:28 2017
@author: horverno (github.com/horverno)
Python version: 3.4.2
"""

import io
import socket
import struct
import time
import picamera
import picamera.array


tcpIpSend = "0.0.0.0" #socket.gethostname()#"192.168.1.90" #
tcpPortSend = 5009

print("tcp.server %s:%d" % (tcpIpSend, tcpPortSend))
#snap1 = snapShotImage()
snap1 = bytes([0x00, 0x00, 0x56, 0x45, 0x74, 0x44, 0x76, 0x46, 0x3F, 0xD0, 0x00, 0x08 ])
sockT = socket.socket()#(socket.AF_INET, socket.SOCK_STREAM)

sockT.bind((tcpIpSend, tcpPortSend))#((tcpIpSend, tcpPortSend))
sockT.listen(5)
sockT.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
sockTconn, addr = sockT.accept()
print('Connection estabilished! ', addr)
while True:
    time.sleep(0.01)
    try:
        with picamera.PiCamera() as camera:
            with picamera.array.PiRGBArray(camera) as output:
                camera.resolution = (160, 128)
                camera.capture(output, 'rgb')
                print('Captured %dx%d image' % (output.array.shape[1], output.array.shape[0]))
                snap1 = output.array
                sockTconn.send(("B" + str(output.array.shape[1]*output.array.shape[0]*3).zfill(16) + "E").encode('ascii')) # 18 bytes to send (and read on the PC side)
                sockTconn.send(snap1) # TCP
    except:
        print("Exception: maybe broken pipe, closing connection")
        sockTconn.close()
        break


