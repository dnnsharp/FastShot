<%@ Control Language="C#" AutoEventWireup="true" Inherits="avt.FastShot.Main" EnableViewState = "true" CodeFile="Main.ascx.cs" %>

<div class = "FastShot">

    <div runat ="server" id = "pnlErr"></div> 
    <div runat = "server" id = "itemContainer"></div>

    <div id = "pnlSettings" runat = "server" style = "clear: both;">
        <a href = "<%=TemplateSourceDirectory %>/FastShotStudio.aspx?mid=<%=ModuleId %>">
            <img src = '<%=TemplateSourceDirectory %>/res/fastshot_small.png' border = "0" style = "margin-right: 2px;" align = "absmiddle" class = "pngFix" height = "20" />
            FastShot Studio &gt;
        </a>
    </div>

    <div style ="clear: both"></div>

</div>


