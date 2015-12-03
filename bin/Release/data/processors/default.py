from DociPy import AbstractDocEngine, PyColumbus
from DociPy.PyTypes import *
import System
from System.ComponentModel import *
from System import *



# Generates HTML files with given configuration options for the 
# list of modules. Creates appropriate output directories. Copies
# stylesheet files as mentioned in the theme.config file.
# <label class='label-warning'>Not intended to be used directly.</label> 
class _HTMLDocGen(object):

	__project_title = "MyProject"
	__project_version = "1.0.0"
	__config = {}
	__styles = []
	__themeDir = ""
	__output = ""
	__root = ""
	__module_template_data = ""
	__index_template_data = ""
	__log = None
	__project_info = ""
	__recreate_root = False
	__include_module_doc = False
	__options = {}


	# @param:options: A dictionary containing all the options.
	# @param:log_func: A function handle which will be called as a log function
	def __init__(self, options, log_func):
		super(_HTMLDocGen, self).__init__()
		self.__project_title = options["project"]["title"]
		self.__project_version = options["project"]["version"]
		self.__project_info = options["project"]["info"]
		self.__root  =options["project"]["root"]
		self.__output = options["project"]["docs_dir"]
		self.__recreate_root = options["config"]["recreate_root"]
		self.__themeDir = options["config"]["theme"]
		self.__include_module_doc = options["config"]["include_moduledoc"]
		self.__show_private_members = options["config"]["show_private_members"]
		self.__log = log_func
		self.__createThemeConfig()

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
		self.__styles[:] = []
		path = self.__themeDir + "\\theme.config"
		f = open(path,"r")
		data = f.readlines()
		f.close()
		f= open(self.__themeDir + "\\module.html")
		self.__module_template_data = f.read()
		f.close()
		f = open(self.__themeDir + "\\index.html")
		self.__index_template_data = f.read()
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
			d = self.__config["args"]
			d = d.replace("[arg_name]", arg.Name)
			d = d.replace("[arg_info]", str(arg.Description))
			d = d.replace("[arg_default]", str(arg.Default))
			html += d
		if html == "": html = "(no arguments)"
		return html

	def __createArgTuple(self, args):
		html = []
		for arg in args:
			s = self.__config["arg"]
			s = s.replace("[arg_name]", arg.Name)
			if arg.Default !=None: s = s.replace("[arg_default]", "= %s" % arg.Default)
			else: s = s.replace("[arg_default]","")
			html.append(s)
		return ",".join(html)


	__properties = {}

	def __isAsetter(self, line):
		if line.startswith("@param"):
			return False
		if line.startswith("@"):
			n = line[1:str(line).index(".")]
			if n in self.__properties.keys(): return True
		return False

	def __wrapProperties(self):
		html = ""
		for prop in self.__properties.values():
			func = prop[0]
			_get = prop[1].replace("true","visible")
			_get = _get.replace("false","not-visible")
			_set = prop[2].replace("true","visible")
			_set = _set.replace("false","not-visible")
			d = self.__config["property"]
			d = d.replace("[property_name]", func.Name)
			d = d.replace("[property_info]", func.Description)
			d = d.replace("[property_set]", _set)
			d = d.replace("[property_get]", _get)
			html += d
		self.__properties.clear()
		if html == "": return "(no properties)"
		return html

	def __wrapFunctions(self, functions, method = False, className = ""):
		html = ""
		if method : sub = "method"
		else: sub = "function"
		for func in functions:
			d = self.__config["%s" % sub]
			if func.Name == "__init__" and method: 
				func.Name = className 
				d = self.__config["constructor"]
			elif func.Name == "__del__" and method:
				func.Name = "~" + className
				d = self.__config["destructor"]
			else:
				prop = False
				for line in func.RawDocLine:
					if line.startswith("@property"):
						self.__properties[func.Name] = [func, "true", "false"]
						prop = True
						break
					elif self.__isAsetter(line):
						if func.Name in self.__properties.keys():
							self.__properties[func.Name][2] = "true"
							prop = True
							break
				if prop : continue
			if not self.__show_private_members:
				if (func.Name.startswith("__") and not func.Name.endswith("__")):
					d = ""
			d = d.replace("[%s_name]" % sub, func.Name)
			d = d.replace("[%s_info]" % sub, func.Description)
			d = d.replace("[arg_tuple]",str(self.__createArgTuple(func.Args)))
			d = d.replace("[arguments]",self.__wrapArguments(func.Args) )
			html += d
		if html == "": 
			if method: html = "(no methods)"
			else: html = "(no functions)"
		return html

	def __wrapClasses(self, _classes):
		html = ""
		for _class in _classes:
			d = self.__config["class"]
			d = d.replace("[class_name]", _class.Name)
			d = d.replace("[class_info]", _class.Description)
			d = d.replace("[methods]", 
				self.__wrapFunctions(_class.Methods,True,_class.Name))
			d = d.replace("[properties]", self.__wrapProperties())
			bases = ""
			for base in _class.BaseClasses:
				_b = self.__config["parent_class"]
				_b =_b.replace("[base]", base)
				bases += _b
			d  =d.replace("[bases]", bases)
			html += d
		if html == "": html = "(no classes)"
		return html

	def __wrapModule(self, module):
		html = self.__module_template_data
		mod = self.__config["module"]
		mod = mod.replace("[module_name]", module.Name)
		mod = mod.replace("[module_info]", module.Description)
		mod = mod.replace("[classes]", self.__wrapClasses(module.Classes))
		mod = mod.replace("[functions]", self.__wrapFunctions(module.Functions))
		html = html.replace("[module]",mod)
		return html


	def __process_template(self,html,dst_path):
		html = html.replace("[styles]", self.__createStyles(dst_path))
		html = html.replace("[project_title]", self.__project_title)
		html = html.replace("[project_version]", self.__project_version)
		html = html.replace("[project_description]", " ".join(self.__project_info))
		return html


	def __process(self, module,dst_path):
		html = self.__wrapModule(module)
		html = self.__process_template(html,dst_path)
		self.__createFile(dst_path, html)
		return dst_path


	__classes = []
	def __wrapIndexModule(self, module,module_path, include_doc = True):
		name = str(module.Name).strip("\\").replace("\\",".").replace(".py","")
		self.__classes.append((name, list(module.Classes),module_path))
		if include_doc: data = str(module.Description)
		else: data = ""
		html = self.__config["indexmodule"]
		html = html.replace("[module_name]",name)
		html = html.replace("[module_info]", data)
		html = html.replace("[module_path]", module_path)
		return html

	def __wrapIndexClasses(self):
		html = ""
		if len(self.__classes) == 0 : 
			return "(no classes)"
		for module,class_list, module_path in self.__classes:
			for _class in class_list:
				d = self.__config["indexclass"]
				d = d.replace("[class_name]", module + "." + _class.Name)
				d = d.replace("[module_path]", module_path)
				html += d
		if html == "" : html  ="(no classes)"
		self.__classes[:] =[ ]
		return html


	# Generate the HTML files for the modules
	# @param:modules:List of <code>PyModule</code> objects
	def generate(self,modules):
		html = ""
		for mod in modules:
			self.__log("generating %s.html ..." % mod.Name.split("\\")[-1])
			src_path = str(mod.Name).replace(self.__root + "\\","")
			if not self.__recreate_root: src_path = src_path.replace("\\",".")
			dst_path = System.IO.Path.Combine(self.__output, src_path) + ".html"
			mod.Name = str(mod.Name).replace(self.__root, "")
			dst_path = self.__process(mod,dst_path)
			self.__log("done\n")
			html += self.__wrapIndexModule(mod,dst_path.replace(self.__output,".\\"), self.__include_module_doc) + "\n"
		index_file = self.__output + "\\index.html"
		index = self.__process_template(self.__index_template_data, index_file)
		html = index.replace("[modules]", html)
		html = html.replace("[classes]", self.__wrapIndexClasses())
		self.__createFile(index_file, html)


		

class EngineHTML(AbstractDocEngine):
	__output_path = ".\docs\\"
	__root_path =""
	__theme_path = ""
	__parser = None
	__htmlgen = None
	__useBootstrap = True
	__classFirst = True
	__project_title = "MyProject"
	__project_version = "1.0.0"
	__project_description = Array[str]([])
	__root_path_recreate = False
	__include_mod_doc_in_index = True
	__show_private_members = False

	def __init__(self):
		super(EngineHTML, self).__init__()
		self.__output_path = "docs"
		self.__parser = PyColumbus()


	# title of the project
	@property
	def ProjectTitle(self):
	    return self.__project_title

	@ProjectTitle.setter
	def ProjectTitle(self,val):
		self.__project_title = val

	@property
	def ProjectDescription(self):
	    return self.__project_description

	@ProjectDescription.setter
	def ProjectDescription(self,val):
		self.__project_description = val

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

	@property
	def RecreateRootStructure(self):
		return self.__root_path_recreate

	# @RecreateRootStructure.setter
	# def RecreateRootStructure(self,val):
	# 	self.__root_path_recreate = val

	@property
	def IncludeModuleDocInIndex(self):
		return self.__include_mod_doc_in_index

	@IncludeModuleDocInIndex.setter
	def IncludeModuleDocInIndex(self,val):
		self.__include_mod_doc_in_index = val

	@property
	def ShowPrivateMembers(self):
		return self.__show_private_members

	@ShowPrivateMembers.setter
	def ShowPrivateMembers(self,val):
		self.__show_private_members = val


	def __invokeParser(self,files):
		pymodules = []
		for _file in files:
			src_path = System.IO.Path.Combine(self.__root_path, _file)
			self.log("processing %s" % str(_file).split("\\")[-1])
			pymodules.append(self.__parser.Process(src_path))
			self.log("... done\n")
		self.log("done parsing scripts. No errors found\n")
		return pymodules

	def __getOptions(self):
		options = {}
		options["project"] = {}
		options["config"] = {} 
		options["project"]["title"] = self.ProjectTitle
		options["project"]["version"] = self.ProjectVersion
		options["project"]["info"] = self.ProjectDescription
		options["project"]["root"] = self.__root_path
		options["project"]["docs_dir"] = self.__output_path
		options["config"]["theme"] = self.__theme_path
		options["config"]["recreate_root"] = self.RecreateRootStructure
		options["config"]["include_moduledoc"] = self.IncludeModuleDocInIndex
		options["config"]["show_private_members"] = self.ShowPrivateMembers
		return options


	def Generate(self, files):		
		if System.IO.Directory.Exists(self.__root_path):
			self.__htmlgen = _HTMLDocGen(self.__getOptions(), self.log)
			self.log("starting..\n")
			self.log("creating output directory..")
			System.IO.Directory.CreateDirectory(self.__output_path)
			self.log("done\n")
			pymods = self.__invokeParser(files)
			self.log("generating html files...\n")
			self.__htmlgen.generate(pymods)
			self.log("finished.")
		else:
			self.log("invalid root path: %s\n" % self.__root_path)


