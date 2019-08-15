Imports System.IO
Imports System.Security

Namespace Modules
    ''' <summary>
    ''' keep the log folder clean and only store a set number of log files. Days to keep is set on the "Settings" tab
    ''' </summary>
    Module LogMaint

        ''http://stackoverflow.com/questions/9194749/trying-to-delete-files-older-than-x-days-vb-net
        Public Sub PerformLogMaintenance()

            FrmMain.RtbLog.AppendText($"<{SEPARATOR}{vbCrLf}")
            Dim intdays = KSet.GetValue(My.Resources.log_days, 5)
            If intdays <= 0 Then
                FrmMain.RtbLog.AppendText($"Logs set to keep all.{vbCrLf}")
                Return
            End If

            Dim fc As Short
            Try
                For Each file In New DirectoryInfo(LogPath).GetFiles("uv_*.log")
                    If (Now - file.LastWriteTime).Days > intdays Then
                        fc += 1
                        file.Delete()
                        FrmMain.RtbLog.AppendText($"File deleted -> {file.FullName}{vbCrLf} File Date: {file.LastWriteTime}{vbCrLf}")
                    End If
                    Application.DoEvents()
                Next

                For Each file In New DirectoryInfo(TempPath).GetFiles("*.json")
                    If (Now - file.LastWriteTime).Days > intdays Then
                        fc += 1
                        file.Delete()
                        FrmMain.RtbLog.AppendText($"File deleted -> {file.FullName}{vbCrLf} File Date: {file.LastWriteTime}{vbCrLf}")
                    End If
                    Application.DoEvents()
                Next
            Catch ex As ArgumentException
                FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbCrLf}   Location: {ex.TargetSite.ToString}{vbCrLf}   Trace: { ex.StackTrace.ToString}{vbCrLf}")
            Catch ex As SecurityException
                FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbCrLf}   Location: {ex.TargetSite.ToString}{vbCrLf}   Trace: { ex.StackTrace.ToString}{vbCrLf}")
            Catch ex As Exception
                FrmMain.RtbLog.AppendText($"   Error: {ex.Message}{vbCrLf}   Location: {ex.TargetSite.ToString}{vbCrLf}   Trace: { ex.StackTrace.ToString}{vbCrLf}")
            Finally
                ''trap error
            End Try
            FrmMain.RtbLog.AppendText($"Files over {intdays} days in age deleted.({fc} files deleted.){vbCrLf}")
            FrmMain.RtbLog.AppendText($"{SEPARATOR}>{vbCrLf}")
            SaveLogs()
        End Sub

        Public Sub SaveLogs()
            Try
                FrmMain.RtbLog.SaveFile(LogFile, RichTextBoxStreamType.PlainText)
            Catch ex As Exception
                MsgBox($"Unable to save logfile.{vbCrLf}Error: {ex.Message}")
            Finally
                ''
            End Try
        End Sub

    End Module
End Namespace