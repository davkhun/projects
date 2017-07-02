<%@ Page Title="" Language="C#" MasterPageFile="~/UE.Master" AutoEventWireup="true" CodeBehind="admin.aspx.cs" Inherits="UEBase.admin" %>

<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit, Version=4.1.60623.0, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <asp:Button ID="newUserBtn" runat="server" Text="Новый пользователь" CssClass="btn btn-xs btn-default" OnClick="newUserBtn_Click" />
    <div class="panel panel-primary" id="newUserPanel" runat="server" visible="false" style="width: 50%">
        <div class="panel-heading">
            <label class="panel-title">Новый пользователь</label>
        </div>
        <div class="panel-body">

            <table class="table">
                <tr>
                    <td style="padding-top: 15px; width: 15%;" nowrap>
                        <label class="control-label">Доменное имя</label></td>
                    <td>
                        <asp:TextBox ID="unameT" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                    <td>
                        <asp:Button ID="findByUnameBtn" runat="server" Text="Найти" CssClass="btn btn-sm btn-default" OnClick="findByUnameBtn_Click" />
                    </td>
                </tr>
                <tr>
                    <td style="padding-top: 15px; width: 15%;" nowrap>
                        <label class="control-label">ФИО</label></td>
                    <td colspan="2">
                        <asp:TextBox ID="fioT" runat="server" CssClass="form-control input-sm" ReadOnly="true"></asp:TextBox></td>
                </tr>
                <tr>
                    <td style="padding-top: 15px; width: 15%;" nowrap>
                        <label class="control-label">Офис (AD)</label></td>
                    <td colspan="2">
                        <asp:TextBox ID="officeT" runat="server" CssClass="form-control input-sm" ReadOnly="true"></asp:TextBox></td>
                </tr>
                <tr>
                    <td style="padding-top: 15px; width: 15%;" nowrap>
                        <label class="control-label">Подразделение</label></td>
                    <td colspan="2">
                        <asp:CheckBoxList ID="divisionsCbl" runat="server" CssClass="checkbox checkbox-inline"></asp:CheckBoxList></td>
                </tr>
                                <tr>
                    <td style="padding-top: 15px; width: 15%;" nowrap>
                        <label class="control-label">Администратор</label></td>
                    <td colspan="2">
                        <asp:CheckBox ID="adminCb" runat="server" AutoPostBack="True"/></td>
                </tr>
                <tr>
                    <td style="padding-top: 15px; width: 15%;" nowrap>
                        <label class="control-label">Главный администратор (все права)</label></td>
                    <td colspan="2">
                        <asp:CheckBox ID="superadminCb" runat="server" AutoPostBack="True" OnCheckedChanged="adminCb_CheckedChanged" /></td>
                </tr>
            </table>

        </div>
        <div class="panel-footer">
            <asp:Button ID="AddUserBtn" runat="server" Text="Добавить" CssClass="btn btn-sm btn-success" OnClick="AddUserBtn_Click" />
        </div>
    </div>

    <asp:GridView ID="GridView1" runat="server" GridLines="none" CssClass="table table-striped" AllowPaging="true" PageSize="20" Font-Size="12px" AutoGenerateColumns="false" DataSourceID="MSQLData" DataKeyNames="id" OnRowDataBound="GridView1_RowDataBound" OnRowUpdating="GridView1_RowUpdating" OnRowDeleting="GridView1_RowDeleting">
        <EditRowStyle BackColor="#FFFF99" />
        <Columns>
            <asp:CommandField ButtonType="Image" ShowDeleteButton="true" DeleteImageUrl="~/img/glyphicons_313_ax.png" ShowEditButton="true" EditImageUrl="~/img/glyphicons_150_edit.png" UpdateImageUrl="~/img/glyphicons_198_ok.png" CancelImageUrl="~/img/glyphicons_197_remove.png">
                <FooterStyle Wrap="false" />
                <HeaderStyle Width="60px" Wrap="false" />
                <ItemStyle Wrap="false" />
            </asp:CommandField>
            <asp:BoundField DataField="fio" HeaderText="ФИО" SortExpression="fio" ReadOnly="true" />
            <asp:BoundField DataField="office" HeaderText="Офис" SortExpression="office" ReadOnly="true" />
            <asp:TemplateField HeaderText="Подразделения">
                <ItemTemplate>
                    <asp:Label ID="divLbl" runat="server" Text='<%# Eval("divisions_id") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:Label ID="divLbl1" runat="server" Text='<%# Eval("divisions_id") %>' Visible="false"></asp:Label>
                    <asp:CheckBoxList ID="divCbl" runat="server" CssClass="checkbox checkbox-inline"></asp:CheckBoxList>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Админ">
                <ItemTemplate>
                    <asp:CheckBox ID="adminC" runat="server" Enabled="false" Checked='<%# Bind("admin") %>' />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="adminC" runat="server" Checked='<%# Eval("admin") %>' />
                </EditItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <asp:SqlDataSource ID="MSQLData" runat="server"
        ConnectionString="<%$ ConnectionStrings:MySQLString %>"
        ProviderName="<%$ ConnectionStrings:MySQLString.ProviderName %>"
        SelectCommand="SELECT id,uname,fio,divisions_id,office,admin FROM rights ORDER BY fio ASC" />


    <asp:Button ID="btnShowMessage" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="MessageExtender" runat="server" Enabled="true" PopupControlID="MessagePanel" TargetControlID="btnShowMessage" BackgroundCssClass="modalBackground" CancelControlID="btnCloseMessage"></asp:ModalPopupExtender>
    <asp:Panel ID="MessagePanel" runat="server">
        <asp:Button ID="btnCloseMessage" runat="server" Text="X" Style="display: none" />
        <dav:Message ID="M1" runat="server"></dav:Message>
    </asp:Panel>


    <asp:Button ID="btnShowQuestion" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="QuestionExtender" runat="server" Enabled="true" PopupControlID="QuestionPanel" TargetControlID="btnShowQuestion" BackgroundCssClass="modalBackground" CancelControlID="btnCloseQuestion"></asp:ModalPopupExtender>
    <asp:Panel ID="QuestionPanel" runat="server">
        <asp:Button ID="btnCloseQuestion" runat="server" Text="X" Style="display: none" />
        <dav:Question ID="Q1" runat="server"></dav:Question>
    </asp:Panel>
</asp:Content>
