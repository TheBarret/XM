Imports Xm.Core
Imports Xm.Utilities
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Globalization

Namespace Library
    Public Class [String]
        <Method(Types.String, "length")>
        Public Shared Function Length(str As String) As Integer
            Return str.Length
        End Function
        <Method(Types.String, "empty")>
        Public Shared Function IsEmpty(str As String) As Boolean
            Return String.IsNullOrEmpty(str)
        End Function
        <Method(Types.String, "insert")>
        Public Shared Function Insert(str As String, i As Integer, value As String) As String
            If (i >= 0 AndAlso i <= str.Length) Then
                Return str.Insert(i, value)
            End If
            Return str
        End Function
        <Method(Types.String, "zip")>
        Public Shared Function Zip(str As String) As String
            Return Convert.ToBase64String(Encoding.UTF8.GetBytes(str).Compress)
        End Function
        <Method(Types.String, "unzip")>
        Public Shared Function Unzip(str As String) As String
            Return Encoding.UTF8.GetString(Convert.FromBase64String(str).Decompress)
        End Function
        <Method(Types.String, "trim")>
        Public Shared Function Trim(str As String) As String
            Return str.Trim
        End Function
        <Method(Types.String, "asc")>
        Public Shared Function Asc(str As String) As Integer
            Return Strings.AscW(str)
        End Function
        <Method(Types.String, "ucase")>
        Public Shared Function UCase(str As String) As String
            Return Strings.UCase(str)
        End Function
        <Method(Types.String, "lcase")>
        Public Shared Function LCase(str As String) As String
            Return Strings.LCase(str)
        End Function
        <Method(Types.String, "reverse")>
        Public Shared Function Reverse(str As String) As String
            Return Strings.StrReverse(str)
        End Function
        <Method(Types.String, "replace")>
        Public Shared Function Replace(str As String, f As String, r As String) As String
            Return Strings.Replace(str, f, r)
        End Function
        <Method(Types.String, "left")>
        Public Shared Function LeftOfString(str As String, len As Integer) As String
            Return Strings.Left(str, len)
        End Function
        <Method(Types.String, "right")>
        Public Shared Function RightOfString(str As String, len As Integer) As String
            Return Strings.Right(str, len)
        End Function
        <Method(Types.String, "substr")>
        Public Shared Function SubStr(str As String, index As Integer, len As Integer) As String
            Return str.Substring(index, len)
        End Function
        <Method(Types.String, "hex")>
        Public Shared Function Hexadecimal(str As String) As String
            Return String.Concat(Text.Encoding.UTF8.GetBytes(str).Select(Function(v) v.ToString("X")))
        End Function
    End Class
End Namespace