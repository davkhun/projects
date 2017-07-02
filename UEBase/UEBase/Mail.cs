using System.Linq;
using System.Net.Mail;

namespace UEBase
{
    internal class CMail
    {
        private readonly string smtpAddr;
        private readonly int smtpPort;

        public void MailSend(string from, string to, string subj, string body, bool bcc = false)
        {
            MailMessage msg = new MailMessage { From = new MailAddress(from) };
            if (to.IndexOf(';') > -1)
            {
                foreach (string oneTo in to.Split(';').Where(oneTo => oneTo.Contains("@")))
                {
                    msg.To.Add(oneTo.Trim());
                }
            }
            else
                msg.To.Add(to);
            msg.Subject = subj;
            msg.Body = body;
            msg.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient(smtpAddr, smtpPort);
            smtp.Send(msg);
            smtp.Dispose();

        }

        public CMail(string adr, int port)
        {
            smtpAddr = adr;
            smtpPort = port;
        }
    }
}