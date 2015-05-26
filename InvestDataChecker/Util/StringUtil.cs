using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestDataChecker.Util

{
    public class StringUtil
    {
        public static double ReplaceDoubleValue(string stringValue)
        {
            return double.Parse(stringValue.Replace("(±0.00％)", "0").Replace(",", "").Replace("(", "").Replace(")", "").Replace("--", "0").Replace("%", "").Replace("％", "").Replace("円", "").Replace("株", "").Replace("倍", "").Replace("百万", "").Replace("倍", "").Replace("N/A", "0"));
        }
      
    }
}
