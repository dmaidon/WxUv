Imports System.Runtime.CompilerServices
Imports Newtonsoft.Json

Namespace Forecast

    ''https://app.quicktype.io/#r=json2csharp
    Partial Public Class UvFcast

        <JsonProperty("result")>
        Public Property Result As Result()

    End Class

    Partial Public Class Result

        <JsonProperty("uv")>
        Public Property Uv As Double

        <JsonProperty("uv_time")>
        Public Property UvTime As Date

        <JsonProperty("sun_position")>
        Public Property SunPosition As SunPosition

    End Class

    Partial Public Class SunPosition

        <JsonProperty("azimuth")>
        Public Property Azimuth As Double

        <JsonProperty("altitude")>
        Public Property Altitude As Double

    End Class

    Partial Public Class UvFcast

        Public Shared Function FromJson(json As String) As UvFcast
            Return JsonConvert.DeserializeObject(Of UvFcast)(json, Converter.Settings)
        End Function

    End Class

    Module Serialize

        <Extension>
        Function ToJson(self As UvFcast) As String
            Return JsonConvert.SerializeObject(self, Converter.Settings)
        End Function

    End Module

    Public Class Converter

        Public Shared ReadOnly _
            Settings As JsonSerializerSettings = New JsonSerializerSettings _
            With {.MetadataPropertyHandling = MetadataPropertyHandling.Ignore, .DateParseHandling = DateParseHandling.None}

    End Class

End Namespace

''{
''"result": [{
''"uv": 0,
''"uv_time": "2018-02-10T12:06:44.806Z"
''}, {
''"uv": 0.2561,
''"uv_time": "2018-02-10T13:06:44.806Z"
''}, {
''"uv": 0.9424,
''"uv_time": "2018-02-10T14:06:44.806Z"
''}, {
''"uv": 2.1102,
''"uv_time": "2018-02-10T15:06:44.806Z"
''}, {
''"uv": 3.3804,
''"uv_time": "2018-02-10T16:06:44.806Z"
''}, {
''"uv": 4.0155,
''"uv_time": "2018-02-10T17:06:44.806Z"
''}, {
''"uv": 4.0155,
''"uv_time": "2018-02-10T18:06:44.806Z"
''}, {
''"uv": 2.9911,
''"uv_time": "2018-02-10T19:06:44.806Z"
''}, {
''"uv": 1.8029,
''"uv_time": "2018-02-10T20:06:44.806Z"
''}, {
''"uv": 0.758,
''"uv_time": "2018-02-10T21:06:44.806Z"
''}, {
''"uv": 0.1434,
''"uv_time": "2018-02-10T22:06:44.806Z"
''}
'']
''}