<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="supdocs.aspx.cs" Inherits="UEBase.supdocs" MasterPageFile="~/UE.Master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <script>
        $(function () {
            $('#priceT').mask('00/00/0000', { placeholder: "__/__/____" });
        });
    </script>
    <h3>Накладные</h3>
    <asp:Button ID="addRecBtn" runat="server" Text="Новая накладная" CssClass="btn btn-primary btn-sm" OnClick="addRecBtn_Click" />

    <div class="row" runat="server" id="newRecDiv" visible="false">
        <div class="col-md-5">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <label class="panel-title">Добавление накладной</label>
                </div>
                <div class="panel-body">
                    <table class="table">
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">Поставщик</label></td>
                            <td>
                                <asp:DropDownList ID="supDdl" runat="server" CssClass="form-control input-sm"></asp:DropDownList></td>
                        </tr>
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">Номер документа</label></td>
                            <td>
                                <asp:TextBox ID="doc_numberT" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">Дата</label></td>
                            <td>
                                <asp:TextBox ID="dateT" runat="server" CssClass="form-control input-sm" onkeydown="return false;"></asp:TextBox>
                                <asp:CalendarExtender ID="dateT_CalendarExtender" runat="server" Enabled="True" TargetControlID="dateT" Format="dd.MM.yyyy">
                                </asp:CalendarExtender>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">Сумма</label></td>
                            <td>
                                <asp:TextBox ID="priceT" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td style="padding-top: 15px">
                                <label class="control-label">Накладная</label></td>
                            <td>
                                <asp:FileUpload ID="FileUpload1" runat="server" AllowMultiple="true" /></td>
                        </tr>
                    </table>
                </div>
                <div class="panel-footer">
                    <asp:Button ID="Button1" runat="server" Text="Добавить" CssClass="btn btn-default" OnClick="Button1_Click" />
                </div>
            </div>
        </div>
    </div>

    <asp:Button ID="filterBtn" runat="server" Text="Фильтр" CssClass="btn btn-info btn-sm" OnClick="filterBtn_Click" />
    <div class="row" runat="server" id="filterDiv" visible="false">
        <div class="col-md-5">
            <div class="panel panel-info">
                <div class="panel-heading">
                    <label class="panel-title">Фильтр</label>
                </div>
                <div class="panel-body">
                    <label>Выберите поставщика:</label><br />
                    <asp:DropDownList ID="supplierFilterDdl" runat="server" CssClass="form-control input-sm"></asp:DropDownList><br />
                    <label>Или даты документов:</label>
                    <table>
                        <tr>
                            <td>
                                <asp:TextBox ID="dateFilterT1" runat="server" CssClass="form-control input-sm" onkeydown="return false;"></asp:TextBox>
                                <asp:CalendarExtender ID="dateFilterT1_CalendarExtender" runat="server" Enabled="True" TargetControlID="dateFilterT1" Format="dd.MM.yyyy">
                                </asp:CalendarExtender>

                            </td>
                            <td>
                                <asp:TextBox ID="dateFilterT2" runat="server" CssClass="form-control input-sm" onkeydown="return false;"></asp:TextBox>
                                <asp:CalendarExtender ID="dateFilterT2_CalendarExtender" runat="server" Enabled="True" TargetControlID="dateFilterT2" Format="dd.MM.yyyy">
                                </asp:CalendarExtender>

                            </td>
                        </tr>
                    </table>
                    <label>Или введите часть или номер накладной:</label>
                    <asp:TextBox ID="docNumT" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                </div>
                <div class="panel-footer">
                    <asp:Button ID="applyFilterBtn" runat="server" Text="Применить" CssClass="btn btn-default" OnClick="applyFilterBtn_Click" /><asp:Button ID="clearFilterBtn" runat="server" Text="Очистить фильтр" CssClass="btn btn-default" OnClick="clearFilterBtn_Click" />
                </div>
            </div>
        </div>
    </div>
    <asp:GridView ID="GridView1" runat="server" CssClass="table table-striped table-bordered" DataSourceID="MSQLData" GridLines="None" AutoGenerateColumns="false" DataKeyNames="id" Font-Size="12px" AllowSorting="true" Width="70%" AllowPaging="true" PageSize="30" OnRowCommand="GridView1_RowCommand" OnPreRender="GridView1_PreRender" OnRowDeleting="GridView1_RowDeleting" OnRowUpdating="GridView1_RowUpdating" OnRowDataBound="GridView1_RowDataBound" OnRowCreated="GridView1_RowCreated">
        <PagerStyle CssClass="bs-pagination" />
        <EditRowStyle BackColor="#FFFF99" />
        <Columns>
            <asp:CommandField ButtonType="Image" ShowDeleteButton="true" DeleteImageUrl="~/img/glyphicons_313_ax.png" ShowEditButton="true" EditImageUrl="~/img/glyphicons_150_edit.png" UpdateImageUrl="~/img/glyphicons_198_ok.png" CancelImageUrl="~/img/glyphicons_197_remove.png">
                <FooterStyle Wrap="false" />
                <HeaderStyle Width="60px" Wrap="false" />
                <ItemStyle Wrap="false" />
            </asp:CommandField>
            <asp:TemplateField HeaderText="Поставщик" SortExpression="name">
                <ItemTemplate>
                    <asp:Label ID="nameL" runat="server" Text='<%# Eval("short_name") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Номер документа" SortExpression="doc_number">
                <ItemTemplate>
                    <asp:Label ID="docnumL" runat="server" Text=' <%# Eval("doc_number") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Дата" SortExpression="date">
                <ItemTemplate>
                    <%# Eval("date") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="dateTb" runat="server" CssClass="form-control input-sm" Text='<%# Bind("date") %>' onkeydown="return false;"></asp:TextBox>
                    <asp:CalendarExtender ID="dateT_CalendarExtender" runat="server" Enabled="True" TargetControlID="dateTb" Format="dd.MM.yyyy">
                    </asp:CalendarExtender>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Стоимость" SortExpression="price">
                <ItemTemplate>
                    <%# $"{Eval("price"):C}" %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="priceTb" runat="server" CssClass="form-control input-sm" Text='<%# Bind("price") %>'></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Накладная">
                <ItemTemplate>

                    <asp:LinkButton ID="downloadL" runat="server" CommandName="downloadSup" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>">Скачать</asp:LinkButton>
                    <asp:Label ID="downloadPath" runat="server" Text='<%# Eval("scan_path") %>' Visible="false"></asp:Label>
                    <asp:LinkButton ID="showL" runat="server" CommandName="ShowSup" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>">Показать</asp:LinkButton>
                    <asp:Image ID="Image1" runat="server" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:Label ID="downloadPath" runat="server" Text='<%# Eval("scan_path") %>' Visible="false"></asp:Label>
                    <asp:FileUpload ID="uploadDocFU" runat="server" AllowMultiple="true" />
                    <asp:Button ID="uploadDocBtn" runat="server" Text="Загрузить" CssClass="btn btn-sm btn-success" CommandName="uploadSup" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" />
                </EditItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:SqlDataSource ID="MSQLData" runat="server"
        ConnectionString="<%$ ConnectionStrings:MySQLString %>"
        ProviderName="<%$ ConnectionStrings:MySQLString.ProviderName %>" />

    <asp:Button ID="btnShowPan" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="ShowPanExtender" runat="server" Enabled="true" PopupControlID="ShowPanel" TargetControlID="btnShowPan" BackgroundCssClass="modalBackground" CancelControlID="btnCloseShowPan"></asp:ModalPopupExtender>
    <asp:Panel ID="ShowPanel" runat="server">
        <div class="modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <asp:LinkButton ID="btnCloseShowPan" runat="server" Text="x" CssClass="close" />
                    <h4 class="modal-title">Накладная</h4>
                </div>
                <div class="modal-body">
                    <asp:Image ID="Image1" runat="server" />
                </div>
            </div>
        </div>
    </asp:Panel>


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
