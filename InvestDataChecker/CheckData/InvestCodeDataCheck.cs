using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using InvestDataChecker.Data.DataSetTableAdapters;
using InvestDataChecker.Util;
using InvestDataChecker.Xml;

namespace InvestDataChecker.CheckData
{
    public class InvestCodeDataCheck : ICheckData
    {
        public void Execute()
        {
            var ta = new invest_dataTableAdapter();

            for (int i = 1; i <= 9; i++)
            {
                var xmlData = new XmlData(string.Format("http://minkabu.jp/stock/list/{0}", i));
                var codeData = xmlData.XmlDocument.Descendants(xmlData.NameSpace + "td")
                    .Where(x => x.Attribute("class") != null)
                    .Where(x => x.Attribute("class").Value == "tac wsnw")
                    .ToArray();

                var nameData = xmlData.XmlDocument.Descendants(xmlData.NameSpace + "a").ToArray();
                for (int j = 0; j < 9999; j++)
                {
                    var num =  j + (3 * j);
                    var code = codeData.Skip(num).Take(1).FirstOrDefault();
                    if (code != null)
                    {
                        var investCode = int.Parse(code.Value);
                        var investData = ta.GetDataByPK(investCode).FirstOrDefault();
                        if (investData == null)
                        {
                            var name = nameData.Where(x => x.Attribute("href") != null)
                                .FirstOrDefault(x => x.Attribute("href").Value.Contains(investCode.ToString()));

                            ta.AddStockIdNew(investCode, name != null ? name.LastAttribute.Value : string.Empty);
                            Console.WriteLine("{0}:{1}のデータを追加しました", investCode, name.LastAttribute.Value);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
          
        }
    }
}
