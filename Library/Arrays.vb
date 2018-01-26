Imports Xm.Core

Namespace Library
    Public Class Arrays
        <Method(Types.Array, "count")>
        Public Shared Function Count(collection As List(Of Object)) As Integer
            Return collection.Count
        End Function
        <Method(Types.Array, "first")>
        Public Shared Function First(collection As List(Of Object)) As Object
            If (collection.Any) Then
                Return collection.First
            End If
            Return Nothing
        End Function
        <Method(Types.Array, "last")>
        Public Shared Function Last(collection As List(Of Object)) As Object
            If (collection.Any) Then
                Return collection.Last
            End If
            Return Nothing
        End Function
        <Method(Types.Array, "concat")>
        Public Shared Function Concat(collection As List(Of Object)) As String
            If (collection.Any) Then
                Return String.Concat(collection)
            End If
            Return String.Empty
        End Function
        <Method(Types.Array, "join")>
        Public Shared Function Join(collection As List(Of Object), separator As String) As String
            If (collection.Any) Then
                Return String.Join(separator, collection)
            End If
            Return String.Empty
        End Function
        <Method(Types.Array, "add")>
        Public Shared Function Add(collection As List(Of Object), value As Object) As List(Of Object)
            collection.Add(value)
            Return collection
        End Function
        <Method(Types.Array, "replace")>
        Public Shared Function Replace(collection As List(Of Object), value As Object, replacement As Object) As List(Of Object)
            If (collection.Any) Then
                If (collection.Any(Function(x) x.Equals(value))) Then
                    collection(collection.IndexOf(value)) = replacement
                End If
            End If
            Return collection
        End Function
        <Method(Types.Array, "remove")>
        Public Shared Function Remove(collection As List(Of Object), value As Object) As List(Of Object)
            If (collection.Any) Then
                If (collection.Any(Function(x) x.Equals(value))) Then
                    collection.RemoveAll(Function(x) x.Equals(value))
                End If
            End If
            Return collection
        End Function
        <Method(Types.Array, "removeat")>
        Public Shared Function RemoveAt(collection As List(Of Object), index As Integer) As List(Of Object)
            If (collection.Any) Then
                If (index > -1 AndAlso index <= collection.Count - 1) Then
                    collection.RemoveAt(index)
                End If
            End If
            Return collection
        End Function
        <Method(Types.Array, "insert")>
        Public Shared Function Insert(collection As List(Of Object), index As Integer, value As Object) As List(Of Object)
            collection.Insert(index, value)
            Return collection
        End Function
        <Method(Types.Array, "indexof")>
        Public Shared Function IndexOf(collection As List(Of Object), value As Object) As Object
            If (collection.Any) Then
                Return collection.IndexOf(collection.Where(Function(x) x.Equals(value)).FirstOrDefault)
            End If
            Return String.Empty
        End Function
        <Method(Types.Array, "ascend")>
        Public Shared Function SortAscending(collection As List(Of Object)) As List(Of Object)
            Return collection.OrderBy(Function(x) x).ToList
        End Function
        <Method(Types.Array, "descend")>
        Public Shared Function SortDescending(collection As List(Of Object)) As List(Of Object)
            Return collection.OrderByDescending(Function(x) x).ToList
        End Function
        <Method(Types.Array, "shuffle")>
        Public Shared Function Shuffle(collection As List(Of Object)) As List(Of Object)
            Static rnd As New Random(Environment.TickCount)
            Dim n As Integer = collection.Count
            While n > 1
                n -= 1
                Dim k As Integer = rnd.Next(n + 1)
                Dim value As Object = collection(k)
                collection(k) = collection(n)
                collection(n) = value
            End While
            Return collection
        End Function
        <Method(Types.Array, "format")>
        Public Shared Function Format(collection As List(Of Object), str As String) As String
            If (collection.Any) Then
                Return String.Format(str, collection.ToArray)
            End If
            Return String.Empty
        End Function
    End Class
End Namespace