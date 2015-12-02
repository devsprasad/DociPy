
from NovaLogic import AnalogOut
__analogOut = AnalogOut([],param("fs"),0.5)

ANALOG_MAXLEN = 2000000  

def writeOut(buff = [],fs = -1,wait = True):
	global __analogOut,ANALOG_MAXLEN
	if fs == -1:
		fs = param("fs")
		if fs is None or fs == -1: fs = param("f")*30
	__analogOut.AutoPlayOnUpdate = True
	if len(buff)  > ANALOG_MAXLEN:
		sim.status("finished with errors")
		err("max buffer length reached")
	else:
		x = __analogOut.Update(buff,fs)
		while wait and __analogOut.IsPlaying(): pass
		return x


def writeStop():
	global __analogOut
	__analogOut.Stop()

