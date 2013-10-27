using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;
using RadaCode.Tegenaria.Core;
using RadaCode.Tegenaria.Core.Interfaces;

namespace RadaCode.Tegenaria.Ninject
{
    public class ServiceModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ILocStore>().To<DefaultLocStore>().InSingletonScope();
            Bind<IRobot>().To<Robot>().InThreadScope();
            Bind<IXmlGenerator>().To<XmlGenerator>().InThreadScope();
        }
    }
}
