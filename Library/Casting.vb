Imports Xm.Core
Imports Xm.Utilities

Namespace Library
    Public Class Casting
        <Method(Types.Any, "int")>
        Public Shared Function ToInteger(value As Object) As Integer
            Dim result As Integer = 0
            If (Not Integer.TryParse(value.ToString, result)) Then
                Return CInt(value)
            End If
            Return result
        End Function
        <Method(Types.Any, "float")>
        Public Shared Function ToFloat(value As Object) As Single
            Dim result As Single = 0
            If (Not Single.TryParse(value.ToString, result)) Then
                Return CSng(value)
            End If
            Return result
        End Function
        <Method(Types.Any, "bool")>
        Public Shared Function ToBoolean(value As Object) As Boolean
            Dim result As Boolean = False
            Boolean.TryParse(value.ToString, result)
            Return result
        End Function
        <Method(Types.Any, "string")>
        Public Shared Function ToStr(value As Object) As String
            Return value.ToString
        End Function
        <Method(Types.Any, "parse")>
        Public Shared Function TryParse(value As Object) As Object
            Dim strvalue As String = value.ToString
            If (Not TypeOf value Is Null AndAlso strvalue.Length > 0) Then
                If (Boolean.TryParse(strvalue, Nothing)) Then
                    Return Boolean.Parse(strvalue)
                ElseIf (Integer.TryParse(strvalue, Nothing)) Then
                    Return Integer.Parse(strvalue, Env.Integers, Env.Culture)
                ElseIf (Single.TryParse(strvalue, Nothing)) Then
                    Return Single.Parse(strvalue, Env.Floats, Env.Culture)
                ElseIf (strvalue.ToLower.StartsWith("0x") Or strvalue.ToLower.StartsWith("&H")) Then
                    Dim int As Integer = 0
                    If (Integer.TryParse(strvalue.Remove("0x", "&h"), Env.Hexadecimal, Env.Culture, int)) Then
                        Return int
                    End If
                End If
            End If
            Return value
        End Function
    End Class
End Namespace