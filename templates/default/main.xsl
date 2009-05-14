<?xml version="1.0" encoding="ISO-8859-1"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="html" indent="no" omit-xml-declaration="yes"/>

<xsl:template match="/">
    <xsl:if test = "/fastshot/img">
    <div class = "FastShot_default">
        <xsl:for-each select = "/fastshot/img">
            <div style = "float: left; margin: 6px;">
                <xsl:if test = "/fastshot/mng">
                    <div class = "FastShot_default_modify" style = "text-align: center; margin-bottom: 4px;">
                        <a><xsl:attribute name="href"><xsl:value-of select="editurl" /></xsl:attribute>edit</a> | 
                        <a><xsl:attribute name="href"><xsl:value-of select="deleteurl" /></xsl:attribute>delete</a>
                    </div>
                </xsl:if>
                <a class="lightbox imgThumb" style = "text-align: center; display: block;">
                    <xsl:attribute name="rel"><xsl:value-of select="/fastshot/mid" /></xsl:attribute>
                    <xsl:attribute name="href"><xsl:value-of select="imgurl" /></xsl:attribute>
                    <xsl:attribute name="alt"><xsl:value-of select="desc" /></xsl:attribute>
                    <xsl:attribute name="title"><xsl:value-of select="desc" /></xsl:attribute>
                    <img border = "0" align = "" style = "" class = "pngFix">
                        <xsl:attribute name="src"><xsl:value-of select="thumburl" /></xsl:attribute>
                    </img>
                </a>
                <a class="lightbox imgTitle" style = "text-align: center; display: block;">
                    <xsl:attribute name="rel"><xsl:value-of select="/fastshot/mid" /></xsl:attribute>
                    <xsl:attribute name="href"><xsl:value-of select="imgurl" /></xsl:attribute>
                    <xsl:value-of select="title" />
                </a>
            </div>
        </xsl:for-each>
    </div>
    </xsl:if>
    <div style = "clear: both"></div>
</xsl:template>
</xsl:stylesheet>