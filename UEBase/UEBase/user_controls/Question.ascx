<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Question.ascx.cs" Inherits="UEBase.user_controls.Question" %>
<asp:HiddenField ID="HiddenValue" runat="server" />
<div class="modal-sm">
    <div class="modal-content">
        <div class="modal-body">
                <asp:Label ID="QuestionLbl" runat="server" Text="Sure?"></asp:Label>
        </div>
        <div class="modal-footer">
            <asp:Button ID="yesBtn" runat="server" Text="Да" CssClass="btn btn-primary" OnClick="yesBtn_Click" />
            <asp:Button ID="noBtn" runat="server" Text="Нет" CssClass="btn btn-default" />
        </div>
    </div>
</div>

