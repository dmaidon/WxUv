Imports System.IO
Imports System.Net
Imports System.Text
Imports WxUV.Forecast

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

        Private Sub DisplayUvForecast()
            Try
                Dim aa As List(Of String)
                Dim rr As String
                If File.Exists(_uf) Then
                    rr = File.ReadAllText(_uf)
                Else
                    Return
                End If
                Dim jj = CountString(rr, "uv_time")
                FrmMain.LblDate.Text = $"{UvNfo.Result(0).UvTime.ToLocalTime:D}"
                For j = 0 To jj - 1
                    Dim ab = UvNfo.Result(j).Uv
                    Dim tm = (UvNfo.Result(j).UvTime).ToLocalTime
                    aa = GetUvLevel(ab)
                    Select Case aa.Item(3)
                        Case "Green"
                            'UvArr(j).BackColor = Color.Green
                            UvArr(j).BackColor = Color.FromArgb(85, 139, 47)        ''#558b25
                        Case "Yellow"
                            'UvArr(j).BackColor = Color.Yellow
                            UvArr(j).BackColor = Color.FromArgb(247, 228, 0)        ''#f9a825
                        Case "Orange"
                            'UvArr(j).BackColor = Color.Orange
                            UvArr(j).BackColor = Color.FromArgb(249, 108, 0)        ''#ef6c00
                        Case "Red"
                            'UvArr(j).BackColor = Color.Firebrick
                            UvArr(j).BackColor = Color.FromArgb(183, 28, 28)        ''#b71c1c
                        Case "Purple"
                            'UvArr(j).BackColor = Color.Purple
                            UvArr(j).BackColor = Color.FromArgb(106, 27, 154)        ''#681b9a
                    End Select

                    Dim tmpImg = Path.Combine(Application.StartupPath, "Images", $"{UvNfo.Result(j).Uv:N0}.png")
                    UvArr(j).Image = Image.FromFile(tmpImg)
                    UvArr(j).BorderStyle = BorderStyle.FixedSingle
                    LblArr(j).Text = $"{aa.Item(1)}{vbLf}{(UvNfo.Result(j).UvTime).ToLocalTime.Hour}{vbLf}{""}"

                    ''add the sun altitude and azimuth to tooltip popup.
                    Dim abc As New StringBuilder(aa.Item(4))
                    abc.Append($"{vbLf}{vbLf}Sun position:{vbLf}Altitude: {UvNfo.Result(j).SunPosition.Altitude}{vbLf}Azimuth: {UvNfo.Result(j).SunPosition.Azimuth}")
                    FrmMain.TTip.SetToolTip(UvArr(j), abc.ToString())

                    ''uncomment the line below and comment the 3 lines above to remove the altitude and azimuth from the hourly tool tips.
                    'FrmMain.TTip.SetToolTip(UvArr(j), aa.Item(4))

                    FrmMain.RtbLog.AppendText _
                        ($"{ab} @ {tm}:{vbLf}     Level name: {aa.Item(0)}{vbLf}     Level short name: {aa.Item(1)}{vbLf}     Color id: {aa.Item(2)}{vbLf _
                            }     Color name: { _
                            aa.Item(3)}{vbLf}{vbLf}")
                    Application.DoEvents()
                Next j

                FrmMain.GbEt.Text = $"Safe Exposure [UV: {RtNfo.Result.Uv:N2}]"
                Dim et As String
                Dim mu = " minutes"
                For j = 0 To 5
                    Select Case j + 1
                        Case 1
                            et = RtNfo.Result.SafeExposureTime.St1
                        Case 2
                            et = RtNfo.Result.SafeExposureTime.St2
                        Case 3
                            et = RtNfo.Result.SafeExposureTime.St3
                        Case 4
                            et = RtNfo.Result.SafeExposureTime.St4
                        Case 5
                            et = RtNfo.Result.SafeExposureTime.St5
                        Case Else
                            et = RtNfo.Result.SafeExposureTime.St6
                    End Select
                    If et = vbNullString Then
                        et = "N/A"
                        mu = vbNullString
                    End If
                    LblStArr(j).Text = $"Skin Type {j + 1}: {et:N0}{mu}"
                Next
            Catch ex As Exception
                FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbLf}   Location: {ex.TargetSite.ToString}{vbLf}   Trace: { ex.StackTrace.ToString}{vbLf}")
            Finally
                FrmMain.RtbLog.AppendText($"{My.Resources.separator}{vbLf}")
                SaveLogs()
            End Try
        End Sub

        Private Async Sub DownloadUvForecast()
            OzLevel = KInfo.GetValue(My.Resources.oz, 0)
            ApiKey = KTok.GetValue(My.Resources.key_uv, "")
            If ApiKey.Trim() = "" Then
                Keyset = False
                'MsgBox($"OpenUV API key not entered.{vblf}Please enter key on 'Settings' tab.")
                FrmMain.TC.SelectedTab = FrmMain.TpSettings
                Return
            Else
                Keyset = True
            End If
            Try
                Dim request = CType _
                    (WebRequest.Create($"https://api.openuv.io/api/v1/forecast?lat={CLatitude}&lng={CLongitude}&alt={Altitude}&ozone={OzLevel}"), HttpWebRequest)
                With request
                    .Headers.Add($"x-access-token: {ApiKey}")
                    .AutomaticDecompression = DecompressionMethods.GZip Or DecompressionMethods.Deflate
                    .Accept = "application/json"
                    .Timeout = 120000
                    .Headers.Add("Accept-Encoding", "gzip, deflate")
                    .UserAgent = Use_Agent
                End With

                Using response = CType(Await request.GetResponseAsync(), HttpWebResponse)
                    FrmMain.RtbLog.AppendText($"{response.StatusCode}{vbLf}{response.StatusDescription}{vbLf}{vbLf}")
                    Dim dStr = response.GetResponseStream()
                    Using reader As New StreamReader(dStr)
                        Dim resp = Await reader.ReadToEndAsync()
                        FrmMain.RtbLog.AppendText(resp & vbLf & vbLf)
                        File.WriteAllText(_uf, resp)
                        UvNfo = UvFcast.FromJson(resp)
                        DisplayUvForecast()
                    End Using
                End Using
                FrmMain.RtbLog.AppendText($"-{Now:t}- Downloaded UV Forecast -> [{_uf}]{vbLf}")
            Catch ex As Exception
                FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbLf}   Location: {ex.TargetSite.ToString}{vbLf}   Trace: { ex.StackTrace.ToString}{vbLf}")
            Finally
                FrmMain.RtbLog.AppendText($"{My.Resources.separator}{vbLf}")
                SaveLogs()
            End Try
        End Sub

        Private Async Sub ParseJson(fn As String)
            Try
                Using reader As New StreamReader(fn)
                    Dim resp = Await reader.ReadToEndAsync()
                    UvNfo = UvFcast.FromJson(resp)
                    DisplayUvForecast()
                End Using
                FrmMain.RtbLog.AppendText($"-{Now:t}- Parsed UV Forecast -> [{fn}]{vbLf}")
            Catch ex As Exception
                FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbLf}   Location: {ex.TargetSite.ToString}{vbLf}   Trace: { ex.StackTrace.ToString}{vbLf}")
            Finally
                FrmMain.RtbLog.AppendText($"{My.Resources.separator}{vbLf}")
            End Try
            SaveLogs()
        End Sub

    End Module
End Namespace