<%@ Control Language="C#" AutoEventWireup="true" Inherits="avt.FastShot.Main" EnableViewState = "true" CodeFile="Main.ascx.cs" %>

<div class = "FastShot">
    <div id = "pnlErr" runat = "server"></div>
    <asp:UpdatePanel runat = "server" ID = "upnlRender" UpdateMode = "Conditional">
        <ContentTemplate>
            
            <div runat = "server" id = "pnlDragOrdering" visible = "false">
                <asp:HiddenField runat = "server" ID = "hdnItemOrder" />
                <div runat = "server" id = "fsOrderingCapabilities">
                    <b>Tip:</b> You can order images by dragging them on the screen...
                </div>
                <div  runat = "server" id = "fsOrderingChanged" class = "fs_pending_changes" style = "display: none;">
                    FastShot detected changes. Click <a href = "javascript: ;" onclick = "onSaveOrder<%=ModuleId %>()">here</a> to save them when done.
                </div>
                <script type = "text/javascript">
                    function onSaveOrder<%=ModuleId %>() {
                        
                        avt.fastshot.$("#<%=fsOrderingChanged.ClientID %>").html("Saving, please wait...");
                        
                        var items = "";
                        avt.fastshot.$("#<%=itemContainer.ClientID %>").find(":hidden").each(function() {
                            items += avt.fastshot.$(this).val() + ",";
                        });
                        items = items.substring(0, items.length-1);
                        
                        avt.fastshot.$("#<%=hdnItemOrder.ClientID %>").val(items);
                        __doPostBack('<%=triggerOrderChange.UniqueID %>');
                    }
                </script>
            </div>
        
            <asp:LinkButton runat = "server" ID = "triggerRender" CausesValidation = "false" style = "display: none" OnClick = "OnRender" ></asp:LinkButton>
            <asp:LinkButton runat = "server" ID = "triggerSave" CausesValidation = "false" style = "display: none" OnClick = "OnSaveSuccess" ></asp:LinkButton>
            <asp:LinkButton runat = "server" ID = "triggerSaveSettings" CausesValidation = "false" style = "display: none" OnClick = "OnSaveSettingsSuccess" ></asp:LinkButton>
            <asp:LinkButton runat = "server" ID = "triggerOrderChange" CausesValidation = "false" style = "display: none" OnClick = "OnChangeOrder" ></asp:LinkButton>
            <div runat = "server" id = "itemContainer"></div>
            
            <div id = "newItem" class = "FastShot_dlg" style ="display: none;">
                <asp:UpdatePanel runat = "server" ID = "upnlConf" UpdateMode = "Conditional">
                <ContentTemplate>
                    <asp:LinkButton runat = "server" ID = "triggerAddEdit" CausesValidation = "false" style = "display: none" OnClick = "OnShowAddEdit"></asp:LinkButton>
                    <asp:LinkButton runat = "server" ID = "triggerDelete" CausesValidation = "false" style = "display: none" OnClick = "OnDelete"></asp:LinkButton>
                </ContentTemplate>
                </asp:UpdatePanel>
            </div>

            <div id = "pnlSettings" runat = "server" style = "clear: both;">
                <a href="javascript: void(0)" class="fg-button fg-button-icon-right ui-widget ui-state-default ui-corner-all" runat = "server" id = "btnFsSettings"><span class="ui-icon ui-icon-triangle-1-s"></span><img src = '<%=TemplateSourceDirectory %>/res/fastshot_small.png' border = "0" style = "margin-right: 6px;" align = "absmiddle" class = "pngFix" height = "20" /> FastShot</a>
                <div style = "position:absolute; top:0; left:-9999px; width:1px; height:1px; overflow:hidden">
                    <ul style = "font-size: 12px;" class = "nobullets">
                        <li><asp:LinkButton runat = "server" id = "btnAddNewItem">New Image</asp:LinkButton></li>
                        <li><asp:LinkButton runat = "server" id = "btnAddNewItemNewWnd" ToolTip = "Edit image in a compatibility mode - the form opens in a separate page to avoid javascript errors on the page preventing the dialog from opening.">New Image (comp.)</asp:LinkButton></li>
                        <li><asp:LinkButton runat = "server" id = "btnSettings">Settings</asp:LinkButton></li>
                        <li><asp:LinkButton runat = "server" id = "btnSettingsNewWnd" ToolTip = "Edit settings in a compatibility mode - the form opens in a separate page to avoid javascript errors on the page preventing the dialog from opening.">Settings (comp.)</asp:LinkButton></li>
                        <li><asp:LinkButton runat = "server" id = "btnActivate">Activate</asp:LinkButton></li>
                        <li><asp:LinkButton runat = "server" id = "btnActivateNewWnd">Activate (comp.)</asp:LinkButton></li>
                        <li style = "border-bottom: 1px dashed #929292; height: 1px; margin: 4px 0 4px 0; font-size: 1px;"></li>
                        <li>
                            <a href = "http://www.avatar-soft.ro/Products/FastShot/tabid/131/Default.aspx">FastShot</a>
                            <ul class = "nobullets">
                                <li><a href = "http://www.avatar-soft.ro/Products/FastShot/tabid/131/Default.aspx">Product Page</a></li>
                                <li><a href = "http://docs.avatar-soft.ro/doku.php?id=fshot:start">Documentation</a></li>
                                <li><a href = "http://www.avatar-soft.ro/Support/Forums/tabid/99/Default.aspx">Forums</a></li>
                                <li><a href = "http://www.avatar-soft.ro/Contact/tabid/57/Default.aspx">Contact</a></li>
                            </ul>
                        </li>
                    </ul>
                </div>


                <script type="text/javascript">
                    function initProductMenu<%=ModuleId %>() {
                        //avt.fs.$(function(){
                            // BUTTONS
                            avt.fs.$("#<%= upnlRender.ClientID %>").find('.fg-button').hover(
                                function(){ avt.fs.$(this).removeClass('ui-state-default').addClass('ui-state-focus'); },
                                function(){ avt.fs.$(this).removeClass('ui-state-focus').addClass('ui-state-default'); }
                            );

                            // MENUS
                           avt.fs.$('#<%= btnFsSettings.ClientID %>').menu({
                                content: avt.fs.$('#<%= btnFsSettings.ClientID %>').next().html(), // grab content from this page
                                showSpeed: 200,
                                width: 160,
                                flyOut: true
                            });
                        //});
                    }

                </script>

            </div>

            <div style ="clear: both"></div>

        </ContentTemplate>
    </asp:UpdatePanel>

</div>


