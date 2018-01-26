Imports Xm.Core
Imports System.Text
Imports Rx = System.Text.RegularExpressions

Namespace Library
    Public Class Regex
        <Method(Types.Null, "rxt")>
        Public Shared Function RegexIsMatch(str As String, pattern As String) As Boolean
            Return Rx.Regex.IsMatch(str, pattern, Rx.RegexOptions.IgnoreCase Or Rx.RegexOptions.Singleline)
        End Function
        <Method(Types.Null, "rxm")>
        Public Shared Function RegexGetMatch(str As String, pattern As String) As String
            Dim m As Rx.Match = Rx.Regex.Match(str, pattern, Rx.RegexOptions.IgnoreCase Or Rx.RegexOptions.Singleline)
            If (m.Success) Then
                Return m.Value
            End If
            Return String.Empty
        End Function
        <Method(Types.Null, "rxmall")>
        Public Shared Function RegexGetMatches(str As String, pattern As String) As String
            Dim results As New List(Of String)
            For Each m As Rx.Match In Rx.Regex.Matches(str, pattern, Rx.RegexOptions.IgnoreCase Or Rx.RegexOptions.Singleline)
                If (m.Success) Then
                    results.Add(m.Value)
                End If
            Next
            Return String.Join(",", results)
        End Function
    End Class
End Namespace