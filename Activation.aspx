<%@ Page Language="C#" AutoEventWireup="true" Inherits="avt.FastShot.ActivationWnd" EnableViewState = "true" CodeFile="Activation.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html>
<head>
    <title>FastShot Activation</title>
    <link type = "text/css" rel = "stylesheet" href = "<%=TemplateSourceDirectory %>/act/style.css" />
    
    <style type = "text/css">
        
        
        
    </style>
</head>

<body id="Body" runat="server">

<form runat = "server" id = "Form" method = "post" onsubmit = "">


<div class = "act_wnd">

    <div class = "title">Activate FastShot</div>
    <div class = "help">
        <a class = "help_link" href = "http://docs.avatar-soft.ro/doku.php?id=general:activation">Help</a>
    </div>
    <div style = "clear: both;"></div>
    
    <asp:Panel runat = "server" ID = "pnlSuccess" Visible = "false" style ="height: 290px; margin-top: 30px; padding-top: 30px;">
        <br />
        <b>Activation Successfull!</b><br /><br />
        <div style = "text-align: left">
            This copy of FastShot has been successfully activated.<br /><br />
            For additional information, support, user manuals and other resources please check our website at <a href="http://www.avatar-soft.ro">www.avatar-soft.ro</a>
            or the <a href = "<%= avt.FastShot.FastShotController.DocSrv%>">documentation server</a>.
            <br /><br /><br /><br /><br /><br />
        </div>
    </asp:Panel>

    <div style = "height: 320px; margin-top: 30px;" runat = "server" id = "pnlActivateForm">
    
        <div style = "height: 20px">
            <asp:CustomValidator runat = "server" ID = "validateActivation" CssClass = "lblerr" ControlToValidate = "txtRegistrationCode" Text = "" Display = "Static" ></asp:CustomValidator>
        </div>
        
        <div class = "instructions">
            Please input the registration code you've received on email.<br />
            If you don't have one yet, <a href = "<%= avt.FastShot.FastShotController.BuyLink %>">click here</a> to purchase a new license.
        </div>
        
        <div class = "reg_code" runat = "server" id = "pnlRegCode">
            <div class = "field_title">
                Registration code <asp:RequiredFieldValidator ID="RequiredFieldValidator1" CssClass = "lblerr" runat = "server" ControlToValidate = "txtRegistrationCode" Text = "required"></asp:RequiredFieldValidator>
            </div>
            <asp:LinkButton ID="btnNext" runat = "server" CssClass = "btn_next" OnClick = "OnNext">Next</asp:LinkButton>
            <asp:TextBox runat = "server" ID = "txtRegistrationCode" CssClass = "input_field"></asp:TextBox>
            <div class = "small_desc">Please input the registration code you've received on email</div>
            <div style = "clear: both;"></div>
        </div>
        
        <div runat = "server" id = "pnlHosts" visible = "false" style = "margin-top: 30px;">
            <div class = "field_title">
                Select Host <asp:RequiredFieldValidator ID="RequiredFieldValidator2" CssClass = "lblerr" runat = "server" ControlToValidate = "ddHosts" Text = "required"></asp:RequiredFieldValidator>
            </div>
            <asp:LinkButton ID="LinkButton1" runat = "server" CssClass = "btn_next" OnClick = "OnActivate">Activate</asp:LinkButton>
            <asp:DropDownList runat = "server" ID = "ddHosts" CssClass = "input_field"></asp:DropDownList>
            <div class = "small_desc">Choose the host to activate for</div>
            <div style = "clear: both;"></div>
        </div>
        
    </div>
    
    
    <div class = "btns">
        <a runat = "server" id = "lnkPurchase" class = "btn_purchase">Purchase</a>
        <asp:LinkButton ID="btnClose" runat = "server" CssClass = "btn_cancel" OnClick = "OnCloseSA" CausesValidation = "false">Cancel</asp:LinkButton>
        <div style = "clear: both;"></div>
    </div>
</div>


</form>
</body>
</html>