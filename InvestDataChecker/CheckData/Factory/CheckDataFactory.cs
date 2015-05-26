using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace InvestDataChecker.CheckData
{
    class CheckDataFactory
    {

        public static ICheckData Create(InvestConst.CheckDataType checkDataType)
        {
            switch (checkDataType)
            {
                case InvestConst.CheckDataType.巡回データチェック:
                    return new MinutelyCheckData();
                case InvestConst.CheckDataType.日経データ取込:
                    return new NikkeiDataCheck();
                case InvestConst.CheckDataType.乖離データ取込:
                    return new KairiDataCheck();
                case InvestConst.CheckDataType.日時株価データ取込:
                    return new DailyDataCheck();
                case InvestConst.CheckDataType.証券コード取込:
                    return new InvestCodeDataCheck();
                default:
                    return null;
            }
        }
    }
}
