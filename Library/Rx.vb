Imports Xm.Core
Imports System.Text
Imports System.Text.RegularExpressions

Namespace Library
    Public Class Rx
        <Method(Types.Null, "rxt")>
        Public Shared Function RegexIsMatch(str As String, pattern As String) As Boolean
            Return Regex.IsMatch(str, pattern, RegexOptions.IgnoreCase Or RegexOptions.Singleline)
        End Function
        <Method(Types.Null, "rxm")>
        Public Shared Function RegexGetMatch(str As String, pattern As String) As String
            Dim m As Match = Regex.Match(str, pattern, RegexOptions.IgnoreCase Or RegexOptions.Singleline)
            If (m.Success) Then
                Return m.Value
            End If
            Return String.Empty
        End Function
        <Method(Types.Null, "rxmall")>
        Public Shared Function RegexGetMatches(str As String, pattern As String) As String
            Dim results As New List(Of String)
            For Each m As Match In Regex.Matches(str, pattern, RegexOptions.IgnoreCase Or RegexOptions.Singleline)
                If (m.Success) Then
                    results.Add(m.Value)
                End If
            Next
            Return String.Join(",", results)
        End Function
    End Class
End Namespace