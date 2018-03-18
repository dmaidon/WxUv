Imports System.IO
Imports System.Net
Imports WxUV.Forecast

Namespace Modules
    Module Uv_Forecast
        Private uf As String

        Public Sub GetUVForecast()
            uf = Path.Combine(TempPath, UvFil)
            If File.Exists(uf) Then
                FrmMain.RtbLog.AppendText($"Reading cached UV file -> [{uf}]{vbCrLf}")
                ParseJson(uf)
            Else
                DownloadUVForecast()
            End If
            DisplayUvForecast()

        End Sub

        Private Sub DisplayUvForecast()
            Try
                Dim aa As List(Of String)
                Dim rr = File.ReadAllText(uf)
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

                    Dim _tmpImg = Path.Combine(Application.StartupPath, "Images", $"{UvNfo.result(j).uv:N0}.png")
                    UvArr(j).Image = Image.FromFile(_tmpImg)
                    UvArr(j).BorderStyle = BorderStyle.FixedSingle
                    LblArr(j).Text = $"{aa.Item(1)}{vbCrLf}{(UvNfo.result(j).uvtime).tolocaltime.hour}{vbCrLf}{""}"
                    FrmMain.TTip.SetToolTip(UvArr(j), aa.Item(4))
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

        Private Sub DownloadUVForecast()
            OzLevel = KInfo.GetValue("Ozone", 0)
            ApiKey = Kuv.GetValue("Key", "")
            Try
                Dim Request = CType(WebRequest.Create($"https://api.openuv.io/api/v1/forecast?lat={cLatitude}&lng={cLongitude}&alt={Altitude}&ozone={OzLevel}"), HttpWebRequest)
                Request.Headers.Add($"x-access-token: {ApiKey}")

                Dim Response = CType(Request.GetResponse(), HttpWebResponse)
                FrmMain.RtbDebug.AppendText(Response.StatusCode & vbCrLf)
                FrmMain.RtbDebug.AppendText(Response.StatusDescription & vbCrLf & vbCrLf)
                Dim dStr = Response.GetResponseStream()
                Dim reader As New StreamReader(dStr)
                Dim resp = reader.ReadToEnd()
                FrmMain.RtbDebug.AppendText(resp & vbCrLf & vbCrLf)
                File.WriteAllText(uf, resp)
                UvNfo = UvFcast.FromJson(resp)
                reader.Close()
                Response.Close()
                FrmMain.RtbLog.AppendText($"-{Now:t}- Downloaded UV Forecast -> [{uf}]{vbCrLf}")
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