﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using RadaCode.Tegenaria.Core.Interfaces;

namespace RadaCode.Tegenaria.Core
{
    public class XmlGenerator : IXmlGenerator
    {
        public int IndexedPagesCount { get; set; }

        public void GetSiteMapXml(Uri uri, string path, ISet<string> urls)
        {
            IndexedPagesCount = urls.Count;

            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            
            var sitemap = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(ns + "urlset",
                    from i in urls
                    select
                        //Add ns to every element.
                    new XElement(ns + "url",
                      new XElement(ns + "loc", i)
                      )
                    )
                  );
            string headerXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n";
            Byte[] info = new UTF8Encoding(true).GetBytes(headerXml + sitemap.ToString());
            using (FileStream fs = File.Create(path + uri.Authority + ".xml"))
            {
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }
        }
    }
}
