<?xml version="1.0" encoding="iso-8859-1"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="html" indent="no" omit-xml-declaration="yes"/>

<xsl:template match="/">
    <xsl:if test = "/fastshot/img">
	<div class = " FastShot_grid_general">
		<div class = "FastShot_grid_top">
		<ul class = "FastShot_grid">
			<xsl:for-each select = "/fastshot/img">
			   <li style = " border: 3px sold #fafafa;" >
					<input type = "hidden">
						<xsl:attribute name="value"><xsl:value-of select="id" /></xsl:attribute>
					</input>
					<xsl:if test = "/fastshot/mng">
						<div class = "FastShot_default_modify" style = "text-align: center; margin-bottom: 4px;">
							<a><xsl:attribute name="href"><xsl:value-of select="editurl" /></xsl:attribute>edit </a>
							<a alt = "edit in compatibility mode" title = "edit in compatibility mode"><xsl:attribute name="href"><xsl:value-of select="editurl_comp" /></xsl:attribute>&gt;</a> | 
							<a><xsl:attribute name="href"><xsl:value-of select="deleteurl" /></xsl:attribute>delete</a>
						</div>
					</xsl:if>
					<a class="lightbox imgThumb" style = "text-align: center; display: block;">
						<xsl:attribute name="rel"><xsl:value-of select="/fastshot/mid" /></xsl:attribute>
						<xsl:attribute name="href"><xsl:value-of select="imgurl" /></xsl:attribute>
						<xsl:attribute name="alt"><xsl:value-of select="desc" /></xsl:attribute>
						<xsl:attribute name="title"><xsl:value-of select="desc" /></xsl:attribute>
						<img class = "pngFix" style = " border: 2px solid #dedede;" >
							<xsl:attribute name="alt"><xsl:value-of select="desc" /></xsl:attribute>
							<xsl:attribute name="src"><xsl:value-of select="thumburl" /></xsl:attribute>
							<xsl:attribute name="width"><xsl:value-of select="thumb_width" /></xsl:attribute>
							<xsl:attribute name="height"><xsl:value-of select="thumb_height" /></xsl:attribute>
						</img>
					</a>
				</li>
			</xsl:for-each>
		</ul>
		<div style = "clear: both"></div>
		</div>
	</div>
    <div style = "clear: both"></div>
    </xsl:if>
</xsl:template>
</xsl:stylesheet>