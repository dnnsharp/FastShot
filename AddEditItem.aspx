<%@ Page Language="C#" Inherits = "avt.FastShot.AddEditItem" AutoEventWireup="True" CodeFile="AddEditItem.aspx.cs" %>

<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html>
<head>

    <title>FastShot - Add/Edit Item</title>
    
    <script type = "text/javascript" src = "<%=TemplateSourceDirectory %>/../../js/dnncore.js"></script>
    <script type = "text/javascript" src = "<%=TemplateSourceDirectory %>/js/jQuery-1.3.2.av1.js"></script>
    <script type = "text/javascript" src = "<%=TemplateSourceDirectory %>/js/jquery-ui-1.7.2.av1.js"></script>
    <script type = "text/javascript" src = "<%=TemplateSourceDirectory %>/js/avt.core-1.4.js"></script>
    <script type = "text/javascript" src = "<%=TemplateSourceDirectory %>/js/avtFastShot.js"></script>
    <link type = "text/css" rel = "stylesheet" href = "<%=TemplateSourceDirectory %>/module.css" />

<script type = "text/javascript">
if (!window.console) {
    window.console = {};
}
</script>

<style type = "text/css">
    body {
        margin: 0; padding: 0;
    }
</style>

</head>

<body>
<form id="Form1" runat = "server" method="POST" target="_self" enctype="multipart/form-data" action = "">

<input id="ScrollTop" runat="server" name="ScrollTop" type="hidden" />
<input id="__dnnVariable" runat="server" name="__dnnVariable" type="hidden" />

<asp:ScriptManager runat = "server" ID = "scriptManager"></asp:ScriptManager>

<div runat = "server"  id = "pnlTitleSA" style = "border: 1px solid #828282; background-color: #e9e9e9; padding: 6px; font-size: 20px; margin: 0 0 10px 0;">
    FastShot - Add New Image
</div>

<asp:UpdatePanel id = "upnlAddEdit" runat = "server">
<ContentTemplate>

<asp:HiddenField runat = "server" id = "hdnItemId"></asp:HiddenField>

<table class = "FS_Form" cellpadding = "0" cellspacing = "0" border = "0" style = "margin-top: 12px;">
    <tr>
        <td class = "lbl">
            <dnn:label id="lblTitle" runat="server" controlname="txtTitle" CssClass="SubHead" /> <span class = "required">required</span>
        </td>
        <td class = "fld">
            <asp:TextBox ID="txtTitle" runat = "server" style ="width: 180px;" ></asp:TextBox><br />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat = "server" ControlToValidate = "txtTitle" Text = "Please enter a title." Display = "Dynamic"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td class = "lbl">
            <dnn:label id="lblDesc" runat="server" controlname="txtDesc" CssClass="SubHead" />
        </td>
        <td class = "fld">
            <asp:TextBox ID="txtDesc" runat = "server" style ="width: 180px; height: 80px;" TextMode = "MultiLine"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class = "lbl">
            <dnn:label id="lblImg" runat="server" controlname="fileImg" CssClass="SubHead" /> <span runat = "server" id = "lblReqImage" class = "required">required</span>
        </td>
        <td class = "fld">
            <asp:FileUpload runat = "server" ID = "flImage" />
            <asp:RequiredFieldValidator runat = "server" ID = "reqImage" Text = "Please upload an image file.<br />" ControlToValidate = "flImage" Display = "Dynamic"></asp:RequiredFieldValidator>
            <asp:Label runat = "server" ID = "lblExistingImage" style = "font-size: 11px;"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class = "lbl">
            <dnn:label id="lblThumb" runat="server" controlname="fileThumb" CssClass="SubHead" /> <span runat = "server" id = "lblReqThumb" class = "required">required</span>
        </td>
        <td class = "fld">
            <asp:FileUpload runat = "server" ID = "flThumb" /><br />
            <asp:Label runat = "server" ID = "lblExistingThumb" style = "font-size: 11px;"></asp:Label>
            <asp:RequiredFieldValidator runat = "server" ID = "reqThumb" Text = "Please upload a thumb file.<br />" ControlToValidate = "flThumb" Display = "Dynamic"></asp:RequiredFieldValidator>
            <label><asp:CheckBox runat = "server" ID = "cbAutoGenerate" type = "checkbox" onclick = "onAutoGenerate(this);" />Auto Generate (based on settings)</label>
        </td>
    </tr>
    <tr>
        <td class = "lbl">
            <dnn:label id="lblViewOrder" runat="server" controlname="txtViewOrder" CssClass="SubHead" />
        </td>
        <td class = "fld">
            <asp:TextBox ID="txtViewOrder" runat = "server" style ="width: 50px;" ></asp:TextBox>
            <span style ="font-size: 10px; font-style: italic;">Leave blank to append at end of list.</span><br />
            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat = "server" ControlToValidate = "txtViewOrder" ValidationExpression = "\d*" Text = "View Order must be in numeric format."></asp:RegularExpressionValidator>
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


<div runat = "server" id = "pnlStandAloneBtns" style = "margin-left: 200px; margin-top: 20px;">
    <asp:LinkButton runat = "server" id = "btnSave" class = "bluebtn" OnClick = "OnSave" CausesValidation = "true">Save</asp:LinkButton>
    <asp:LinkButton runat = "server" id = "btnCancel" class = "bluebtn" OnClick = "OnCloseSA" CausesValidation = "false">Cancel</asp:LinkButton>
</div>

<script type ="text/javascript">

function onAutoGenerate(obj) {
    if (obj.checked) {
        avt.fastshot.$(obj).parents("tr").find(".lbl").css("color", "#929292");
        avt.fastshot.$(obj).parents("tr").find(".required").hide();
        avt.fastshot.$(obj).parents("tr").find(":input").not(obj).attr("disabled", "disabled").css("background-color", "#cacaca");
        
        var myVal = document.getElementById('reqThumb');
        myVal && ValidatorEnable(myVal, false); 


    } else {
        avt.fastshot.$(obj).parents("tr").find(".lbl").css("color", "#000000");
        avt.fastshot.$(obj).parents("tr").find(".required").show();
        avt.fastshot.$(obj).parents("tr").find(":input").not(obj).removeAttr("disabled").css("background-color", "#ffffff");
        
        var myVal = document.getElementById('reqThumb');
        myVal && ValidatorEnable(myVal, true); 
    }
}

</script> 

</form>
</body>

</html>