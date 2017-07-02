using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace zakupki_ftp
{
    internal class FraudNotice
    {
        private MySqlCommand comm;
        private readonly MySqlConnection conn;
        private readonly nParse NParse;

        public FraudNotice()
        {
            conn = new MySqlConnection(ConfigurationManager.AppSettings["ConnString"]);
            NParse = new nParse();
        }

        public struct fraudNotice
        {
            public string notificationType;
            public string notificationNumber;
            public string orderName;
            public string publishDate;
            public string status;
            public string region;
            public string maxPrice;
            public string type;
            public string href;
        }

        public void InsertFraudNotice()
        {
            List<fraudNotice> fLNotice = new List<fraudNotice>();
            comm = new MySqlCommand("SELECT orderName,notificationNumber,status,notificationType,maxPrice,publishDate,region,lot.type,href FROM notification,lot WHERE lot.notificationID=notification.id AND have_eng='1' AND publishDate>(SELECT MAX(publishDate) FROM notification) GROUP BY notificationNumber", conn);
            conn.Open();
            MySqlDataReader reader = comm.ExecuteReader();
            while (reader.Read())
            {
                fraudNotice fNotice = new fraudNotice
                {
                    orderName = NParse.LetChange(reader.GetString(0)),
                    notificationNumber = reader.IsDBNull(1) ? null : reader.GetString(1),
                    status = reader.IsDBNull(2) ? null : reader.GetString(2),
                    notificationType = reader.GetString(3),
                    maxPrice = reader.IsDBNull(4) ? null : reader.GetString(4),
                    publishDate = reader.GetString(5),
                    region = reader.GetString(6),
                    type = reader.GetString(7),
                    href = reader.IsDBNull(8) ? null : reader.GetString(8)
                };
                fLNotice.Add(fNotice);
            }
            reader.Close();
            foreach (fraudNotice oneNotice in fLNotice)
            {
                comm = new MySqlCommand("INSERT INTO 223fraudNotice (noticeType,registrationNumber,name,publicationDateTime,status,region,sum,FZtype,href) VALUES (@noticeType,@registrationNumber,@name,@publicationDateTime,@status,@region,@sum,@fztype,@href)", conn);
                comm.Parameters.AddWithValue("@noticeType", oneNotice.notificationType);
                comm.Parameters.AddWithValue("@registrationNumber", oneNotice.notificationNumber);
                comm.Parameters.AddWithValue("@name", oneNotice.orderName);
                comm.Parameters.AddWithValue("@publicationDateTime", Common.IsDate(oneNotice.publishDate));
                comm.Parameters.AddWithValue("@status", oneNotice.status);
                comm.Parameters.AddWithValue("@region", oneNotice.region);
                comm.Parameters.AddWithValue("@fztype", oneNotice.type);
                comm.Parameters.AddWithValue("@href", oneNotice.href);
                comm.Parameters.AddWithValue("@sum", Common.IsNumericFraud(oneNotice.maxPrice));
                comm.ExecuteNonQuery();
            }
            conn.Close();
        }

    }
}
