Imports System.Runtime.CompilerServices
Imports Newtonsoft.Json

Namespace RealTime
#Disable Warning IDE1006 ' Naming Styles

    Partial Friend Class UvRtCast
        Public Property result As Result
    End Class

    Partial Public Class Result
        Public Property uv As Double
        Public Property uv_time As Date
        Public Property uv_max As Single
        Public Property uv_max_time As Date
        Public Property ozone As Double
        Public Property ozone_time As Date
        Public Property safe_exposure_time As Safe_Exposure_Time
        Public Property sun_info As Sun_Info
    End Class

    Partial Public Class Safe_Exposure_Time
        Public Property st1 As Object
        Public Property st2 As Object
        Public Property st3 As Object
        Public Property st4 As Object
        Public Property st5 As Object
        Public Property st6 As Object
    End Class

    Partial Public Class Sun_Info
        Public Property sun_times As Sun_Times
        Public Property sun_position As Sun_Position
    End Class

    Partial Public Class Sun_Times
        Public Property solarNoon As Date
        Public Property nadir As Date
        Public Property sunrise As Date
        Public Property sunset As Date
        Public Property sunriseEnd As Date
        Public Property sunsetStart As Date
        Public Property dawn As Date
        Public Property dusk As Date
        Public Property nauticalDawn As Date
        Public Property nauticalDusk As Date
        Public Property nightEnd As Date
        Public Property night As Date
        Public Property goldenHourEnd As Date
        Public Property goldenHour As Date
    End Class

    Partial Public Class Sun_Position
        Public Property azimuth As Decimal
        Public Property altitude As Decimal
    End Class

#Enable Warning IDE1006 ' Naming Styles

    Partial Friend Class UvRtCast

        Friend Shared Function FromJson(json As String) As UvRtCast
            ''https://stackoverflow.com/questions/31813055/how-to-handle-null-empty-values-in-jsonconvert-deserializeobject
            Dim settings = New JsonSerializerSettings With {
                    .NullValueHandling = NullValueHandling.Ignore,
                    .MissingMemberHandling = MissingMemberHandling.Ignore
                    }
            Return JsonConvert.DeserializeObject(Of UvRtCast)(json, settings)
        End Function

    End Class

    Module Serialize

        <Extension()>
        Function ToJson(self As UvRtCast) As String
            Return JsonConvert.SerializeObject(self, Converter.Settings)
        End Function

    End Module

    Friend Class Converter

        Public Shared ReadOnly Settings As JsonSerializerSettings = New JsonSerializerSettings With {.MetadataPropertyHandling = MetadataPropertyHandling.Ignore, .DateParseHandling = DateParseHandling.None}
    End Class

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