<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TView.Default" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Tender View</title>

    <meta content="text/html; charset=utf-8" http-equiv="Content-type" />
    <meta content="IE=edge" http-equiv="X-UA-Compatible" />

    <script type="text/javascript" src="/script/jquery-1.8.3.js"></script>
    <script type="text/javascript" src="/script/jquery-ui.js"></script>
    <script type="text/javascript" src="/Calendar/Scripts/calendar.min.js"></script>
    <script type="text/javascript" src="/script/jquery.disable.text.select.js"></script>
    <script type="text/javascript" src="/script/jquery.easing.js"></script>
    <script type="text/javascript" src="/script/jquery.mousewheel.js"></script>
    <script type="text/javascript" src="/script/tview.js"></script>

    <link href="/Calendar/Styles/Calendar.css" rel="stylesheet" type="text/css" />
    <link href="/css/jquery-ui.css" rel="stylesheet" type="text/css" />
    <link href="/css/tview.css" rel="stylesheet" type="text/css" />
    <link href="/css/ke.css" rel="stylesheet" type="text/css" />
    <link href="/css/table.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript">
        function SelectFile() {
            var fu = document.getElementById('FileUpload1');
            fu.click();
        }

        // загрузка страницы
        var DDE;
        var DDE1;
        function pageLoad() {

            var calendar = $.calendar({
                field: "input",
                minYear: 1940,
                maxYear: 2020,
                language: "RU",
                format: "dd.mm.yyyy",
                offsetX: -5,
                offsetY: 10
            });
            calendar.addCalendarField($(".showCalendar"));

            DDE = $find('OKDPdde');
            if (DDE._dropDownControl) {
                $common.removeHandlers(DDE._dropDownControl, DDE._dropDownControl$delegates);
            }
            DDE._dropDownControl$delegates = {
                click: Function.createDelegate(DDE, ShowMe),
                contextmenu: Function.createDelegate(DDE, DDE._dropDownControl_oncontextmenu)
            }
            $addHandlers(DDE._dropDownControl, DDE._dropDownControl$delegates);

            DDE1 = $find('TextBox6_DropDownExtender');
            if (DDE1._dropDownControl) {
                $common.removeHandlers(DDE1._dropDownControl, DDE1._dropDownControl$delegates);
            }

            DDE1._dropDownControl$delegates = {
                click: Function.createDelegate(DDE1, ShowMe1),
                contextmenu: Function.createDelegate(DDE1, DDE1._dropDownControl_oncontextmenu)
            }
            $addHandlers(DDE1._dropDownControl, DDE1._dropDownControl$delegates);
        }

        function ShowMe() {
            DDE._wasClicked = true;
        }

        function ShowMe1() {
            DDE1._wasClicked = true;
        }
        // документ загружен
        $(document).ready(
            DocReady());
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server" ID="ScriptManagerMain"></asp:ScriptManager>
        <div class="wrapperOuter">
            <div class="wrapper">
                <div class="header">
                </div>
                <div class="middle" style="min-width: 1120px;">
                    <asp:UpdatePanel ID="UpdatePanelMain" runat="server">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="Button3" EventName="Click" />
                            <asp:PostBackTrigger ControlID="ExportXLSButton" />
                            <asp:AsyncPostBackTrigger ControlID="AddOKDPButton" EventName="Click" />
                            <asp:PostBackTrigger ControlID="LoadFileButton" />
                            <asp:PostBackTrigger ControlID="SearchTab" />
                            <asp:PostBackTrigger ControlID="SettingsTab" />
                            <asp:PostBackTrigger ControlID="DeleteOptionsBtn" />
                            <asp:PostBackTrigger ControlID="DeleteMailsBtn" />
                        </Triggers>
                        <ContentTemplate>
                            <div class="navigation">
                                <div class="navigationLevel2">
                                    <div class="navigationLevel2_item navigationLevel2_item__active" id="divSearch" runat="server">
                                        <span class="navigationLevel2_pointer"></span>
                                        <asp:LinkButton ID="SearchTab" runat="server" CssClass="navigationLevel2_link" OnClick="SearchTab_Click">Поиск</asp:LinkButton>
                                    </div>
                                    <div class="navigationLevel2_item" id="divSettings" runat="server">
                                        <span class="navigationLevel2_pointer"></span>
                                        <asp:LinkButton ID="SettingsTab" runat="server" CssClass="navigationLevel2_link" OnClick="SettingsTab_Click">Настройка</asp:LinkButton>
                                    </div>
                                    <div class="navigationLevel2_item" id="divB2B" runat="server">
                                        <span class="navigationLevel2_pointer"></span>
                                        <asp:LinkButton ID="TabB2B" runat="server" CssClass="navigationLevel2_link" OnClick="TabB2B_Click" Visible="false">B2B</asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                            <div class="content">
                                <asp:MultiView ID="TablesView" runat="server" ActiveViewIndex="0">
                                    <asp:View ID="MainView" runat="server">
                                        <p class="headingTitle">Условия поиска</p>
                                        <hr />
                                        <div class="g-24 safeContext">
                                            <!-- первый столбец -->
                                            <div class="safeContext">
                                                <div class="g-col-1 g-span-4">
                                                    <asp:DropDownList ID="DropDownList2" runat="server"
                                                        OnSelectedIndexChanged="DropDownList2_SelectedIndexChanged"
                                                        CssClass="button button__type button__type__dropdown" AutoPostBack="True">
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="g-col-5 g-span-2">

                                                    <asp:Button ID="Button6" runat="server" Text="Удалить" OnClick="Button6_Click" CssClass="button" Style="width: 70%; display: none" />
                                                </div>
                                            </div>
                                        </div>
                                        <hr />
                                        <div class="g-24 safeContext">
                                            <!-- Основной поиск первая строка -->
                                            <div class="safeContext">
                                                <div class="g-col-1 g-span-4">
                                                    Регион
                                                </div>
                                                <div class="g-col-4 g-span-3">
                                                    <asp:Label ID="RegionLbl" runat="server" CssClass="field field__px150 field_unhover" Text="Все"></asp:Label>
                                                    <asp:TextBox ID="TextBox6" runat="server" ReadOnly="true" Style="cursor: default" Visible="false"></asp:TextBox>
                                                    <asp:Panel ID="Panel2" runat="server" BackColor="#CCCCCC" BorderStyle="Solid" BorderColor="Black" BorderWidth="1">
                                                        <div id="cblLayer" style="position: relative; width: 220px; height: 200px; overflow: scroll; background-color: white; top: 0px; left: 0px;">
                                                            <asp:CheckBoxList ID="CheckBoxList1" runat="server" CssClass="checkboxList" RepeatLayout="UnorderedList" AutoPostBack="False" />
                                                        </div>
                                                        <asp:Button ID="Button8" runat="server" Text="Применить" OnClick="Button8_Click" /><asp:Button ID="Button9" runat="server" Text="Очистить список" CssClass="btn btn-warning btn-mini" OnClick="Button9_Click" />
                                                    </asp:Panel>
                                                    <ajaxToolkit:DropDownExtender ID="TextBox6_DropDownExtender" runat="server" DynamicServicePath="" Enabled="True" TargetControlID="RegionLbl" DropDownControlID="Panel2">
                                                    </ajaxToolkit:DropDownExtender>
                                                </div>
                                                <div class="g-col-8 g-span-3">
                                                    <span class="field field__turbo field__size12 field__unhover" style="margin-left: 20px">
                                                        <asp:TextBox ID="TextBox1" runat="server" MaxLength="12" onkeypress="return textBoxKeyPress()" /></span>
                                                    <ajaxToolkit:AutoCompleteExtender ServiceMethod="SearchOKDP"
                                                        MinimumPrefixLength="2"
                                                        CompletionInterval="100"
                                                        EnableCaching="false"
                                                        CompletionSetCount="10"
                                                        TargetControlID="TextBox1"
                                                        ID="AutoCompleteOKDP"
                                                        runat="server"
                                                        FirstRowSelected="false"
                                                        UseContextKey="True"
                                                        BehaviorID="AutoCompleteEx"
                                                        OnClientPopulated="onListPopulated"
                                                        OnClientItemSelected="subStringTextBox"
                                                        CompletionListCssClass="wrapper middle content" />
                                                </div>
                                                <div class="g-col-13 g-span-3">
                                                    <asp:Button ID="AddOKDPButton" runat="server" Text="Добавить ОКДП" OnClick="AddOKDPButton_Click" CssClass="button" />
                                                </div>
                                                <div class="g-col-16 g-span-3">
                                                    Макс. цена
                                                </div>
                                                <div class="g-col-18 g-span-4">
                                                    <span class="field field__px100 field_unhover">
                                                        <asp:TextBox ID="MaxPriceTB" runat="server" MaxLength="100" /></span>

                                                </div>
                                            </div>
                                            <!-- вторая строка -->
                                            <div class="safeContext">
                                                <div class="g-col-1 g-span-3">
                                                    <span>Название заказа</span>
                                                </div>
                                                <div class="g-col-4 g-span-3">
                                                    <span class="field field__px150 field_unhover">
                                                        <asp:TextBox ID="TextBox3" runat="server" MaxLength="100" /></span>
                                                </div>
                                                <div class="g-col-8 g-span-3">
                                                    <span class="field field__size12 field_unhover" id="OKDPspan" runat="server" style="margin-left: 20px">Список ОКДП</span>

                                                    <asp:Panel ID="OKDPPanel" runat="server" BackColor="#CCCCCC" BorderStyle="Solid" BorderColor="Black" BorderWidth="1">
                                                        <div id="OKDPdiv" style="position: relative; width: 220px; height: 200px; overflow: scroll; background-color: white; top: 0px; left: 0px;">
                                                            <asp:CheckBoxList ID="OKDPcbl" runat="server" CssClass="checkboxList" RepeatLayout="UnorderedList" />
                                                        </div>
                                                    </asp:Panel>
                                                    <ajaxToolkit:DropDownExtender ID="OKDPdde" runat="server" DynamicServicePath="" Enabled="True" TargetControlID="OKDPspan" DropDownControlID="OKDPPanel">
                                                    </ajaxToolkit:DropDownExtender>
                                                </div>
                                                <div class="g-col-13 g-span-3">
                                                    <asp:Button ID="ClearOKDPbtn" runat="server" Text="Очистить список" OnClick="ClearOKDPbtn_Click" CssClass="button" />
                                                </div>
                                                <div class="g-col-16 g-span-3">
                                                    Площадка/ФЗ
                                                </div>
                                                <div class="g-col-18 g-span-4">
                                                    <asp:DropDownList ID="FZTypeDDL" runat="server" CssClass="button button__type button__type__dropdown" Width="80px">
                                                        <asp:ListItem>Все</asp:ListItem>
                                                        <asp:ListItem>94ФЗ</asp:ListItem>
                                                        <asp:ListItem>223ФЗ</asp:ListItem>
                                                        <asp:ListItem>B2B</asp:ListItem>
                                                        <asp:ListItem>44ФЗ</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <!-- третья строка  -->
                                            <div class="safeContext">
                                                <div class="g-col-1 g-span-8">
                                                    <asp:CheckBox ID="InDocCB" runat="server" Text="Искать в документах заказа" CssClass="checkboxList_item" />
                                                </div>
                                                <div class="g-col-8 g-span-8">
                                                    <asp:CheckBox ID="CheckBox2" runat="server" Text="Искать только с этими кодами" CssClass="checkboxList_item" Style="margin-left: 20px" />
                                                </div>
                                            </div>

                                            <!-- четвертая строка -->
                                            <div class="safeContext">
                                                <div class="g-col-1 g-span-3">
                                                    Дата публикации
                                                </div>
                                                <div class="g-col-4 g-span-3">
                                                    <span class="field field__px100 showCalendar">
                                                        <asp:TextBox ID="TextBox4" name="available_date" runat="server" />
                                                    </span>
                                                </div>
                                                <div class="g-col-8 g-span-8">
                                                    <a href="#" id="fUploadLnk" runat="server" class="linkFile" style="margin-left: 20px" onclick="SelectFile()">Загрузить список ОКДП...</a>
                                                    <asp:FileUpload ID="FileUpload1" runat="server" onchange="__doPostBack('LoadFileButton','')" Style="display: none" />
                                                    <asp:Button ID="LoadFileButton" runat="server" OnClick="Button4_Click" Text="Загрузить" CssClass="button" Style="display: none" />
                                                </div>
                                                <div class="g-col-14 g-span-4">
                                                </div>
                                            </div>
                                            <!-- пятая строка -->
                                            <div class="safeContext">
                                                <div class="g-col-1 g-span-3">
                                                </div>
                                                <div class="g-col-4 g-span-3">
                                                    <asp:CheckBox ID="CheckBox1" runat="server" Text="С этой даты" CssClass="checkboxList_item" />
                                                </div>

                                            </div>
                                        </div>

                                        <br />
                                        <!-- поиск (шестая строка) -->
                                        <div class="g-12 safeContext">
                                            <div class="safeContext">
                                                <div class="g-col-6 g-span-2">
                                                    <asp:Button ID="Button3" runat="server" Text="Искать" OnClick="Button3_Click" CssClass="button button__colored__purple" Style="color: white; width: 100px; height: 35px;" />
                                                </div>
                                                <div class="g-col-7 g-span-1">
                                                    <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                                                        <ProgressTemplate>
                                                            <asp:Image ID="waitIMG" runat="server" ImageUrl="/image/send.gif" Style="padding-left: 12px; padding-top: 4px;" />
                                                        </ProgressTemplate>
                                                    </asp:UpdateProgress>
                                                </div>
                                            </div>
                                        </div>
                                        <!-- новый поиск и сохранение условия -->
                                        <div class="g-24 safeContext">
                                            <div class="safeContext">
                                                <div class="g-col-1 g-span-4">
                                                    <asp:LinkButton ID="ClearFormLnkB" runat="server" CssClass="linkClear" OnClick="ClearFormBtn_Click">Очистить</asp:LinkButton>
                                                </div>
                                                <div class="g-col-3 g-span-6">
                                                    <asp:LinkButton ID="SaveVariantLnkB" runat="server" CssClass="linkSave" Visible="false">Сохранить условие</asp:LinkButton>
                                                    <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtender3" runat="server" Enabled="True" PopupControlID="SaveIshtPanel" TargetControlID="SaveVariantLnkB" CancelControlID="IshtCloseButton" BackgroundCssClass="modalBackground">
                                                    </ajaxToolkit:ModalPopupExtender>
                                                    <asp:Panel ID="SaveIshtPanel" runat="server" Style="display: none">
                                                        <div class="wrapper" style="max-width: 350px">
                                                            <div class="middle" style="max-width: 350px">
                                                                <div class="content">
                                                                    <div class="safeContext">
                                                                        <div class="g-col-1 g-span-2">
                                                                            <span class="field field__px150 field_unhover">
                                                                                <asp:TextBox ID="TextBox5" runat="server" MaxLength="128" placeholder="Имя условия поиска" /></span>
                                                                        </div>
                                                                        <div class="g-col-4 g-span-1">
                                                                            <asp:Button ID="Button5" runat="server" Text="Сохранить" OnClick="Button5_Click" CssClass="button" />
                                                                        </div>
                                                                    </div>
                                                                    <hr />
                                                                    <br />
                                                                    <asp:Button ID="IshtCloseButton" runat="server" Text="Закрыть" CssClass="button" OnClick="IshtCloseButton_Click" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </asp:Panel>
                                                </div>
                                            </div>
                                        </div>

                                        <hr />
                                        <div class="g-24 safeContext">
                                            <div class="safeContext">
                                                <div class="g-col-1 g-span-4">
                                                    <span class="button button__type button__type__excel" tabindex="6" id="ExpToExcel" runat="server" onclick="ExportToExcel()" visible="false">
                                                        <span class="button_content">Сохранить в Excel</span>
                                                    </span>
                                                </div>
                                                <div class="g-col-10 g-span-8">
                                                    <asp:Label ID="Label2" runat="server" Text="Label" Visible="false" ForeColor="Red" Font-Bold="true" />
                                                </div>
                                                <div class="g-col-5 g-span-4">
                                                    <span class="button button__type button__type__send" tabindex="6" id="CreateMailer" runat="server" onclick="CreateMailerF()" visible="false">
                                                        <span class="button_content">Создать рассылку</span>
                                                    </span>
                                                    <asp:Button ID="Button10" runat="server" Text="Создать рассылку" CssClass="button" OnClick="Button10_Click" Style="display: none" />
                                                </div>
                                            </div>
                                        </div>

                                        <br />
                                        <!-- таблицы тендеров -->

                                        <!-- управление таблицой -->
                                        <div class="g-24 safeContext">
                                            <div class="safeContext">
                                                <div class="g-col-18 g-span-5" style="text-align: right; padding: 3px 5px 0px 0px;">
                                                    <asp:Label ID="ViewOnPageLbl" runat="server" Text="Отображать на странице:" Visible="false" />
                                                </div>
                                                <div class="g-col-23 g-span-2">
                                                    <asp:DropDownList ID="ItemsOnPageDDL" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ItemsOnPageDDL_SelectedIndexChanged" Width="60px" CssClass="button button__type button__type__dropdown" Visible="false">
                                                        <asp:ListItem>8</asp:ListItem>
                                                        <asp:ListItem>10</asp:ListItem>
                                                        <asp:ListItem>15</asp:ListItem>
                                                        <asp:ListItem>20</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                        <!-- таблицы -->
                                        <asp:Repeater ID="ZGrid" runat="server" OnItemCommand="ZGrid_ItemCommand" OnItemDataBound="ZGrid_ItemDataBound">
                                            <HeaderTemplate>
                                                <table class="table table-hover" style="font-size: 12px; width: 100%">
                                                    <thead>
                                                        <tr>
                                                            <th><b>Инфо</b></th>
                                                            <th><b>Площадка</b></th>
                                                            <th style="display: none">ID</th>
                                                            <th><b>Номер извещения</b></th>
                                                            <th><b>Тип</b></th>
                                                            <th><b>Заказ</b></th>
                                                            <th style="width: 90px;"><b>
                                                                <asp:LinkButton ID="MaxPricelb" runat="server" Text="Макс. цена" OnClick="MaxPricelb_Click" /></b></th>
                                                            <th><b>Разместил</b></th>
                                                            <th style="width: 110px;"><b>
                                                                <asp:LinkButton ID="PublishDatelb" runat="server" Text="Дата размещения" OnClick="PublishDatelb_Click" /></b></th>
                                                            <th><b>
                                                                <asp:LinkButton ID="Regionlb" runat="server" Text="Регион" OnClick="Regionlb_Click" /></b></th>
                                                        </tr>
                                                    </thead>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <tr>
                                                    <td>
                                                        <asp:Button ID="InfoBtn" runat="server" CssClass="button" Text="Инфо" Width="50px" Height="30px" />
                                                    </td>
                                                    <td style="display: none">
                                                        <asp:Label ID="IDlbl" runat="server" Text='<%# Eval("ID") %>' Visible="false" />
                                                    </td>
                                                    <td><%# Eval("FZtype") %></a></td>
                                                    <td><%# GetHTMLlink(Eval("FZtype").ToString(),Eval("HREF").ToString(),Eval("NotificationNumber").ToString(),Eval("NotificationNumber").ToString()) %></a></td>
                                                    <td><%# ReplaceAucType(Eval("PWName").ToString()) %></td>
                                                    <td><%# Eval("OrderName") %></td>
                                                    <td><%# String.Format("{0:c}",Eval("MaxPrice")) %></td>
                                                    <td style="width: 200px;"><%# GetFocusHTMLlink(Eval("FZtype").ToString(),Eval("HREF223").ToString(),Eval("HREFB2B").ToString(),Eval("PlacerFullName").ToString()) %></td>
                                                    <td><%# Convert.ToDateTime(Eval("PublishDate")).ToShortDateString() %></td>
                                                    <td><%# Eval("REGION") %></td>
                                                </tr>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                </table>
                                            </FooterTemplate>
                                        </asp:Repeater>
                                        <hr />
                                        <!-- крутой выбор страниц -->
                                        Страница: 
                                        <asp:LinkButton ID="pageFirst" runat="server" Visible="false" OnClick="pageFirst_Click">Первая</asp:LinkButton>
                                        <asp:LinkButton ID="page1" runat="server" Visible="false" OnClick="page1_Click">1</asp:LinkButton>
                                        <asp:LinkButton ID="page2" runat="server" Visible="false" OnClick="page2_Click">2</asp:LinkButton>
                                        <asp:LinkButton ID="page3" runat="server" Visible="false" OnClick="page3_Click">3</asp:LinkButton>
                                        <asp:LinkButton ID="page4" runat="server" Visible="false" OnClick="page4_Click">4</asp:LinkButton>
                                        <asp:LinkButton ID="page5" runat="server" Visible="false" OnClick="page5_Click">5</asp:LinkButton>
                                        <asp:LinkButton ID="pageLast" runat="server" Visible="false" OnClick="pageLast_Click">Последняя</asp:LinkButton>
                                        <!-- тут закончим -->
                                        <br />


                                        <br />

                                        <asp:Button ID="ExportXLSButton" runat="server" Text="Экспорт в Excel" OnClick="Button7_Click" CssClass="button" Visible="false" Style="display: none" />
                                        <br />
                                        <br />
                                        <asp:LinkButton ID="AboutButton" runat="server">Справка</asp:LinkButton>
                                        <br />

                                        <a href="mailto:tview@skbkontur.ru?subject=TenderView.%20Обратная%20связь">Обратная связь</a>

                                        <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtender1" runat="server" Enabled="True" PopupControlID="AboutPanel" TargetControlID="AboutButton" CancelControlID="AboutCloseButton" BackgroundCssClass="modalBackground">
                                        </ajaxToolkit:ModalPopupExtender>

                                        <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtender2" runat="server" Enabled="True" PopupControlID="LotPan" TargetControlID="LotButton" CancelControlID="LotPanelCloseButton" BackgroundCssClass="modalBackground">
                                        </ajaxToolkit:ModalPopupExtender>

                                        <asp:Button ID="LotButton" runat="server" Text="Button" Style="display: none" />

                                        <asp:Panel ID="LotPan" runat="server" Style="display: none">
                                            <div class="wrapper">
                                                <div class="middle">
                                                    <div class="content">
                                                        <div style="position: relative; width: auto; height: 600px; overflow: scroll; background-color: white; top: 0px; left: 0px;">
                                                            <h2 class="headingTitle">Особенности тендера</h2>
                                                            <asp:Table ID="Table1" runat="server" CssClass="table"></asp:Table>
                                                        </div>
                                                        <br />
                                                        <asp:Button ID="LotPanelCloseButton" runat="server" Text="Закрыть" CssClass="button" />
                                                    </div>

                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel ID="AboutPanel" runat="server" Style="display: none">
                                            <div class="wrapper">
                                                <div class="middle">
                                                    <div class="content">
                                                        <p>Поиск в системе осуществляется через наложение различных фильтров (<i>Регион, коды ОКДП, наименование заказа, даты публикации</i>).</p>
                                                        <p>
                                                            <strong>Регион.</strong><br />
                                                            Выбирается список регионов, в которых будет производиться поиск тендера. Пустой список определяет поиск по всем регионам.
                                                        </p>
                                                        <p>
                                                            <strong>Список ОКДП.</strong><br />
                                                            Определяет по каким кодам ОКДП искать тендеры. Существует возможность загрузки списка кодов из текстового файла (каждый код ОКДП должен находиться на отдельной строке).<br />
                                                            Существует возможность использовать фильтр для категорий кодов, например "0112*" будет искать по всем подгруппам и конкретным продуктам данного вида. Фильтр работает, начиная от 2-х первых цифр кода, т.е. допускается поиск по "01*"<br />
                                                            Параметр «Искать тендеры только с этими кодами» определяет, будут ли включены в результаты поиска тендеры, в которых помимо указанных кодов, могут присутствовать другие. Пустой список определяет поиск по всем кодам ОКДП.
                                                        </p>
                                                        <p>
                                                            <strong>Наименование заказа.</strong><br />
                                                            Выполняет поиск по подстроке в названиях заказов.
                                                        </p>
                                                        <p>
                                                            <strong>Дата публикации.</strong><br />
                                                            Выполняет поиск по дате публикации тендера. Использование параметра «С этой даты» означает, что будет произведен поиск тендеров от выбранной даты до текущего момента (но не более 5 дней).
                                                        </p>
                                                        <p><strong>Кнопка «Экспорт в Excel»</strong> позволяет сохранять найденный список тендеров в формат XLS-файла.</p>
                                                        <p>
                                                            Введенные условия поиска тендеров (списки ОКДП, регионы и проч.) можно сохранять в виде профилей.<br />
                                                            После сохранения условия поиска оно станет доступно в списке сохраненных условий.
                                                        </p>
                                                        <br />
                                                        <asp:Button ID="AboutCloseButton" runat="server" Text="Закрыть" />
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                    </asp:View>
                                    <!-- Личный кабинет -->
                                    <asp:View ID="SettingsView" runat="server">
                                        <p class="headingTitle">Управление</p>
                                        <hr />
                                        <div class="g-24 safeContext">
                                            <!-- Первая строка -->
                                            <div class="safeContext">
                                                <div class="g-col-1 g-span-4">
                                                    <span>Сохраненные условия</span>
                                                </div>
                                                <div class="g-col-7 g-span-4">
                                                    <span>Сохраненные рассылки</span>
                                                </div>
                                            </div>

                                            <!-- Вторая строка -->
                                            <div class="safeContext">
                                                <div class="g-col-1 g-span-4">
                                                    <asp:ListBox ID="SavedOptionsLB" runat="server" Width="160"></asp:ListBox>
                                                </div>
                                                <div class="g-col-7 g-span-4">
                                                    <asp:ListBox ID="SavedMailsLB" runat="server" Width="160"></asp:ListBox>
                                                </div>
                                            </div>
                                            <!-- Третья строка -->
                                            <div class="safeContext">
                                                <div class="g-col-1 g-span-4">
                                                    <asp:Button ID="DeleteOptionsBtn" runat="server" Text="Удалить" CssClass="button" OnClick="DeleteOptionsBtn_Click" />
                                                </div>
                                                <div class="g-col-7 g-span-4">
                                                    <asp:Button ID="DeleteMailsBtn" runat="server" Text="Удалить" CssClass="button" OnClick="DeleteMailsBtn_Click" />
                                                </div>
                                            </div>
                                        </div>
                                    </asp:View>
                                </asp:MultiView>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>
            </div>
        </div>

        <asp:SqlDataSource ID="mSQLData" runat="server"
            ConnectionString="<%$ ConnectionStrings:MySQLString %>"
            ProviderName="<%$ ConnectionStrings:MySQLString.ProviderName %>" EnableCaching="true" CacheDuration="6000" />

    </form>
</body>
</html>
