Imports Xm.Core
Imports Xm.Core.Elements.Types
Imports Xm.Utilities

Namespace Library
    Public Class Linq
        <Method(Types.Array, "grab")>
        Public Shared Function GrabItemsWithCondition(rt As Runtime, collection As List(Of Object), func As TFunction) As List(Of Object)
            If (collection.Any AndAlso func.Parameters.Count >= 1) Then
                Dim buffer As New List(Of Object)
                For Each item As Object In collection
                    rt.Scope.SetVariable(func.Parameters.First.GetStringValue, New TValue(item))
                    Dim result As TValue = rt.Resolve(func, {item}.ToList)
                    If (result.IsBoolean AndAlso result.Cast(Of Boolean)()) Then
                        Buffer.Add(item)
                    End If
                Next
                Return Buffer
            End If
            Return Nothing
        End Function
    End Class
End Namespace