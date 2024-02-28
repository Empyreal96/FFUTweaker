<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                              xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
                              xmlns:css="http://www.w3.org/TR/XSL-for-CSS">

  <xsl:output method="html" indent="yes"/>

  <xsl:template match="MtbfReport">
    <css:table margin-left="auto" margin-right="auto"/>


    <html>
      <head>
        <style type="text/css">
          H2 {text-align: center }
          H1 {text-align: center}
          td {text-align: right }
          th {text-align: center }
          .leftcol {text-align: left }
          .rightcol {text-align: right }
          .sectioncol
          {
          background-color: #779999;
          font-weight: bold;
          }
          .testrow0 {background-color: #F0F0F0;}
          .totalrightcol
          {
          border-right-width:2px ;
          }

          .scroll
          {
          padding: 20px;
          width: 100%;
          overflow-x: auto;
          overflow-y: hidden;
          }

          table
          {
          border-collapse:collapse;
          }
          table, th, td
          {
          margin-left:auto;
          margin-right:auto;
          border: 1px solid black;
          }
          td, th
          {
          white-space: nowrap;
          padding:5px;
          }
          th
          {
          background-color: #C8C8C8;
          }
          body {text-align:center;}
        </style>
      </head>

      <body>
        <h1>MTBF 概述</h1>
        <table>
          <tr>
            <th>平台</th>
            <th>版本</th>
            <th>设备数</th>
            <th>
              设备数<br/>已完成/<br/>正在进行
            </th>
            <th>Bug Numbers</th>
            <th>
              总持续时间
            </th>
            <th>
              已通过/<br/>预期
            </th>
            <th>通过率</th>
            <th>已禁用测试</th>
            <th>最终状态</th>
          </tr>
          <td class="leftcol"/>
          <td class="leftcol"/>
          <td>
            <xsl:value-of select="count(Devices/Device)"/>
          </td>
          <td/>
          <td class="leftcol"/>

          <xsl:variable name="overviewTotalDuration" select="sum(Devices/Device/Groups/Group[@Name='Main']/@DurationMilliSeconds)"/>
          <td class="rightcol">
            <xsl:value-of select="format-number(floor($overviewTotalDuration div (60 * 60 * 1000)), '00')"/>
            <xsl:text>:</xsl:text>
            <xsl:value-of select="format-number(floor($overviewTotalDuration div (60 * 1000)) - (floor($overviewTotalDuration div (60 * 60 * 1000)) * 60), '00')"/>
            <xsl:text>:</xsl:text>
            <xsl:value-of select="format-number(floor($overviewTotalDuration div 1000) - (floor($overviewTotalDuration div (60 * 1000)) * 60), '00')"/>
          </td>

          <xsl:variable name="overviewTotalPassed" select="sum(Devices/Device/Groups/Group[@Name='Main']/Loops/Loop/Sections/Section/Tests/Test/Result/@PassedCount)"/>
          <xsl:variable name="overviewTotalExpected" select="sum(Devices/Device/Groups/Group[@Name='Main']/Loops/Loop/Sections/Section/Tests/Test/Result/@ExpectedCount)"/>
          <td class="rightcol">
            <xsl:value-of select="$overviewTotalPassed"/>
            <xsl:text>/</xsl:text>
            <xsl:value-of select="$overviewTotalExpected"/>
          </td>

          <xsl:variable name="overviewTotalRate">
            <xsl:choose>
              <xsl:when test="$overviewTotalExpected != 0">
                <xsl:value-of select="format-number(($overviewTotalPassed * 100) div $overviewTotalExpected, '0.00')"/>
              </xsl:when>
              <xsl:otherwise>
                <xsl:text>0</xsl:text>
              </xsl:otherwise>
            </xsl:choose>
          </xsl:variable>

          <td class="rightcol">
            <xsl:choose>
              <xsl:when test="$overviewTotalRate &lt; 99">
                <font color="red">
                  <xsl:value-of select="$overviewTotalRate"/>%
                </font>
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="$overviewTotalRate"/>%
              </xsl:otherwise>
            </xsl:choose>
          </td>
          <td class="leftcol"/>
          <td class="leftcol"/>
        </table>
        <br/>
        <br/>
        <br/>
        <br/>
        <h1>MTBF 已禁用测试</h1>
        <table>
          <caption>(仅限主测试)</caption>
          <tr>
            <th>设备</th>
            <th>预期</th>
            <th>已通过</th>
            <th>比率</th>
            <th>持续时间</th>
          </tr>

          <xsl:for-each select="Devices/Device">
            <tr>
              <td class="leftcol">
                <xsl:value-of select="@Number"/>
              </td>

              <xsl:variable name="deviceTotalExpected" select="sum(Groups/Group[@Name='Main']/Loops/Loop/Sections/Section/Tests/Test/Result/@ExpectedCount)"/>
              <td class="rightcol">
                <xsl:value-of select="$deviceTotalExpected"/>
              </td>

              <xsl:variable name="deviceTotalPassed" select="sum(Groups/Group[@Name='Main']/Loops/Loop/Sections/Section/Tests/Test/Result/@PassedCount)"/>
              <td class="rightcol">
                <xsl:value-of select="$deviceTotalPassed"/>
              </td>

              <xsl:variable name="deviceTotalRate">
                <xsl:choose>
                  <xsl:when test="$deviceTotalExpected != 0">
                    <xsl:value-of select="format-number(($deviceTotalPassed * 100) div $deviceTotalExpected, '0.00')"/>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:text>0</xsl:text>
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:variable>

              <td class="rightcol">
                <xsl:choose>
                  <xsl:when test="$deviceTotalRate &lt; 99">
                    <font color="red">
                      <xsl:value-of select="$deviceTotalRate"/>%
                    </font>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="$deviceTotalRate"/>%
                  </xsl:otherwise>
                </xsl:choose>
              </td>

              <xsl:variable name="deviceTotalDuration" select="sum(Groups/Group[@Name='Main']/@DurationMilliSeconds)"/>
              <td class="rightcol">
                <xsl:value-of select="format-number(floor($deviceTotalDuration div (60 * 60 * 1000)), '00')"/>
                <xsl:text>:</xsl:text>
                <xsl:value-of select="format-number(floor($deviceTotalDuration div (60 * 1000)) - (floor($deviceTotalDuration div (60 * 60 * 1000)) * 60), '00')"/>
                <xsl:text>:</xsl:text>
                <xsl:value-of select="format-number(floor($deviceTotalDuration div 1000) - (floor($deviceTotalDuration div (60 * 1000)) * 60), '00')"/>
              </td>
            </tr>
          </xsl:for-each>

          <tr>
            <th>总计</th>

            <xsl:variable name="summaryTotalExpected" select="sum(Devices/Device/Groups/Group[@Name='Main']/Loops/Loop/Sections/Section/Tests/Test/Result/@ExpectedCount)"/>
            <th class="rightcol">
              <xsl:value-of select="$summaryTotalExpected"/>
            </th>

            <xsl:variable name="summaryTotalPassed" select="sum(Devices/Device/Groups/Group[@Name='Main']/Loops/Loop/Sections/Section/Tests/Test/Result/@PassedCount)"/>
            <th class="rightcol">
              <xsl:value-of select="$summaryTotalPassed"/>
            </th>

            <xsl:variable name="summaryTotalRate">
              <xsl:choose>
                <xsl:when test="$summaryTotalExpected != 0">
                  <xsl:value-of select="format-number(($summaryTotalPassed * 100) div $summaryTotalExpected, '0.00')"/>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:text>0</xsl:text>
                </xsl:otherwise>
              </xsl:choose>
            </xsl:variable>

            <th class="rightcol">
              <xsl:choose>
                <xsl:when test="$summaryTotalRate &lt; 99">
                  <font color="red">
                    <xsl:value-of select="$summaryTotalRate"/>%
                  </font>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="$summaryTotalRate"/>%
                </xsl:otherwise>
              </xsl:choose>
            </th>

            <xsl:variable name="summaryTotalDuration" select="sum(Devices/Device/Groups/Group[@Name='Main']/@DurationMilliSeconds)"/>
            <th class="rightcol">
              <xsl:value-of select="format-number(floor($summaryTotalDuration div (60 * 60 * 1000)), '00')"/>
              <xsl:text>:</xsl:text>
              <xsl:value-of select="format-number(floor($summaryTotalDuration div (60 * 1000)) - (floor($summaryTotalDuration div (60 * 60 * 1000)) * 60), '00')"/>
              <xsl:text>:</xsl:text>
              <xsl:value-of select="format-number(floor($summaryTotalDuration div 1000) - (floor($summaryTotalDuration div (60 * 1000)) * 60), '00')"/>
            </th>
          </tr>

        </table>

        <br/>
        <br/>
        <br/>
        <br/>
        <h1>MTBF 详细测试报告</h1>
        <xsl:for-each select="Devices/Device">
          <h2>
            <u>
              设备 <xsl:value-of select="@Number"/>
            </u>
          </h2>

          <xsl:for-each select="Groups/Group">
            <br/>
            <br/>
            <div class="scroll">
              <table>
                <caption align="center">
                  <h3>
                    <xsl:value-of select="@Name"/> Tests
                  </h3>
                </caption>

                <tr>
                  <th rowspan="2">测试</th>
                  <th colspan="3" class="totalrightcol">总计</th>
                  <xsl:for-each select="Loops/Loop">
                    <xsl:if test="count(Sections/Section/Tests/Test) != 0">
                      <th colspan="3">
                        循环 <xsl:value-of select="@Number"/>
                      </th>
                    </xsl:if>
                  </xsl:for-each>
                </tr>
                <tr>
                  <th>
                    已通过/<br/>预期
                  </th>
                  <th>比率</th>
                  <th class="totalrightcol">持续时间</th>
                  <xsl:for-each select="Loops/Loop">
                    <xsl:if test="count(Sections/Section/Tests/Test) != 0">
                      <th>
                        已通过/<br/>预期
                      </th>
                      <th>比率</th>
                      <th>持续时间</th>
                    </xsl:if>
                  </xsl:for-each>
                </tr>

                <xsl:for-each select="Loops/Loop[1]/Sections/Section">
                  <xsl:variable name="sectionNumber" select="@Number"/>
                  <xsl:variable name="sectionIndex" select="position()"/>

                  <tr>
                    <td class="leftcol sectioncol">
                      节: <xsl:value-of select="@Number"/>
                    </td>

                    <xsl:variable name="sectionTotalExpected" select="sum(../../../../Loops/Loop/Sections/Section[@Number=$sectionNumber]/Tests/Test/Result/@ExpectedCount)"/>
                    <xsl:variable name="sectionTotalPassed" select="sum(../../../../Loops/Loop/Sections/Section[@Number=$sectionNumber]/Tests/Test/Result/@PassedCount)"/>
                    <td  class="sectioncol">
                      <xsl:value-of select="$sectionTotalPassed"/>/<xsl:value-of select="$sectionTotalExpected"/>
                    </td>

                    <xsl:variable name="sectionTotalRate">
                      <xsl:choose>
                        <xsl:when test="$sectionTotalExpected != 0">
                          <xsl:value-of select="format-number(($sectionTotalPassed * 100) div $sectionTotalExpected, '0.00')"/>
                        </xsl:when>
                        <xsl:otherwise>
                          <xsl:text>0</xsl:text>
                        </xsl:otherwise>
                      </xsl:choose>
                    </xsl:variable>

                    <td class="sectioncol">
                      <xsl:choose>
                        <xsl:when test="$sectionTotalRate &lt; 99">
                          <font color="red">
                            <xsl:value-of select="$sectionTotalRate"/>%
                          </font>
                        </xsl:when>
                        <xsl:otherwise>
                          <xsl:value-of select="$sectionTotalRate"/>%
                        </xsl:otherwise>
                      </xsl:choose>
                    </td>

                    <xsl:variable name="sectionTotalDuration" select="sum(../../../../Loops/Loop/Sections/Section[@Number=$sectionNumber]/Tests/Test/@DurationMilliSeconds)"/>
                    <td class="sectioncol totalrightcol">
                      <xsl:value-of select="format-number(floor($sectionTotalDuration div (60 * 60 * 1000)), '00')"/>
                      <xsl:text>:</xsl:text>
                      <xsl:value-of select="format-number(floor($sectionTotalDuration div (60 * 1000)) - (floor($sectionTotalDuration div (60 * 60 * 1000)) * 60), '00')"/>
                      <xsl:text>:</xsl:text>
                      <xsl:value-of select="format-number(floor($sectionTotalDuration div 1000) - (floor($sectionTotalDuration div (60 * 1000)) * 60), '00')"/>
                    </td>

                    <xsl:for-each select="../../../../Loops/Loop">

                      <xsl:if test="count(Sections/Section[@Number=$sectionNumber]/Tests/Test) != 0">
                        <xsl:variable name="sectionLoopTotalExpected" select="sum(Sections/Section[@Number=$sectionNumber]/Tests/Test/Result/@ExpectedCount)"/>
                        <xsl:variable name="sectionLoopTotalPassed" select="sum(Sections/Section[@Number=$sectionNumber]/Tests/Test/Result/@PassedCount)"/>
                        <td class="sectioncol">
                          <xsl:value-of select="$sectionLoopTotalPassed"/>/<xsl:value-of select="$sectionLoopTotalExpected"/>
                        </td>

                        <xsl:variable name="sectionLoopTotalRate">
                          <xsl:choose>
                            <xsl:when test="$sectionLoopTotalExpected != 0">
                              <xsl:value-of select="format-number(($sectionLoopTotalPassed * 100) div $sectionLoopTotalExpected, '0.00')"/>
                            </xsl:when>
                            <xsl:otherwise>
                              <xsl:text>0</xsl:text>
                            </xsl:otherwise>
                          </xsl:choose>
                        </xsl:variable>
                        <td class="sectioncol">
                          <xsl:choose>
                            <xsl:when test="$sectionLoopTotalRate &lt; 99">
                              <font color="red">
                                <xsl:value-of select="$sectionLoopTotalRate"/>%
                              </font>
                            </xsl:when>
                            <xsl:otherwise>
                              <xsl:value-of select="$sectionLoopTotalRate"/>%
                            </xsl:otherwise>
                          </xsl:choose>
                        </td>


                        <xsl:variable name="sectionLoopDuration" select="Sections/Section[@Number=$sectionNumber]/@DurationMilliSeconds"/>
                        <td class="sectioncol">
                          <xsl:value-of select="format-number(floor($sectionLoopDuration div (60 * 60 * 1000)), '00')"/>
                          <xsl:text>:</xsl:text>
                          <xsl:value-of select="format-number(floor($sectionLoopDuration div (60 * 1000)) - (floor($sectionLoopDuration div (60 * 60 * 1000)) * 60), '00')"/>
                          <xsl:text>:</xsl:text>
                          <xsl:value-of select="format-number(floor($sectionLoopDuration div 1000) - (floor($sectionLoopDuration div (60 * 1000)) * 60), '00')"/>
                        </td>
                      </xsl:if>
                    </xsl:for-each>
                  </tr>



                  <xsl:for-each select="Tests/Test">
                    <xsl:variable name="testIndex" select="position()"/>


                    <tr class="testrow{($sectionIndex + $testIndex) mod 2}">
                      <td class="leftcol">
                        <xsl:choose>
                          <xsl:when test="@Name != ''">
                            <xsl:value-of select="@Name"/>
                          </xsl:when>
                          <xsl:otherwise>
                            <xsl:value-of select="@Command"/>
                            <xsl:value-of select="@Parameter"/>
                          </xsl:otherwise>
                        </xsl:choose>
                      </td>

                      <xsl:variable name="totalExpected" select="sum(../../../../../../Loops/Loop/Sections/Section[@Number=$sectionNumber]/Tests/Test[$testIndex]/Result/@ExpectedCount)"></xsl:variable>
                      <xsl:variable name="totalPassed" select="sum(../../../../../../Loops/Loop/Sections/Section[@Number=$sectionNumber]/Tests/Test[$testIndex]/Result/@PassedCount)"/>
                      <td>
                        <xsl:value-of select="$totalPassed"/>/<xsl:value-of select="$totalExpected"/>
                      </td>

                      <xsl:variable name="totalRate">
                        <xsl:choose>
                          <xsl:when test="$totalExpected != 0">
                            <xsl:value-of select="format-number(($totalPassed * 100) div $totalExpected, '0.00')"/>
                          </xsl:when>
                          <xsl:otherwise>
                            <xsl:text>0</xsl:text>
                          </xsl:otherwise>
                        </xsl:choose>
                      </xsl:variable>

                      <td>
                        <xsl:choose>
                          <xsl:when test="$totalRate &lt; 99">
                            <font color="red">
                              <xsl:value-of select="$totalRate"/>%
                            </font>
                          </xsl:when>
                          <xsl:otherwise>
                            <xsl:value-of select="$totalRate"/>%
                          </xsl:otherwise>
                        </xsl:choose>
                      </td>

                      <xsl:variable name="totalDuration" select="sum(../../../../../../Loops/Loop/Sections/Section[@Number=$sectionNumber]/Tests/Test[$testIndex]/@DurationMilliSeconds)"/>
                      <td class="totalrightcol">
                        <xsl:value-of select="format-number(floor($totalDuration div (60 * 60 * 1000)), '00')"/>
                        <xsl:text>:</xsl:text>
                        <xsl:value-of select="format-number(floor($totalDuration div (60 * 1000)) - (floor($totalDuration div (60 * 60 * 1000)) * 60), '00')"/>
                        <xsl:text>:</xsl:text>
                        <xsl:value-of select="format-number(floor($totalDuration div 1000) - (floor($totalDuration div (60 * 1000)) * 60), '00')"/>
                      </td>

                      <xsl:for-each select="../../../../../../Loops/Loop">
                        <xsl:for-each select="Sections/Section[@Number=$sectionNumber]/Tests/Test">
                          <xsl:variable name="i" select="position()"/>

                          <xsl:if test="$i = $testIndex">
                            <td>
                              <xsl:value-of select="Result/@PassedCount"/>/<xsl:value-of select="Result/@ExpectedCount"/>
                            </td>

                            <xsl:variable name="rate">
                              <xsl:choose>
                                <xsl:when test="Result/@ExpectedCount != 0">
                                  <xsl:value-of select="format-number(((Result/@PassedCount) * 100) div (Result/@ExpectedCount), '0.00')"/>
                                </xsl:when>
                                <xsl:otherwise>
                                  <xsl:text>0</xsl:text>
                                </xsl:otherwise>
                              </xsl:choose>
                            </xsl:variable>

                            <xsl:variable name="logFile">
                              <xsl:for-each select="Logs/Log">
                                  <xsl:choose>
                                    <xsl:when test="contains(@FileName, '.metadata.xml')">
                                    </xsl:when>
                                    <xsl:when test="contains(@FileName, '.trace')">
                                    </xsl:when>
                                    <xsl:when test="contains(@FileName, '.wtl')">
                                      <xsl:value-of select="@FileName"/>
                                    </xsl:when>
                                    <xsl:when test="contains(@FileName, '_tuxnet_')">
                                      <xsl:value-of select="@FileName"/>
                                    </xsl:when>
                                  </xsl:choose>
                                  <!--<xsl:if test="contains(@FileName, '.metadata.xml')">-->
                                    <!--<xsl:value-of select="@FileName"/>-->
                                  <!--</xsl:if>-->
                              </xsl:for-each>
                            </xsl:variable>

                            <td>
                              <xsl:choose>
                                <xsl:when test="$rate &lt; 100">
                                  <xsl:choose>
                                    <xsl:when test="$logFile = ''">
                                      <font color="red">
                                        <xsl:value-of select="$rate"/>%
                                      </font>
                                    </xsl:when>
                                    <xsl:otherwise>
                                    <a>
                                      <xsl:attribute name="href">
                                        <xsl:value-of select="$logFile"/>
                                      </xsl:attribute>
                                      <font color="red">
                                        <xsl:value-of select="$rate"/>%
                                      </font>
                                     </a>
                                    </xsl:otherwise>
                                  </xsl:choose>
                                </xsl:when>
                                <xsl:otherwise>
                                  <xsl:choose>
                                    <xsl:when test="$logFile=''">
                                      <xsl:value-of select="$rate"/>%
                                    </xsl:when>
                                    <xsl:otherwise>
                                      <a>
                                        <xsl:attribute name="href">
                                          <xsl:value-of select="$logFile"/>
                                        </xsl:attribute>
                                        <xsl:value-of select="$rate"/>%
                                      </a>
                                    </xsl:otherwise>
                                  </xsl:choose>
                                </xsl:otherwise>
                              </xsl:choose>
                            </td>

                            <td>
                              <xsl:value-of select="format-number(floor(@DurationMilliSeconds div (60 * 60 * 1000)), '00')"/>
                              <xsl:text>:</xsl:text>
                              <xsl:value-of select="format-number(floor(@DurationMilliSeconds div (60 * 1000)) - (floor(@DurationMilliSeconds div (60 * 60 * 1000)) * 60), '00')"/>
                              <xsl:text>:</xsl:text>
                              <xsl:value-of select="format-number(floor(@DurationMilliSeconds div 1000) - (floor(@DurationMilliSeconds div (60 * 1000)) * 60), '00')"/>
                            </td>
                          </xsl:if>
                        </xsl:for-each>

                      </xsl:for-each>
                    </tr>
                  </xsl:for-each>
                </xsl:for-each>

                <tr>
                  <th>总计</th>

                  <xsl:variable name="grandTotalExpected" select="sum(Loops/Loop/Sections/Section/Tests/Test/Result/@ExpectedCount)"/>
                  <xsl:variable name="grandTotalPassed" select="sum(Loops/Loop/Sections/Section/Tests/Test/Result/@PassedCount)"/>
                  <th class="rightcol">
                    <xsl:value-of select="$grandTotalPassed"/>/<xsl:value-of select="$grandTotalExpected"/>
                  </th>

                  <xsl:variable name="grandTotalRate">
                    <xsl:choose>
                      <xsl:when test="$grandTotalExpected != 0">
                        <xsl:value-of select="format-number(($grandTotalPassed * 100) div $grandTotalExpected, '0.00')"/>
                      </xsl:when>
                      <xsl:otherwise>
                        <xsl:text>0</xsl:text>
                      </xsl:otherwise>
                    </xsl:choose>
                  </xsl:variable>

                  <th class="rightcol">
                    <xsl:choose>
                      <xsl:when test="$grandTotalRate &lt; 99">
                        <font color="red">
                          <xsl:value-of select="$grandTotalRate"/>%
                        </font>
                      </xsl:when>
                      <xsl:otherwise>
                        <xsl:value-of select="$grandTotalRate"/>%
                      </xsl:otherwise>
                    </xsl:choose>
                  </th>

                  <th class="rightcol totalrightcol">
                    <xsl:value-of select="format-number(floor(@DurationMilliSeconds div (60 * 60 * 1000)), '00')"/>
                    <xsl:text>:</xsl:text>
                    <xsl:value-of select="format-number(floor(@DurationMilliSeconds div (60 * 1000)) - (floor(@DurationMilliSeconds div (60 * 60 * 1000)) * 60), '00')"/>
                    <xsl:text>:</xsl:text>
                    <xsl:value-of select="format-number(floor(@DurationMilliSeconds div 1000) - (floor(@DurationMilliSeconds div (60 * 1000)) * 60), '00')"/>
                  </th>

                  <xsl:for-each select="Loops/Loop">

                    <xsl:if test="count(Sections/Section/Tests/Test) != 0">
                      <xsl:variable name="loopTotalExpected" select="sum(Sections/Section/Tests/Test/Result/@ExpectedCount)"/>
                      <xsl:variable name="loopTotalPassed" select="sum(Sections/Section/Tests/Test/Result/@PassedCount)"/>
                      <th class="rightcol">
                        <xsl:value-of select="$loopTotalPassed"/>/<xsl:value-of select="$loopTotalExpected"/>
                      </th>

                      <xsl:variable name="loopTotalRate">
                        <xsl:choose>
                          <xsl:when test="$loopTotalExpected != 0">
                            <xsl:value-of select="format-number(($loopTotalPassed * 100) div $loopTotalExpected, '0.00')"/>
                          </xsl:when>
                          <xsl:otherwise>
                            <xsl:text>0</xsl:text>
                          </xsl:otherwise>
                        </xsl:choose>
                      </xsl:variable>

                      <th class="rightcol">
                        <xsl:choose>
                          <xsl:when test="$loopTotalRate &lt; 99">
                            <font color="red">
                              <xsl:value-of select="$loopTotalRate"/>%
                            </font>
                          </xsl:when>
                          <xsl:otherwise>
                            <xsl:value-of select="$loopTotalRate"/>%
                          </xsl:otherwise>
                        </xsl:choose>
                      </th>

                      <th class="rightcol">
                        <xsl:value-of select="format-number(floor(@DurationMilliSeconds div (60 * 60 * 1000)), '00')"/>
                        <xsl:text>:</xsl:text>
                        <xsl:value-of select="format-number(floor(@DurationMilliSeconds div (60 * 1000)) - (floor(@DurationMilliSeconds div (60 * 60 * 1000)) * 60), '00')"/>
                        <xsl:text>:</xsl:text>
                        <xsl:value-of select="format-number(floor(@DurationMilliSeconds div 1000) - (floor(@DurationMilliSeconds div (60 * 1000)) * 60), '00')"/>
                      </th>
                    </xsl:if>
                  </xsl:for-each>
                </tr>
              </table>
            </div>
            <br/>
            <br/>
            <br/>
          </xsl:for-each>
        </xsl:for-each>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
