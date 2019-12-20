Imports System.IO
Imports System.Net
Imports System.Text
Imports Microsoft.Win32
Imports WxUV.RealTime

Namespace Modules
    Module UvRealTime
        Private _urt

        Public Sub GetUvRealTime()
            _urt = Path.Combine(TempPath, RtFil)
            If File.Exists(_urt) Then
                FrmMain.RtbLog.AppendText($"Reading cached Real-time UV file -> [{_urt}]{vbCrLf}")
                ParseRtJson(_urt)
                DisplayInfo()
            Else
                DownloadUvRealTime()
            End If
        End Sub

        Private Async Sub DownloadUvRealTime()
            ''https://api.openuv.io/api/v1/uv?lat=-33.34&lng=115.342&dt=2018-01-24T10:50:52.283Z
            ''date "&dt=" defaults to current time
            Dim dt = $"{Now:O}"
            ApiKey = KTok.GetValue(My.Resources.key_uv, "")
            If ApiKey.Trim() = "" Then
                Keyset = False
                'MsgBox($"OpenUV API key not entered.{vbCrLf}Please enter key on 'Settings' tab.")
                FrmMain.TC.SelectedTab = FrmMain.TpSettings
                Return
            Else
                Keyset = True
            End If
            ''2018-01-24T10:50:52.283Z
            FrmMain.RtbLog.AppendText($"Time: {Now:G}     UTC: {Now.ToUniversalTime():O}{vbCrLf}")
            Try
                Dim request = CType(WebRequest.Create($"https://api.openuv.io/api/v1/uv?lat={CLatitude}&lng={CLongitude}&alt={Altitude}&dt={dt}"), HttpWebRequest)
                With request
                    .Headers.Add($"x-access-token: {ApiKey}")
                    .AutomaticDecompression = DecompressionMethods.GZip Or DecompressionMethods.Deflate
                    .Accept = "application/json"
                    .Timeout = 120000
                    .Headers.Add("Accept-Encoding", "gzip, deflate")
                    .UserAgent = Use_Agent
                End With

                Using response = CType(request.GetResponse(), HttpWebResponse)
                    FrmMain.RtbLog.AppendText($"{response.StatusCode}{vbCrLf}{response.StatusDescription}{vbCrLf}{vbCrLf}")
                    Dim dStr = response.GetResponseStream()
                    Using reader As New StreamReader(dStr)
                        Dim resp = Await reader.ReadToEndAsync()
                        FrmMain.RtbLog.AppendText($"{resp}{vbCrLf}{vbCrLf}")
                        File.WriteAllText(_urt, resp)
                        RtNfo = UvRtCast.FromJson(resp)
                    End Using
                    Dim tmpImg = Path.Combine(Application.StartupPath, "Images", $"{RtNfo.Result.Uv:N0}.png")
                    FrmMain.RtbLog.AppendText($"Set Image: {tmpImg}{vbCrLf}")
                    FrmMain.PbUV.Image = Image.FromFile(tmpImg)

                    ''set ozone level in registry to UV Forecast
                    KInfo.SetValue(My.Resources.oz, $"{RtNfo.Result.Ozone:N0}", RegistryValueKind.String)
                    FrmMain.LblCurOzone.Text = $"Ozone: {RtNfo.Result.Ozone:N0}"

                    Dim ab = RtNfo.Result.Uv
                    Dim aa = GetUvLevel(ab)
                    Select Case aa.Item(3)
                        Case "Green"
                            'UvArr(j).BackColor = Color.Green
                            FrmMain.PbUV.BackColor = Color.FromArgb(85, 139, 47)        ''#558b25
                        Case "Yellow"
                            'UvArr(j).BackColor = Color.Yellow
                            FrmMain.PbUV.BackColor = Color.FromArgb(247, 228, 0)        ''#f9a825
                        Case "Orange"
                            'UvArr(j).BackColor = Color.Orange
                            FrmMain.PbUV.BackColor = Color.FromArgb(249, 108, 0)        ''#ef6c00
                        Case "Red"
                            'UvArr(j).BackColor = Color.Firebrick
                            FrmMain.PbUV.BackColor = Color.FromArgb(183, 28, 28)        ''#b71c1c
                        Case "Purple"
                            'UvArr(j).BackColor = Color.Purple
                            FrmMain.PbUV.BackColor = Color.FromArgb(106, 27, 154)        ''#681b9a
                    End Select

                    Dim sunrise = RtNfo.Result.SunInfo.SunTimes.Sunrise   '.tolocaltime5
                    'MsgBox(Date2Unix(RtNfo.result.suninfo.suntimes.sunrise))
                    Dim sunset = RtNfo.Result.SunInfo.SunTimes.Sunset '.tolocaltime
                    Dim a = Date2Unix(RtNfo.Result.SunInfo.SunTimes.Sunrise)
                    Dim b = Date2Unix(RtNfo.Result.SunInfo.SunTimes.Sunset)
                    FrmMain.LblDayLen.Text = DisplayDayLen(TimeSpan.FromSeconds(b - a))
                    'Dim dSpan = TimeSpan.FromSeconds(b - a)
                    'FrmMain.LblDayLen.Text = $"LoD: {dSpan.Hours:N0} hrs. {dSpan.Minutes:N0} min. {dSpan.Seconds} sec."

                    Dim t = RtNfo.Result.SunInfo.SunTimes.Sunrise
                    Dim x = RtNfo.Result.SunInfo.SunTimes.Sunset
                    t = t.ToLocalTime.ToString
                    x = x.ToLocalTime.ToString
                    Dim dttSunrise As Date = t.ToShortTimeString()
                    Dim dttSunset As Date = x.ToShortTimeString()
                    If Now.IsDaylightSavingTime() Then
                        sunrise = sunrise.Add(Subduration)
                        sunset = sunset.Add(Subduration)
                        FrmMain.RtbLog.AppendText($"Daylight Savings Time calculated.{vbCrLf}")
                    End If

                    FrmMain.RtbLog.AppendText($"Now: {Now.ToLongTimeString()}     Sunrise: {sunrise}     Sunset: {sunset}{vbCrLf}")
                    Daylight = My.Computer.Clock.LocalTime.ToShortTimeString >= dttSunrise AndAlso My.Computer.Clock.LocalTime.ToShortTimeString <= dttSunset
                    FrmMain.RtbLog.AppendText($"Sunrise: {sunrise}     Daylight = {Daylight}{vbCrLf}{Separator}{vbCrLf}")
                    DisplayInfo()
                    FrmMain.RtbLog.AppendText($"-{Now:t}- Downloaded Real-time UV Forecast -> [{_urt}]{vbCrLf}")
                    FrmMain.RtbLog.AppendText _
                        ($"Time: {(RtNfo.Result.UvTime).ToLocalTime}{vbCrLf}UV Level: {RtNfo.Result.Uv}{vbCrLf}UV Maximum Time: {RtNfo.Result.UvMaxTime}{vbCrLf _
                            }UV Maximum: {RtNfo.Result.UvMax}{vbCrLf}Ozone: {RtNfo.Result.Ozone}{vbCrLf}Ozone Time: {(RtNfo.Result.OzoneTime).ToLocalTime _
                            }{vbCrLf}{vbCrLf}")
                End Using
            Catch ex As NotSupportedException
                FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbCrLf}   Location: {ex.TargetSite.ToString}{vbCrLf}   Trace: { ex.StackTrace.ToString}{vbCrLf}")
            Catch ex As Exception
                FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbCrLf}   Location: {ex.TargetSite.ToString}{vbCrLf}   Trace: { ex.StackTrace.ToString}{vbCrLf}")
            Finally
                ''
            End Try
            FrmMain.RtbLog.AppendText($"{Squiggley}{vbCrLf}")
            SaveLogs()
        End Sub

        Private Function DisplayDayLen(a As TimeSpan) As String
            Return $"LoD: {a.Hours:N0} hrs. {a.Minutes:N0} min. {a.Seconds} sec."
        End Function

        Private Sub DisplayInfo()
            Dim rtt = RtNfo.Result.SunInfo.SunTimes
            With FrmMain
                Dim sb As New StringBuilder($"Sunrise: {rtt.Sunrise.ToLocalTime:T}{vbLf}")
                sb.Append($"Sunset: {rtt.Sunset.ToLocalTime.Add(Subduration):T}{vbLf}")
                sb.Append($"Solar Noon: {rtt.SolarNoon.ToLocalTime.Add(Subduration):T}{vbLf}")
                .LblSunrise.Text = sb.ToString
                sb.Clear()

                sb.Append($"Sunrise: {rtt.Sunrise.ToLocalTime:T}{vbLf}")
                sb.Append($"Sunset: {rtt.Sunset.ToLocalTime.Add(Subduration):T}{vbLf}")
                sb.Append($"Solar Noon: {rtt.SolarNoon.ToLocalTime.Add(Subduration):T}{vbLf}")
                sb.Append($"Sunrise End: {rtt.SunriseEnd.ToLocalTime.Add(Subduration):t}{vbLf}")
                sb.Append($"Sunset Start: {rtt.SunsetStart.ToLocalTime.Add(Subduration):t}{vbLf}")
                sb.Append($"Dawn: {rtt.Dawn.ToLocalTime.Add(Subduration):t}{vbLf}")
                sb.Append($"Dusk: {rtt.Dusk.ToLocalTime.Add(Subduration):t}")
                .LblSi1.Text = sb.ToString
                sb.Clear()

                sb.Append($"Nautical Dawn: {rtt.NauticalDawn.ToLocalTime.Add(Subduration):t}{vbLf}")
                sb.Append($"Nautical Dusk: {rtt.NauticalDusk.ToLocalTime.Add(Subduration):t}{vbLf}")
                sb.Append($"Night Begins: {rtt.Night.ToLocalTime.Add(Subduration):t}{vbLf}")
                sb.Append($"Night Ends: {rtt.NightEnd.ToLocalTime.Add(Subduration):t}{vbLf}")
                sb.Append($"Night Darkest: {rtt.NaDir.ToLocalTime.Add(Subduration):t}{vbLf}")
                sb.Append($"Golden Hour Start: {rtt.GoldenHour.ToLocalTime.Add(Subduration):t}{vbLf}")
                sb.Append($"Golden Hour End: {rtt.GoldenHourEnd.ToLocalTime.Add(Subduration):t}")
                .LblSi2.Text = sb.ToString
                sb.Clear()

                .LblSunPos.Text = $"Azimuth: {RtNfo.Result.SunInfo.SunPosition.Azimuth.ToString()}     Altitude: {RtNfo.Result.SunInfo.SunPosition.Altitude.ToString()}"
                Dim a = Date2Unix(RtNfo.Result.SunInfo.SunTimes.Sunrise)
                Dim b = Date2Unix(RtNfo.Result.SunInfo.SunTimes.Sunset)
                FrmMain.LblDayLen.Text = DisplayDayLen(TimeSpan.FromSeconds(b - a))
            End With
        End Sub

        Private Async Sub ParseRtJson(fn As String)
            Try
                Dim reader As New StreamReader(fn)
                Dim resp = Await reader.ReadToEndAsync()
                RtNfo = UvRtCast.FromJson(resp)
                reader.Close()
                FrmMain.RtbLog.AppendText($"-{Now:t}- Parsed UV Forecast -> [{fn}]{vbCrLf}")
            Catch ex As Exception
                FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbCrLf}   Location: {ex.TargetSite.ToString}{vbCrLf}   Trace: { ex.StackTrace.ToString}{vbCrLf}")
            Finally
                ''
            End Try
            FrmMain.RtbLog.AppendText($"{Separator}{vbCrLf}")
            SaveLogs()
        End Sub

    End Module
End Namespace