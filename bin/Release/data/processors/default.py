from DociPy import AbstractDocEngine, PyColumbus
from DociPy.PyTypes import *
import System




class _HTMLDocGen(object):


	def __init__(self):
		super(_HTMLDocGen, self).__init__()

	def __processArgs(self, Args):
		html = ""
		for arg in Args:
			html += "<div class='arg'><label class='arg-name'>" + arg.Name + "</label>"
			if arg.Description: html += "<p class='arg-info'>" + arg.Description + "</p>"
			if arg.Default != None: html += "<label class='arg-default'>" + arg.Default + "</label>"
			html += "</div>"
		return html

	def __processMethods(self,methods):
		html = ""
		for method in methods:
			html += "<div class='method'><label class='method-name'>" + method.Name+ "</label>"
			html += "<p class='method-info'>" + method.Description + "</p>"
			html += "<div class='args'><label class='arg-head'>Arguments</label>"
			html += self.__processArgs(method.Args)
			html += "</div></div>"
		return html

	def __processClasses(self,classes):
		html = ""
		for _class in classes:
			html += "<div class='class'><label class='class-name'>" + _class.Name + "</label>"
			html += "<p class='class-info'>" + str(_class.Description) + "</p>"
			html += "<div class='methods'><label class='method-head'>Methods</label>"
			html += self.__processMethods(_class.Methods)
			html += "</div></div>"
		return html

	def __processFunctions(self,functions):
		html = ""
		for function in functions:
			html += "<div class='function'><label class='function-name'>" + function.Name+ "</label>"
			html += "<p class='function-info'>" + function.Description + "</p>"
			html += "<div class='args'><label class='arg-head'>Arguments</label>"
			html += self.__processArgs(function.Args)
			html += "</div></div>"
		return html


	def __processModule(self,mod):
		html = "<div class='module'><label class='module-name'>" + str(mod.Name)  + "</label>"
		html += "<p class='module-info'>" + str(mod.Description) + "</p>"
		html += "<div class='functions'><label class='function-head'>Functions</label>"
		html += self.__processFunctions(mod.Functions)
		html += "<div class='classes'><label class='class-head'>Classes</label>"
		html += self.__processClasses(mod.Classes)
		html += "</div></div></div>"
		return html

	def _invoke(self,module, theme_file):
		html = "<html><head><title></title>"
		html += "<link rel=\"stylesheet\" type=\"text/css\" href=\"%s\">" % theme_file
		html += "</link></head><body>"
		html += str(self.__processModule(module))
		html += "</body></html>"
		return html
		

class EngineDefaultThemer(AbstractDocEngine):

	_output_path = ".\docs\\"
	_root_path =""
	_theme_path = ""
	_parser = PyColumbus()
	_htmlgen = _HTMLDocGen()
	Test = ""

	def __init__(self):
		super(EngineDefaultThemer, self).__init__()
		self._output_path = "docs"
	
	@property
	def Theme(self):
	    return self._theme_path

	@Theme.setter
	def Theme(self,val):
		self._theme_path = val

	@property
	def OutputDirectory(self):
	    return self._output_path

	@OutputDirectory.setter
	def OutputDirectory(self,val):
		self._output_path = val

	@property
	def RootDirectory(self):
	    return self._root_path

	@RootDirectory.setter
	def RootDirectory(self,val):
		self.log("RootDirectory changed")
		self._root_path = val

	def __invokeParser(self,files):
		pymodules = []
		for _file in files:
			src_path = System.IO.Path.Combine(self._root_path, _file)
			dst_path = System.IO.Path.Combine(self._output_path, _file)
			self.log("processing %s" % str(_file).split("\\")[-1])
			pymodules.append(self._parser.Process(src_path))
			self.log("... done\n")
		self.log("done parsing scripts. No errors found\n")
		return pymodules

	def __createFile(self,file_path, data=""):
		fi = System.IO.FileInfo(file_path)
		_dir = fi.Directory
		System.IO.Directory.CreateDirectory(_dir.FullName)
		f = open(file_path,"w")
		f.write(data)
		f.close()


	# private string find_relative_path(string target, string exact_file)
 #        {
 #            string rel_path = "";
 #            
 #            
 #            
 #            for (int i = 0 ; i < count ; i++)
 #            {
 #                rel_path += "../";
 #            }
 #            
 #            rel_path = rel_path.Replace("\\", "/");
 #            return rel_path;
 #        }
	def __relativeStylePath(self,target,exact_file):
		fi = System.IO.FileInfo(exact_file)
		target = target.replace('\\', '/').replace("\\","/").strip('/');
		count = len(target.split('/')) - 2;
		rel_path = System.IO.Path.Combine("../" * count, "styles",fi.Name)
		return rel_path


	def Generate(self, files):
		if System.IO.Directory.Exists(self._root_path):
			self.log("starting..\n")
			pymods = self.__invokeParser(files)
			self.log("generating html files...\n")
			style = open(self._theme_path).read()
			style_path = self._output_path + "\\styles\\style.css"
			self.__createFile(style_path, style)
			for mod in pymods:
				self.log("generating %s.html ..." % mod.Name.split("\\")[-1])
				src_path = str(mod.Name).replace(self._root_path + "\\","")
				dst_path = System.IO.Path.Combine(self._output_path, src_path) + ".html"
				css =  self.__relativeStylePath(dst_path, style_path)
				html = self._htmlgen._invoke(mod,css)
				self.__createFile(dst_path,html)
				self.log("done\n")
		else:
			self.log("invalid root path: %s\n" % self._root_path)


