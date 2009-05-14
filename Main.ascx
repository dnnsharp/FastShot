<%@ Control Language="C#" AutoEventWireup="true" Inherits="avt.FastShot.Main" EnableViewState = "true" CodeFile="Main.ascx.cs" %>

<div class = "FastShot">
    <div id = "pnlErr" runat = "server"></div>
    <asp:UpdatePanel runat = "server" ID = "upnlRender" UpdateMode = "Conditional">
        <ContentTemplate>
            <asp:LinkButton runat = "server" ID = "triggerRender" CausesValidation = "false" style = "display: none" OnClick = "OnRender" ></asp:LinkButton>
            <div runat = "server" id = "itemContainer"></div>
            
            <div id = "newItem" class = "FastShot_dlg" style ="display: none;">
                <asp:UpdatePanel runat = "server" ID = "upnlConf" UpdateMode = "Conditional">
                <ContentTemplate>
                    <asp:LinkButton runat = "server" ID = "triggerAddEdit" CausesValidation = "false" style = "display: none" OnClick = "OnShowAddEdit"></asp:LinkButton>
                    <asp:LinkButton runat = "server" ID = "triggerDelete" CausesValidation = "false" style = "display: none" OnClick = "OnDelete" ></asp:LinkButton>
                </ContentTemplate>
                </asp:UpdatePanel>
            </div>

            <div runat = "server" id = "pnlManage" visible = "false" style ="margin-top: 20px; float: right;">
                <asp:LinkButton runat = "server" ID = "btnAddNewItem" CausesValidation = "false" CssClass = "btnIcon" OnClientClick = "return false;">
                    <img src = "/DesktopModules/avt.FastShot/res/fastshot_small.png" border = "0" class = "fixPng" />New Item...
                </asp:LinkButton>
                
                <asp:LinkButton runat = "server" ID = "btnSettings" CausesValidation = "false" CssClass = "btnIcon" OnClientClick = "avt.fastshot.$$.showDlg('/DesktopModules/avt.FastShot/AddEditItem.aspx', {cssClass : 'FastShot_dlg'}); return false;">
                    <img src = "/DesktopModules/avt.FastShot/res/fastshot_small.png" border = "0" class = "fixPng" />Settings...
                </asp:LinkButton>
               
                <asp:LinkButton runat = "server" ID = "btnActivate" CausesValidation = "false" CssClass = "btnIcon" OnClientClick = "return false;">
                    <img src = "/DesktopModules/avt.FastShot/res/fastshot_small.png" border = "0" class = "fixPng" />Activate...
                </asp:LinkButton>
            </div>
            <div style ="clear: both"></div>
            
            
        </ContentTemplate>
    </asp:UpdatePanel>


</div>

