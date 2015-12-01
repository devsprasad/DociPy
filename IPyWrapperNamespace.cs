
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Diagnostics;
using Microsoft.Scripting.Hosting;
using IronPython.Hosting;
using System.Reflection;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using System.Windows.Forms;

namespace DociPy
{
    public class IPyEngine
    {

        #region "PublicVars"
        public Microsoft.Scripting.Hosting.ScriptScope EngineScope;
        public ScriptEngine Engine;
        #endregion

        public ScriptRuntime Runtime;
        #region "PrivateVars"
        private Microsoft.VisualBasic.Devices.Computer my = new Computer();
        private System.IO.Stream _io;
        #endregion
        private Microsoft.Scripting.Hosting.ScriptSource gScript;

        #region "Basic"
        /// <summary>
        /// Creates new instance of IPy engine with given stream
        /// </summary>
        /// <param name="iostream">stream object to be used for I/O</param>
        /// <remarks></remarks>
        public IPyEngine(System.IO.Stream iostream, bool AddExecutingAssembly = true)
        {
            this.Engine = Python.CreateEngine();
            Runtime = this.Engine.Runtime;
            EngineScope = this.Engine.CreateScope();
            _io = iostream;
            SetStreams(_io);
            if (AddExecutingAssembly)
            {
                Runtime.LoadAssembly(Assembly.GetExecutingAssembly());
            }
        }

        /// <summary>
        /// Sets output and error stream handles to the given 'stream' object
        /// </summary>
        /// <param name="stream">Stream object to use</param>
        /// <remarks>This function will not change the Input stream of the engine. 
        /// Input stream will be set to the stream object used during the initialization 
        /// of the class</remarks>
        public void SetStreams(System.IO.Stream stream)
        {
            Runtime.IO.SetInput(_io, Encoding.UTF8);
            Runtime.IO.SetOutput(stream, Encoding.UTF8);
            Runtime.IO.SetErrorOutput(stream, Encoding.UTF8);
        }

        /// <summary>
        /// Returns Python style traceback of the given exception object
        /// </summary>
        /// <param name="e">exception handle</param>
        /// <returns>String representing the traceback information</returns>
        public object getTraceback(Exception e)
        {
            ExceptionOperations eo = Engine.GetService<ExceptionOperations>();
            return eo.FormatException(e);
        }

        /// <summary>
        /// Adds a .NET Dynamic Link Library(DLL) to Python scope
        /// </summary>
        /// <param name="path">Fully qualified file path</param>
        /// <remarks>To add a directory containing multiple DLL files, use LoadAssemblies()</remarks>
        public void AddAssembly(string path)
        {
            Assembly pluginsAssembly = Assembly.LoadFile(path);
            Runtime.LoadAssembly(pluginsAssembly);
        }

        /// <summary>
        /// Run the script and import all the variables/functions/objects to scope
        /// </summary>
        /// <param name="path">path to python script</param>
        /// <returns>True if file found and loaded, false otherwise</returns>
        /// <remarks>No errors will be raised in case the file does not exist</remarks>
        public bool LoadScript(string path)
        {
            if (my.FileSystem.FileExists(path))
            {
                gScript = Engine.CreateScriptSourceFromFile(path);
                gScript.Compile();
                gScript.Execute(EngineScope);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds all the .NET Dynamic Link Libraries(DLL) availble in the taget
        /// dir to Python scope
        /// </summary>
        /// <param name="dir">Folder containing DLL files</param>
        /// <remarks>No errors will be raised</remarks>
        public void LoadAssemblies(string dir)
        {
            dynamic fList = my.FileSystem.GetFiles(dir, Microsoft.VisualBasic.FileIO.SearchOption.SearchAllSubDirectories, "*.dll");
            foreach (string dll_file in fList)
            {
                AddAssembly(dll_file);
            }
        }

        /// <summary>
        /// Loads init.py and all the DLLs from a directory
        /// </summary>
        /// <param name="path">path to the library folder</param>
        /// <remarks></remarks>
        public void LoadLibs(string path)
        {
            if (my.FileSystem.FileExists(path + "./init.py"))
            {
                LoadAssemblies(path);
                path += "./init.py";
                LoadScript(path);
            }
        }

        #endregion

        #region "User"

        public object PyExecute(string source, Microsoft.Scripting.SourceCodeKind kind = Microsoft.Scripting.SourceCodeKind.Expression)
        {
            Microsoft.Scripting.Hosting.ScriptSource src = default(Microsoft.Scripting.Hosting.ScriptSource);
            src = Engine.CreateScriptSourceFromString(source, kind);
            object res = src.Execute(EngineScope);
            return res;
        }

        #endregion

        #region "ScopeMgmt"
        public bool IsDefined(string name)
        {
            return EngineScope.ContainsVariable(name);
        }

        public object getFunction(string name)
        {
            object p = EngineScope.GetVariable(name);
            if ((p != null))
            {
                if (Engine.Operations.IsCallable(p))
                {
                    return p;
                }
            }
            return null;
        }

        public void ClearScope()
        {
            EngineScope = Engine.CreateScope();
        }
        #endregion

    }

    internal class CustomStream : MemoryStream
    {

        Object _output;
        private byte[] buffer;
        private int offset;

        private int count;

        public CustomStream(object textbox)
        {
            if (textbox == null)
            {
                textbox = new Object();
                Debug.Print("not a valid stream object");
            }
            _output = textbox;
        }


        private void _write()
        {
            string s = Encoding.UTF8.GetString(buffer, offset, count);
            if (_output.GetType() == typeof(TextBox))
            {
                ((TextBox)_output).AppendText(s);
            }
            else if (_output.GetType() == typeof(RichTextBox))
            {
                ((RichTextBox)_output).AppendText(s);
            }
            else if (_output.GetType() == typeof(SimbedEnvisionCodeEditor.InteractiveCommandEditor))
            {
                ((dynamic)_output).AppendText(s);
            }
            else
            {
                Debug.Print(s);
            }
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            this.buffer = buffer;
            this.offset = offset;
            this.count = count;
            _write();
        }
    }
}


