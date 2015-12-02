from DociPy import AbstractDocEngine, PyColumbus
from DociPy.PyTypes import *
import System
from System.ComponentModel import *


_twitter_format =   { 
						"module":""	
					}

class _TwitterFormat(object):

	def __init__(self):
		super(_TwitterFormat, self).__init__()

	def substitue(self,html = ""):
		for key,value in _twitter_format:
			html = 	html.replace("class='%s'" % key, "class='%s'" % value)
		return html
		



class _HTMLDocGen(object):

	__config = {}
	__styles = []
	__themeDir = ""
	__output = ""
	__template_data = ""

	def __init__(self,theme_dir, output_dir):
		super(_HTMLDocGen, self).__init__()
		self.__themeDir = theme_dir
		self.__output = output_dir
		self.__createThemeConfig()

	def __relativeStylePath(self,target,exact_file):
		fi = System.IO.FileInfo(exact_file)
		target = target.replace('\\', '/')
		count = len(target.split('/')) - 3;
		rel_path = System.IO.Path.Combine("..\\" * count, "styles",fi.Name)
		return rel_path

	def __createStyles(self):
		styles = ""
		for style in self.__styles:
			fi = System.IO.FileInfo(style)
			out = self.__output + "\\styles\\" + fi.Name
			self.__copyFile(style,out)
			rel = self.__relativeStylePath(out, style)
			styles += "<link rel=\"stylesheet\" type=\"text/css\" href=\"%s\">" % rel
		return styles

	def __copyFile(self, src, dst):
		fi = System.IO.FileInfo(dst)
		_dir = fi.Directory
		System.IO.Directory.CreateDirectory(_dir.FullName)
		System.IO.File.Copy(src, dst)

	def __createFile(self,file_path, data=""):
		fi = System.IO.FileInfo(file_path)
		_dir = fi.Directory
		System.IO.Directory.CreateDirectory(_dir.FullName)
		f = open(file_path,"w")
		f.write(data)
		f.close()

	def __createThemeConfig(self):
		path = self.__themeDir + "\\theme.config"
		f = open(path,"r")
		data = f.readlines()
		f.close()
		f= open(self.__themeDir + "\\template.html")
		self.__template_data = f.read()
		f.close()
		key = ""
		for line in data:
			line = str(line)
			if(line.startswith("@")):
				key = line[1:].strip("\n").strip()
				self.__config[key] = ""
			elif line.startswith("$style:"):
				css = str(line[len("$style:"):]).strip()
				self.__styles.append(System.IO.Path.Combine(self.__themeDir, css))
			else:
				if key in self.__config.keys():
					self.__config[key] += line

	def __wrapArguments(self, args):
		html = ""
		for arg in args:
			d = self.__config["arg"]
			d = d.replace("[arg_name]", arg.Name)
			d = d.replace("[arg_info]", str(arg.Description))
			d = d.replace("[arg_default]", str(arg.Default))
			html += d
		return html

	def __wrapFunctions(self, functions):
		html = ""
		for func in functions:
			d = self.__config["function"]
			d = d.replace("[function_name]", func.Name)
			d = d.replace("[function_info]", func.Description)
			d = d.replace("[arguments]", self.__wrapArguments(func.Args))
			html += d
		return html

	def __wrapClasses(self, _classes):
		html = ""
		for _class in _classes:
			d = self.__config["class"]
			d = d.replace("[class_name]", _class.Name)
			d = d.replace("[class_info]", _class.Description)
			html += d
		return html

	def __wrapModule(self, module):
		html = self.__template_data
		mod = self.__config["module"]
		mod = mod.replace("[module_name]", module.Name)
		mod = mod.replace("[module_info]", module.Description)
		mod = mod.replace("[classes]", self.__wrapClasses(module.Classes))
		mod = mod.replace("[functions]", self.__wrapFunctions(module.Functions))
		html = html.replace("[module]",mod)
		return html


	def generate(self,module,dst_path):
		html = self.__wrapModule(module)
		html = html.replace("[styles]", self.__createStyles())
		self.__createFile(dst_path, html)
		return html


		

class EngineDynaThemer(AbstractDocEngine):

	__output_path = ".\docs\\"
	__root_path =""
	__theme_path = ""
	__parser = None
	__htmlgen = None
	__useBootstrap = True
	__classFirst = True

	def __init__(self):
		super(EngineDynaThemer, self).__init__()
		self.__output_path = "docs"
		self.__parser = PyColumbus()

	@property
	def Theme(self):
	    return self.__theme_path

	@Theme.setter
	def Theme(self,val):
		self.__theme_path = val

	@property
	def OutputDirectory(self):
	    return self.__output_path

	@OutputDirectory.setter
	def OutputDirectory(self,val):
		self.__output_path = val


	@property
	def RootDirectory(self):
		return self.__root_path

	@RootDirectory.setter
	def RootDirectory(self,val):
		self.__root_path = val

	def __invokeParser(self,files):
		pymodules = []
		for _file in files:
			src_path = System.IO.Path.Combine(self.__root_path, _file)
			self.log("processing %s" % str(_file).split("\\")[-1])
			pymodules.append(self.__parser.Process(src_path))
			self.log("... done\n")
		self.log("done parsing scripts. No errors found\n")
		return pymodules


	def Generate(self, files):
		if System.IO.Directory.Exists(self.__root_path):
			self.__htmlgen = _HTMLDocGen(self.__theme_path, self.__output_path)
			self.log("starting..\n")
			self.log("creating output directory..")
			System.IO.Directory.CreateDirectory(self.__output_path)
			self.log("done\n")
			pymods = self.__invokeParser(files)
			self.log("generating html files...\n")
			for mod in pymods:
				self.log("generating %s.html ..." % mod.Name.split("\\")[-1])
				src_path = str(mod.Name).replace(self.__root_path + "\\","")
				dst_path = System.IO.Path.Combine(self.__output_path, src_path) + ".html"
				mod.Name = str(mod.Name).replace(self.__root_path, "")
				self.__htmlgen.generate(mod,dst_path)
				self.log("done\n")
		else:
			self.log("invalid root path: %s\n" % self.__root_path)


