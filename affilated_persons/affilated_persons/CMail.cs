using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace affilated_persons
{
    class CMail
    {
        /// <summary>
        /// Отправка писем
        /// </summary>
        /// <param name="from">От кого</param>
        /// <param name="to">Кому (несколько получателей через ;)</param>
        /// <param name="subj">Тема</param>
        /// <param name="body">Тело</param>
        public static bool Send(string to, string subj, string body, string from = ConfigurationManager.AppSettings["mailFrom"])
        {
            try
            {
                MailMessage msg = new MailMessage { From = new MailAddress(from) };
                if (to.IndexOf(';') > -1)
                {
                    foreach (string oneTo in to.Split(';').Where(oneTo => oneTo.Length > 3))
                        msg.To.Add(oneTo.Trim());
                }
                else
                    msg.To.Add(to.Trim());
                msg.Subject = subj;
                msg.Body = body;
                msg.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SMTPaddress"], Convert.ToInt32(ConfigurationManager.AppSettings["SMTPport"]));
                smtp.Send(msg);
                smtp.Dispose();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool SendAsync(string to, string subj, string body, string from = ConfigurationManager.AppSettings["mailFrom"])
        {
            try
            {
                MailMessage msg = new MailMessage { From = new MailAddress(from) };
                if (to.IndexOf(';') > -1)
                {
                    foreach (string oneTo in to.Split(';').Where(oneTo => oneTo.Length > 3))
                        msg.To.Add(oneTo.Trim());
                }
                else
                    msg.To.Add(to.Trim());
                msg.Subject = subj;
                msg.Body = body;
                msg.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SMTPaddress"], Convert.ToInt32(ConfigurationManager.AppSettings["SMTPport"]));
                object state = msg;
                smtp.SendCompleted += smtp_SendCompleted;
                smtp.SendAsync(msg, state);
                smtp.Dispose();
            }
            catch
            {
                return false;
            }
            return true;
        }

        static void smtp_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            MailMessage mail = e.UserState as MailMessage;
        }


    }
}
