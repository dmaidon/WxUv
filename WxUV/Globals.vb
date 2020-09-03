Imports System.Globalization
Imports System.IO
Imports Microsoft.Win32
Imports WxUV.Models.Forecast
Imports WxUV.Models.Protection
Imports WxUV.Models.RealTime

Module Globals
    Public ReadOnly KSet As RegistryKey = Registry.CurrentUser.CreateSubKey("Software\PAROLE Software\WxUV\Setup")
    Public ReadOnly KInfo As RegistryKey = Registry.CurrentUser.CreateSubKey("Software\PAROLE Software\WxUV\Information")
    Public ReadOnly KTok As RegistryKey = Registry.CurrentUser.CreateSubKey("Software\PAROLE Software\WxUV\Tokens\Security\Keys")

    ''OpenUV API Key
    Public ApiKey As String = ""

    Public Keyset As Boolean = False
    Public ReadOnly CLatitude As Double = CType(KSet.GetValue(My.Resources.reg_lat, "37.787644"), Double)
    Public ReadOnly CLongitude As Double = CDbl(KSet.GetValue(My.Resources.reg_lng, "-79.44189"))

    Public ReadOnly TempDir As String = Path.Combine(Application.StartupPath, "$tmp")
    Public ReadOnly LogDir As String = Path.Combine(Application.StartupPath, "Logs")
    Public LogFile As String

    Public ReadOnly LblStArr As Label() = {FrmMain.LblSt1, FrmMain.LblSt2, FrmMain.LblSt3, FrmMain.LblSt4, FrmMain.LblSt5, FrmMain.LblSt6}
    Public ReadOnly EOpt As RadioButton() = {FrmMain.RbElev0, FrmMain.RbElev1}

    ''Google
    Public ReadOnly GElev As String = $"elevation.json"

    Public Altitude As Double = 0.0

    Public Cpy As String
    Public LMsg As String
    'Public ReadOnly Updatetime As Integer = 0

    'Astro
    Public Daylight As Boolean = False

    Public Subduration As TimeSpan = New TimeSpan(60)

    ''UV Forecast
    Public ReadOnly UvFil As String = $"uv-fc_{Now:Mdyy}.json"

    Public UvNfo As UvFcast

    ''Real-time
    Public ReadOnly RtFil As String = $"uv-rt_{Now:Mdyy}.json"

    Public RtNfo As UvRtCast

    'Public OzNfo As Object
    Public OzLevel As String

    ''Protection
    Public ReadOnly ProtFil As String = $"uv-pt_{Now:Mdyy}.json"

    Public ProtNfo As Dpt

    'https://www.whatsmyua.info/
    Public Const UseAgent As String = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.83 Safari/537.36 Edg/85.0.564.44"
End Module