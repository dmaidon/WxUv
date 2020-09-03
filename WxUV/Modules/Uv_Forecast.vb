Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Text
Imports WxUV.Models.Forecast

Namespace Modules
    Friend Module UvForecast
        Private _uf As String

        Public Sub GetUvForecast()
            _uf = Path.Combine(TempDir, UvFil)
            If File.Exists(_uf) Then
                FrmMain.RtbLog.AppendText($"Reading cached UV file -> [{_uf}]{vbLf}")
                ParseJson(_uf)
            Else
                DownloadUvForecast()
            End If
        End Sub

        Private Sub CreateForecastGrid()
            With FrmMain.DgvUv
                .Columns.Clear()
                If .Columns.Count <= 0 Then

                    For j = 0 To 7
                        .Columns.Add(New DataGridViewTextBoxColumn)
                        .Columns(j).Name = $"uv{j}"
                    Next

                    For j = 0 To 3
                        .Rows.Add()
                        .Rows(j).Height = 35
                    Next
                    .CellBorderStyle = DataGridViewCellBorderStyle.None
                End If
            End With
        End Sub

        Private Sub DisplayUvForecast()
            Try
                Dim aa As List(Of String)
                Dim ab As Double
                Dim rt = UvNfo.Result
                Dim ut As Date
                If Not File.Exists(_uf) Then
                    Return
                End If
                Dim jj = UvNfo.Result.Length
                FrmMain.RtbLog.AppendText($"UV Update @ {Now}: Count = {jj}{vbLf}")
                Dim sb As New StringBuilder
                With FrmMain.DgvUv
                    For j = 0 To jj - 1
                        ab = Math.Round(rt(j).Uv)
                        ab = If(ab > 11, 11, ab)
                        aa = GetUvLevel(ab)
                        sb.Append(aa.Item(4))
                        sb.Append($"{vbLf}{vbLf}Sun position:{vbLf}Altitude: {UvNfo.Result(j).SunPosition.Altitude}{vbLf}Azimuth: {UvNfo.Result(j).SunPosition.Azimuth}")
                        ut = UvNfo.Result(j).UvTime.ToLocalTime
                        If j <= 7 Then
                            Select Case aa.Item(3)
                                Case "Green"
                                    .Rows(0).Cells(j).Style.BackColor = ColorTranslator.FromHtml("#558b25")
                                Case "Yellow"
                                    .Rows(0).Cells(j).Style.BackColor = ColorTranslator.FromHtml("#f9a825")
                                Case "Orange"
                                    .Rows(0).Cells(j).Style.BackColor = ColorTranslator.FromHtml("#ef6c00")
                                Case "Red"
                                    .Rows(0).Cells(j).Style.BackColor = ColorTranslator.FromHtml("#b71c1c")
                                Case "Purple"
                                    .Rows(0).Cells(j).Style.BackColor = ColorTranslator.FromHtml("#681b9a")
                            End Select
                            .Rows(0).Cells(j).Value = ab
                            .Rows(1).Cells(j).Value = $"{aa.Item(1)}{vbLf}{ut.Hour}"
                            .Rows(0).Cells(j).ToolTipText = sb.ToString
                        Else
                            Select Case aa.Item(3)
                                Case "Green"
                                    .Rows(2).Cells(j - 8).Style.BackColor = ColorTranslator.FromHtml("#558b25")
                                Case "Yellow"
                                    .Rows(2).Cells(j - 8).Style.BackColor = ColorTranslator.FromHtml("#f9a825")
                                Case "Orange"
                                    .Rows(2).Cells(j - 8).Style.BackColor = ColorTranslator.FromHtml("#ef6c00")
                                Case "Red"
                                    .Rows(2).Cells(j - 8).Style.BackColor = ColorTranslator.FromHtml("#b71c1c")
                                Case "Purple"
                                    .Rows(2).Cells(j - 8).Style.BackColor = ColorTranslator.FromHtml("#681b9a")
                            End Select
                            .Rows(2).Cells(j - 8).Value = ab
                            .Rows(3).Cells(j - 8).Value = $"{aa.Item(1)}{vbLf}{ut.Hour}"
                            .Rows(2).Cells(j - 8).ToolTipText = sb.ToString
                        End If
                        FrmMain.RtbLog.AppendText($"Time: {UvNfo.Result(j).UvTime}  Hour: {ut.Hour}   UV: {ab}{vbLf}")
                        sb.Clear()
                    Next

                    .Rows(0).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                    .Rows(0).DefaultCellStyle.Font = New Font("Times New Roman", 20, FontStyle.Bold)
                    .Rows(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                    .Rows(1).DefaultCellStyle.Font = New Font("Arial", 9, FontStyle.Regular)
                    .Rows(2).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                    .Rows(2).DefaultCellStyle.Font = New Font("Times New Roman", 20, FontStyle.Bold)
                    .Rows(3).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                    .Rows(3).DefaultCellStyle.Font = New Font("Arial", 9, FontStyle.Regular)
                    .ClearSelection()
                End With

                FrmMain.LblDate.Text = $"{UvNfo.Result(0).UvTime.ToLocalTime:D}"

                FrmMain.GbEt.Text = $"Safe Exposure [UV: {RtNfo.Result.Uv:N2}]"
                Dim et As String
                Dim mu = " minutes"
                For j = 0 To 5
                    Select Case j + 1
                        Case 1
                            et = RtNfo.Result.SafeExposureTime.St1.ToString(CultureInfo.CurrentCulture)
                        Case 2
                            et = RtNfo.Result.SafeExposureTime.St2.ToString(CultureInfo.CurrentCulture)
                        Case 3
                            et = RtNfo.Result.SafeExposureTime.St3.ToString(CultureInfo.CurrentCulture)
                        Case 4
                            et = RtNfo.Result.SafeExposureTime.St4.ToString(CultureInfo.CurrentCulture)
                        Case 5
                            et = RtNfo.Result.SafeExposureTime.St5.ToString(CultureInfo.CurrentCulture)
                        Case Else
                            et = RtNfo.Result.SafeExposureTime.St6.ToString(CultureInfo.CurrentCulture)
                    End Select
                    If String.IsNullOrEmpty(et) Then
                        et = "N/A"
                        mu = vbNullString
                    End If
                    LblStArr(j).Text = $"Skin Type {j + 1}: {et:N0}{mu}"
                Next
            Catch ex As Exception
                FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbLf}   Location: {ex.TargetSite}{vbLf}   Trace: { ex.StackTrace}{vbLf}")
            Finally
                FrmMain.RtbLog.AppendText($"{My.Resources.separator}{vbLf}")
                SaveLogs()
            End Try
        End Sub

        Private Async Sub DownloadUvForecast()
            OzLevel = KInfo.GetValue(My.Resources.oz, 0).ToString
            ApiKey = KTok.GetValue(My.Resources.key_uv, "").ToString
            If String.IsNullOrEmpty(ApiKey.Trim) Then
                Keyset = False
                FrmMain.TC.SelectedTab = FrmMain.TpSettings
                Return
            Else
                Keyset = True
            End If
            Try
                Dim request = CType _
                    (WebRequest.Create(New Uri($"https://api.openuv.io/api/v1/forecast?lat={CLatitude}&lng={CLongitude}&alt={Altitude}&ozone={OzLevel}")), HttpWebRequest)
                With request
                    .Headers.Add($"x-access-token: {ApiKey}")
                    .AutomaticDecompression = DecompressionMethods.GZip Or DecompressionMethods.Deflate
                    .Accept = "application/json"
                    .Timeout = 120000
                    .Headers.Add("Accept-Encoding", "gzip, deflate")
                    .UserAgent = UseAgent
                End With

                Using response = CType(Await request.GetResponseAsync().ConfigureAwait(True), HttpWebResponse)
                    FrmMain.RtbLog.AppendText($"{response.StatusCode}{vbLf}{response.StatusDescription}{vbLf}{vbLf}")
                    Dim dStr = response.GetResponseStream()
                    Using reader As New StreamReader(dStr)
                        Dim resp = Await reader.ReadToEndAsync().ConfigureAwait(True)
                        FrmMain.RtbLog.AppendText(resp & vbLf & vbLf)
                        File.WriteAllText(_uf, resp)
                        UvNfo = UvFcast.FromJson(resp)
                        CreateForecastGrid()
                        DisplayUvForecast()
                    End Using
                End Using
                FrmMain.RtbLog.AppendText($"-{Now:t}- Downloaded UV Forecast -> [{_uf}]{vbLf}")
            Catch ex As Exception
                FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbLf}   Location: {ex.TargetSite}{vbLf}   Trace: { ex.StackTrace}{vbLf}")
            Finally
                FrmMain.RtbLog.AppendText($"{My.Resources.separator}{vbLf}")
                SaveLogs()
            End Try
        End Sub

        Private Async Sub ParseJson(fn As String)
            Try
                Using reader As New StreamReader(fn)
                    Dim resp = Await reader.ReadToEndAsync().ConfigureAwait(True)
                    UvNfo = UvFcast.FromJson(resp)
                    CreateForecastGrid()
                    DisplayUvForecast()
                End Using
                FrmMain.RtbLog.AppendText($"-{Now:t}- Parsed UV Forecast -> [{fn}]{vbLf}")
            Catch ex As ArgumentOutOfRangeException
                FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbLf}   Location: {ex.TargetSite}{vbLf}   Trace: { ex.StackTrace}{vbLf}")
            Catch ex As Exception
                FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbLf}   Location: {ex.TargetSite}{vbLf}   Trace: { ex.StackTrace}{vbLf}")
            Finally
                FrmMain.RtbLog.AppendText($"{My.Resources.separator}{vbLf}")
            End Try
            SaveLogs()
        End Sub
    End Module
End Namespace