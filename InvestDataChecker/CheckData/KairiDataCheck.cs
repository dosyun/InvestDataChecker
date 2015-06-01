using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestDataChecker.Data.InvestDataSetTableAdapters;
using InvestDataChecker.Util;
using InvestDataChecker.Xml;

namespace InvestDataChecker.CheckData
{
    class KairiDataCheck : ICheckData
    {
        public void Execute()
        {
            var ta = new invest_dataTableAdapter();
            var allData = ta.GetData().ToArray();
            foreach (var d in allData)
            {
                var stockId = d.stock_id;
                Console.WriteLine("現在ID:{0}のデータを取得しています", stockId);

                #region 乖離率

                var kabuComXmlData =
                    new XmlData(
                        string.Format(
                            "http://jp.kabumap.com/servlets/kabumap/Action?SRC=basic/top/base&codetext={0}",
                            stockId));

                var kabuComPriceData =
                    kabuComXmlData.XmlDocument.Descendants(kabuComXmlData.NameSpace + "td").ToArray();

                //25日乖離率
                var kairi25 = StringUtil.ReplaceDoubleValue(kabuComPriceData.ElementAt(83).Value);

                //75日乖離率
                var kairi75 = StringUtil.ReplaceDoubleValue(kabuComPriceData.ElementAt(84).Value);

                //200日乖離率
                var kairi200 = StringUtil.ReplaceDoubleValue(kabuComPriceData.ElementAt(85).Value);

                ta.UpdateKairiByPK(kairi25, kairi75, kairi200, stockId);

                #endregion
            }
        }
    }
}
