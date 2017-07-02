using System;

namespace zakupki_ftp
{
    public static class Common
    {
        public static string IsDate(string value)
        {
            DateTime dtOut;
            return DateTime.TryParse(value, out dtOut) ? Convert.ToDateTime(value).ToString("yyyy-MM-dd hh-mm-ss") : null;
        }

        public static string IsNumericFraud(string value)
        {
            if (value != null)
            {
                double res;
                return double.TryParse(value, out res) ? value.Replace(',', '.') : null;
            }
            return null;
        }

        public static string IsNumeric(string value)
        {
            if (value != null)
            {
                value = value.Replace('.', ',');
                double res;
                return double.TryParse(value, out res) ? value.Split(',')[0] : null;
            }
            return null;
        }
    }
}
