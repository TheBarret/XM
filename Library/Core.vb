Imports Xm.Core
Imports Xm.Utilities
Imports System.IO
Imports System.Reflection

Namespace Library
    Public Class Core
        <Method(Types.Any, "type")>
        Public Shared Function Type(value As Object) As String
            Dim tval As New TValue(value)
            Return tval.ScriptType.ToString
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