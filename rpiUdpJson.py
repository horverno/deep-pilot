#!/usr/bin/python

# Raspberry Pi script which recieves and sends UDP packets from the PC

import socket
import json

UDP_IP = "192.168.0.1"
UDP_PORT = 5006


sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM) # UDP
sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
sock.bind((UDP_IP, UDP_PORT))

print("Listening... (waiting for exit message)")

data = ""
while (data != 'exit'.encode('UTF-8')):
    data, addr = sock.recvfrom(1024) # buffer size is 1024 bytes
    print("Received:", data)

print("Exit message recieved")
