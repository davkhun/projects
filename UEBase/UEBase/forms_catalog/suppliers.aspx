<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="suppliers.aspx.cs" Inherits="UEBase.suppliers" MasterPageFile="~/UE.Master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>



<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <h3>Поставщики</h3>
    <asp:Button ID="addRecBtn" runat="server" Text="Добавить поставщика" CssClass="btn btn-primary btn-sm" OnClick="addRecBtn_Click" />

    <div class="row" runat="server" id="newRecDiv" visible="false">
        <div class="col-md-3">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <label class="panel-title">Добавление поставщика</label>
                </div>
                <div class="panel-body">
                    <table class="table">
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">Наименование</label></td>
                            <td>
                                <asp:TextBox ID="nameT" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">Краткое наименование</label></td>
                            <td>
                                <asp:TextBox ID="shortNameT" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">ИНН</label></td>
                            <td>
                                <asp:TextBox ID="innT" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">Адрес</label></td>
                            <td>
                                <asp:TextBox ID="addressT" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">Сайт</label></td>
                            <td>
                                <asp:TextBox ID="siteT" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">Менеджер</label></td>
                            <td>
                                <asp:TextBox ID="managerT" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">Телефон</label></td>
                            <td>
                                <asp:TextBox ID="phoneT" runat="server" CssClass="form-control input-sm" TextMode="MultiLine"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">Email</label></td>
                            <td>
                                <asp:TextBox ID="emailT" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                        </tr>
                    </table>
                </div>
                <div class="panel-footer">
                    <asp:Button ID="Button1" runat="server" Text="Добавить" CssClass="btn btn-default" OnClick="Button1_Click" />
                </div>
            </div>
        </div>
    </div>
    <br />

    <asp:GridView ID="GridView1" runat="server" CssClass="table table-striped table-bordered" DataSourceID="MSQLData" GridLines="None" AutoGenerateColumns="false" DataKeyNames="id" Font-Size="12px" AllowSorting="true" Width="70%" OnRowDeleting="GridView1_RowDeleting" OnPreRender="GridView1_PreRender" OnRowUpdating="GridView1_RowUpdating">
        <EditRowStyle BackColor="#FFFF99" />
        <Columns>
            <asp:CommandField ButtonType="Image" ShowDeleteButton="true" DeleteImageUrl="~/img/glyphicons_313_ax.png" ShowEditButton="true" EditImageUrl="~/img/glyphicons_150_edit.png" UpdateImageUrl="~/img/glyphicons_198_ok.png" CancelImageUrl="~/img/glyphicons_197_remove.png">
                <FooterStyle Wrap="false" />
                <HeaderStyle Width="60px" Wrap="false" />
                <ItemStyle Wrap="false" />
            </asp:CommandField>
            <asp:TemplateField HeaderText="Наименование" SortExpression="name">
                <ItemTemplate>
                    <asp:Label ID="nameL" runat="server" Text='<%# Eval("name") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Краткое наименование" SortExpression="short_name">
                <ItemTemplate>
                    <asp:Label ID="shortNameL" runat="server" Text='<%# Eval("short_name") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="shortNameTb" runat="server" CssClass="form-control input-sm" Text='<%# Bind("short_name") %>'></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="ИНН" SortExpression="inn">
                <ItemTemplate>
                    <%# Eval("inn") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="innTb" runat="server" CssClass="form-control input-sm" Text='<%# Bind("inn") %>' MaxLength="12"></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Адрес" SortExpression="addresss">
                <ItemTemplate>
                    <%# Eval("address") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="addressTb" runat="server" CssClass="form-control input-sm" Text='<%# Bind("address") %>'></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Сайт" SortExpression="site">
                <ItemTemplate>
                    <%# Eval("site") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="siteTb" runat="server" CssClass="form-control input-sm" Text='<%# Bind("site") %>'></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Менеджер" SortExpression="manager">
                <ItemTemplate>
                    <%# Eval("manager") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="managerTb" runat="server" CssClass="form-control input-sm" Text='<%# Bind("manager") %>'></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Телефон" SortExpression="phone">
                <ItemTemplate>
                    <%# Eval("phone") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="phoneTb" runat="server" CssClass="form-control input-sm" Text='<%# Bind("phone") %>'></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Email" SortExpression="email">
                <ItemTemplate>
                    <%# Eval("email") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="emailTb" runat="server" CssClass="form-control input-sm" Text='<%# Bind("email") %>'></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:SqlDataSource ID="MSQLData" runat="server"
        ConnectionString="<%$ ConnectionStrings:MySQLString %>"
        ProviderName="<%$ ConnectionStrings:MySQLString.ProviderName %>"
        SelectCommand="SELECT id,name,short_name,inn,address,site,manager,phone,email FROM suppliers ORDER BY id DESC" />

    <asp:Button ID="btnShowQuestion" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="QuestionExtender" runat="server" Enabled="true" PopupControlID="QuestionPanel" TargetControlID="btnShowQuestion" BackgroundCssClass="modalBackground" CancelControlID="btnCloseQuestion"></asp:ModalPopupExtender>
    <asp:Panel ID="QuestionPanel" runat="server">
        <asp:Button ID="btnCloseQuestion" runat="server" Text="X" Style="display: none" />
        <dav:Question ID="Q1" runat="server"></dav:Question>
    </asp:Panel>

    <asp:Button ID="btnShowMessage" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="MessageExtender" runat="server" Enabled="true" PopupControlID="MessagePanel" TargetControlID="btnShowMessage" BackgroundCssClass="modalBackground" CancelControlID="btnCloseMessage"></asp:ModalPopupExtender>
    <asp:Panel ID="MessagePanel" runat="server">
        <asp:Button ID="btnCloseMessage" runat="server" Text="X" Style="display: none" />
        <dav:Message ID="M1" runat="server"></dav:Message>
    </asp:Panel>
</asp:Content>
