﻿Namespace Core
    Public Enum Tokens As Byte
        T_Null = &H0
        T_String
        T_Integer
        T_Hexadecimal
        T_Float
        T_Bool
        T_Identifier
        T_Plus
        T_Minus
        T_Mult
        T_Div
        T_Mod
        T_Assign
        T_Like
        T_Equal
        T_NotEqual
        T_Greater
        T_Lesser
        T_EqualOrGreater
        T_EqualOrLesser
        T_RShift
        T_LShift
        T_If
        T_Else
        T_Or
        T_And
        T_Xor
        T_QM
        T_Colon
        T_Comma
        T_Dot
        T_Negate
        T_Function
        T_ILFunc
        T_Return
        T_Use
        T_ParenthesisOpen
        T_ParenthesisClose
        T_BraceOpen
        T_BraceClose
        T_BracketOpen
        T_BracketClose
        T_Space
        T_Newline
        T_BlockComment
        T_LineComment
        T_EndStatement
        T_EndOfFile = &HFF
    End Enum
End Namespace