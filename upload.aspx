<%@ Page Language="C#" Inherits = "avt.FastShot.Upload" CodeFile="upload.aspx.cs" AutoEventWireup="True" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html>
<head>

<script>
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
<form method="POST" target="_self" enctype="multipart/form-data" action = "">
    <input type = "hidden" id = "hdFilename" name = "hdFilename" runat = "server" />
    <div runat = "server" id = "pnlFile">
        <input id = "upl" type="file" name="upl" style = "width: 200px" />
        <input type = "submit" style = "display: none" />
    </div>
</form>
</body>

</html>