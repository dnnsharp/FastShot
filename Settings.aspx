<%@ Page Language="C#" Inherits = "avt.FastShot.Settings" AutoEventWireup="True" CodeFile="Settings.aspx.cs" %>

<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html>
<head>

    <title>FastShot - Add/Edit Item</title>
    
    <script type = "text/javascript" src = "<%=TemplateSourceDirectory %>/../../js/dnncore.js"></script>
    <script type = "text/javascript" src = "<%=TemplateSourceDirectory %>/js/jQuery/jQuery-1.3.2.js"></script>
    <script type = "text/javascript" src = "<%=TemplateSourceDirectory %>/js/jquery/jquery-ui-1.6.js"></script>
    <script type = "text/javascript" src = "<%=TemplateSourceDirectory %>/js/avt.core-1.0.js"></script>
    <script type = "text/javascript" src = "<%=TemplateSourceDirectory %>/js/avtFastShot.js"></script>
    <link type = "text/css" rel = "stylesheet" href = "<%=TemplateSourceDirectory %>/module.css" />

<script type = "text/javascript">
if (!window.console) {
    window.console = {};
}
</script>

<style type = "text/css">
    body {
        margin: 0; padding: 0; width: 430px;
    }
</style>

</head>

<body>
<form id="Form1" runat = "server" method="POST" target="_self" enctype="multipart/form-data" action = "">

<input id="ScrollTop" runat="server" name="ScrollTop" type="hidden" />
<input id="__dnnVariable" runat="server" name="__dnnVariable" type="hidden" />

<asp:ScriptManager runat = "server" ID = "scriptManager"></asp:ScriptManager>

<asp:UpdatePanel id = "upnlAddEdit" runat = "server">
<ContentTemplate>


<table class = "FS_Form" cellpadding = "0" cellspacing = "0" border = "0" style = "width: 100%; margin-top: 12px;">
    <tr>
        <td class = "lbl" style = "background-color: #efefef; border: 1px solid #dadada; font-weight: bold;">
            ModuleId: <b><asp:Label runat = "server" id = "txtModuleId" style = ""></asp:Label>
        </td>
        <td class = "fld" style = "border-bottom: 2px dashed #d2d2d2;">
            &nbsp;
        </td>
    </tr>
    <tr>
        <td class = "lbl">
            <dnn:label id="lblTemplate" runat="server" controlname="ddTemplate" CssClass="SubHead" />
        </td>
        <td class = "fld">
            <asp:DropDownList ID="ddTemplate" runat = "server" style ="width: 180px;" ></asp:DropDownList><br />
        </td>
    </tr>
    <tr>
        <td class = "lbl">
            <dnn:label id="lblThumbWidth" runat="server" controlname="txtThumbWidth" CssClass="SubHead" />
        </td>
        <td class = "fld">
            <asp:TextBox ID="txtThumbWidth" runat = "server" style ="width: 180px;"></asp:TextBox><br />
            <asp:RegularExpressionValidator runat = "server" ControlToValidate = "txtThumbWidth" ValidationExpression = "\d*" Text = "Thumb width must be a number greater than or equal to 0." Display = "Dynamic"></asp:RegularExpressionValidator>
        </td>
    </tr>
    <tr>
        <td class = "lbl">
            <dnn:label id="lblThumbHeight" runat="server" controlname="txtThumbHeight" CssClass="SubHead" />
        </td>
        <td class = "fld">
            <asp:TextBox ID="txtThumbHeight" runat = "server" style ="width: 180px;"></asp:TextBox><br />
            <asp:RegularExpressionValidator runat = "server" ControlToValidate = "txtThumbHeight" ValidationExpression = "\d*" Text = "Thumb height must be a number greater than or equal to 0." Display = "Dynamic"></asp:RegularExpressionValidator>
        </td>
    </tr>

    <tr style = "display: none;">
        <td colspan = "2" style ="padding-left: 120px; padding-top: 10px; vertical-align: bottom;">
            <asp:LinkButton id="cmdUpdate" runat="server" class = "bluebtn" CausesValidation="True" Text = "Save" OnClick = "OnSave" rel = "save" />
            <asp:LinkButton id="cmdCancel" runat="server" class = "bluebtn" CausesValidation="False" Text = "Cancel" OnClick = "OnCancel" />
        </td>
    </tr>
</table>

</ContentTemplate>
</asp:UpdatePanel>

<script type ="text/javascript">

function onAutoGenerate(obj) {
    if (obj.checked) {
        avt.fastshot.$(obj).parents("tr").find(".lbl").css("color", "#929292");
        avt.fastshot.$(obj).parents("tr").find(".required").hide();
        avt.fastshot.$(obj).parents("tr").find(":input").not(obj).attr("disabled", "disabled").css("background-color", "#cacaca");
        
        var myVal = document.getElementById('reqThumb');
        ValidatorEnable(myVal, false); 


    } else {
        avt.fastshot.$(obj).parents("tr").find(".lbl").css("color", "#000000");
        avt.fastshot.$(obj).parents("tr").find(".required").show();
        avt.fastshot.$(obj).parents("tr").find(":input").not(obj).removeAttr("disabled").css("background-color", "#ffffff");
        
        var myVal = document.getElementById('reqThumb');
        ValidatorEnable(myVal, true); 
    }
}

</script> 
    
</form>
</body>

</html>