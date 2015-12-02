
if param("fs") is None: 
	param("fs",1000,"Hz","Sampling rate")



def nsinusoid(f,time_end = 1.0,phase = 0.0,fs = -1,set_fs = True):
	"""Generates a sinusoid of given length in time scale"""
	overSampRate=31.5
	if fs == -1:
		fs = overSampRate * f 
	if set_fs:
		param("fs",fs)
		# param("f",f)
	tmp = 0
	t = []
	while tmp < time_end:
		t.append(tmp)
		tmp += (1.0/fs)
	x = [sin(2.0*pi*f*ti+phase) for ti in t]
	param("tscale",t)
	return (x,t)


def sinusoid(f,cycles = 1,phase = 0,set_fs = True):
	"""Generates a sinusoid of given frequency with given number of cycles"""
	overSampRate=31.5
	fs = overSampRate * f
	# param("fs",fs)
	# param("f",f)
	T = 1.0/f
	tmp = 0
	t = []
	while tmp < (cycles * T):
		t.append(tmp)
		tmp += (1.0/fs)
	x = [sin(2.0*pi*f*ti+phase) for ti in t]
	return (x,t)