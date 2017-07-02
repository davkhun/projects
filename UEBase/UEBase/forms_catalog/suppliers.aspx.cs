using System;
using System.Web.UI.WebControls;

namespace UEBase
{
    public partial class suppliers : System.Web.UI.Page
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
            MaintainScrollPositionOnPostBack = true;
            if (!user.superAdmin)
                Response.Redirect("~/default.aspx");
            Q1.btnHandler += Q1_btnHandler;
        }

        private void Q1_btnHandler(string value)
        {
            if (value.Contains("SupplierDelete"))
            {
                MSQLData.DeleteCommand = "DELETE FROM suppliers WHERE id=@id";
                MSQLData.DeleteParameters.Add("@id", value.Split('|')[1]);
                MSQLData.Delete();
            }
        }

        protected void addRecBtn_Click(object sender, EventArgs e)
        {
            newRecDiv.Visible = !newRecDiv.Visible;
            GridView1.EditIndex = -1;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(shortNameT.Text))
            {
                M1.SetMessage = "Краткое наименование должно быть заполнено!";
                MessageExtender.Show();
                return;
            }
            if (string.IsNullOrEmpty(nameT.Text))
            {
                M1.SetMessage = "Наименование должно быть заполнено!";
                MessageExtender.Show();
                return;
            }
            MSQLData.InsertCommand = "INSERT IGNORE INTO suppliers (name,short_name,inn,address,site,manager,phone,email) VALUES (@name,@short_name,@inn,@address,@site,@manager,@phone,@email)";
            MSQLData.InsertParameters.Add("@name", nameT.Text);
            MSQLData.InsertParameters.Add("@short_name", shortNameT.Text);
            MSQLData.InsertParameters.Add("@inn", innT.Text);
            MSQLData.InsertParameters.Add("@address", addressT.Text);
            MSQLData.InsertParameters.Add("@site", siteT.Text);
            MSQLData.InsertParameters.Add("@manager", managerT.Text);
            MSQLData.InsertParameters.Add("@phone", phoneT.Text.Replace("\n","<br/>"));
            MSQLData.InsertParameters.Add("@email", emailT.Text);
            MSQLData.Insert();

            nameT.Text = string.Empty;
            innT.Text = string.Empty;
            addressT.Text = string.Empty;
            siteT.Text = string.Empty;
            managerT.Text = string.Empty;
            phoneT.Text = string.Empty;
            emailT.Text = string.Empty;
            shortNameT.Text = string.Empty;
            newRecDiv.Visible = false;
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            MSQLData.DeleteCommand = "SELECT 1";
            Q1.SetQuestion = "Вы действительно хотите удалить поставщика: <b>" + (GridView1.Rows[e.RowIndex].FindControl("nameL") as Label).Text + "</b>?";
            Q1.Hidden = "SupplierDelete|" + e.Keys["id"];
            QuestionExtender.Show();
        }

        protected void GridView1_PreRender(object sender, EventArgs e)
        {
            if (GridView1.EditIndex != -1)
            {
                GridViewRow r = GridView1.Rows[GridView1.EditIndex];
                foreach (TableCell tc in r.Cells)
                {
                    foreach (object c in tc.Controls)
                    {
                        if (c.GetType().Name == "TextBox")
                        {
                            string evt = Page.ClientScript.GetPostBackClientHyperlink(GridView1, "Update$" + GridView1.EditIndex.ToString());
                            System.Text.StringBuilder js = new System.Text.StringBuilder();
                            js.Append("if(event.which || event.keyCode)");
                            js.Append("{ if ((event.which == 13) || (event.keyCode == 13)) ");
                            js.Append($"{{{evt};return false;}}}}");
                            string strJs = js.ToString();
                            ((TextBox)(c)).Attributes.Add("onkeydown", strJs);
                            
                        }
                    }
                }
            }
            if (GridView1.Rows.Count > 0)
            {
                GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            MSQLData.UpdateCommand = "UPDATE suppliers SET short_name=@sname,inn=@inn,address=@address,site=@site,manager=@manager,phone=@phone,email=@email WHERE id=@id";
            MSQLData.UpdateParameters.Add("@inn", e.NewValues["inn"]?.ToString());
            MSQLData.UpdateParameters.Add("@address", e.NewValues["address"]?.ToString());
            MSQLData.UpdateParameters.Add("@site", e.NewValues["site"]?.ToString());
            MSQLData.UpdateParameters.Add("@manager", e.NewValues["manager"]?.ToString());
            MSQLData.UpdateParameters.Add("@phone", e.NewValues["phone"]?.ToString());
            MSQLData.UpdateParameters.Add("@email", e.NewValues["email"]?.ToString());
            MSQLData.UpdateParameters.Add("@sname", e.NewValues["short_name"]?.ToString());
            MSQLData.UpdateParameters.Add("@id", GridView1.DataKeys[e.RowIndex].Value.ToString());
            MSQLData.Update();
        }
    }
}