using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadaCode.Tegenaria.Core.Interfaces
{
    public interface IXmlGenerator
    {
        int IndexedPagesCount { get; set; }

        void GetSiteMapXml(Uri uri, string path, ISet<string> urls);
    }
}
