using System;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Text.RegularExpressions;

namespace TView
{
    public partial class register : System.Web.UI.Page
    {
        private MySqlConnection conn;
        private MySqlCommand comm;

        private bool IsPresent(string key, string value)
        {
            comm = new MySqlCommand("SELECT COUNT(*) FROM _users WHERE " + key + "=@value", conn);
            comm.Parameters.AddWithValue("@value", value);
            int count = Convert.ToInt16(comm.ExecuteScalar());
            return count != 0;
        }

        protected void Page_Load(object sender, EventArgs e)
        {           
            conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQLString"].ConnectionString);
            if (Request.QueryString["validate"] != null) // получили строку, парсим
            {
                EmailLb.Text = "";
                ValidPanel.Visible = true;
                string tmpHash = Request.QueryString["validate"];
                conn.Open();
                // проверим есть ли хеш. Если есть, показываем пароль, если нет - не показываем
                if (IsPresent("tmpHash", tmpHash))
                {
                    // проверим, не прошел ли день с момента первичной регистрации. Если прошел - трём запись
                    comm = new MySqlCommand("SELECT (UNIX_TIMESTAMP(now())-UNIX_TIMESTAMP(regTime))/3600 FROM _users WHERE tmpHash=@tmpHash", conn);
                    comm.Parameters.AddWithValue("@tmpHash", tmpHash);
                    int timeLapse = Convert.ToInt16(comm.ExecuteScalar());
                    if (timeLapse < 24)
                    {
                        // тащим емейл
                        comm = new MySqlCommand("SELECT email FROM _users WHERE tmpHash=@tmpHash", conn);
                        comm.Parameters.AddWithValue("@tmpHash", tmpHash);
                        string email = Convert.ToString(comm.ExecuteScalar());
                        string pass = DateTime.Now.Ticks.ToString().Substring(DateTime.Now.Ticks.ToString().Length - 6, 6);
                        comm = new MySqlCommand("UPDATE _users SET tmpHash=NULL,password=@password WHERE tmpHash=@tmpHash", conn);
                        comm.Parameters.AddWithValue("@password", Common.ToMD5(pass));
                        comm.Parameters.AddWithValue("@tmpHash", tmpHash);
                        comm.ExecuteNonQuery();
                        finishLbl.Text = "Спасибо за регистрацию! Ваш пароль: " + pass;
                        Common.MailSend("tview@skbkontur.ru", email, "Добро пожаловать в TenderView", "Здравствуйте.<br/><br/>Ваш пароль: " + pass);
                    }
                    else // если больше прошло - трём запись
                    {
                        comm = new MySqlCommand("DELETE FROM _users WHERE tmpHash=@tmpHash", conn);
                        comm.Parameters.AddWithValue("@tmpHash", tmpHash);
                        comm.ExecuteNonQuery();
                        finishLbl.Text = "Прошло более 24 часов с момента получения письма. Пройдите регистрацию повторно!";
                    }
                }
                else
                {
                    finishLbl.ForeColor = System.Drawing.Color.Red;
                    finishLbl.Text = "Произошла ошибка. Введен неправильный адрес!";
                }
                conn.Close();
            }
            else
                RegPanel.Visible = true;
            msgLbl.Text = "";
        }

        protected void SendButton_Click(object sender, EventArgs e)
        {
            EmailLb.Text = "";
            if (Page.IsValid)
            {
                if (Regex.Match(EmailTB.Text, @"^[a-z0-9][a-z0-9_\.-]{0,}[a-z0-9]@[a-z0-9][a-z0-9_\.-]{0,}[a-z0-9][\.][a-z0-9]{2,4}$").Success)
                {
                    string login = LoginTB.Text.Trim();
                    string email = EmailTB.Text.Trim();
                    conn.Open();
                    // проверим наличие пользователя в базе по логину
                    if (!IsPresent("login", login))
                    {
                        // если пользователя нет, формируем хеш
                        string tmpHash = Common.ToMD5(login + "©" + email);
                        // вставляем в базу логин, хеш, email
                        comm = new MySqlCommand("INSERT INTO _users (login,email,tmpHash,regTime) VALUES (@login,@email,@tmpHash,now())", conn);
                        comm.Parameters.AddWithValue("@login", login);
                        comm.Parameters.AddWithValue("@email", email);
                        comm.Parameters.AddWithValue("@tmpHash", tmpHash);
                        comm.ExecuteNonQuery();
                        // отправляем письмо со ссылкой для валидации
                        Common.MailSend(ConfigurationManager.AppSettings["mailFrom"], email, "Подтверждение регистрации TenderView", "Здравствуйте.<br/><br/> Для завершения регистрации Вам необходимо пройти по ссылке: http://localhost/register.aspx?validate=" + tmpHash + "<br/><br/>Ссылка действительна в течение 24 часов.");
                        msgLbl.ForeColor = System.Drawing.Color.Black;
                        msgLbl.Text = "Письмо с подтверждением регистрации успешно отправлено. Проверьте вашу почту.";
                    }
                    else
                    {
                        msgLbl.ForeColor = System.Drawing.Color.Red;
                        msgLbl.Text = "Пользователь " + login + " уже зарегистрирован!";
                    }
                    conn.Close();
                }
                else
                    EmailLb.Text = "* Введен неправильный email!";
            }
        }
    }
}