Imports Xm.Utilities
Imports Xm.Core
Imports Xm.Core.Elements
Imports Xm.Core.Elements.Types

Namespace Syntax
    Partial Public Class Parser
        Private Function ParseNull() As Expression
            Try
                Return New TNull
            Finally
                Me.NextToken()
            End Try
        End Function
        Private Function ParseBoolean() As Expression
            Try
                Return New TBoolean(Boolean.Parse(Me.Current.Value))
            Finally
                Me.NextToken()
            End Try
        End Function
        Private Function ParseString() As Expression
            Try
                Return New TString(Me.Current.Value.Substring(1, Me.Current.Value.Length - 2))
            Finally
                Me.NextToken()
            End Try
        End Function
        Private Function ParseIdentifier() As Expression
            Try
                Return New TIdentifier(Me.Current.Value)
            Finally
                Me.NextToken()
            End Try
        End Function
        Private Function ParseSignedParentheses(op As Tokens) As Expression
            Return New Unary(Me.ParseExpression(False), op, True)
        End Function
        Private Function ParseSignedIdentifier(op As Tokens) As Expression
            Try
                Return New Unary(New TIdentifier(Me.Current.Value), op, True)
            Finally
                Me.NextToken()
            End Try
        End Function
        Private Function ParseHexadecimal(signed As Boolean, Optional op As Tokens = Tokens.T_Null) As Expression
            Try
                If (signed And op = Tokens.T_Minus) Then
                    Return New TInteger(Integer.Parse(Me.Current.Value.Remove("0x", "&h"), Env.Hexadecimal) * -1)
                End If
                Return New TInteger(Integer.Parse(Me.Current.Value.Remove("0x", "&h"), Env.Hexadecimal) * 1)
            Finally
                Me.NextToken()
            End Try
        End Function
        Private Function ParseInteger(signed As Boolean, Optional op As Tokens = Tokens.T_Null) As Expression
            Try
                If (signed And op = Tokens.T_Minus) Then
                    Return New TInteger(Integer.Parse(Me.Current.Value, Env.Integers) * -1)
                End If
                Return New TInteger(Integer.Parse(Me.Current.Value, Env.Integers) * 1)
            Finally
                Me.NextToken()
            End Try
        End Function
        Private Function ParseFloat(signed As Boolean, Optional op As Tokens = Tokens.T_Null) As Expression
            Try
                If (signed And op = Tokens.T_Minus) Then
                    Return New TFloat(Single.Parse(Me.Current.Value, Env.Floats) * -1.0F)
                End If
                Return New TFloat(Single.Parse(Me.Current.Value, Env.Floats) * 1.0F)
            Finally
                Me.NextToken()
            End Try
        End Function
        Private Function ParseArray() As Expression
            Return New TArray(Me.ParseTuples(Tokens.T_BracketOpen, Tokens.T_BracketClose))
        End Function
        Private Function ParseReturn() As Expression
            Me.NextToken()
            Return New TReturn(Me.ParseExpression(False))
        End Function
        Private Function ParseParenthesis() As Expression
            Try
                Me.NextToken()
                Return Me.ParseExpression(False)
            Finally
                Me.Match(Tokens.T_ParenthesisClose)
            End Try
        End Function
        Private Function ParseCondition() As Expression
            Me.NextToken()
            Dim e As New TConditional(Me.ParseExpression(False)) With {.True = Me.ParseBraceBlock}
            If (Me.Current.Type = Tokens.T_Else) Then
                Me.NextToken()
                e.False.AddRange(Me.ParseBraceBlock)
            End If
            Return e
        End Function
        Private Function ParseFunction() As Expression
            Me.NextToken()
            If (Me.Current.Type = Tokens.T_Identifier) Then
                Return New TFunction(CType(Me.ParseIdentifier, TIdentifier), Me.ParseTuples(Tokens.T_ParenthesisOpen, Tokens.T_ParenthesisClose), Me.ParseBraceBlock)
            Else
                Return New TFunction(New TIdentifier(Env.RandomName(8, "func_")), Me.ParseTuples(Tokens.T_ParenthesisOpen, Tokens.T_ParenthesisClose), Me.ParseBraceBlock)
            End If
        End Function
        Private Function ParseFunctionIL() As Expression
            Dim e As TFunction = Nothing
            Me.NextToken()
            If (Me.Current.Type = Tokens.T_Identifier) Then
                e = New TFunction(CType(Me.ParseIdentifier, TIdentifier), Me.ParseTuples(Tokens.T_ParenthesisOpen, Tokens.T_ParenthesisClose), Me.ParseBraceBlock(False))
            Else
                e = New TFunction(New TIdentifier(Env.RandomName(8, "func_")), Me.ParseTuples(Tokens.T_ParenthesisOpen, Tokens.T_ParenthesisClose), Me.ParseBraceBlock(False))
            End If
            If (e.Body.Count = 1) Then
                If (Not TypeOf e.Body.First Is TReturn) Then
                    e.Body(0) = New TReturn(e.Body.First)
                End If
            ElseIf (e.Body.Count > 1) Then
                Throw New Exception("Inline functions only accept one expression")
            End If
            Return e
        End Function
        Private Function ParseBraceBlock(Optional ExpectEnd As Boolean = True) As List(Of Expression)
            Dim body As New List(Of Expression)
            Me.Match(Tokens.T_BraceOpen)
            Do While True
                If (Me.Current.Type = Tokens.T_BraceClose) Then
                    Me.NextToken()
                    Exit Do
                ElseIf (Me.Current.Type = Tokens.T_EndOfFile) Then
                    Throw New Exception(String.Format("Unexpected end of file at line {0}", Me.Current.Line))
                Else
                    body.Add(Me.ParseExpression(ExpectEnd))
                End If
            Loop
            Return body
        End Function
        Private Function ParseTuples(Open As Tokens, Close As Tokens) As List(Of Expression)
            Dim params As New List(Of Expression)
            Me.Match(Open)
            While Me.Current.Type <> Close
                params.Add(Me.ParseExpression(False))
                If (Me.Current.Type <> Tokens.T_Comma) Then
                    Exit While
                End If
                Me.NextToken()
            End While
            Me.Match(Close)
            Return params
        End Function
        Private Function ParseImport() As Expression
            Me.NextToken()
            Return New TUse(Me.ParseExpression(True))
        End Function
    End Class
End Namespace