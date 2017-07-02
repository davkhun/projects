using System;
using System.Configuration;
using System.Linq;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace UEBase
{
    public partial class admin : System.Web.UI.Page
    {

        public CUser user
        {
            get
            {
                return Session["_UEuser"] as CUser;
            }
        }

        private MySqlConnection conn;
        private MySqlCommand comm;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!DesignMode && !Page.IsPostBack)
            {
                if (divisionsCbl.Items.Count == 0)
                {
                    conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQLString"].ConnectionString);
                    comm = new MySqlCommand("SELECT concat(code,' - ',description),id FROM divisions ORDER BY code ASC", conn);
                    conn.Open();
                    MySqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                        divisionsCbl.Items.Add(new ListItem(reader.GetString(0), reader.GetString(1)));
                    reader.Close();
                    conn.Close();
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!user.superAdmin)
                Response.Redirect("~/default.aspx");
            conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQLString"].ConnectionString);
            if (!Page.IsPostBack)
                GridView1.DataBind();
            Q1.btnHandler += Q1_btnHandler;
        }

        void Q1_btnHandler(string value)
        {
            if (value.Contains("UserDelete"))
            {
                MSQLData.DeleteCommand = "DELETE FROM rights WHERE id=@id";
                MSQLData.DeleteParameters.Add("@id", value.Split('|')[1]);
                MSQLData.Delete();
            }
        }

        protected void newUserBtn_Click(object sender, EventArgs e)
        {
            GridView1.EditIndex = -1;
            newUserPanel.Visible = !newUserPanel.Visible;
        }

        protected void findByUnameBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(unameT.Text))
            {
                ADconnector ad = new ADconnector();
                ADuser usr = ad.GetByUname(unameT.Text);
                if (string.IsNullOrEmpty(usr.name))
                {
                    fioT.Text = "<Пользователь не найден!>";
                    AddUserBtn.Enabled = false;
                }
                else
                {
                    fioT.Text = usr.name;
                    AddUserBtn.Enabled = true;
                    officeT.Text = usr.office;
                }
            }
        }

        protected void AddUserBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(unameT.Text))
            {
                M1.SetMessage = "Доменное имя не должно быть пустым!";
                MessageExtender.Show();
                return;
            }
            if (fioT.Text == "<Пользователь не найден!>")
            {
                M1.SetMessage = "Пользователь не найден по доменному имени!";
                MessageExtender.Show();
                return;
            }
            if (string.IsNullOrEmpty(fioT.Text))
            {
                M1.SetMessage = "Необходимо найти пользователя по доменному имени!";
                MessageExtender.Show();
                return;
            }
            // собираем список подразделений
            string selectedDivistions = string.Join(",", divisionsCbl.Items.Cast<ListItem>().Where(li => li.Selected).Select(li => li.Value).ToList());
            if (string.IsNullOrEmpty(selectedDivistions) && !superadminCb.Checked)
            {
                M1.SetMessage = "Не выбрано ни одного подразделения!";
                MessageExtender.Show();
                return;
            }
            comm = new MySqlCommand("INSERT IGNORE INTO rights (uname,fio,divisions_id,office,admin,super_admin) VALUES (@uname,@fio,@divisions_id,@office,@admin,@superadmin)", conn);
            comm.Parameters.AddWithValue("@uname", unameT.Text.Trim());
            comm.Parameters.AddWithValue("@fio", fioT.Text.Trim());
            comm.Parameters.AddWithValue("@office", officeT.Text);
            comm.Parameters.AddWithValue("@admin", adminCb.Checked ? 1 : 0);
            comm.Parameters.AddWithValue("@superadmin", superadminCb.Checked ? 1 : 0);
            comm.Parameters.AddWithValue("@divisions_id", string.IsNullOrEmpty(selectedDivistions) ? null : selectedDivistions);
            conn.Open();
            comm.ExecuteNonQuery();
            conn.Close();
            GridView1.DataBind();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label l = e.Row.FindControl("divLbl") as Label;
                if (!string.IsNullOrEmpty(l?.Text))
                {
                    string divs = null;
                    foreach (string one in l.Text.Split(','))
                    {

                        foreach (ListItem li in divisionsCbl.Items)
                            if (one == li.Value)
                                divs += li.Text + "<br/>";
                    }
                    if (!string.IsNullOrEmpty(divs))
                    {
                        divs = divs.Remove(divs.Length - 1, 1);
                        l.Text = divs;
                    }
                }

                if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                {
                    CheckBoxList cbl = e.Row.FindControl("divCbl") as CheckBoxList;
                    Label l1 = e.Row.FindControl("divLbl1") as Label;
                    foreach (ListItem one in divisionsCbl.Items)
                        cbl.Items.Add(one);
                    for (int i = 0; i < cbl.Items.Count; i++)
                        cbl.Items[i].Selected = false;
                    if (!string.IsNullOrEmpty(l1.Text))
                    {

                        foreach (string oneValue in l1.Text.Split(','))
                        {
                            for (int i = 0; i < cbl.Items.Count; i++)
                            {
                                if (cbl.Items[i].Value == oneValue)
                                {
                                    cbl.Items[i].Selected = true;
                                    break;
                                }
                            }
                        }
                    }

                }
            }
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            CheckBox adm = GridView1.Rows[e.RowIndex].FindControl("adminC") as CheckBox;
            CheckBoxList cbl = GridView1.Rows[e.RowIndex].FindControl("divCbl") as CheckBoxList;
            string selectedDivistions = string.Join(",", cbl.Items.Cast<ListItem>().Where(li => li.Selected).Select(li => li.Value).ToList());
            MSQLData.UpdateCommand = "UPDATE rights SET admin=@adm,divisions_id=@divs WHERE id=@id";
            MSQLData.UpdateParameters.Add("@adm", adm.Checked ? "1" : "0");
            MSQLData.UpdateParameters.Add("@divs", selectedDivistions);
            MSQLData.UpdateParameters.Add("@id", e.Keys["id"].ToString());
            
        }

        protected void adminCb_CheckedChanged(object sender, EventArgs e)
        {
            if (superadminCb.Checked)
            {
                foreach (ListItem li in divisionsCbl.Items)
                    li.Selected = false;
                divisionsCbl.Enabled = false;
                adminCb.Checked = false;
                adminCb.Enabled = false;
            }
            else
            {
                divisionsCbl.Enabled = true;
                adminCb.Enabled = true;
            }
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            MSQLData.DeleteCommand = "SELECT 1";
            Q1.Hidden = "UserDelete|" + e.Keys["id"];
            QuestionExtender.Show();
            Q1.SetQuestion = "Удалить пользователя <b>" + GridView1.Rows[e.RowIndex].Cells[1].Text + "</b>?";
            QuestionExtender.Show();
        }
    }
}