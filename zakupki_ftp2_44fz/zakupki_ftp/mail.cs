using System;
using System.Configuration;
using System.Net.Mail;

namespace zakupki_ftp
{
    internal class MailSender
    {
        /// <summary>
        /// Отправка писем
        /// </summary>
        /// <param name="to">Кому (несколько получателей через ;)</param>
        /// <param name="subj">Тема</param>
        /// <param name="body">Тело</param>
        /// <param name="bcc"></param>
        /// <param name="attach"></param>
        public static bool Send(string to, string subj, string body, bool bcc = false, string attach = "")
        {
            try
            {
                MailMessage msg = new MailMessage {From = new MailAddress(ConfigurationManager.AppSettings["sendMailFrom"]) };
                if (to.IndexOf(';') > -1)
                {
                    foreach (string oneTo in to.Split(';'))
                        if (oneTo.Length > 3)
                            msg.To.Add(oneTo.Trim());
                }
                else
                    msg.To.Add(to.Trim());
                msg.Subject = subj;
                msg.Body = body;
                msg.IsBodyHtml = true;
                if (attach != "")
                {
                    if (attach.Contains(";"))
                    {
                        foreach (string oneAttach in attach.Split(';'))
                            msg.Attachments.Add(new Attachment(oneAttach));
                    }
                    else
                        msg.Attachments.Add(new Attachment(attach));
                }
                SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["smtpAddress"], Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"]));
                smtp.Send(msg);
                smtp.Dispose();
                msg.Attachments.Clear();
                msg.Dispose();
            }
            catch
            {
                return false;
            }
            return true;
        }

    }
}
