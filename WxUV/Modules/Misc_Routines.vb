﻿Namespace Modules
    Module MiscRoutines

        ''count the number of times a string occurs within a string.  used bu UVForecast to determine max array of counter
        Public Function CountString(inputString As String, searchStr As String) As Integer
            Return Text.RegularExpressions.Regex.Split(inputString, searchStr).Length - 1
        End Function

        ''' <summary>
        ''' Time To Burn
        ''' Used by the "Exposure" tab to calcultate exposure time before sunburning
        ''' </summary>
        ''' <param name="st"></param>
        ''' <param name="uv"></param>
        ''' <returns></returns>
        Public Function Time2Burn(st As Double, uv As Double) As Integer
            Select Case st      ''.SelectedIndex
                Case 0
                    Return (200 * 2.5) / (3 * uv)
                Case 1
                    Return (200 * 3) / (3 * uv)
                Case 2
                    Return (200 * 4) / (3 * uv)
                Case 3
                    Return (200 * 5) / (3 * uv)
                Case 4
                    Return (200 * 8) / (3 * uv)
                Case Else
                    Return (200 * 15) / (3 * uv)
            End Select
        End Function

        Public Function CalcUpTime() As TimeSpan
            'Dim uptimeTs As New TimeSpan()
            Dim pc As New PerformanceCounter("System", "System Up Time")
            pc.NextValue()
            Dim uptimeTs = TimeSpan.FromSeconds(pc.NextValue())
            Return uptimeTs
        End Function

        Public Function GetSystemUpTimeInfo() As String
            Try
                Dim time = GetSystemUpTime()
                Dim upTime = String.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms", time.Hours, time.Minutes, time.Seconds, time.Milliseconds)
                Return String.Format("{0}", upTime)
            Catch ex As Exception
                'handle the exception your way
                Return String.Empty
            End Try
        End Function

        Public Function GetSystemUpTime() As TimeSpan
            Try
                Dim uptime = New PerformanceCounter("System", "System Up Time")
                uptime.NextValue()
                Return TimeSpan.FromSeconds(uptime.NextValue)
            Catch ex As Exception
                'handle the exception your way
                Return New TimeSpan(0, 0, 0, 0)
            End Try
        End Function

        ''' <summary>
        ''' Methods to convert DateTime to Unix time stamp
        ''' </summary>
        ''' <returns>Return Unix time stamp as long type</returns>
        Public Function Date2Unix(dt As Date) As Long
            Dim unixTimeSpan = Dt.Subtract(New DateTime(1970, 1, 1, 0, 0, 0))
            Return Fix(unixTimeSpan.TotalSeconds)
        End Function

    End Module
End Namespace