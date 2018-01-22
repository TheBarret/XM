Imports Xm.Core
Imports System.IO

Namespace Library
    Public Class Encoders
        <Method(Types.Null, "base64e")>
        Public Shared Function Base64Encode(str As Object) As String
            Return Convert.ToBase64String(Text.Encoding.UTF8.GetBytes(str.ToString))
        End Function
        <Method(Types.Null, "base64d")>
        Public Shared Function Base64Decode(str As Object) As String
            Return Text.Encoding.UTF8.GetString(Convert.FromBase64String(str.ToString))
        End Function
    End Class
End Namespace