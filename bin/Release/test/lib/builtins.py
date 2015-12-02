import clr
clr.AddReference("System")
clr.AddReference("mscorlib")
import System,alglib

def chunks(seq, n):
    return list(seq[i:i+n] for i in xrange(0, len(seq), n))

def nchunks(seq, size):
	input_size = len(seq)
	slice_size = input_size / size
	remain = input_size % size
	result = []
	iterator = iter(seq)
	for i in range(size):
	    result.append([])
	    for j in range(slice_size):
	        result[i].append(iterator.next())
	    if remain:
	        result[i].append(iterator.next())
	        remain -= 1
	return result

def func_sig(f):
	name = f.func_name
	var = list(f.func_code.co_varnames)
	defs = list(f.func_defaults)
	var.reverse()
	defs.reverse()
	for i in range(len(defs)):
		var[i] = "%s = %s"  % (var[i],defs[i])
	var.reverse()
	defs.reverse()
	return "%s(%s)" % (name,",".join(var))


def netarray(_list,_type = int):
	return System.Array[_type](_list)

def antilog(x,log_base = 10):
	return pow(log_base, x)

def dBtoNum(dbVal):
	dbVal = dbVal / 10
	return antilog(dbVal) 

def todB(val):
	return 10*math.log10(val)

def linspace(start,stop,num = 10):
	ret = []
	tmp = start
	step = (stop-start)/(num-1) 
	while tmp < stop:
		ret.append(tmp)
		tmp += step
	return ret


def plot(x,**args):
	if "y" in args.keys():
		y = args["y"]
	else:
		y = range(0,len(x))
	if "title" in args: 
		title = args["title"]
	else: 
		title = "(no title)"
	if "xlabel" in args: 
		xlabel = args["xlabel"]
	else: 
		xlabel = "x-axis"
	if "ylabel" in args: 
		ylabel = args["ylabel"]
	else: 
		ylabel = "y-axis"
	f = sim.plotting.figure()
	f.xlabel(xlabel)
	f.ylabel(ylabel)
	f.title(title)
	f.plot(title,y,x)


def pad(x,max_len,pad_with = 0):
	if isinstance(x,list):
		while len(x) < max_len:
			x.append(pad_with)
	return x


def randombin(L):
	"""Generates a random sequence of binary digits"""
	r = [int(x) for x in round(base.div(base.rand(L-1,0,100),100))]
	return r

def doc(obj):
	if hasattr(obj, '__doc__'):
		print(obj.__doc__)
	else:
		help(obj)

def wgn(p,l):
	"""Generates White Gaussian Noise"""
	r = base.rand(l)
	if l <= 1 : r = [r]
	r = [p*x for x in r]
	return r
