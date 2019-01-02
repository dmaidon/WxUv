Imports System.IO
Imports System.Net
Imports System.Text
Imports WxUV.Forecast

Namespace Modules
    Module UvForecast
        Private _uf As String

        Public Sub GetUvForecast()
            _uf = Path.Combine(TempPath, UvFil)
            If File.Exists(_uf) Then
                FrmMain.RtbLog.AppendText($"Reading cached UV file -> [{_uf}]{vbCrLf}")
                ParseJson(_uf)
            Else
                DownloadUvForecast()
            End If
            DisplayUvForecast()

        End Sub

        Private Sub DisplayUvForecast()
            Try
                Dim aa As List(Of String)
                Dim rr = File.ReadAllText(_uf)
                Dim jj = CountString(rr, "uv_time")
                FrmMain.LblDate.Text = $"{UvNfo.result(0).uvtime.tolocaltime:D}"
                For j = 0 To jj - 1
                    Dim ab = UvNfo.result(j).uv
                    Dim tm = (UvNfo.result(j).uvtime).tolocaltime
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

                    Dim tmpImg = Path.Combine(Application.StartupPath, "Images", $"{UvNfo.result(j).uv:N0}.png")
                    UvArr(j).Image = Image.FromFile(tmpImg)
                    UvArr(j).BorderStyle = BorderStyle.FixedSingle
                    LblArr(j).Text = $"{aa.Item(1)}{vbCrLf}{(UvNfo.result(j).uvtime).tolocaltime.hour}{vbCrLf}{""}"

                    ''add the sun altitude and azimuth to tooltip popup.
                    Dim abc As New StringBuilder(aa.Item(4))
                    abc.Append($"{vbCrLf}{vbCrLf}Sun position:{vbCrLf}Altitude: {UvNfo.result(j).sunposition.altitude}{vbCrLf}Azimuth: {UvNfo.result(j).sunposition.azimuth}")
                    FrmMain.TTip.SetToolTip(UvArr(j), abc.ToString())

                    ''uncomment the line below and comment the 3 lines above to remove the altitude and azimuth from the hourly tool tips.
                    'FrmMain.TTip.SetToolTip(UvArr(j), aa.Item(4))

                    FrmMain.RtbDebug.AppendText(
                        $"{ab} @ {tm}:{vbCrLf}     Level name: {aa.Item(0)}{vbCrLf}     Level short name: {aa.Item(1)}{vbCrLf}     Color id: {aa.Item(2)}{vbCrLf}     Color name: { _
                                                   aa.Item(3)}{vbCrLf}{vbCrLf}")
                    Application.DoEvents()
                Next j

                FrmMain.GbEt.Text = $"Safe Exposure [UV: {RtNfo.result.uv:N2}]"
                Dim et As String
                Dim mu = " minutes"
                For j = 0 To 5
                    Select Case j + 1
                        Case 1
                            et = RtNfo.result.safe_exposure_time.st1
                        Case 2
                            et = RtNfo.result.safe_exposure_time.st2
                        Case 3
                            et = RtNfo.result.safe_exposure_time.st3
                        Case 4
                            et = RtNfo.result.safe_exposure_time.st4
                        Case 5
                            et = RtNfo.result.safe_exposure_time.st5
                        Case Else
                            et = RtNfo.result.safe_exposure_time.st6
                    End Select
                    If et = vbNullString Then
                        et = "N/A"
                        mu = vbNullString
                    End If
                    LblStArr(j).Text = $"Skin Type {j + 1}: {et:N0}{mu}"
                Next
            Catch ex As Exception
                FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbCrLf}   Location: {ex.TargetSite.ToString}{vbCrLf}   Trace: { ex.StackTrace.ToString}{vbCrLf}")
            Finally
                FrmMain.RtbLog.AppendText($"{Separator}{vbCrLf}")
                SaveLogs()
            End Try
        End Sub

        Private Sub DownloadUvForecast()
            OzLevel = KInfo.GetValue("Ozone", 0)
            ApiKey = Kuv.GetValue("Key", "")
            Try
                Dim request = CType(WebRequest.Create($"https://api.openuv.io/api/v1/forecast?lat={CLatitude}&lng={CLongitude}&alt={Altitude}&ozone={OzLevel}"), HttpWebRequest)
                request.Headers.Add($"x-access-token: {ApiKey}")

                Dim response = CType(request.GetResponse(), HttpWebResponse)
                FrmMain.RtbDebug.AppendText(response.StatusCode & vbCrLf)
                FrmMain.RtbDebug.AppendText(response.StatusDescription & vbCrLf & vbCrLf)
                Dim dStr = response.GetResponseStream()
                Dim reader As New StreamReader(dStr)
                Dim resp = reader.ReadToEnd()
                FrmMain.RtbDebug.AppendText(resp & vbCrLf & vbCrLf)
                File.WriteAllText(_uf, resp)
                UvNfo = UvFcast.FromJson(resp)
                reader.Close()
                response.Close()
                FrmMain.RtbLog.AppendText($"-{Now:t}- Downloaded UV Forecast -> [{_uf}]{vbCrLf}")
            Catch ex As Exception
                FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbCrLf}   Location: {ex.TargetSite.ToString}{vbCrLf}   Trace: { ex.StackTrace.ToString}{vbCrLf}")
            Finally
                FrmMain.RtbLog.AppendText($"{Separator}{vbCrLf}")
                SaveLogs()
            End Try

        End Sub

        Private Sub ParseJson(fn As String)
            FrmMain.RtbDebug.AppendText($"Reading from cached file. {vbCrLf}")

            Try
                Dim reader As New StreamReader(fn)
                Dim resp = reader.ReadToEnd()
                UvNfo = UvFcast.FromJson(resp)
                reader.Close()
                FrmMain.RtbLog.AppendText($"-{Now:t}- Parsed UV Forecast -> [{fn}]{vbCrLf}")
            Catch ex As Exception
                FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbCrLf}   Location: {ex.TargetSite.ToString}{vbCrLf}   Trace: { ex.StackTrace.ToString}{vbCrLf}")
            Finally
                FrmMain.RtbLog.AppendText($"{Separator}{vbCrLf}")
            End Try
            SaveLogs()
        End Sub

    End Module
End Namespace