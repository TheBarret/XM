Imports Xm.Core
Imports System.Text
Imports System.Text.RegularExpressions

Namespace Lexer
    Public NotInheritable Class Parser
        Inherits List(Of Token)
        Private Property Length As Integer
        Private Property Flag As Boolean
        Private Property Line As Integer
        Private Property Index As Integer
        Private Property Position As Integer
        Sub New()
            Me.Line = 0
            Me.Index = 0
            Me.Position = 0
        End Sub
        Public Function Analyze(context As String) As List(Of Token)
            If (Not String.IsNullOrEmpty(context)) Then
                Me.Length = context.Length
                Using Definitions As New Syntax.Table
                    Do
                        Me.Flag = False
                        For Each Rule As KeyValuePair(Of Tokens, String) In Definitions
                            Dim match As Match = Definitions.Match(Rule.Key, context)
                            If (match.Success) Then
                                Me.Flag = True
                                context = context.Remove(match.Index, match.Length)
                                If (Rule.Key = Tokens.T_Newline) Then
                                    Me.Line += 1
                                ElseIf (Rule.Key = Tokens.T_LineComment) Then
                                    Me.Line += Regex.Matches(match.Value, "\r\n").Count
                                ElseIf (Rule.Key = Tokens.T_BlockComment) Then
                                    Me.Line += Regex.Matches(match.Value, "\r\n").Count
                                End If
                                Me.Add(New Token(match.Value, Rule.Key, Me.Line, Me.Index, match.Length))
                                Me.Index += match.Length
                            End If
                            If (Me.Flag) Then Exit For
                        Next
                        If (Not Me.Flag) Then
                            Throw New Exception(String.Format("Undefined character '{0}' at index {1} line {2}", context(0), Index, Line))
                        End If
                    Loop Until Index = Me.Length
                End Using
                Me.Add(Token.Create(Tokens.T_EndOfFile, String.Empty, Me.Line, Me.Index))
                Return Me.Trim(Tokens.T_Space, Tokens.T_Newline, Tokens.T_BlockComment, Tokens.T_LineComment)
            End If
            Throw New Exception("context is empty")
        End Function
        Public ReadOnly Property Trim(ParamArray Types() As Tokens) As List(Of Token)
            Get
                Return Me.Where(Function(token) Not Types.Contains(token.Type)).ToList
            End Get
        End Property
    End Class
End Namespace