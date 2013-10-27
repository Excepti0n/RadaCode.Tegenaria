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
        private static IKernel kernel;
        

        static void Main(string[] args)
        {
            string url;
            Console.WriteLine("Enter the link:");
            url = Console.ReadLine();
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
            kernel = new StandardKernel(new ServiceModule());
            var robot = kernel.Get<IRobot>();
            var siteMap = kernel.Get<IXmlGenerator>();
            siteMap.GetSiteMapXml(new Uri(url), path, robot.spiderMe(new Uri(url)));
            
        }

        
    }
}
