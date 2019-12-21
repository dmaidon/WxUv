Imports System.IO
Imports System.Net
Imports WxUV.Models.Protection

Namespace Modules
    Friend Module UvProtection
        Private _upf As String

        Friend Sub GetUvProtection()
            _upf = Path.Combine(TempDir, ProtFil)
            DownloadUvProtection(FrmMain.NumFrom.Value, FrmMain.NumTo.Value)
        End Sub

        Private Async Sub DownloadUvProtection(fromm As Double, too As Double)
            OzLevel = KInfo.GetValue(My.Resources.oz, 0)
            ApiKey = KTok.GetValue(My.Resources.key_uv, "")
            If Not Keyset Then
                FrmMain.TC.SelectedTab = FrmMain.TpSettings
                Return
            End If
            FrmMain.RtbLog.AppendText _
                ($"Protection Debug{vbLf}Lat: {CLatitude}   Long: {CLongitude}{vbLf}Altitude: {Altitude}{vbLf}Ozone: {OzLevel}{vbLf}From: {fromm}   To: {too}{vbLf _
                    }{vbLf}")
            Try
                Dim request = CType _
                    (WebRequest.Create _
                        (New Uri($"https://api.openuv.io/api/v1/protection?lat={CLatitude}&lng={CLongitude}&alt={Altitude}&ozone={OzLevel}&from={fromm}&to={too}")),
                    HttpWebRequest)
                With request
                    .Headers.Add($"x-access-token: {ApiKey}")
                    .AutomaticDecompression = DecompressionMethods.GZip Or DecompressionMethods.Deflate
                    .Accept = "application/json"
                    .Timeout = 120000
                    .Headers.Add("Accept-Encoding", "gzip, deflate")
                    .UserAgent = UseAgent
                End With

                Using response = CType(Await request.GetResponseAsync().ConfigureAwait(True), HttpWebResponse)
                    FrmMain.RtbLog.AppendText($"{response.StatusCode}{vbLf}{response.StatusDescription}{vbLf}{vbLf}")
                    Dim dStr = response.GetResponseStream()
                    Using reader As New StreamReader(dStr)
                        Dim resp = Await reader.ReadToEndAsync().ConfigureAwait(True)
                        FrmMain.RtbLog.AppendText(resp & vbLf & vbLf)
                        File.WriteAllText(_upf, resp)
                        ProtNfo = Dpt.FromJson(resp)
                    End Using
                End Using
                FrmMain.RtbLog.AppendText($"-{Now:t}- Downloaded UV Protection file -> [{_upf}]{vbLf}")
            Catch ex As Exception
                FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbLf}   Location: {ex.TargetSite.ToString}{vbLf}   Trace: { ex.StackTrace}{vbLf}")
            Finally
                FrmMain.RtbLog.AppendText($"{My.Resources.separator}{vbLf}")
                SaveLogs()
            End Try
            DisplayProtectionInfo()
        End Sub

        Private Sub DisplayProtectionInfo()
            With FrmMain
                Try
                    .LblProtFrom.Text = $"From: {ProtNfo.Result.FromTime.ToLocalTime:t}"
                    .LblProtTo.Text = $"To: {ProtNfo.Result.ToTime.ToLocalTime:t}"
                    .LblProtLo.Left = .LblProtFrom.Left + .LblProtFrom.Width + 5
                    .LblProtLo.Text = $"UV: {ProtNfo.Result.FromUv}"
                    .LblProtHi.Left = .LblProtTo.Left + .LblProtTo.Width + 5
                    .LblProtHi.Text = $"UV: {ProtNfo.Result.ToUv}"
                Catch ex As Exception
                    FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbLf}   Location: {ex.TargetSite.ToString}{vbLf}   Trace: { ex.StackTrace}{vbLf}")
                End Try
            End With
        End Sub
    End Module
End Namespace