using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.AccessControl;
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
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
            Program program = new Program();
            //program.SiteMapBuilder(Console.ReadLine());
            GetSiteMapXml(new Uri(Console.ReadLine()), path);
        }

        

        private static void GetSiteMapXml(Uri uri, string path)
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var items = spiderMe(uri);
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

            Byte[] info = new UTF8Encoding(true).GetBytes(sitemap.ToString());
            using (FileStream fs = File.Create(path + uri.Authority + ".xml"))
            {
                
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }
        }

        private static ISet<string> spiderMe(Uri url)
        {
            Uri baseUri;
            if (url.Authority.Contains("localhost:"))
            {
                string localExtra = url.LocalPath.Substring(0, url.LocalPath.IndexOf("/", 1));
                baseUri = new Uri(url.Scheme + Uri.SchemeDelimiter + url.Authority + localExtra + "/");
            }
            else
            {
                baseUri = new Uri(url.Scheme + Uri.SchemeDelimiter + url.Authority);
            }

            ISet<string> urls = new HashSet<string>();
            urls.Add(preparaUrl(url.AbsoluteUri));
            urls.Add(preparaUrl(baseUri.AbsoluteUri));


            //cambios(getHTML(url), baseUri, ref urls, 0);
            cambios(getHTML(baseUri), baseUri, ref urls, 0);

            return urls;
        }

        private static void cambios(string texto, Uri baseUri, ref ISet<string> urls, int deepLevel)
        {
            string pattern_hRef = @"(?<=<a[^<>]+href=""?)[^""<>]+(?=""?[^<>]*>)";

            MatchCollection col = Regex.Matches(texto, pattern_hRef, RegexOptions.IgnoreCase);

            Uri href;
            string _aux;
            bool X = true;
            foreach (Match match in col)
            {
                if (Uri.IsWellFormedUriString(match.Value, UriKind.Absolute))
                    href = new Uri(match.Value);
                else if (Uri.IsWellFormedUriString(match.Value, UriKind.Relative))
                {
                    href = new Uri(makeRelative(baseUri.AbsoluteUri, match.Value));

                }
                else continue;


                if (baseUri.DnsSafeHost.Replace("www.", string.Empty)
                    == href.DnsSafeHost.Replace("www.", string.Empty))
                {
                    _aux = preparaUrl(href.AbsoluteUri);

                    if (!urls.Contains(_aux))
                    {
                        urls.Add(_aux);

                        if (X && (deepLevel < 1))
                        {
                            System.Threading.Thread.Sleep(1500);
                            X = false;
                            cambios(getHTML(new Uri(_aux)), baseUri, ref urls, ++deepLevel);
                        }
                    }
                }
            }
        }

        private static string preparaUrl(string url)
        {
            string _aux = url;
            _aux = _aux.EndsWith("/") ? _aux.Substring(0, _aux.Length - 1) : _aux;
            _aux = _aux.EndsWith("#") ? _aux.Substring(0, _aux.Length - 1) : _aux;
            _aux = _aux.Trim();

            return _aux;
        }

        private static string getHTML(Uri url)
        {
            string retorno = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        StreamReader sr = new StreamReader(response.GetResponseStream());
                        retorno = sr.ReadToEnd();
                    }
                }
            }
            catch
            { retorno = string.Empty; }

            return retorno;
        }

        internal static string makeRelative(string baseUrl, string relativeUrl)
        {
            baseUrl = baseUrl.Trim(new char[] { '/' });
            relativeUrl = relativeUrl.Trim(new char[] { '/' });

            int ipuntosBarra = 0;
            string spuntosBarra = "../";
            while (relativeUrl.StartsWith(spuntosBarra))
            {
                ipuntosBarra++;
                relativeUrl = relativeUrl.Substring(0, spuntosBarra.Length);
            }

            for (int i = 0; i < ipuntosBarra; i++)
            {
                baseUrl = baseUrl.Substring(0, baseUrl.LastIndexOf('/'));
            }

            return baseUrl + "/" + relativeUrl;
        }
    }
}
