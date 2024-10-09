using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telerik.JustMock.DemoLib
{
    public class DemoLibLocalFunctions
    {
        public int Method()
        {
            return 5 + Local();
            int Local()
            {
                return 10;
            }
        }
    }
}
