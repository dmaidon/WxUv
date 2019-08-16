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
            OzLevel = KInfo.GetValue(My.Resources.oz, 0)
            ApiKey = KTok.GetValue(My.Resources.key_uv, "")
            If Not Keyset Then
                FrmMain.TC.SelectedTab = FrmMain.TpSettings
                Return
            End If
            FrmMain.RtbLog.AppendText($"Protection Debug{vbCrLf}Lat: {CLatitude}   Long: {CLongitude}{vbCrLf}Altitude: {Altitude}{vbCrLf}Ozone: {OzLevel}{vbCrLf}From: {fromm}   To: {too}{vbCrLf}{vbCrLf}")
            Try
                Dim request = CType(WebRequest.Create($"https://api.openuv.io/api/v1/protection?lat={CLatitude}&lng={CLongitude}&alt={Altitude}&ozone={OzLevel}&from={fromm}&to={too}"), HttpWebRequest)
                With request
                    .Headers.Add($"x-access-token: {ApiKey}")
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
                        File.WriteAllText(_upf, resp)
                        ProtNfo = Dpt.FromJson(resp)
                    End Using
                End Using
                FrmMain.RtbLog.AppendText($"-{Now:t}- Downloaded UV Protection file -> [{_upf}]{vbCrLf}")
            Catch ex As Exception
                FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbCrLf}   Location: {ex.TargetSite.ToString}{vbCrLf}   Trace: { ex.StackTrace.ToString}{vbCrLf}")
            Finally
                FrmMain.RtbLog.AppendText($"{SQUIGGLEY}{vbCrLf}")
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
                    FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbCrLf}   Location: {ex.TargetSite.ToString}{vbCrLf}   Trace: { ex.StackTrace.ToString}{vbCrLf}")
                End Try
            End With
        End Sub

    End Module
End Namespace