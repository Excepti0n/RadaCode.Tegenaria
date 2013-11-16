using System;
using System.IO;
using System.Reflection;
using Ninject;
using RadaCode.Tegenaria.Core.Interfaces;
using RadaCode.Tegenaria.Ninject;

namespace RadaCode.Tegenaria
{
    class Program
    {
        private static IKernel _kernel;
        
        static internal void SetupKernel()
        {
            _kernel = new StandardKernel(new ServiceModule());

        }

        static void Main(string[] args)
        {
            SetupKernel();

            Console.Write("Enter the link: ");
            var url = Console.ReadLine();
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";

            var robot = _kernel.Get<IRobot>();
            var siteMapXmlGenerator = _kernel.Get<IXmlGenerator>();

            siteMapXmlGenerator.GetSiteMapXml(new Uri(url), path, robot.spiderMe(new Uri(url)));

            Console.WriteLine(String.Format("Crawling completed. {0} pages have been indexed.", siteMapXmlGenerator.IndexedPagesCount));
            Console.ReadKey();

        }

        
    }
}
