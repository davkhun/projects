<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="UEBase._default" MasterPageFile="~/UE.Master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <asp:Button ID="addUEBtn" runat="server" Text="Добавить учетную единицу" CssClass="btn btn-primary btn-sm" OnClick="addUEBtn_Click" />

    <div class="row" runat="server" id="newUEDiv" visible="false">
        <div class="col-md-4">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <label class="panel-title">Добавление учетной единицы</label>
                </div>
                <div class="panel-body">
                    <table class="table">
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">Инвентарный номер</label></td>
                            <td>
                                <asp:TextBox ID="invNumT" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">Серийный номер</label></td>
                            <td>
                                <asp:TextBox ID="serNumT" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">Инвентаризирован</label></td>
                            <td>
                                <asp:CheckBox ID="isInvC" runat="server" /></td>
                        </tr>
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">Тип</label></td>
                            <td>
                                <asp:DropDownList ID="typeDdl" runat="server" CssClass="form-control input-sm" AutoPostBack="True" OnSelectedIndexChanged="typeDdl_SelectedIndexChanged"></asp:DropDownList><a href="forms_catalog/uetype.aspx" runat="server" id="addNewTypeA" style="font-size: 10px">Добавить новый</a></td>

                        </tr>
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">Модель</label></td>
                            <td>
                                <asp:DropDownList ID="modelsDdl" runat="server" CssClass="form-control input-sm"></asp:DropDownList><a href="forms_catalog/models.aspx" runat="server" id="addNewModelA" style="font-size: 10px">Добавить новую</a>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">Статус</label></td>
                            <td>
                                <asp:DropDownList ID="statusDdl" runat="server" CssClass="form-control input-sm"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">Сотрудник</label></td>
                            <td>
                                <asp:TextBox ID="managerT" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                            <td>
                                <asp:Button ID="searchManagerB" runat="server" Text="Найти" CssClass="btn btn-info btn-sm" OnClick="searchManagerB_Click" />
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">Подразделение</label></td>
                            <td>
                                <asp:TextBox ID="subdivisionT" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">Подразделение (Де Юре)</label></td>
                            <td>
                                <asp:DropDownList ID="subdivisionDdl" runat="server" CssClass="form-control input-sm"></asp:DropDownList></td>
                        </tr>
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">Офис</label></td>
                            <td>
                                <asp:TextBox ID="officeT" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">Стоимость</label></td>
                            <td>
                                <asp:TextBox ID="priceT" runat="server" CssClass="form-control input-sm" data-mask="99 999 999.99"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">Накладная</label></td>
                            <td>
                                <asp:TextBox ID="supDocT" runat="server" CssClass="form-control input-sm" placeholder="Поставщик, цена, дата или номер документа..."></asp:TextBox>
                                <asp:AutoCompleteExtender
                                    ServiceMethod="SearchDoc"
                                    MinimumPrefixLength="2"
                                    CompletionInterval="10"
                                    EnableCaching="false"
                                    CompletionSetCount="10"
                                    TargetControlID="supDocT"
                                    ID="supDocExtender"
                                    runat="server"
                                    FirstRowSelected="false"
                                    UseContextKey="True"
                                    BehaviorID="AutoCompleteEx"
                                    CompletionListCssClass="modal-content modal-lg modal-body">
                                </asp:AutoCompleteExtender>
                            </td>

                        </tr>
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">Комментарий</label></td>
                            <td>
                                <asp:TextBox ID="commentT" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>

                        </tr>
                    </table>
                </div>
                <div class="panel-footer">
                    <asp:Button ID="Button1" runat="server" Text="Добавить" CssClass="btn btn-success btn-sm" OnClick="Button1_Click" />
                    <asp:Button ID="Button2" runat="server" Text="Дублировать" CssClass="btn btn-default btn-sm" OnClick="Button2_Click" />
                </div>
            </div>
        </div>
    </div>

    <asp:Button ID="showFilter" runat="server" Text="Фильтр" CssClass="btn btn-info btn-sm" OnClick="showFilter_Click" />

    <div class="row" runat="server" id="filterDiv" visible="false">
        <div class="col-md-4">
            <div class="panel panel-info">
                <div class="panel-heading">
                    <label class="panel-title">Фильтр</label>
                </div>
                <div class="panel-body">
                    <table class="table">
                        <thead>
                            <tr>
                                <th>Тип</th>
                                <th>Модель</th>
                                <th>Подразделение</th>
                            </tr>
                        </thead>
                        <tr>
                            <td>
                                <asp:DropDownList ID="typeDl" runat="server" CssClass="input-sm form-control" AutoPostBack="True" OnSelectedIndexChanged="typeDl_SelectedIndexChanged"></asp:DropDownList></td>
                            <td>
                                <asp:DropDownList ID="modelDl" runat="server" CssClass="input-sm form-control"></asp:DropDownList></td>
                            <td>
                                <asp:DropDownList ID="divisionDl" runat="server" CssClass="input-sm form-control"></asp:DropDownList></td>
                        </tr>
                    </table>
                    <asp:TextBox ID="filterTb" runat="server" CssClass="input-sm form-control" placeholder="Или введите строку для поиска"></asp:TextBox>
                    <asp:AutoCompleteExtender
                        ServiceMethod="SearchBase"
                        MinimumPrefixLength="2"
                        CompletionInterval="100"
                        EnableCaching="false"
                        CompletionSetCount="10"
                        TargetControlID="filterTb"
                        ID="AutoCompleteTable"
                        runat="server"
                        FirstRowSelected="false"
                        CompletionListCssClass="modal-content modal-body">
                    </asp:AutoCompleteExtender>
                </div>
                <div class="panel-footer">
                    <asp:Button ID="applyFilterBtn" runat="server" Text="Применить" CssClass="btn btn-primary btn-sm" OnClick="applyFilterBtn_Click" />
                </div>
            </div>
        </div>
    </div>
    <asp:Button ID="requestUeBtn" runat="server" Text="Заказать технику" CssClass="btn btn-default" OnClick="requestUeBtn_Click" />
    <div class="row" runat="server" id="requestUeDiv" visible="false">
        <div class="col-md-4">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <label class="panel-title">Оформление заказа</label>
                </div>
                <div class="panel-body">
                    <table class="table">
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">Тип</label></td>
                            <td>
                                <asp:DropDownList ID="requestTypeDdl" runat="server" CssClass="form-control"></asp:DropDownList></td>
                        </tr>
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">Комментарий</label></td>
                            <td>
                                <asp:TextBox ID="requestCommentT" runat="server" CssClass="form-control input-sm" TextMode="Multiline" Height="250"></asp:TextBox></td>
                        </tr>
                    </table>
                </div>
                <div class="panel-footer">
                    <asp:Button ID="sendRequestBtn" runat="server" Text="Отправить заказ" CssClass="btn btn-primary" OnClick="sendRequestBtn_Click" />
                </div>
            </div>
        </div>
    </div>
    <asp:Label ID="emptyGridLabel" runat="server" Text="" CssClass="label label-info"></asp:Label>
    <asp:GridView ID="GridView1" runat="server" CssClass="table table-striped table-bordered" DataSourceID="MSQLData" GridLines="None" AutoGenerateColumns="false" DataKeyNames="id" Font-Size="12px" AllowSorting="true" AllowPaging="true" PageSize="40" OnRowDataBound="GridView1_RowDataBound" OnRowCommand="GridView1_RowCommand" OnRowUpdating="GridView1_RowUpdating" OnPreRender="GridView1_PreRender">
        <PagerStyle CssClass="bs-pagination" />
        <SelectedRowStyle BackColor="#FFFF99" />
        <EditRowStyle BackColor="#FFFF99" />
        <Columns>
            <asp:CommandField ButtonType="Image" ShowEditButton="true" EditImageUrl="~/img/glyphicons_150_edit.png" UpdateImageUrl="~/img/glyphicons_198_ok.png" CancelImageUrl="~/img/glyphicons_197_remove.png">
                <FooterStyle Wrap="false" />
                <HeaderStyle Width="60px" Wrap="false" />
                <ItemStyle Wrap="false" />
            </asp:CommandField>
            <asp:TemplateField HeaderText="Инв. №" SortExpression="inv_number">
                <ItemTemplate>
                    <asp:Label ID="invNumL" runat="server" Text='<%# Eval("inv_number") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="И." SortExpression="is_inv">
                <ItemTemplate>
                    <asp:CheckBox ID="isInvC" runat="server" Checked='<%# Eval("is_inv") %>' Enabled="false" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="isInvC" runat="server" Checked='<%# Bind("is_inv") %>' />
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Тип, модель" SortExpression="type">
                <ItemTemplate>
                    <asp:Label ID="typeL" runat="server" Text='<%# Eval("type") %>'></asp:Label><br />
                    <asp:Label ID="modelL" runat="server" Text='<%# Eval("model") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Статус" SortExpression="value">
                <ItemTemplate>
                    <asp:Label ID="statusL" runat="server" Text='<%# Eval("value") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="statusDdl" runat="server" CssClass="form-control input-sm" Width="150px"></asp:DropDownList>
                    <asp:Label ID="statusL" runat="server" Text='<%# Eval("value") %>' Visible="false"></asp:Label>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Владелец" SortExpression="manager">
                <ItemTemplate>
                    <asp:HyperLink ID="managerHl" runat="server" Target="_blank" NavigateUrl='<%# string.Format("https://staff.skbkontur.ru/profile/{0}",Eval("manager_uname")) %>' Visible='<%# Eval("manager_uname").ToString().Length>0 %>'><%# Eval("manager") %></asp:HyperLink>
                    <asp:Label ID="managerL" runat="server" Text='<%# Eval("manager") %>' Visible='<%# Eval("manager_uname").ToString().Length==0 %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <div class="form-group">
                        <div class="input-group">
                            <asp:TextBox ID="managerTb" runat="server" Text='<%# Bind("manager") %>' CssClass="form-control input-sm"></asp:TextBox>
                            <span class="input-group-btn">
                                <asp:Button ID="findManagerADbtn" runat="server" Text="Найти" CssClass="btn btn-sm btn-info" CommandName="FindAD" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" />
                            </span>
                        </div>
                    </div>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Подразделение факт." SortExpression="location">
                <ItemTemplate>
                    <asp:Label ID="divisionFactL" runat="server" Text='<%# string.Format("{0}<br/>{1}",Eval("office"),Eval("manager_placement")) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Подразделение юр." SortExpression="code">
                <ItemTemplate>
                    <asp:Label ID="divisionUrL" runat="server" Text='<%# Eval("code") + " - " + Eval("description") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Стоимость" SortExpression="ue_price">
                <ItemTemplate>
                    <asp:Label ID="priceL" runat="server" Text='<%# string.Format("{0:C}",Eval("ue_price")) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Накладная" SortExpression="doc_number">
                <ItemTemplate>
                    <asp:Label ID="docNumberL" runat="server" Text='<%# Eval("doc_number") + " (" + Eval("name") + ")" %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Комментарий" SortExpression="comment">
                <ItemTemplate>
                    <asp:Label ID="commentL" runat="server" Text='<%# Eval("comment") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="commentTb" runat="server" Text='<%# Bind("comment") %>' CssClass="form-control input-sm" TextMode="MultiLine"></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <EditItemTemplate>
                    <asp:Button ID="duplicateBtn" runat="server" Text="Дублировать" CssClass="btn btn-xs btn-info" CommandName="DuplicateUE" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" />
                </EditItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:SqlDataSource ID="MSQLData" runat="server"
        ConnectionString="<%$ ConnectionStrings:MySQLString %>"
        ProviderName="<%$ ConnectionStrings:MySQLString.ProviderName %>" OnSelected="MSQLData_Selected" />


    <asp:Button ID="btnShowUserPanel" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="UserPanelExtender" runat="server" Enabled="true" PopupControlID="UserPanel" TargetControlID="btnShowUserPanel" BackgroundCssClass="modalBackground" CancelControlID="btnCloseUserPanel"></asp:ModalPopupExtender>
    <asp:Panel ID="UserPanel" runat="server">
        <asp:Button ID="btnCloseUserPanel" runat="server" Text="X" Style="display: none" />
        <div class="modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <asp:LinkButton ID="btnClose" runat="server" Text="x" CssClass="close" />
                    <h4 class="modal-title">Найденные сотрудники</h4>
                </div>
                <div class="modal-body">
                    <asp:GridView ID="UserGrid" runat="server" CssClass="table table-stripped" GridLines="None" AutoGenerateColumns="true" AllowPaging="true" PageSize="10" Font-Size="12px" OnSelectedIndexChanging="UserGrid_SelectedIndexChanging">
                        <Columns>
                            <asp:CommandField ButtonType="Image" ShowSelectButton="true" SelectImageUrl="~/img/glyphicons_198_ok.png">
                                <FooterStyle Wrap="false" />
                                <HeaderStyle Width="60px" Wrap="false" />
                                <ItemStyle Wrap="false" />
                            </asp:CommandField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </asp:Panel>

    <asp:Button ID="btnShowMessage" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="MessageExtender" runat="server" Enabled="true" PopupControlID="MessagePanel" TargetControlID="btnShowMessage" BackgroundCssClass="modalBackground" CancelControlID="btnCloseMessage"></asp:ModalPopupExtender>
    <asp:Panel ID="MessagePanel" runat="server">
        <asp:Button ID="btnCloseMessage" runat="server" Text="X" Style="display: none" />
        <dav:Message ID="M1" runat="server"></dav:Message>
    </asp:Panel>
</asp:Content>
