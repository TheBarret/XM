Imports Xm.Core
Imports System.Text
Imports System.Security.Cryptography

Namespace Library
    Public Class Crypt
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
        <Method(Types.Null, "rsae")>
        Public Shared Function RSAEncrypt(str As String, key As String) As String
            If (Not String.IsNullOrEmpty(str) AndAlso Not String.IsNullOrEmpty(key)) Then
                Using rsa As RSACryptoServiceProvider = New RSACryptoServiceProvider(New CspParameters() With {.KeyContainerName = key}) With {.PersistKeyInCsp = True}
                    Return Convert.ToBase64String(rsa.Encrypt(Text.Encoding.UTF8.GetBytes(str), True))
                End Using
            End If
            Return "Invalid parameters"
        End Function
        <Method(Types.Null, "rsad")>
        Public Shared Function RSADecrypt(str As String, key As String) As String
            If (Not String.IsNullOrEmpty(str) AndAlso Not String.IsNullOrEmpty(key)) Then
                Using rsa As RSACryptoServiceProvider = New RSACryptoServiceProvider(New CspParameters() With {.KeyContainerName = key}) With {.PersistKeyInCsp = True}
                    Return Text.Encoding.UTF8.GetString(rsa.Decrypt(Convert.FromBase64String(str), True))
                End Using
            End If
            Return "Invalid parameters"
        End Function
    End Class
End Namespace