using System;
using System.Configuration;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Web.Security;

namespace TView
{
    public partial class login : System.Web.UI.Page
    {
        private MySqlConnection conn;
        private MySqlCommand comm;

        protected void Page_Load(object sender, EventArgs e)
        {
            conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQLString"].ConnectionString);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Session["main_validate"] = "true";
            Session["change_validate"] = "false";
            Session["restore_validate"] = "false";
            Page.Validate();
            if (Page.IsValid)
            {
                if (Common.ValidateUser(TextBox1.Text, Common.ToMD5(TextBox2.Text)))
                    FormsAuthentication.RedirectFromLoginPage(TextBox1.Text, false);
                else
                    Response.Redirect("login.aspx", true);
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Session["change_validate"] = "false";
            Session["main_validate"] = "false";
            Session["restore_validate"] = "false";
            if (Panel1.Visible == false)
            {
                Panel1.Visible = true;
                Panel2.Visible = false;
                TextBox3.Focus();
            }
            else
                Panel1.Visible = false;
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            Session["change_validate"] = "true";
            Session["main_validate"] = "false";
            Session["restore_validate"] = "false";
            Page.Validate();
            if (Page.IsValid)
            {
                if (TextBox5.Text == TextBox6.Text)
                {
                    Label1.Text = "";
                    if (Common.ValidateUser(TextBox3.Text,Common.ToMD5(TextBox4.Text)))
                    {
                        comm = new MySqlCommand("UPDATE _users SET password=@password WHERE login=@login", conn);
                        comm.Parameters.AddWithValue("@login", TextBox3.Text);
                        comm.Parameters.AddWithValue("@password", Common.ToMD5(TextBox5.Text));
                        conn.Open(); comm.ExecuteNonQuery(); conn.Close();
                        Label1.Text = "Пароль успешно изменен!";
                    }
                }
                else
                    Label1.Text = "Новые введенные пароли не совпадают!";
            }
        }

        protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (Session["change_validate"].ToString() == "true")
            {
                args.IsValid = TextBox3.Text != "";
            }
            else
                args.IsValid = true;
        }

        protected void CustomValidator5_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (Session["main_validate"].ToString() == "true")
            {
                args.IsValid = TextBox1.Text != "";
            }
            else
                args.IsValid = true;
        }

        protected void CustomValidator6_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (Session["main_validate"].ToString() == "true")
            {
                args.IsValid = TextBox2.Text != "";
            }
            else
                args.IsValid = true;
        }

        protected void CustomValidator2_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (Session["change_validate"].ToString() == "true")
            {
                args.IsValid = TextBox4.Text != "";
            }
            else
                args.IsValid = true;
        }

        protected void CustomValidator3_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (Session["change_validate"].ToString() == "true")
            {
                args.IsValid = TextBox5.Text != "";
            }
            else
                args.IsValid = true;
        }

        protected void CustomValidator4_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (Session["change_validate"].ToString() == "true")
            {
                args.IsValid = TextBox6.Text != "";
            }
            else
                args.IsValid = true;
        }

        protected void CustomValidator7_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (Session["restore_validate"].ToString() == "true")
            {
                args.IsValid = TextBox7.Text != "";
            }
            else
                args.IsValid = true;
        }

        protected void CustomValidator8_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (Session["restore_validate"].ToString() == "true")
            {
                args.IsValid = TextBox8.Text != "";
            }
            else
                args.IsValid = true;
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            Session["restore_validate"] = "true";
            Session["change_validate"] = "false";
            Session["main_validate"] = "false";
            Page.Validate();
            if (Page.IsValid)
            {
                comm = new MySqlCommand("SELECT COUNT(*) FROM _users WHERE login=@login AND email=@email", conn);
                comm.Parameters.AddWithValue("@login", TextBox7.Text);
                comm.Parameters.AddWithValue("@email", TextBox8.Text);
                conn.Open(); int count = Convert.ToInt16(comm.ExecuteScalar()); conn.Close();
                if (count > 0)
                {
                    string newPass = DateTime.Now.Ticks.ToString();
                    comm = new MySqlCommand("UPDATE _users SET password=@password WHERE login=@login", conn);
                    comm.Parameters.AddWithValue("@password", Common.ToMD5(newPass));
                    comm.Parameters.AddWithValue("@login", TextBox7.Text);
                    conn.Open(); comm.ExecuteNonQuery(); conn.Close();
                    Common.MailSend(ConfigurationManager.AppSettings["mailFrom"], TextBox8.Text, "Tender View: Восстановление пароля", "Здравствуйте.<br/>Ваш новый пароль " + newPass + ".<br/><br/>С уважением, служба Tender View.");
                    Label2.Text = "Пароль успешно изменен! Проверьте вашу почту, указанную при регистрации.";
                }
                else
                    Label2.Text = "Пользователь не найден!";
            }
        }

        protected void LinkButton2_Click(object sender, EventArgs e)
        {
            Session["change_validate"] = "false";
            Session["main_validate"] = "false";
            Session["restore_validate"] = "false";
            if (Panel2.Visible == false)
            {
                Panel2.Visible = true;
                Panel1.Visible = false;
                TextBox7.Focus();
            }
            else
                Panel2.Visible = false;
        }


    }
}