using System;
using System.Configuration;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace UEBase.forms_catalog
{
    public partial class models : System.Web.UI.Page
    {

        string commStr
        {
            get
            {
                return Session["_modelsSelect"]?.ToString();
            }
            set
            {
                Session["_modelsSelect"] = value;
            }
        }


        public CUser user
        {
            get
            {
                return Session["_UEuser"] as CUser;
            }
        }



        protected void Page_Init(object sender, EventArgs e)
        {
            if (!DesignMode && !Page.IsPostBack)
            {
                commStr = "SELECT t.value,m.name,m.id FROM models m JOIN ue_type t ON m.type_id=t.id ORDER BY m.name ASC";
                if (DropDownList1.Items.Count == 0)
                {
                    filterDdl.Items.Add(new ListItem("Все типы", "-1"));
                    MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQLString"].ConnectionString);
                    MySqlCommand comm = new MySqlCommand("SELECT value,id FROM ue_type ORDER BY value ASC", conn);
                    conn.Open();
                    MySqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        DropDownList1.Items.Add(new ListItem(reader.GetString(0), reader.GetString(1)));
                        filterDdl.Items.Add(new ListItem(reader.GetString(0), reader.GetString(1)));
                    }
                    reader.Close();
                    conn.Close();
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!user.superAdmin)
                Response.Redirect("~/default.aspx");
            MSQLData.SelectCommand = commStr;
            Q1.btnHandler += Q1_btnHandler;

        }

        private void Q1_btnHandler(string value)
        {
            if (value.Contains("ModelDelete"))
            {
                MSQLData.DeleteCommand = "DELETE FROM models WHERE id=@id";
                MSQLData.DeleteParameters.Add("@id", value.Split('|')[1]);
                MSQLData.Delete();
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TextBox1.Text))
            {
                M1.SetMessage = "Поле название быть заполнено!";
                MessageExtender.Show();
                return;
            }

            if (Button1.Text.Equals("Добавить"))
            {
                MSQLData.InsertCommand = "INSERT IGNORE INTO models (type_id,name) VALUES (@tid,@name)";
                MSQLData.InsertParameters.Add("@name", TextBox1.Text.Trim());
                MSQLData.InsertParameters.Add("@tid", DropDownList1.SelectedValue);
                MSQLData.Insert();
            }
            else if (Button1.Text.Equals("Изменить"))
            {
                MSQLData.UpdateCommand = "UPDATE IGNORE models SET type_id=@tid,name=@name WHERE id=@id";
                MSQLData.UpdateParameters.Add("@name", TextBox1.Text.Trim());
                MSQLData.UpdateParameters.Add("@tid", DropDownList1.SelectedValue);
                MSQLData.UpdateParameters.Add("@id", GridView1.SelectedDataKey.Value.ToString());
                MSQLData.Update();
                Button1.Text = "Добавить";
                GridView1.SelectedIndex = -1;
            }
            TextBox1.Text = string.Empty;
        }

        protected void GridView1_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            if (e.NewSelectedIndex > -1)
            {
                Button1.Text = "Изменить";
                TextBox1.Text = GridView1.Rows[e.NewSelectedIndex].Cells[2].Text;
                for (int i = 0; i < DropDownList1.Items.Count; i++)
                {
                    if (DropDownList1.Items[i].Text == GridView1.Rows[e.NewSelectedIndex].Cells[1].Text)
                    {
                        DropDownList1.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            MSQLData.DeleteCommand = "SELECT 1";
            Q1.SetQuestion = "Вы действительно хотите удалить модель: <b>" + GridView1.Rows[e.RowIndex].Cells[2].Text + "</b>?";
            Q1.Hidden = "ModelDelete|" + e.Keys["id"];
            QuestionExtender.Show();
        }

        protected void filterDdl_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridView1.SelectedIndex = -1;
            Button1.Text = "Добавить";
            TextBox1.Text = string.Empty;
            if (filterDdl.SelectedIndex > 0)
                commStr = "SELECT t.value,m.name,m.id FROM models m JOIN ue_type t ON m.type_id=t.id WHERE m.type_id=" + filterDdl.SelectedValue + " ORDER BY m.name ASC";
            else
                commStr = "SELECT t.value,m.name,m.id FROM models m JOIN ue_type t ON m.type_id=t.id ORDER BY m.name ASC";
            MSQLData.SelectCommand = commStr;
        }
    }
}