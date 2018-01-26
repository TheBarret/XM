Namespace Core.Elements
    Public NotInheritable Class TCollectionValue
        Inherits Expression
        Public Property Index As Expression
        Public Property Identifier As Expression
        Sub New(ident As Expression, index As Expression)
            Me.Identifier = ident
            Me.Index = index
        End Sub
        Public Overrides Function ToString() As String
            Return String.Format("{0}[{1}]", Me.Identifier.ToString, Me.Index.ToString)
        End Function
    End Class
End Namespace