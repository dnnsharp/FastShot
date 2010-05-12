<%@ Control Language="C#" AutoEventWireup="true" Inherits="avt.FastShot.PatchModules" EnableViewState = "true" CodeFile="SkinObject_PatchModule.ascx.cs" %>

<script type ="text/javascript">
    avt.fs.$(document).ready(function($) {
        if (<%= PatchModuleId %> != -1) {
            $("a[name=<%= PatchModuleId %>]").next().find("a.lightbox").each(function() {
                $(this).lightbox({
                    fileLoadingImage: '<%= TemplateSourceDirectory %>/js/jquery-lightbox/images/loading.gif',
                    fileBottomNavCloseImage: '<%= TemplateSourceDirectory %>/js/jquery-lightbox/images/closelabel.gif'
                });
            });

            avt.fs.$$.fixPng();
        } else {
            $("a.lightbox").each(function() {
                $(this).lightbox({
                    fileLoadingImage: '<%= TemplateSourceDirectory %>/js/jquery-lightbox/images/loading.gif',
                    fileBottomNavCloseImage: '<%= TemplateSourceDirectory %>/js/jquery-lightbox/images/closelabel.gif'
                });
            });

            avt.fs.$$.fixPng();
        }
    });
</script>


