using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
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

        public static decimal GetDecimal( string number )
        {
            if (string.IsNullOrWhiteSpace(number)) return 0;

            return Decimal.Parse(number);
        }

        public static string GetMoneyFormatString(string number)
        {
            decimal money = Util.GetDecimal(number);
            return money.ToString("C", CultureInfo.CurrentCulture);
        }

        public static void SendMail(string toAddress, string title, string bodyHtml, Attachment attachment = null)
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

            if ( toAddress.IndexOf(";") >= 0 )
            {
                string[] ar = toAddress.Split(';').ToArray();
                for (int i = 0; i < ar.Length; i++)
                    message.To.Add(toAddress); //ex : ooo@gmail.com
            }
            else
                message.To.Add(toAddress); //ex : ooo@gmail.com

            message.Subject = title;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.Body = bodyHtml;
            message.IsBodyHtml = true;
            message.BodyEncoding = System.Text.Encoding.UTF8;

            if (attachment != null )
                message.Attachments.Add(attachment);

            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.naver.com", 587);
            smtp.UseDefaultCredentials = false; // 시스템에 설정된 인증 정보를 사용하지 않는다.
            smtp.EnableSsl = true;  // SSL을 사용한다.
            smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network; // 이걸 하지 않으면 naver 에 인증을 받지 못한다.
            smtp.Credentials = new System.Net.NetworkCredential("kdy2509", "eodyd@509");
            smtp.Send(message);

        }

        public static DataTable CreateDataTable<T>(IEnumerable<T> list)
        {
            Type type = typeof(T);
            var properties = type.GetProperties();

            DataTable dataTable = new DataTable();
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            foreach (T entity in list)
            {
                object[] values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(entity);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }
}
