# basic default parameters
parameters = {
				"fs":(1000,"samples/sec","Sampling rate",""),
				"tx":([],"volts","modulated data to be transmitted","Signal"),
				"f":(10,"Hz","Primary frequency",""),
				"data":("1010","bits","Data to be sent",""),
				"rx":([],"volts","received signal",""),
			 }

for par in parameters:
	x = list(parameters[par])
	sim.parameters.setParam(par,x)

fs = f * 30.0


# core libraries
# must be loaded after parameter setup
base.__load__("./core/lib/builtins.py")
base.__load__("./core/lib/math.py")
base.__load__("./core/lib/signal_gen.py")
base.__load__("./core/lib/signal_proc.py")
base.__load__("./core/lib/analogIO.py")
base.__load__("./core/lib/digital.py")
base.__load__("./core/lib/pyc.py")