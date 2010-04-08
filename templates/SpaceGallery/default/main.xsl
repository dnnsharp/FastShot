<?xml version="1.0" encoding="iso-8859-1"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="html" indent="no" omit-xml-declaration="yes"/>

<xsl:template match="/">
    <xsl:if test = "/fastshot/img">
    <div class = "spacegallery fsSpaceGallery fsSpaceGallery_default" style = "">
        <xsl:attribute name="style">height: <xsl:value-of select="/fastshot/max_thumb_height * 1.5" />px;</xsl:attribute>
        <xsl:for-each select = "/fastshot/img">
            <img border = "0" class = "pngFix">
                <xsl:attribute name="alt"><xsl:value-of select="title" /></xsl:attribute>
                <xsl:attribute name="title"><xsl:value-of select="title" /></xsl:attribute>
                <xsl:attribute name="src"><xsl:value-of select="thumburl" /></xsl:attribute>
                <xsl:attribute name="width"><xsl:value-of select="thumb_width" /></xsl:attribute>
                <xsl:attribute name="height"><xsl:value-of select="thumb_height" /></xsl:attribute>
            </img>
        </xsl:for-each>
    </div>
    </xsl:if>
</xsl:template>
</xsl:stylesheet>