from DociPy import AbstractDocEngine, PyColumbus
from DociPy.PyTypes import *
import System
from System.ComponentModel import *




class _HTMLDocGen(object):

	__project_title = "MyProject"
	__project_version = "1.0.0"
	__config = {}
	__styles = []
	__themeDir = ""
	__output = ""
	__template_data = ""
	__log = None

	def __init__(self,title, version, theme_dir, output_dir,log_func):
		super(_HTMLDocGen, self).__init__()
		self.__themeDir = theme_dir
		self.__output = output_dir
		self.__createThemeConfig()
		self.__log = log_func
		self.__project_title =title
		self.__project_version = version

	def __relativeStylePath(self,target,exact_file):
		fi = System.IO.FileInfo(exact_file)
		target = target.replace('\\', '/')
		count = len(target.split('/')) - 2;
		rel_path = System.IO.Path.Combine("..\\" * count, "styles",fi.Name)
		return rel_path

	def __createStyles(self, html_file):
		styles = ""
		for style in self.__styles:
			if(style.startswith(r"https://") or 
				style.startswith(r"http://")):
				rel = style
			else:
				fi = System.IO.FileInfo(style)
				out = self.__output + "\\styles\\" + fi.Name
				self.__copyFile(style,out)
				rel = self.__relativeStylePath(html_file,out )
			styles += "<link rel=\"stylesheet\" type=\"text/css\" href=\"%s\">\n" % rel
		return styles

	def __copyFile(self, src, dst):
		if not System.IO.File.Exists(dst):
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
				if(css.startswith(r"https://") or 
					css.startswith(r"http://")):
					self.__styles.append(css)
				else:
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
			d = d.replace("[methods]", self.__wrapFunctions(_class.Methods))
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
		html = html.replace("[styles]", self.__createStyles(dst_path))
		html = html.replace("[project_title]", self.__project_title)
		html = html.replace("[project_version]", self.__project_version)
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
	__project_title = "MyProject"
	__project_version = "1.0.0"

	def __init__(self):
		super(EngineDynaThemer, self).__init__()
		self.__output_path = "docs"
		self.__parser = PyColumbus()

	@property
	def ProjectTitle(self):
	    return self.__project_title

	@ProjectTitle.setter
	def ProjectTitle(self,val):
		self.__project_title = val

	@property
	def ProjectVersion(self):
	    return self.__project_version

	@ProjectVersion.setter
	def ProjectVersion(self,val):
		self.__project_version = val

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
			self.__htmlgen = _HTMLDocGen(self.ProjectTitle, self.ProjectVersion,
			 self.__theme_path, self.__output_path, self.log)
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
			self.log("finished.")
		else:
			self.log("invalid root path: %s\n" % self.__root_path)


