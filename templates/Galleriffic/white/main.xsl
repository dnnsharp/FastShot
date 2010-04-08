<?xml version="1.0" encoding="iso-8859-1"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="html" indent="no" omit-xml-declaration="yes"/>

<xsl:template match="/">
    <xsl:if test = "/fastshot/img">
    <div class = "fsGalleriffic fsGalleriffic_white ">
        <div class = "fsgNavOuterC">
            <div class = "fsgNavC" style = "">
                <div class="fsgThumbs navigation">
                    <a class="pageLink prev" style="visibility: hidden;" href="#" title="Previous Page"></a>
                    
                    <ul class="fsgThumbsC noscript">
                                
                    <xsl:for-each select = "/fastshot/img">
                        <li>
                            <a class="thumb" name="leaf">
                                <xsl:attribute name="rel"><xsl:value-of select="/fastshot/mid" /></xsl:attribute>
                                <xsl:attribute name="href"><xsl:value-of select="imgurl" /></xsl:attribute>
                                <xsl:attribute name="alt"><xsl:value-of select="desc" /></xsl:attribute>
                                <xsl:attribute name="title"><xsl:value-of select="desc" /></xsl:attribute>
                                <img border = "0" class = "pngFix">
                                    <xsl:attribute name="alt"><xsl:value-of select="title" /></xsl:attribute>
                                    <xsl:attribute name="title"><xsl:value-of select="title" /></xsl:attribute>
                                    <xsl:attribute name="src"><xsl:value-of select="thumburl" /></xsl:attribute>
                                    <xsl:attribute name="width"><xsl:value-of select="thumb_width" /></xsl:attribute>
                                    <xsl:attribute name="height"><xsl:value-of select="thumb_height" /></xsl:attribute>
                                </img>
                            </a>
                            <div class="caption">
                                <div class="image-title"><xsl:value-of select="title" disable-output-escaping="yes" /></div>
                                <div class="image-desc"><xsl:value-of select="desc" disable-output-escaping="yes" /></div>
                                <div class="download">
                                    <a>
                                        <xsl:attribute name="href"><xsl:value-of select="imgurl" /></xsl:attribute>
                                        Download Original
                                    </a>
                                </div>
                            </div>
                        </li>
                    </xsl:for-each>
                        <li style = "clear: both;"></li>
                    </ul>
                    <a class="pageLink next" style="visibility: hidden;" href="#" title="Next Page"></a>
                    <div style = "clear: both;"></div>
                </div>
            </div>
            <div style = "clear: both;"></div>
        </div>
        
        <div class="fsgContent">
            <div class="slideshow-container">
                <div class="controls"></div>
                <div class="loader"></div>
                <div class="slideshow"></div>
            </div>
            <div class="caption-container">
                <div class="photo-index"></div>
            </div>
            <div style = "clear: both;"></div>
        </div>
    </div>
    </xsl:if>
</xsl:template>
</xsl:stylesheet>