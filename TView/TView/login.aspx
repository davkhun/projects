<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="TView.login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Вход в TenderView</title>
    <meta content="text/html; charset=utf-8" http-equiv="Content-type" />
    <meta content="IE=edge" http-equiv="X-UA-Compatible" />
    <link href="/css/tview.css" rel="stylesheet" type="text/css" />
    <link href="/css/ke.css" rel="stylesheet" type="text/css" />
    <link href="/css/table.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="content">
            <div class="wrapperOuter">
                <div class="wrapper">
                    <div style="padding: 15px;" class="middle">
                        <div class="g-24 safeContext">
                            <div class="safeContext">
                                <div class="g-col-12 g-span-4">
                                    <h2 style="text-align: center; height: 10px;">Вход в TenderView</h2>
                                </div>
                            </div>
                            <div class="safeContext">
                                <div class="g-col-1 g-span-24">
                                    <hr />
                                </div>
                            </div>
                            <div class="safeContext">
                                <div class="g-col-11 g-span-2">
                                    Логин:
                                </div>
                                <div class="g-col-13 g-span-3">
                                    <span class="field field__px150 field_unhover">
                                        <asp:TextBox ID="TextBox1" runat="server" MaxLength="64" /></span>
                                </div>
                                <div class="g-col-16 g-span-6">
                                    <asp:CustomValidator ID="CustomValidator5" runat="server" ControlToValidate="TextBox1" ErrorMessage="CustomValidator" OnServerValidate="CustomValidator5_ServerValidate" ValidateEmptyText="True" Style="padding-left: 10px; color: red;">Поле не может быть пустым!</asp:CustomValidator>
                                </div>
                            </div>
                            <div class="safeContext">
                                <div class="g-col-11 g-span-2">
                                    Пароль:
                                </div>
                                <div class="g-col-13 g-span-3">
                                    <span class="field field__px150 field_unhover">
                                        <asp:TextBox ID="TextBox2" runat="server" MaxLength="64" TextMode="Password" /></span>
                                </div>
                                <div class="g-col-16 g-span-6">
                                    <asp:CustomValidator ID="CustomValidator6" runat="server" ControlToValidate="TextBox2" ErrorMessage="Поле не может быть пустым!" OnServerValidate="CustomValidator6_ServerValidate" ValidateEmptyText="True" Style="padding-left: 10px; color: red;">Поле не может быть пустым!</asp:CustomValidator>
                                </div>
                            </div>
                            <div class="safeContext">
                                <div class="g-col-13 g-span-1">
                                    <br />
                                    <asp:Button ID="Button1" runat="server" Text="Войти" OnClick="Button1_Click" CausesValidation="False" CssClass="button button__colored__purple" Style="color: white; width: 100px;" />
                                </div>
                            </div>
                            <br />
                            <div class="safeContext">
                                <div class="g-col-11 g-span-5">
                                    <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" OnClick="LinkButton1_Click">Изменение пароля</asp:LinkButton>
                                    <br />
                                    <asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" OnClick="LinkButton2_Click">Восстановление пароля</asp:LinkButton>
                                    <br />
                                    <br />
                                    <a href="/register.aspx">Зарегистрироваться</a>
                                </div>
                            </div>

                            <br />
                            <br />
                            <!-- изменение пароля -->
                            <asp:Panel ID="Panel1" runat="server" Visible="False">
                                <div class="safeContext">
                                    <div class="g-col-11 g-span-3">
                                        Логин:
                                    </div>
                                    <div class="g-col-14 g-span-3">
                                        <span class="field field__px150 field_unhover">
                                            <asp:TextBox ID="TextBox3" runat="server" MaxLength="64" Width="120px" /></span>
                                    </div>
                                    <div class="g-col-17 g-span-6">
                                        <asp:CustomValidator ID="CustomValidator1" runat="server" ControlToValidate="TextBox3" ErrorMessage="Поле не может быть пустым!" OnServerValidate="CustomValidator1_ServerValidate" ValidateEmptyText="True" Style="padding-left: 10px; color: red;">Поле не может быть пустым!</asp:CustomValidator>
                                    </div>

                                </div>
                                <div class="safeContext">
                                    <div class="g-col-11 g-span-3">
                                        Старый пароль:
                                    </div>
                                    <div class="g-col-14 g-span-3">
                                        <span class="field field__px150 field_unhover">
                                            <asp:TextBox ID="TextBox4" runat="server" MaxLength="64" TextMode="Password" /></span>
                                    </div>
                                    <div class="g-col-17 g-span-6">
                                        <asp:CustomValidator ID="CustomValidator2" runat="server" ControlToValidate="TextBox4" ErrorMessage="CustomValidator" OnServerValidate="CustomValidator2_ServerValidate" ValidateEmptyText="True" Style="padding-left: 10px; color: red;">Поле не может быть пустым!</asp:CustomValidator>

                                    </div>

                                </div>
                                <div class="safeContext">
                                    <div class="g-col-11 g-span-3">
                                        Новый пароль:
                                    </div>
                                    <div class="g-col-14 g-span-3">
                                        <span class="field field__px150 field_unhover">
                                            <asp:TextBox ID="TextBox5" runat="server" MaxLength="64" TextMode="Password" /></span>
                                    </div>
                                    <div class="g-col-17 g-span-6">
                                        <asp:CustomValidator ID="CustomValidator3" runat="server" ControlToValidate="TextBox5" ErrorMessage="CustomValidator" OnServerValidate="CustomValidator3_ServerValidate" ValidateEmptyText="True" Style="padding-left: 10px; color: red;">Поле не может быть пустым!</asp:CustomValidator>
                                    </div>

                                </div>
                                <div class="safeContext">
                                    <div class="g-col-11 g-span-3">
                                        Повторите пароль:
                                    </div>
                                    <div class="g-col-14 g-span-3">
                                        <span class="field field__px150 field_unhover">
                                            <asp:TextBox ID="TextBox6" runat="server" MaxLength="64" TextMode="Password" /></span>
                                    </div>
                                    <div class="g-col-17 g-span-6">
                                        <asp:CustomValidator ID="CustomValidator4" runat="server" ControlToValidate="TextBox6" ErrorMessage="CustomValidator" OnServerValidate="CustomValidator4_ServerValidate" ValidateEmptyText="True" Style="padding-left: 10px; color: red;">Поле не может быть пустым!</asp:CustomValidator>
                                    </div>

                                </div>
                                <div class="safeContext">
                                    <div class="g-col-11 g-span-6">
                                        <br />
                                        <asp:Button ID="Button2" runat="server" Text="Изменить" CausesValidation="False" OnClick="Button2_Click" CssClass="button" />
                                        <asp:Label ID="Label1" runat="server" ForeColor="Red" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <!-- восстановление пароля -->
                            <asp:Panel ID="Panel2" runat="server" Visible="False">
                                <div class="safeContext">
                                    <div class="g-col-11 g-span-2">Логин:</div>
                                    <div class="g-col-13 g-span-3">
                                        <span class="field field__px150 field_unhover">
                                            <asp:TextBox ID="TextBox7" runat="server" MaxLength="64" /></span>
                                    </div>
                                    <div class="g-col-16 g-span-6">
                                        <asp:CustomValidator ID="CustomValidator7" runat="server" ControlToValidate="TextBox7" ErrorMessage="CustomValidator" OnServerValidate="CustomValidator7_ServerValidate" ValidateEmptyText="True" Style="padding-left: 10px; color: red;">Поле не может быть пустым!</asp:CustomValidator>
                                    </div>
                                </div>
                                <div class="safeContext">
                                    <div class="g-col-11 g-span-2">Email:</div>
                                    <div class="g-col-13 g-span-3">
                                        <span class="field field__px150 field_unhover">
                                            <asp:TextBox ID="TextBox8" runat="server" MaxLength="64" /></span>
                                    </div>
                                    <div class="g-col-16 g-span-6">
                                        <asp:CustomValidator ID="CustomValidator8" runat="server" ControlToValidate="TextBox8" ErrorMessage="CustomValidator" OnServerValidate="CustomValidator8_ServerValidate" ValidateEmptyText="True" Style="padding-left: 10px; color: red;">Поле не может быть пустым!</asp:CustomValidator>
                                    </div>
                                </div>
                                <div class="safeContext">
                                    <div class="g-col-11 g-span-4">
                                        <br />
                                        <asp:Button ID="Button3" runat="server" Text="Восстановить" CausesValidation="False" OnClick="Button3_Click" CssClass="button" />
                                        <asp:Label ID="Label2" runat="server" ForeColor="Red" />
                                    </div>
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
