Imports System.IO
Imports System.Net
Imports Microsoft.Win32
Imports WxUV.Google

Namespace Modules
    Module Elevations

        ''' <summary>
        ''' Google Maps Elevation API go to http://developers.google.com and create credentials for a Google Maps Elevation API Key.ue You will need to enter this information
        ''' on the Settings tab
        ''' </summary>
        Public Sub GetElevation()
            DownloadElevation()
        End Sub

        Public Sub DownloadElevation()
            GoogleKey = Kg.GetValue("Elevation API Key", "")
            Dim ue = Path.Combine(TempPath, GElev)
            If GoogleKey.Trim() = "" Then
                ''if we don't have the Google Elevation Key set, exit this sub
                FrmMain.RtbLog.AppendText($"Google Elevation API Key not set -> Elevation not set.")
                Return
            Else
                Try
                    Dim request = CType(WebRequest.Create($"https://maps.googleapis.com/maps/api/elevation/json?locations={CLatitude},{CLongitude}&key={GoogleKey}"),
                                        HttpWebRequest)
                    Dim response = CType(request.GetResponse(), HttpWebResponse)
                    FrmMain.RtbDebug.AppendText(response.StatusCode & vbCrLf)
                    FrmMain.RtbDebug.AppendText(response.StatusDescription & vbCrLf & vbCrLf)
                    Dim dStr = response.GetResponseStream()
                    Dim reader As New StreamReader(dStr)
                    Dim resp = reader.ReadToEnd()
                    FrmMain.RtbDebug.AppendText(resp & vbCrLf & vbCrLf)
                    File.WriteAllText(ue, resp)
                    GNfo = Goog.FromJson(resp)
                    reader.Close()
                    KInfo.SetValue("Altitude", $"{GNfo.Results(0).elevation:N0}", RegistryValueKind.DWord)
                    response.Close()
                    FrmMain.RtbLog.AppendText($"-{Now:t}- Downloaded Google Elevation file -> [{ue}]")
                    FrmMain.RtbDebug.AppendText($"Elevation: {GNfo.Results(0).elevation:N6}{vbCrLf}{vbCrLf}")
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