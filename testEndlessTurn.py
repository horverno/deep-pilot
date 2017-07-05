# python 3.4
from ax12 import Ax12
import time

IdS = 2
IdW = 12
pos0 = -999
pos1 = -999
pos2 = -999
load2 = -999

self = Ax12()

'''
#print("setBaud:", end="")
#self.setBaudRate(15, 9600)
#time.sleep(0.01)
self.setAngleLimit(IdW, 0, 1023)
time.sleep(0.1)

#print("ping ", end="")
self.ping(IdW)
time.sleep(0.01)
pos0 = self.readPosition(IdW)
print("pos0: %d   " %pos0, end="")

#print("move1:  ", end="")
self.move(IdW,312)
time.sleep(2)
pos1 = self.readPosition(IdW)
print("pos1: %d   " %pos1, end="")

#print("move2:  ", end="")
self.moveSpeed(IdW,712, 100)
time.sleep(0.1)
load2 = self.readLoad(IdW)
print("load2: %d   " % load2, end="")
time.sleep(2)
pos2 = self.readPosition(IdW)
print("pos2: %d   " %pos2, end="")
'''
# Endless Turn - If both values for the CW Angle Limit and the CCW Angle Limit are set to 0, an Endless Turn mode can be implemented by setting the Moving Speed
self.setAngleLimit(IdW, 0, 0)
time.sleep(0.5)
self.setMovingSpeed(IdW, 200)
#print("move3:  ", end="")
#self.move(15,10)
#time.sleep(0.01)
#self.move(IdW,512)


print("\nend ----")
