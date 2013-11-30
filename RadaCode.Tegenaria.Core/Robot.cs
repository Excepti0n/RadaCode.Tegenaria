using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RadaCode.Tegenaria.Core.Interfaces;

namespace RadaCode.Tegenaria.Core
{
    public class Robot : IRobot
    {
        ISet<string> urls = new HashSet<string>();

        public ISet<string> spiderMe(Uri url)
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

            urls.Add(preparaUrl(url.AbsoluteUri));
            urls.Add(preparaUrl(baseUri.AbsoluteUri));
            cambios(getHTML(baseUri), baseUri, baseUri, urls, 0).ToList();
            for (int i = 0; i < urls.Count(); i++)
            {
                if (baseUri.Equals(new Uri(urls.ElementAt(i)))) continue;
                cambios(getHTML(new Uri(urls.ElementAt(i))), baseUri, new Uri(urls.ElementAt(i)), urls, 0);
            }

            return urls;
        }

        private ISet<string> cambios(string texto, Uri baseUri, Uri crowlUri, ISet<string> urls, int deepLevel)
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
                    href = new Uri(MakeRelative(baseUri.AbsoluteUri, match.Value));
                }
                else continue;


                if (crowlUri.DnsSafeHost.Replace("www.", string.Empty)
                    == href.DnsSafeHost.Replace("www.", string.Empty))
                {
                    string correctUrl = ConfirmUrl(new Uri(href.AbsoluteUri));
                    if(correctUrl == "") continue;
                    _aux = preparaUrl(correctUrl);

                    if (!urls.Contains(_aux))
                    {
                        urls.Add(_aux);

                        if (X && (deepLevel < 1))
                        {
                            System.Threading.Thread.Sleep(1500);
                            X = false;
                            cambios(getHTML(new Uri(_aux)), baseUri, crowlUri, urls, ++deepLevel);
                        }
                    }
                }
            }
            return urls;
        }

        private string preparaUrl(string url)
        {
            string _aux = url;
            _aux = _aux.EndsWith("/") ? _aux.Substring(0, _aux.Length - 1) : _aux;
            _aux = _aux.EndsWith("#") ? _aux.Substring(0, _aux.Length - 1) : _aux;
            _aux = _aux.Trim();

            return _aux;
        }

        private string getHTML(Uri url)
        {
            string retorno = string.Empty;
            
            var request = (HttpWebRequest) WebRequest.Create(url);
            try
            {
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var sr = new StreamReader(response.GetResponseStream());
                        retorno = sr.ReadToEnd();
                    }
                }
            }
            catch
            {
                retorno = string.Empty;
            } 
            return retorno;
        }

        private string ConfirmUrl(Uri url)
        {
            string returnUrl = "";

            var request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        returnUrl = response.ResponseUri.AbsoluteUri;
                    }
                }
            }
            catch
            {
                returnUrl = "";
            }
            return returnUrl;
        }

        internal string MakeRelative(string baseUrl, string relativeUrl)
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
