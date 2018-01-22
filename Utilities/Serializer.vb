Imports Xm.Core

Namespace Utilities
    Public NotInheritable Class Serializer
        Public Const Shift As Byte = &H0
        Public Const Offset As Byte = &H5
        Public Shared Function Create(Context As String) As Byte()
            Return New Syntax.Parser().Tokenize(New Lexer.Parser().Analyze(Context), Serializer.Offset, Serializer.Shift)
        End Function
        Public Shared Function Create(Context As String, offset As Byte, shift As Byte) As Byte()
            Return New Syntax.Parser().Tokenize(New Lexer.Parser().Analyze(Context), offset, shift)
        End Function
        Public Shared Function Create(stream As List(Of Token), offset As Byte, shift As Byte) As Byte()
            Dim buffer As New List(Of Byte) From {&H58, &H4D, &H54, offset, shift}
            For Each current As Token In stream
                buffer.Add(current.Type)
                Select Case current.Type
                    Case Tokens.T_Identifier
                        Serializer.WriteString(buffer, current.Value, offset, shift)
                    Case Tokens.T_String
                        Serializer.WriteString(buffer, current.Value, offset, shift)
                    Case Tokens.T_Integer
                        buffer.AddRange(BitConverter.GetBytes(Integer.Parse(current.Value, Env.Integers)))
                    Case Tokens.T_Float
                        buffer.AddRange(BitConverter.GetBytes(Single.Parse(current.Value.Replace(".", ","), Env.Floats)))
                    Case Tokens.T_Bool
                        buffer.AddRange(BitConverter.GetBytes(Boolean.Parse(current.Value)))
                End Select
            Next
            Return buffer.ToArray
        End Function
        Public Shared Function Deserialize(buffer() As Byte) As List(Of Token)
            If (buffer IsNot Nothing AndAlso buffer.Length >= 5) Then
                If (Serializer.HasHeader(buffer)) Then
                    Dim offset As Byte = buffer(3), shift As Byte = buffer(4)
                    Dim stream As New List(Of Token), current As Token, i As Integer = 5
                    Do
                        current = New Token(String.Empty, CType(buffer(i), Tokens))
                        Select Case current.Type
                            Case Tokens.T_Identifier
                                For x As Integer = i + 1 To buffer.Length - 1 Step 2
                                    If (buffer(x) = 0) Then Exit For
                                    current.Value &= Strings.ChrW(BitConverter.ToUInt16({buffer(x), buffer(x + 1)}, 0) - offset)
                                    i += 2
                                Next
                                current.Value = Serializer.Unmask(current.Value, shift)
                                stream.Add(current)
                                i += 2
                                Continue Do
                            Case Tokens.T_String
                                For x As Integer = i + 1 To buffer.Length - 1 Step 2
                                    If (buffer(x) = 0) Then Exit For
                                    current.Value &= Strings.ChrW(BitConverter.ToUInt16({buffer(x), buffer(x + 1)}, 0) - offset)
                                    i += 2
                                Next
                                current.Value = Serializer.Unmask(current.Value, shift)
                                stream.Add(current)
                                i += 2
                                Continue Do
                            Case Tokens.T_Integer
                                current.Value = BitConverter.ToInt32({buffer(i + 1), buffer(i + 2), buffer(i + 3), buffer(i + 4)}, 0).ToString
                                stream.Add(current)
                                i += 5
                                Continue Do
                            Case Tokens.T_Float
                                current.Value = BitConverter.ToSingle({buffer(i + 1), buffer(i + 2), buffer(i + 3), buffer(i + 4)}, 0).ToString
                                current.Value.Replace(",", ".") '//Adjust decimal notation
                                stream.Add(current)
                                i += 5
                                Continue Do
                            Case Tokens.T_Bool
                                current.Value = BitConverter.ToBoolean(buffer, i + 1).ToString
                                stream.Add(current)
                                i += 2
                                Continue Do
                        End Select
                        i += 1
                        stream.Add(current)
                    Loop Until current.Type = Tokens.T_EndOfFile
                    Return stream
                End If
                Throw New Exception("header mismatch")
            End If
            Throw New Exception("invalid buffer")
        End Function
        Public Shared Function HasHeader(buffer() As Byte) As Boolean
            Return buffer IsNot Nothing AndAlso buffer.Length >= 3 AndAlso {buffer(0), buffer(1), buffer(2)}.SequenceEqual({&H58, &H4D, &H54})
        End Function
        Public Shared Function Mask(value As String, shift As Byte) As String
            If (shift > 8 Or shift < 0) Then shift = 5
            Return value.Select(Function(ch) Strings.AscW(ch) << shift).Aggregate(String.Empty, Function(x, y) String.Format("{0}{1}", x, Strings.ChrW(y * 2)))
        End Function
        Public Shared Function Unmask(value As String, shift As Byte) As String
            If (shift > 8 Or shift < 0) Then shift = 5
            Return value.Select(Function(ch) Strings.AscW(ch) >> shift).Aggregate(String.Empty, Function(x, y) String.Format("{0}{1}", x, Strings.ChrW(y \ 2)))
        End Function
        Public Shared Sub WriteString(buffer As List(Of Byte), value As String, offset As Byte, shift As Byte)
            For Each ch As Char In Serializer.Mask(value, shift)
                buffer.AddRange(BitConverter.GetBytes(CUShort(Strings.AscW(ch) + offset)))
            Next
            buffer.Add(0)
        End Sub
    End Class
End Namespace