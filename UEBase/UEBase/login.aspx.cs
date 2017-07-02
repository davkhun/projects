using System;
using MySql.Data.MySqlClient;

namespace UEBase
{
    public partial class login : System.Web.UI.Page
    {
        protected string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.Cookies["UEUname"] != null && Request.Cookies["UEPwd"] != null)
                {
                    TextBox1.Text = Request.Cookies["UEUname"].Value;
                    TextBox2.Text = Request.Cookies["UEPwd"].Value;
                    rememberCb.Checked = true;
                    Button1_Click(null, null);
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            CLogin login = new CLogin();
            if (login.Login(TextBox1.Text, TextBox2.Text))
            {
                // если прошли, читаем настройки, если пользователя нет, режем по максимуму права
                MySqlConnection conn = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MySQLString"].ConnectionString);
                bool user_found = false;
                MySqlCommand comm = new MySqlCommand("SELECT uname,fio,divisions_id,office,admin,super_admin FROM rights WHERE uname=@login", conn);
                comm.Parameters.AddWithValue("@login", TextBox1.Text);
                conn.Open();
                CUser usr = new CUser();
                MySqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    user_found = true;
                    usr = new CUser
                    {
                        uname = TextBox1.Text,
                        fio = reader.IsDBNull(1) ? null : reader.GetString(1),
                        divisions = reader.IsDBNull(2) ? null : reader.GetString(2),
                        office = reader.IsDBNull(3) ? null : reader.GetString(3),
                        admin = reader.GetBoolean(4),
                        superAdmin = reader.GetBoolean(5),
                        registered = true
                    };

                }
                reader.Close();
                conn.Close();
                // если пользователя не нашли, соберем по нему хоть что-нибудь.
                if (!user_found)
                {
                    ADconnector adc = new ADconnector();
                    ADuser adu = adc.GetByUname(TextBox1.Text);
                    usr = new CUser
                    {
                        uname = TextBox1.Text,
                        fio = adu.name,
                        office = adu.office,
                        admin = false,
                        superAdmin = false,
                        registered = false
                    };
                }

                Session["_UEuser"] = usr;

                System.Web.Security.FormsAuthentication.RedirectFromLoginPage(TextBox1.Text, false);
                if (rememberCb.Checked)
                {
                    Response.Cookies["UEUname"].Expires = DateTime.Now.AddDays(30);
                    Response.Cookies["UEPwd"].Expires = DateTime.Now.AddDays(30);
                }
                else
                {
                    Response.Cookies["UEUname"].Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies["UEPwd"].Expires = DateTime.Now.AddDays(-1);
                }

                Response.Cookies["UEUname"].Value = TextBox1.Text;
                Response.Cookies["UEPwd"].Value = TextBox2.Text;
            }
            else
                Label1.Text = "Неправильный логин или пароль!";
        }
    }
}