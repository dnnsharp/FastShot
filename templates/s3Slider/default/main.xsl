<?xml version="1.0" encoding="iso-8859-1"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="html" indent="no" omit-xml-declaration="yes"/>

<xsl:template match="/">
    <xsl:if test = "/fastshot/img">
    <div class = "fss3Slider fss3Slider_default" style = "">
        <ul class="sliderContent">
        <xsl:for-each select = "/fastshot/img">
            <li class="slider1Image">
                <img border = "0" class = "pngFix">
                    <xsl:attribute name="alt"><xsl:value-of select="title" /></xsl:attribute>
                    <xsl:attribute name="title"><xsl:value-of select="title" /></xsl:attribute>
                    <xsl:attribute name="src"><xsl:value-of select="thumburl" /></xsl:attribute>
                    <!-- <xsl:attribute name="width"><xsl:value-of select="thumb_width" /></xsl:attribute>
                    <xsl:attribute name="height"><xsl:value-of select="thumb_height" /></xsl:attribute> -->
                </img>
                <span>
                    <xsl:attribute name="class">leftSlideText slideText <xsl:value-of select="tplParams" /></xsl:attribute>
                    <strong><xsl:value-of select="title" /></strong><br />
                    <xsl:value-of select="desc" disable-output-escaping="yes" />
                </span>
            </li>
        </xsl:for-each>
        <li class="clear"></li>
        </ul>
    </div>
    </xsl:if>
</xsl:template>
</xsl:stylesheet>