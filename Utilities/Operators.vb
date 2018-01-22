Imports Xm.Core
Imports System.Reflection
Imports System.Text

Namespace Utilities
    Public NotInheritable Class Operators
        Public Shared Function Addition(a As TValue, b As TValue) As TValue
            If (a.ScriptType = Types.Integer And b.ScriptType = Types.Integer) Then
                Return New TValue(a.Cast(Of Integer)() + b.Cast(Of Integer)())
            ElseIf (a.ScriptType = Types.Float And b.ScriptType = Types.Integer) Then
                Return New TValue(a.Cast(Of Single)() + b.Cast(Of Integer)())
            ElseIf (a.ScriptType = Types.Integer And b.ScriptType = Types.Float) Then
                Return New TValue(a.Cast(Of Integer)() + b.Cast(Of Single)())
            ElseIf (a.ScriptType = Types.Float And b.ScriptType = Types.Float) Then
                Return New TValue(a.Cast(Of Single)() + b.Cast(Of Single)())
            Else
                Return New TValue(a.Value.ToString & b.Value.ToString)
            End If
        End Function
        Public Shared Function Subtraction(a As TValue, b As TValue) As TValue
            If (a.ScriptType = Types.Integer And b.ScriptType = Types.Integer) Then
                Return New TValue(a.Cast(Of Integer)() - b.Cast(Of Integer)())
            ElseIf (a.ScriptType = Types.Float And b.ScriptType = Types.Integer) Then
                Return New TValue(a.Cast(Of Single)() - b.Cast(Of Integer)())
            ElseIf (a.ScriptType = Types.Integer And b.ScriptType = Types.Float) Then
                Return New TValue(a.Cast(Of Integer)() - b.Cast(Of Single)())
            ElseIf (a.ScriptType = Types.Float And b.ScriptType = Types.Float) Then
                Return New TValue(a.Cast(Of Single)() - b.Cast(Of Single)())
            Else
                Return TValue.Null
            End If
        End Function
        Public Shared Function Multiplication(a As TValue, b As TValue) As TValue
            If (a.ScriptType = Types.Integer And b.ScriptType = Types.Integer) Then
                Return New TValue(a.Cast(Of Integer)() * b.Cast(Of Integer)())
            ElseIf (a.ScriptType = Types.Float And b.ScriptType = Types.Integer) Then
                Return New TValue(a.Cast(Of Single)() * b.Cast(Of Integer)())
            ElseIf (a.ScriptType = Types.Integer And b.ScriptType = Types.Float) Then
                Return New TValue(a.Cast(Of Integer)() * b.Cast(Of Single)())
            ElseIf (a.ScriptType = Types.Float And b.ScriptType = Types.Float) Then
                Return New TValue(a.Cast(Of Single)() * b.Cast(Of Single)())
            ElseIf (a.ScriptType = Types.String And b.ScriptType = Types.Integer) Then
                Return Operators.Repeat(a, b)
            ElseIf (a.ScriptType = Types.Integer And b.ScriptType = Types.String) Then
                Return Operators.Repeat(b, a)
            Else
                Return TValue.Null
            End If
        End Function
        Public Shared Function Repeat(v As TValue, c As TValue) As TValue
            Dim count As Integer = c.Cast(Of Integer)()
            If (count >= 0 AndAlso count <= Env.MaxRepeats) Then
                Return New TValue(v.Cast(Of String)().Repeat(count))
            End If
            Throw New Exception(String.Format("Count value out of range, max = {0}", Env.MaxRepeats))
        End Function
        Public Shared Function Division(a As TValue, b As TValue) As TValue
            If (a.ScriptType = Types.Integer And b.ScriptType = Types.Integer) Then
                Return New TValue(a.Cast(Of Integer)() \ b.Cast(Of Integer)())
            ElseIf (a.ScriptType = Types.Float And b.ScriptType = Types.Integer) Then
                Return New TValue(a.Cast(Of Single)() / b.Cast(Of Integer)())
            ElseIf (a.ScriptType = Types.Integer And b.ScriptType = Types.Float) Then
                Return New TValue(a.Cast(Of Integer)() / b.Cast(Of Single)())
            ElseIf (a.ScriptType = Types.Float And b.ScriptType = Types.Float) Then
                Return New TValue(a.Cast(Of Single)() / b.Cast(Of Single)())
            Else
                Return TValue.Null
            End If
        End Function
        Public Shared Function Modulo(a As TValue, b As TValue) As TValue
            If (a.ScriptType = Types.Integer And b.ScriptType = Types.Integer) Then
                Return New TValue(a.Cast(Of Integer)() Mod b.Cast(Of Integer)())
            ElseIf (a.ScriptType = Types.Float And b.ScriptType = Types.Integer) Then
                Return New TValue(a.Cast(Of Single)() Mod b.Cast(Of Integer)())
            ElseIf (a.ScriptType = Types.Integer And b.ScriptType = Types.Float) Then
                Return New TValue(a.Cast(Of Integer)() Mod b.Cast(Of Single)())
            ElseIf (a.ScriptType = Types.Float And b.ScriptType = Types.Float) Then
                Return New TValue(a.Cast(Of Single)() Mod b.Cast(Of Single)())
            Else
                Return TValue.Null
            End If
        End Function
        Public Shared Function Sign(a As TValue, op As Tokens) As TValue
            If (op = Tokens.T_Plus) Then
                Return Operators.SignPositive(a)
            ElseIf (op = Tokens.T_Minus) Then
                Return Operators.SignNegative(a)
            Else
                Return TValue.Null
            End If
        End Function
        Public Shared Function SignPositive(a As TValue) As TValue
            If (a.ScriptType = Types.Integer) Then
                Return New TValue(a.Cast(Of Integer)() * 1)
            ElseIf (a.ScriptType = Types.Float) Then
                Return New TValue(a.Cast(Of Single)() * 1.0R)
            Else
                Return TValue.Null
            End If
        End Function
        Public Shared Function SignNegative(a As TValue) As TValue
            If (a.ScriptType = Types.Integer) Then
                Return New TValue(a.Cast(Of Integer)() * -1)
            ElseIf (a.ScriptType = Types.Float) Then
                Return New TValue(a.Cast(Of Single)() * -1.0R)
            Else
                Return TValue.Null
            End If
        End Function
        Public Shared Function [And](a As TValue, b As TValue) As TValue
            If (a.ScriptType = Types.Boolean And b.ScriptType = Types.Boolean) Then
                Return New TValue(a.Cast(Of Boolean)() And b.Cast(Of Boolean)())
            ElseIf (a.ScriptType = Types.Integer And b.ScriptType = Types.Integer) Then
                Return New TValue(a.Cast(Of Integer)() And b.Cast(Of Integer)())
            Else
                Return TValue.Null
            End If
        End Function
        Public Shared Function [Or](a As TValue, b As TValue) As TValue
            If (a.ScriptType = Types.Boolean And b.ScriptType = Types.Boolean) Then
                Return New TValue(a.Cast(Of Boolean)() Or b.Cast(Of Boolean)())
            ElseIf (a.ScriptType = Types.Integer And b.ScriptType = Types.Integer) Then
                Return New TValue(a.Cast(Of Integer)() Or b.Cast(Of Integer)())
            Else
                Return TValue.Null
            End If
        End Function
        Public Shared Function [Xor](a As TValue, b As TValue) As TValue
            If (a.ScriptType = Types.Boolean And b.ScriptType = Types.Boolean) Then
                Return New TValue(a.Cast(Of Boolean)() And b.Cast(Of Boolean)())
            ElseIf (a.ScriptType = Types.Integer Xor b.ScriptType = Types.Integer) Then
                Return New TValue(a.Cast(Of Integer)() Xor b.Cast(Of Integer)())
            Else
                Return TValue.Null
            End If
        End Function
        Public Shared Function [Not](a As TValue) As TValue
            If (a.ScriptType = Types.Integer) Then
                Return New TValue(Not a.Cast(Of Integer)())
            ElseIf (a.ScriptType = Types.Boolean) Then
                Return New TValue(Not a.Cast(Of Boolean)())
            Else
                Return TValue.Null
            End If
        End Function
        Public Shared Function ShiftRight(a As TValue, b As TValue) As TValue
            If (a.ScriptType = Types.Integer And b.ScriptType = Types.Integer) Then
                Return New TValue(a.Cast(Of Integer)() >> b.Cast(Of Integer)())
            Else
                Return TValue.Null
            End If
        End Function
        Public Shared Function ShiftLeft(a As TValue, b As TValue) As TValue
            If (a.ScriptType = Types.Integer And b.ScriptType = Types.Integer) Then
                Return New TValue(a.Cast(Of Integer)() << b.Cast(Of Integer)())
            Else
                Return TValue.Null
            End If
        End Function
        Public Shared Function IsEqual(a As TValue, b As TValue) As TValue
            Return New TValue(a.Value.Equals(b.Value))
        End Function
        Public Shared Function IsNotEqual(a As TValue, b As TValue) As TValue
            Return New TValue(Not a.Value.Equals(b.Value))
        End Function
        Public Shared Function IsGreater(a As TValue, b As TValue) As TValue
            If (a.ScriptType = Types.Integer And b.ScriptType = Types.Integer) Then
                Return New TValue(a.Cast(Of Integer)() > b.Cast(Of Integer)())
            ElseIf (a.ScriptType = Types.Float And b.ScriptType = Types.Integer) Then
                Return New TValue(a.Cast(Of Single)() > b.Cast(Of Integer)())
            ElseIf (a.ScriptType = Types.Integer And b.ScriptType = Types.Float) Then
                Return New TValue(a.Cast(Of Integer)() > b.Cast(Of Single)())
            ElseIf (a.ScriptType = Types.Float And b.ScriptType = Types.Float) Then
                Return New TValue(a.Cast(Of Single)() > b.Cast(Of Single)())
            Else
                Return TValue.Null
            End If
        End Function
        Public Shared Function IsLesser(a As TValue, b As TValue) As TValue
            If (a.ScriptType = Types.Integer And b.ScriptType = Types.Integer) Then
                Return New TValue(a.Cast(Of Integer)() < b.Cast(Of Integer)())
            ElseIf (a.ScriptType = Types.Float And b.ScriptType = Types.Integer) Then
                Return New TValue(a.Cast(Of Single)() < b.Cast(Of Integer)())
            ElseIf (a.ScriptType = Types.Integer And b.ScriptType = Types.Float) Then
                Return New TValue(a.Cast(Of Integer)() < b.Cast(Of Single)())
            ElseIf (a.ScriptType = Types.Float And b.ScriptType = Types.Float) Then
                Return New TValue(a.Cast(Of Single)() < b.Cast(Of Single)())
            Else
                Return TValue.Null
            End If
        End Function
        Public Shared Function IsEqualOrGreater(a As TValue, b As TValue) As TValue
            If (a.ScriptType = Types.Integer And b.ScriptType = Types.Integer) Then
                Return New TValue(a.Cast(Of Integer)() >= b.Cast(Of Integer)())
            ElseIf (a.ScriptType = Types.Float And b.ScriptType = Types.Integer) Then
                Return New TValue(a.Cast(Of Single)() >= b.Cast(Of Integer)())
            ElseIf (a.ScriptType = Types.Integer And b.ScriptType = Types.Float) Then
                Return New TValue(a.Cast(Of Integer)() >= b.Cast(Of Single)())
            ElseIf (a.ScriptType = Types.Float And b.ScriptType = Types.Float) Then
                Return New TValue(a.Cast(Of Single)() >= b.Cast(Of Single)())
            Else
                Return TValue.Null
            End If
        End Function
        Public Shared Function IsEqualOrLesser(a As TValue, b As TValue) As TValue
            If (a.ScriptType = Types.Integer And b.ScriptType = Types.Integer) Then
                Return New TValue(a.Cast(Of Integer)() <= b.Cast(Of Integer)())
            ElseIf (a.ScriptType = Types.Float And b.ScriptType = Types.Integer) Then
                Return New TValue(a.Cast(Of Single)() <= b.Cast(Of Integer)())
            ElseIf (a.ScriptType = Types.Integer And b.ScriptType = Types.Float) Then
                Return New TValue(a.Cast(Of Integer)() <= b.Cast(Of Single)())
            ElseIf (a.ScriptType = Types.Float And b.ScriptType = Types.Float) Then
                Return New TValue(a.Cast(Of Single)() <= b.Cast(Of Single)())
            Else
                Return TValue.Null
            End If
        End Function
    End Class
End Namespace