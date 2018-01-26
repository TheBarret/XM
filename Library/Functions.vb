Imports Xm.Core
Imports Xm.Core.Elements.Types
Imports Xm.Utilities

Namespace Library
    Public Class Functions
        <Method(Types.Function, "name")>
        Public Shared Function GetFunctionReference(func As TFunction) As String
            Return func.Name.GetStringValue
        End Function
        <Method(Types.Function, "count")>
        Public Shared Function GetFunctionParamCount(func As TFunction) As Integer
            Return func.Parameters.Count
        End Function
    End Class
End Namespace