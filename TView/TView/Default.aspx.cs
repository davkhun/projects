using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.IO;
using System.Data;
using System.Text;

namespace TView
{
    public partial class Default : Page
    {
        private MySqlConnection conn;
        private MySqlConnection connDoc;
        private MySqlCommand comm;
        private MySqlDataReader reader;

        private SphinxSearch ss;

        #region Глобальные переменные и структуры
        // docInfo
        public List<DocInfo> _di
        {
            get
            {
                return (List<DocInfo>) Session["_DI"];
            }
            set
            {
                Session["_DI"] = value;
            }
        }
        // сохраненная площадка/фз при поиске

        // выбранная площадка/фз
        public int _fzIndex
        {
            get
            {
                return FZTypeDDL.SelectedIndex;
            }
            set
            {
                FZTypeDDL.SelectedIndex = value;
            }
        }

        // сортировка
        public string _sortStr
        {
            get 
            {
                return (string) ViewState["_SortInd"];
            }
            set
            {
                ViewState["_SortInd"] = value;
            }
        }
        
        // сохраненное условие поиска
        public string _commStr
        {
            get 
            {
                return (string) ViewState["_CommStr"];
            }
            set 
            {
                ViewState["_CommStr"] = value;
            }
        }

        //
        public int _currentPage
        {
            get
            {
                object o = ViewState["_CurrentPage"];
                if (o == null)
                    return 0;
                return (int)o;
            }
            set
            {
                ViewState["_CurrentPage"] = value;
            }
        }
        // всего страниц
        public int _pageCount
        {
            get
            {
                object o = ViewState["_PageCount"];
                if (o == null)
                    return 0;
                return (int)o;
            }
            set
            {
                ViewState["_PageCount"] = value;
            }
        }

        public struct SDemand // структура хранения созданного запроса
        {
            public string productCode { get; set; }
            public string productName { get; set; }
            public string publishDate { get; set; }
            public string maxPrice { get; set; }
            public string orderName { get; set; }
            public string region { get; set; }
        }
        #endregion

        #region Самописные ф-ии
        // Универсальный разбор ссылки для разных фз
        public static string GetHTMLlink(string type,string href,string name,string href223)
        {
            switch (type)
            {
                case "94": return "<a href='" + href + "' target=\"_blank\">" + name + "</a>";
                case "44": return "<a href='" + href + "' target=\"_blank\">" + name + "</a>";
                case "223": return "<a href=\"http://zakupki.gov.ru/223/purchase/public/notification/search.html?customerOrgId=&purchaseMethodId=&purchase=" + href223 + "&purchaseMethodName=&purchaseStages=APPLICATION_FILING&_purchaseStages=on&purchaseStages=COMMISSION_ACTIVITIES&_purchaseStages=on&_purchaseStages=on&purchaseStages=PLACEMENT_COMPLETE&_purchaseStages=on&publishDateFrom=&publishDateTo=&organName=&customerOrgId=&customerOrgName=&contractName=&organName=&okdpId=&okdpText=&okdpCode=&organName=&okvedId=&okvedText=&okvedCode=&startingContractPriceFrom=&startingContractPriceTo=&searchWord=&fullTextSearchType=INFOS_AND_DOCUMENTS&activeTab=0\" target=\"_blank\">" + name + "</a>";
                case "B2B": return "<a href='" + href + "' target=\"_blank\">" + name + "</a>";
                default: return null;
            }
        }
        // Универсальный разбор ссылки для Фокуса
        public static string GetFocusHTMLlink(string type, string href223,string hrefb2b, string name)
        {
            switch (type)
            {
                case "94": return "<a href=\"https://focus.kontur.ru/search?query=" + urlEncode(name) + "\" target=\"_blank\">" + name + "</a>";
                case "44": return "<a href=\"https://focus.kontur.ru/search?query=" + urlEncode(name) + "\" target=\"_blank\">" + name + "</a>";
                case "223": return "<a href=\"https://focus.kontur.ru/entity?query=" + href223 + "\" target=\"_blank\">" + name + "</a>";
                case "B2B": return "<a href=\"https://focus.kontur.ru/search?query=" + hrefb2b + "&region=&industry=&state=081077917\" target=\"_blank\">" + name + "</a>";
                default: return null;
            }
        }

        // определение типа аукциона
        public static string ReplaceAucType(string type)
        {
            switch (type)
            {
                case "EF": return "Открытый аукцион";
                case "OK": return "Открытый конкурс";
                case "ZK": return "Запрос котировок";
                case "PO": return "Предварительный отбор";
                case "ZKS": return "Запрос котировок (строительство)";
                case "SZ": return "Сообщение о заинтересованности";
                case "OKN": return "Открытый конкурс (науч. работа)";
                case "ZKM": return "Запрос котировок (медицина)";
                case "ZKI": return "Запрос котировок (иностранцы)";
                case "ESZ": return "Открытый аукцион (энергия)";
                case "OKL": return "Открытый конкурс (искусство)";
                case "ZKE": return "Запрос котировок (энергия)";
                case "OKF": return "Открытый конкурс (фильм)";
                case "OKE": return "Открытый конкурс (энергия)";
                default: return type;
            }
        }

        // создание таблицы
        protected void TableText(string cel1 = "", string cel2 = "")
        {
            TableRow trow = new TableRow();
            TableCell tcel = new TableCell();
            tcel.Width = Unit.Pixel(180);
            tcel.Text = cel1;
            trow.Cells.Add(tcel);
            tcel = new TableCell();
            tcel.Text = cel2;
            trow.Cells.Add(tcel);
            Table1.Rows.Add(trow);
        }
        // преобразование текста в HTML-формат
        public static string urlEncode(string src)
        {
            string[] p = HttpUtility.UrlEncode(src).Split('%');
            string res = p[0];

            for (int x = 1; x <= p.Length - 1; x++)
                res += "%" + p[x].Substring(0, 2).ToUpper() + (p[x].Length > 2 ? p[x].Substring(2) : "");

            return res.Replace("!", "%21");
        }
        #endregion

        #region Инициализация страницы
        protected void Page_Init(object sender, EventArgs e)
        {
            if (!Page.IsPostBack && !DesignMode)
            {
                conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQLString"].ConnectionString);
                comm = new MySqlCommand("SELECT DISTINCT region FROM notification", conn);
                conn.Open(); reader = comm.ExecuteReader();
                while (reader.Read())
                    if (!reader.IsDBNull(0)) CheckBoxList1.Items.Add(reader.GetString(0));
                reader.Close(); conn.Close();
                ItemsOnPageDDL.SelectedIndex = 0;
                // загружаем доступные профили

                DropDownList2.Items.Add("Сохраненое условие...");
                comm = new MySqlCommand("SELECT profileName FROM _profiles WHERE uname=@uname AND active='1'", conn);
                comm.Parameters.AddWithValue("@uname", HttpContext.Current.User.Identity.Name);
                conn.Open(); reader = comm.ExecuteReader();
                while (reader.Read())
                    if (!reader.IsDBNull(0)) DropDownList2.Items.Add(reader.GetString(0));
                reader.Close(); conn.Close();
                // дату публикации ставим на вчера
                TextBox4.Text = DateTime.Now.AddDays(-1).ToShortDateString();
            }
        }
        #endregion

        #region Загрузка страницы
        protected void Page_Load(object sender, EventArgs e)
        {
            conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQLString"].ConnectionString);
           
            MaintainScrollPositionOnPostBack = true;
            foreach (ListItem li in CheckBoxList1.Items)
                li.Attributes.Add("class", "checkboxList_item");
            foreach (ListItem li in OKDPcbl.Items)
                li.Attributes.Add("class", "checkboxList_item");

            foreach (RepeaterItem ritem in ZGrid.Items)
            {
                Button btn = ritem.FindControl("InfoBtn") as Button;
                btn.Click += (InfoBtn_Click);
            }
            // при добавлении айтема считает как 0, нужен костыль
            OKDPinnerText();

        }
        #endregion
        
        #region Отображение контекста
        public void VisibleContext(bool dontShowErr = false)
        {
            bool visible = ZGrid.Items.Count > 0;
            ViewOnPageLbl.Visible = visible;
            ItemsOnPageDDL.Visible = visible;
            ZGrid.Visible = visible;
            Label2.Text = "Ничего не найдено!";
            Label2.Visible = !visible;
            ExportXLSButton.Visible = visible;
            ExpToExcel.Visible = visible;
            CreateMailer.Visible = visible;
            SaveVariantLnkB.Visible = visible;
            if (dontShowErr)
                Label2.Visible = false;
        }
        #endregion


        #region Страницы
        #region Постраничное перелистывание
        private void GetPageItems(int pageSize)
        {
            ZGrid.DataSource = null;
            PagedDataSource pds = new PagedDataSource();
            if (_commStr != null)
            {
                mSQLData.SelectCommand = _commStr;
                
                DataView dv = (DataView)mSQLData.Select(DataSourceSelectArguments.Empty);

                dv.Sort = _sortStr;
                pds.DataSource = dv;

                pds.AllowPaging = true;
                pds.PageSize = pageSize;
                _pageCount = pds.PageCount;
                pds.CurrentPageIndex = _currentPage;
                SetVisiblePages(pds.PageCount);
                ZGrid.DataSource = pds;
                ZGrid.DataBind();
            }
        }
        // отображение кол-ва страниц
        private void SetVisiblePages(int pCount)
        {
            switch (pCount)
            {
                case 0: VisiblePages(); break;
                case 1: VisiblePages(false, true); break;
                case 2: VisiblePages(false, true, true); break;
                case 3: VisiblePages(false, true, true, true); break;
                case 4: VisiblePages(false, true, true, true, true); break;
                case 5: VisiblePages(false, true, true, true, true, true); break;
            }
            if (pCount > 5)
            {
                if (_currentPage == _pageCount - 1)
                    VisiblePages(true, true, true, true, true, true);
                else if (_currentPage != 0)
                    VisiblePages(true, true, true, true, true, true, true);
                else
                    VisiblePages(false, true, true, true, true, true, true);
            }
        }

        private void PageClick(DropDownList ddl, LinkButton lb)
        {
            _currentPage = Convert.ToInt16(lb.Text) - 1;
            GetPageItems(Convert.ToInt16(ddl.SelectedItem.ToString()));
        }

        // крутой вариант перелистывания

        #region Универсальные методы перелистывания страниц
        // перелистывание 2-4 стр
        void Page2_4(DropDownList ddl, LinkButton lbFirst, LinkButton lb)
        {
            lbFirst.Visible = true;
            PageClick(ddl, lb);
            SetPageColor();
        }
        // перелистывание первая, стр 1 и последняя
        void PageFirst1_5Last(DropDownList ddl, LinkButton lbFirst, LinkButton lb1, LinkButton lb2, LinkButton lb3, LinkButton lb4, LinkButton lb5, LinkButton lbLast, string pageVariant)
        {
            if (pageVariant == "first")
            {
                lbFirst.Visible = false;
                lbLast.Visible = true;
                lb1.Text = "1";
                lb2.Text = "2";
                lb3.Text = "3";
                lb4.Text = "4";
                lb5.Text = "5";
                PageClick(ddl, lb1);
                SetPageColor();
            }
            else if (pageVariant == "1")
            {
                PageClick(ddl, lb1);
                if (lb1.Text != "1") // ворочаем назад
                {
                    lbLast.Visible = true;
                    if (Convert.ToInt16(lb1.Text) <= 5)
                    {
                        lb1.Text = "1";
                        lb2.Text = "2";
                        lb3.Text = "3";
                        lb4.Text = "4";
                        lb5.Text = "5";
                    }
                    else
                    {
                        lb1.Text = (Convert.ToInt16(lb1.Text) - 4).ToString();
                        lb2.Text = (Convert.ToInt16(lb2.Text) - 4).ToString();
                        lb3.Text = (Convert.ToInt16(lb3.Text) - 4).ToString();
                        lb4.Text = (Convert.ToInt16(lb4.Text) - 4).ToString();
                        lb5.Text = (Convert.ToInt16(lb5.Text) - 4).ToString();
                    }
                }
                SetPageColor();
            }
            else if (pageVariant == "5")
            {
                lbFirst.Visible = true;
                PageClick(ddl, lb5);
                if (_pageCount <= 10) // ворочаем вперед
                {
                    lb5.Text = _pageCount.ToString();
                    lb4.Text = (_pageCount - 1).ToString();
                    lb3.Text = (_pageCount - 2).ToString();
                    lb2.Text = (_pageCount - 3).ToString();
                    lb1.Text = (_pageCount - 4).ToString();
                }
                else
                {
                    if ((Convert.ToInt16(lb5.Text) + 4) >= _pageCount)
                    {
                        lb5.Text = _pageCount.ToString();
                        lb4.Text = (_pageCount - 1).ToString();
                        lb3.Text = (_pageCount - 2).ToString();
                        lb2.Text = (_pageCount - 3).ToString();
                        lb1.Text = (_pageCount - 4).ToString();
                    }
                    else
                    {
                        lb5.Text = (Convert.ToInt16(lb5.Text) + 4).ToString();
                        lb4.Text = (Convert.ToInt16(lb4.Text) + 4).ToString();
                        lb3.Text = (Convert.ToInt16(lb3.Text) + 4).ToString();
                        lb2.Text = (Convert.ToInt16(lb2.Text) + 4).ToString();
                        lb1.Text = (Convert.ToInt16(lb1.Text) + 4).ToString();
                    }
                }
                SetPageColor();
            }
            else if (pageVariant == "last")
            {
                lbFirst.Visible = true;
                lbLast.Visible = false;
                lb1.Text = (_pageCount - 4).ToString();
                lb2.Text = (_pageCount - 3).ToString();
                lb3.Text = (_pageCount - 2).ToString();
                lb4.Text = (_pageCount - 1).ToString();
                lb5.Text = _pageCount.ToString();
                PageClick(ddl, lb5);
                SetPageColor();
            }
        }
        #endregion


        #region Кнопки переключения страниц
        protected void pageFirst_Click(object sender, EventArgs e)
        {
            PageFirst1_5Last(ItemsOnPageDDL, pageFirst, page1, page2, page3, page4, page5, pageLast, "first");
        }

        protected void page1_Click(object sender, EventArgs e)
        {
            PageFirst1_5Last(ItemsOnPageDDL, pageFirst, page1, page2, page3, page4, page5, pageLast, "1");
        }

        protected void page2_Click(object sender, EventArgs e)
        {
            Page2_4(ItemsOnPageDDL, page1, page2);
        }

        protected void page3_Click(object sender, EventArgs e)
        {
            Page2_4(ItemsOnPageDDL, page1, page3);
        }

        protected void page4_Click(object sender, EventArgs e)
        {
            Page2_4(ItemsOnPageDDL, page1, page4);
        }

        protected void page5_Click(object sender, EventArgs e)
        {
            PageFirst1_5Last(ItemsOnPageDDL, pageFirst, page1, page2, page3, page4, page5, pageLast, "5");
        }

        protected void pageLast_Click(object sender, EventArgs e)
        {
            PageFirst1_5Last(ItemsOnPageDDL, pageFirst, page1, page2, page3, page4, page5, pageLast, "last");
        }
        #endregion

        // -------
        #endregion

        #region Отображение и покраска страниц
        public void VisiblePages(bool first = false, bool p1 = false, bool p2 = false, bool p3 = false, bool p4 = false, bool p5 = false, bool last = false)
        {
            pageFirst.Visible = first;
            page1.Visible = p1;
            page2.Visible = p2;
            page3.Visible = p3;
            page4.Visible = p4;
            page5.Visible = p5;
            pageLast.Visible = last;
        }

        public void SetPageColor()
        {
            if (page1.Text == (_currentPage + 1).ToString())
                page1.Attributes.Add("style", "background-color:yellow");
            else
                page1.Attributes.Remove("style");
            if (page2.Text == (_currentPage + 1).ToString())
                page2.Attributes.Add("style", "background-color:yellow");
            else
                page2.Attributes.Remove("style");
            if (page3.Text == (_currentPage + 1).ToString())
                page3.Attributes.Add("style", "background-color:yellow");
            else
                page3.Attributes.Remove("style");
            if (page4.Text == (_currentPage + 1).ToString())
                page4.Attributes.Add("style", "background-color:yellow");
            else
                page4.Attributes.Remove("style");
            if (page5.Text == (_currentPage + 1).ToString())
                page5.Attributes.Add("style", "background-color:yellow");
            else
                page5.Attributes.Remove("style");

        }
        #endregion

        #region Количество отображаемых элементов таблицы на странице
        void SetViewItems(DropDownList ddl)
        {
            _currentPage = 0;
            switch (ddl.SelectedIndex)
            {
                case 0: GetPageItems(8); break;
                case 1: GetPageItems(10); break;
                case 2: GetPageItems(15); break;
                case 3: GetPageItems(20); break;
            }
        }

        protected void ItemsOnPageDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetViewItems(ItemsOnPageDDL);
        }

        #endregion 
        #endregion


        #region ОКДП
        #region Парсилка ОКДП
        public string OKDPparse(ListItemCollection OKDPLst)
        {
            List<string> pureCode = new List<string>();
            List<string> subCode = new List<string>();
            string result = "";
            foreach (ListItem li in OKDPLst)
            {
                if (li.Selected)
                {
                    if (li.Text.IndexOf('*') > -1) subCode.Add(li.Text);
                    else pureCode.Add(li.Text);
                }
            }
            // если есть части кодов, оборачиваем всё в ()
            if (subCode.Count > 0)
            {
                result = "(";
                // собираем IN для "чистых" кодов
                if (pureCode.Count > 0)
                    result += "prod.code IN('" + string.Join("','", pureCode.ToArray()) + "') OR ";
                // соединяем с частями через OR
                foreach (string oneSub in subCode)
                {
                    // ищем только от 2-х символов 
                    if (oneSub.Length > 2)
                        result += "prod.code LIKE '" + oneSub.Remove(oneSub.Length - 1, 1) + "%' OR ";
                }
                result = result.Remove(result.Length - 3, 3);
                result += ") AND ";
            }
            else
            {
                // собираем IN для "чистых" кодов
                if (pureCode.Count > 0)
                    result += "prod.code IN('" + string.Join("','", pureCode.ToArray()) + "') AND ";
            }
            return result;
        }
        #endregion

        #region Текст в поле ОКДП
        private void OKDPinnerText()
        {
            try
            {
                if (OKDPcbl.Items[0] != null)
                    OKDPspan.InnerText = "Список ОКДП";
            }
            catch
            {
                OKDPspan.InnerText = "Список пуст";
            }
        }
        #endregion

        #region Добавление кода ОКДП
        protected void AddOKDPButton_Click(object sender, EventArgs e)
        {
            if (TextBox1.Text != "")
            {
                OKDPcbl.Items.Add(new ListItem(TextBox1.Text));
                OKDPcbl.Items[OKDPcbl.Items.Count - 1].Attributes.Add("class", "checkboxList_item");
                OKDPcbl.Items[OKDPcbl.Items.Count - 1].Selected = true;
                TextBox1.Text = "";
                OKDPinnerText();
            }
        }
        #endregion

        #region Загрузка файла с кодами ОКДП
        protected void Button4_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                fUploadLnk.InnerText = "Загружаем файл...";
                using (StreamReader sr = new StreamReader(FileUpload1.FileContent))
                {
                    while (!sr.EndOfStream)
                    {
                        OKDPcbl.Items.Add(new ListItem(sr.ReadLine()));
                        OKDPcbl.Items[OKDPcbl.Items.Count - 1].Attributes.Add("class", "checkboxList_item");
                        OKDPcbl.Items[OKDPcbl.Items.Count - 1].Selected = true;
                        OKDPinnerText();
                    }
                }
                FileUpload1.FileContent.Close();
                fUploadLnk.InnerText = "Загрузить список ОКДП...";
            }
            FileUpload1.Dispose();
        }
        #endregion 

        #region Автозаполнение ОКДП
        [System.Web.Script.Services.ScriptMethod]
        [System.Web.Services.WebMethod]
        public static List<string> SearchOKDP(string prefixText, int count)
        {
            MySqlConnection okdpConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQLString"].ConnectionString);
            List<string> result = new List<string> ();
            string commStr;
            if (prefixText[0] >= '0' && prefixText[0] <= '9')
                commStr = "SELECT code,name FROM _okdp WHERE code LIKE '" + prefixText + "%' LIMIT 10";
            else
                commStr = "SELECT code,name FROM _okdp WHERE MATCH (name) AGAINST('" + prefixText + "*' IN BOOLEAN MODE) LIMIT 10";
            MySqlCommand okdpComm = new MySqlCommand(commStr, okdpConn);
            okdpConn.Open();
            MySqlDataReader okdpReader = okdpComm.ExecuteReader();
            while (okdpReader.Read())
                result.Add(okdpReader.GetValue(0) + " - " + okdpReader.GetValue(1));
            okdpReader.Close();
            okdpConn.Close();
            return result;
        }
        #endregion

        #region Очистка кодов ОКДП
        protected void ClearOKDPbtn_Click(object sender, EventArgs e)
        {
            OKDPcbl.Items.Clear();
            OKDPinnerText();
        }
        #endregion
        #endregion


        #region Регионы
        #region Очистка списка регионов
        protected void Button9_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < CheckBoxList1.Items.Count; i++)
                CheckBoxList1.Items[i].Selected = false;
            RegionLbl.Text = "Все";
            RegionLbl.ToolTip = "";
        }
        #endregion

        #region Заполнение списка регионов
        protected void Button8_Click(object sender, EventArgs e)
        {
            string regions = "";
            foreach (ListItem li in CheckBoxList1.Items)
                if (li.Selected) regions += li.Text + ",";
            regions = regions.Remove(regions.Length - 1, 1);
            RegionLbl.Text = Common.TruncateStr(regions);
            RegionLbl.ToolTip = regions;
        }
        #endregion
        #endregion


        #region Поиск в таблице
        private void SearchResult(int variant)
        {
            string commStr = @"SELECT 
                                n.id as 'ID'
                                ,n.notificationNumber as 'NotificationNumber'
                                ,n.notificationType as 'PWName'
                                ,n.orderName as 'OrderName'
                                ,l.maxPrice as 'MaxPrice'
                                ,n.orgName as 'PlacerFullName'
                                ,n.publishDate as 'PublishDate'
                                ,n.href as 'HREF'
                                ,n.region as 'REGION'
                                ,n.type as 'FZtype'
                                ,n.orgINN as 'HREFB2B'
                                ,n.orgOGRN as 'HREF223'
                                FROM notification AS n 
                                LEFT JOIN products AS prod ON prod.notificationID=n.id
                                LEFT JOIN lot as l ON prod.lotID=l.id
                                WHERE ";
            
            string var = "";
            // если код окдп введен но не добавлен, добавляем его
            if (TextBox1.Text != "")
            {
                OKDPcbl.Items.Add(new ListItem(TextBox1.Text));
                OKDPcbl.Items[OKDPcbl.Items.Count - 1].Attributes.Add("class", "checkboxList_item");
                OKDPcbl.Items[OKDPcbl.Items.Count - 1].Selected = true;
                TextBox1.Text = "";
                OKDPinnerText();
            }
            // смотрим на регион
            foreach (ListItem itm in CheckBoxList1.Items)
                if (itm.Selected)
                    var += "'" + itm.Text + "',";
            if (var != "")
                commStr += "n.region IN (" + var.Remove(var.Length - 1, 1) + ") AND ";

            if (TextBox3.Text != "")
            {
                try
                {
                    // инициализируем поисковый движок Sphinx
                    // в зависимости от сервера
                    List<long> sphinxID;
                    if (InDocCB.Checked)
                    {
                        // получаем списки id's по извещениям
                        List<DocInfo> di = new List<DocInfo>();
                        string docIDs=null;
                        ss = new SphinxSearch(ConfigurationManager.AppSettings["SphinxDocHost"], Convert.ToInt16(ConfigurationManager.AppSettings["SphinxDocPort"]));
                        sphinxID = ss.GetResult(TextBox3.Text, "tenderDoc");
                        connDoc = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQLDocString"].ConnectionString);
                        comm = new MySqlCommand("SELECT notificationID,filename FROM doccontents WHERE id IN (" + ss.GetIDstring(sphinxID) + ")", connDoc);
                        connDoc.Open();
                        reader = comm.ExecuteReader();
                        while (reader.Read())
                        {
                            docIDs += reader.GetString(0) + ",";
                            di.Add(new DocInfo(reader.GetString(1), reader.GetString(0)));
                        }
                        connDoc.Close();
                        connDoc.Dispose();
                        if (docIDs != null)
                        {
                            docIDs = docIDs.Remove(docIDs.Length - 1, 1);
                            commStr += "n.id IN (" + docIDs + ") AND ";
                            _di = di;
                        }
                    }
                    else
                    {
                        ss = new SphinxSearch(ConfigurationManager.AppSettings["SphinxHost"], Convert.ToInt16(ConfigurationManager.AppSettings["SphinxPort"]));
                        sphinxID = ss.GetResult(TextBox3.Text, "tenderOrder");
                        commStr += "n.id IN (" + ss.GetIDstring(sphinxID) + ") AND ";
                    }
                }
                catch // сфинкс затупил, воспользуемся старыми методами
                {
                    commStr += "n.orderName LIKE '%" + TextBox3.Text + "%' AND ";
                }
            }
            // смотрим на максимальную цену
            if (MaxPriceTB.Text != "")
                commStr += "l.maxPrice<=" + MaxPriceTB.Text + " AND l.maxPrice IS NOT NULL AND ";
            // смотрим на дату заказа
            if (TextBox4.Text != "")
            {
                if (CheckBox1.Checked) // с даты до сегодня
                    commStr += "n.publishDate>='" + Convert.ToDateTime(TextBox4.Text).ToString("yyyy-MM-dd") + "' AND ";
                else
                    commStr += "n.publishDate='" + Convert.ToDateTime(TextBox4.Text).ToString("yyyy-MM-dd") + "' AND ";
            }
            // смотрим на ФЗ
            if (variant == 1) // 94фз
                commStr += "n.type='94' AND ";
            else if (variant == 2) //223фз
                commStr += "n.type='223' AND ";
            else if (variant == 3) // B2B
                commStr += "n.type='B2B' AND ";
            else if (variant == 4) // B2B
                commStr += "n.type='44' AND ";
            // смотрим на ОКДП
            string OKDPcodes = OKDPparse(OKDPcbl.Items);
            if (OKDPcodes.Length > 0) OKDPcodes = OKDPcodes.Remove(OKDPcodes.Length - 4, 4);
            commStr += OKDPcodes;
            // если окдп кодов нет, режем последний AND
            if (commStr.Substring(commStr.Length - 4, 4).IndexOf("AND") > -1)
                commStr = commStr.Remove(commStr.Length - 4, 4);
            if (commStr.Substring(commStr.Length - 2, 2) != "E ") // если ничего не ввели
            {
                commStr += "GROUP BY n.id ";
                // если поиск только с кодами ОКДП в списке
                bool haveCheckedOKDP = false;
                foreach (ListItem li in OKDPcbl.Items)
                {
                    if (li.Selected)
                    {
                        haveCheckedOKDP = true;
                        break;
                    }
                }
                if (CheckBox2.Checked && haveCheckedOKDP)
                    commStr += " HAVING (SELECT COUNT(*) FROM products WHERE products.notificationID=n.id)<=(SELECT COUNT(*) FROM products WHERE " + OKDPcodes.Replace("prod.code", "products.code") + " AND products.notificationID=n.id)";
                
              //  mSQLData.SelectCommand = commStr;
                _commStr = commStr;
                GetPageItems(Convert.ToInt16(ItemsOnPageDDL.SelectedItem.ToString()));
                // пишем выполненный запрос
                comm = new MySqlCommand("INSERT INTO _demands (uname,whereStr,demand_date) VALUES ('" + HttpContext.Current.User.Identity.Name + "','" + commStr.Substring(commStr.IndexOf("WHERE")).Replace('\'', '"') + "',now())", conn);
                conn.Open(); comm.ExecuteNonQuery(); conn.Close();
            }
            else
            {
                Label2.Text = "Не выбрано ни одного параметра!";
                Label2.Visible = true;
            }
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            Label2.Visible = false;
            SearchResult(_fzIndex);
            _currentPage = 0;
            VisibleContext(); // смотрим нашли иль нет, отображаем кнопки управления
            // добавляем триггеры
            SetPageColor();
            // восстанавливаем страницы
            PageFirst1_5Last(ItemsOnPageDDL, pageFirst, page1, page2, page3, page4, page5, pageLast, "first");
        }
        #endregion

        #region Информация о тендере
        void InfoBtn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            RepeaterItem ritem = (RepeaterItem)btn.NamingContainer;
            Label lbl = (Label)ritem.FindControl("IDlbl");
            string id = lbl.Text;
            StringBuilder res = new StringBuilder();
            conn.Open();
            // дата окончания торгов
            comm = new MySqlCommand("SELECT submissionCloseDateTime FROM notification WHERE id=" + id, conn);
            res.Append(Convert.ToString(comm.ExecuteScalar()));
            if (!string.IsNullOrEmpty(res.ToString()))
                TableText("Дата окончания", res.ToString());
            // особенности размещения
            res = new StringBuilder();
            comm = new MySqlCommand("SELECT placementFeature_name FROM notification WHERE id=" + id, conn);
            res.Append(Convert.ToString(comm.ExecuteScalar()));
            if (!string.IsNullOrEmpty(res.ToString()))
                TableText("Особенность размещения", res.ToString());
            // Товар, работы, услуги
            res = new StringBuilder();
            comm = new MySqlCommand("SELECT prod.code,prod.name,prod.price FROM products AS prod JOIN notification AS n ON prod.notificationID=n.id WHERE n.id=" + id, conn);
            reader = comm.ExecuteReader();
            while (reader.Read())
            {
                if (!reader.IsDBNull(0))
                    res.Append(reader.GetString(0) + (reader.IsDBNull(1) ? "" : " - " + reader.GetString(1)) + "<br/>" + (reader.IsDBNull(2) ? "Цена не указана" : "Цена: " + reader.GetString(2)) + "<br/><br/>");
            }
            reader.Close();
            TableText("ОКДП", res.ToString());

            if (InDocCB.Checked)
            {
                string docStr = null;
                string fname = null;
                foreach (DocInfo oneDi in _di)
                {
                    if (oneDi._id == id)
                    {
                        fname = oneDi._fileName;
                        break;
                    }
                }
                comm = new MySqlCommand("SELECT url FROM documents WHERE  notificationID=" + id + " AND filename='" + fname + "'", conn);
                reader = comm.ExecuteReader();
                while (reader.Read())
                    docStr += "<a href=\"" + reader.GetString(0) + "\">" + fname + "</a><br/>";
                reader.Close();
                TableText("Найдено в документах:", docStr);
            }
            conn.Close();
            ModalPopupExtender2.Show();
        }
        #endregion
        
        #region Работа с профилями
        // сохранение
        protected void Button5_Click(object sender, EventArgs e)
        {
            if (TextBox5.Text != "")
            {
                // формируем запрос
                SDemand demand = new SDemand();
                // смотрим на регион
                foreach (ListItem itm in CheckBoxList1.Items)
                    if (itm.Selected) demand.region += itm.Text + ",";
                if (demand.region?.Length > 3) demand.region = demand.region.Remove(demand.region.Length - 1, 1);
                // смотрим на ОКДП
                foreach (ListItem itm in OKDPcbl.Items)
                {
                    if (itm.Selected)
                        demand.productCode += itm.Text + ",";
                }
                if (demand.productCode?.Length > 3) demand.productCode = demand.productCode.Remove(demand.productCode.Length - 1, 1);
                // смотрим на тендер, заказ
                if (TextBox3.Text != "")
                    demand.orderName = TextBox3.Text;
                // смотрим на дату заказа
                if (TextBox4.Text != "")
                {
                    if (CheckBox1.Checked)
                        demand.publishDate = ">" + Convert.ToDateTime(TextBox4.Text).ToString("yyyy-MM-dd");
                    else
                        demand.publishDate = Convert.ToDateTime(TextBox4.Text).ToString("yyyy-MM-dd");
                }
                else
                    demand.publishDate = "";
                if (MaxPriceTB.Text != "")
                    demand.maxPrice = MaxPriceTB.Text;
                // проверим, если такое имя профиля есть, заменим его
                comm = new MySqlCommand("SELECT COUNT(*) FROM _profiles WHERE profileName=@profileName AND uname=@uname", conn);
                comm.Parameters.AddWithValue("@profileName",TextBox5.Text);
                comm.Parameters.AddWithValue("@uname", HttpContext.Current.User.Identity.Name);
                conn.Open(); int profileCount = Convert.ToInt16(comm.ExecuteScalar()); conn.Close();
                if (profileCount == 0)
                {
                    comm = new MySqlCommand("INSERT INTO _profiles (profileName,uname,productCode,productName,publishDate,maxPrice,orderName,region) VALUES (@profileName,@uname,@productCode,@productName,@publishDate,@maxPrice,@orderName,@region)", conn);
                    DropDownList2.Items.Add(TextBox5.Text);
                }
                else
                    comm = new MySqlCommand("UPDATE _profiles SET productCode=@productCode,productName=@productName,publishDate=@publishDate,maxPrice=@maxPrice,orderName=@orderName,region=@region WHERE profileName=@profileName AND uname=@uname", conn);
                comm.Parameters.AddWithValue("@profileName", TextBox5.Text);
                comm.Parameters.AddWithValue("@productCode", demand.productCode);
                comm.Parameters.AddWithValue("@productName", demand.productName);
                comm.Parameters.AddWithValue("@publishDate", demand.publishDate);
                comm.Parameters.AddWithValue("@maxPrice", demand.maxPrice);
                comm.Parameters.AddWithValue("@orderName", demand.orderName);
                comm.Parameters.AddWithValue("@region", demand.region);
                comm.Parameters.AddWithValue("@uname", HttpContext.Current.User.Identity.Name);
                conn.Open(); comm.ExecuteNonQuery(); conn.Close();

                TextBox5.Text = "";
                IshtCloseButton_Click(sender, e);
            }
        }


        protected void Button6_Click(object sender, EventArgs e)
        {
            if (DropDownList2.SelectedIndex > 0)
            {
                comm = new MySqlCommand("UPDATE _profiles SET active='0' WHERE uname=@uname AND profileName=@profileName", conn);
                comm.Parameters.AddWithValue("@profileName", DropDownList2.SelectedItem.ToString());
                comm.Parameters.AddWithValue("@uname", HttpContext.Current.User.Identity.Name);
                conn.Open(); comm.ExecuteNonQuery(); conn.Close();
                DropDownList2.Items.RemoveAt(DropDownList2.SelectedIndex);
            }
        }

        // выбранный профиль
        protected void DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownList2.SelectedIndex > 0)
            {
                // чистим от старых данных
                OKDPcbl.Items.Clear();
                TextBox3.Text = "";
                TextBox4.Text = "";
                foreach (ListItem li in CheckBoxList1.Items)
                    li.Selected = false;
                CheckBox1.Checked = false;
                CheckBox2.Checked = false;
                SDemand demand = new SDemand();
                comm = new MySqlCommand("SELECT productCode,productName,publishDate,maxPrice,orderName,region FROM _profiles WHERE profileName=@profileName AND uname=@uname", conn);
                comm.Parameters.AddWithValue("@profileName", DropDownList2.SelectedItem.ToString());
                comm.Parameters.AddWithValue("@uname", HttpContext.Current.User.Identity.Name);
                conn.Open(); reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0)) demand.productCode = reader.GetString(0);
                    if (!reader.IsDBNull(1)) demand.productName = reader.GetString(1);
                    if (!reader.IsDBNull(2)) demand.publishDate = reader.GetString(2);
                    if (!reader.IsDBNull(3)) demand.maxPrice = reader.GetString(3);
                    if (!reader.IsDBNull(4)) demand.orderName = reader.GetString(4);
                    if (!reader.IsDBNull(5)) demand.region = reader.GetString(5);
                }
                reader.Close(); conn.Close();
                // смотрим че и куда запихнуть
                if (demand.productCode != null)
                {
                    foreach (string oneOKDP in demand.productCode.Split(','))
                    {
                        OKDPcbl.Items.Add(oneOKDP);
                        OKDPcbl.Items[OKDPcbl.Items.Count - 1].Attributes.Add("class", "checkboxList_item");
                        OKDPcbl.Items[OKDPcbl.Items.Count - 1].Selected = true;
                    }
                }
                TextBox3.Text = demand.orderName ?? "";
                if (demand.publishDate != null)
                {
                    if (demand.publishDate.IndexOf('>') > -1)
                    {
                        TextBox4.Text = demand.publishDate.Remove(0, 1);
                        CheckBox1.Checked = true;
                    }
                    else
                    {
                        TextBox4.Text = demand.publishDate;
                        CheckBox1.Checked = false;
                    }
                }
                // регион
                for (int i = 0; i < CheckBoxList1.Items.Count; i++)
                    CheckBoxList1.Items[i].Selected = false;
                if (demand.region != null)
                {
                    foreach (string oneRegion in demand.region.Split(','))
                    {
                        for (int i = 0; i < CheckBoxList1.Items.Count; i++)
                        {
                            if (oneRegion == CheckBoxList1.Items[i].Text)
                                CheckBoxList1.Items[i].Selected = true;
                        }
                    }
                    RegionLbl.Text = Common.TruncateStr(demand.region);
                    RegionLbl.ToolTip = demand.region;
                }
                else
                {
                    RegionLbl.Text = "Все";
                }
                OKDPinnerText();
            }
        }
        #endregion

        #region Экспорт в Эксель
        protected void Button7_Click(object sender, EventArgs e)
        {
            if (_commStr != null)
            {
                Response.Clear();
                Response.AddHeader("content-disposition", "attachment;filename=TViewExport_" + DateTime.Now.ToShortDateString() + ".xls");
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/vnd.xls";

                mSQLData.SelectCommand = _commStr;
                DataView dv = (DataView)mSQLData.Select(DataSourceSelectArguments.Empty);
                DataTable dt = dv.ToTable();
                dt.Columns[0].ColumnName = "ID";
                dt.Columns[1].ColumnName = "Номер извещения";
                dt.Columns[2].ColumnName = "Тип";
                dt.Columns[3].ColumnName = "Заказ";
                dt.Columns[4].ColumnName = "Макс. цена";
                dt.Columns[5].ColumnName = "Разместил";
                dt.Columns[6].ColumnName = "Дата";
                dt.Columns[7].ColumnName = "Ссылка";
                dt.Columns[8].ColumnName = "Регион";
                dt.Columns[9].ColumnName = "Площадка";
                dt.Columns[10].ColumnName = "ИНН";
                dt.Columns[11].ColumnName = "ОГРН";

                StringWriter stringWrite = new StringWriter();
                HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
                DataGrid dg = new DataGrid();
                dg.DataSource = dt;      
                dg.DataBind();
                dg.RenderControl(htmlWrite);
                Response.Write(stringWrite.ToString());
                Response.End();
            }
        }
        #endregion
        
        #region Очистка формы поиска
        protected void ClearFormBtn_Click(object sender, EventArgs e)
        {
            TextBox3.Text = "";
            TextBox4.Text = DateTime.Now.ToShortDateString();
            MaxPriceTB.Text = "";
            RegionLbl.Text = "Все";
            CheckBox1.Checked = false;
            CheckBox2.Checked = false;
            OKDPcbl.Items.Clear();
            TextBox6.Text = DateTime.Now.ToShortDateString();
            foreach (ListItem li in CheckBoxList1.Items)
                if (li.Selected) li.Selected = false;
            OKDPinnerText();
        } 
        #endregion

        #region Создание рассылки по профилю
        protected void Button10_Click(object sender, EventArgs e)
        {
            if (DropDownList2.SelectedIndex > 0)
            {
                // смотрим, не создана ли уже подобная рассылка
                comm = new MySqlCommand("SELECT COUNT(*) FROM _request WHERE uname=@uname AND profileName=@profileName", conn);
                comm.Parameters.AddWithValue("@uname", HttpContext.Current.User.Identity.Name);
                comm.Parameters.AddWithValue("@profileName", DropDownList2.SelectedItem.ToString());
                conn.Open(); int cnt = Convert.ToInt16(comm.ExecuteScalar());
                if (cnt == 0)
                {
                    // получаем email
                    comm = new MySqlCommand("SELECT email FROM _users WHERE login=@login", conn);
                    comm.Parameters.AddWithValue("@login", HttpContext.Current.User.Identity.Name);
                    string email = Convert.ToString(comm.ExecuteScalar());

                    // пишем всё что нужно в _request
                    comm = new MySqlCommand("INSERT INTO _request (uname,profileName,email,commandString,attach) VALUES (@uname,@profileName,@email,@commandString,'1')", conn);
                    comm.Parameters.AddWithValue("@uname", HttpContext.Current.User.Identity.Name);
                    comm.Parameters.AddWithValue("@profileName",DropDownList2.SelectedItem.ToString());
                    comm.Parameters.AddWithValue("@email",email);
                    comm.Parameters.AddWithValue("@commandString",_commStr);
                    comm.ExecuteNonQuery();
                }
                else
                {
                    Label2.Text = "Рассылка по выбранному профилю уже создана!";
                    Label2.Visible = true;
                }
                conn.Close();
            }
        }
        #endregion
        
        #region Переключение табов
        protected void SearchTab_Click(object sender, EventArgs e)
        {
            divSettings.Attributes["class"] = "navigationLevel2_item";
            divSearch.Attributes["class"] = "navigationLevel2_item navigationLevel2_item__active";
            TablesView.ActiveViewIndex = 0;
        }

        protected void SettingsTab_Click(object sender, EventArgs e)
        {
            divSearch.Attributes["class"] = "navigationLevel2_item";
            divSettings.Attributes["class"] = "navigationLevel2_item navigationLevel2_item__active";
            TablesView.ActiveViewIndex = 1;
            SavedOptionsLB.Items.Clear();
            SavedMailsLB.Items.Clear();
            conn.Open();
            comm = new MySqlCommand("SELECT profileName FROM _profiles WHERE uname=@uname AND active='1'", conn);
            comm.Parameters.AddWithValue("@uname", HttpContext.Current.User.Identity.Name);
            reader = comm.ExecuteReader();
            while (reader.Read())
                if (!reader.IsDBNull(0))
                    SavedOptionsLB.Items.Add(reader.GetString(0));
            reader.Close();
            comm = new MySqlCommand("SELECT profileName FROM _request WHERE uname=@uname AND active='1'", conn);
            comm.Parameters.AddWithValue("@uname", HttpContext.Current.User.Identity.Name);
            reader = comm.ExecuteReader();
            while (reader.Read())
                if (!reader.IsDBNull(0))
                    SavedMailsLB.Items.Add(reader.GetString(0));
            reader.Close();
            conn.Close();
        }

        protected void TabB2B_Click(object sender, EventArgs e)
        {
            divSearch.Attributes["class"] = "navigationLevel2_item";
            divSettings.Attributes["class"] = "navigationLevel2_item";
            TablesView.ActiveViewIndex = 2;
          //  VisibleContext(true);
        }

        #endregion

        #region Пустые необходимые методы
        protected void ZGrid_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

        }

        protected void LotButton_Click(object sender, EventArgs e)
        {

        }

        protected void IshtCloseButton_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region Grid DataBound
        protected void ZGrid_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (_sortStr != null)
            {
                LinkButton lb = (LinkButton)ZGrid.Controls[0].Controls[0].FindControl("MaxPricelb");
                lb.Text = "Макс. цена";
                lb = (LinkButton)ZGrid.Controls[0].Controls[0].FindControl("PublishDatelb");
                lb.Text = "Дата размещения";
                lb = (LinkButton)ZGrid.Controls[0].Controls[0].FindControl("Regionlb");
                lb.Text = "Регион";
                if (_sortStr.IndexOf("MaxPrice") > -1)
                {
                    lb = (LinkButton)ZGrid.Controls[0].Controls[0].FindControl("MaxPricelb");
                    lb.Text = _sortStr.IndexOf("ASC") > -1 ? "Макс. цена ↑" : "Макс. цена ↓";
                }
                else if (_sortStr.IndexOf("PublishDate") > -1)
                {
                    lb = (LinkButton)ZGrid.Controls[0].Controls[0].FindControl("PublishDatelb");
                    lb.Text = _sortStr.IndexOf("ASC") > -1 ? "Дата размещения ↑" : "Дата размещения ↓";
                }
                else if (_sortStr.IndexOf("REGION") > -1)
                {
                    lb = (LinkButton)ZGrid.Controls[0].Controls[0].FindControl("Regionlb");
                    lb.Text = _sortStr.IndexOf("ASC") > -1 ? "Регион ↑" : "Регион ↓";
                }
            }
        }

        #endregion

        #region Сортировки
        private void SortColumn(string value)
        {
            if (_sortStr != null)
                _sortStr = _sortStr.IndexOf(value + " ASC") > -1 ? value + " DESC" : value + " ASC";
            else
                _sortStr = value + " ASC";
            GetPageItems(Convert.ToInt16(ItemsOnPageDDL.SelectedValue));
        }

        protected void MaxPricelb_Click(object sender, EventArgs e)
        {
            SortColumn("MaxPrice");
        }

        protected void PublishDatelb_Click(object sender, EventArgs e)
        {
            SortColumn("PublishDate");
        }

        protected void Regionlb_Click(object sender, EventArgs e)
        {
            SortColumn("REGION");
        } 
        #endregion


        #region Управление

        #region Удаление рассылки и условий поиска
        protected void DeleteOptionsBtn_Click(object sender, EventArgs e)
        {
            if (SavedOptionsLB.SelectedIndex >= 0)
            {
                comm = new MySqlCommand("UPDATE _profiles SET active='0' WHERE profileName=@profileName", conn);
                comm.Parameters.AddWithValue("@profileName", SavedOptionsLB.SelectedValue);
                conn.Open();
                comm.ExecuteNonQuery();
                conn.Close();
                SavedOptionsLB.Items.RemoveAt(SavedOptionsLB.SelectedIndex);
            }
        }

        protected void DeleteMailsBtn_Click(object sender, EventArgs e)
        {
            if (SavedMailsLB.SelectedIndex >= 0)
            {
                comm = new MySqlCommand("UPDATE _request SET active='0' WHERE profileName=@profileName", conn);
                comm.Parameters.AddWithValue("@profileName", SavedMailsLB.SelectedValue);
                conn.Open();
                comm.ExecuteNonQuery();
                conn.Close();
                SavedMailsLB.Items.RemoveAt(SavedMailsLB.SelectedIndex);
            }
        }
        #endregion

        #endregion



    }
}