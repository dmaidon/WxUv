'Imports System.Runtime.CompilerServices
Imports Newtonsoft.Json

Namespace RealTime

    Partial Public Class UvRtCast
        <JsonProperty("result")> Public Property Result() As Result
    End Class

    Partial Public Class Result
        <JsonProperty("uv")> Public Property Uv As Double

        <JsonProperty("uv_time")> Public Property UvTime As Date

        <JsonProperty("uv_max")> Public Property UvMax As Single

        <JsonProperty("uv_max_time")> Public Property UvMaxTime As Date

        <JsonProperty("ozone")> Public Property Ozone As Double

        <JsonProperty("ozone_time")> Public Property OzoneTime As Date

        <JsonProperty("safe_exposure_time")> Public Property SafeExposureTime As SafeExposureTime

        <JsonProperty("sun_info")> Public Property SunInfo As SunInfo
    End Class

    Partial Public Class SafeExposureTime
        <JsonProperty("st1")> Public Property St1 As Integer

        <JsonProperty("st2")> Public Property St2 As Integer

        <JsonProperty("st3")> Public Property St3 As Integer

        <JsonProperty("st4")> Public Property St4 As Integer

        <JsonProperty("st5")> Public Property St5 As Integer

        <JsonProperty("st6")> Public Property St6 As Integer
    End Class

    Partial Public Class SunInfo
        <JsonProperty("sun_times")> Public Property SunTimes As SunTimes

        <JsonProperty("sun_position")> Public Property SunPosition As SunPosition
    End Class

    Partial Public Class SunTimes
        <JsonProperty("solarNoon")> Public Property SolarNoon As Date

        <JsonProperty("nadir")> Public Property NaDir As Date

        <JsonProperty("sunrise")> Public Property Sunrise As Date

        <JsonProperty("sunset")> Public Property Sunset As Date

        <JsonProperty("sunriseEnd")> Public Property SunriseEnd As Date

        <JsonProperty("sunsetStart")> Public Property SunsetStart As Date

        <JsonProperty("dawn")> Public Property Dawn As Date

        <JsonProperty("dusk")> Public Property Dusk As Date

        <JsonProperty("nauticalDawn")> Public Property NauticalDawn As Date

        <JsonProperty("nauticalDusk")> Public Property NauticalDusk As Date

        <JsonProperty("nightEnd")> Public Property NightEnd As Date

        <JsonProperty("night")> Public Property Night As Date

        <JsonProperty("goldenHourEnd")> Public Property GoldenHourEnd As Date

        <JsonProperty("goldenHour")> Public Property GoldenHour As Date
    End Class

    Partial Public Class SunPosition
        <JsonProperty("azimuth")> Public Property Azimuth As Decimal

        <JsonProperty("altitude")> Public Property Altitude As Decimal
    End Class

#Enable Warning IDE1006 ' Naming Styles

    Partial Public Class UvRtCast

        Friend Shared Function FromJson(json As String) As UvRtCast
            ''https://stackoverflow.com/questions/31813055/how-to-handle-null-empty-values-in-jsonconvert-deserializeobject
            Dim settings = New JsonSerializerSettings With {
                    .NullValueHandling = NullValueHandling.Ignore,
                    .MissingMemberHandling = MissingMemberHandling.Ignore
                    }
            Return JsonConvert.DeserializeObject(Of UvRtCast)(json, settings)
        End Function

    End Class

    'Module Serialize

    ' <Extension()> Function ToJson(self As UvRtCast) As String Return JsonConvert.SerializeObject(self, Converter.Settings) End Function

    'End Module

    'public Class Converter

    '    Public Shared ReadOnly Settings As JsonSerializerSettings = New JsonSerializerSettings With {.MetadataPropertyHandling = MetadataPropertyHandling.Ignore, .DateParseHandling = DateParseHandling.None}
    'End Class
End Namespace

''{
''"result": {
''"uv": 0,
''"uv_time": "2018-03-13T08:33:50.073Z",
''"uv_max": 3.9911,
''"uv_max_time": "2018-03-13T17:28:36.862Z",
''"ozone": 429.6,
''"ozone_time": "2018-03-13T06:05:54.517Z",
''"safe_exposure_time": {
''"st1": null,
''"st2": null,
''"st3": null,
''"st4": null,
''"st5": null,
''"st6": null
''},
''"sun_info": {
''"sun_times": {
''"solarNoon": "2018-03-13T17:28:36.862Z",
''"nadir": "2018-03-13T05:28:36.862Z",
''"sunrise": "2018-03-13T11:33:17.769Z",
''"sunset": "2018-03-13T23:23:55.955Z",
''"sunriseEnd": "2018-03-13T11:35:59.891Z",
''"sunsetStart": "2018-03-13T23:21:13.833Z",
''"dawn": "2018-03-13T11:07:08.018Z",
''"dusk": "2018-03-13T23:50:05.706Z",
''"nauticalDawn": "2018-03-13T10:36:42.159Z",
''"nauticalDusk": "2018-03-14T00:20:31.566Z",
''"nightEnd": "2018-03-13T10:06:00.615Z",
''"night": "2018-03-14T00:51:13.109Z",
''"goldenHourEnd": "2018-03-13T12:08:03.842Z",
''"goldenHour": "2018-03-13T22:49:09.882Z"
''},
''"sun_position": {
''"azimuth": -2.0541846218270616,
''"altitude": -0.6135944116185721
''}
''}
''}
''}