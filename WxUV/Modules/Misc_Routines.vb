Imports System.Globalization
Imports System.Text.RegularExpressions
Imports Microsoft.Win32

Namespace Modules
    Friend Module MiscRoutines
        ''count the number of times a string occurs within a string.  used bu UVForecast to determine max array of counter
        Public Function CountString(inputString As String, searchStr As String) As Integer
            Return Regex.Split(inputString, searchStr).Length - 1
        End Function

        ''' <summary>
        '''     Time To Burn Used by the "Exposure" tab to calcultate exposure time before sunburning
        ''' </summary>
        ''' <param name="st"></param>
        ''' <param name="uv"></param>
        ''' <returns></returns>
        Public Function Time2Burn(st As Double, uv As Double) As Double
            Select Case st ''.SelectedIndex
                Case 0
                    Return 200 * 2.5 / (3 * uv)
                Case 1
                    Return 200 * 3 / (3 * uv)
                Case 2
                    Return 200 * 4 / (3 * uv)
                Case 3
                    Return 200 * 5 / (3 * uv)
                Case 4
                    Return 200 * 8 / (3 * uv)
                Case Else
                    Return 200 * 15 / (3 * uv)
            End Select
        End Function

        Public Function CalcUpTime() As TimeSpan
            'Dim uptimeTs As New TimeSpan()
            Using pc As New PerformanceCounter("System", "System Up Time")
                pc.NextValue()
                Dim uptimeTs = TimeSpan.FromSeconds(pc.NextValue())
                Return uptimeTs
            End Using
        End Function

        Public Function GetSystemUpTimeInfo() As String
            Try
                Dim time = GetSystemUpTime()
                Dim upTime = String.Format(CultureInfo.CurrentCulture, "{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms", time.Hours, time.Minutes, time.Seconds, time.Milliseconds)
                Return String.Format(CultureInfo.CurrentCulture, "{0}", upTime)
            Catch ex As Exception
                'handle the exception your way
                Return String.Empty
            End Try
        End Function

        Private Function GetSystemUpTime() As TimeSpan
            Try
                Using uptime As New PerformanceCounter("System", "System Up Time")
                    uptime.NextValue()
                    Return TimeSpan.FromSeconds(uptime.NextValue)
                End Using
            Catch ex As Exception
                'handle the exception your way
                Return New TimeSpan(0, 0, 0, 0)
            End Try
        End Function

        ''' <summary>
        '''     Methods to convert DateTime to Unix time stamp
        ''' </summary>
        ''' <returns>Return Unix time stamp as long type</returns>
        Public Function Date2Unix(dt As Date) As Long
            Dim unixTimeSpan = dt.Subtract(New DateTime(1970, 1, 1, 0, 0, 0))
            Return CLng(Fix(unixTimeSpan.TotalSeconds))
        End Function

        ''' <summary>
        '''     https://stackoverflow.com/questions/33594401/how-can-i-detect-if-microsoft-edge-is-installed
        ''' </summary>
        ''' <returns></returns>
        Public Function GetEdgeVer() As String
            Dim reg = Registry.ClassesRoot.OpenSubKey("Local Settings\Software\Microsoft\Windows\CurrentVersion\AppModel\PackageRepository\Packages")
            If reg IsNot Nothing Then
                For Each rxEdgeVersion In _
                    From subkey In reg.GetSubKeyNames() Where subkey.StartsWith("Microsoft.MicrosoftEdge", StringComparison.CurrentCulture)
                        Let rxEdgeVersion1 = CType(Nothing, Match)
                        Select rxEdgeVersion1 = Regex.Match(subkey, "(Microsoft.MicrosoftEdge.Canary_)(?<version>\d+\.\d+\.\d+\.\d+)(_neutral__8wekyb3d8bbwe)")
                        Where rxEdgeVersion1.Success
                    Return rxEdgeVersion.Groups("version").Value
                Next
            End If
            Return "80.0.361.5"
        End Function
    End Module
End Namespace