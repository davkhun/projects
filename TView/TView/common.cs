using System;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Text;
using System.Security.Cryptography;
using System.Net.Mail;

namespace TView
{

    public class DocInfo
    {
        public string _fileName, _id;
        public DocInfo(string fileName, string id)
        {
            _fileName = fileName;
            _id = id;
        }
    }

    public static class Common
    {
        public static string ByteArrayToString(byte[] arrInput)
        {
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            foreach (byte c in arrInput)
                sOutput.Append(c.ToString("X2"));
            return sOutput.ToString();
        }

        public static bool ValidateUser(string login, string password)
        {
            MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQLString"].ConnectionString);
            MySqlCommand comm = new MySqlCommand("SELECT COUNT(*) FROM _users WHERE login=@login AND password=@password", conn);
            comm.Parameters.AddWithValue("@login", login);
            comm.Parameters.AddWithValue("@password", password);
            conn.Open(); object found = comm.ExecuteScalar(); conn.Close();
            return Convert.ToInt16(found) > 0;
        }

        public static string ToMD5(string str)
        {
            return ByteArrayToString(new MD5CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(str)));
        }

        #region Отправка email-уведомлений
        /// <summary>
        /// Отправка писем
        /// </summary>
        /// <param name="from">От кого</param>
        /// <param name="to">Кому (несколько получателей через ;)</param>
        /// <param name="subj">Тема</param>
        /// <param name="body">Тело</param>
        public static bool MailSend(string from, string to, string subj, string body)
        {
            try
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(from);
                if (to.IndexOf(';') > -1)
                {
                    foreach (string oneTo in to.Split(';'))
                        msg.To.Add(oneTo);
                }
                else
                    msg.To.Add(to);
                msg.Subject = subj;
                msg.Body = body;
                msg.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["smtpAddress"], Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"]));
               
                
                smtp.Send(msg);
                smtp.Dispose();
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Обрезание символов с добавлением ...
        public static string TruncateStr(string str, int length = 15)
        {
            if (str.Length >= length)
                return str.Substring(0, length) + "...";
            return str;
        }
        #endregion
    }


}