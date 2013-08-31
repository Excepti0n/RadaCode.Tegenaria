using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Ninject;

namespace RadaCode.Tegenaria
{
    class Program
    {
        private static IKernel kernel;

        static void Main(string[] args)
        {
            Console.WriteLine("");
            Program program = new Program();
            program.SiteMapBuilder(Console.ReadLine());

        }

        public void SiteMapBuilder(string url)
        {
            WebRequest myWebRequest;
            WebResponse myWebResponse;
            String URL = url;

            myWebRequest = WebRequest.Create(URL);
            myWebResponse = myWebRequest.GetResponse();//Returns a response from an Internet resource

            Stream streamResponse = myWebResponse.GetResponseStream();//return the data stream from the internet
            //and save it in the stream

            StreamReader sreader = new StreamReader(streamResponse);//reads the data stream
            string rstring = sreader.ReadToEnd();//reads it to the end
            var Links = Index(rstring);//gets the links only

            
            streamResponse.Close();
            sreader.Close();
            myWebResponse.Close();
        }

        public ISet<string> GetNewLinks(string content)
        {
            Regex regexLink = new Regex("(?<=<a\\s*?href=(?:'|\"))[^'\"]*?(?=(?:'|\"))");

            ISet<string> newLinks = new HashSet<string>();
            foreach (var match in regexLink.Matches(content))
            {
                if (!newLinks.Contains(match.ToString()))
                    newLinks.Add(match.ToString());
            }

            return newLinks;
        }

        public string Index(string content)
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var items = GetNewLinks(content);
            var sitemap = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(ns + "urlset",
                    from i in items
                    select
                        //Add ns to every element.
                    new XElement(ns + "url",
                      new XElement(ns + "loc", i)
                      )
                    )
                  );
            return sitemap.ToString();
        }
    }
}
