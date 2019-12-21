Imports System.ComponentModel
Imports System.IO
Imports System.Timers
Imports IWshRuntimeLibrary
Imports Microsoft.Win32
Imports WxUV.Modules

Friend Class FrmMain
    'Adds the applications AssemblyName to the Desktop's path and adds the .lnk extension used for shortcuts
    Private ReadOnly _desktopPathName As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), My.Application.Info.AssemblyName & $".lnk")

    'Adds the applications AssemblyName to the Startup folder path and adds the .lnk extension used for shortcuts
    Private ReadOnly _startupPathName As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), My.Application.Info.AssemblyName & $".lnk")

    'Used to stop the CheckBoxes CheckedChanged events from calling the CreateShortcut sub when the form is
    'loading and setting the Checkboxes states to true if the shortcuts exist.
    Private _loading As Boolean = True

    ''OpenUV
    Private Sub FrmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        UpgradeMySettings()
        'Sets the Desktop checkbox checked state to true if the desktop shortcut exists
        ChkDeskShort.Checked = IO.File.Exists(_desktopPathName)
        'Sets the Startup Folder checkbox checked state to true if the Startup folder shortcut exists
        ChkStartShort.Checked = IO.File.Exists(_startupPathName)
        'The checkboxes checked states have been set so set Loading to false to allow the CreateShortcut sub to be called now
        _loading = False
        Cpy = $"©{Date.Now.Year}, {Application.CompanyName}"
        CreateFolders()

        ''set the header for the .log file
        Dim timesrun = My.Settings.TimesRun + 1
        My.Settings.TimesRun = timesrun
        My.Settings.Save()

        KInfo.SetValue("Last Run", Now.ToString, RegistryValueKind.String)
        KInfo.SetValue("FirstRun", False, RegistryValueKind.String)
        LogFile = $"{Path.Combine(Application.StartupPath, LogDir)}\uv_{Format(Now, "MMddyyyy_").ToString}{timesrun + 1}.log"
        LMsg = ""
        LMsg = $"Log file started: {Now}{vbLf}"
        LMsg &= $"Program: {Application.ProductName} v{Application.ProductVersion}{vbLf}"
        LMsg &= $"Times run: {timesrun + 1}{vbLf}"
        'LMsg &= $"Update frequency: {Updatetime} minutes.{vbLf}"
        LMsg &= $"{My.Resources.separator}{vbLf}"
        RtbLog.AppendText(LMsg)

        ''cleanup the logfile folder and delete the old files.
        PerformLogMaintenance()
        GetUvRealTime()
        'InitializeArrays()
        With Me
            '.Top = CInt(KMet.GetValue("Top", 100))
            '.Left = CInt(KMet.GetValue("Left", 100))
            .Text = $"{Application.ProductName}"
            .TsslVer.Text = $"{Application.ProductVersion}"
            .TsslCpy.Text = $"{Cpy}"
            .TsslTimesRun.Text = String.Format(TsslTimesRun.Tag, timesrun)
            .LblAbout.Text = My.Resources.written_by
            .SetTimers()
            .Show()
        End With

        If KInfo.GetValue(My.Resources.alt_set, 0) = 0 Then
            GetElevation()
        Else
            RtbLog.AppendText($"Altitude set from Registry: {KInfo.GetValue(My.Resources.alt, 0)} meters{vbLf}")
        End If
        Altitude = ($"{KInfo.GetValue(My.Resources.alt, 0)}")

        RtbLog.AppendText($"{My.Resources.separator}{vbLf}")
        SaveLogs()

        GetUvForecast()

        If NumTo.Value > 0 Then
            GetUvProtection()
        End If

        CollectMemoryGarbage()
        'MsgBox(GetEdgeVer)
    End Sub

    Private Sub FrmMain_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        My.Settings.Save()
        RtbLog.AppendText($"Log closed @ {Now.ToLongTimeString()}")
        TIcon.Dispose()
        CollectMemoryGarbage()
        SaveLogs()
    End Sub

    ''' <summary>
    '''     Manually update application settings In Settings create MustUpgrade/Boolean/User/True
    ''' </summary>
    Private Shared Sub UpgradeMySettings()
        'https://stackoverflow.com/questions/1702260/losing-vb-net-my-settings-with-each-new-clickonce-deployment-release
        If My.Settings.MustUpgrade Then
            My.Settings.Upgrade()
            My.Settings.MustUpgrade = False
            My.Settings.Save()
        End If
    End Sub

    Private Sub CreateFolders()
        Dim fName As New List(Of String)({LogDir, TempDir})
        For j = 0 To fName.Count - 1
            Try
                If Not Directory.Exists(fName(j)) Then
                    Directory.CreateDirectory(fName(j))
                    RtbLog.AppendText($"{fName(j)} ==> created.{vbLf}")
                Else
                    RtbLog.AppendText($"{fName(j)} --> exists.{vbLf}")
                End If
            Catch ex As Exception
                RtbLog.AppendText($"   Error: {ex.Message}{vbLf}   Location: {ex.TargetSite.ToString}{vbLf}   Trace: { ex.StackTrace.ToString}{vbLf}")
            Finally
                'a
            End Try
        Next
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
        RtbLog.AppendText($"{msg}{vbLf}{My.Resources.separator}{vbLf}")
        SaveLogs()
    End Sub

#Region "TpDaily"

    Private Shared Sub BtnUpdateRT_Click(sender As Object, e As EventArgs) Handles BtnUpdateRT.Click
        GetUvRealTime()
        GetUvForecast()
    End Sub

#End Region

#Region "Timers"

    Private Sub SetTimers()
        Try
            Dim aa As Integer = KInfo.GetValue(My.Resources.uv_int, 0)
            If aa > 0 Then
                TmrRtUV.Interval = TimeSpan.FromMinutes(aa).TotalMilliseconds ' * Timer_Multiplier
                TmrRtUV.Enabled = True
                TmrRtUV.Start()
            Else
                TmrRtUV.Stop()
                TmrRtUV.Enabled = False
            End If
            RtbLog.AppendText($"Set RealTime UV Timer. {vbLf}")
        Catch ex As Exception
            RtbLog.AppendText($"   Error: {ex.Message}{vbLf}   Location: {ex.TargetSite.ToString}{vbLf}   Trace: { ex.StackTrace.ToString}{vbLf}")
        Finally
            SaveLogs()
        End Try
    End Sub

    Private Shared Sub TmrRtUV_Elapsed(sender As Object, e As ElapsedEventArgs) Handles TmrRtUV.Elapsed
        GetUvRealTime()
        GetUvForecast()
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

    ''' <summary>
    '''     Creates or removes a shortcut for this application at the specified pathname.
    ''' </summary>
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
                RtbLog.AppendText($"   Error: {ex.Message}{vbLf}   Location: {ex.TargetSite.ToString}{vbLf}   Trace: { ex.StackTrace.ToString}{vbLf}")
            Finally
                ''
            End Try
        Else
            Try
                If IO.File.Exists(shortcutPathName) Then IO.File.Delete(shortcutPathName)
            Catch ex As Exception
                RtbLog.AppendText($"   Error: {ex.Message}{vbLf}   Location: {ex.TargetSite.ToString}{vbLf}   Trace: { ex.StackTrace.ToString}{vbLf}")
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
        GetUvProtection()
    End Sub

    Private Shared Sub NumEnter(sender As Object, e As EventArgs) Handles NumFrom.Enter, NumTo.Enter, NumElevation.Enter, NumRTInterval.Enter, NumLogDays.Enter
        Dim ct = DirectCast(sender, NumericUpDown)
        ct.Select(0, ct.Text.Length)
    End Sub

    Private Sub NumTo_ValueChanged(sender As Object, e As EventArgs) Handles NumTo.ValueChanged
        BtnProtection.Enabled = NumTo.Value > 0
    End Sub

#End Region

#Region "Exposure"

    Private Sub BtnBurnCalc_Click(sender As Object, e As EventArgs) Handles BtnBurnCalc.Click
        LblBurnTime.Text = $"{Time2Burn(CbSkinType.SelectedIndex, CbUv.SelectedItem)} minutes."
    End Sub

#End Region

#Region "About"

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click, PictureBox2.Click
        Try
            With DirectCast(sender, PictureBox)
                Select Case CInt(.Tag)
                    Case 0
                        Process.Start($"https://www.openuv.io")
                    Case 1
                        Process.Start($"http://parolesoftware.com/")
                End Select
            End With
        Catch ex As Exception
            RtbLog.AppendText($"   Error: {ex.Message}{vbLf}   Location: {ex.TargetSite.ToString}{vbLf}   Trace: { ex.StackTrace.ToString}{vbLf}")
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
        KTok.SetValue(My.Resources.key_uv, TxtUvKey.Text, RegistryValueKind.String)
    End Sub

    Private Sub TxtGoogleKey_TextChanged(sender As Object, e As EventArgs) Handles TxtGoogleKey.TextChanged
        KTok.SetValue(My.Resources.key_goog, TxtGoogleKey.Text, RegistryValueKind.String)
    End Sub

    Private Sub TxtElevationApiKey_TextChanged(sender As Object, e As EventArgs) Handles TxtElevationApiKey.TextChanged
        KTok.SetValue(My.Resources.key_elev, TxtElevationApiKey.Text, RegistryValueKind.String)
    End Sub

    Private Shared Sub RbElevCheckedChanged(sender As Object, e As EventArgs) Handles RbElev0.CheckedChanged, RbElev1.CheckedChanged
        Dim ni = DirectCast(sender, RadioButton)
        KSet.SetValue(My.Resources.elev_toggle, CInt(ni.Tag), RegistryValueKind.DWord)
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
        TxtLatitude.Text = KSet.GetValue(My.Resources.reg_lat, "37.787644")
        TxtLongitude.Text = KSet.GetValue(My.Resources.reg_lng, "-79.44189")
        TxtUvKey.Text = KTok.GetValue(My.Resources.key_uv, "")
        TxtGoogleKey.Text = KTok.GetValue(My.Resources.key_goog, "")
        TxtElevationApiKey.Text = KTok.GetValue(My.Resources.key_elev, "")
        ChkHideLog.Checked = KSet.GetValue(My.Resources.hide_log, 0)
        NumElevation.Value = KInfo.GetValue(My.Resources.alt, 0)
        NumRTInterval.Value = KInfo.GetValue(My.Resources.uv_int, 0)
        NumLogDays.Value = KSet.GetValue(My.Resources.log_days, 3)
        LblElevHelp.Text = My.Resources.elev_help
        EOpt(KSet.GetValue(My.Resources.elev_toggle, 1)).Checked = True
    End Sub

    Private Sub BntResetAlt_Click(sender As Object, e As EventArgs) Handles BntResetAlt.Click
        Const msg = "Are you sure that you would like to reset the Altitude?"
        Const caption = "Reset Altitude"
        Dim result = MessageBox.Show(msg, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        ' If the no button was pressed ...
        If result = DialogResult.No Then
            ' cancel reset
            Return
        Else
            RtbLog.AppendText($"{My.Resources.separator}{vbLf}")
            Try
                Dim ue = Path.Combine(TempDir, GElev)
                If IO.File.Exists(ue) Then
                    IO.File.Delete(ue)
                    RtbLog.AppendText($"Delete -> {ue}{vbLf}")
                End If
            Catch ex As Exception
                RtbLog.AppendText($"   Error: {ex.Message}{vbLf}   Location: {ex.TargetSite.ToString}{vbLf}   Trace: { ex.StackTrace.ToString}{vbLf}")
            Finally
                ''
            End Try
            KInfo.SetValue("Altitude Set", 0)
            RtbLog.AppendText($"~~~~~ Altitude reset{vbLf}{My.Resources.separator}{vbLf}")
            GetElevation()
        End If
    End Sub

    Private Sub TxtLatitude_TextChanged(sender As Object, e As EventArgs) Handles TxtLatitude.TextChanged
        KSet.SetValue(My.Resources.reg_lat, TxtLatitude.Text, RegistryValueKind.String)
    End Sub

    Private Sub TxtLongitude_TextChanged(sender As Object, e As EventArgs) Handles TxtLongitude.TextChanged
        KSet.SetValue(My.Resources.reg_lng, TxtLongitude.Text, RegistryValueKind.String)
    End Sub

    Private Sub NumRTInterval_ValueChanged(sender As Object, e As EventArgs) Handles NumRTInterval.ValueChanged
        KInfo.SetValue(My.Resources.uv_int, NumRTInterval.Value, RegistryValueKind.DWord)
    End Sub

    Private Sub NumElevation_ValueChanged(sender As Object, e As EventArgs) Handles NumElevation.ValueChanged
        KInfo.SetValue(My.Resources.alt, $"{NumElevation.Value:N0}", RegistryValueKind.DWord)
    End Sub

    Private Sub NumLogDays_ValueChanged(sender As Object, e As EventArgs) Handles NumLogDays.ValueChanged
        KSet.SetValue(My.Resources.log_days, NumLogDays.Value, RegistryValueKind.DWord)
    End Sub

#End Region
End Class