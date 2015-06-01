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
    class NikkeiDataCheck : ICheckData
    {
        public void Execute()
        {
            var ta = new invest_dataTableAdapter();
            var vwta = new vw_invest_dataTableAdapter();
            var allData = vwta.GetData().ToArray();
            allData.AsParallel().ForEach(d =>
            {
                var stockId = d.stock_id;
                Console.WriteLine("現在ID:{0}のデータを取得しています", stockId);

                var nikkeiXmlData =
                    new XmlData(string.Format("http://www.nikkei.com/markets/company/?scode={0}", stockId));

                var currentPriceData =
                    nikkeiXmlData.XmlDocument.Descendants(nikkeiXmlData.NameSpace + "dd").ToArray();

                if (currentPriceData.Count() <= 3)
                {
                    Console.WriteLine("現在ID:{0}のデータを取得できませんでした", stockId);
                    ta.DeleteByPK(stockId);
                    return;
                }

                //現在値
                var currentPrice = StringUtil.ReplaceDoubleValue(currentPriceData.ElementAt(0).Value);

                //前日差額
                var diffPriceData =
                    nikkeiXmlData.XmlDocument.Descendants(nikkeiXmlData.NameSpace + "span").ToArray();

                var diffPriceRateData = diffPriceData.Where(x => x.Attribute("class") != null)
                    .FirstOrDefault(x => x.Attribute("class")
                        .Value.Equals("stc-percent"))
                    .Value;

                var plus = diffPriceData.Where(x => x.Attribute("class") != null)
                    .FirstOrDefault(x => x.Attribute("class")
                        .Value
                        .Equals("cmn-plus"));


                var minus = diffPriceData.Where(x => x.Attribute("class") != null)
                    .FirstOrDefault(x => x.Attribute("class")
                        .Value
                        .Equals("cmn-minus"));

                var diffPrice = plus != null ? plus.Value : minus != null ? minus.Value : string.Empty;

                var diffPriceRate = StringUtil.ReplaceDoubleValue(diffPriceRateData);

                //始値
                var startPrice = StringUtil.ReplaceDoubleValue(currentPriceData.ElementAt(2).Value);

                //高値
                var maxPrice = StringUtil.ReplaceDoubleValue(currentPriceData.ElementAt(3).Value);

                //安値
                var minPrice = StringUtil.ReplaceDoubleValue(currentPriceData.ElementAt(4).Value);

                //売買高
                var tradeNum = StringUtil.ReplaceDoubleValue(currentPriceData.ElementAt(5).Value);

                //PBR
                var pbr = StringUtil.ReplaceDoubleValue(currentPriceData.ElementAt(6).Value);

                //PER
                var per = StringUtil.ReplaceDoubleValue(currentPriceData.ElementAt(7).Value);

                //年初来高値
                var yearMaxPrice = StringUtil.ReplaceDoubleValue(currentPriceData.ElementAt(8).Value);

                //年初来安値
                var yearMinPrice = StringUtil.ReplaceDoubleValue(currentPriceData.ElementAt(9).Value);

                //売買単位
                var unit = StringUtil.ReplaceDoubleValue(currentPriceData.ElementAt(12).Value);

                //最低購入代金
                var minBuyPrice = StringUtil.ReplaceDoubleValue(currentPriceData.ElementAt(13).Value);

                //配当利回り
                var shareInterestRate = StringUtil.ReplaceDoubleValue(currentPriceData.ElementAt(14).Value);

                //株式利回り
                var stockCapital = StringUtil.ReplaceDoubleValue(currentPriceData.ElementAt(15).Value);

                //株式数
                var stockNum = StringUtil.ReplaceDoubleValue(currentPriceData.ElementAt(16).Value);

                //時価総額
                var allPrice = StringUtil.ReplaceDoubleValue(currentPriceData.ElementAt(17).Value);

                #region 信用残

                var nikkeiTrustXmlData =
                    new XmlData(
                        string.Format("http://www.nikkei.com/markets/company/history/trust.aspx?scode={0}",
                            stockId));

                var trustData =
                    nikkeiTrustXmlData.XmlDocument.Descendants(nikkeiTrustXmlData.NameSpace + "td").ToArray();

                //信用売残
                var trustSellRest = StringUtil.ReplaceDoubleValue(trustData.ElementAt(0).Value);

                //信用買残
                var trustBuyRest = StringUtil.ReplaceDoubleValue(trustData.ElementAt(1).Value);

                //信用率
                var trustRate = StringUtil.ReplaceDoubleValue(trustData.ElementAt(2).Value);

                #endregion

                #region 指標

                var nikkeiIndexXmlData =
                    new XmlData(
                        string.Format("http://www.nikkei.com/markets/company/kessan/shihyo.aspx?scode={0}",
                            stockId));

                var indexData =
                    nikkeiIndexXmlData.XmlDocument.Descendants(nikkeiIndexXmlData.NameSpace + "td").ToArray();

                //一株資産
                var perCapitalPrice = StringUtil.ReplaceDoubleValue(indexData.ElementAt(58).Value == "-" ? "0" : indexData.ElementAt(58).Value);

                //ROE
                var roe = StringUtil.ReplaceDoubleValue(indexData.ElementAt(59).Value == "-" ? "0" : indexData.ElementAt(59).Value);

                //売上高経常利益率
                var sellBenefitRate = StringUtil.ReplaceDoubleValue(indexData.ElementAt(60).Value == "-" ? "0" : indexData.ElementAt(60).Value);

                //自己資本比率
                var ownedCapitalRate = StringUtil.ReplaceDoubleValue(indexData.ElementAt(61).Value == "-" ? "0" : indexData.ElementAt(61).Value);

                ta.UpdateByPK(
                    currentPrice,
                    diffPrice,
                    diffPriceRate,
                    startPrice,
                    maxPrice,
                    minPrice,
                    tradeNum,
                    pbr,
                    per,
                    yearMaxPrice,
                    yearMinPrice,
                    unit,
                    minBuyPrice,
                    shareInterestRate,
                    stockCapital,
                    stockNum,
                    allPrice,
                    perCapitalPrice,
                    roe,
                    sellBenefitRate,
                    ownedCapitalRate,
                    trustSellRest,
                    trustBuyRest,
                    trustRate,
                    stockId
                    );

                #endregion
            });
        }
    }
}
