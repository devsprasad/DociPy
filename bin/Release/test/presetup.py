# ===========================================================
# ==> DO NOT MESS WITH THIS FILE  <==
# ==> DO NOT MODIFY THIS FILE UNLESS YOU KNOW WHAT EXACTLY
# 	  YOU ARE DOING..
# ==> MISCONFIGURING THIS FILE WILL RENDER THE SIMULATION
# 	  SYSTEM UNUSABLE.
# ===========================================================


sim._log("pre-setup..")

def getmodels(_type):
	return list(models.getModels(_type))

_dir = dir
def dir(obj):
	props = _dir(obj)
	props = [x for x in props if not x.startswith("__") and not x.startswith("_") ]
	return props

_help = help
def help(topic = None):
	if topic is None:
		print(open("./core/documentation.txt","r").read())
	else:
		_help(topic)

def __getProps(obj):
	props = dir(obj)
	props = [x for x in props if not x.startswith("__") and not x.startswith("_") ]
	return props


# ==============================
import sys
sys.path.append("./Lib")
sys.path.append("./core")
# ==============================


import NovaLogic

base = NovaLogic.BaseCore 
models = sim.ModelsControl 

# DO NOT DELETE THIS VARIABLE EVER
# This variables holds all the model types along with their descriptions
# model_types = {
# 					# Tx side
# 					"tformat":"Formatter block",
# 					"srcenc":"Source Encoder block (optional)",
# 					"encrypt":"Encryption block (optional)",
# 					"chaenc":"Channel encoder",
# 					"pulsemod": "Pulse modulator",
# 					"modulator":"Bandpass modulator",
# 					"fspread" : "Frequency spreading",
# 					# Tx-Rx interface
# 					"channel":"Transmission Channel models",
# 					# Rx side
# 					"fdspread":"Frequency de-spread",
# 					"demodulator":"bandpass demodulator",
# 					"detector":"Pulse modulation detector",
# 					"chadec" : "Channel decoder",
# 					"decrypt": "Decyption unit",
# 					"srcdec" : "Source decoder",
# 					"rformat": "symbol to bitstream format",
# 					"sim":"simulation setups"
# 			  }
model_types = {
				"encoder":"Encodes the binary data with an appropriate encoding method",
				"decoder":"Decodes the given encoded binary sequence",
				"channel":"Characterizes the channel and its behavior",
				"modulator":"modulates the signal",
				"demodulator":"demodulates the signal",
				"noise":"estimates and characterizes the behavior of noise",
				"attenuation":"estimates the attenuation of signal",
				"speed":"estimates the accurate speed of signal using different models",
				"sim":"simulation setups"
			  }


import thread
def simCall(func,args = ()):
	"""DO NOT MESS WITH THIS"""
	# global __simThread
	# if __simThread:
	# 	thread.start_new(func,tuple(args))
	# else:
	func(*args)


def param(name,value = None,unit = None,description="",category = "unknown"):
	if value is None and unit is None and description == "":
		return sim.parameters.getParam(name)
	elif unit is None and description == "" and sim.parameters.paramExists(name):
		p = sim.parameters.getParamTuple(name)
		unit = p[1] ; description = p[2] ; category = p[3]
	t = [value,unit,description,category]
	sim.parameters.setParam(name,t)

def ask(msg,_type="info"):
	return sim.askPrompt(str(msg),"",_type)
def info(msg):
	sim.msg(str(msg),"","info")
def warn(msg):
	sim.msg(str(msg),"","warn")
def err(msg):
	sim.msg(str(msg),"","err")

sim._log("pre-setup finished.")


def how_to_use(feature =""):
	if feature == "":
		print(open("./core/docs/howtouse.txt").read())
	else:
		return "no docs for %s" % feature
