﻿Imports Xm.Utilities
Imports Xm.Core.Elements
Imports Xm.Core.Elements.Types
Imports System.IO
Imports System.Reflection

Namespace Core
    Public NotInheritable Class Scope
        Inherits Dictionary(Of String, TValue)
        Implements IDisposable
        Public Property Value As TValue
        Public Property Leave As Boolean
        Public Property Runtime As Runtime
        Sub New(Runtime As Runtime)
            Me.Leave = False
            Me.Runtime = Runtime
        End Sub
        ''' <summary>
        ''' Assigns new or existing variable
        ''' </summary>
        Public Sub SetVariable(Name As String, value As TValue)
            If (Me.ContainsKey(Name.ToLower)) Then
                Me(Name.ToLower) = value
            Else
                Me.Add(Name.ToLower, value)
            End If
        End Sub
        ''' <summary>
        ''' Retrieves variable located by reference
        ''' </summary>
        Public Function GetVariable(Name As String) As TValue
            For Each Scope In Me.Runtime
                For Each var In Scope
                    If (var.Key.Equals(Name.ToLower)) Then
                        Return var.Value
                    End If
                    If (Env.Wildcards AndAlso Name.ToLower Like var.Key) Then
                        Return var.Value
                    End If
                Next
            Next
            Return TValue.Null
        End Function
        ''' <summary>
        ''' Returns boolean if a given variable reference exists in this or parent scopes
        ''' </summary>
        Public Function Variable(Name As String) As Boolean
            For Each Scope In Me.Runtime
                For Each var In Scope
                    If (var.Key.Equals(Name.ToLower)) Then
                        Return True
                    End If
                    If (Env.Wildcards AndAlso Name.ToLower Like var.Key) Then
                        Return True
                    End If
                Next
            Next
            Return False
        End Function
        ''' <summary>
        ''' Imports static methods with specific XM attribute
        ''' </summary>
        Public Sub Import(type As Type)
            For Each m As MethodInfo In type.GetMethods(BindingFlags.Public Or BindingFlags.Static)
                For Each attr As Method In m.GetCustomAttributes.OfType(Of Method)()
                    If (attr.Type = Types.Null) Then
                        Me.SetVariable(attr.Reference, New TValue(type.CreateDelegate(m.Name)))
                    ElseIf (attr.Type = Types.Any) Then
                        Me.SetVariable(String.Format("*.{1}", attr.Type, attr.Reference), New TValue(type.CreateDelegate(m.Name)))
                    Else
                        Me.SetVariable(String.Format("{0}.{1}", attr.Type, attr.Reference), New TValue(type.CreateDelegate(m.Name)))
                    End If
                Next
            Next
        End Sub
        ''' <summary>
        ''' Store parameters
        ''' </summary>
        Public Sub StoreParameters(ParamArray Parameters() As Object)
            Me.SetVariable("args", TValue.Wrap(Parameters.ToList))
        End Sub
        ''' <summary>
        ''' Collect functions
        ''' </summary>
        Public Sub StoreFunctions()
            Dim changed As Boolean
            Do
                changed = False
                For Each e As Expression In Me.Runtime.Context
                    If (TypeOf e Is TFunction) Then
                        Me.SetVariable(CType(e, TFunction).GetStringValue, New TValue(e))
                        Me.Runtime.Context.Remove(e)
                        changed = True
                        Exit For
                    End If
                Next
            Loop While changed
            Me.Runtime.Functions.ForEach(Sub(d) Me.SetVariable(d.Method.Name, New TValue(d)))
            Me.Runtime.Functions.Clear()
        End Sub
        ''' <summary>
        ''' Store globals
        ''' </summary>
        Public Sub StoreGlobals()
            Me.Runtime.Globals.ForEach(Sub(x, y) Me.SetVariable(x, New TValue(y)))
            Me.Runtime.Globals.Clear()
        End Sub
        Private disposedValue As Boolean
        Protected Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    Me.Clear()
                End If
            End If
            Me.disposedValue = True
        End Sub
        Public Sub Dispose() Implements IDisposable.Dispose
            Me.Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace