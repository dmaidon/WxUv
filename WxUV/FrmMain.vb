Imports System.ComponentModel
Imports System.IO
Imports System.Timers
Imports IWshRuntimeLibrary
Imports Microsoft.Win32
Imports WxUV.Modules

Public Class FrmMain

    'Adds the applications AssemblyName to the Desktop's path and adds the .lnk extension used for shortcuts
    Private ReadOnly _desktopPathName As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), My.Application.Info.AssemblyName & $".lnk")

    'Adds the applications AssemblyName to the Startup folder path and adds the .lnk extension used for shortcuts
    Private ReadOnly _startupPathName As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), My.Application.Info.AssemblyName & $".lnk")

    'Used to stop the CheckBoxes CheckedChanged events from calling the CreateShortcut sub when the form is
    'loading and setting the Checkboxes states to true if the shortcuts exist.
    Private _loading As Boolean = True

    ''OpenUV
    Private Sub FrmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Sets the Desktop checkbox checked state to true if the desktop shortcut exists
        ChkDeskShort.Checked = IO.File.Exists(_desktopPathName)
        'Sets the Startup Folder checkbox checked state to true if the Startup folder shortcut exists
        ChkStartShort.Checked = IO.File.Exists(_startupPathName)
        'The checkboxes checked states have been set so set Loading to false to allow the CreateShortcut sub to be called now
        _loading = False
        Cpy = $"©{DateTime.Now.Year}, {Application.CompanyName}"
        CreateFolders()

        ''set the header for the .log file
        Dim timesRun As Long
        timesRun = CLng((KInfo.GetValue("TimesRun", 0))) + 1
        KInfo.SetValue("TimesRun", timesRun, RegistryValueKind.QWord)
        KInfo.SetValue("Last Run", Now.ToString, RegistryValueKind.String)
        KInfo.SetValue("FirstRun", False, RegistryValueKind.String)
        LogFile = $"{Path.Combine(Application.StartupPath, LogDir)}\uv_{Format(Now, "MMddyyyy_").ToString}{timesRun}.log"
        LMsg = ""
        LMsg = $"Log file started: {Now}{vbCrLf}"
        LMsg = LMsg & $"Program: {Application.ProductName} v{Application.ProductVersion}{vbCrLf}"
        LMsg = LMsg & $"Times run: {timesRun}{vbCrLf}"
        LMsg = LMsg & $"Update frequency: {Updatetime} minutes.{vbCrLf}{Separator}{vbCrLf}"
        RtbLog.AppendText(LMsg)

        ''cleanup the logfile folder and delete the old files.
        PerformLogMaintenance()
        GetUVRealTime()
        InitializeArrays()
        With Me
            .Top = CInt(KMet.GetValue("Top", 100))
            .Left = CInt(KMet.GetValue("Left", 100))
            .Text = $"{Application.ProductName}"
            .TsslVer.Text = $"{Application.ProductVersion}"
            .TsslCpy.Text = $"{Cpy}"
            .TsslTimesRun.Text = $"[{timesRun}]"
            .SetTimers()
            .Show()
        End With

        If KInfo.GetValue("Altitude Set", 0) = 0 Then
            GetElevation()
        Else
            RtbLog.AppendText($"Altitude set from Registry: {KInfo.GetValue("Altitude", 0)} meters{vbCrLf}")
        End If
        Altitude = ($"{KInfo.GetValue("Altitude", 0)}")

        RtbLog.AppendText($"{Squiggley}{vbCrLf}")
        SaveLogs()

        GetUVForecast()

        If NumTo.Value > 0 Then
            GetUVProtection()
        End If

        CollectMemoryGarbage()
    End Sub

    Private Sub FrmMain_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        KMet.SetValue("Top", Top, RegistryValueKind.DWord)
        KMet.SetValue("Left", Left, RegistryValueKind.DWord)
        RtbLog.AppendText($"Log closed @ {Now.ToLongTimeString()}")
        TIcon.Dispose()
        CollectMemoryGarbage()
        SaveLogs()
    End Sub

    Private Sub InitializeArrays()
        UvArr(0) = PbUv1
        UvArr(1) = PbUv2
        UvArr(2) = PbUv3
        UvArr(3) = PbUv4
        UvArr(4) = PbUv5
        UvArr(5) = PbUv6
        UvArr(6) = PbUv7
        UvArr(7) = PbUv8
        UvArr(8) = PbUv9
        UvArr(9) = PbUv10
        UvArr(10) = PbUv11
        UvArr(11) = PbUv12
        UvArr(12) = PbUv13
        UvArr(13) = PbUv14
        UvArr(14) = PbUv15
        UvArr(15) = PbUv16
        UvArr(16) = PbUv17
        UvArr(17) = PbUv18

        LblArr(0) = Label1
        LblArr(1) = Label2
        LblArr(2) = Label3
        LblArr(3) = Label4
        LblArr(4) = Label5
        LblArr(5) = Label6
        LblArr(6) = Label7
        LblArr(7) = Label8
        LblArr(8) = Label9
        LblArr(9) = Label10
        LblArr(10) = Label11
        LblArr(11) = Label12
        LblArr(12) = Label13
        LblArr(13) = Label14
        LblArr(14) = Label15
        LblArr(15) = Label16
        LblArr(16) = Label17
        LblArr(17) = Label18

        LblStArr(0) = LblSt1
        LblStArr(1) = LblSt2
        LblStArr(2) = LblSt3
        LblStArr(3) = LblSt4
        LblStArr(4) = LblSt5
        LblStArr(5) = LblSt6
    End Sub

    Private Sub CreateFolders()
        TempPath = Path.Combine(Application.StartupPath, TempDir)
        LogPath = Path.Combine(Application.StartupPath, LogDir)
        Try
            If Not Directory.Exists(TempPath) Then
                Directory.CreateDirectory(TempPath)
            End If
        Catch ex As Exception
            RtbLog.AppendText($"   Error: {ex.Message}{vbCrLf}   Location: {ex.TargetSite.ToString}{vbCrLf}   Trace: { ex.StackTrace.ToString}{vbCrLf}")
        Finally
            ''
        End Try

        Try
            If Not Directory.Exists(LogPath) Then
                Directory.CreateDirectory(LogPath)
            End If
        Catch ex As Exception
            RtbLog.AppendText($"   Error: {ex.Message}{vbCrLf}   Location: {ex.TargetSite.ToString}{vbCrLf}   Trace: { ex.StackTrace.ToString}{vbCrLf}")
        Finally
            ''
        End Try
    End Sub

    Private Sub CollectMemoryGarbage()
        Dim mbc As Long
        Dim mac As Long
        Dim hg = GC.MaxGeneration
        Dim tmb = GC.GetTotalMemory(False)
        mbc = GC.GetTotalMemory(False)
        GC.Collect()
        Dim tma = GC.GetTotalMemory(False)
        mac = GC.GetTotalMemory(False)
        ''XML literal
        Dim msg = <msg>
Memory Garbage Collection
Highest Generation: <%= hg %>
Total memory before: <%= tmb.ToString("#,### bytes") %>
Total memory after: <%= tma.ToString("#,### bytes") %>
Total memory collected: <%= (mbc - mac).ToString("#,### bytes") %>
                  </msg>.Value
        '        Dim msg2 = <msg2>
        'To disable this function, go to the
        '"WxReport/WxNow" tab in WxSettings.
        '                  </msg2>.Value
        'TTip.SetToolTip(TsslUrl, $"{msg}{vbCrLf}{msg2}")
        RtbLog.AppendText($"{msg}{vbCrLf}")
        RtbLog.AppendText($"{Separator}{vbCrLf}")
        SaveLogs()
    End Sub

#Region "TpDaily"

    Private Shared Sub BtnUpdateRT_Click(sender As Object, e As EventArgs) Handles BtnUpdateRT.Click
        GetUVRealTime()
        GetUVForecast()
    End Sub

#End Region

#Region "Timers"

    Private Sub SetTimers()
        Try
            If KInfo.GetValue("RealTime UV Interval", 0) > 0 Then
                TmrRtUV.Interval = KInfo.GetValue("RealTime UV Interval", 0) * TimerMultiplier
                TmrRtUV.Enabled = True
                TmrRtUV.Start()
            Else
                TmrRtUV.Stop()
                TmrRtUV.Enabled = False
            End If
            RtbLog.AppendText($"Set RealTime UV Timer. {vbCrLf}")
        Catch ex As Exception
            RtbLog.AppendText($"   Error: {ex.Message}{vbCrLf}   Location: {ex.TargetSite.ToString}{vbCrLf}   Trace: { ex.StackTrace.ToString}{vbCrLf}")
        Finally
            SaveLogs()
        End Try
    End Sub

    Private Shared Sub TmrRtUV_Elapsed(sender As Object, e As ElapsedEventArgs) Handles TmrRtUV.Elapsed
        GetUVRealTime()
        GetUVForecast()
    End Sub

#End Region

#Region "TpDebug"

    Private Shared Sub BtnForecast_Click(sender As Object, e As EventArgs) Handles BtnForecast.Click
        GetUVForecast()
    End Sub

    Private Shared Sub BtnRealTime_Click_1(sender As Object, e As EventArgs) Handles BtnRealTime.Click
        GetUVRealTime()
    End Sub

#End Region

#Region "Shortcuts"

    Private Sub ChkDeskShort_CheckedChanged(sender As Object, e As EventArgs) Handles ChkDeskShort.CheckedChanged
        If Not _loading Then
            If ChkDeskShort.Checked Then
                CreateShortcut(_desktopPathName, True) 'Create a shortcut on the desktop
            Else
                CreateShortcut(_desktopPathName, False) 'Remove the shortcut from the desktop
            End If
        End If
    End Sub

    Private Sub ChkStartShort_CheckedChanged(sender As Object, e As EventArgs) Handles ChkStartShort.CheckedChanged
        If Not _loading Then
            If ChkStartShort.Checked Then
                CreateShortcut(_startupPathName, True) 'Create a shortcut in the startup folder
            Else
                CreateShortcut(_startupPathName, False) 'Remove the shortcut in the startup folder
            End If
        End If
    End Sub

    ''' <summary>Creates or removes a shortcut for this application at the specified pathname.</summary>
    ''' <param name="shortcutPathName">
    '''     The path where the shortcut is to be created or removed from including the (.lnk)
    '''     extension.
    ''' </param>
    ''' <param name="create">True to create a shortcut or False to remove the shortcut.</param>
    Private Sub CreateShortcut(shortcutPathName As String, create As Boolean)
        If create Then
            Try
                Dim shortcutTarget = Path.Combine(Application.StartupPath, My.Application.Info.AssemblyName & ".exe")
                Dim myShell As New WshShell()
                Dim myShortcut = CType(myShell.CreateShortcut(shortcutPathName), WshShortcut)
                myShortcut.TargetPath = shortcutTarget 'The exe file this shortcut executes when double clicked
                myShortcut.IconLocation = shortcutTarget & ",0" 'Sets the icon of the shortcut to the exe`s icon
                myShortcut.WorkingDirectory = Application.StartupPath 'The working directory for the exe
                myShortcut.Arguments = "" 'The arguments used when executing the exe
                myShortcut.Save() 'Creates the shortcut
            Catch ex As Exception
                RtbLog.AppendText($"   Error: {ex.Message}{vbCrLf}   Location: {ex.TargetSite.ToString}{vbCrLf}   Trace: { ex.StackTrace.ToString}{vbCrLf}")
            Finally
                ''
            End Try
        Else
            Try
                If IO.File.Exists(shortcutPathName) Then IO.File.Delete(shortcutPathName)
            Catch ex As Exception
                RtbLog.AppendText($"   Error: {ex.Message}{vbCrLf}   Location: {ex.TargetSite.ToString}{vbCrLf}   Trace: { ex.StackTrace.ToString}{vbCrLf}")
            Finally
                ''
            End Try
        End If
    End Sub

    Private Sub TmrClock_Elapsed(sender As Object, e As ElapsedEventArgs) Handles TmrClock.Elapsed
        TsslClock.Text = Now.ToLongTimeString()
    End Sub

#End Region

#Region "Protection"

    Private Shared Sub BtnProtection_Click(sender As Object, e As EventArgs) Handles BtnProtection.Click
        GetUVProtection()
    End Sub

    Private Sub NumFrom_Enter(sender As Object, e As EventArgs) Handles NumFrom.Enter
        NumFrom.Select(0, NumFrom.Text.Length)
    End Sub

    Private Sub NumTo_Enter(sender As Object, e As EventArgs) Handles NumTo.Enter
        NumTo.Select(0, NumTo.Text.Length)
    End Sub

    Private Sub NumTo_ValueChanged(sender As Object, e As EventArgs) Handles NumTo.ValueChanged
        If NumTo.Value > 0 Then
            BtnProtection.Enabled = True
        Else
            BtnProtection.Enabled = False
        End If
    End Sub

#End Region

#Region "Exposure"

    Private Sub BtnBurnCalc_Click(sender As Object, e As EventArgs) Handles BtnBurnCalc.Click
        LblBurnTime.Text = $"{Time2Burn(CbSkinType.SelectedIndex, CbUv.SelectedItem)} minutes."
    End Sub

#End Region

#Region "About"

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        Try
            Process.Start("http://parolesoftware.com/")
        Catch ex As Exception
            RtbLog.AppendText($"   Error: {ex.Message}{vbCrLf}   Location: {ex.TargetSite.ToString}{vbCrLf}   Trace: { ex.StackTrace.ToString}{vbCrLf}")
        Finally
            ''
        End Try
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        Try
            Process.Start("https://www.openuv.io")
        Catch ex As Exception
            RtbLog.AppendText($"   Error: {ex.Message}{vbCrLf}   Location: {ex.TargetSite.ToString}{vbCrLf}   Trace: { ex.StackTrace.ToString}{vbCrLf}")
        Finally
            ''
        End Try
    End Sub

    Private Sub TpAbout_Enter(sender As Object, e As EventArgs) Handles TpAbout.Enter
        Dim ts = CalcUpTime()
        LblUpTime.Text = ($"{ts.TotalDays:N0} Days  {ts.Hours:N0} Hours  {ts.Minutes:N0} Minutes  {ts.Seconds:N0} Seconds")
        SaveLogs()
    End Sub

    Private Shared Sub TpAbout_Leave(sender As Object, e As EventArgs) Handles TpAbout.Leave
        SaveLogs()
    End Sub

#End Region

#Region "TpSettings"

    Private Sub TxtUvKey_TextChanged(sender As Object, e As EventArgs) Handles TxtUvKey.TextChanged
        ''My.Computer.Registry.CurrentUser.DeleteSubKey("Software\TestApp")
        Kuv.SetValue("Key", TxtUvKey.Text, RegistryValueKind.String)
    End Sub

    Private Sub TxtGoogleKey_TextChanged(sender As Object, e As EventArgs) Handles TxtGoogleKey.TextChanged
        Kg.SetValue("Elevation API Key", TxtGoogleKey.Text, RegistryValueKind.String)
    End Sub

    Private Sub ChkHideDebuf_CheckedChanged(sender As Object, e As EventArgs) Handles ChkHideDebug.CheckedChanged
        KSet.SetValue("Hide Debug Tab", ChkHideDebug.Checked, RegistryValueKind.DWord)
        If ChkHideDebug.Checked Then
            TC.TabPages.Remove(TpDebug)
        Else
            TC.TabPages.Insert(5, TpDebug)
        End If
    End Sub

    Private Sub ChkHideLog_CheckedChanged(sender As Object, e As EventArgs) Handles ChkHideLog.CheckedChanged
        KSet.SetValue("Hide Log Tab", ChkHideLog.Checked, RegistryValueKind.DWord)
        If ChkHideLog.Checked Then
            TC.TabPages.Remove(TpLog)
        Else
            TC.TabPages.Insert(6, TpLog)
        End If
    End Sub

    Private Sub TpSettings_Enter(sender As Object, e As EventArgs) Handles TpSettings.Enter
        TxtLatitude.Text = KSet.GetValue("Latitude", "37.787644")
        TxtLongitude.Text = KSet.GetValue("Longitude", "-79.44189")
        TxtUvKey.Text = Kuv.GetValue("Key", "")
        TxtGoogleKey.Text = Kg.GetValue("Elevation API key", "")
        ChkHideDebug.Checked = KSet.GetValue("Hide Debug Page", 0)
        ChkHideLog.Checked = KSet.GetValue("Hide Log Page", 0)
        NumElevation.Value = KInfo.GetValue("Altitude", 0)
        NumRTInterval.Value = KInfo.GetValue("RealTime UV Interval", 0)
        NumLogDays.Value = KSet.GetValue("Days to keep logs", 3)
    End Sub

    Private Sub BntResetAlt_Click(sender As Object, e As EventArgs) Handles BntResetAlt.Click
        Const msg = "Are you sure that you would like to reset the Altitude?"
        Const caption = "Reset Altitude"
        Dim result = MessageBox.Show(msg, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        ' If the no button was pressed ...
        If result = DialogResult.No Then
            ' cancel reset
            Exit Sub
        Else
            RtbLog.AppendText($"{Squiggley}{vbCrLf}")
            Try
                Dim ue = Path.Combine(TempPath, GElev)
                If IO.File.Exists(ue) Then
                    IO.File.Delete(ue)
                    RtbLog.AppendText($"Delete -> {ue}{vbCrLf}")
                End If
            Catch ex As Exception
                RtbLog.AppendText($"   Error: {ex.Message}{vbCrLf}   Location: {ex.TargetSite.ToString}{vbCrLf}   Trace: { ex.StackTrace.ToString}{vbCrLf}")
            Finally
                ''
            End Try
            KInfo.SetValue("Altitude Set", 0)
            RtbLog.AppendText($"~~~~~ Altitude reset{vbCrLf}{Squiggley}{vbCrLf}")
            RtbDebug.AppendText($"~~~~~ Altitude reset{vbCrLf}{vbCrLf}")
            DownloadElevation()
        End If
    End Sub

    Private Sub TxtLatitude_TextChanged(sender As Object, e As EventArgs) Handles TxtLatitude.TextChanged
        KSet.SetValue("Latitude", TxtLatitude.Text, RegistryValueKind.String)
    End Sub

    Private Sub TxtLongitude_TextChanged(sender As Object, e As EventArgs) Handles TxtLongitude.TextChanged
        KSet.SetValue("Longitude", TxtLongitude.Text, RegistryValueKind.String)
    End Sub

    Private Sub NumRTInterval_Enter(sender As Object, e As EventArgs) Handles NumRTInterval.Enter
        NumRTInterval.Select(0, NumRTInterval.Text.Length)
    End Sub

    Private Sub NumRTInterval_ValueChanged(sender As Object, e As EventArgs) Handles NumRTInterval.ValueChanged
        KInfo.SetValue("RealTime UV Interval", NumRTInterval.Value, RegistryValueKind.DWord)
    End Sub

    Private Sub NumElevation_ValueChanged(sender As Object, e As EventArgs) Handles NumElevation.ValueChanged
        KInfo.SetValue("Altitude", $"{NumElevation.Value:N0}", RegistryValueKind.DWord)
    End Sub

    Private Sub NumElevation_Enter(sender As Object, e As EventArgs) Handles NumElevation.Enter
        NumElevation.Select(0, NumElevation.Text.Length)
    End Sub

    Private Sub NumLogDays_Enter(sender As Object, e As EventArgs) Handles NumLogDays.Enter
        ''set the number of days to maintain the log files in the LOG folder.
        NumLogDays.Select(0, NumLogDays.Text.Length)
    End Sub

    Private Sub NumLogDays_ValueChanged(sender As Object, e As EventArgs) Handles NumLogDays.ValueChanged
        KSet.SetValue("Days to keep logs", NumLogDays.Value, RegistryValueKind.DWord)
    End Sub

#End Region

End Class