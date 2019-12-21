﻿Imports System.IO
Imports System.Net
Imports Microsoft.Win32
Imports WxUV.Models.ElevationAPI
Imports WxUV.Models.Google

Namespace Modules
    Friend Module Elevations
        Private _gNfo As Goog
        Private _eNfo As ElevationData

        ''' <summary>
        '''     Google Maps Elevation API go to http://developers.google.com and create credentials for a Google Maps Elevation API
        '''     Key.ue You will need to enter this information
        '''     Elevation-API - https://elevation-api.io/ on the Settings tab
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

        Private Async Sub DownloadGoogleElevation()
            Dim googleKey = KTok.GetValue(My.Resources.key_goog, "")
            Dim ue = Path.Combine(TempDir, GElev)
            If googleKey.Trim() = $"" Then
                ''if we don't have the Google Elevation key set, exit this sub routine
                FrmMain.RtbLog.AppendText($"Google Elevation API key not set -> Elevation not set.{vbLf}")
                FrmMain.TC.SelectedTab = FrmMain.TpSettings
                Return
            Else
                Try
                    Dim request = CType _
                        (WebRequest.Create(New Uri($"https://maps.googleapis.com/maps/api/elevation/json?locations={CLatitude},{CLongitude}&key={googleKey}")), HttpWebRequest)
                    With request
                        .AutomaticDecompression = DecompressionMethods.GZip Or DecompressionMethods.Deflate
                        .Accept = "application/json"
                        .Timeout = 120000
                        .Headers.Add("Accept-Encoding", "gzip, deflate")
                        .UserAgent = UseAgent
                    End With
                    Using response = CType(Await request.GetResponseAsync(), HttpWebResponse)
                        FrmMain.RtbLog.AppendText($"{response.StatusCode}{vbLf}{response.StatusDescription}{vbLf}{vbLf}")
                        Dim dStr = response.GetResponseStream()
                        Using reader As New StreamReader(dStr)
                            Dim resp = Await reader.ReadToEndAsync()
                            FrmMain.RtbLog.AppendText(resp & vbLf & vbLf)
                            File.WriteAllText(ue, resp)
                            _gNfo = Goog.FromJson(resp)
                            KInfo.SetValue(My.Resources.alt, $"{_gNfo.Results(0).Elevation:N0}", RegistryValueKind.DWord)
                        End Using
                    End Using
                    FrmMain.RtbLog.AppendText($"-{Now:t}- Downloaded Google Elevation file -> [{ue}]{vbLf}")
                    FrmMain.RtbLog.AppendText($"Elevation: {_gNfo.Results(0).Elevation:N6}{vbLf}{vbLf}")
                Catch ex As Exception
                    FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbLf}   Location: {ex.TargetSite.ToString}{vbLf}   Trace: { ex.StackTrace.ToString}{vbLf}")
                Finally
                    FrmMain.RtbLog.AppendText($"{My.Resources.separator}{vbLf}")
                    SaveLogs()
                End Try
            End If
        End Sub

        Private Async Sub DownloadElevationApi()
            Dim eKey = KTok.GetValue(My.Resources.key_elev, "")
            Dim ue = Path.Combine(TempDir, GElev)
            If eKey.Trim() = $"" Then
                ''if we don't have the Elevation-API key set, exit this sub routine
                FrmMain.RtbLog.AppendText($"Elevation-API key not set -> Elevation not set.{vbLf}")
                FrmMain.TC.SelectedTab = FrmMain.TpSettings
                Return
            Else
                Try
                    Dim request = CType(WebRequest.Create(New Uri($"https://elevation-api.io/api/elevation?points=({CLatitude},{CLongitude})&key={eKey}")), HttpWebRequest)
                    With request
                        .AutomaticDecompression = DecompressionMethods.GZip Or DecompressionMethods.Deflate
                        .Accept = "application/json"
                        .Timeout = 120000
                        .Headers.Add("Accept-Encoding", "gzip, deflate")
                        .UserAgent = UseAgent
                    End With
                    Using response = CType(Await request.GetResponseAsync(), HttpWebResponse)
                        FrmMain.RtbLog.AppendText($"{response.StatusCode}{vbLf}{response.StatusDescription}{vbLf}{vbLf}")
                        Dim dStr = response.GetResponseStream()
                        Using reader As New StreamReader(dStr)
                            Dim resp = Await reader.ReadToEndAsync()
                            FrmMain.RtbLog.AppendText(resp & vbLf & vbLf)
                            File.WriteAllText(ue, resp)
                            _eNfo = ElevationData.FromJson(resp)
                            KInfo.SetValue(My.Resources.alt, $"{_eNfo.Elevations(0).ElevationElevation:N0}", RegistryValueKind.DWord)
                        End Using
                    End Using
                    FrmMain.RtbLog.AppendText($"-{Now:t}- Downloaded Elevation-API file -> [{ue}]{vbLf}")
                    FrmMain.RtbLog.AppendText($"Elevation: {_eNfo.Elevations(0).ElevationElevation:N6}{vbLf}{vbLf}")
                Catch ex As Exception
                    FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbLf}   Location: {ex.TargetSite.ToString}{vbLf}   Trace: { ex.StackTrace.ToString}{vbLf}")
                Finally
                    FrmMain.RtbLog.AppendText($"{My.Resources.separator}{vbLf}")
                    SaveLogs()
                End Try
            End If
        End Sub

    End Module
End Namespace