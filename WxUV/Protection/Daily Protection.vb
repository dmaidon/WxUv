Imports System
Imports System.Net
Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports System.Runtime.CompilerServices

Namespace Protection

    Partial Public Class Dpt

        <JsonProperty("result")>
        Public Property Result As Result

    End Class

    Partial Public Class Result

        <JsonProperty("from_time")>
        Public Property FromTime As System.DateTime

        <JsonProperty("from_uv")>
        Public Property FromUv As Double

        <JsonProperty("to_time")>
        Public Property ToTime As System.DateTime

        <JsonProperty("to_uv")>
        Public Property ToUv As Double

    End Class

    Partial Public Class Dpt

        Public Shared Function FromJson(ByVal json As String) As Dpt
            Return JsonConvert.DeserializeObject(Of Dpt)(json, Protection.Converter.Settings)
        End Function

    End Class

    Module Serialize

        <Extension()>
        Function ToJson(ByVal self As Dpt) As String
            Return JsonConvert.SerializeObject(self, Protection.Converter.Settings)
        End Function

    End Module

    Public Class Converter

        Public Shared ReadOnly Settings As JsonSerializerSettings = New JsonSerializerSettings With {.MetadataPropertyHandling = MetadataPropertyHandling.Ignore, .DateParseHandling = DateParseHandling.None}
    End Class

End Namespace

''{
''"result": {
''"from_time": "2018-02-12T16:14:44.865Z",
''"from_uv": 3.6134,
''"to_time": "2018-02-12T18:44:44.865Z",
''"to_uv": 3.6134
''}
''}