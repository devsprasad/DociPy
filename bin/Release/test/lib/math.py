

from math import pi
import math

def nearest(x,vals = []):
	vals_d = base.sub(vals,x)
	n = min(abs(vals_d))
	if n in vals_d:	return vals_d.index(n)

def conj(x):
	if isinstance(x,list):
		return [complex(n).conjugate() for n in x]
	else:
		return complex(x).conjugate()
_abs = abs
def abs(x):
	if isinstance(x,list) : return [_abs(i) for i in x]
	else: return _abs(x)
_round = round 
def round(x):
	if isinstance(x,list) : return [_round(i) for i in x]
	else: return _round(x)	
def sin(x):
	if isinstance(x,list) : return [math.sin(i) for i in x]	
	else: return math.sin(x) 
def cos(x):
	if isinstance(x,list) : return [math.cos(i) for i in x]	
	else: return math.cos(x) 
def tan(x):
	if isinstance(x,list) : return [math.tan(i) for i in x]	
	else: return math.tan(x) 

def trapz(fx,a=0,b=-1):
	"""Integrate a function using trapezoidal rule"""
	return base.trapz(fx,a,b)

def pnorm(fx,p = 2.0):
	"""Calculate p-norm of a vector. Matrix is not supported"""
	# refer: http://www.gaussianwaves.com/2013/12/computation-of-power-of-a-signal-in-matlab-simulation-and-verification/
	fx_powp = [pow(abs(x),p) for x in fx]
	pnorm = pow(sum(fx_powp),1.0/p)
	return pnorm