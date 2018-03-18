Imports Microsoft.Win32

Module Globals

    Public ReadOnly KSet As RegistryKey = Registry.CurrentUser.CreateSubKey("Software\PAROLE Software\WxUV\Setup")
    Public ReadOnly KMet As RegistryKey = Registry.CurrentUser.CreateSubKey("Software\PAROLE Software\WxUV\Metrics")
    Public ReadOnly KInfo As RegistryKey = Registry.CurrentUser.CreateSubKey("Software\PAROLE Software\WxUV\Information")
    Public ReadOnly KTwit As RegistryKey = Registry.CurrentUser.CreateSubKey("Software\PAROLE Software\WxUV\Tokens\Consumer\Twitter\Keys")
    Public ReadOnly Kuv As RegistryKey = Registry.CurrentUser.CreateSubKey("Software\PAROLE Software\WxUV\Tokens\Consumer\OpenUV\Api")
    Public ReadOnly Kg As RegistryKey = Registry.CurrentUser.CreateSubKey("Software\PAROLE Software\WxUV\Tokens\Consumer\Google\Api")

    ''OpenUV API Key
    Public ApiKey As String = ""

    Public Keyset As Boolean = False

    ''Google Elevation API Key
    Public GoogleKey As String = ""

    Public cLatitude As Double = KSet.GetValue("Latitude", "37.787644")
    Public cLongitude As Double = KSet.GetValue("Longitude", "-79.44189")

    Public Const TempDir As String = "$tmp"
    Public TempPath As String

    Public Const LogDir As String = "Logs"
    Public LogPath As String
    Public LogFile As String

    Public UvArr(17) As PictureBox
    Public LblArr(17) As Label
    Public LblStArr(5) As Label

    ''Google
    Public GElev = $"elevation.json"

    Public Altitude As Double = 0.0
    Public GNfo As Object

    ''log items

    Public Separator As String = "-----------------------------"
    Public Squiggley As String = "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"

    ''main items
    Public TimerMultiplier As Long = 60000

    Public Cpy As String
    Public LMsg As String
    Public Updatetime As Integer = 0

    'Astro
    Public Daylight As Boolean = False

    Public Subduration As TimeSpan = New TimeSpan(60)
    Public ssFil As String = $"astro_{Now.ToString("Mdyy")}.json"
    Public ssNfo As Object

    ''UV Forecast
    Public UvFil As String = $"uv-fc_{Now.ToString("Mdyy")}.json"

    Public UvNfo As Object

    ''Real-time
    Public RtFil As String = $"uv-rt_{Now.ToString("Mdyy")}.json"

    Public RtNfo As Object

    Public OzNfo As Object
    Public OzLevel As String

    ''Protection
    Public ProtFil As String = $"uv-pt_{Now.ToString("Mdyy")}.json"

    Public ProtNfo As Object
    Public FrmNdx As Double = 0
    Public ToNdx As Double = 0

    ''Auth variables
    Public AuthPath As String = ""

    Public Const AuthDir As String = "Auth"
    Public AuthFile As String
    Public Const TwitterAuthfile As String = "Twitter.auth"

    '' Twitter App Keys
    Public ConsumerKey As String = ""

    Public ConsumerKeySecret As String = ""

End Module