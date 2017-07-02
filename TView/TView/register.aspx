<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="register.aspx.cs" Inherits="TView.register" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Регистрация TenderView</title>
    <meta content="text/html; charset=utf-8" http-equiv="Content-type" />
    <meta content="IE=edge" http-equiv="X-UA-Compatible" />
    <link href="/css/tview.css" rel="stylesheet" type="text/css" />
    <link href="/css/ke.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="wrapperOuter">
            <div class="wrapper">
                <div class="header">
                </div>
                <div class="middle">
                    <div class="content">
                        <p class="headingTitle">Регистрация в системе TenderView</p>
                        <hr />
                        <br />
                        <div class="g-24 safeContext">

                            <asp:Panel ID="RegPanel" runat="server" Visible="false">
                                <div class="safeContext">
                                    <div class="g-col-9 g-span-3">
                                        Введите логин:
                                    </div>
                                    <div class="g-col-12 g-span-3">
                                        <span class="field field__px150 field_unhover">
                                            <asp:TextBox ID="LoginTB" runat="server" /></span>
                                    </div>
                                    <div class="g-col-16 g-span-8">
                                        <asp:RequiredFieldValidator ID="LoginValidator" runat="server" ErrorMessage="* Поле не может быть пустым!" ControlToValidate="LoginTB" Style="color: red;"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="safeContext">
                                    <div class="g-col-9 g-span-3">
                                        Введите email:
                                    </div>
                                    <div class="g-col-12 g-span-3">
                                        <span class="field field__px150 field_unhover">
                                            <asp:TextBox ID="EmailTB" runat="server" />
                                        </span>
                                    </div>
                                    <div class="g-col-16 g-span-8">
                                        <asp:Label ID="EmailLb" runat="server" Text="" Style="color: red;"></asp:Label>
                                        <asp:RequiredFieldValidator ID="EmailValidator" runat="server" ErrorMessage="* Поле не может быть пустым!" ControlToValidate="EmailTB" Style="color: red;"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <br />
                                <div class="safeContext">
                                    <div class="g-col-12 g-span-4">
                                        <asp:Button ID="SendButton" runat="server" Text="Зарегистрироваться" OnClick="SendButton_Click" class="button button__colored button__colored__purple" />
                                    </div>
                                </div>
                                <div class="g-col-11 g-span-6">
                                    <p><b>
                                        <asp:Label ID="msgLbl" runat="server" Text="" /></b></p>
                                </div>
                            </asp:Panel>
                            <asp:Panel ID="ValidPanel" runat="server">

                                <div class="g-col-9 g-span-12">
                                    <h3><b>
                                        <asp:Label ID="finishLbl" runat="server" Text="" /></b></h3>
                                </div>
                            </asp:Panel>

                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
