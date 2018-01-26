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
            If (method IsNot Nothing) Then
                Return method.CreateDelegate(Exp.GetDelegateType((From p In method.GetParameters Select p.ParameterType).Concat(New Type() {method.ReturnType}).ToArray))
            End If
            Throw New Exception(String.Format("Method '{0}.{1}' not found or not qualified for delegate import", Type.Name, Name))
        End Function
        <Extension>
        Public Function IsObject(type As Type) As Boolean
            Return type Is GetType(Object) Or type Is GetType(List(Of Object))
        End Function
        <Extension>
        Public Sub ForEach(Of X, Y)(dict As Dictionary(Of X, Y), action As Action(Of X, Y))
            For Each pair As KeyValuePair(Of X, Y) In dict
                action.Invoke(pair.Key, pair.Value)
            Next
        End Sub
        <Extension>
        Public Function Push(Of T)(collection As List(Of T), values As List(Of T)) As List(Of T)
            collection.InsertRange(0, values)
            Return collection
        End Function
        <Extension>
        Public Function Push(Of T)(collection As List(Of T), value As T) As List(Of T)
            collection.Insert(0, value)
            Return collection
        End Function
        <Extension>
        Public Function Remove(str As String, ParamArray values() As String) As String
            For Each v As String In values
                If (str.Contains(v.ToLower)) Then
                    str = str.Replace(v.ToLower, String.Empty)
                End If
                If (str.Contains(v.ToUpper)) Then
                    str = str.Replace(v.ToUpper, String.Empty)
                End If
            Next
            Return str
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
                Case GetType(TArrayAccess)
                    Return Extensions.GetStringValue(CType(e, TArrayAccess).Name)
                Case Else
                    Return e.ToString
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
                Case Tokens.T_BracketOpen : Return "["
                Case Tokens.T_BracketClose : Return "]"
                Case Tokens.T_Null : Return "Null"
                Case Tokens.T_EndStatement : Return ";"
                Case Tokens.T_EndOfFile : Return "EOF"
                Case Else : Return type.ToString
            End Select
        End Function
    End Module
End Namespace