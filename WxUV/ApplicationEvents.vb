Imports Microsoft.VisualBasic.ApplicationServices

Namespace My

    ' The following events are available for MyApplication:
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active.
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication

        ' Catch an unhandled exception.
        'http://www.vb-helper.com/howto_net_catch_unhandled_exceptions.html
        Private Shared Sub MyApplication_UnhandledException(sender As Object, e As UnhandledExceptionEventArgs) Handles Me.UnhandledException
            e.ExitApplication = False
            FrmMain.RtbLog.AppendText _
                ($"Unhandled Exception:{vbLf}{e.Exception.Message}{vbLf}Stack Trace:{vbLf}{e.Exception.StackTrace}{vbCrLf}Target Site:{vbLf}{e.Exception.TargetSite}{vbLf _
                    }Base Exception:{vbLf}{e.Exception.GetBaseException}{vbLf}Source:{vbLf}{e.Exception.Source}{vbLf}{StrDup(50, "-"c)}{vbLf}")
        End Sub

    End Class

End Namespace