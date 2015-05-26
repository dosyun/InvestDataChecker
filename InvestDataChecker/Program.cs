using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using InvestDataChecker.CheckData;

namespace InvestDataChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("取得タイプを入力して下さい");
            int type = int.Parse(Console.ReadLine());

            if ((InvestConst.CheckDataType)type == InvestConst.CheckDataType.通しモード)
            {
                while (true)
                {
                    var patrol = CheckDataFactory.Create(InvestConst.CheckDataType.巡回データチェック);
                    patrol.Execute();

                    var nikkei = CheckDataFactory.Create(InvestConst.CheckDataType.日経データ取込);
                    nikkei.Execute();

                    var kairi = CheckDataFactory.Create(InvestConst.CheckDataType.乖離データ取込);
                    kairi.Execute();

                    var daily = CheckDataFactory.Create(InvestConst.CheckDataType.日時株価データ取込);
                    daily.Execute();
                }
            }
            else
            {
                var checkData = CheckDataFactory.Create((InvestConst.CheckDataType)type);
                checkData.Execute();
            }

        }

    }

}
