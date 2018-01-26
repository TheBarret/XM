Imports Xm.Core
Imports System.Text
Imports System.Security.Cryptography

Namespace Library
    Public Class Cryptography
        <Method(Types.Null, "md5")>
        Public Shared Function MD5Hash(str As Object) As String
            Return String.Concat(MD5.Create.ComputeHash(Text.Encoding.UTF8.GetBytes(str.ToString)).Select(Function(v) v.ToString("X")))
        End Function
        <Method(Types.Null, "sha1")>
        Public Shared Function SHA1Hash(str As Object) As String
            Return String.Concat(SHA1.Create.ComputeHash(Text.Encoding.UTF8.GetBytes(str.ToString)).Select(Function(v) v.ToString("X")))
        End Function
        <Method(Types.Null, "sha256")>
        Public Shared Function SHA256Hash(str As Object) As String
            Return String.Concat(SHA256.Create.ComputeHash(Text.Encoding.UTF8.GetBytes(str.ToString)).Select(Function(v) v.ToString("X")))
        End Function
        <Method(Types.Null, "sha384")>
        Public Shared Function SHA384Hash(str As Object) As String
            Return String.Concat(SHA384.Create.ComputeHash(Text.Encoding.UTF8.GetBytes(str.ToString)).Select(Function(v) v.ToString("X")))
        End Function
        <Method(Types.Null, "sha512")>
        Public Shared Function SHA512Hash(str As Object) As String
            Return String.Concat(SHA512.Create.ComputeHash(Text.Encoding.UTF8.GetBytes(str.ToString)).Select(Function(v) v.ToString("X")))
        End Function
    End Class
End Namespace