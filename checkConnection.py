# -*- coding: utf-8 -*-
"""
Created on Thu Jul  6 08:52:28 2017

@author: horverno (github.com/horverno)

Python version: 3.5.2
"""

import sys
import subprocess
from subprocess import Popen, PIPE
'''
subprocess.call('netsh WLAN show profiles')
subprocess.call('netsh interface ip show addresses "Wi-Fi")
subprocess.call('netsh wlan connect ssid=ePiWifi name=ePiWifi')
'''
                
p = Popen('netsh WLAN show pro-files', stdin=PIPE, stdout=PIPE, stderr=PIPE)
output, err = p.communicate()
rc = p.returncode
#print(str(output, 'utf-8'))
if "ePiWifi" in str(output, 'utf-8'):
    p = Popen('netsh wlan connect ssid=ePiWifi name=ePiWifi', stdin=PIPE, stdout=PIPE, stderr=PIPE)
    output, err = p.communicate()
    rc = p.returncode
    print("Connected to ePiWifi")
    
    p = Popen("netsh interface ip show addresses \"Wi-Fi\"", stdin=PIPE, stdout=PIPE, stderr=PIPE)
    output, err = p.communicate()
    rc = p.returncode
    for item in str(output, 'utf-8').split("\n"):
        if "IP Address" in item:
            print(item.strip())   
else:
    print("No ePiWifi profile found :(")
    
