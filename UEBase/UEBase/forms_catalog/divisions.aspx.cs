using System;
using System.Web.UI.WebControls;

namespace UEBase.forms_catalog
{
    public partial class divisions : System.Web.UI.Page
    {


        public CUser user
        {
            get
            {
                return Session["_UEuser"] as CUser;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!user.superAdmin)
                Response.Redirect("~/default.aspx");
            Q1.btnHandler += Q1_btnHandler;

        }

        private void Q1_btnHandler(string value)
        {
            if (value.Contains("DivisionDelete"))
            {
                MSQLData.DeleteCommand = "DELETE FROM divisions WHERE id=@id";
                MSQLData.DeleteParameters.Add("@id", value.Split('|')[1]);
                MSQLData.Delete();
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(TextBox1.Text))
            {
                M1.SetMessage = "Поле код быть заполнено!";
                MessageExtender.Show();
                return;
            }

            if (Button1.Text.Equals("Добавить"))
            {
                MSQLData.InsertCommand = "INSERT IGNORE INTO divisions (code,description) VALUES (@code,@descr)";
                MSQLData.InsertParameters.Add("@code", TextBox1.Text.Trim());
                MSQLData.InsertParameters.Add("@descr", TextBox2.Text.Trim());
                MSQLData.Insert();
            }
            else if (Button1.Text.Equals("Изменить"))
            {
                MSQLData.UpdateCommand = "UPDATE IGNORE divisions SET code=@code,description=@descr WHERE id=@id";
                MSQLData.UpdateParameters.Add("@code", TextBox1.Text.Trim());
                MSQLData.UpdateParameters.Add("@descr", TextBox2.Text.Trim());
                MSQLData.UpdateParameters.Add("@id", GridView1.SelectedDataKey.Value.ToString());
                MSQLData.Update();
                Button1.Text = "Добавить";
                GridView1.SelectedIndex = -1;
            }
            TextBox1.Text = string.Empty;
            TextBox2.Text = string.Empty;
        }

        protected void GridView1_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            if (e.NewSelectedIndex > -1)
            {
                Button1.Text = "Изменить";
                TextBox1.Text = GridView1.Rows[e.NewSelectedIndex].Cells[1].Text;
                TextBox2.Text = GridView1.Rows[e.NewSelectedIndex].Cells[2].Text;
            }
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            MSQLData.DeleteCommand = "SELECT 1";
            Q1.SetQuestion = "Вы действительно хотите удалить подразделение: <b>" + GridView1.Rows[e.RowIndex].Cells[1].Text + "</b>?";
            Q1.Hidden = "DivisionDelete|" + e.Keys["id"];
            QuestionExtender.Show();
        }
    }
}