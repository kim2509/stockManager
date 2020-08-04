using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp2.Common
{
    public class Util
    {
        public static int GetInt(string number)
        {
            if (string.IsNullOrWhiteSpace(number)) return 0;

            return int.Parse(number);
        }

        public static double GetDouble(string number)
        {
            if (string.IsNullOrWhiteSpace(number)) return 0.0;

            return double.Parse(number);
        }

        public static void SendMail(string toAddress, string title, string bodyHtml)
        {
            //MailMessage message = new System.Net.Mail.MailMessage();
            //message.From = new System.Net.Mail.MailAddress("kdy2509@naver.com"); //ex : ooo@naver.com
            //message.To.Add(toAddress); //ex : ooo@gmail.com
            //message.Subject = title;
            //message.SubjectEncoding = System.Text.Encoding.UTF8;
            //message.Body = bodyHtml;
            //message.BodyEncoding = System.Text.Encoding.UTF8;
            //System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.naver.com", 587);
            //smtp.UseDefaultCredentials = false; // 시스템에 설정된 인증 정보를 사용하지 않는다.
            //smtp.EnableSsl = true;  // SSL을 사용한다.
            //smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network; // 이걸 하지 않으면 naver 에 인증을 받지 못한다.
            //smtp.Credentials = new System.Net.NetworkCredential("kdy2509", "fdsa4321");
            //smtp.Send(message);


            MailMessage message = new System.Net.Mail.MailMessage();
            message.From = new System.Net.Mail.MailAddress("kdy2509@naver.com"); //ex : ooo@naver.com
            message.To.Add("kim2509@gmail.com"); //ex : ooo@gmail.com
            message.Subject = title;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.Body = bodyHtml;
            message.IsBodyHtml = true;
            message.BodyEncoding = System.Text.Encoding.UTF8;


            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.naver.com", 587);
            smtp.UseDefaultCredentials = false; // 시스템에 설정된 인증 정보를 사용하지 않는다.
            smtp.EnableSsl = true;  // SSL을 사용한다.
            smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network; // 이걸 하지 않으면 naver 에 인증을 받지 못한다.
            smtp.Credentials = new System.Net.NetworkCredential("kdy2509", "eodyd@509");
            smtp.Send(message);

        }
    }
}
