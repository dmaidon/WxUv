Imports System.IO
Imports System.Net
Imports Microsoft.Win32
Imports WxUV.Google

Namespace Modules
    Module Elevations
        Private ue As String

        ''' <summary>
        '''     Google Maps Elevation API
        '''     go to http://developers.google.com and create credentials for a Google Maps Elevation API Key.ue  You will need to
        '''     enter this information on the Settings tab
        ''' </summary>
        Public Sub GetElevation()
            ue = Path.Combine(TempPath, GElev)
            DownloadElevation()
        End Sub

        Public Sub DownloadElevation()
            GoogleKey = Kg.GetValue("Elevation API Key", "")

            If GoogleKey.Trim() = "" Then
                ''if we don't have the Google Elevation Key set, exit this sub
                FrmMain.RtbLog.AppendText($"Google Elevation API Key not set -> Elevation not set.")
                Exit Sub
            Else
                Try
                    Dim Request = CType(WebRequest.Create($"https://maps.googleapis.com/maps/api/elevation/json?locations={cLatitude},{cLongitude}&key={GoogleKey}"),
                                        HttpWebRequest)
                    Dim Response = CType(Request.GetResponse(), HttpWebResponse)
                    FrmMain.RtbDebug.AppendText(Response.StatusCode & vbCrLf)
                    FrmMain.RtbDebug.AppendText(Response.StatusDescription & vbCrLf & vbCrLf)
                    Dim dStr = Response.GetResponseStream()
                    Dim reader As New StreamReader(dStr)
                    Dim resp = reader.ReadToEnd()
                    FrmMain.RtbDebug.AppendText(resp & vbCrLf & vbCrLf)
                    File.WriteAllText(ue, resp)
                    GNfo = Goog.FromJson(resp)
                    reader.Close()
                    KInfo.SetValue("Altitude Set", True, RegistryValueKind.DWord)
                    KInfo.SetValue("Altitude", $"{GNfo.Results(0).elevation:N0}", RegistryValueKind.DWord)
                    FrmMain.LblAltitude.Text = $"Altitude: {GNfo.Results(0).elevation:N0} meters"
                    Response.Close()
                    FrmMain.RtbLog.AppendText($"-{Now:t}- Downloaded Google Elevation file -> [{ue}]")
                    FrmMain.RtbDebug.AppendText($"Elevation: {GNfo.Results(0).elevation:N6}{vbCrLf}{vbCrLf}")
                Catch ex As Exception
                    FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbCrLf}   Location: {ex.TargetSite.ToString}{vbCrLf}   Trace: { ex.StackTrace.ToString}{vbCrLf}")
                End Try
            End If
            SaveLogs()
        End Sub

    End Module
End Namespace