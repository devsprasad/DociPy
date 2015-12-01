using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DociPy
{
    public class IPyBaseBridge
    {

        public bool ShortErrorMessage { get; set; }

        public IPyBaseBridge()
        {
            this.ShortErrorMessage = true;
        }
    }
}
