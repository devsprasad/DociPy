def sigEnergy(fx,pinf = 1000, ninf = -1000):
	"""Calculate signal energy"""
	fx_square = [abs(x)**2 for x in fx]
	return sum(fx_square)

def xcorr(x,y,n = -1,m = -1):
	"""Cross-Correlation of two sequences. 
Syntax x = xcorr(x,y)"""
	x = System.Array[float](x)
	y = System.Array[float](y)
	if n == -1: n = len(x)
	if m == -1: m = len(y)
	l = list(alglib.corrr1d(y,n,x,m))
	f = l[:n][::-1]
	s = l[n:][::-1]
	return f+s


def autocorr(x):
	"""Auto-Correlation of a sequence
Syntax: c = autocorr(x) 
Example: c = autocorr(sinusoid(10,2))"""
	return xcorr(x,x)


def conv(x,y,n = -1,m = -1):
	"""Convolution of x and y
Syntax: x (+) y = conv(x,y)"""
	x = System.Array[float](x)
	y = System.Array[float](y)
	if n == -1: n = len(x)
	if m == -1: m = len(y)
	return alglib.convr1d(x,n,y,n)

def fft(xt,N = 1024):
	"""Fast-Fourier Transform of xt. N is by default is 1024"""
	xt = pad(xt,N)
	c = alglib.fftr1d(System.Array[float](xt),N)
	ret = [complex(n.x,n.y) for n in c]
	return ret

def ifft(Xf,N = 1024):
	"""Inverse of Fast-Fourier Transform of Xf. N is by default 1024"""
	Xf = System.Array[alglib.complex]([alglib.complex(c.real,c.imag) for c in Xf])
	return alglib.fftr1dinv(Xf,N)

def fftshift(Xf):
	"""Shifts the Xf vector to center"""
	l = len(Xf)
	l2 = l/2
	xf1 = Xf[0:l2]
	xf2 = Xf[l2:]
	xf = xf2 + xf1
	return xf

def DFTplot(xt,N = 1024, fs = -1):
	"""Plots DTFT vs Frequency(Hz) of xt"""
	if fs == -1 : fs = param("fs")
	if fs is None: fs = 1000
	Xf = abs(fftshift(fft(xt,N)))
	Tf = base.div(range(-N/2+1,N/2),N)
	Tf = base.mul(Tf,fs)
	p = sim.plotting.plot("",Tf,Xf)
	p.title("Frequency-Domain Plot")
	p.xlabel("Frequency (Hz)")
	p.ylabel("DTFT Values")

def freqvalues(xt,N = 1024,fs = -1,PSD = False):
	if fs == -1 : fs = param("fs")
	if fs is None: fs = 1000
	Px = abs((fft(xt,N)))
	if PSD : Px = [10*math.log10(x/(N*len(xt))) for x in Px]	
	else: Px = [x/(N*len(xt)) for x in Px]	
	Tf = base.div(range(0,N/2),N)
	Tf = base.mul(Tf,fs)
	return (Px[0:N/2],Tf)

def freqplot(xt,N=1024,fs = -1,PSD = False,**args):
	"""Plots Power vs Frequency(Hz) of xt if PSD = False
Plots PSD (dB) vs Frequency(Hz) of xt """
	if fs == -1 : fs = param("fs")
	if fs is None: fs = 1000
	Px = abs((fft(xt,N)))
	if PSD : Px = [10*math.log10(x/(N*len(xt))) for x in Px]	
	else: Px = [x/(N*len(xt)) for x in Px]	
	Tf = base.div(range(0,N/2),N)
	Tf = base.mul(Tf,fs)
	p = sim.plotting.figure()
	p.plot("",Tf,Px[0:N/2])
	p.title(args.get("title") or "Frequency-Domain Plot")
	p.xlabel(args.get("xlabel") or "Frequency (Hz)")
	p.ylabel(args.get("ylabel") or "Power")
	return p

def frequency(xt,fs = -1,N = 1024):
	if fs == -1 : fs = param("fs")
	if fs is None: fs = 1000
	Xf = abs(fftshift(fft(xt,N)))
	Tf = base.div(range(-N/2,N/2),N)
	Tf = base.mul(Tf,fs)
	f = round(abs(Tf[Xf.index(max(Xf))]))
	return f


def peaks(xt,threshold = 0,keys = None):
	for i in range(len(xt)):
		x = xt[i]
		if not keys is None and x > threshold: 
			yield (x,keys[i])
		elif x > threshold: 
			yield x 