# This file is a sample for testing and understanding DociPy's 
# documentation capabilities. This comment will be considered
# as the documentation or description for the module sample1.py
# itself. And this whole comment will be rendered as a single line. 
# So styling need to be done accordingly to wrap the lines. But html
# tags can override the parsers and create a new line. For flexibility
# the parser and templating engines are separate. Also, a Python 
# script containing a custom parser and templating engine can be 
# presented to the tool. These will be loaded dynamically and appear 
# as a list on GUI.



# this is a `function` of the module. 
# Arguments can be described using @param directive.
# Example usage is shown below. 
# @param:arg0:first argument of the function
# @param:arg1:second argument of the function
def test(arg0,arg1):
	"""this docstring will be ignored by the default parser.
	however, a custom parser can still parse this!!!"""
	pass


# A test class.
# This class only inherits from the concrete class
# 'object'. 
class Test(object):
	def __init__(self):
		super(Test, self).__init__()

	# This is called a `method`. Same function documenting
	# rules can be used here as well. 
	# @param:arg0:just another argument
	def test(self, arg0):
		pass

# Yet another class example.
# This class inherits from `Test` class.  
class TestClass(object, Test):
 	"""docstring for TestClass will be ignored"""
 	def __init__(self, arg):
 		super(TestClass, self).__init__()
 		self.arg = arg

 	# no documentation.
 	# just kidding.
 	def Test(self):
 		return self


 		 


