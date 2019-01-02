Imports System.IO
Imports System.Net
Imports WxUV.Protection

Namespace Modules
    Module UvProtection
        Private _upf As String

        Friend Sub GetUvProtection()
            _upf = Path.Combine(TempPath, ProtFil)
            DownloadUvProtection(FrmMain.NumFrom.Value, FrmMain.NumTo.Value)
        End Sub

        Private Sub DownloadUvProtection(fromm As Double, too As Double)
            OzLevel = KInfo.GetValue("Ozone", 0)
            ApiKey = Kuv.GetValue("Key", "")
            If Not Keyset Then
                Exit Sub
            End If
            FrmMain.RtbDebug.AppendText($"Protection Debug{vbCrLf}Lat: {CLatitude}   Long: {CLongitude}{vbCrLf}Altitude: {Altitude}{vbCrLf}Ozone: {OzLevel}{vbCrLf}From: {fromm}   To: {too}{vbCrLf}{vbCrLf}")
            Try
                Dim request = CType(WebRequest.Create($"https://api.openuv.io/api/v1/protection?lat={CLatitude}&lng={CLongitude}&alt={Altitude}&ozone={OzLevel}&from={fromm}&to={too}"), HttpWebRequest)
                request.Headers.Add($"x-access-token: {ApiKey}")

                Dim response = CType(request.GetResponse(), HttpWebResponse)
                FrmMain.RtbDebug.AppendText(response.StatusCode & vbCrLf)
                FrmMain.RtbDebug.AppendText(response.StatusDescription & vbCrLf & vbCrLf)
                Dim dStr = response.GetResponseStream()
                Dim reader As New StreamReader(dStr)
                Dim resp = reader.ReadToEnd()
                FrmMain.RtbDebug.AppendText(resp & vbCrLf & vbCrLf)
                File.WriteAllText(_upf, resp)
                ProtNfo = Dpt.FromJson(resp)
                reader.Close()
                response.Close()
                FrmMain.RtbLog.AppendText($"-{Now:t}- Downloaded UV Protection file -> [{_upf}]{vbCrLf}")
            Catch ex As Exception
                FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbCrLf}   Location: {ex.TargetSite.ToString}{vbCrLf}   Trace: { ex.StackTrace.ToString}{vbCrLf}")
            Finally
                FrmMain.RtbLog.AppendText($"{Squiggley}{vbCrLf}")
                SaveLogs()
            End Try
            DisplayProtectionInfo()
        End Sub

        Private Sub DisplayProtectionInfo()
            With FrmMain
                Try
                    .LblProtFrom.Text = $"From: {ProtNfo.result.fromtime.tolocaltime:t}"
                    .LblProtTo.Text = $"To: {ProtNfo.result.totime.tolocaltime:t}"
                    .LblProtLo.Left = .LblProtFrom.Left + .LblProtFrom.Width + 5
                    .LblProtLo.Text = $"UV: {ProtNfo.result.fromuv}"
                    .LblProtHi.Left = .LblProtTo.Left + .LblProtTo.Width + 5
                    .LblProtHi.Text = $"UV: {ProtNfo.result.touv}"
                Catch ex As Exception
                    FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbCrLf}   Location: {ex.TargetSite.ToString}{vbCrLf}   Trace: { ex.StackTrace.ToString}{vbCrLf}")
                End Try
            End With
        End Sub

    End Module
End Namespace