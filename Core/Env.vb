﻿Imports System.Globalization
Namespace Core
    Public Class Env
        Public Const MaxRepeats As Integer = 255
        Public Const Wildcards As Boolean = True
        Public Const Floats As NumberStyles = NumberStyles.Float
        Public Const Integers As NumberStyles = NumberStyles.Integer
        Public Const Hexadecimal As NumberStyles = NumberStyles.HexNumber
        Public Shared Function Culture() As CultureInfo
            Return New CultureInfo("en-US")
        End Function
        Public Shared Function RandomName(len As Integer, Optional prefix As String = "") As String
            Static rnd As New Random(Environment.TickCount)
            Static table As String = "ABCDEF1234567890"
            Dim result As String = String.Empty
            For i As Integer = 1 To len
                result &= table(rnd.Next(0, table.Length - 1))
            Next
            Return String.Format("{0}{1}", prefix, result)
        End Function
    End Class
End Namespace