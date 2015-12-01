using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DociPy
{
    static class Program
    {

        public static IPyEngine ipy;
        public static CustomStream cstream;
        public static IPyBaseBridge ipy_bridge;
        public static void PyInit(CustomStream stream)
        {
            cstream = stream;
            ipy = new IPyEngine(cstream);
            ipy_bridge = new IPyBaseBridge();
            ipy.EngineScope.SetVariable("ipy", ipy_bridge);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new mainForm());
        }
    }
}
