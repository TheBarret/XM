Imports Xm.Core
Imports System.Globalization

Namespace Library
    Public Class [Date]
        <Method(Types.Null, "date")>
        Public Shared Function CurrentDateAndTime() As String
            Return String.Format("{0}", DateTime.Now.ToString("f"))
        End Function
    End Class
End Namespace