Imports Xm.Core
Imports System.Text

Namespace Library
    Public Class [Integer]
        <Method(Types.Integer, "abs")>
        Public Shared Function Abs(value As Integer) As Integer
            Return System.Math.Abs(value)
        End Function
        <Method(Types.Integer, "chr")>
        Public Shared Function Chr(value As Integer) As String
            Return Strings.ChrW(value).ToString
        End Function
        <Method(Types.Integer, "hex")>
        Public Shared Function Hexadecimal(value As Integer) As String
            Return String.Format("0x{0}", value.ToString("X"))
        End Function
    End Class
End Namespace