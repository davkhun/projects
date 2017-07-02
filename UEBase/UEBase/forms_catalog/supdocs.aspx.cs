using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Ionic.Zip;

namespace UEBase
{
    public partial class supdocs : System.Web.UI.Page
    {

        public CUser user
        {
            get
            {
                return Session["_UEuser"] as CUser;
            }
        }

        private string commStr { get { return Session["_supdocsSelect"]?.ToString(); } set { Session["_supdocsSelect"] = value; } }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!DesignMode && !Page.IsPostBack)
            {
                commStr = user.admin ?
                    "SELECT sd.id,s.short_name,sd.doc_number,date_format(sd.date,'%d.%m.%Y') as `date`,sd.price,sd.scan_path FROM supdocs sd LEFT JOIN suppliers s ON s.id=sd.supplier_id WHERE uname='" + user.uname + "' ORDER BY sd.id DESC" :
                    "SELECT sd.id,s.short_name,sd.doc_number,date_format(sd.date,'%d.%m.%Y') as `date`,sd.price,sd.scan_path FROM supdocs sd LEFT JOIN suppliers s ON s.id=sd.supplier_id ORDER BY sd.id DESC";
                MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQLString"].ConnectionString);

                if (supDdl.Items.Count == 0)
                {
                    supplierFilterDdl.Items.Add(new ListItem("", "-1"));
                    MySqlCommand comm = new MySqlCommand("SELECT short_name,id FROM suppliers ORDER BY name ASC", conn);
                    conn.Open();
                    MySqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        supDdl.Items.Add(new ListItem(reader.GetString(0), reader.GetString(1)));
                        supplierFilterDdl.Items.Add(new ListItem(reader.GetString(0), reader.GetString(1)));
                    }
                    reader.Close();
                    conn.Close();
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            MaintainScrollPositionOnPostBack = true;
            if (!user.admin && !user.superAdmin)
                Response.Redirect("~/default.aspx");
            if (user.admin)
                filterBtn.Visible = false;
            MSQLData.SelectCommand = commStr;
            Q1.btnHandler += Q1_btnHandler;
        }

        private void Q1_btnHandler(string value)
        {
            if (value.Contains("SupDocDelete"))
            {
                MSQLData.UpdateCommand = "UPDATE supdocs SET active=0 WHERE id=@id";
                MSQLData.UpdateParameters.Add("@id", value.Split('|')[1]);
                MSQLData.Update();
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (!FileUpload1.HasFile)
            {
                M1.SetMessage = "Не выбран скан накладной!";
                MessageExtender.Show();
                return;
            }

            if (string.IsNullOrEmpty(doc_numberT.Text))
            {
                M1.SetMessage = "Не указан номер накладной!";
                MessageExtender.Show();
                return;
            }

            List<string> fileList = new List<string>();
            HttpFileCollection hfc = Request.Files;
            foreach (string file in hfc.Keys)
            {
                HttpPostedFile hpf = hfc[file];
                FileInfo fi = new FileInfo(hpf.FileName);
                string fn = fi.Name + "_" + DateTime.Now.Ticks + fi.Extension;
                fileList.Add(fn);
                string saveLocation = Server.MapPath("~/upload/") + fn;
                FileUpload1.PostedFile.SaveAs(saveLocation);
            }
            MSQLData.InsertCommand = "INSERT IGNORE INTO supdocs (supplier_id,doc_number,date,price,scan_path,uname,date_insert) VALUES (@sid,@docnum,@date,@price,@scan_path,@uname,now())";
            MSQLData.InsertParameters.Add("@sid", supDdl.SelectedValue);
            MSQLData.InsertParameters.Add("@docnum", doc_numberT.Text);
            MSQLData.InsertParameters.Add("@date", string.IsNullOrEmpty(dateT.Text) ? null : Convert.ToDateTime(dateT.Text).ToString("yyyy-MM-dd"));
            MSQLData.InsertParameters.Add("@price", priceT.Text.Replace(" ", "").Replace("_", "").Replace(",", "."));
            MSQLData.InsertParameters.Add("@scan_path", JsonConvert.SerializeObject(fileList));
            MSQLData.InsertParameters.Add("@uname", user.uname);
            MSQLData.Insert();

            doc_numberT.Text = string.Empty;
            dateT.Text = string.Empty;
            priceT.Text = string.Empty;
            newRecDiv.Visible = false;
        }

        protected void addRecBtn_Click(object sender, EventArgs e)
        {
            newRecDiv.Visible = !newRecDiv.Visible;
            GridView1.EditIndex = -1;
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "downloadSup")
            {
                int rowInd = Convert.ToInt32(e.CommandArgument);
                string jsonpath = (GridView1.Rows[rowInd].FindControl("downloadPath") as Label).Text;
                List<string> path = JsonConvert.DeserializeObject<List<string>>(jsonpath);
                if (path.Count == 1)
                {
                    FileInfo fi = new FileInfo(path[0]);
                    string downloadName = (GridView1.Rows[rowInd].FindControl("nameL") as Label).Text + "_" + (GridView1.Rows[rowInd].FindControl("docnumL") as Label).Text + fi.Extension;
                    Response.Clear();
                    Response.AddHeader("Content-Disposition", $"attachment;filename=\"{downloadName}\"");
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    HttpContext.Current.Response.Charset = "utf-8";
                    HttpContext.Current.Response.ContentEncoding = Encoding.GetEncoding("windows-1251");
                    Response.TransmitFile(Server.MapPath("~/upload/" + path[0]));
                }
                else // если файлов много - пакуем и отдаем архив
                {
                    foreach (string fname in Directory.GetFiles(Server.MapPath("~") + @"upload\tmp\"))
                        File.Delete(fname);
                    string packageName = "docs_" + DateTime.Now.ToShortDateString() + ".zip";
                    foreach (string one in path)
                    {

                        File.Copy(Server.MapPath("~") + @"upload\" + one, Server.MapPath("~") + @"upload\tmp\" + one);
                    }
                    using (ZipFile z = new ZipFile())
                    {
                        z.AlternateEncodingUsage = ZipOption.Always;
                        z.AlternateEncoding = Encoding.UTF8;
                        foreach (string fname in Directory.GetFiles(Server.MapPath("~") + @"upload\tmp\"))
                            z.AddFile(fname, "");
                        z.Save(Server.MapPath("~") + @"upload\tmp\" + packageName);
                    }
                    Response.Clear();
                    Response.AddHeader("Content-Disposition",$"attachment;filename=\"{HttpUtility.UrlEncode(packageName, Encoding.UTF8).Replace('+', '_')}\"");
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.TransmitFile(Server.MapPath("~/upload/tmp/" + packageName));

                }

                Response.End();
            }
            if (e.CommandName == "ShowSup")
            {

                int rowInd = Convert.ToInt32(e.CommandArgument);
                LinkButton lb = GridView1.Rows[rowInd].FindControl("showL") as LinkButton;

                Image img = GridView1.Rows[rowInd].FindControl("Image1") as Image;
                if (lb.Text == "Показать")
                {
                    string path = (GridView1.Rows[rowInd].FindControl("downloadPath") as Label).Text;
                    List<string> jsonPath = JsonConvert.DeserializeObject<List<string>>(path);
                    img.ImageUrl = "/upload/" + jsonPath[0];
                    lb.Text = "Скрыть";
                    return;
                }
                if (lb.Text == "Скрыть")
                {
                    img.ImageUrl = null;
                    lb.Text = "Показать";
                }
            }
            if (e.CommandName == "uploadSup")
            {
                int rowInd = Convert.ToInt32(e.CommandArgument);
                FileUpload fu = GridView1.Rows[rowInd].FindControl("uploadDocFU") as FileUpload;
                List<string> fileList = new List<string>();
                HttpFileCollection hfc = Request.Files;
                foreach (string file in hfc.Keys)
                {
                    HttpPostedFile hpf = hfc[file];
                    FileInfo fi = new FileInfo(hpf.FileName);
                    string fn = fi.Name + "_" + DateTime.Now.Ticks + fi.Extension;
                    fileList.Add(fn);
                    string saveLocation = Server.MapPath("~/upload/") + fn;
                    fu.PostedFile.SaveAs(saveLocation);
                }


                MSQLData.UpdateCommand = "UPDATE supdocs SET scan_path=@sp WHERE id=@id";
                MSQLData.UpdateParameters.Add("@sp", JsonConvert.SerializeObject(fileList));
                MSQLData.UpdateParameters.Add("@id", GridView1.DataKeys[rowInd].Value.ToString());
                MSQLData.Update();
            }
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            MSQLData.DeleteCommand = "SELECT 1";
            Q1.SetQuestion = "Вы действительно хотите удалить накладную <b>№ " + (GridView1.Rows[e.RowIndex].FindControl("docnumL") as Label).Text + "</b>?";
            Q1.Hidden = "SupDocDelete|" + e.Keys["id"];
            QuestionExtender.Show();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            MSQLData.UpdateCommand = "UPDATE supdocs SET date=@dt,price=@price WHERE id=@id";
            MSQLData.UpdateParameters.Add("@dt", string.IsNullOrEmpty(e.NewValues["date"].ToString()) ? null : Convert.ToDateTime(e.NewValues["date"]).ToString("yyyy-MM-dd"));
            MSQLData.UpdateParameters.Add("@price", e.NewValues["price"]?.ToString().Replace(" ", "").Replace("_", "").Replace(",", "."));
            MSQLData.UpdateParameters.Add("@id", GridView1.DataKeys[e.RowIndex].Value.ToString());
            MSQLData.Update();
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
                            string evt = Page.ClientScript.GetPostBackClientHyperlink(GridView1, "Update$" + GridView1.EditIndex);
                            StringBuilder js = new StringBuilder();
                            js.Append("if(event.which || event.keyCode)");
                            js.Append("{ if ((event.which == 13) || (event.keyCode == 13)) ");
                            js.Append($"{{{evt};return false;}}}}");
                            string strJs = js.ToString();
                            ((TextBox) (c)).Attributes.Add("onkeydown", strJs);

                        }
                    }
                }
            }
            if (GridView1.Rows.Count > 0)
            {
                GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }

        protected void filterBtn_Click(object sender, EventArgs e) { filterDiv.Visible = !filterDiv.Visible; }

        #region Фильтр

        protected void applyFilterBtn_Click(object sender, EventArgs e)
        {
            if (supplierFilterDdl.SelectedIndex > 0)
                commStr = "SELECT sd.id,s.short_name,sd.doc_number,date_format(sd.date,'%d.%m.%Y') as `date`,sd.price,sd.scan_path FROM supdocs sd LEFT JOIN suppliers s ON s.id=sd.supplier_id WHERE sd.supplier_id=" + supplierFilterDdl.SelectedValue + " ORDER BY sd.id DESC";
            if (!string.IsNullOrEmpty(dateFilterT1.Text))
            {
                if (!string.IsNullOrEmpty(dateFilterT2.Text))
                    commStr = "SELECT sd.id,s.short_name,sd.doc_number,date_format(sd.date,'%d.%m.%Y') as `date`,sd.price,sd.scan_path FROM supdocs sd LEFT JOIN suppliers s ON s.id=sd.supplier_id WHERE date BETWEEN '" + Convert.ToDateTime(dateFilterT1.Text).ToString("yyyy-MM-dd") + "' AND '" + Convert.ToDateTime(dateFilterT2.Text).ToString("yyyy-MM-dd") + "' ORDER BY sd.id DESC";
                else
                    commStr = "SELECT sd.id,s.short_name,sd.doc_number,date_format(sd.date,'%d.%m.%Y') as `date`,sd.price,sd.scan_path FROM supdocs sd LEFT JOIN suppliers s ON s.id=sd.supplier_id WHERE date>='" + Convert.ToDateTime(dateFilterT1.Text).ToString("yyyy-MM-dd") + "' ORDER BY sd.id DESC";

            }
            if (string.IsNullOrEmpty(docNumT.Text))
            {
                commStr = "SELECT sd.id,s.short_name,sd.doc_number,date_format(sd.date,'%d.%m.%Y') as `date`,sd.price,sd.scan_path FROM supdocs sd LEFT JOIN suppliers s ON s.id=sd.supplier_id WHERE doc_number LIKE '" + docNumT.Text + "%' ORDER BY sd.id DESC";
            }
            MSQLData.SelectCommand = commStr;
        }

        protected void clearFilterBtn_Click(object sender, EventArgs e)
        {
            GridView1.EditIndex = -1;
            supplierFilterDdl.SelectedIndex = 0;
            dateFilterT1.Text = string.Empty;
            dateFilterT2.Text = string.Empty;
            docNumT.Text = string.Empty;
            commStr = "SELECT sd.id,s.short_name,sd.doc_number,date_format(sd.date,'%d.%m.%Y') as `date`,sd.price,sd.scan_path FROM supdocs sd LEFT JOIN suppliers s ON s.id=sd.supplier_id ORDER BY sd.id DESC";
            MSQLData.SelectCommand = commStr;
        }

        #endregion

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label dpath = e.Row.FindControl("downloadPath") as Label;
                // если накладной нет, покажем возможность загрузки
                if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                {
                    dpath = e.Row.FindControl("downloadPath") as Label;
                    FileUpload fu = e.Row.FindControl("uploadDocFU") as FileUpload;
                    Button fuBtn = e.Row.FindControl("uploadDocBtn") as Button;
                    if (string.IsNullOrEmpty(dpath.Text))
                    {
                        fu.Visible = true;
                        fuBtn.Visible = true;
                    }
                    else
                    {
                        fu.Visible = false;
                        fuBtn.Visible = false;
                    }
                }
                    // прячем скачку если накладной нет
                else
                {
                    LinkButton downloadL = e.Row.FindControl("downloadL") as LinkButton;
                    LinkButton showL = e.Row.FindControl("showL") as LinkButton;
                    if (string.IsNullOrEmpty(dpath.Text))
                    {


                        downloadL.Visible = false;
                        showL.Visible = false;
                    }
                    else // показываем количество файлов для скачки
                    {
                        int fileCount = JsonConvert.DeserializeObject<List<string>>(dpath.Text).Count;

                        string fileEndStr = null;
                        if (fileCount == 1)
                            fileEndStr = " файл)";
                        else if (fileCount > 1 && fileCount < 5)
                            fileEndStr = " файла)";
                        else if (fileCount >= 5)
                            fileEndStr = " файлов)";
                        downloadL.Text = "Скачать (" + fileCount + fileEndStr;
                    }

                }

            }
        }

        protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
        {
        }


    }

}
