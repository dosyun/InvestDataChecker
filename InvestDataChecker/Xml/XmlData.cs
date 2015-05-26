using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Sgml;

namespace InvestDataChecker.Xml
{
    public class XmlData
    {
        private string _url;
        public XmlData(string url)
        {
            _url = url;
        }

        public string Url
        {
            get { return _url; }
        }

        public XDocument XmlDocument
        {
            get
            {
                try
                {
                    // WebClientを使ってHTTPレスポンスBodyを取得 
                    using (var stream = new WebClient().OpenRead(Url))
                    {
                        using (var reader = new StreamReader(stream, Encoding.GetEncoding("UTF-8")))
                        {
                            // SGMLReaderの準備 
                            var sgmlReader = new SgmlReader
                            {
                                DocType = "HTML",
                                CaseFolding = CaseFolding.ToLower,
                                InputStream = reader,
                                IgnoreDtd = true
                            };

                            // HTMLをXML形式にする 
                            return XDocument.Load(sgmlReader);
                        }
                    }
                }
                catch (Exception)
                {
                    return null;
                }

            }
        }

        public XNamespace NameSpace
        {
            get
            {
                if (XmlDocument != null && XmlDocument.Root != null)
                    return XmlDocument.Root.Name.Namespace;

                return null;
            }
        }
    }
}
