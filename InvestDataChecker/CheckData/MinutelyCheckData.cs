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
    class MinutelyCheckData : ICheckData
    {
        public void Execute()
        {
            var ta = new invest_dataTableAdapter();
            var vwta = new vw_invest_dataTableAdapter();
            var alertTa = new invest_alert_dataTableAdapter();
            var minuteTa = new invest_minutely_dataTableAdapter();
            while (true)
            {
                //休場中なら待ち
                if (DateUtil.IsMarketRest())
                    continue;

                //終了したら抜ける
                if (DateUtil.IsMarketEnd())
                    break;

                var checkData = vwta.GetData().ToArray();
                foreach (var c in checkData)
                {
                    var stockId = c.stock_id;
                    var beforePrice = c.current_price;
                    Console.WriteLine("現在ID:{0}のデータを取得しています", stockId);
                    var nikkeiXmlData =
                        new XmlData(string.Format("http://www.nikkei.com/markets/company/?scode={0}", stockId));

                    var currentPriceData =
                        nikkeiXmlData.XmlDocument.Descendants(nikkeiXmlData.NameSpace + "dd").ToArray();

                    if (currentPriceData.Count() <= 3)
                        continue;

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

                    if ((int)c.start_price == (int)startPrice)
                    {
                        if (!c.Isalert_buy_priceNull() && !c.Isis_buy_alertNull() && c.is_buy_alert)
                        {
                            if (c.alert_buy_price > 0 && currentPrice <= c.alert_buy_price)
                            {
                                //MailUtil.SendMail("買いシグナル", string.Format("{0}:{1} {2}現在{3}円", stockId, c.stock_name, DateTime.Now.ToString("HH:mm:ss"), currentPrice));
                                ta.UpdateByIsBuyAlert(false, stockId);
                                alertTa.AddNew(stockId, c.stock_name, (int)InvestConst.BuySellAlertFlag.Buy, currentPrice);

                            }
                        }
                    }

                    if (!c.Isalert_sell_priceNull() && !c.Isis_sell_alertNull() && c.is_sell_alert)
                    {
                        if (c.alert_sell_price > 0 && currentPrice >= c.alert_sell_price)
                        {
                            MailUtil.SendMail("売りシグナル", string.Format("{0}:{1} {2}現在{3}円", stockId, c.stock_name, DateTime.Now.ToString("HH:mm:ss"), currentPrice));
                            ta.UpdateByIsSellAlert(false, stockId);
                            alertTa.AddNew(stockId, c.stock_name, (int)InvestConst.BuySellAlertFlag.Sell, currentPrice);
                        }
                    }

                    //高値
                    var maxPrice = StringUtil.ReplaceDoubleValue(currentPriceData.ElementAt(3).Value);

                    //安値
                    var minPrice = StringUtil.ReplaceDoubleValue(currentPriceData.ElementAt(4).Value);

                    //売買高
                    var tradeNum = StringUtil.ReplaceDoubleValue(currentPriceData.ElementAt(5).Value);

                    ta.UpdateMinutelyByPK(currentPrice, diffPrice, diffPriceRate, startPrice, maxPrice, minPrice,
                        tradeNum, stockId);

                    Func<double, double, InvestConst.UpDownFlag> updownType = (before, after) =>
                    {
                        if ((int)before > (int)after)
                            return InvestConst.UpDownFlag.Down;
                        if ((int)before == (int)after)
                            return InvestConst.UpDownFlag.Even;

                        return InvestConst.UpDownFlag.Up;
                    };

                    minuteTa.AddNew(stockId, currentPrice, (int)updownType(beforePrice, currentPrice));
                }
            }
        }
    }
}
