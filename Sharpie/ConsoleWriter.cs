using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpie
{
    public class ConsoleWriter
    {

        public bool printToConsole(Model m)
        {
            if (m.Active)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
