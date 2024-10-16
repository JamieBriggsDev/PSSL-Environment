#region Header Licence
//  ---------------------------------------------------------------------
// 
//  Copyright (c) 2009 Alexandre Mutel and Microsoft Corporation.  
//  All rights reserved.
// 
//  This code module is part of NShader, a plugin for visual studio
//  to provide syntax highlighting for shader languages (hlsl, glsl, cg)
// 
//  ------------------------------------------------------------------
// 
//  This code is licensed under the Microsoft Public License. 
//  See the file License.txt for the license details.
//  More info on: http://nshader.codeplex.com
// 
//  ------------------------------------------------------------------
#endregion
namespace NShader.Lexer
{
    public enum ShaderToken
    {
        EOF,
        UNDEFINED,
        PREPROCESSOR,
        KEYWORD,
        KEYWORD_FX,
        KEYWORD_SPECIAL,
        TYPE,
        IDENTIFIER,
        INTRINSIC,
        COMMENT_LINE,
        COMMENT,
        NUMBER,
        FLOAT,
        STRING_LITERAL,
        OPERATOR,
        DELIMITER,
        LEFT_BRACKET, 
        RIGHT_BRACKET, 
        LEFT_PARENTHESIS, 
        RIGHT_PARENTHESIS, 
        LEFT_SQUARE_BRACKET, 
        RIGHT_SQUARE_BRACKET,
        UNITY_STRUCTURE,
        UNITY_TYPE,
        UNITY_VALUE,
        UNITY_FIXED
    }
}