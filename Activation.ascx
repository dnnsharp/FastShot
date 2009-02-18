<%@ Control Language="C#" AutoEventWireup="true" Inherits="avt.FastShot.Activation" EnableViewState = "true" CodeFile="Activation.ascx.cs" %>

<asp:Panel runat = "server" ID = "pnlActivateDlg" class = "blue" style ="text-align: center; font-family: Arial; display: none;">
<asp:UpdatePanel runat = "server" ID = "upnlActivate" UpdateMode = "Conditional">
    <ContentTemplate>
        <asp:LinkButton runat = "server" ID = "triggerActivate" CausesValidation = "false" style = "display: none" OnClick = "OnShowActivate"></asp:LinkButton>
        <asp:Panel runat = "server" ID = "pnlActivate" Visible = "false">
            <table cellpadding = "0" cellspacing = "0" border = "0" style ="width: 100%; height: 246px; margin-top: 6px; text-align: left;">
                <tr>
                    <td valign = "top" style = "padding: 10px; height: 30px;">
                        Please enter the registration code that you received on the email.
                        If you don't have a license yet, <a href = "<%=avt.FastShot.FastShotController.BuyLink %>" target = "_blank">click here</a> to purchase one from Snowcovered
                    </td>
                </tr>
                <tr>
                    <td valign = "top" style = "padding-top: 10px; padding-left: 50px; height: 30px;">
                        <b>Hostname:</b><br />
                        <asp:DropDownList runat = "server" ID = "ddHosts" style = "width: 240px; padding: 2px; font-size: 11px; font-weight: bold; color: #282828; margin: 4px; "></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td valign = "top" style = "padding-top: 10px; padding-left: 50px; height: 30px;">
                        <b>Registration Code:</b><br />
                        <asp:TextBox runat = "server" ID = "txtRegistrationCode" style = "width: 240px; padding: 2px; font-size: 11px; font-weight: bold; color: #282828; margin: 4px; " ValidationGroup = "avtFasShotValid"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style = "padding: 10px; padding-left: 30px;" valign = "top">
                        <asp:CustomValidator runat = "server" ID = "validateActivation" ControlToValidate = "txtRegistrationCode" Text = "" ValidationGroup = "avtFasShotValid"></asp:CustomValidator>
                    </td>
                </tr>
            </table>
            <asp:LinkButton runat = "server" ID = "btnActivate" Text = "Activate" class = "navxp_tab" OnClick = "OnActivate" OnClientClick = "avt.Common.disableInput(); " style = "margin-left: 70px;" CausesValidation = "true" ValidationGroup = "avtFasShotValid"></asp:LinkButton>
            <asp:LinkButton runat = "server" ID = "btnCloseAct" Text = "Cancel" class = "navxp_tab" OnClick = "OnCloseActivation" OnClientClick = "avt.Common.disableInput(); " style = "" CausesValidation = "false"></asp:LinkButton>
            <a href = "http://snowcovered.com" class = "navxp_tab" target = "_blank">Purchase</a>
        </asp:Panel>
        
        <asp:Panel runat = "server" ID = "pnlInvalidHost" Visible = "false" style ="padding: 6px; text-align: left;">
            <br />
            <b>Activation Not Possible!</b><br /><br />
            <div style = "text-align: left">
                There is no portal alias defined in your DNN installation that is based on host address rather than IPs. 
                Since NavXp is licensed per host (for example www.abccompany.com), activation is not possible at this point.
                <br /><br />
                Please go into Site Settings and add a valid Portal Alias for this installation.
                <br /><br /><br /><br /><br />
            </div>
            <asp:LinkButton runat = "server" ID = "LinkButton1" Text = "Close" class = "navxp_tab" OnClick = "OnCloseActivation" OnClientClick = "avt.Common.disableInput(); " style = "margin-left: 160px;" CausesValidation = "false"></asp:LinkButton>
        </asp:Panel>
        
        <asp:Panel runat = "server" ID = "pnlSuccess" Visible = "false" style ="padding: 6px;">
            <br />
            <b>Activation Successfull!</b><br /><br />
            <div style = "text-align: left">
                This copy of FastShot has been successfully activated. For more information on activations and deactivations consult <a href="#">this page</a> of the documentation.<br /><br />
                For additional information, support, user manuals and tons of other resources please login at <a href="http://products.avatar-soft.ro" target = "_blank">products.avatar-soft.ro</a> with the credentials that were provided to you.
                <br /><br /><br /><br /><br /><br />
            </div>
            <asp:LinkButton runat = "server" ID = "btnClose" Text = "Close" class = "navxp_tab" OnClick = "OnCloseActivation" OnClientClick = "avt.Common.disableInput(); " style = "margin-left: 160px;" CausesValidation = "false"></asp:LinkButton>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
</asp:Panel>

