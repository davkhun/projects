using System;
using System.Web.UI;

namespace UEBase
{
    public partial class UE : MasterPage
    {

        public CUser user => Session["_UEuser"] as CUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            // проверяем все права и отображаем только то, что нужно
            if (user == null)
                Response.Redirect("login.aspx");
            // если обычный пользователь - недоступно ничего
            if (!user.registered)
            {
                catalogA.Visible = false;
                adminLi.Visible = false;
            }
            // если обычный администратор - недоступны справочники
            if (user.admin)
            {
                adminLi.Visible = false;
                suppliersLi.Visible = false;
                statusLi.Visible = false;
                uetypeLi.Visible = false;
                modelsLi.Visible = false;
                divisionsLi.Visible = false;

            }
            // если обычный, зарегистрированный пользователь - недоступны все справочники и админка
            if (!user.admin && !user.superAdmin)
            {
                catalogA.Visible = false;
                adminLi.Visible = false;
            }
        }
    }
}