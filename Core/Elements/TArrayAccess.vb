Namespace Core.Elements
    Public NotInheritable Class TArrayAccess
        Inherits Expression
        Public Property Name As Expression
        Public Property Index As Expression
        Sub New(Name As Expression, index As Expression)
            Me.Name = Name
            Me.Index = index
        End Sub
        Public Overrides Function ToString() As String
            Return String.Format("{0}[{1}]", Me.Name.ToString, Me.Index.ToString)
        End Function
    End Class
End Namespace