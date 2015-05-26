using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestDataChecker.Util
{
    public class DateUtil
    {
        public static bool IsMarketStart()
        {
            return DateTime.Now >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0) && !IsMarketEnd();
        }

        public static bool IsMarketRest()
        {
            var restStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 11, 30, 00);
            var restEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 30, 00);
            return DateTime.Now >= restStart && DateTime.Now <= restEnd;
        }

        public static bool IsMarketEnd()
        {
            return DateTime.Now >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 15, 00, 00);
        }
    }
}
