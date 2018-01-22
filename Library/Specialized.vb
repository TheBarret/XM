Imports Xm.Core
Imports System.Net
Imports System.Text
Imports System.Globalization
Imports System.Drawing

Namespace Library
    Public Class Specialized
        Public Shared UnitsTens As String() = {"zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety"}
        Public Shared Units As String() = {"zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten",
                                           "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen"}
        <Method(Types.Null, "urldecode")>
        Public Shared Function UrlDecode(str As String) As String
            Return WebUtility.UrlDecode(str)
        End Function
        <Method(Types.Null, "urlencode")>
        Public Shared Function UrlEncode(str As String) As String
            Return WebUtility.UrlEncode(str)
        End Function
        <Method(Types.Null, "size")>
        Public Shared Function HumanReadableSize(value As Integer) As String
            Static sizes() As String = {"B", "KB", "MB", "GB", "TB"}
            Dim len As Long = value, position As Integer = 0
            While len >= 1024 And position < sizes.Length - 1
                position += 1
                len = len \ 1024
            End While
            Return String.Format("{0:0.##} {1}", len, sizes(position))
        End Function
        <Method(Types.Null, "diaar")>
        Public Shared Function DiacriticAccentRemover(str As String) As String
            Dim normalized As String = str.Normalize(NormalizationForm.FormD)
            Dim sb As StringBuilder = New StringBuilder()
            For chr As Integer = 0 To normalized.Length - 1
                Dim uc As UnicodeCategory = CharUnicodeInfo.GetUnicodeCategory(normalized(chr))
                If uc <> UnicodeCategory.NonSpacingMark Then
                    sb.Append(normalized(chr))
                End If
            Next
            Return (sb.ToString().Normalize(NormalizationForm.FormC))
        End Function
        <Method(Types.Null, "color")>
        Public Shared Function ColorToInt(str As String) As Integer
            Return Color.FromName(str).ToArgb()
        End Function
        <Method(Types.Null, "rgb")>
        Public Shared Function IntToRgb(int As Integer) As String
            Dim c As Color = Color.FromArgb(int)
            Return String.Format("{0},{1},{2}", c.R, c.G, c.B)
        End Function
        <Method(Types.Null, "argb")>
        Public Shared Function IntToArgb(int As Integer) As String
            Dim c As Color = Color.FromArgb(int)
            Return String.Format("{0},{1},{2},{3}", c.A, c.R, c.G, c.B)
        End Function
        <Method(Types.Integer, "words")>
        Public Shared Function ToWords(value As Integer) As String
            If value = 0 Then
                Return "zero"
            ElseIf value < 0 Then
                Return String.Format("minus {0}", Specialized.ToWords(System.Math.Abs(value)))
            Else
                Dim result As String = String.Empty
                If (value \ 1000000) > 0 Then
                    result += Specialized.ToWords(value \ 1000000) & " million "
                    value = value Mod 1000000
                End If
                If (value \ 1000) > 0 Then
                    result += Specialized.ToWords(value \ 1000) & " thousand "
                    value = value Mod 1000
                End If
                If (value \ 100) > 0 Then
                    result += Specialized.ToWords(value \ 100) & " hundred "
                    value = value Mod 100
                End If
                If value > 0 Then
                    If (result.Length > 0) Then
                        result += "and "
                    End If
                    If (value < 20) Then
                        result += Specialized.Units(value)
                    Else
                        result += Specialized.UnitsTens(value \ 10)
                        If (value Mod 10) > 0 Then
                            result += String.Format(" {0}", Specialized.Units(value Mod 10))
                        End If
                    End If
                End If
                Return result
            End If
        End Function
        <Method(Types.Integer, "roman")>
        Public Shared Function ToRoman(value As Integer) As String
            If (value > 0 AndAlso value <= 4000) Then
                If value >= 1000 Then
                    Return "M" & Specialized.ToRoman(value - 1000)
                ElseIf value >= 900 Then
                    Return "CM" & Specialized.ToRoman(value - 900)
                ElseIf value >= 500 Then
                    Return "D" & Specialized.ToRoman(value - 500)
                ElseIf value >= 400 Then
                    Return "CD" & Specialized.ToRoman(value - 400)
                ElseIf value >= 100 Then
                    Return "C" & Specialized.ToRoman(value - 100)
                ElseIf value >= 90 Then
                    Return "XC" & Specialized.ToRoman(value - 90)
                ElseIf value >= 50 Then
                    Return "L" & Specialized.ToRoman(value - 50)
                ElseIf value >= 40 Then
                    Return "XL" & Specialized.ToRoman(value - 40)
                ElseIf value >= 10 Then
                    Return "X" & Specialized.ToRoman(value - 10)
                ElseIf value >= 9 Then
                    Return "IX" & Specialized.ToRoman(value - 9)
                ElseIf value >= 5 Then
                    Return "V" & Specialized.ToRoman(value - 5)
                ElseIf value >= 4 Then
                    Return "IV" & Specialized.ToRoman(value - 4)
                ElseIf value >= 1 Then
                    Return "I" & Specialized.ToRoman(value - 1)
                End If
            End If
            Return String.Empty
        End Function
    End Class
End Namespace