Imports System.IO
Imports System.Net
Imports Microsoft.Win32
Imports WxUV.RealTime

Namespace Modules
    Module UvRealTime
        Private _urt

        Public Sub GetUvRealTime()
            _urt = Path.Combine(TempPath, RtFil)
            'If File.Exists(urt) Then
            '    FrmMain.RtbLog.AppendText($"Reading cached Real-time UV file -> [{urt}]{vbCrLf}")
            '    ParseJson(urt)
            'Else
            DownloadUvRealTime()
            ' End If

        End Sub

        Private Sub DownloadUvRealTime()
            ''https://api.openuv.io/api/v1/uv?lat=-33.34&lng=115.342&dt=2018-01-24T10:50:52.283Z
            ''date "&dt=" defaults to current time
            'OzLevel = KInfo.GetValue("Ozone", 0)
            Dim dt = $"{Now:O}"
            ApiKey = Kuv.GetValue("Key", "")
            If ApiKey.Trim() = "" Then
                Keyset = False
                'MsgBox($"OpenUV API key not entered.{vbCrLf}Please enter key on 'Settings' tab.")
                FrmMain.TC.SelectedTab = FrmMain.TpSettings
            Else
                Keyset = True
            End If
            ''2018-01-24T10:50:52.283Z
            FrmMain.RtbDebug.AppendText($"Time: {Now:G}     UTC: {Now.ToUniversalTime():O}{vbCrLf}")
            FrmMain.RtbLog.AppendText($"Time: {Now:G}     UTC: {Now.ToUniversalTime():O}{vbCrLf}")
            Try
                ''Dim Request = CType(WebRequest.Create($"https://api.openuv.io/api/v1/uv?lat={cLatitude}&lng={cLongitude}&alt={Altitude}&ozone={OzLevel}&dt={dt}"), HttpWebRequest)
                Dim request = CType(WebRequest.Create($"https://api.openuv.io/api/v1/uv?lat={CLatitude}&lng={CLongitude}&alt={Altitude}&dt={dt}"), HttpWebRequest)
                request.Headers.Add($"x-access-token: {ApiKey}")

                Dim response = CType(request.GetResponse(), HttpWebResponse)
                FrmMain.RtbDebug.AppendText(response.StatusCode & vbCrLf)
                FrmMain.RtbDebug.AppendText(response.StatusDescription & vbCrLf & vbCrLf)
                Dim dStr = response.GetResponseStream()
                Dim reader As New StreamReader(dStr)
                Dim resp = reader.ReadToEnd()
                FrmMain.RtbDebug.AppendText(resp & vbCrLf & vbCrLf)
                File.WriteAllText(_urt, resp)
                RtNfo = UvRtCast.FromJson(resp)
                reader.Close()
                Dim tmpImg = Path.Combine(Application.StartupPath, "Images", $"{RtNfo.result.uv:N0}.png")
                FrmMain.RtbLog.AppendText($"Set Image: {tmpImg}{vbCrLf}")
                FrmMain.PbUV.Image = Image.FromFile(tmpImg)

                ''set ozone level in registry to UV Forecast
                KInfo.SetValue("Ozone", $"{RtNfo.result.Ozone:N0}", RegistryValueKind.String)
                FrmMain.LblCurOzone.Text = $"Ozone: {RtNfo.result.Ozone:N0}"

                Dim ab = RtNfo.result.uv
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

                Dim sunrise As Date = RtNfo.result.sun_info.sun_times.sunrise   '.tolocaltime5
                'MsgBox(Date2Unix(RtNfo.result.sun_info.sun_times.sunrise))
                Dim sunset As Date = RtNfo.result.sun_info.sun_times.sunset '.tolocaltime
                Dim a = Date2Unix(RtNfo.result.sun_info.sun_times.sunrise)
                Dim b = Date2Unix(RtNfo.result.sun_info.sun_times.sunset)
                Dim dSpan = TimeSpan.FromSeconds(b - a)
                FrmMain.LblDayLen.Text = $"LoD: {dSpan.Hours:N0} hrs. {dSpan.Minutes:N0} min. {dSpan.Seconds} sec."
                Dim t As Date = RtNfo.result.sun_info.sun_times.sunrise
                Dim x As Date = RtNfo.result.sun_info.sun_times.sunset
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
                'If Now.ToLongTimeString() >= $"{sunrise}" And Now.ToLongTimeString() <= $"{sunset}" Then
                'MsgBox(My.Computer.Clock.LocalTime.ToShortTimeString & vbCrLf & sunrise.ToLocalTime.ToShortTimeString())
                Daylight = My.Computer.Clock.LocalTime.ToShortTimeString >= dttSunrise AndAlso My.Computer.Clock.LocalTime.ToShortTimeString <= dttSunset
                'MsgBox(Daylight)
                FrmMain.RtbLog.AppendText($"Sunrise: {sunrise}     Daylight = {Daylight}{vbCrLf}")
                FrmMain.RtbLog.AppendText(SEPARATOR & vbCrLf)
                DisplayInfo()

                response.Close()
                FrmMain.RtbLog.AppendText($"-{Now:t}- Downloaded Real-time UV Forecast -> [{_urt}]{vbCrLf}")

                FrmMain.RtbDebug.AppendText(
                    $"Time: {(RtNfo.result.uv_time).tolocaltime}{vbCrLf}UV Level: {RtNfo.result.uv}{vbCrLf}UV Maximum Time: {RtNfo.result.uv_max_time}{vbCrLf _
                                               }UV Maximum: {RtNfo.result.uv_max}{vbCrLf}Ozone: {RtNfo.result.ozone}{vbCrLf}Ozone Time: {(RtNfo.result.ozone_time).tolocaltime _
                                               }{vbCrLf}{vbCrLf}")
            Catch ex As Exception
                FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbCrLf}   Location: {ex.TargetSite.ToString}{vbCrLf}   Trace: { ex.StackTrace.ToString}{vbCrLf}")
            Finally
                FrmMain.RtbLog.AppendText($"{SQUIGGLEY}{vbCrLf}")
                SaveLogs()
            End Try

        End Sub

        Private Sub DisplayInfo()
            With FrmMain
                .LblSunrise.Text = $"Sunrise: {RtNfo.result.sun_info.sun_times.sunrise.tolocaltime:t}"
                .LblSunset.Text = $"Sunset: {RtNfo.result.sun_info.sun_times.sunset.tolocaltime:t}"
                .LblSolarNoon.Text = $"Solar Noon: {RtNfo.result.sun_info.sun_times.solarnoon.tolocaltime:t}"
                '.LblDayLen.Text = $"Length of Day: {RtNfo.result.sun_info.sun_times.daylength}"
                .LblSiSunrise.Text = $"Sunrise: {RtNfo.result.sun_info.sun_times.sunrise.tolocaltime:t}"
                .LblSiSunset.Text = $"Sunset: {RtNfo.result.sun_info.sun_times.sunset.tolocaltime:t}"
                .LblSiSolarNoon.Text = $"Solar Noon: {RtNfo.result.sun_info.sun_times.solarnoon.tolocaltime:t}"
                .LblSiSunriseEnd.Text = $"Sunrise End: {RtNfo.result.sun_info.sun_times.sunriseend.tolocaltime:t}"
                .LblSiSunsetStart.Text = $"Sunset Start: {RtNfo.result.sun_info.sun_times.sunsetstart.tolocaltime:t}"
                .LblSiDawn.Text = $"Dawn: {RtNfo.result.sun_info.sun_times.dawn.tolocaltime:t}"
                .LblSiDusk.Text = $"Dusk: {RtNfo.result.sun_info.sun_times.dusk.tolocaltime:t}"
                .LblSiNautDawn.Text = $"Nautical Dawn: {RtNfo.result.sun_info.sun_times.nauticaldawn.tolocaltime:t}"
                .LblSiNautDusk.Text = $"Nautical Dusk: {RtNfo.result.sun_info.sun_times.nauticaldusk.tolocaltime:t}"
                .LblSiNightStart.Text = $"Night Begins: {RtNfo.result.sun_info.sun_times.night.tolocaltime:t}"
                .LblSiNightEnd.Text = $"Night Ends: {RtNfo.result.sun_info.sun_times.nightend.tolocaltime:t}"
                .LblSiNightDark.Text = $"Night Darkest: {RtNfo.result.sun_info.sun_times.nadir.tolocaltime:t}"
                .LblsiGoldenStart.Text = $"Golden Hour Start: {RtNfo.result.sun_info.sun_times.goldenhour.tolocaltime:t}"
                .LblSiGoldenEnd.Text = $"Golden Hour End: {RtNfo.result.sun_info.sun_times.goldenhourend.tolocaltime:t}"
                .LblSunPos.Text = $"Azimuth: {RtNfo.result.sun_info.sun_position.azimuth.ToString()}     Altitude: {RtNfo.result.sun_info.sun_position.altitude.ToString()}"
            End With
        End Sub

    End Module
End Namespace