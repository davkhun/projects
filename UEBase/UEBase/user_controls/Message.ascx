<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Message.ascx.cs" Inherits="UEBase.user_controls.Message" %>
<div class="modal-sm">
    <div class="modal-content">
        <div class="modal-body">
                <asp:Label ID="MessageLbl" runat="server" Text="Sure?"></asp:Label>
        </div>
        <div class="modal-footer">
            <asp:Button ID="okBtn" runat="server" Text="OK" CssClass="btn btn-primary"/>

        </div>
    </div>
</div>

