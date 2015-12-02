# This module gives basic testing framework for DociPy.
# This module tests all the features that the DociPy
# has.


# this is a test function with arguments.
# arguments have no default value here.
# this is a test function with arguments.
# arguments have no default value here.
# this is a test function with arguments.
# arguments have no default value here.
# @param:arg0:0th argument
# @param:arg1:1st argument
def test(arg0,arg1):
	pass


# this is another test function with arguments
# too. But this function has arguments with 
# default values.
# @param:arg0:0th argument
# @param:arg1:1st argument [default = 0]
def test2(arg0, arg1=0):
	pass


# this is a class.
# functions that occur inside the 
# class are called methods.
class Test(object):

	# initializer method. 
	# In default documentation engine,
	# this is considered just as a method even though
	# it is a constructor
	def __init__(self, arg):
		super(Test, self).__init__()
		self.arg = arg
	
	# a function which returns the self
	# object. takes no arguments.
	def myself(self):
		return self