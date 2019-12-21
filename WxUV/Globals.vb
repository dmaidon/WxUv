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
    Public ReadOnly CLatitude As Double = KSet.GetValue(My.Resources.reg_lat, "37.787644")
    Public ReadOnly CLongitude As Double = KSet.GetValue(My.Resources.reg_lng, "-79.44189")

    Public ReadOnly TempDir As String = Path.Combine(Application.StartupPath, "$tmp")
    Public ReadOnly LogDir As String = Path.Combine(Application.StartupPath, "Logs")
    Public LogFile As String

    Public ReadOnly _
        UvArr As PictureBox() =
            {FrmMain.PbUv1, FrmMain.PbUv2, FrmMain.PbUv3, FrmMain.PbUv4, FrmMain.PbUv5, FrmMain.PbUv6, FrmMain.PbUv7, FrmMain.PbUv8, FrmMain.PbUv9, FrmMain.PbUv10,
                FrmMain.PbUv11, FrmMain.PbUv12, FrmMain.PbUv13, FrmMain.PbUv14, FrmMain.PbUv15, FrmMain.PbUv16, FrmMain.PbUv17, FrmMain.PbUv18}

    Public ReadOnly _
        LblArr As Label() =
            {FrmMain.Label1, FrmMain.Label2, FrmMain.Label3, FrmMain.Label4, FrmMain.Label5, FrmMain.Label6, FrmMain.Label7, FrmMain.Label8, FrmMain.Label9, FrmMain.Label10,
                FrmMain.Label11, FrmMain.Label12, FrmMain.Label13, FrmMain.Label14, FrmMain.Label15, FrmMain.Label16, FrmMain.Label17, FrmMain.Label18}

    Public ReadOnly LblStArr As Label() = {FrmMain.LblSt1, FrmMain.LblSt2, FrmMain.LblSt3, FrmMain.LblSt4, FrmMain.LblSt5, FrmMain.LblSt6}
    Public ReadOnly EOpt As RadioButton() = {FrmMain.RbElev0, FrmMain.RbElev1}

    ''Google
    Public ReadOnly GElev = $"elevation.json"

    Public Altitude As Double = 0.0

    Public Cpy As String
    Public LMsg As String
    'Public ReadOnly Updatetime As Integer = 0

    'Astro
    Public Daylight As Boolean = False

    Public Subduration As TimeSpan = New TimeSpan(60)

    ''UV Forecast
    Public ReadOnly UvFil As String = $"uv-fc_{Now.ToString("Mdyy")}.json"

    Public UvNfo As UvFcast

    ''Real-time
    Public ReadOnly RtFil As String = $"uv-rt_{Now.ToString("Mdyy")}.json"

    Public RtNfo As UvRtCast

    'Public OzNfo As Object
    Public OzLevel As String

    ''Protection
    Public ReadOnly ProtFil As String = $"uv-pt_{Now.ToString("Mdyy")}.json"

    Public ProtNfo As Dpt

    Public Const UseAgent As String = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3794.0 Safari/537.36 Edg/80.0.361.5"
End Module