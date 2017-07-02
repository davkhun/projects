<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="UEBase.login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head" runat="server">
    <title>Вход</title>
    <meta content="text/html; charset=utf-8" http-equiv="Content-type" />
    <meta content="IE=edge" http-equiv="X-UA-Compatible" />
    <link href="content/modern.css" rel="stylesheet" type="text/css" />
    <script src='scripts/jquery-2.1.3.min.js'></script>
    <style>
        #bg {
            position: fixed;
            top: 0;
            left: 0;
        }

        .bgwidth {
            width: 100%;
        }

        .bgheight {
            height: 100%;
        }
    </style>
    <script>
        $(window).load(function () {

            var theWindow = $(window),
                $bg = $("#bg"),
                aspectRatio = $bg.width() / $bg.height();

            function resizeBg() {

                if ((theWindow.width() / theWindow.height()) < aspectRatio) {
                    $bg
                        .removeClass()
                        .addClass('bgheight');
                } else {
                    $bg
                        .removeClass()
                        .addClass('bgwidth');
                }

            }

            theWindow.resize(resizeBg).trigger("resize");

        });
    </script>
</head>
<body>
   
    <form id="form1" runat="server">
        <div class="main-section">
            <div class="sign" style="width: 300px; position: relative;">
                <fieldset>

                    <legend>Учёт техники</legend>
                    <table class="table-wrapper" width="100%">
                        <tbody>
                            <tr>
                                <td>
                                    <div class="group-item">
                                        <asp:TextBox ID="TextBox1" runat="server" MaxLength="64" placeholder="Логин" />
                                    </div>
                                    <div class="group-item password">
                                        <asp:TextBox ID="TextBox2" runat="server" MaxLength="64" TextMode="Password" placeholder="Пароль" />
                                    </div>
                                    <div class="submit-block-wrapper">
                                        <div class="group-item submit">
                                            <div class="checkbox">
                                                <asp:CheckBox ID="rememberCb" runat="server" />
                                                <label for="rememberCb">Запомнить меня</label>
                                            </div>
                                        </div>
                                        <button type="submit" id="Submit1" value="submit" class="submit-button" runat="server" onserverclick="Button1_Click">Войти</button>
                                    </div>
                                    <asp:Label ID="Label1" runat="server" Text="" ForeColor="Red"></asp:Label>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </fieldset>
            </div>
        </div>

    </form>
</body>
</html>
