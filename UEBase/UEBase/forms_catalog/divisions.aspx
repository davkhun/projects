﻿<%@ Page Title="" Language="C#" MasterPageFile="~/UE.Master" AutoEventWireup="true" CodeBehind="divisions.aspx.cs" Inherits="UEBase.forms_catalog.divisions" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <h3>Справочник подразделений</h3>
    <div class="row">
        <div class="col-md-2">
            <table class="table">
                <thead>
                    <tr>
                        <th>Код</th>
                        <th>Название</th>
                    </tr>
                </thead>
                <tr>
                    <td>
                        <asp:TextBox ID="TextBox1" runat="server" CssClass="form-control input-sm" Width="70" MaxLength="5"></asp:TextBox></td>
                    <td>
                        <asp:TextBox ID="TextBox2" runat="server" CssClass="form-control input-sm" Width="200" MaxLength="255"></asp:TextBox></td>
                    <td>
                        <asp:Button ID="Button1" runat="server" Text="Добавить" CssClass="btn btn-sm btn-primary" OnClick="Button1_Click" /></td>
                </tr>
            </table>
            <div class="form-group">
                <div class="input-group">


                    <span class="input-group-btn"></span>
                </div>
            </div>
        </div>
    </div>
    <asp:GridView ID="GridView1" runat="server" CssClass="table table-striped table-bordered" DataSourceID="MSQLData" GridLines="None" AutoGenerateColumns="false" DataKeyNames="id" Font-Size="12px" AllowSorting="true" Width="50%" OnRowDeleting="GridView1_RowDeleting" OnSelectedIndexChanging="GridView1_SelectedIndexChanging">
        <SelectedRowStyle BackColor="#FFFF99" />
        <Columns>
            <asp:CommandField ButtonType="Image" ShowDeleteButton="true" DeleteImageUrl="~/img/glyphicons_313_ax.png" ShowSelectButton="true" SelectImageUrl="~/img/glyphicons_198_ok.png">
                <FooterStyle Wrap="false" />
                <HeaderStyle Width="60px" Wrap="false" />
                <ItemStyle Wrap="false" />
            </asp:CommandField>
            <asp:BoundField DataField="code" HeaderText="Код" SortExpression="code" />
            <asp:BoundField DataField="description" HeaderText="Название" SortExpression="description" />
        </Columns>
    </asp:GridView>
    <asp:SqlDataSource ID="MSQLData" runat="server"
        ConnectionString="<%$ ConnectionStrings:MySQLString %>"
        ProviderName="<%$ ConnectionStrings:MySQLString.ProviderName %>"
        SelectCommand="SELECT code,description,id FROM divisions ORDER BY code ASC" />

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
