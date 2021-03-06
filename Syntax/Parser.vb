﻿Imports Xm.Utilities
Imports Xm.Core
Imports Xm.Core.Elements

Namespace Syntax
    Public NotInheritable Class Parser
        Inherits List(Of Expression)
        Private Property Index As Integer
        Private Property Current As Token
        Public Property Stream As List(Of Token)
#Region "Recursive Parser"
        Public Function Analyze(stream As List(Of Token)) As List(Of Expression)
            If (stream.Count > 0) Then
                Me.Index = 0
                Me.Stream = stream
                Me.Current = stream.First
                Me.Parse()
            End If
            Return Me
        End Function
        Public Function Tokenize(stream As List(Of Token), offset As Byte, shift As Byte) As Byte()
            If (stream.Count > 0) Then
                Me.Index = 0
                Me.Stream = stream
                Me.Current = stream.First
                Me.Parse()
            End If
            Return Serializer.Create(Me.Stream, offset, shift)
        End Function
        Private Sub Parse()
            Do Until Me.Current.Type = Tokens.T_EndOfFile
                Me.Add(Me.ParseExpression(True))
            Loop
        End Sub
        Private Function ParseExpression(ExpectEnd As Boolean) As Expression
            Select Case Me.Current.Type
                Case Tokens.T_If
                    Return Me.ParseCondition
                Case Tokens.T_Use
                    Return Me.ParseImport
                Case Else
                    Dim e As Expression = Me.Assignment
                    If (ExpectEnd) Then
                        If (Me.Current.Type = Tokens.T_EndStatement) Then
                            Me.NextToken()
                        ElseIf (Me.Current.Type = Tokens.T_EndOfFile) Then
                            Me.NextToken()
                        Else
                            Throw New Exception(String.Format("Expecting end at '{0}' line {1} ", Me.Current.Type.Name, Me.Current.Line))
                        End If
                    ElseIf (e Is Nothing) Then
                        Throw New Exception(String.Format("Unexpected '{0}' at line {1} ", Me.Current.Type.Name, Me.Current.Line))
                    End If
                    Return e
            End Select
        End Function
        Private Function Assignment() As Expression
            Dim e As Expression = Me.Ternary
            While (Me.Current.Type = Tokens.T_Assign)
                Me.NextToken()
                e = New Binary(e, Tokens.T_Assign, Me.ParseExpression(False))
            End While
            Return e
        End Function
        Private Function Ternary() As Expression
            Dim e As Expression = Me.LogicalOr()
            While (Me.Current.Type = Tokens.T_QM)
                Me.NextToken()
                Dim a As Expression = Me.ParseExpression(False)
                Me.Match(Tokens.T_Colon)
                Dim b As Expression = Me.ParseExpression(False)
                e = New Ternary(e, a, b)
            End While
            Return e
        End Function
        Private Function LogicalOr() As Expression
            Dim e As Expression = Me.LogicalAnd()
            While (Me.Current.Type = Tokens.T_Or)
                Dim op As Tokens = Me.Current.Type
                Me.NextToken()
                e = New Binary(e, op, Me.LogicalAnd)
            End While
            Return e
        End Function
        Private Function LogicalAnd() As Expression
            Dim e As Expression = Me.Relational()
            While (Me.Current.Type = Tokens.T_And)
                Dim op As Tokens = Me.Current.Type
                Me.NextToken()
                e = New Binary(e, op, Me.Relational)
            End While
            Return e
        End Function
        Private Function Relational() As Expression
            Dim e As Expression = Me.BitShift()
            While (Me.Current.Type = Tokens.T_Equal) OrElse
                  (Me.Current.Type = Tokens.T_NotEqual) OrElse
                  (Me.Current.Type = Tokens.T_Greater) OrElse
                  (Me.Current.Type = Tokens.T_Lesser) OrElse
                  (Me.Current.Type = Tokens.T_EqualOrGreater) OrElse
                  (Me.Current.Type = Tokens.T_EqualOrLesser) OrElse
                  (Me.Current.Type = Tokens.T_Like)
                Dim op As Tokens = Me.Current.Type
                Me.NextToken()
                e = New Binary(e, op, Me.BitShift)
            End While
            Return e
        End Function
        Private Function BitShift() As Expression
            Dim e As Expression = Me.PostfixUnary()
            While (Me.Current.Type = Tokens.T_RShift) OrElse
                  (Me.Current.Type = Tokens.T_LShift)
                Dim op As Tokens = Me.Current.Type
                Me.NextToken()
                e = New Binary(e, op, Me.PostfixUnary)
            End While
            Return e
        End Function
        Private Function PostfixUnary() As Expression
            Dim e As Expression = Me.LogicalXor()
            '//TODO
            Return e
        End Function
        Private Function LogicalXor() As Expression
            Dim e As Expression = Me.AdditionOrSubtraction()
            While (Me.Current.Type = Tokens.T_Xor)
                Me.NextToken()
                e = New Binary(e, Tokens.T_Xor, Me.AdditionOrSubtraction())
            End While
            Return e
        End Function
        Private Function AdditionOrSubtraction() As Expression
            Dim e As Expression = Me.MultiplicationOrDivision()
            While (Me.Current.Type = Tokens.T_Plus) OrElse
                  (Me.Current.Type = Tokens.T_Minus)
                Dim Op As Tokens = Me.Current.Type
                Me.NextToken()
                e = New Binary(e, Op, Me.MultiplicationOrDivision)
            End While
            Return e
        End Function
        Private Function MultiplicationOrDivision() As Expression
            Dim e As Expression = Me.PrefixUnary()
            While (Me.Current.Type = Tokens.T_Mult) OrElse
                  (Me.Current.Type = Tokens.T_Div) OrElse
                  (Me.Current.Type = Tokens.T_Mod)
                Dim Op As Tokens = Me.Current.Type
                Me.NextToken()
                e = New Binary(e, Op, Me.PrefixUnary)
            End While
            Return e
        End Function
        Private Function PrefixUnary() As Expression
            Dim e As Expression = Me.ArrayAccess()
            While (Me.Current.Type = Tokens.T_Negate)
                Dim Op As Tokens = Me.Current.Type
                Me.NextToken()
                e = New Unary(Me.ParseExpression(False), Op, True)
            End While
            Return e
        End Function
        Private Function ArrayAccess() As Expression
            Dim e As Expression = Me.TMember()
            While (Me.Current.Type = Tokens.T_BracketOpen)
                Me.NextToken()
                Dim key As Expression = Me.ParseExpression(False)
                Me.Match(Tokens.T_BracketClose)
                e = New TArrayAccess(e, key)
            End While
            Return e
        End Function
        Private Function TMember() As Expression
            Dim e As Expression = Me.TCall()
            While (Me.Current.Type = Tokens.T_Dot)
                Me.NextToken()
                e = New TMember(e, Me.ParseIdentifier, Me.ParseTuples(Tokens.T_ParenthesisOpen, Tokens.T_ParenthesisClose))
            End While
            Return e
        End Function
        Private Function TCall() As Expression
            Dim e As Expression = Me.Factor
            While (Me.Current.Type = Tokens.T_ParenthesisOpen)
                e = New TCall(e, Me.ParseTuples(Tokens.T_ParenthesisOpen, Tokens.T_ParenthesisClose))
            End While
            Return e
        End Function
        Private Function Factor() As Expression
            Dim e As Expression = Nothing
            If (Me.Current.Type = Tokens.T_Return) Then
                e = Me.ParseReturn
            ElseIf (Me.Current.Type = Tokens.T_Function) Then
                e = Me.ParseFunction
            ElseIf (Me.Current.Type = Tokens.T_ILFunc) Then
                e = Me.ParseFunctionIL
            ElseIf (Me.Current.Type = Tokens.T_String) Then
                e = Me.ParseString
            ElseIf (Me.Current.Type = Tokens.T_Identifier) Then
                e = Me.ParseIdentifier
            ElseIf (Me.Current.Type = Tokens.T_Bool) Then
                e = Me.ParseBoolean
            ElseIf (Me.Current.Type = Tokens.T_Integer) Then
                e = Me.ParseInteger(False)
            ElseIf (Me.Current.Type = Tokens.T_Float) Then
                e = Me.ParseFloat(False)
            ElseIf (Me.Current.Type = Tokens.T_Hexadecimal) Then
                e = Me.ParseHexadecimal(False)
            ElseIf (Me.Current.Type = Tokens.T_Null) Then
                e = Me.ParseNull
            ElseIf (Me.Current.Type = Tokens.T_BracketOpen) Then
                e = Me.ParseArray
            ElseIf (Me.Current.Type = Tokens.T_ParenthesisOpen) Then
                e = Me.ParseParenthesis
            ElseIf (Me.Current.Type = Tokens.T_Minus) Then
                Me.NextToken()
                If (Me.Current.Type = Tokens.T_Integer) Then
                    e = Me.ParseInteger(True, Tokens.T_Minus)
                ElseIf (Me.Current.Type = Tokens.T_Hexadecimal) Then
                    e = Me.ParseHexadecimal(True, Tokens.T_Minus)
                ElseIf (Me.Current.Type = Tokens.T_Float) Then
                    e = Me.ParseFloat(True, Tokens.T_Minus)
                ElseIf (Me.Current.Type = Tokens.T_Identifier) Then
                    e = Me.ParseSignedIdentifier(Tokens.T_Minus)
                ElseIf (Me.Current.Type = Tokens.T_ParenthesisOpen) Then
                    e = Me.ParseSignedParentheses(Tokens.T_Minus)
                End If
            ElseIf (Me.Current.Type = Tokens.T_Plus) Then
                If (Me.Current.Type = Tokens.T_Integer) Then
                    e = Me.ParseInteger(False, Tokens.T_Plus)
                ElseIf (Me.Current.Type = Tokens.T_Float) Then
                    e = Me.ParseFloat(False, Tokens.T_Plus)
                ElseIf (Me.Current.Type = Tokens.T_Hexadecimal) Then
                    e = Me.ParseHexadecimal(False, Tokens.T_Plus)
                ElseIf (Me.Current.Type = Tokens.T_Identifier) Then
                    e = Me.ParseSignedIdentifier(Tokens.T_Plus)
                ElseIf (Me.Current.Type = Tokens.T_ParenthesisOpen) Then
                    e = Me.ParseSignedParentheses(Tokens.T_Plus)
                End If
            End If
            Return e
        End Function
#End Region
#Region "Token Stream Control"
        Private Function NextToken() As Token
            If (Me.Index >= Me.Stream.Count - 1) Then
                Me.Current = Token.Create(Tokens.T_EndOfFile)
            Else
                Me.Index += 1
                Me.Current = Me.Stream(Me.Index)
            End If
            Return Me.Current
        End Function
        Private Function Peek() As Token
            If (Me.Index + 1 >= Me.Stream.Count - 1) Then
                Return Token.Create(Tokens.T_EndOfFile)
            Else
                Return Me.Stream(Me.Index + 1)
            End If
        End Function
        Private Function Match(T As Tokens) As Token
            If (Not Me.Current.Type = T) Then
                Throw New Exception(String.Format("Unexpected '{0}' at index {1} line {2}; expecting '{3}'", Me.Current.Type.Name, Me.Current.Index, Me.Current.Line, T.Name))
            End If
            If (Me.Index >= Me.Stream.Count - 1) Then
                Me.Current = Token.Create(Tokens.T_EndOfFile)
            Else
                Me.Index += 1
                Me.Current = Me.Stream(Me.Index)
            End If
            Return Me.Current
        End Function
#End Region
    End Class
End Namespace