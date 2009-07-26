<%@ Page Language="C#" AutoEventWireup="true" Inherits="avt.FastShot.ActivationWnd" EnableViewState = "true" CodeFile="Activation.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html>
<head>
    <title>FastShot Activation</title>
    <script type = "text/javascript" src = "<%=TemplateSourceDirectory %>/../../js/dnncore.js"></script>
    <script type = "text/javascript" src = "<%=TemplateSourceDirectory %>/js/jQuery-1.3.2.av1.js"></script>
    <script type = "text/javascript" src = "<%=TemplateSourceDirectory %>/js/jquery-ui-1.7.2.av1.js"></script>
    <script type = "text/javascript" src = "<%=TemplateSourceDirectory %>/js/avt.core-1.4.js"></script>
    <script type = "text/javascript" src = "<%=TemplateSourceDirectory %>/js/avtFastShot.js"></script>
    <link type = "text/css" rel = "stylesheet" href = "<%=TemplateSourceDirectory %>/module.css" />
    
    <style type = "text/css">
        .portal_tree {
            width: 280px; 
            height: 370px; 
            overflow: auto; 
            border: 1px solid #d2d2d2; 
            float: left;
            display: none;
        }
        
        a:link, a:visited {
            text-decoration: none;
            font-weight: bold;
            color: #4b7ba1;
            font-size: 12px;
        }
        
        a:hover, a:active {
            color: #4b7ba1;
            text-decoration: underline;
        }
        
    </style>
</head>

<body id="Body" runat="server">

<form runat = "server" id = "Form" method = "post" onsubmit = "">

<asp:ScriptManager runat = "server" ID = "scriptManager"></asp:ScriptManager>


<asp:UpdatePanel runat = "server" ID = "upnlActivate" UpdateMode = "Conditional">
    <ContentTemplate>
        <asp:Panel runat = "server" ID = "pnlActivate" Visible = "true">
            <table cellpadding = "0" cellspacing = "0" border = "0" style ="width: 430px; margin-top: 6px; text-align: left;">
                <tr>
                    <td valign = "top" style = "padding: 10px; height: 30px;">
                        Please enter the registration code that you received on the email.
                        If you don't have a license yet, <a href = "<%=avt.FastShot.FastShotController.BuyLink %>" target = "_blank">click here</a> to purchase one from Snowcovered
                    </td>
                </tr>
                <tr>
                    <td valign = "top" style = "padding-top: 20px; padding-left: 50px; height: 30px;">
                        <b>Registration Code:</b> <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat = "server" ControlToValidate = "txtRegistrationCode" Text = "required"></asp:RequiredFieldValidator><br />
                        <asp:TextBox runat = "server" ID = "txtRegistrationCode" style = "width: 380px; padding: 2px; font-size: 11px; font-weight: bold; color: #282828; margin: 4px;"></asp:TextBox> <br />
                        <span style = "font-size: 11px;">Fill with the registration code you received on the email.</span>
                    </td>
                </tr>
                <tr>
                    <td valign = "top" style = "padding-top: 10px; padding-left: 50px; height: 30px;" visible = "false" runat = "server" id = "trDomains">
                        <asp:Label runat = "server" ID = "lblDomainTitle" style = "font-weight: bold"></asp:Label>
                        <asp:DropDownList runat = "server" ID = "ddHosts" style = "width: 240px; padding: 2px; font-size: 11px; font-weight: bold; color: #282828; margin: 4px; "></asp:DropDownList><br />
                        <span style = "font-size: 11px;" runat = "server" visible = "false" id ="lblDomain">Choose the domain you wish to activate for.</span>
                        <span style = "font-size: 11px;" runat = "server" visible = "false" id ="lblPrimaryDomain">The primary domain is used to talk back to your server for validating activation.</span>
                    </td>
                </tr>
                <tr>
                    <td style = "padding: 10px; padding-left: 30px;" valign = "top">
                        <asp:CustomValidator runat = "server" ID = "validateActivation" ControlToValidate = "txtRegistrationCode" Text = ""></asp:CustomValidator>
                    </td>
                </tr>
            </table>
            
            <asp:LinkButton runat = "server" id = "btnActivate" class = "" OnClick = "OnActivate" rel = "save" style = "margin-top: 30px; margin-left: 360px; display: inline;font-size: 13px; padding: 8px; border: 1px solid #969696;" CausesValidation = "true" OnClientClick = "avt.fastshot.$$.frameLoading(parent, window);" Visible = "false">
                <img src = "<%=TemplateSourceDirectory %>/res/globe.png" border = "0" style = "margin-right: 6px;" align = "absmiddle" />Activate
            </asp:LinkButton>
            
            <asp:LinkButton runat = "server" id = "btnNext" class = "" OnClick = "OnNext" rel = "save" style = "margin-top: 30px; margin-left: 360px; display: inline;font-size: 13px; padding: 8px; border: 1px solid #969696;" CausesValidation = "true" OnClientClick = "avt.fastshot.$$.frameLoading(parent, window);">
                <img src = "<%=TemplateSourceDirectory %>/res/globe.png" border = "0" style = "margin-right: 6px;" align = "absmiddle" />Next
            </asp:LinkButton>
    
        </asp:Panel>
        
        <asp:Panel runat = "server" ID = "pnlInvalidHost" Visible = "false" style ="padding: 6px; text-align: left;">
            <br />
            <b>Activation Not Possible!</b><br /><br />
            <div style = "text-align: left">
                There is no portal alias defined in your DNN installation that is based on host address rather than IPs. 
                Since FastShot is licensed per domain (for example www.abccompany.com), activation is not possible at this point.
                <br /><br />
                Please go into Site Settings and add a valid Portal Alias for this installation.
                <br /><br /><br /><br /><br />
            </div>
        </asp:Panel>
        
        <asp:Panel runat = "server" ID = "pnlSuccess" Visible = "false" style ="padding: 6px;">
            <br />
            <b>Activation Successfull!</b><br /><br />
            <div style = "text-align: left">
                This copy of FastShot has been successfully activated. For more information on activations and deactivations consult <a href="#">this page</a> of the documentation.<br /><br />
                For additional information, support, user manuals and tons of other resources please login at <a href="http://products.avatar-soft.ro" target = "_blank">products.avatar-soft.ro</a> with the credentials that were provided to you.
                <br /><br /><br /><br /><br /><br />
            </div>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>


</form>
</body>
</html>