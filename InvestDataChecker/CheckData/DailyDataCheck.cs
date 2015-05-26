using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using InvestDataChecker.Data.DataSetTableAdapters;
using InvestDataChecker.Util;
using InvestDataChecker.Xml;

namespace InvestDataChecker.CheckData
{
    public class DailyDataCheck : ICheckData
    {
        public void Execute()
        {
            var vwta = new vw_invest_dataTableAdapter();
            var dta = new invest_daily_dataTableAdapter();
            var allData = vwta.GetData().ToArray();

            foreach (var d in allData)
            {
                var stockId = d.stock_id;
                Console.WriteLine("現在ID:{0}のデータを取得しています", stockId);

                var nikkeiXmlData =
                    new XmlData(string.Format("http://www.nikkei.com/markets/company/history/dprice.aspx?scode={0}", stockId));

                var headerData =
                    nikkeiXmlData.XmlDocument.Descendants(nikkeiXmlData.NameSpace + "th")
                        .Where(x => x.Value.Contains("月")
                            || x.Value.Contains("火")
                            || x.Value.Contains("水")
                            || x.Value.Contains("木")
                            || x.Value.Contains("金")
                            )
                        .ToArray();

                var priceData = nikkeiXmlData.XmlDocument.Descendants(nikkeiXmlData.NameSpace + "td").ToArray();

                var index = 0;
                foreach (var hdata in headerData)
                {
                    if (dta.GetDataByPK(stockId, hdata.Value).FirstOrDefault() == null)
                    {
                        var dailyData = priceData.Skip(index).Take(6).ToArray();
                        var startPrice = StringUtil.ReplaceDoubleValue(dailyData.ElementAt(0).Value);
                        var maxPrice = StringUtil.ReplaceDoubleValue(dailyData.ElementAt(1).Value);
                        var minPrice = StringUtil.ReplaceDoubleValue(dailyData.ElementAt(2).Value);
                        var endPrice = StringUtil.ReplaceDoubleValue(dailyData.ElementAt(3).Value);
                        var tradeNum = StringUtil.ReplaceDoubleValue(dailyData.ElementAt(4).Value);

                        dta.AddNew(stockId, hdata.Value, startPrice, endPrice, minPrice, maxPrice, 0, tradeNum);
                        index += 6;
                        Thread.Sleep(10);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            vwta.GetData().ForEach(x =>
            {
                var beforePrice = 0d;
                dta.GetDataByInvestId(x.stock_id).OrderByDescending(y => y.addtime).ForEach(y =>
                {
                    if ((int)beforePrice == 0)
                    {
                        dta.UpdateByPK(0, y.stock_id, y.date);
                        beforePrice = y.end_price;
                        return;
                    }
                    dta.UpdateByPK(y.end_price - beforePrice, y.stock_id, y.date);
                    beforePrice = y.end_price;

                });
            });

           
           
        }
    }
}
