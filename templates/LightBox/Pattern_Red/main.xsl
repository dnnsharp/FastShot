<?xml version="1.0" encoding="iso-8859-1"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="html" indent="no" omit-xml-declaration="yes"/>

<xsl:template match="/">
    <xsl:if test = "/fastshot/img">
	<div class = " FastShot_grid_general">
		<div class = "">
		<ul class = "FastShot_grid ">
			<xsl:for-each select = "/fastshot/img">
				<li style = "">
					<input type = "hidden">
						<xsl:attribute name="value"><xsl:value-of select="id" /></xsl:attribute>
					</input>
					<a class="lightbox imgThumb">
						<xsl:attribute name="rel"><xsl:value-of select="/fastshot/mid" /></xsl:attribute>
						<xsl:attribute name="href"><xsl:value-of select="imgurl" /></xsl:attribute>
						<xsl:attribute name="alt"><xsl:value-of select="desc" /></xsl:attribute>
						<xsl:attribute name="title"><xsl:value-of select="desc" /></xsl:attribute>
						<xsl:attribute name="style">text-align: center; display: block; width: <xsl:value-of select="/fastshot/max_thumb_width" />px; height: <xsl:value-of select="/fastshot/max_thumb_height" />px;</xsl:attribute>
						<img border = "0" class = "pngFix">
							<xsl:attribute name="alt"><xsl:value-of select="title" /></xsl:attribute>
							<xsl:attribute name="title"><xsl:value-of select="title" /></xsl:attribute>
							<xsl:attribute name="src"><xsl:value-of select="thumburl" /></xsl:attribute>
							<xsl:attribute name="width"><xsl:value-of select="thumb_width" /></xsl:attribute>
							<xsl:attribute name="height"><xsl:value-of select="thumb_height" /></xsl:attribute>
						</img>
					</a>
					<a class="lightbox imgTitle" style = "text-align: center; display: block;">
						<xsl:attribute name="rel"><xsl:value-of select="/fastshot/mid" /></xsl:attribute>
						<xsl:attribute name="href"><xsl:value-of select="imgurl" /></xsl:attribute>
						<xsl:value-of select="title" disable-output-escaping="yes" />
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