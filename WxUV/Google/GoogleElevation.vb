Imports Newtonsoft.Json
Imports System.Runtime.CompilerServices

Namespace Google

    Partial Public Class Goog

        <JsonProperty("results")>
        Public Property Results As Result()

        <JsonProperty("status")>
        Public Property Status As String

    End Class

    Partial Public Class Result

        <JsonProperty("elevation")>
        Public Property Elevation As Double

        <JsonProperty("location")>
        Public Property Location As Location

        <JsonProperty("resolution")>
        Public Property Resolution As Double

    End Class

    Partial Public Class Location

        <JsonProperty("lat")>
        Public Property Lat As Double

        <JsonProperty("lng")>
        Public Property Lng As Double

    End Class

    Partial Public Class Goog

        Public Shared Function FromJson(ByVal json As String) As Goog
            Return JsonConvert.DeserializeObject(Of Goog)(json, Converter.Settings)
        End Function

    End Class

    Module Serialize

        <Extension()>
        Function ToJson(ByVal self As Goog) As String
            Return JsonConvert.SerializeObject(self, Converter.Settings)
        End Function

    End Module

    Public Class Converter

        Public Shared ReadOnly Settings As JsonSerializerSettings = New JsonSerializerSettings With {.MetadataPropertyHandling = MetadataPropertyHandling.Ignore, .DateParseHandling = DateParseHandling.None}
    End Class

End Namespace

''{
''"results" : [
''{
''"elevation" : 77.50787353515625,
''"location" : {
''"lat" : 35.625556,
''"lng" : -78.328611
''},
''"resolution" : 4.771975994110107
''}
''],
''"status" : "OK"
''}