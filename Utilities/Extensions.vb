Imports Xm.Core
Imports Xm.Core.Elements
Imports Xm.Core.Elements.Types
Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Exp = System.Linq.Expressions.Expression

Namespace Utilities
    Public Module Extensions
        <Extension>
        Public Function Compress(Buffer() As Byte) As Byte()
            Using ms As New MemoryStream
                Using gzs As New GZipStream(ms, CompressionLevel.Optimal, True)
                    gzs.Write(Buffer, 0, Buffer.Length)
                End Using
                Return ms.ToArray
            End Using
        End Function
        <Extension>
        Public Function Decompress(Buffer() As Byte) As Byte()
            Using input As New MemoryStream(Buffer)
                Using gzs As New GZipStream(input, CompressionMode.Decompress)
                    Using ms As New MemoryStream
                        gzs.CopyTo(ms)
                        Return ms.ToArray
                    End Using
                End Using
            End Using
        End Function
        <Extension>
        Public Function CreateDelegate(Type As Type, Name As String) As [Delegate]
            Dim method As MethodInfo = Type.GetMethod(Name, BindingFlags.Public Or BindingFlags.Static Or BindingFlags.IgnoreCase)
            If (method IsNot Nothing AndAlso method.IsStatic And method.IsPublic) Then
                Return method.CreateDelegate(Exp.GetDelegateType((From p In method.GetParameters Select p.ParameterType).Concat(New Type() {method.ReturnType}).ToArray))
            End If
            Throw New Exception(String.Format("Method '{0}.{1}' not found or not qualified for delegate import", Type.Name, Name))
        End Function
        <Extension>
        Public Sub ForEach(dict As Dictionary(Of String, Object), action As Action(Of String, Object))
            For Each pair As KeyValuePair(Of String, Object) In dict
                action.Invoke(pair.Key, pair.Value)
            Next
        End Sub
        <Extension>
        Public Function ConvertToList(Of T)(value As T) As List(Of T)
            Return New List(Of T) From {value}
        End Function
        <Extension>
        Public Function ConvertToList(Of T)(value As T, collection As List(Of T)) As List(Of T)
            Dim result As New List(Of T) From {value}
            result.AddRange(collection)
            Return result
        End Function
        <Extension>
        Public Function StripHexPrefix(value As String) As String
            If (value.ToLower.StartsWith("0x") Or value.ToLower.StartsWith("&h")) Then
                Return value.Substring(2)
            End If
            Return value
        End Function
        <Extension>
        Public Function RequiresRuntime(d As [Delegate]) As Boolean
            Return d.Method.GetParameters.Any AndAlso d.Method.GetParameters.First.ParameterType Is GetType(Runtime)
        End Function
        <Extension>
        Public Function Repeat(v As String, c As Integer) As String
            Return New StringBuilder(v.Length * c).Insert(0, v, c).ToString
        End Function
        <Extension>
        Public Function GetStringValue(e As Expression) As String
            Select Case e.GetType
                Case GetType(TString)
                    Return CType(e, TString).Value
                Case GetType(TIdentifier)
                    Return CType(e, TIdentifier).Value
                Case GetType(TInteger)
                    Return CType(e, TInteger).Value.ToString
                Case GetType(TFloat)
                    Return CType(e, TFloat).Value.ToString
                Case GetType(TBoolean)
                    Return CType(e, TBoolean).Value.ToString
                Case GetType(TFunction)
                    Return Extensions.GetStringValue(CType(e, TFunction).Name)
                Case Else
                    Return e.ToString
            End Select
        End Function
        <Extension>
        Public Function IsPrimitive(value As Expression) As Boolean
            Select Case value.GetType
                Case GetType(TString)
                    Return True
                Case GetType(TBoolean)
                    Return True
                Case GetType(TInteger)
                    Return True
                Case GetType(TFloat)
                    Return True
                Case Else
                    Return False
            End Select
        End Function
        <Extension>
        Public Function GetExpressionType(value As Expression) As Types
            Select Case value.GetType
                Case GetType(TString)
                    Return Types.String
                Case GetType(TBoolean)
                    Return Types.Boolean
                Case GetType(TInteger)
                    Return Types.Integer
                Case GetType(TFloat)
                    Return Types.Float
                Case GetType(TFunction)
                    Return Types.Function
                Case GetType(TNull)
                    Return Types.Null
                Case Else
                    Return Types.Undefined
            End Select
        End Function
        <Extension>
        Public Function Name(type As Tokens) As String
            Select Case type
                Case Tokens.T_Plus : Return "+"
                Case Tokens.T_Minus : Return "-"
                Case Tokens.T_Mult : Return "*"
                Case Tokens.T_Div : Return "/"
                Case Tokens.T_Mod : Return "%"
                Case Tokens.T_Assign : Return "="
                Case Tokens.T_Or : Return "|"
                Case Tokens.T_And : Return "&"
                Case Tokens.T_Xor : Return "^"
                Case Tokens.T_Comma : Return ","
                Case Tokens.T_Dot : Return "."
                Case Tokens.T_Negate : Return "!"
                Case Tokens.T_Equal : Return "=="
                Case Tokens.T_NotEqual : Return "!="
                Case Tokens.T_Greater : Return ">"
                Case Tokens.T_Lesser : Return "<"
                Case Tokens.T_EqualOrGreater : Return ">="
                Case Tokens.T_EqualOrLesser : Return "<="
                Case Tokens.T_ParenthesisOpen : Return "("
                Case Tokens.T_ParenthesisClose : Return ")"
                Case Tokens.T_BraceOpen : Return "{"
                Case Tokens.T_BraceClose : Return "}"
                Case Tokens.T_Null : Return "Null"
                Case Tokens.T_EndStatement : Return ";"
                Case Tokens.T_EndOfFile : Return "EOF"
                Case Else : Return type.ToString
            End Select
        End Function
    End Module
End Namespace