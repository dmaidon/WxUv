Imports System.IO
Imports System.Net
Imports Microsoft.Win32
Imports WxUV.ElevationAPI
Imports WxUV.Google

Namespace Modules
    Friend Module Elevations

        Private _gNfo As Goog
        Private _eNfo As ElevationData

        ''' <summary>
        ''' Google Maps Elevation API go to http://developers.google.com and create credentials for a Google Maps Elevation API Key.ue You will need to enter this information
        ''' Elevation-API - https://elevation-api.io/ on the Settings tab
        ''' </summary>
        Public Sub GetElevation()
            'Use the service which is checked on the "Settings" tab.
            Select Case KSet.GetValue(My.Resources.elev_toggle, 1)
                Case 0
                    DownloadGoogleElevation()
                Case 1
                    DownloadElevationApi()
            End Select

        End Sub

        Private Sub DownloadGoogleElevation()
            Dim googleKey = KTok.GetValue(My.Resources.key_goog, "")
            Dim ue = Path.Combine(TempPath, GElev)
            If googleKey.Trim() = $"" Then
                ''if we don't have the Google Elevation key set, exit this sub routine
                FrmMain.RtbLog.AppendText($"Google Elevation API key not set -> Elevation not set.{vbLf}")
                FrmMain.TC.SelectedTab = FrmMain.TpSettings
                Return
            Else
                Try
                    Dim request = CType(WebRequest.Create(New Uri($"https://maps.googleapis.com/maps/api/elevation/json?locations={CLatitude},{CLongitude}&key={googleKey}")),
                                        HttpWebRequest)
                    With request
                        .AutomaticDecompression = DecompressionMethods.GZip Or DecompressionMethods.Deflate
                        .Accept = "application/json"
                        .Timeout = 120000
                        .Headers.Add("Accept-Encoding", "gzip, deflate")
                        .UserAgent = USE_AGENT
                    End With
                    Using response = CType(request.GetResponse(), HttpWebResponse)
                        FrmMain.RtbLog.AppendText($"{response.StatusCode}{vbCrLf}{response.StatusDescription}{vbCrLf}{vbCrLf}")
                        Dim dStr = response.GetResponseStream()
                        Using reader As New StreamReader(dStr)
                            Dim resp = reader.ReadToEnd()
                            FrmMain.RtbLog.AppendText(resp & vbCrLf & vbCrLf)
                            File.WriteAllText(ue, resp)
                            _gNfo = Goog.FromJson(resp)
                            KInfo.SetValue(My.Resources.alt, $"{_gNfo.Results(0).Elevation:N0}", RegistryValueKind.DWord)
                        End Using
                    End Using
                    FrmMain.RtbLog.AppendText($"-{Now:t}- Downloaded Google Elevation file -> [{ue}]{vbLf}")
                    FrmMain.RtbLog.AppendText($"Elevation: {_gNfo.Results(0).Elevation:N6}{vbCrLf}{vbCrLf}")
                Catch ex As Exception
                    FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbCrLf}   Location: {ex.TargetSite.ToString}{vbCrLf}   Trace: { ex.StackTrace.ToString}{vbCrLf}")
                Finally
                    FrmMain.RtbLog.AppendText($"{SQUIGGLEY}{vbCrLf}")
                    SaveLogs()
                End Try
            End If

        End Sub

        Private Sub DownloadElevationApi()
            Dim eKey = KTok.GetValue(My.Resources.key_elev, "")
            Dim ue = Path.Combine(TempPath, GElev)
            If eKey.Trim() = $"" Then
                ''if we don't have the Elevation-API key set, exit this sub routine
                FrmMain.RtbLog.AppendText($"Elevation-API key not set -> Elevation not set.{vbLf}")
                FrmMain.TC.SelectedTab = FrmMain.TpSettings
                Return
            Else
                Try
                    Dim request = CType(WebRequest.Create(New Uri($"https://elevation-api.io/api/elevation?points=({CLatitude},{CLongitude})&key={eKey}")),
                                        HttpWebRequest)
                    With request
                        .AutomaticDecompression = DecompressionMethods.GZip Or DecompressionMethods.Deflate
                        .Accept = "application/json"
                        .Timeout = 120000
                        .Headers.Add("Accept-Encoding", "gzip, deflate")
                        .UserAgent = USE_AGENT
                    End With
                    Using response = CType(request.GetResponse(), HttpWebResponse)
                        FrmMain.RtbLog.AppendText($"{response.StatusCode}{vbCrLf}{response.StatusDescription}{vbCrLf}{vbCrLf}")
                        Dim dStr = response.GetResponseStream()
                        Using reader As New StreamReader(dStr)
                            Dim resp = reader.ReadToEnd()
                            FrmMain.RtbLog.AppendText(resp & vbCrLf & vbCrLf)
                            File.WriteAllText(ue, resp)
                            _eNfo = ElevationData.FromJson(resp)
                            KInfo.SetValue(My.Resources.alt, $"{_eNfo.Elevations(0).ElevationElevation:N0}", RegistryValueKind.DWord)
                        End Using
                    End Using
                    FrmMain.RtbLog.AppendText($"-{Now:t}- Downloaded Elevation-API file -> [{ue}]{vbLf}")
                    FrmMain.RtbLog.AppendText($"Elevation: {_eNfo.Elevations(0).ElevationElevation:N6}{vbCrLf}{vbCrLf}")
                Catch ex As Exception
                    FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbCrLf}   Location: {ex.TargetSite.ToString}{vbCrLf}   Trace: { ex.StackTrace.ToString}{vbCrLf}")
                Finally
                    FrmMain.RtbLog.AppendText($"{SQUIGGLEY}{vbCrLf}")
                    SaveLogs()
                End Try
            End If
        End Sub

    End Module
End Namespace