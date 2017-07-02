using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;


namespace UEBase
{


    public partial class _default : System.Web.UI.Page
    {

        private MySqlConnection conn;
        private MySqlCommand comm;
        private MySqlDataReader reader;
        private MySqlDataAdapter adap;


        public CUser user
        {
            get
            {
                return Session["_UEuser"] as CUser;
            }
        }

        public string commStr
        {
            get
            {
                return Session["__COMMSTR"]?.ToString();
            }
            set { Session["__COMMSTR"] = value; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {

            if (Request.QueryString["exit"] != null)
            {
                Response.Cookies["UEUname"].Expires = DateTime.Now.AddDays(-1);
                Response.Cookies["UEPwd"].Expires = DateTime.Now.AddDays(-1);
                Response.Redirect("login.aspx");
            } 
            if (user == null)
                Response.Redirect("login.aspx");

            if (!Page.IsPostBack && !DesignMode)
            {



                conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQLString"].ConnectionString);
                // разные условия, суперадмин видит всё, простой админ - только в разрезе своих подразделений, пользователь - только свои, если руководитель - только в свои подразделения
                // руководитель
                if (user.registered)
                    commStr = @"SELECT u.id,u.inv_number,u.is_inv,t.value as type,m.name as model,u.manager,u.manager_uname,st.value,d.code,d.description,u.office,u.manager_placement,u.ue_price,sd.doc_number,s.name,u.comment
FROM ue u 
LEFT JOIN ue_type t ON t.id=u.type_id
LEFT JOIN models m ON m.type_id=t.id AND m.id=u.model_id
LEFT JOIN status st ON st.id = u.status_id
LEFT JOIN divisions d ON d.id=u.subdivision_id
LEFT JOIN supdocs sd ON sd.id=u.supdocs_id
LEFT JOIN suppliers s ON s.id=sd.supplier_id
WHERE u.subdivision_id IN (" + user.divisions + @")
GROUP BY u.id ORDER BY u.id DESC";
                if (user.superAdmin)
                    commStr = @"SELECT u.id,u.inv_number,u.is_inv,t.value as type,m.name as model,u.manager,u.manager_uname,st.value,d.code,d.description,u.office,u.manager_placement,u.ue_price,sd.doc_number,s.name,u.comment
FROM ue u 
LEFT JOIN ue_type t ON t.id=u.type_id
LEFT JOIN models m ON m.type_id=t.id AND m.id=u.model_id
LEFT JOIN status st ON st.id = u.status_id
LEFT JOIN divisions d ON d.id=u.subdivision_id
LEFT JOIN supdocs sd ON sd.id=u.supdocs_id
LEFT JOIN suppliers s ON s.id=sd.supplier_id
GROUP BY u.id ORDER BY u.id DESC";
                if (user.admin && !string.IsNullOrEmpty(user.divisions))
                    commStr = @"SELECT u.id,u.inv_number,u.is_inv,t.value as type,m.name as model,u.manager,u.manager_uname,st.value,d.code,d.description,u.office,u.manager_placement,u.ue_price,sd.doc_number,s.name,u.comment
FROM ue u 
LEFT JOIN ue_type t ON t.id=u.type_id
LEFT JOIN models m ON m.type_id=t.id AND m.id=u.model_id
LEFT JOIN status st ON st.id = u.status_id
LEFT JOIN divisions d ON d.id=u.subdivision_id
LEFT JOIN supdocs sd ON sd.id=u.supdocs_id
LEFT JOIN suppliers s ON s.id=sd.supplier_id
WHERE u.subdivision_id IN (" + user.divisions + @")
GROUP BY u.id ORDER BY u.id DESC";
                if (user.admin && string.IsNullOrEmpty(user.divisions))
                {
                    Response.Write("Обнаружены права админа подразделения, однако не выбрано подразделений для пользователя " + user.uname);
                    return;
                }

                // обычный чел
                if (!user.registered)
                    commStr = @"SELECT u.id,u.inv_number,u.is_inv,t.value as type,m.name as model,u.manager,u.manager_uname,st.value,d.code,d.description,u.office,u.manager_placement,u.ue_price,sd.doc_number,s.name,u.comment
FROM ue u 
LEFT JOIN ue_type t ON t.id=u.type_id
LEFT JOIN models m ON m.type_id=t.id AND m.id=u.model_id
LEFT JOIN status st ON st.id = u.status_id
LEFT JOIN divisions d ON d.id=u.subdivision_id
LEFT JOIN supdocs sd ON sd.id=u.supdocs_id
LEFT JOIN suppliers s ON s.id=sd.supplier_id
WHERE u.manager='" + user.fio + @"' 
GROUP BY u.id ORDER BY u.id DESC";

                if (typeDdl.Items.Count == 0)
                {
                    typeDl.Items.Add(new ListItem("", "-1"));
                    modelDl.Items.Add(new ListItem("", "-1"));
                    divisionDl.Items.Add(new ListItem("", "-1"));
                    conn.Open();
                    comm = new MySqlCommand("SELECT value,id FROM ue_type sd ORDER BY value ASC", conn);
                    reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        typeDdl.Items.Add(new ListItem(reader.GetString(0), reader.GetString(1)));
                        typeDl.Items.Add(new ListItem(reader.GetString(0), reader.GetString(1)));
                        requestTypeDdl.Items.Add(new ListItem(reader.GetString(0), reader.GetString(1)));
                    }
                    reader.Close();
                    comm = new MySqlCommand("SELECT value,id FROM status ORDER BY value ASC", conn);
                    reader = comm.ExecuteReader();
                    while (reader.Read())
                        statusDdl.Items.Add(new ListItem(reader.GetString(0), reader.GetString(1)));
                    reader.Close();
                    comm = new MySqlCommand("SELECT code,description,id FROM divisions ORDER BY description ASC", conn);
                    reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        subdivisionDdl.Items.Add(new ListItem($"{reader.GetString(0)} - {reader.GetString(1)}", reader.GetString(2)));
                        divisionDl.Items.Add(new ListItem($"{reader.GetString(0)} - {reader.GetString(1)}", reader.GetString(2)));
                    }
                    reader.Close();
                    comm = new MySqlCommand("SELECT name,id FROM models WHERE type_id=@id", conn);
                    comm.Parameters.AddWithValue("@id", typeDdl.SelectedValue);
                    reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        modelsDdl.Items.Add(new ListItem(reader.GetString(0), reader.GetString(1)));
                        modelDl.Items.Add(new ListItem(reader.GetString(0), reader.GetString(1)));
                    }
                    reader.Close();
                    conn.Close();
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {           
         

            // скрываем управление для пользователя
            if (!user.admin && !user.superAdmin)
            {
                addUEBtn.Visible = false;
                showFilter.Visible = false;

                requestUeBtn.Visible = true;
            }
            else
                requestUeBtn.Visible = false;
            // скрываем фильтр для простого админа и добавление новых моделей 
            if (user.admin)
            {
                addNewModelA.Visible = false;
                addNewTypeA.Visible = false;
                showFilter.Visible = false;
            }

            MSQLData.SelectCommand = commStr;
            conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQLString"].ConnectionString);
        }

        protected void addUEBtn_Click(object sender, EventArgs e) { newUEDiv.Visible = !newUEDiv.Visible; }

        protected void searchManagerB_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(managerT.Text))
            {
                M1.SetMessage = "Поле не должно быть пустым!";
                MessageExtender.Show();
                return;
            }
            if (managerT.Text.Length < 2)
            {
                M1.SetMessage = "Минимальная длина фамилии 3 символа!";
                MessageExtender.Show();
                return;
            }
            
            List<ADuser> usr = new ADconnector().GetUser(managerT.Text);
            if (usr.Count > 0)
            {
                if (usr.Count == 1)
                {
                    managerT.Text = usr[0].name;
                    subdivisionT.Text = usr[0].reply;
                    officeT.Text = usr[0].office;
                }
                else
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("ФИО");
                    dt.Columns.Add("Офис");
                    dt.Columns.Add("Подразделение");
                    foreach (ADuser one in usr)
                        dt.Rows.Add(one.name, one.office, one.reply);
                    dt.AcceptChanges();
                    UserGrid.DataSource = dt;
                    UserGrid.DataBind();
                    UserPanelExtender.Show();
                }
            }
            else
            {
                subdivisionT.Text = "<Не найдено!>";
                officeT.Text = "<Не найдено!>";
            }
        }

        protected void UserGrid_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            if (GridView1.EditIndex != -1) // если таблица редактируется, подставим в нее, иначе в форму добавления
            {
                TextBox t = GridView1.Rows[Convert.ToInt32(Session["_editRowIndex"])].FindControl("managerTb") as TextBox;
                Label sd = GridView1.Rows[Convert.ToInt32(Session["_editRowIndex"])].FindControl("divisionFactL") as Label;
                t.Text = UserGrid.Rows[e.NewSelectedIndex].Cells[1].Text;
                sd.Text = $"{UserGrid.Rows[e.NewSelectedIndex].Cells[2].Text}<br/>{UserGrid.Rows[e.NewSelectedIndex].Cells[3].Text}";
            }
            else
            {
                managerT.Text = UserGrid.Rows[e.NewSelectedIndex].Cells[1].Text;
                officeT.Text = UserGrid.Rows[e.NewSelectedIndex].Cells[2].Text;
                subdivisionT.Text = UserGrid.Rows[e.NewSelectedIndex].Cells[3].Text;
            }
        }

        protected void typeDdl_SelectedIndexChanged(object sender, EventArgs e)
        {
            modelsDdl.Items.Clear();
            comm = new MySqlCommand("SELECT name,id FROM models WHERE type_id=@id", conn);
            comm.Parameters.AddWithValue("@id", typeDdl.SelectedValue);
            conn.Open();
            reader = comm.ExecuteReader();
            while (reader.Read())
                modelsDdl.Items.Add(new ListItem(reader.GetString(0), reader.GetString(1)));
            reader.Close();
            conn.Close();
        }


        public void InsertUeData()
        {
            if (string.IsNullOrEmpty(invNumT.Text))
            {
                M1.SetMessage = "Не заполнен инвентаризационный номер!";
                MessageExtender.Show();
                return;
            }
            if (string.IsNullOrEmpty(supDocT.Text))
            {
                M1.SetMessage = "Не выбрана накладная!";
                MessageExtender.Show();
                return;
            }
            if (!supDocT.Text.Contains('-') && !supDocT.Text.Contains('('))
            {
                M1.SetMessage = "Неправильно выбрана накладная!";
                MessageExtender.Show();
                return;
            }
            List<ADuser> usr = new List<ADuser>();
            if (!string.IsNullOrEmpty(managerT.Text))
                usr = new ADconnector().GetUser(managerT.Text);
            comm = new MySqlCommand("INSERT IGNORE INTO ue (inv_number,serial_number,is_inv,type_id,model_id,manager,manager_uname,status_id,subdivision_id,office,ue_price,supdocs_id,comment,update_date,uname,manager_placement) VALUES (@inv_number,@sn,@is_inv,@type_id,@model_id,@manager,@manager_uname,@status_id,@subdivision_id,@office,@ue_price,@supdocs_id,@comment,now(),@uname,@mp)", conn);
            comm.Parameters.AddWithValue("@inv_number", invNumT.Text.Trim());
            comm.Parameters.AddWithValue("@sn", serNumT.Text.Trim());
            comm.Parameters.AddWithValue("@is_inv", isInvC.Checked ? 1 : 0);
            comm.Parameters.AddWithValue("@type_id", typeDdl.SelectedValue);
            comm.Parameters.AddWithValue("@model_id", modelsDdl.SelectedValue);
            comm.Parameters.AddWithValue("@manager", string.IsNullOrEmpty(managerT.Text) ? null : managerT.Text);
            comm.Parameters.AddWithValue("@manager_uname", usr.Count > 0 ? usr[0].samAccountName : null);
            comm.Parameters.AddWithValue("@status_id", statusDdl.SelectedValue);
            comm.Parameters.AddWithValue("@subdivision_id", subdivisionDdl.SelectedValue);
            comm.Parameters.AddWithValue("@office", string.IsNullOrEmpty(officeT.Text) ? null : officeT.Text);
            comm.Parameters.AddWithValue("@ue_price", priceT.Text.Replace(" ", ""));
            comm.Parameters.AddWithValue("@supdocs_id", supDocT.Text.Split('-')[0].Trim());
            comm.Parameters.AddWithValue("@comment", string.IsNullOrEmpty(commentT.Text) ? null : commentT.Text);
            comm.Parameters.AddWithValue("@mp", string.IsNullOrEmpty(subdivisionT.Text) ? null : subdivisionT.Text);
            comm.Parameters.AddWithValue("@uname", user.uname);
            conn.Open();
            comm.ExecuteNonQuery();
            if (usr.Count > 0) // если есть пользователь в AD, пишем его текущее положение в базу
            {
                comm = new MySqlCommand("INSERT IGNORE INTO manager_move (division,date,placement,uname,manager) VALUES (@division,now(),@placement,@uname,@manager)", conn);
                comm.Parameters.AddWithValue("@division", usr[0].office);
                comm.Parameters.AddWithValue("@placement", usr[0].placement);
                comm.Parameters.AddWithValue("@uname", usr[0].samAccountName);
                comm.Parameters.AddWithValue("@manager", usr[0].name);
                comm.ExecuteNonQuery();
            }
            conn.Close();
            GridView1.DataBind();
        }


        protected void Button1_Click(object sender, EventArgs e)
        {
            InsertUeData();
            invNumT.Text = string.Empty;
            isInvC.Checked = false;
            managerT.Text = string.Empty;
            statusDdl.SelectedIndex = 0;
            subdivisionDdl.SelectedIndex = 0;
            officeT.Text = string.Empty;
            priceT.Text = string.Empty;
            supDocT.Text = string.Empty;
            commentT.Text = string.Empty;
            modelsDdl.SelectedIndex = 0;
            subdivisionT.Text = string.Empty;
           
            newUEDiv.Visible = false;

        }


        public void SetSelectedIndex(DropDownList ddl, DataRowView drv, string cellName)
        {
            for (int i = 0; i < ddl.Items.Count; i++)
            {
                if (ddl.Items[i].Text.Equals(drv[cellName].ToString()))
                {
                    ddl.SelectedIndex = i;
                    break;
                }
            }
        }

        public void SetupDropDownList(DropDownList ddl, string commstr, string textField, string valueField)
        {
            DataSet ds = new DataSet();
            adap = new MySqlDataAdapter(commstr, conn);
            adap.Fill(ds);
            ddl.DataSource = ds;
            ddl.DataTextField = textField;
            ddl.DataValueField = valueField;
            ddl.DataBind();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // скрываем редактирование для обычного пользователя
                if (!user.admin && !user.superAdmin)
                    GridView1.Columns[0].Visible = false;
                if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                {
                    DataRowView dr = e.Row.DataItem as DataRowView;
                    DropDownList ddl = (DropDownList) e.Row.FindControl("statusDdl");
                    SetupDropDownList(ddl, "SELECT id,value FROM status", "value", "id");
                    SetSelectedIndex(ddl, dr, "value");

                }
                else // а тут с простыми строками
                {
                    HyperLink hl = e.Row.FindControl("managerHl") as HyperLink;
                    Label l = e.Row.FindControl("managerL") as Label;
                    if (!hl.Visible && l.Text.Length>0)
                        e.Row.BackColor = System.Drawing.Color.LightPink;
                    Label stL = e.Row.FindControl("statusL") as Label;
                    if (stL.Text.ToLower().Contains("нов") && l.Text.Length == 0)
                        e.Row.BackColor = System.Drawing.Color.LightGreen;
                }
            }
            
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "FindAD")
            {
                int rowInd = Convert.ToInt32(e.CommandArgument);
                Session["_editRowIndex"] = rowInd;
                TextBox t = GridView1.Rows[rowInd].FindControl("managerTb") as TextBox;
                Label sd = GridView1.Rows[rowInd].FindControl("divisionFactL") as Label;
                if (string.IsNullOrEmpty(t.Text))
                {
                    M1.SetMessage = "Поле не должно быть пустым!";
                    MessageExtender.Show();
                    return;
                }
                if (t.Text.Length < 2)
                {
                    M1.SetMessage = "Минимальная длина фамилии 3 символа!";
                    MessageExtender.Show();
                    return;
                }

                List<ADuser> usr = new ADconnector().GetUser(t.Text);
                if (usr.Count > 0)
                {
                    if (usr.Count == 1)
                    {
                        t.Text = usr[0].name;
                        sd.Text = usr[0].office + "<br/>" + usr[0].reply;
                    }
                    else
                    {
                        DataTable dt = new DataTable();
                        dt.Columns.Add("ФИО");
                        dt.Columns.Add("Офис");
                        dt.Columns.Add("Подразделение");
                        foreach (ADuser one in usr)
                            dt.Rows.Add(one.name, one.office, one.reply);
                        dt.AcceptChanges();
                        UserGrid.DataSource = dt;
                        UserGrid.DataBind();
                        UserPanelExtender.Show();
                    }
                }
                else
                {
                    sd.Text = "<Не найдено!>";
                }
            }
            else if (e.CommandName == "DuplicateUE")
            {
                int rowInd = Convert.ToInt32(e.CommandArgument);
                string mySQLid = GridView1.DataKeys[rowInd].Value.ToString();
                comm = new MySqlCommand("SELECT type_id,model_id,status_id,subdivision_id,ue_price FROM ue WHERE id=@id", conn);
                comm.Parameters.AddWithValue("@id", mySQLid);
                newUEDiv.Visible = true;
                conn.Open();
                reader = comm.ExecuteReader();
                string modelID = null;
                while (reader.Read())
                {
                    typeDdl.SelectedValue = reader.GetString(0);
                    modelID = reader.GetString(1);
                    statusDdl.SelectedValue = reader.GetString(2);
                    subdivisionDdl.SelectedValue = reader.GetString(3);
                    priceT.Text = reader.GetString(4);
                }
                reader.Close();
                // тут изврат, строим таблицу моделей исходя из типа и выбираем его
                comm = new MySqlCommand("SELECT name,id FROM models WHERE type_id=@id", conn);
                comm.Parameters.AddWithValue("@id", typeDdl.SelectedValue);
                reader = comm.ExecuteReader();
                while (reader.Read())
                    modelsDdl.Items.Add(new ListItem(reader.GetString(0), reader.GetString(1)));
                reader.Close();
                modelsDdl.SelectedValue = modelID;
                conn.Close();
            }
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            List<ADuser> usr = new List<ADuser>();
            if (e.NewValues["manager"] != null)
                usr = new ADconnector().GetUser(e.NewValues["manager"].ToString());

            MSQLData.UpdateCommand = "UPDATE ue SET is_inv=@is_inv,manager=@manager,manager_uname=@muname,status_id=@sid,office=@office,comment=@comment,update_date=now(),uname=@uname,manager_placement=@mp WHERE id=@id";
            MSQLData.UpdateParameters.Add("@is_inv", e.NewValues["is_inv"].ToString().ToLower() == "false" ? "0" : "1");
            MSQLData.UpdateParameters.Add("@manager", e.NewValues["manager"]?.ToString());
            MSQLData.UpdateParameters.Add("@muname", usr.Count == 0 ? null : usr[0].samAccountName);
            MSQLData.UpdateParameters.Add("@mp", usr.Count == 0 ? null : usr[0].placement);
            MSQLData.UpdateParameters.Add("@sid", ((DropDownList)(GridView1.Rows[e.RowIndex].FindControl("statusDdl"))).SelectedValue);
            MSQLData.UpdateParameters.Add("@office", usr.Count == 0 ? null : usr[0].office);
            MSQLData.UpdateParameters.Add("@comment", e.NewValues["comment"]?.ToString());
            MSQLData.UpdateParameters.Add("@uname", user.uname);
            MSQLData.UpdateParameters.Add("@id", GridView1.DataKeys[e.RowIndex].Value.ToString());
            MSQLData.Update();

            // пишем изменение статуса
            Label l = GridView1.Rows[e.RowIndex].FindControl("statusL") as Label;
            if (l.Text != ((DropDownList) (GridView1.Rows[e.RowIndex].FindControl("statusDdl"))).SelectedItem.Text)
            {
                MSQLData.InsertCommand = "INSERT INTO status_history (date,old_status,new_status,uname) VALUES (now(),@os,@ns,@uname)";
                MSQLData.InsertParameters.Add("@os", l.Text);
                MSQLData.InsertParameters.Add("@ns", ((DropDownList) (GridView1.Rows[e.RowIndex].FindControl("statusDdl"))).SelectedItem.Text);
                MSQLData.InsertParameters.Add("@uname", user.uname);
                MSQLData.Insert();
            }
        }

        protected void GridView1_PreRender(object sender, EventArgs e)
        {
            if (GridView1.EditIndex > -1)
            {
                GridView1.Columns[7].Visible = false;
                GridView1.Columns[9].Visible = false;
                GridView1.Columns[10].Visible = false;

            }
            else
            {
                GridView1.Columns[7].Visible = true;
                GridView1.Columns[9].Visible = true;
                GridView1.Columns[10].Visible = true;

            }
        }

        protected void showFilter_Click(object sender, EventArgs e) { filterDiv.Visible = !filterDiv.Visible; }

        protected void applyFilterBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(filterTb.Text))
            {
                if (typeDl.SelectedIndex > 0 || modelDl.SelectedIndex > 0 || divisionDl.SelectedIndex > 0)
                {
                    string whereStr = null;
                    if (typeDl.SelectedIndex > 0)
                        whereStr += " t.id=" + typeDl.SelectedItem.Value + " AND";
                    if (modelDl.SelectedIndex > 0)
                        whereStr += " m.id=" + modelDl.SelectedItem.Value + " AND";
                    if (divisionDl.SelectedIndex > 0)
                        whereStr += " d.id=" + divisionDl.SelectedItem.Value + " AND";

                    whereStr = whereStr.Remove(whereStr.Length - 3);
                    commStr = @"SELECT u.id,u.inv_number,u.is_inv,t.value as type,m.name as model,u.manager,u.manager_uname,st.value,d.code,d.description,u.office,u.manager_placement,u.ue_price,sd.doc_number,s.name,u.comment
FROM ue u 
LEFT JOIN ue_type t ON t.id=u.type_id
LEFT JOIN models m ON m.type_id=t.id AND m.id=u.model_id
LEFT JOIN status st ON st.id = u.status_id
LEFT JOIN divisions d ON d.id=u.subdivision_id
LEFT JOIN supdocs sd ON sd.id=u.supdocs_id
LEFT JOIN suppliers s ON s.id=sd.supplier_id WHERE " + whereStr + @" 
GROUP BY u.id ORDER BY u.id DESC";
                    MSQLData.SelectCommand = commStr;

                }
                else
                {
                    commStr = @"SELECT u.id,u.inv_number,u.is_inv,t.value as type,m.name as model,u.manager,u.manager_uname,st.value,d.code,d.description,u.office,u.manager_placement,u.ue_price,sd.doc_number,s.name,u.comment
FROM ue u 
LEFT JOIN ue_type t ON t.id=u.type_id
LEFT JOIN models m ON m.type_id=t.id AND m.id=u.model_id
LEFT JOIN status st ON st.id = u.status_id
LEFT JOIN divisions d ON d.id=u.subdivision_id
LEFT JOIN supdocs sd ON sd.id=u.supdocs_id
LEFT JOIN suppliers s ON s.id=sd.supplier_id
GROUP BY u.id ORDER BY u.id DESC";
                    MSQLData.SelectCommand = commStr;

                }
            }
            else
            {
                string invNum = filterTb.Text.Split('-')[0].Trim();
                commStr = @"SELECT u.id,u.inv_number,u.is_inv,t.value as type,m.name as model,u.manager,u.manager_uname,st.value,d.code,d.description,u.office,u.manager_placement,u.ue_price,sd.doc_number,s.name,u.comment
FROM ue u 
LEFT JOIN ue_type t ON t.id=u.type_id
LEFT JOIN models m ON m.type_id=t.id AND m.id=u.model_id
LEFT JOIN status st ON st.id = u.status_id
LEFT JOIN divisions d ON d.id=u.subdivision_id
LEFT JOIN supdocs sd ON sd.id=u.supdocs_id
LEFT JOIN suppliers s ON s.id=sd.supplier_id WHERE u.inv_number='" + invNum + @"'
GROUP BY u.id ORDER BY u.id DESC";
                MSQLData.SelectCommand = commStr;
            }
        }

        protected void typeDl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (typeDl.SelectedIndex > 0)
            {
                conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQLString"].ConnectionString);
                conn.Open();
                modelDl.Items.Clear();
                modelDl.Items.Add(new ListItem("", "-1"));
                comm = new MySqlCommand("SELECT name,id FROM models WHERE type_id=@id", conn);
                comm.Parameters.AddWithValue("@id", typeDl.SelectedValue);
                reader = comm.ExecuteReader();
                while (reader.Read())
                    modelDl.Items.Add(new ListItem(reader.GetString(0), reader.GetString(1)));
                reader.Close();
                conn.Close();
            }
        }


        [System.Web.Script.Services.ScriptMethod]
        [System.Web.Services.WebMethod]
        public static List<string> SearchBase(string prefixText, int count)
        {
            MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQLString"].ConnectionString);
            List<string> result = new List<string>();
            string commstr = @"SELECT u.inv_number,t.value as type,m.name as model,u.office 
FROM ue u 
LEFT JOIN ue_type t ON t.id=u.type_id
LEFT JOIN models m ON m.type_id=t.id AND m.id=u.model_id
LEFT JOIN status st ON st.id = u.status_id
LEFT JOIN divisions d ON d.id=u.subdivision_id
LEFT JOIN supdocs sd ON sd.id=u.supdocs_id
LEFT JOIN suppliers s ON s.id=sd.supplier_id WHERE u.inv_number LIKE '" + prefixText + @"%' OR t.value LIKE '" + prefixText + @"%' OR m.name LIKE '" + prefixText + @"%' OR u.manager LIKE '" + prefixText + @"%' OR st.value LIKE '" + prefixText + @"%' OR sd.doc_number LIKE '" + prefixText + @"%' OR s.name LIKE '" + prefixText + @"%' OR u.comment LIKE '" + prefixText + @"%' 
GROUP BY u.id ORDER BY u.id DESC";
            MySqlCommand comm = new MySqlCommand(commstr, conn);
            conn.Open();
            using (MySqlDataReader reader = comm.ExecuteReader())
            {
                while (reader.Read())
                    result.Add($"{reader.GetString(0)} - {reader.GetString(1)} {reader.GetString(2)} {(reader.IsDBNull(3) ? "" : "(" + reader.GetString(3) + ")")}");
            }
            conn.Close();
            return result;
        }

        [System.Web.Script.Services.ScriptMethod]
        [System.Web.Services.WebMethod]
        public static List<string> SearchDoc(string prefixText, int count)
        {
            MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQLString"].ConnectionString);
            List<string> result = new List<string>();
            string commstr = @"SELECT concat(sd.id,' - ',sd.doc_number,' (',s.name,') ',sd.date,' ',sd.price) FROM supdocs sd JOIN suppliers s ON sd.supplier_id=s.id WHERE s.name LIKE '" + prefixText + "%' OR sd.price LIKE '" + prefixText + "%' OR date_format(sd.date,'%d.%m.%Y') LIKE '" + prefixText + "%' OR sd.doc_number LIKE '" + prefixText + "%' OR s.short_name LIKE '" + prefixText + "%' LIMIT 10";
            MySqlCommand comm = new MySqlCommand(commstr, conn);
            conn.Open();
            using (MySqlDataReader reader = comm.ExecuteReader())
            {
                while (reader.Read())
                    result.Add(reader.GetString(0));
            }
            conn.Close();
            return result;
        }

        // дублирование шаблона накладной
        protected void Button2_Click(object sender, EventArgs e)
        {
            InsertUeData();
            invNumT.Text = string.Empty;
            serNumT.Text = string.Empty;
            managerT.Text = string.Empty;
            officeT.Text = string.Empty;
            commentT.Text = string.Empty;
            subdivisionT.Text = string.Empty;
        }

        #region Заказ техники сотрудником
        protected void requestUeBtn_Click(object sender, EventArgs e) { requestUeDiv.Visible = !requestUeDiv.Visible; }
        #endregion

        protected void sendRequestBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(requestCommentT.Text))
            {
                M1.SetMessage = "Необходимо ввести комментарий!";
                MessageExtender.Show();
                return;
            }

            CMail mail = new CMail(ConfigurationManager.AppSettings["SMTPaddress"], Convert.ToInt32(ConfigurationManager.AppSettings["SMTPport"]));
            string mailFrom = ConfigurationManager.AppSettings["mailFrom"];
            string mailTo = ConfigurationManager.AppSettings["mailTo"];
            string body = $"Добрый день.<br/><br/>Пользователь {user.fio} отправил заявку на технику: {requestTypeDdl.SelectedItem.Text}.<br/><br/>Комментарий: {requestCommentT.Text}";
            mail.MailSend(mailFrom, mailTo, "Заказ техники UEBase", body);
            M1.SetMessage = "Письмо о запросе техники отправлено.";
            MessageExtender.Show();

            requestCommentT.Text = string.Empty;
            requestUeDiv.Visible = false;
        }

        protected void MSQLData_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows==0)
            emptyGridLabel.Text = "Нет закрепленной техники";
        }

    }
}