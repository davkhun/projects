using System;
using System.Web.UI.WebControls;

namespace UEBase
{
    public partial class uetype : System.Web.UI.Page
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
            if (value.Contains("TypeDelete"))
            {
                MSQLData.DeleteCommand = "DELETE FROM ue_type WHERE id=@id";
                MSQLData.DeleteParameters.Add("@id", value.Split('|')[1]);
                MSQLData.Delete();
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TextBox1.Text))
            {
                M1.SetMessage = "Поле должно быть заполнено!";
                MessageExtender.Show();
                return;
            }

            if (Button1.Text.Equals("Добавить"))
            {
                MSQLData.InsertCommand = "INSERT IGNORE INTO ue_type (value) VALUES (@type)";
                MSQLData.InsertParameters.Add("@type", TextBox1.Text.Trim());
                MSQLData.Insert();
            }
            else if (Button1.Text.Equals("Изменить"))
            {
                MSQLData.UpdateCommand = "UPDATE IGNORE ue_type SET value=@type WHERE id=@id";
                MSQLData.UpdateParameters.Add("@type", TextBox1.Text.Trim());
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
                TextBox1.Text = GridView1.Rows[e.NewSelectedIndex].Cells[1].Text;
            }
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            MSQLData.DeleteCommand = "SELECT 1";
            Q1.SetQuestion = "Вы действительно хотите удалить тип: <b>" + GridView1.Rows[e.RowIndex].Cells[1].Text + "</b>?";
            Q1.Hidden = "TypeDelete|" + e.Keys["id"];
            QuestionExtender.Show();
        }
    }
}