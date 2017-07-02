using System;

namespace UEBase.user_controls
{
    public partial class Question : System.Web.UI.UserControl
    {

        public delegate void OnButtonClick(string value);

        public event OnButtonClick btnHandler;

        public string SetQuestion
        {
            get { return QuestionLbl.Text; }
            set { QuestionLbl.Text = value; }
        }

        public string Hidden
        {
            get { return HiddenValue.Value; }
            set { HiddenValue.Value = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void yesBtn_Click(object sender, EventArgs e)
        {
            string handler = Hidden;
            if (handler != null)
                btnHandler?.Invoke(handler);
        }
    }
}