using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace zakupki_ftp
{
    internal class NotificationB2B
    {
        private XmlDocument doc;
        private XmlNode node;


        private List<schemas.Notification> NClist;

        private readonly nParse NParse = new nParse();

        private readonly MySqlConnection conn;
        private MySqlCommand comm;

        private string commStr;
        private readonly string nodePath;
        private readonly string path;
        public NotificationB2B(string Path)
        {
            conn = new MySqlConnection(ConfigurationManager.AppSettings["ConnString"]);
            nodePath = "rss/channel/item";
            path = Path + "b2b.xml";
        }

        private string GetNodeText(string nod, XmlNode xmlnode = null)
        {
            if (xmlnode == null) xmlnode = node;
            if (xmlnode.SelectSingleNode(nod) == null)
                return null;
            else
            {
                string result = xmlnode.SelectSingleNode(nod).InnerText;
                return result.Length > 2000 ? result.Remove(2000) : result;
            }
        }

        private string GetLastIDfromNotice()
        {
            comm = new MySqlCommand("SELECT MAX(id) FROM tenderbase.notification WHERE type='B2B'", conn);
            return Convert.ToString(comm.ExecuteScalar());
        }

        private int ExecuteNonQuery(MySqlCommand comm)
        {
            try
            {
                int result = comm.ExecuteNonQuery();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка запроса: " + ex);
                Console.WriteLine(commStr);
                return -1;
            }
        }

        private static string RegionParse(string code)
        {
            switch (code)
            {
                case "1": return "Адыгея"; 
                case "2": return "Башкортостан"; 
                case "3": return "Бурятия"; 
                case "4": return "Алтай"; 
                case "5": return "Дагестан"; 
                case "6": return "Ингушетия"; 
                case "7": return "Кабардино-Балкарская респ"; 
                case "8": return "Калмыкия"; 
                case "9": return "Карачаево-Черкесская респ"; 
                case "10": return "Карелия"; 
                case "11": return "Коми"; 
                case "12": return "Марий Эл"; 
                case "13": return "Мордовия"; 
                case "14": return "Саха-Якутия"; 
                case "15": return "Северная Осетия - Алания"; 
                case "16": return "Татарстан"; 
                case "17": return "Тува"; 
                case "18": return "Удмуртия"; 
                case "19": return "Хакасия"; 
                case "20": return "Чечня"; 
                case "21": return "Чувашия"; 
                case "22": return "Алтайский край"; 
                case "23": return "Красноярский край"; 
                case "24": return "Краснодарский край"; 
                case "25": return "Приморский край"; 
                case "26": return "Ставропольский край"; 
                case "27": return "Хабаровск"; 
                case "28": return "Амурская обл"; 
                case "29": return "Архангельская обл"; 
                case "30": return "Астраханская обл"; 
                case "31": return "Белгородская обл"; 
                case "32": return "Брянская обл"; 
                case "33": return "Владимирская обл"; 
                case "34": return "Волгоградская обл"; 
                case "35": return "Вологодская обл"; 
                case "36": return "Воронежская обл"; 
                case "37": return "Ивановская обл"; 
                case "38": return "Иркутская обл"; 
                case "39": return "Калининградская обл"; 
                case "40": return "Калужская обл"; 
                case "41": return "Камчатский край"; 
                case "42": return "Кемеровская обл"; 
                case "43": return "Кировская обл"; 
                case "44": return "Костромская обл"; 
                case "45": return "Курганская обл"; 
                case "46": return "Курская обл"; 
                case "47": return "Ленинградская обл"; 
                case "48": return "Липецкая обл"; 
                case "49": return "Магаданская обл"; 
                case "50": return "Московская обл"; 
                case "51": return "Мурманская обл"; 
                case "52": return "Нижегородская обл"; 
                case "53": return "Новгородская обл"; 
                case "54": return "Новосибирская обл"; 
                case "55": return "Омская обл"; 
                case "56": return "Оренбургская обл"; 
                case "57": return "Орловская обл"; 
                case "58": return "Пензенская обл"; 
                case "59": return "Пермский край"; 
                case "60": return "Псковская обл"; 
                case "61": return "Ростовская обл"; 
                case "62": return "Рязанская обл"; 
                case "63": return "Самарская обл"; 
                case "64": return "Саратовская обл"; 
                case "65": return "Сахалинская обл"; 
                case "66": return "Свердловская обл"; 
                case "67": return "Смоленская обл"; 
                case "68": return "Тамбовская обл"; 
                case "69": return "Тверская обл"; 
                case "70": return "Томская обл"; 
                case "71": return "Тульская обл"; 
                case "72": return "Тюменская обл"; 
                case "73": return "Ульяновская обл"; 
                case "74": return "Челябинская обл"; 
                case "75": return "Забайкальский край"; 
                case "76": return "Ярославская обл"; 
                case "77": return "Москва"; 
                case "78": return "Санкт-Петербург"; 
                case "79": return "Еврейская АО"; 
                case "80": return "Забайкальский край"; 
                case "81": return "Коми"; 
                case "82": return "82"; 
                case "83": return "Ненецкий АО"; 
                case "84": return "84";
                case "85": return "Иркутская обл"; 
                case "86": return "Ханты-Мансийский АО"; 
                case "87": return "Чукотский АО";
                case "88": return "Ямало-Ненецкий АО"; 
                case "99": return "Байконур"; 
                default: return null;
            }
            
        }

        private void ParseB2B(string xmlPath)
        {
            doc = new XmlDocument();
            doc.Load(xmlPath);
            NClist = new List<schemas.Notification>();
            for (int i = 0; i < doc.SelectNodes(nodePath).Count; i++)
            {
                node = doc.SelectNodes(nodePath)[i];
                schemas.Notification NC = new schemas.Notification();
                NC.notice.notificationType = GetNodeText("title");
                NC.notice.href = GetNodeText("link");
                NC.notice.orderName = GetNodeText("description");
                NC.notice.notificationNumber = GetNodeText("guid");
                NC.notice.publishDate = Common.IsDate(GetNodeText("pubDate"));
                NC.notice.orgName = GetNodeText("orgName");
                NC.notice.orgINN = GetNodeText("orgInn");
                // фейковый лот для суммы и валюты
                NC.oneLot.maxPrice = GetNodeText("price");
                NC.oneLot.currencyName = GetNodeText("currency"); 
                NC.notice.region = GetNodeText("region");
                NC.notice.okdpPath = GetNodeText("okdpPaths");
                if (!string.IsNullOrEmpty(NC.notice.okdpPath))
                {
                    NC.notice.okdpPath = NC.notice.okdpPath.Replace(",,", ",");
                    NC.notice.okdpPath = NC.notice.okdpPath.Remove(0, 1);
                    NC.notice.okdpPath = NC.notice.okdpPath.Remove(NC.notice.okdpPath.Length - 1, 1);

                }
                NC.notice.notificationType = NC.notice.notificationType.Replace("&quot;", "\"");
                NC.notice.orderName = NC.notice.orderName.Replace("&quot;", "\"");
                NC.notice.orgName = NC.notice.orgName.Replace("&quot;", "\"");
                NClist.Add(NC);
            }
            File.Delete(path);
        }

        private void InsertIntoBaseB2B()
        {
            foreach (schemas.Notification NC in NClist)
            {
                commStr = @"INSERT IGNORE tenderbase.notification (notificationType,href,orderName,notificationNumber,publishDate,orgName,orgINN,region,have_eng,type) 
                                                         VALUES (@notificationType,@href,@orderName,@notificationNumber,@publishDate,@orgName,@orgINN,@region,@have_eng,'B2B')";
                comm = new MySqlCommand(commStr, conn);
                comm.Parameters.AddWithValue("@notificationType", NC.notice.notificationType);
                comm.Parameters.AddWithValue("@href", NC.notice.href);
                comm.Parameters.AddWithValue("@orderName", NC.notice.orderName);
                comm.Parameters.AddWithValue("@notificationNumber", NC.notice.notificationNumber);
                comm.Parameters.AddWithValue("@publishDate", NC.notice.publishDate);
                comm.Parameters.AddWithValue("@orgName", NC.notice.orgName);
                comm.Parameters.AddWithValue("@orgINN", NC.notice.orgINN);
                comm.Parameters.AddWithValue("@region", RegionParse(NC.notice.region));
                comm.Parameters.AddWithValue("@have_eng", (NParse.NavalnyParse(NC.notice.orderName) ? "1" : "0"));
                if (ExecuteNonQuery(comm) == 0) // если команда выдает 0 - значит запись уже есть, обновляем publishDate, иначе, вставляем как новую
                {
                    commStr = @"UPDATE tenderbase.notification SET publishDate=@publishDate WHERE notificationNumber=@notificationNumber";
                    comm = new MySqlCommand(commStr, conn);
                    comm.Parameters.AddWithValue("@publishDate", NC.notice.publishDate);
                    comm.Parameters.AddWithValue("@notificationNumber", NC.notice.notificationNumber);
                    ExecuteNonQuery(comm);
                }
                else
                {
                    string nID = GetLastIDfromNotice();
                    // вставляем фейковый лот
                    commStr = @"INSERT INTO tenderbase.lot (notificationID,currencyName,maxPrice,type) VALUES (@notificationID,@currencyName,@maxPrice,'B2B')";
                    comm = new MySqlCommand(commStr, conn);
                    comm.Parameters.AddWithValue("@notificationID", nID);
                    comm.Parameters.AddWithValue("@currencyName", NC.oneLot.currencyName);
                    comm.Parameters.AddWithValue("@maxPrice", NC.oneLot.maxPrice);
                    ExecuteNonQuery(comm);
                    foreach (string oneOKDP in NC.notice.okdpPath.Split(','))
                    {
                        comm = new MySqlCommand("INSERT INTO tenderbase.products (notificationID,code) VALUES (@notificationID,@code)", conn);
                        comm.Parameters.AddWithValue("@notificationID", nID);
                        comm.Parameters.AddWithValue("@code", oneOKDP);
                        ExecuteNonQuery(comm);
                    }
                }
            }
        }

        public void DoWork()
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create("http://www.b2b-center.ru/market/?export=rss2");
            System.Net.WebResponse resp = req.GetResponse();
            StreamReader sr = new StreamReader(resp.GetResponseStream());
            using (StreamWriter sw = new StreamWriter(path, false))
                sw.Write(sr.ReadToEnd().Trim());
            try
            {
                if (!Directory.Exists(path + "done"))
                    Directory.CreateDirectory(path + "done");
                if (!Directory.Exists(path + "unknown"))
                    Directory.CreateDirectory(path + "unknown");
                ParseB2B(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Logger.WriteLogB2B("Ошибка парсинга: " + ex.Message);
            }

            conn.Open();
            InsertIntoBaseB2B();
            conn.Close();


        }
    }
}
