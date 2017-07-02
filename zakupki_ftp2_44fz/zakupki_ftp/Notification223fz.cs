using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace zakupki_ftp
{
    internal class Notification223fz
    {
     //   public string startupPath;

        public struct Surls
        {
            public string url;
            public string region;
            public Surls(string URL, string REGION)
            {
                url = URL;
                region = REGION;
            }
        }

        public List<Surls> urls;

        public struct Sregions
        {
            public string eng;
            public string name;
            public Sregions(string ENG, string NAME)
            {
                eng = ENG;
                name = NAME;
            }
        }

        public List<Sregions> regions;

        private XmlDocument doc;
        private XmlNamespaceManager xnm;
        private XmlNode node;

        private MySqlCommand comm;
        private readonly MySqlConnection conn;

        private List<schemas.Notification> Nlist;

        private readonly string path;

        public string region;

        public string commStr;

        private readonly nParse NParse = new nParse();

        private const string startNodeN = "ns2:purchaseNotice/ns2:body/ns2:item/ns2:purchaseNoticeData";
        private const string startNodeAE = "ns2:purchaseNoticeAE/ns2:body/ns2:item/ns2:purchaseNoticeAEData";
        private const string startNodeEP = "ns2:purchaseNoticeEP/ns2:body/ns2:item/ns2:purchaseNoticeEPData";
        private const string startNodeOA = "ns2:purchaseNoticeOA/ns2:body/ns2:item/ns2:purchaseNoticeOAData";
        private const string startNodeOK = "ns2:purchaseNoticeOK/ns2:body/ns2:item/ns2:purchaseNoticeOKData";
        private const string startNodeZK = "ns2:purchaseNoticeZK/ns2:body/ns2:item/ns2:purchaseNoticeZKData";


        private string GetNodeText(string nod, XmlNode xmlnode = null)
        {
            if (xmlnode == null) xmlnode = node;
            return xmlnode.SelectSingleNode(nod, xnm) == null ? null : xmlnode.SelectSingleNode(nod, xnm).InnerText;
        }

        private string NotNull(string str)
        {
            if (str != null)
            {
                str = str.Replace('\'', '"');
                str = str.Replace('\\', '/');
                return "'" + str + "'";
            }
            return "NULL";
        }

        private static int ExecuteNonQuery(MySqlCommand comm)
        {
            try
            {
                int result = comm.ExecuteNonQuery();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка запроса: " + ex);
                Logger.WriteLog223("Ошибка запроса: " + ex);
                return -1;
            }
        }

        // таблицы для получения последнего ID
        private enum ETables { notice, lot };

        private string GetLastIDfromNotice(ETables tbl)
        {
            switch (tbl)
            {
                case ETables.notice: comm = new MySqlCommand("SELECT MAX(id) FROM tenderbase.notification WHERE type='223'", conn); break;
                case ETables.lot: comm = new MySqlCommand("SELECT MAX(id) FROM tenderbase.lot WHERE type='223'", conn); break;
                default: return null;
            }
            return Convert.ToString(comm.ExecuteScalar());
        }


        #region Инициализация
        public Notification223fz(string Path)
        {
            conn = new MySqlConnection(ConfigurationManager.AppSettings["ConnString"]);
            path = Path;
            region = null;
            // чистим базу
            regions = new List<Sregions>
            {
                new Sregions("Adygeya_Resp", "Адыгея"),
                new Sregions("Altay_Resp", "Алтай"),
                new Sregions("Altayskii__krai", "Алтайский край"),
                new Sregions("Amurskaya_obl", "Амурская обл"),
                new Sregions("Arhangelskaya_obl", "Архангельская обл"),
                new Sregions("Astrahanskaya_obl", "Астраханская обл"),
                new Sregions("Baikonur_g", "Байконур"),
                new Sregions("Bashkortostan_Resp", "Башкортостан"),
                new Sregions("Belgorodskaya_obl", "Белгородская обл"),
                new Sregions("Brianskaya_obl", "Брянская обл"),
                new Sregions("Buryatiya_Resp", "Бурятия"),
                new Sregions("Chechenskaya_Resp", "Чечня"),
                new Sregions("Cheliabinskaya_obl", "Челябинская обл"),
                new Sregions("Chukotskii_AO", "Чукотский АО"),
                new Sregions("Chuvashskaya_Respublika", "Чувашия"),
                new Sregions("Dagestan_Resp", "Дагестан"),
                new Sregions("Evreiskaya_Aobl", "Еврейская АО"),
                new Sregions("Habarovskii_krai", "Хабаровск"),
                new Sregions("Hakasiia_Resp", "Хакасия"),
                new Sregions("Hanty-Mansiiskii_AO_Iugra_AO", "Ханты-Мансийский АО"),
                new Sregions("Ingushetiya_Resp", "Ингушетия"),
                new Sregions("Irkutskaya_obl", "Иркутская обл"),
                new Sregions("Irkutskaya_obl_Ust-Ordynskii_Buriatskii_okrug", "Иркутская обл, Бурятский окр"),
                new Sregions("Ivanowskaya_obl", "Ивановская обл"),
                new Sregions("Jamalo-Nenetckii_AO", "Ямало-Ненецкий АО"),
                new Sregions("Jaroslavskaya_obl", "Ярославская обл"),
                new Sregions("Kabardino-Balkarskaya_Resp", "Кабардино-Балкарская респ"),
                new Sregions("Kaliningradskaya_obl", "Калининградская обл"),
                new Sregions("Kalmykiya_Resp", "Калмыкия"),
                new Sregions("Kaluzhskaya_obl", "Калужская обл"),
                new Sregions("Kamchatskii_krai", "Камчатский край"),
                new Sregions("Karachaevo-Cherkesskaya_Resp", "Карачаево-Черкесская респ"),
                new Sregions("Kareliya_Resp", "Карелия"),
                new Sregions("Kemerowskaya_obl", "Кемеровская обл"),
                new Sregions("Kirowskaya_obl", "Кировская обл"),
                new Sregions("Komi_Resp", "Коми"),
                new Sregions("Kostromskaya_obl", "Костромская обл"),
                new Sregions("Krasnodarskii_krai", "Краснодарский край"),
                new Sregions("Krasnoyarskii_krai", "Красноярский край"),
                new Sregions("Kurganskaya_obl", "Курганская обл"),
                new Sregions("Kurskaya_obl", "Курская обл"),
                new Sregions("Leningradskaya_obl", "Ленинградская обл"),
                new Sregions("Lipetckaya_obl", "Липецкая обл"),
                new Sregions("Magadanskaya_obl", "Магаданская обл"),
                new Sregions("Marii_El_Resp", "Марий Эл"),
                new Sregions("Mordoviya_Resp", "Мордовия"),
                new Sregions("Moskovskaya_obl", "Московская обл"),
                new Sregions("Moskva", "Москва"),
                new Sregions("Murmanskaya_obl", "Мурманская обл"),
                new Sregions("Nenetckii_AO", "Ненецкий АО"),
                new Sregions("Nizhegorodskaya_obl", "Нижегородская обл"),
                new Sregions("Novgorodskaya_obl", "Новгородская обл"),
                new Sregions("Novosibirskaya_obl", "Новосибирская обл"),
                new Sregions("Omskaya_obl", "Омская обл"),
                new Sregions("Orenburgskaya_obl", "Оренбургская обл"),
                new Sregions("Orlovskaya_obl", "Орловская обл"),
                new Sregions("Penzenskaya_obl", "Пензенская обл"),
                new Sregions("Permskii_krai", "Пермский край"),
                new Sregions("Primorskii_krai", "Приморский край"),
                new Sregions("Pskovskaya_obl", "Псковская обл"),
                new Sregions("Ryazanskaya_obl", "Рязанская обл"),
                new Sregions("Rostovskaya_obl", "Ростовская обл"),
                new Sregions("Saha_Jakutiya_Resp", "Саха-Якутия"),
                new Sregions("Sahalinskaya_obl", "Сахалинская обл"),
                new Sregions("Samarskaya_obl", "Самарская обл"),
                new Sregions("Sankt-Peterburg", "Санкт-Петербург"),
                new Sregions("Saratovskaya_obl", "Саратовская обл"),
                new Sregions("Severnaia_Osetiya_Alaniia_Resp", "Северная Осетия - Алания"),
                new Sregions("Smolenskaya_obl", "Смоленская обл"),
                new Sregions("Stavropolskii_krai", "Ставропольский край"),
                new Sregions("Sverdlovskaya_obl", "Свердловская обл"),
                new Sregions("Tambovskaya_obl", "Тамбовская обл"),
                new Sregions("Tatarstan_Resp", "Татарстан"),
                new Sregions("Tiumenskaya_obl", "Тюменская обл"),
                new Sregions("Tomskaya_obl", "Томская обл"),
                new Sregions("Tulskaya_obl", "Тульская обл"),
                new Sregions("Tverskaya_obl", "Тверская обл"),
                new Sregions("Tyva_Resp", "Тува"),
                new Sregions("Udmurtskaya_Resp", "Удмуртия"),
                new Sregions("Ulianovskaya_obl", "Ульяновская обл"),
                new Sregions("Vladimirskaya_obl", "Владимирская обл"),
                new Sregions("Volgogradskaya_obl", "Волгоградская обл"),
                new Sregions("Vologodskaya_obl", "Вологодская обл"),
                new Sregions("Voronezhskaya_obl", "Воронежская обл"),
                new Sregions("Zabaikalskii_krai", "Забайкальский край"),
                new Sregions("Zabaikalskii_krai_Aginskii_Buriatskii_okrug", "Забайкальский край, Агинский округ")
            };

            urls = new List<Surls>();
            foreach (Sregions reg in regions)
            {
                urls.Add(new Surls("ftp://fz223free:fz223free@ftp.zakupki.gov.ru/out/published/" + reg.eng + "/purchaseNotice/daily/", reg.name));
                urls.Add(new Surls("ftp://fz223free:fz223free@ftp.zakupki.gov.ru/out/published/" + reg.eng + "/purchaseNoticeAE/daily/", reg.name));
                urls.Add(new Surls("ftp://fz223free:fz223free@ftp.zakupki.gov.ru/out/published/" + reg.eng + "/purchaseNoticeEP/daily/", reg.name));
                urls.Add(new Surls("ftp://fz223free:fz223free@ftp.zakupki.gov.ru/out/published/" + reg.eng + "/purchaseNoticeOA/daily/", reg.name));
                urls.Add(new Surls("ftp://fz223free:fz223free@ftp.zakupki.gov.ru/out/published/" + reg.eng + "/purchaseNoticeOK/daily/", reg.name));
                urls.Add(new Surls("ftp://fz223free:fz223free@ftp.zakupki.gov.ru/out/published/" + reg.eng + "/purchaseNoticeZK/daily/", reg.name));
            }
        }
        #endregion

        #region Парсилка Notice
        public void ParseXML(string xmlPath)
        {
            Nlist = new List<schemas.Notification>();
            foreach (string oneXML in Directory.GetFiles(xmlPath))
            {
                doc = new XmlDocument();
                doc.Load(oneXML);
                xnm = new XmlNamespaceManager(doc.NameTable);
                xnm.AddNamespace("ns2", "http://zakupki.gov.ru/223fz/purchase/1");
                xnm.AddNamespace("typ", "http://zakupki.gov.ru/223fz/types/1");

                schemas.Notification NC = new schemas.Notification();
                // проверим что это Notice
                if (doc.SelectSingleNode(startNodeN, xnm) != null)
                {
                    node = doc.SelectNodes(startNodeN, xnm)[0];
                    NC.notice.notificationType = "NC";
                }
                else if (doc.SelectSingleNode(startNodeAE, xnm) != null)
                {
                    node = doc.SelectNodes(startNodeAE, xnm)[0];
                    NC.notice.notificationType = "AE";
                }
                else if (doc.SelectSingleNode(startNodeEP, xnm) != null)
                {
                    node = doc.SelectNodes(startNodeEP, xnm)[0];
                    NC.notice.notificationType = "EP";
                }
                else if (doc.SelectSingleNode(startNodeOA, xnm) != null)
                {
                    node = doc.SelectNodes(startNodeOA, xnm)[0];
                    NC.notice.notificationType = "OA";
                }
                else if (doc.SelectSingleNode(startNodeOK, xnm) != null)
                {
                    node = doc.SelectNodes(startNodeOK, xnm)[0];
                    NC.notice.notificationType = "OK";
                }
                else if (doc.SelectSingleNode(startNodeZK, xnm) != null)
                {
                    node = doc.SelectNodes(startNodeZK, xnm)[0];
                    NC.notice.notificationType = "ZK";
                }
                else
                    continue;
               
                // разбираем purchaceNoticeData
                NC.notice.notificationNumber = GetNodeText("ns2:registrationNumber");
                NC.notice.orderName = GetNodeText("ns2:name");
                NC.notice.purchaseMethodCode = GetNodeText("ns2:purchaseMethodCode");
                // для Notice - purchaseCodeName, для остальных - purchaseMethodName
                NC.notice.purchaseMethodName = GetNodeText(NC.notice.notificationType == "NC" ? "ns2:purchaseCodeName" : "ns2:purchaseMethodName");
                NC.notice.submissionCloseDateTime = GetNodeText("ns2:submissionCloseDateTime");
                try
                {
                    NC.notice.submissionCloseDateTime = NC.notice.submissionCloseDateTime.Replace('T', ' ');
                    NC.notice.submissionCloseDateTime = NC.notice.submissionCloseDateTime.Replace("+04:00", "");
                }
                catch { }
                NC.notice.publishDate = GetNodeText("ns2:publicationDateTime");
                try
                {
                    NC.notice.publishDate = NC.notice.publishDate.Replace('T', ' ');
                    NC.notice.publishDate = NC.notice.publishDate.Replace("+04:00", "");
                }
                catch { }
                NC.notice.status = GetNodeText("ns2:status");
                NC.notice.modificationDescription = GetNodeText("ns2:modificationDescription");
                NC.notice.modificationDate = GetNodeText("ns2:modificationDate");
                try
                {
                    NC.notice.modificationDate = NC.notice.modificationDate.Replace('T', ' ');
                    NC.notice.modificationDate = NC.notice.modificationDate.Replace("+04:00", "");
                }
                catch { }
                NC.notice.ep_name = GetNodeText("ns2:electronicPlaceInfo/typ:name");
                NC.notice.ep_url = GetNodeText("ns2:electronicPlaceInfo/typ:url");
                NC.notice.orgName = GetNodeText("ns2:customer/typ:mainInfo/typ:shortName");
                NC.notice.orgOGRN = GetNodeText("ns2:customer/typ:mainInfo/typ:ogrn");
                NC.notice.orgINN = GetNodeText("ns2:customer/typ:mainInfo/typ:inn");
                // разбираем документы
                foreach (XmlNode oneDoc in node.SelectNodes("ns2:attachements/typ:document", xnm))
                {
                    NC.oneDocument.fileName = GetNodeText("typ:fileName", oneDoc);
                    NC.oneDocument.docDescription = GetNodeText("typ:description", oneDoc);
                    NC.oneDocument.url = GetNodeText("typ:url", oneDoc);
                    NC.documents.Add(NC.oneDocument);
                }

                // PurchaseNoticeData/lots
                int lotCount = 0;
                foreach (XmlNode oneLot in node.SelectNodes("ns2:lots/typ:lot/typ:lotData", xnm))
                {
                    NC.oneLot.subject = GetNodeText("typ:subject", oneLot);
                    NC.oneLot.currencyCode = GetNodeText("typ:currency/typ:code", oneLot);
                    NC.oneLot.currencyName = GetNodeText("typ:currency/typ:name", oneLot);
                    NC.oneLot.maxPrice = GetNodeText("typ:initialSum", oneLot);
                    NC.lots.Add(NC.oneLot);
                    foreach (XmlNode oneLotItem in oneLot.SelectNodes("typ:lotItems/typ:lotItem", xnm))
                    {
                        NC.oneProduct.code = GetNodeText("typ:okpd2/typ:code", oneLotItem);
                        NC.oneProduct.name = GetNodeText("typ:okpd2/typ:name", oneLotItem);
                        NC.oneProduct.lotID = lotCount;
                        NC.products.Add(NC.oneProduct);
                    }
                    lotCount++;
                }
                Nlist.Add(NC);
                File.Delete(oneXML);
            }
        }
        #endregion

        #region Вставка в базу Notice
        public void InsertIntoBase()
        {
            if (Nlist.Count > 0)
                Console.WriteLine("{0}. Notice {1}: {2}",region, Nlist[0].notice.notificationType, Nlist.Count.ToString());
            // по каждой записи 
            foreach (schemas.Notification NC in Nlist)
            {
                string nType = NC.notice.notificationType;
                // вставляем основу, получаем последний ID
                commStr = @"INSERT IGNORE tenderbase.notification (dateinsert,notificationNumber,notificationType,orderName,purchaseMethodCode,purchaseMethodName,submissionCloseDateTime,publishDate,status,modificationDescription,modificationDate,ep_name,ep_url,orgName,orgINN,orgOGRN,region,have_eng,type)
                                                          VALUES (now(),@notificationNumber,@notificationType,@orderName,@purchaseMethodCode,@purchaseMethodName,@submissionCloseDateTime,@publishDate,@status,@modificationDescription,@modificationDate,@ep_name,@ep_url,@orgName,@orgINN,@orgOGRN,@region,@have_eng,'223')";
                comm = new MySqlCommand(commStr, conn);
                comm.Parameters.AddWithValue("@notificationNumber", NC.notice.notificationNumber);
                comm.Parameters.AddWithValue("@notificationType", nType);
                comm.Parameters.AddWithValue("@orderName", NC.notice.orderName);
                comm.Parameters.AddWithValue("@purchaseMethodCode", NC.notice.purchaseMethodCode);
                comm.Parameters.AddWithValue("@purchaseMethodName", NC.notice.purchaseMethodName);
                comm.Parameters.AddWithValue("@submissionCloseDateTime", Common.IsDate(NC.notice.submissionCloseDateTime));
                comm.Parameters.AddWithValue("@publishDate", Common.IsDate(NC.notice.publishDate));
                comm.Parameters.AddWithValue("@status", NC.notice.status);
                comm.Parameters.AddWithValue("@modificationDescription", NC.notice.modificationDescription);
                comm.Parameters.AddWithValue("@modificationDate", Common.IsDate(NC.notice.modificationDate));
                comm.Parameters.AddWithValue("@ep_name", NC.notice.ep_name);
                comm.Parameters.AddWithValue("@ep_url", NC.notice.ep_url);
                comm.Parameters.AddWithValue("@orgName", NC.notice.orgName);
                comm.Parameters.AddWithValue("@orgINN", NC.notice.orgINN);
                comm.Parameters.AddWithValue("@orgOGRN", NC.notice.orgOGRN);
                comm.Parameters.AddWithValue("@region", region);
                comm.Parameters.AddWithValue("@have_eng", (NParse.NavalnyParse(NC.notice.orderName) ? "1" : "0"));
                if (ExecuteNonQuery(comm) == 0) // если команда выдает 0 - значит запись уже есть, обновляем publishDate, иначе, вставляем как новую
                {
                    commStr = @"UPDATE tenderbase.notification SET publishDate=@publishDate,submissionCloseDateTime=@submissionCloseDateTime WHERE notificationNumber=@notificationNumber";
                    comm = new MySqlCommand(commStr, conn);
                    comm.Parameters.AddWithValue("@publishDate", NC.notice.publishDate);
                    comm.Parameters.AddWithValue("@submissionCloseDateTime", NC.notice.submissionCloseDateTime);
                    comm.Parameters.AddWithValue("@notificationNumber", NC.notice.notificationNumber);
                    ExecuteNonQuery(comm);
                }
                else
                {
                    string Nid = GetLastIDfromNotice(ETables.notice);

                    // вставляем документы
                    StringBuilder attachementsStr = new StringBuilder();
                    for (int i = 0; i < NC.documents.Count; i++)
                        attachementsStr.Append("(" + Nid + "," + NotNull(NC.documents[i].fileName) + "," + NotNull(NC.documents[i].docDescription) + "," + NotNull(NC.documents[i].url) + "),");
                    if (!string.IsNullOrEmpty(attachementsStr.ToString()))
                    {
                        attachementsStr = attachementsStr.Remove(attachementsStr.Length - 1, 1);
                        commStr = "INSERT INTO tenderbase.documents (notificationID,fileName,docDescription,url) VALUES " + attachementsStr;
                        comm = new MySqlCommand(commStr, conn);
                        ExecuteNonQuery(comm);
                    }
                    // вставляем lots
                    for (int i = 0; i < NC.lots.Count; i++)
                    {
                        commStr = @"INSERT INTO tenderbase.lot (notificationID,subject,currencyCode,currencyName,maxPrice,type) 
                                                    VALUES (@notificationID,@subject,@currencyCode,@currencyName,@maxPrice,'223')";
                        comm = new MySqlCommand(commStr, conn);
                        comm.Parameters.AddWithValue("@notificationID", Nid);
                        comm.Parameters.AddWithValue("@subject", NC.lots[i].subject);
                        comm.Parameters.AddWithValue("@maxPrice", Common.IsNumeric(NC.lots[i].maxPrice));
                        comm.Parameters.AddWithValue("@currencyCode", NC.lots[i].currencyCode);
                        comm.Parameters.AddWithValue("@currencyName", NC.lots[i].currencyName);
                        ExecuteNonQuery(comm);

                        string lotID = GetLastIDfromNotice(ETables.lot);
                        // разбираем LotItems - products
                        StringBuilder lotItemStr = new StringBuilder();
                        for (int li = 0; li < NC.products.Count; li++)
                            if (NC.products[li].lotID == i)
                                lotItemStr.Append("(" + lotID + "," + Nid + "," + NotNull(NC.products[li].code) + "," + NotNull(NC.products[li].name) + "),");
                        if (!string.IsNullOrEmpty(lotItemStr.ToString()))
                        {
                            lotItemStr = lotItemStr.Remove(lotItemStr.Length - 1, 1);
                            commStr = "INSERT INTO tenderbase.products (lotID,notificationID,code,name) VALUES " + lotItemStr;
                            comm = new MySqlCommand(commStr, conn);
                            ExecuteNonQuery(comm);
                        }
                    }
                }
            }
        }
        #endregion




        public void DoWork()
        {
            try
            {
                if (!Directory.Exists(path + "done"))
                    Directory.CreateDirectory(path + "done");
                if (!Directory.Exists(path + "unknown"))
                    Directory.CreateDirectory(path + "unknown");
                ParseXML(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Logger.WriteLog223("Ошибка парсинга: " + ex.Message);
            }

            conn.Open();
            InsertIntoBase();
            conn.Close();

            foreach (string oneFile in Directory.GetFiles(path)) 
                File.Delete(oneFile);
        }

    }
}
