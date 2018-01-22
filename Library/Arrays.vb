Imports Xm.Core

Namespace Library
    Public Class Arrays
        <Method(Types.Array, "count")>
        Public Shared Function Count(value As List(Of TValue)) As Integer
            Return value.Count
        End Function
        <Method(Types.Array, "first")>
        Public Shared Function First(value As List(Of TValue)) As Object
            If (value.Count > 0) Then
                Return value.First.Value
            End If
            Return Nothing
        End Function
        <Method(Types.Array, "last")>
        Public Shared Function Last(value As List(Of TValue)) As Object
            If (value.Count > 0) Then
                Return value.Last.Value
            End If
            Return Nothing
        End Function
        <Method(Types.Array, "concat")>
        Public Shared Function Concat(value As List(Of TValue)) As String
            If (value.Count > 0) Then
                Return String.Concat(value.Select(Function(x) x.Value))
            End If
            Return String.Empty
        End Function
        <Method(Types.Array, "join")>
        Public Shared Function Join(value As List(Of TValue), separator As String) As String
            If (value.Count > 0) Then
                Return String.Join(separator, value.Select(Function(x) x.Value))
            End If
            Return String.Empty
        End Function
        <Method(Types.Array, "indexof")>
        Public Shared Function IndexOf(value As List(Of TValue), v As Object) As Object
            If (value.Count > 0) Then
                Return value.IndexOf(value.Where(Function(x) x.Value.Equals(v)).FirstOrDefault)
            End If
            Return String.Empty
        End Function
        <Method(Types.Array, "format")>
        Public Shared Function Format(value As List(Of TValue), str As String) As String
            If (value.Count > 0) Then
                Return String.Format(str, value.Select(Function(x) x.Value).ToArray)
            End If
            Return String.Empty
        End Function
    End Class
End Namespace