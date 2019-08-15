Imports System.Runtime.CompilerServices
Imports Newtonsoft.Json

Namespace Protection

    Partial Public Class Dpt

        <JsonProperty("result")>
        Public Property Result As Result

    End Class

    Partial Public Class Result

        <JsonProperty("from_time")>
        Public Property FromTime As DateTime

        <JsonProperty("from_uv")>
        Public Property FromUv As Double

        <JsonProperty("to_time")>
        Public Property ToTime As DateTime

        <JsonProperty("to_uv")>
        Public Property ToUv As Double

    End Class

    Partial Public Class Dpt

        Friend Shared Function FromJson(json As String) As Dpt
            ''https://stackoverflow.com/questions/31813055/how-to-handle-null-empty-values-in-jsonconvert-deserializeobject
            Dim settings = New JsonSerializerSettings With {
                    .NullValueHandling = NullValueHandling.Ignore,
                    .MissingMemberHandling = MissingMemberHandling.Ignore
                    }
            Return JsonConvert.DeserializeObject(Of Dpt)(json, settings)
        End Function

    End Class

    Module Serialize

        <Extension()>
        Function ToJson(self As Dpt) As String
            Return JsonConvert.SerializeObject(self, Converter.Settings)
        End Function

    End Module

    Friend Class Converter

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