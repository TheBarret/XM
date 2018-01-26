Namespace Core.Elements.Types
    Public NotInheritable Class TCollection
        Inherits Expression
        Public Property Values As List(Of Expression)
        Sub New()
            Me.Values = New List(Of Expression)
        End Sub
        Sub New(Values As List(Of Expression))
            Me.Values = Values
        End Sub
        Public Overrides Function ToString() As String
            Return String.Format("[{0}]", String.Join(",", Me.Values))
        End Function
    End Class
End Namespace