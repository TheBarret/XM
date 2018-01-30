Imports Xm.Utilities
Imports Xm.Core
Imports Xm.Core.Elements
Imports Xm.Core.Elements.Types
Imports System.IO
Imports System.Reflection

Public NotInheritable Class Runtime
    Inherits Stack(Of Scope)
    Implements IDisposable
    Public Property Input As TextReader
    Public Property Output As TextWriter
    Public Property Context As List(Of Expression)
    Public Property Functions As List(Of [Delegate])
    Public Property Globals As Dictionary(Of String, Object)
    Private Property Loaded As Boolean
    ''' <summary>
    ''' Constructors
    ''' </summary>
    Sub New(Context As String)
        Me.Loaded = True
        Me.Functions = New List(Of [Delegate])
        Me.Globals = New Dictionary(Of String, Object)
        Me.Context = New Syntax.Parser().Analyze(New Lexer.Parser().Analyze(Context))
    End Sub
    Sub New(Tokens As List(Of Token))
        Me.Loaded = True
        Me.Functions = New List(Of [Delegate])
        Me.Globals = New Dictionary(Of String, Object)
        Me.Context = New Syntax.Parser().Analyze(Tokens)
    End Sub
    ''' <summary>
    ''' Registering a custom delegate
    ''' </summary>
    Public Sub AddFunction(d As [Delegate])
        Me.Functions.Add(d)
    End Sub
    ''' <summary>
    ''' Registering read only global variables
    ''' </summary>
    Public Sub AddGlobals(name As String, value As Object)
        Me.Globals.Add(String.Format("${0}", name.ToLower), value)
    End Sub
    ''' <summary>
    ''' Register read only global variables
    ''' </summary>
    Public Sub AddGlobals(Of X, Y)(collection As Dictionary(Of X, Y))
        For Each pair As KeyValuePair(Of X, Y) In collection
            Me.Globals.Add(String.Format("${0}", pair.Key.ToString.ToLower), pair.Value)
        Next
    End Sub
    ''' <summary>
    ''' Change Output stream
    ''' </summary>
    Public Sub SetOutput(output As TextWriter)
        Me.Output = output
    End Sub
    ''' <summary>
    ''' Change Input stream
    ''' </summary>
    Public Sub SetInput(input As TextReader)
        Me.Input = input
    End Sub
    ''' <summary>
    ''' Direct function call
    ''' </summary>
    Public Function CallFunction(Of T)(Name As String, ParamArray Parameters() As Object) As T
        Try
            Me.Enter()
            Me.Scope.StoreGlobals()
            Me.Scope.StoreFunctions()
            If (Me.Scope.Variable(Name)) Then
                Dim func As TValue = Me.Scope.GetVariable(Name)
                If (func.IsFunction) Then
                    Return CType(TValue.Unwrap(Me.Resolve(func.Cast(Of TFunction), Parameters.ToList)), T)
                End If
            End If
            Throw New Exception(String.Format("Undefined function '{0}'", Name))
        Finally
            Me.Leave()
        End Try
    End Function
    Public Function CallFunction(Name As String, ParamArray Parameters() As Object) As Object
        Try
            Me.Enter()
            Me.Scope.StoreGlobals()
            Me.Scope.StoreFunctions()
            If (Me.Scope.Variable(Name)) Then
                Dim func As TValue = Me.Scope.GetVariable(Name)
                If (func.IsFunction) Then
                    Return TValue.Unwrap(Me.Resolve(func.Cast(Of TFunction), Parameters.ToList))
                End If
            End If
            Throw New Exception(String.Format("Undefined function '{0}'", Name))
        Finally
            Me.Leave()
        End Try
    End Function
    ''' <summary>
    ''' Evaluate with new scope and return value as T
    ''' </summary>
    Public Function Evaluate(Of T)(ParamArray Parameters() As Object) As T
        If (Me.Loaded AndAlso Me.Context IsNot Nothing AndAlso Me.Context.Any) Then
            Return Me.EvaluateContext(Of T)(Me.Context, Parameters)
        End If
        Return Nothing
    End Function
    ''' <summary>
    ''' Evaluate with new scope and return value as object
    ''' </summary>
    Public Function Evaluate(ParamArray Parameters() As Object) As Object
        If (Me.Loaded AndAlso Me.Context IsNot Nothing AndAlso Me.Context.Any) Then
            Return Me.EvaluateContext(Me.Context, Parameters)
        End If
        Return Nothing
    End Function
    ''' <summary>
    ''' Evaluate context with new scope and return value as T
    ''' </summary>
    Public Function EvaluateContext(Of T)(Context As List(Of Expression), ParamArray Parameters() As Object) As T
        Return CType(Me.EvaluateContext(Context, Parameters), T)
    End Function
    ''' <summary>
    ''' Main evaluation routine
    ''' </summary>
    Public Function EvaluateContext(Context As List(Of Expression), ParamArray Parameters() As Object) As Object
        Try
            Me.Enter()
            Me.Scope.StoreFunctions()
            Me.Scope.StoreGlobals()
            Me.Scope.StoreParameters(Parameters)
            Dim lastValue As TValue = Nothing
            For Each e As Expression In Context
                lastValue = Me.Resolve(e)
                If (Me.Scope.Leave) Then Exit For
            Next
            ''' Single line or script defined return
            If (Context.Count = 1 AndAlso Not Me.Scope.Leave) Then
                Return TValue.Unwrap(lastValue)
            Else
                Return TValue.Unwrap(Me.Scope.Value)
            End If
        Finally
            Me.Leave()
        End Try
    End Function
    ''' <summary>
    ''' Evaluate context without new scope (no unwrapping!)
    ''' </summary>
    Public Function EvaluateContextNoScope(Context As List(Of Expression)) As TValue
        If (Me.HasScope) Then
            Dim lastValue As TValue = Nothing
            For Each e As Expression In Context
                lastValue = Me.Resolve(e)
                If (Me.Scope.Leave) Then Exit For
            Next
            ''' Single line or script defined return
            If (Context.Count = 1 AndAlso Not Me.Scope.Leave) Then
                Return lastValue
            Else
                Return Me.Scope.Value
            End If
        End If
        Throw New Exception("Unable to evaluate, context has no scope available")
    End Function
    ''' <summary>
    ''' Determine expression type and resolve
    ''' </summary>
    Public Function Resolve(e As Expression) As TValue
        If (TypeOf e Is TUse) Then
            Return Me.Resolve(CType(e, TUse))
        ElseIf (TypeOf e Is TNull) Then
            Return New TValue
        ElseIf (TypeOf e Is TString) Then
            Return New TValue(CType(e, TString).Escaped)
        ElseIf (TypeOf e Is TBoolean) Then
            Return New TValue(CType(e, TBoolean).Value)
        ElseIf (TypeOf e Is TInteger) Then
            Return New TValue(CType(e, TInteger).Value)
        ElseIf (TypeOf e Is TFloat) Then
            Return New TValue(CType(e, TFloat).Value)
        ElseIf (TypeOf e Is TIdentifier) Then
            Return Me.Resolve(CType(e, TIdentifier))
        ElseIf (TypeOf e Is TArrayAccess) Then
            Return Me.Resolve(CType(e, TArrayAccess))
        ElseIf (TypeOf e Is TArray) Then
            Return Me.Resolve(CType(e, TArray))
        ElseIf (TypeOf e Is TCall) Then
            Return Me.Resolve(CType(e, TCall))
        ElseIf (TypeOf e Is TMember) Then
            Return Me.Resolve(CType(e, TMember))
        ElseIf (TypeOf e Is TReturn) Then
            Return Me.Resolve(CType(e, TReturn))
        ElseIf (TypeOf e Is Ternary) Then
            Return Me.Resolve(CType(e, Ternary))
        ElseIf (TypeOf e Is Binary) Then
            Return Me.Resolve(CType(e, Binary))
        ElseIf (TypeOf e Is Unary) Then
            Return Me.Resolve(CType(e, Unary))
        ElseIf (TypeOf e Is TConditional) Then
            Return Me.Resolve(CType(e, TConditional))
        ElseIf (TypeOf e Is TFunction) Then
            Return New TValue(CType(e, TFunction))
        End If
        Throw New Exception(String.Format("Undefined expression type '{0}'", e.GetType.Name))
    End Function
    ''' <summary>
    ''' Import internal or external libraries
    ''' </summary>
    Public Function Resolve(e As TUse) As TValue
        Dim reference As String = e.Library.GetStringValue
        If (reference.Contains(".dll")) Then
            If (File.Exists(String.Format(".\{0}", reference))) Then
                For Each t As Type In Assembly.LoadFile(Path.GetFullPath(String.Format(".\{0}", reference))).GetTypes
                    Me.Scope.Import(t)
                Next
            End If
        Else
            Dim typeRef As Type = Type.GetType(reference, False, True)
            If (typeRef IsNot Nothing) Then Me.Scope.Import(typeRef)
        End If
        Return TValue.Null
    End Function
    ''' <summary>
    ''' Resolve ternary expression
    ''' </summary>
    Public Function Resolve(e As Ternary) As TValue
        If (e.Condition IsNot Nothing AndAlso e.True IsNot Nothing AndAlso e.False IsNot Nothing) Then
            Dim condition As TValue = Me.Resolve(e.Condition)
            If (condition.IsBoolean) Then
                If (condition.Cast(Of Boolean)()) Then
                    Return Me.Resolve(e.True)
                Else
                    Return Me.Resolve(e.False)
                End If
            End If
            Throw New Exception(String.Format("Ternary expression '{0}' expecting boolean as condition", e.ToString))
        End If
        Throw New Exception("Invalid expression")
    End Function
    ''' <summary>
    ''' Resolve binary expression
    ''' </summary>
    Public Function Resolve(e As Binary) As TValue
        If (e.Left IsNot Nothing AndAlso e.Right IsNot Nothing) Then
            If (e.Op = Tokens.T_Assign) Then
                Dim name As String = e.Left.GetStringValue
                If (Not name.StartsWith("$")) Then
                    Dim operand As TValue = Me.Resolve(e.Right)
                    Me.Scope.SetVariable(CType(e.Left, TIdentifier).Value, operand)
                    Return operand
                Else
                    Throw New Exception(String.Format("Attempt to write readonly variable '{0}'", name))
                End If
            Else
                Dim left As TValue = Me.Resolve(e.Left)
                Dim right As TValue = Me.Resolve(e.Right)
                If (e.Op = Tokens.T_Plus) Then
                    Return Operators.Addition(left, right)
                ElseIf (e.Op = Tokens.T_Minus) Then
                    Return Operators.Subtraction(left, right)
                ElseIf (e.Op = Tokens.T_Mult) Then
                    Return Operators.Multiplication(left, right)
                ElseIf (e.Op = Tokens.T_Div) Then
                    Return Operators.Division(left, right)
                ElseIf (e.Op = Tokens.T_Mod) Then
                    Return Operators.Modulo(left, right)
                ElseIf (e.Op = Tokens.T_And) Then
                    Return Operators.And(left, right)
                ElseIf (e.Op = Tokens.T_Or) Then
                    Return Operators.Or(left, right)
                ElseIf (e.Op = Tokens.T_Xor) Then
                    Return Operators.Xor(left, right)
                ElseIf (e.Op = Tokens.T_Equal) Then
                    Return Operators.IsEqual(left, right)
                ElseIf (e.Op = Tokens.T_NotEqual) Then
                    Return Operators.IsNotEqual(left, right)
                ElseIf (e.Op = Tokens.T_Greater) Then
                    Return Operators.IsGreater(left, right)
                ElseIf (e.Op = Tokens.T_Lesser) Then
                    Return Operators.IsLesser(left, right)
                ElseIf (e.Op = Tokens.T_EqualOrGreater) Then
                    Return Operators.IsEqualOrGreater(left, right)
                ElseIf (e.Op = Tokens.T_EqualOrLesser) Then
                    Return Operators.IsEqualOrLesser(left, right)
                ElseIf (e.Op = Tokens.T_RShift) Then
                    Return Operators.ShiftRight(left, right)
                ElseIf (e.Op = Tokens.T_LShift) Then
                    Return Operators.ShiftLeft(left, right)
                ElseIf (e.Op = Tokens.T_Like) Then
                    Return Operators.IsLike(left, right)
                End If
            End If
            Throw New Exception(String.Format("Undefined expression type '{0}'", e.GetType.Name))
        End If
        Throw New Exception("Invalid expression")
    End Function
    ''' <summary>
    ''' Resolve unary expression
    ''' </summary>
    Public Function Resolve(e As Unary) As TValue
        If (e.Operand IsNot Nothing) Then
            If (e.Op = Tokens.T_Negate) Then
                Return Operators.Not(Me.Resolve(e.Operand))
            ElseIf (e.Op = Tokens.T_Plus) Then
                Return Operators.Sign(Me.Resolve(e.Operand), e.Op)
            ElseIf (e.Op = Tokens.T_Minus) Then
                Return Operators.Sign(Me.Resolve(e.Operand), e.Op)
            End If
            Throw New Exception(String.Format("Undefined expression type '{0}'", e.GetType.Name))
        End If
        Throw New Exception("Invalid expression")
    End Function
    ''' <summary>
    ''' Resolve condition and evaluate boolean body
    ''' </summary>
    Public Function Resolve(e As TConditional) As TValue
        Dim condition As TValue = Me.Resolve(e.Condition)
        If (condition.IsBoolean) Then
            If (condition.Cast(Of Boolean)()) Then
                Me.EvaluateContextNoScope(e.True)
            Else
                Me.EvaluateContextNoScope(e.False)
            End If
            Return condition
        Else
            Throw New Exception(String.Format("Expecting boolean from '{0}'", e.ToString))
        End If
    End Function
    ''' <summary>
    ''' Resolve identifier
    ''' </summary>
    Public Function Resolve(e As TIdentifier) As TValue
        Dim name As String = e.Value
        If (Me.Scope.Variable(name)) Then Return Me.Scope.GetVariable(name)
        Throw New Exception(String.Format("Undefined identifier '{0}'", name))
    End Function
    ''' <summary>
    ''' Resolve array object
    ''' </summary>
    Public Function Resolve(e As TArray) As TValue
        Return New TValue((From v As Expression In e.Values Select Me.Resolve(v)).ToList)
    End Function
    ''' <summary>
    ''' Resolve array value
    ''' </summary>
    Public Function Resolve(e As TArrayAccess) As TValue
        Dim key As TValue = Me.Resolve(e.Index), var As TValue = Me.Resolve(e.Name)
        If (key.IsInteger) Then
            If (var.IsArray) Then
                Dim index As Integer = key.Cast(Of Integer)()
                Dim array As List(Of TValue) = var.Cast(Of List(Of TValue))()
                If (index >= 0 AndAlso index <= array.Count - 1) Then
                    Return array.ElementAt(index)
                End If
                Throw New Exception(String.Format("Index is out of range '{0}[{1}]'", e.GetStringValue, key.ToString))
            End If
            Throw New Exception(String.Format("'{0}' is not an array", e.GetStringValue))
        End If
        Throw New Exception(String.Format("Index must be nummeric '{0}[{1}]'", e.GetStringValue, key.ToString))
    End Function
    ''' <summary>
    ''' Resolve member
    ''' </summary>
    Public Function Resolve(e As TMember) As TValue
        Dim value As TValue = Me.Resolve(e.Target)
        Dim reference As String = String.Format("{0}.{1}", value.ScriptType, e.Member.GetStringValue)
        If (Me.Scope.Variable(reference)) Then
            Dim func As TValue = Me.Scope.GetVariable(reference)
            If (func.IsDelegate) Then
                Return Me.Resolve(CType(func.Value, [Delegate]), Me.Resolve(e.Parameters).Push(value))
            End If
        End If
        Throw New Exception(String.Format("Undefined function '{0}'", e.ToString))
    End Function
    ''' <summary>
    ''' Resolve call
    ''' </summary>
    Public Function Resolve(e As TCall) As TValue
        If (Me.Scope.Variable(e.Name.GetStringValue)) Then
            Dim func As TValue = Me.Resolve(e.Name)
            If (func.IsFunction) Then
                Return Me.Resolve(func.Cast(Of TFunction), e.Parameters)
            ElseIf (func.IsDelegate) Then
                Return Me.Resolve(CType(func.Value, [Delegate]), Me.Resolve(e.Parameters))
            End If
            Throw New Exception(String.Format("Unexpected '{0}', expecting function or delegate", func.ScriptType))
        ElseIf (TypeOf e.Name Is TFunction) Then
            Return Me.Resolve(CType(e.Name, TFunction), e.Parameters)
        End If
        Throw New Exception(String.Format("Undefined function '{0}'", e.ToString))
    End Function
    ''' <summary>
    ''' Resolve TFunction with object type parameters
    ''' </summary>
    Public Function Resolve(e As TFunction, params As List(Of Object)) As TValue
        Try
            Me.Enter()
            If (e.Parameters.Count = params.Count) Then
                For i As Integer = 0 To e.Parameters.Count - 1
                    Me.Scope.SetVariable(e.Parameters(i).GetStringValue, New TValue(params(i)))
                Next
                Return Me.EvaluateContextNoScope(e.Body)
            End If
        Finally
            Me.Leave()
        End Try
        Throw New Exception(String.Format("Parameter count mismatch for '{0}'", e.ToString))
    End Function
    ''' <summary>
    ''' Resolve TFunction with expression type parameters
    Public Function Resolve(e As TFunction, params As List(Of Expression)) As TValue
        Try
            Me.Enter()
            If (e.Parameters.Count = params.Count) Then
                For i As Integer = 0 To e.Parameters.Count - 1
                    Me.Scope.SetVariable(e.Parameters(i).GetStringValue, Me.Resolve(params(i)))
                Next
                Return Me.EvaluateContextNoScope(e.Body)
            End If
        Finally
            Me.Leave()
        End Try
        Throw New Exception(String.Format("Parameter count mismatch for '{0}'", e.ToString))
    End Function
    ''' <summary>
    ''' Resolve library function
    ''' </summary>
    Public Function Resolve(e As [Delegate], params As List(Of TValue)) As TValue
        Try
            Me.Enter()
            Dim parameters As List(Of Object) = TValue.Unwrap(params)
            If (e.RequiresRuntime) Then parameters.Insert(0, Me)
            If (Me.Validate(e, parameters)) Then
                Return TValue.Wrap(e.Method.Invoke(Nothing, parameters.ToArray))
            End If
            Return TValue.Null
        Finally
            Me.Leave()
        End Try
    End Function
    ''' <summary>
    ''' Validates parameters with target function
    ''' </summary>
    Public Function Validate(e As [Delegate], params As List(Of Object)) As Boolean
        If (e.Method.GetParameters.Count <> params.Count) Then
            Throw New Exception(String.Format("Parameter count mismatch for '{0}()'", e.Method.Name))
        End If
        For i As Integer = 0 To e.Method.GetParameters.Count - 1
            If (e.Method.GetParameters(i).ParameterType.IsObject) Then Continue For
            If (e.Method.GetParameters(i).ParameterType IsNot params(i).GetType) Then
                Throw New Exception(String.Format("Parameter type mismatch for '{0}()'", e.Method.Name))
            End If
        Next
        Return True
    End Function
    ''' <summary>
    ''' Resolve list of expressions
    ''' </summary>
    Public Function Resolve(e As List(Of Expression)) As List(Of TValue)
        Return e.Select(Function(exp) Me.Resolve(exp)).ToList
    End Function
    ''' <summary>
    ''' Resolve and return with operand
    ''' </summary>
    Public Function Resolve(e As TReturn) As TValue
        Me.Scope.Leave = True
        Me.Scope.Value = Me.Resolve(e.Operand)
        Return TValue.Null
    End Function
    ''' <summary>
    ''' Scope control: current
    ''' </summary>
    Public Function Scope() As Scope
        Return Me.Peek
    End Function
    ''' <summary>
    ''' Scope control: enter
    ''' </summary>
    Public Sub Enter()
        Me.Push(New Scope(Me))
        If (Me.Count = 1) Then
            Me.Scope.Import(GetType(Library.Core))
            Me.Scope.Import(GetType(Library.String))
            Me.Scope.Import(GetType(Library.Integer))
            Me.Scope.Import(GetType(Library.Float))
            Me.Scope.Import(GetType(Library.Arrays))
            Me.Scope.Import(GetType(Library.Functions))
        End If
    End Sub
    ''' <summary>
    ''' Scope control: dispose and leave
    ''' </summary>
    Public Sub Leave()
        If (Me.Count > 0) Then
            Me.Scope.Dispose()
            Me.Pop()
        End If
    End Sub
    ''' <summary>
    ''' Returns true if current context as a scope
    ''' </summary>
    Public ReadOnly Property HasScope As Boolean
        Get
            Return Me.Count > 0
        End Get
    End Property
    ''' <summary>
    ''' Returns true if current context as an input stream
    ''' </summary>
    Public ReadOnly Property HasInput As Boolean
        Get
            Return Me.Input IsNot Nothing
        End Get
    End Property
    ''' <summary>
    ''' Returns true if current context as an output stream
    ''' </summary>
    Public ReadOnly Property HasOutput As Boolean
        Get
            Return Me.Output IsNot Nothing
        End Get
    End Property
    ''' <summary>
    ''' IDisposable Support
    ''' </summary>
    Private disposedValue As Boolean
    Protected Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                For Each sc In Me
                    sc.Dispose()
                Next
                Me.Clear()
                Me.Functions.Clear()
                Me.Globals.Clear()
                Me.Context.Clear()
            End If
        End If
        Me.disposedValue = True
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        Me.Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
