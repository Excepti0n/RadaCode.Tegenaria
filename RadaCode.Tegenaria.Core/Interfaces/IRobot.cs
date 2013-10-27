using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadaCode.Tegenaria.Core.Interfaces
{
    public interface IRobot
    {
        ISet<string> spiderMe(Uri url);
    }
}
