Imports Xm.Core
Imports Xm.Utilities
Imports System.IO
Imports System.Reflection

Namespace Library
    Public Class Core
        <Method(Types.Null, "print")>
        Public Shared Sub Print(rt As Runtime, str As Object)
            If (rt.HasOutput) Then
                rt.Output.WriteLine(str)
            Else
                Console.WriteLine(str)
            End If
        End Sub
        <Method(Types.Null, "read")>
        Public Shared Function Read(rt As Runtime) As String
            If (rt.HasInput) Then
                Return rt.Input.ReadLine()
            Else
                Return Console.ReadLine
            End If
        End Function
        <Method(Types.Null, "readall")>
        Public Shared Function ReadAll(rt As Runtime) As String
            If (rt.HasInput) Then
                Return rt.Input.ReadToEnd
            Else
                Return Console.ReadLine
            End If
        End Function
        <Method(Types.Any, "type")>
        Public Shared Function Type(value As Object) As String
            Return New TValue(value).ScriptType.ToString
        End Function
        <Method(Types.Any, "int")>
        Public Shared Function ToInteger(value As Object) As Integer
            Dim result As Integer = 0
            If (Not Integer.TryParse(value.ToString, Env.Integers, Env.Culture, result)) Then
                Return CInt(value)
            End If
            Return result
        End Function
        <Method(Types.Any, "float")>
        Public Shared Function ToFloat(value As Object) As Single
            Dim result As Single = 0
            If (Not Single.TryParse(value.ToString, Env.Floats, Env.Culture, result)) Then
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
        <Method(Types.Any, "isnull")>
        Public Shared Function IsNull(value As Object) As Boolean
            Dim tval As New TValue(value)
            Return tval.IsNull
        End Function
        <Method(Types.Any, "isnumber")>
        Public Shared Function IsNumber(value As Object) As Boolean
            Dim tval As New TValue(value)
            Return tval.IsInteger Or tval.IsFloat
        End Function
        <Method(Types.Null, "isset")>
        Public Shared Function IsSet(rt As Runtime, ref As Object) As Boolean
            Return rt.Scope.Variable(ref.ToString)
        End Function
        <Method(Types.Null, "isprimitive")>
        Public Shared Function IsPrimitive(value As Object) As Boolean
            Return New TValue(value).IsPrimitive
        End Function
        <Method(Types.Null, "location")>
        Public Shared Function Location() As String
            Return Environment.CurrentDirectory
        End Function
        <Method(Types.Null, "serialize")>
        Public Shared Function Serialize(expr As String) As String
            Return String.Format("{0}", String.Join(",", Serializer.Create(expr)))
        End Function
        <Method(Types.Null, "version")>
        Public Shared Function Version() As String
            Return Assembly.GetExecutingAssembly.GetName.Version.ToString
        End Function
    End Class
End Namespace