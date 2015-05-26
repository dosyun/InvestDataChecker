using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace InvestDataChecker.Util
{
    public class MailUtil
    {
        public static void SendMail(string subject, string body)
        {
            //GMail Initialize
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                Credentials = new System.Net.NetworkCredential("", ""),
                EnableSsl = true
            };

            //送信メッセージ
            var oMsg = new MailMessage()
            {
                Subject = subject,
                Body = body,
                From = new MailAddress("", "")
            };

            // 送信先
            oMsg.To.Add("");

            //メール送信
            smtp.Send(oMsg);
            smtp.Dispose();
        }
    }
}
