Imports Xm.Core
Imports System.IO
Imports System.Text
Imports System.Runtime.InteropServices

Namespace Library
    Public Class Float
        <Method(Types.Float, "abs")>
        Public Shared Function Abs(value As Single) As Single
            Return System.Math.Abs(value)
        End Function
        <Method(Types.Float, "hex")>
        Public Shared Function Hexadecimal(value As Single) As String
            Dim sb As New StringBuilder
            Using ms As New MemoryStream(Marshal.SizeOf(value))
                Using sw As New StreamWriter(ms)
                    sw.Write(value)
                    sw.Flush()
                    ms.Seek(0, SeekOrigin.Begin)
                    Dim buffer As Byte() = New Byte(3) {}
                    ms.Read(buffer, 0, 4)
                    For Each b As Byte In buffer
                        sb.AppendFormat("{0:X2}", b)
                    Next
                    Return String.Format("0x{0}", sb.ToString)
                End Using
            End Using
        End Function
    End Class
End Namespace