using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestDataChecker
{
    public class InvestConst
    {
        public enum CheckDataType
        {
            巡回データチェック = 1,
            日経データ取込 = 2,
            乖離データ取込 = 3,
            日時株価データ取込 = 4,
            証券コード取込 = 5,
            通しモード = 9,
        }

        public enum UpDownFlag
        {
            Down = 0,
            Even = 1,
            Up = 2,
        }


        public enum BuySellAlertFlag
        {
            Buy = 0,
            Sell = 1,
        }
    }
}
