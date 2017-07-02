using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace zakupki_ftp
{
    internal class Notification94fz
    {

      //  public string startupPath;

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

        private readonly nParse NParse = new nParse();

        public List<Surls> urls;

        private XmlDocument doc;
        private XmlNamespaceManager xnm;
        private XmlNode node;
        private XmlNodeList nodes;

        private MySqlCommand comm;
        private readonly MySqlConnection conn;

        private List<schemas.Notification> Nlist;

        private const string startNodeEF = "ns2:export/ns2:fcsNotificationEF";
        private const string startNodeOK = "ns2:export/ns2:fcsNotificationOK";
        private const string startNodeZK = "ns2:export/ns2:fcsNotificationZK";
        private const string startNodePO = "ns2:export/ns2:fcsNotificationPO";
        private const string startNodeZP = "ns2:export/ns2:fcsNotificationZP";
        private const string startNodeEP = "ns2:export/ns2:fcsNotificationEP";
        private const string startNodeISM = "ns2:export/ns2:fcsNotificationISM";
        private const string startNodeISO = "ns2:export/ns2:fcsNotificationISO";
        private const string startNodeOKD = "ns2:export/ns2:fcsNotificationOKD";
        private const string startNodeOKOU = "ns2:export/ns2:fcsNotificationOKOU";
        private const string startNodeZakA = "ns2:export/ns2:fcsNotificationZakA";
        private const string startNodeZakKD = "ns2:export/ns2:fcsNotificationZakKD";
        private const string startNodeZakKOU = "ns2:export/ns2:fcsNotificationZakKOU";


        // переменная для региона
        public string region;

        private readonly string path;

        public Notification94fz(string Path)
        {
            conn = new MySqlConnection(ConfigurationManager.AppSettings["ConnString"]);
            region = null;
            path = Path;
            // чистим базу

            urls = new List<Surls>
            {
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Adygeja_Resp/notifications/currMonth/","Адыгея"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Altaj_Resp/notifications/currMonth/","Алтай"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Altajskij_kraj/notifications/currMonth/","Алтайский край"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Amurskaja_obl/notifications/currMonth/","Амурская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Arkhangelskaja_obl/notifications/currMonth/","Архангельская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Astrakhanskaja_obl/notifications/currMonth/","Астраханская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Bajkonur_g/notifications/currMonth/","Байконур"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Bashkortostan_Resp/notifications/currMonth/","Башкортостан"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Belgorodskaja_obl/notifications/currMonth/","Белгородская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Brjanskaja_obl/notifications/currMonth/","Брянская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Burjatija_Resp/notifications/currMonth/","Бурятия"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Chechenskaja_Resp/notifications/currMonth/","Чечня"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Cheljabinskaja_obl/notifications/currMonth/","Челябинская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Chukotskij_AO/notifications/currMonth/","Чукотский АО"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Chuvashskaja_Respublika_-_Chuvashija/notifications/currMonth/","Чувашия"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Dagestan_Resp/notifications/currMonth/","Дагестан"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Evrejskaja_Aobl/notifications/currMonth/","Еврейская АО"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Ingushetija_Resp/notifications/currMonth/","Ингушетия"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Irkutskaja_obl/notifications/currMonth/","Иркутская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Irkutskaja_obl_Ust-Ordynskij_Burjatskij_okrug/notifications/currMonth/","Иркутская обл, Бурятский окр"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Ivanovskaja_obl/notifications/currMonth/","Ивановская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Jamalo-Neneckij_AO/notifications/currMonth/","Ямало-Ненецкий АО"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Jaroslavskaja_obl/notifications/currMonth/","Ярославская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Kabardino-Balkarskaja_Resp/notifications/currMonth/","Кабардино-Балкарская респ"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Kaliningradskaja_obl/notifications/currMonth/","Калининградская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Kalmykija_Resp/notifications/currMonth/","Калмыкия"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Kaluzhskaja_obl/notifications/currMonth/","Калужская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Kamchatskij_kraj/notifications/currMonth/","Камчатский край"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Karachaevo-Cherkesskaja_Resp/notifications/currMonth/","Карачаево-Черкесская респ"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Karelija_Resp/notifications/currMonth/","Карелия"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Kemerovskaja_obl/notifications/currMonth/","Кемеровская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Khabarovskij_kraj/notifications/currMonth/","Хабаровск"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Khakasija_Resp/notifications/currMonth/","Хакасия"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Khanty-Mansijskij_Avtonomnyj_okrug_-_Jugra_AO/notifications/currMonth/","Ханты-Мансийский АО"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Kirovskaja_obl/notifications/currMonth/","Кировская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Komi_Resp/notifications/currMonth/", "Коми"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Kostromskaja_obl/notifications/currMonth/","Костромская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Krasnodarskij_kraj/notifications/currMonth/","Краснодарский край"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Krasnojarskij_kraj/notifications/currMonth/","Красноярский край"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Kurganskaja_obl/notifications/currMonth/","Курганская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Kurskaja_obl/notifications/currMonth/","Курская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Leningradskaja_obl/notifications/v/","Ленинградская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Lipeckaja_obl/notifications/currMonth/","Липецкая обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Magadanskaja_obl/notifications/currMonth/","Магаданская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Marij_El_Resp/notifications/currMonth/","Марий Эл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Mordovija_Resp/notifications/currMonth/","Мордовия"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Moskovskaja_obl/notifications/currMonth/","Московская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Moskva/notifications/currMonth/", "Москва"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Murmanskaja_obl/notifications/currMonth/","Мурманская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Neneckij_AO/notifications/currMonth/","Ненецкий АО"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Nizhegorodskaja_obl/notifications/currMonth/","Нижегородская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Novgorodskaja_obl/notifications/currMonth/","Новгородская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Novosibirskaja_obl/notifications/currMonth/","Новосибирская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Omskaja_obl/notifications/currMonth/","Омская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Orenburgskaja_obl/notifications/currMonth/","Оренбургская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Orlovskaja_obl/notifications/currMonth/","Орловская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Penzenskaja_obl/notifications/currMonth/","Пензенская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Permskij_kraj/notifications/currMonth/","Пермский край"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Primorskij_kraj/notifications/currMonth/","Приморский край"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Pskovskaja_obl/notifications/currMonth/","Псковская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Rjazanskaja_obl/notifications/currMonth/","Рязанская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Rostovskaja_obl/notifications/currMonth/","Ростовская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Sakha_Jakutija_Resp/notifications/currMonth/","Саха-Якутия"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Sakhalinskaja_obl/notifications/currMonth/","Сахалинская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Samarskaja_obl/notifications/currMonth/","Самарская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Sankt-Peterburg/notifications/currMonth/","Санкт-Петербург"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Saratovskaja_obl/notifications/currMonth/","Саратовская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Severnaja_Osetija_-_Alanija_Resp/notifications/currMonth/","Северная Осетия - Алания"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Smolenskaja_obl/notifications/currMonth/","Смоленская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Stavropolskij_kraj/notifications/currMonth/","Ставропольский край"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Sverdlovskaja_obl/notifications/currMonth/","Свердловская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Tambovskaja_obl/notifications/currMonth/","Тамбовская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Tatarstan_Resp/notifications/currMonth/","Татарстан"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Tjumenskaja_obl/notifications/currMonth/","Тюменская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Tomskaja_obl/notifications/currMonth/","Томская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Tulskaja_obl/notifications/currMonth/","Тульская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Tverskaja_obl/notifications/currMonth/","Тверская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Tyva_Resp/notifications/currMonth/", "Тува"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Udmurtskaja_Resp/notifications/currMonth/","Удмуртия"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Uljanovskaja_obl/notifications/currMonth/","Ульяновская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Vladimirskaja_obl/notifications/currMonth/","Владимирская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Volgogradskaja_obl/notifications/currMonth/","Волгоградская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Vologodskaja_obl/notifications/currMonth/","Вологодская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Voronezhskaja_obl/notifications/currMonth/","Воронежская обл"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Zabajkalskij_kraj/notifications/currMonth/","Забайкальский край"),
                new Surls("ftp://free:free@ftp.zakupki.gov.ru/fcs_regions/Zabajkalskij_kraj_Aginskij_Burjatskij_okrug/notifications/currMonth/","Забайкальский край, Агинский округ")
            };

        }

        #region Общие функции
        // таблицы для получения последнего ID
        private enum ETables { notification, lot };

        private string GetLastIDfromNotification(ETables tbl)
        {
            switch (tbl)
            {
                case ETables.notification: comm = new MySqlCommand("SELECT MAX(id) FROM tenderbase.notification WHERE type='44'", conn); break;
                case ETables.lot: comm = new MySqlCommand("SELECT MAX(id) FROM tenderbase.lot WHERE type='44'", conn); break;
                default: return null;
            }
            return Convert.ToString(comm.ExecuteScalar());
        }

        private static int ExecuteNonQuery(MySqlCommand comm)
        {
                int result = comm.ExecuteNonQuery();
                return result;
        }

        private static string NotNull(string str)
        {
            if (str != null)
            {
                str = str.Replace('\'', '"');
                str = str.Replace('\\', '/');
                return "'" + str + "'";
            }
            return "NULL";
        }

        private string GetNodeText(string nod, XmlNode xmlnode = null)
        {
            if (xmlnode == null) xmlnode = node;
            if (xmlnode.SelectSingleNode(nod, xnm) == null)
                return null;
            string result = xmlnode.SelectSingleNode(nod, xnm).InnerText;
            return result.Length > 2000 ? result.Remove(2000) : result;
        }
        #endregion

        public void ParseXML(string xmlPath)
        {
            Nlist = new List<schemas.Notification>();
            foreach (string oneXML in Directory.GetFiles(xmlPath))
            {
                // грузим документ
                doc = new XmlDocument();
                try
                {
                    doc.Load(oneXML);
                }
                catch 
                {
                    // кривой файл пропускаем
                    continue;
                }

                xnm = new XmlNamespaceManager(doc.NameTable);
                xnm.AddNamespace("ns3", "http://zakupki.gov.ru/oos/printform/1");
                xnm.AddNamespace("oos","http://zakupki.gov.ru/oos/types/1");
                xnm.AddNamespace("ns2", "http://zakupki.gov.ru/oos/export/1");
                // проверим что это EF документ 
                // проверим что это Notice
                string noticeType;
                if (doc.SelectSingleNode(startNodeEF, xnm) != null)
                {
                    node = doc.SelectNodes(startNodeEF, xnm)[0];
                    nodes = doc.SelectNodes(startNodeEF, xnm);
                    noticeType = "EF";
                }
                else if (doc.SelectSingleNode(startNodeOK, xnm) != null)
                {
                    node = doc.SelectNodes(startNodeOK, xnm)[0];
                    nodes = doc.SelectNodes(startNodeOK, xnm);
                    noticeType = "OK";
                }
                else if (doc.SelectSingleNode(startNodeZK, xnm) != null)
                {
                    node = doc.SelectNodes(startNodeZK, xnm)[0];
                    nodes = doc.SelectNodes(startNodeZK, xnm);
                    noticeType = "ZK";
                }
                else if (doc.SelectSingleNode(startNodePO, xnm) != null)
                {
                    node = doc.SelectNodes(startNodePO, xnm)[0];
                    nodes = doc.SelectNodes(startNodePO, xnm);
                    noticeType = "PO";
                }
                else if (doc.SelectSingleNode(startNodeEP, xnm) != null)
                {
                    node = doc.SelectNodes(startNodeEP, xnm)[0];
                    nodes = doc.SelectNodes(startNodeEP, xnm);
                    noticeType = "EP";
                }
                else if (doc.SelectSingleNode(startNodeISM, xnm) != null)
                {
                    node = doc.SelectNodes(startNodeISM, xnm)[0];
                    nodes = doc.SelectNodes(startNodeISM, xnm);
                    noticeType = "ISM";
                }
                else if (doc.SelectSingleNode(startNodeISO, xnm) != null)
                {
                    node = doc.SelectNodes(startNodeISO, xnm)[0];
                    nodes = doc.SelectNodes(startNodeISO, xnm);
                    noticeType = "ISO";
                }
                else if (doc.SelectSingleNode(startNodeOKD, xnm) != null)
                {
                    node = doc.SelectNodes(startNodeOKD, xnm)[0];
                    nodes = doc.SelectNodes(startNodeOKD, xnm);
                    noticeType = "OKD";
                }
                else if (doc.SelectSingleNode(startNodeOKOU, xnm) != null)
                {
                    node = doc.SelectNodes(startNodeOKOU, xnm)[0];
                    nodes = doc.SelectNodes(startNodeOKOU, xnm);
                    noticeType = "OKOU";
                }
                else if (doc.SelectSingleNode(startNodeZakA, xnm) != null)
                {
                    node = doc.SelectNodes(startNodeZakA, xnm)[0];
                    nodes = doc.SelectNodes(startNodeZakA, xnm);
                    noticeType = "ZakA";
                }
                else if (doc.SelectSingleNode(startNodeZakKD, xnm) != null)
                {
                    node = doc.SelectNodes(startNodeZakKD, xnm)[0];
                    nodes = doc.SelectNodes(startNodeZakKD, xnm);
                    noticeType = "ZakKD";
                }
                else if (doc.SelectSingleNode(startNodeZakKOU, xnm) != null)
                {
                    node = doc.SelectNodes(startNodeZakKOU, xnm)[0];
                    nodes = doc.SelectNodes(startNodeZakKOU, xnm);
                    noticeType = "ZakKOU";
                }
                else if (doc.SelectSingleNode(startNodeZP, xnm) != null)
                {
                    node = doc.SelectNodes(startNodeZP, xnm)[0];
                    nodes = doc.SelectNodes(startNodeZP, xnm);
                    noticeType = "ZP";
                }
                else
                    continue;

                for (int i = 0; i < nodes.Count; i++)
                {
                    node = nodes[i];
                    schemas.Notification NC = new schemas.Notification();
                    // разбираем notification
                    NC.notice.notificationNumber = GetNodeText("oos:purchaseNumber");
                    NC.notice.notificationType = noticeType;
                    NC.notice.orderName = GetNodeText("oos:purchaseObjectInfo");
                    NC.notice.publishDate = Convert.ToDateTime(GetNodeText("oos:docPublishDate")).ToString("yyyy-MM-dd hh-mm-ss");
                    NC.notice.href = GetNodeText("oos:href");
                    NC.notice.placingWay_code = GetNodeText("oos:placingWay/oos:code");
                    NC.notice.orgName = GetNodeText("oos:purchaseResponsible/oos:responsibleOrg/oos:fullName");
                    NC.notice.orgRegNum = GetNodeText("oos:purchaseResponsible/oos:responsibleOrg/oos:regNum");
                    NC.notice.orgINN = GetNodeText("oos:purchaseResponsible/oos:responsibleOrg/oos:INN");
                   // NC.notice.placementFeature_code = GetNodeText("oos:lots/oos:lot/oos:notificationFeatures/oos:notificationFeature/oos:placementFeature/oos:code");
                   // NC.notice.placementFeature_name = GetNodeText("oos:lots/oos:lot/oos:notificationFeatures/oos:notificationFeature/oos:placementFeature/oos:name");
                    NC.notice.ep_name = GetNodeText("ETP/name");
                    NC.notice.ep_url = GetNodeText("ETP/url");
                    // определяем дату окончания подачи заявки (для каждого вида может быть свой узел)
                    switch (noticeType)
                    {
                        case "EF": NC.notice.submissionCloseDateTime = Convert.ToDateTime(GetNodeText("oos:procedureInfo/oos:collecting/oos:endDate")).ToString("yyyy-MM-dd hh-mm-ss"); break;
                        case "EP": NC.notice.submissionCloseDateTime = Convert.ToDateTime(GetNodeText("oos:procedureInfo/oos:collecting/oos:endDate")).ToString("yyyy-MM-dd hh-mm-ss"); break;
                        case "ISM": NC.notice.submissionCloseDateTime = Convert.ToDateTime(GetNodeText("oos:procedureInfo/oos:collectingEndDate")).ToString("yyyy-MM-dd hh-mm-ss"); break;
                        case "ISO": NC.notice.submissionCloseDateTime = Convert.ToDateTime(GetNodeText("oos:procedureInfo/oos:collectingEndDate")).ToString("yyyy-MM-dd hh-mm-ss"); break;
                        case "OKD": NC.notice.submissionCloseDateTime = Convert.ToDateTime(GetNodeText("oos:procedureInfo/oos:stageOne/oos:collecting/oos:endDate")).ToString("yyyy-MM-dd hh-mm-ss"); break;
                        case "OKOU": NC.notice.submissionCloseDateTime = Convert.ToDateTime(GetNodeText("oos:procedureInfo/oos:collecting/oos:endDate")).ToString("yyyy-MM-dd hh-mm-ss"); break;
                        case "OK": NC.notice.submissionCloseDateTime = Convert.ToDateTime(GetNodeText("oos:procedureInfo/oos:collecting/oos:endDate")).ToString("yyyy-MM-dd hh-mm-ss"); break;
                        case "ZakA": NC.notice.submissionCloseDateTime = Convert.ToDateTime(GetNodeText("oos:procedureInfo/oos:collecting/oos:endDate")).ToString("yyyy-MM-dd hh-mm-ss"); break;
                        case "ZakKD": NC.notice.submissionCloseDateTime = Convert.ToDateTime(GetNodeText("oos:procedureInfo/oos:stageOne/oos:collecting/oos:endDate")).ToString("yyyy-MM-dd hh-mm-ss"); break;
                        case "ZakKOU": NC.notice.submissionCloseDateTime = Convert.ToDateTime(GetNodeText("oos:procedureInfo/oos:collecting/oos:endDate")).ToString("yyyy-MM-dd hh-mm-ss"); break;
                        case "ZakK": NC.notice.submissionCloseDateTime = Convert.ToDateTime(GetNodeText("oos:procedureInfo/oos:collecting/oos:endDate")).ToString("yyyy-MM-dd hh-mm-ss"); break;
                        case "ZK": NC.notice.submissionCloseDateTime = Convert.ToDateTime(GetNodeText("oos:procedureInfo/oos:collecting/oos:endDate")).ToString("yyyy-MM-dd hh-mm-ss"); break;
                        case "PO": NC.notice.submissionCloseDateTime = Convert.ToDateTime(GetNodeText("oos:procedureInfo/oos:collecting/oos:endDate")).ToString("yyyy-MM-dd hh-mm-ss"); break;
                        case "ZP": NC.notice.submissionCloseDateTime = Convert.ToDateTime(GetNodeText("oos:procedureInfo/oos:collecting/oos:endDate")).ToString("yyyy-MM-dd hh-mm-ss"); break;
                       // case "SZ": NC.notice.submissionCloseDateTime = Convert.ToDateTime(GetNodeText("oos:notificationCommission/oos:p1Date")).ToString("yyyy-MM-dd hh-mm-ss"); break;
                    }
                   // разбираем документы
                    foreach (XmlNode oneDoc in node.SelectNodes("oos:attachments/oos:attachment", xnm))
                    {
                        NC.documents.Add(new schemas.Notification.Documents(GetNodeText("oos:fileName", oneDoc), GetNodeText("oos:docDescription", oneDoc),
                            GetNodeText("oos:url", oneDoc)));
                    }
                    // разбираем лоты

                    /* ДОДЕЛАТЬ ЛОТЫ! */
                    if (noticeType == "ISO") // однолотовый
                    {
                        NC.oneLot.subject = GetNodeText("oos:lot/oos:OKPD2/oos:name") ?? GetNodeText("oos:lot/oos:purchaseObjects/oos:purchaseObject/oos:name");
                        NC.oneLot.currencyCode = GetNodeText("oos:lot/oos:currency/oos:code");
                        NC.oneLot.currencyName = GetNodeText("oos:lot/oos:currency/oos:name");
                        NC.oneLot.maxPrice = GetNodeText("oos:lot/oos:maxPrice");
                        NC.lots.Add(NC.oneLot);
                        NC.oneProduct.name = GetNodeText("oos:lot/oos:OKPD2/oos:name");
                        NC.oneProduct.code = GetNodeText("oos:lot/oos:OKPD2/oos:code");
                        if (NC.oneProduct.name == null)
                        {
                            NC.oneProduct.name = GetNodeText("oos:lot/oos:purchaseObjects/oos:purchaseObject/oos:OKPD2/oos:name");
                            NC.oneProduct.code = GetNodeText("oos:lot/oos:purchaseObjects/oos:purchaseObject/oos:OKPD2/oos:code");
                            NC.oneProduct.fullname = GetNodeText("oos:lot/oos:purchaseObjects/oos:purchaseObject/oos:name");
                            NC.oneProduct.price = GetNodeText("oos:lot/oos:purchaseObjects/oos:purchaseObject/oos:sum");
                        }
                        NC.products.Add(NC.oneProduct);
                    }
                    else
                    {
                        if (node.SelectSingleNode("oos:lots/oos:lot", xnm) == null)
                        {
                            XmlNode oneLot = node.SelectSingleNode("oos:lot", xnm);
                            if (noticeType == "OKD" || noticeType == "OKOU" || noticeType == "OK" || noticeType == "ZakKD" || noticeType == "ZakKOU" || noticeType == "ZakK")
                                NC.oneLot.subject = GetNodeText("oos:lotObjectInfo", oneLot);
                            else
                                NC.oneLot.subject = GetNodeText("oos:purchaseObjects/oos:purchaseObject/oos:name", oneLot);
                            NC.oneLot.currencyCode = GetNodeText("oos:currency/oos:code", oneLot);
                            NC.oneLot.currencyName = GetNodeText("oos:currency/oos:name", oneLot);
                            if (NC.oneLot.currencyName.Length > 30)
                                NC.oneLot.currencyName = NC.oneLot.currencyName.Substring(0, 30);
                            NC.oneLot.maxPrice = GetNodeText("oos:maxPrice", oneLot);
                            NC.lots.Add(NC.oneLot);
                            // смотрим на продукты в лоте
                            foreach (XmlNode oneProduct in oneLot.SelectNodes("oos:purchaseObjects/oos:purchaseObject", xnm))
                            {
                                NC.oneProduct.name = GetNodeText("oos:OKPD2/oos:name", oneProduct);
                                NC.oneProduct.code = GetNodeText("oos:OKPD2/oos:code", oneProduct);
                                NC.oneProduct.fullname = GetNodeText("oos:name", oneProduct);
                                if (NC.oneProduct.fullname != null)
                                    if (NC.oneProduct.fullname.Length > 1000)
                                        NC.oneProduct.fullname = NC.oneProduct.fullname.Substring(0, 1000);
                                NC.oneProduct.price = GetNodeText("oos:sum", oneProduct);
                                NC.products.Add(NC.oneProduct);
                            }
                        }
                        else
                        {
                            foreach (XmlNode oneLot in node.SelectNodes("oos:lots/oos:lot", xnm))
                            {
                                if (noticeType == "OKD" || noticeType == "OKOU" || noticeType == "OK" || noticeType == "ZakKD" || noticeType == "ZakKOU" || noticeType == "ZakK")
                                    NC.oneLot.subject = GetNodeText("oos:lotObjectInfo", oneLot);
                                else
                                    NC.oneLot.subject = GetNodeText("oos:purchaseObjects/oos:purchaseObject/oos:name", oneLot); 
                                NC.oneLot.currencyCode = GetNodeText("oos:currency/oos:code", oneLot);
                                NC.oneLot.currencyName = GetNodeText("oos:currency/oos:name", oneLot);
                                if (NC.oneLot.currencyName != null)
                                    if (NC.oneLot.currencyName.Length > 30)
                                        NC.oneLot.currencyName = NC.oneLot.currencyName.Substring(0, 30);
                                NC.oneLot.maxPrice = GetNodeText("oos:maxPrice", oneLot);
                                NC.lots.Add(NC.oneLot);
                                // смотрим на продукты в лоте
                                foreach (XmlNode oneProduct in oneLot.SelectNodes("oos:purchaseObjects/oos:purchaseObject", xnm))
                                {
                                    NC.oneProduct.name = GetNodeText("oos:OKPD2/oos:name", oneProduct);
                                    NC.oneProduct.code = GetNodeText("oos:OKPD2/oos:code", oneProduct);
                                    NC.oneProduct.fullname = GetNodeText("oos:name",oneProduct);
                                    if (NC.oneProduct.fullname != null)
                                        if (NC.oneProduct.fullname.Length > 1000)
                                            NC.oneProduct.fullname = NC.oneProduct.fullname.Substring(0, 1000);
                                    NC.oneProduct.price = GetNodeText("oos:sum", oneProduct);
                                    NC.products.Add(NC.oneProduct);
                                }
                            }
                        }
                    }
                    Nlist.Add(NC);
                    File.Delete(oneXML);
                }
            }
        }

        public void InsertIntoBase()
        {
            // по каждой разобранной записи
         //   int count =0;
         //   int count1 =0;
            foreach (schemas.Notification NC in Nlist)
            {
                string notifType = NC.notice.placingWay_code;
                // вставляем основу notificationEF и получаем последний ID
                string commStr = @"INSERT IGNORE tenderbase.notification (dateinsert,notificationNumber,notificationType,orderName,publishDate,href,placingWay_code,orgName,orgRegNum,placementFeature_code,placementFeature_name,ep_name,ep_url,region,have_eng,submissionCloseDateTime,type,orgINN)
                                                                VALUES (now(),@notificationNumber,@notificationType,@orderName,@publishDate,@href,@placingWay_code,@orgName,@orgRegNum,@placementFeature_code,@placementFeature_name,@ep_name,@ep_url,@region,@have_eng,@submissionCloseDateTime,'44',@orgINN)";
                comm = new MySqlCommand(commStr, conn);
                comm.CommandTimeout = 0;
                comm.Parameters.AddWithValue("@notificationNumber", NC.notice.notificationNumber);
                comm.Parameters.AddWithValue("@notificationType", notifType);
                comm.Parameters.AddWithValue("@orderName", NC.notice.orderName);
                comm.Parameters.AddWithValue("@publishDate", NC.notice.publishDate);
                comm.Parameters.AddWithValue("@href", NC.notice.href);
                comm.Parameters.AddWithValue("@placingWay_code", NC.notice.placingWay_code);
                comm.Parameters.AddWithValue("@orgName", NC.notice.orgName);
                comm.Parameters.AddWithValue("@orgRegNum", NC.notice.orgRegNum);
                comm.Parameters.AddWithValue("@placementFeature_code", NC.notice.placementFeature_code);
                comm.Parameters.AddWithValue("@placementFeature_name", NC.notice.placementFeature_name);
                comm.Parameters.AddWithValue("@ep_name", NC.notice.ep_name);
                comm.Parameters.AddWithValue("@ep_url", NC.notice.ep_url);
                comm.Parameters.AddWithValue("@region", region);
                comm.Parameters.AddWithValue("@have_eng", (NParse.NavalnyParse(NC.notice.orderName) ? "1" : "0"));
                comm.Parameters.AddWithValue("@submissionCloseDateTime", NC.notice.submissionCloseDateTime);
                comm.Parameters.AddWithValue("@orgINN", NC.notice.orgINN);
                if (ExecuteNonQuery(comm) == 0) // если команда выдает 0 - значит запись уже есть, обновляем publishDate, иначе, вставляем как новую
                {
                    commStr = @"UPDATE tenderbase.notification SET publishDate=@publishDate,submissionCloseDateTime=@submissionCloseDateTime,type='44' WHERE notificationNumber=@notificationNumber";
                    comm = new MySqlCommand(commStr, conn);
                    comm.CommandTimeout = 0;
                    comm.Parameters.AddWithValue("@publishDate", NC.notice.publishDate);
                    comm.Parameters.AddWithValue("@submissionCloseDateTime", NC.notice.submissionCloseDateTime);
                    comm.Parameters.AddWithValue("@notificationNumber", NC.notice.notificationNumber);
                    ExecuteNonQuery(comm);
                  //  Console.WriteLine("Dup {0}",count++);
                }
                else
                {
                    string ncID = GetLastIDfromNotification(ETables.notification);

                    // вставляем DocumentMetas
                    StringBuilder documentMetasStr = new StringBuilder();
                    for (int i = 0; i < NC.documents.Count; i++)
                        documentMetasStr.Append("(" + ncID + "," + NotNull(NC.documents[i].fileName) + "," + NotNull(NC.documents[i].docDescription) + "," + NotNull(NC.documents[i].url) + "),");
                    if (!string.IsNullOrEmpty(documentMetasStr.ToString()))
                    {
                        documentMetasStr = documentMetasStr.Remove(documentMetasStr.Length - 1, 1);
                        commStr = "INSERT INTO tenderbase.documents (notificationID,fileName,docDescription,url) VALUES " + documentMetasStr;
                        comm = new MySqlCommand(commStr, conn);
                        comm.CommandTimeout = 0;
                        ExecuteNonQuery(comm);
                    }
                    // вставляем lots
                    #region Lots
                    for (int i = 0; i < NC.lots.Count; i++)
                    {

                        // вставляем основу и узнаем последний id лота
                        commStr = @"INSERT INTO tenderbase.lot (notificationID,subject,currencyCode,currencyName,maxPrice,type) 
                                                 VALUES (@notificationID,@subject,@currencyCode,@currencyName,@maxPrice,'44')";
                        comm = new MySqlCommand(commStr, conn);
                        comm.CommandTimeout = 0;
                        comm.Parameters.AddWithValue("@notificationID", ncID);
                        comm.Parameters.AddWithValue("@subject", NC.lots[i].subject);
                        comm.Parameters.AddWithValue("@currencyCode", NC.lots[i].currencyCode);
                        comm.Parameters.AddWithValue("@currencyName", NC.lots[i].currencyName);
                        comm.Parameters.AddWithValue("@maxPrice", NC.lots[i].maxPrice);
                        ExecuteNonQuery(comm);
                        string lotID = GetLastIDfromNotification(ETables.lot);
                        // разбираем products
                        StringBuilder productStr = new StringBuilder();
                        for (int iprod = 0; iprod < NC.products.Count; iprod++)
                            productStr.Append("(" + ncID + "," + lotID + "," + NotNull(NC.products[iprod].code) + "," + NotNull(NC.products[iprod].name) + "," + NotNull(NC.products[iprod].price) + "," + NotNull(NC.products[iprod].fullname) + "),");
                        if (!string.IsNullOrEmpty(productStr.ToString()))
                        {
                            productStr = productStr.Remove(productStr.Length - 1, 1);
                            commStr = "INSERT INTO tenderbase.products (notificationID,lotID,code,name,price,fullname) VALUES " + productStr;
                            comm = new MySqlCommand(commStr, conn);
                            comm.CommandTimeout = 0;
                            ExecuteNonQuery(comm);
                        }
                    }
                  //  Console.WriteLine("Norm {0}", count1++);
                }
                #endregion
            }
        }
       
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
                Logger.WriteLog94("Ошибка парсинга: " + ex.Message);
            }
            conn.Open();
            InsertIntoBase();
            conn.Close();
            
            //  переносим остатки
            foreach (string oneFile in Directory.GetFiles(path))
                File.Delete(oneFile);
       }

    }
}