using System;

namespace UEBase.user_controls
{
    public partial class Message : System.Web.UI.UserControl
    {

        public string SetMessage
        {
            get { return MessageLbl.Text; }
            set { MessageLbl.Text = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}