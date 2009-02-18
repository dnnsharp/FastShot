<%@ Control Language="C#" AutoEventWireup="true" Inherits="avt.FastShot.AddEditItem" EnableViewState = "true" CodeFile="AddEditItem.ascx.cs" %>

<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<asp:UpdatePanel id = "upnlAddEdit" runat = "server">
<ContentTemplate>

<asp:HiddenField runat = "server" id = "hdnThumbFileName"></asp:HiddenField>
<asp:HiddenField runat = "server" id = "hdnImageFileName"></asp:HiddenField>
<asp:HiddenField runat = "server" id = "hdnItemId"></asp:HiddenField>

<table class = "FS_Form" cellpadding = "0" cellspacing = "0" border = "0" style = "width: 100%; height: 340px; margin-top: 12px;">
    <tr>
        <td class = "lbl">
            <dnn:label id="lblTitle" runat="server" controlname="txtTitle" CssClass="SubHead" /> <span class = "required">required</span>
        </td>
        <td class = "fld">
            <asp:TextBox ID="txtTitle" runat = "server" style ="width: 180px;" ></asp:TextBox><br />
            <asp:RequiredFieldValidator runat = "server" ControlToValidate = "txtTitle" Text = "Please enter a title." Display = "Dynamic"></asp:RequiredFieldValidator>
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
            <dnn:label id="lblThumb" runat="server" controlname="fileThumb" CssClass="SubHead" /> <span class = "required">required</span>
        </td>
        <td class = "fld">
            <iframe id="iUploadThumbFrame" src="<%=TemplateSourceDirectory %>/upload.aspx?moduleId=<%=PModuleId %>&m=thumb" frameborder="0" width = "220px" height = "30px" onload="avt.FastShot.core.thumbUploaded('<%=hdnThumbFileName.ClientID %>');"></iframe>
            <div style = "font-weight: bold; font-size: 11px;" runat = "server" id = "sThumbName"></div>
            <span style = "color: red; display: none;">Please provide a thumbnail file.</span>
        </td>
    </tr>
    <tr>
        <td class = "lbl">
            <dnn:label id="lblImg" runat="server" controlname="fileImg" CssClass="SubHead" /> <span class = "required">required</span>
        </td>
        <td class = "fld">
            <iframe id="iUploadImageFrame" src="<%=TemplateSourceDirectory %>/upload.aspx?moduleId=<%=PModuleId %>" frameborder="0" width = "220px" height = "30px" onload="avt.FastShot.core.imageUploaded('<%=hdnImageFileName.ClientID %>');"></iframe>
            <div style = "font-weight: bold; font-size: 11px;" runat = "server" id = "sImageName"></div>
            <span style = "color: red; display: none;">Please provide an image file.</span>
        </td>
    </tr>
    <tr>
        <td class = "lbl">
            <dnn:label id="lblViewOrder" runat="server" controlname="txtViewOrder" CssClass="SubHead" />
        </td>
        <td class = "fld">
            <asp:TextBox ID="txtViewOrder" runat = "server" style ="width: 50px;" ></asp:TextBox>
            <span style ="font-size: 10px; font-style: italic;">Leave blank to append at end of list.</span><br />
            <asp:RegularExpressionValidator runat = "server" ControlToValidate = "txtViewOrder" ValidationExpression = "\d*" Text = "View Order must be in numeric format."></asp:RegularExpressionValidator>
        </td>
    </tr>
    <tr>
        <td colspan = "2" style ="padding-left: 120px; padding-top: 10px; vertical-align: bottom;">
            <asp:LinkButton id="cmdUpdate" runat="server" class = "bluebtn" CausesValidation="True" Text = "Save" OnClick = "OnSave" />
            <asp:LinkButton id="cmdCancel" runat="server" class = "bluebtn" CausesValidation="False" Text = "Cancel" OnClick = "OnCancel" />
        </td>
    </tr>
</table>

</ContentTemplate>
</asp:UpdatePanel>