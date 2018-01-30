Imports Xm.Utilities

Namespace Core.Elements
    Public NotInheritable Class Ternary
        Inherits Expression
        Public Property Condition As Expression
        Public Property [True] As Expression
        Public Property [False] As Expression
        Sub New(Condition As Expression, [True] As Expression, [False] As Expression)
            Me.Condition = Condition
            Me.True = [True]
            Me.False = [False]
        End Sub
        Public Overrides Function ToString() As String
            Return String.Format("{0} {1} {2}", Me.Condition.ToString, Me.True.ToString, Me.False.ToString)
        End Function
    End Class
End Namespace