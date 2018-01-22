Imports Xm.Core

Namespace Library
    Public Class Math
        <Method(Types.Null, "rnd")>
        Public Shared Function GetRandomInt(min As Integer, max As Integer) As Integer
            Static rnd As New Random(Environment.TickCount)
            Return rnd.Next(min, max)
        End Function
        <Method(Types.Null, "atan")>
        Public Shared Function ATangent(value As Single) As Single
            Return CSng(System.Math.Atan(CDbl(value)))
        End Function
        <Method(Types.Null, "atan2")>
        Public Shared Function ATangent2(value As Single, b As Single) As Single
            Return CSng(System.Math.Atan2(CDbl(value), CDbl(b)))
        End Function
        <Method(Types.Null, "acos")>
        Public Shared Function ACosine(value As Single) As Single
            Return CSng(System.Math.Acos(CDbl(value)))
        End Function
        <Method(Types.Null, "asin")>
        Public Shared Function ASine(value As Single) As Single
            Return CSng(System.Math.Asin(CDbl(value)))
        End Function
        <Method(Types.Null, "tan")>
        Public Shared Function Tangent(value As Single) As Single
            Return CSng(System.Math.Tan(CDbl(value)))
        End Function
        <Method(Types.Null, "tanh")>
        Public Shared Function TangentH(value As Single) As Single
            Return CSng(System.Math.Tanh(CDbl(value)))
        End Function
        <Method(Types.Null, "cos")>
        Public Shared Function Cosine(value As Single) As Single
            Return CSng(System.Math.Cos(CDbl(value)))
        End Function
        <Method(Types.Null, "cosh")>
        Public Shared Function CosineH(value As Single) As Single
            Return CSng(System.Math.Cosh(CDbl(value)))
        End Function
        <Method(Types.Null, "sin")>
        Public Shared Function Sine(value As Single) As Single
            Return CSng(System.Math.Sin(CDbl(value)))
        End Function
        <Method(Types.Null, "sinh")>
        Public Shared Function SineH(value As Single) As Single
            Return CSng(System.Math.Sinh(CDbl(value)))
        End Function
        <Method(Types.Null, "log")>
        Public Shared Function Logarithm(value As Single) As Single
            Return CSng(System.Math.Log(CDbl(value)))
        End Function
        <Method(Types.Null, "logb")>
        Public Shared Function LogarithmBase(value As Single, b As Single) As Single
            Return CSng(System.Math.Log(CDbl(value), b))
        End Function
        <Method(Types.Null, "log10")>
        Public Shared Function LogarithmTen(value As Single) As Single
            Return CSng(System.Math.Log10(CDbl(value)))
        End Function
        <Method(Types.Null, "pi")>
        Public Shared Function Pi() As Single
            Return CSng(System.Math.PI)
        End Function
        <Method(Types.Null, "round")>
        Public Shared Function Round(value As Single, d As Integer) As Single
            Return CSng(System.Math.Round(CDbl(value), d))
        End Function
        <Method(Types.Null, "floor")>
        Public Shared Function Floor(value As Single) As Single
            Return CSng(System.Math.Floor(CDbl(value)))
        End Function
        <Method(Types.Null, "ceiling")>
        Public Shared Function Ceiling(value As Single) As Single
            Return CSng(System.Math.Ceiling(CDbl(value)))
        End Function
        <Method(Types.Null, "trunc")>
        Public Shared Function Truncate(value As Single) As Single
            Return CSng(System.Math.Truncate(CDbl(value)))
        End Function
        <Method(Types.Null, "power")>
        Public Shared Function Power(value As Single, p As Single) As Single
            Return CSng(System.Math.Pow(CDbl(value), CDbl(p)))
        End Function
        <Method(Types.Null, "sqrt")>
        Public Shared Function SquareRoot(value As Single) As Single
            Return CSng(System.Math.Sqrt(CDbl(value)))
        End Function
        <Method(Types.Null, "iseven")>
        Public Shared Function IsEven(value As Integer) As Boolean
            Return value Mod 2 = 0
        End Function
        <Method(Types.Null, "isodd")>
        Public Shared Function IsOdd(value As Integer) As Boolean
            Return value Mod 2 <> 0
        End Function
        <Method(Types.Null, "percentage")>
        Public Shared Function PercentageOf(number As Object, ByVal percent As Integer) As Single
            If (Single.TryParse(number.ToString, Nothing)) Then
                Return CSng((Single.Parse(number.ToString) * percent / 100))
            ElseIf (Integer.TryParse(number.ToString, Nothing)) Then
                Return CSng((Integer.Parse(number.ToString) * percent / 100))
            Else
                Return 0
            End If
        End Function
    End Class
End Namespace