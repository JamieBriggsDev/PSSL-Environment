//
//  This CSharp output file generated by Managed Package LEX
//  Version:  0.6.0 (1-August-2007)
//  Machine:  HOME-PC
//  DateTime: 26.09.2016 20:40:29
//  UserName: Home
//  MPLEX input file <ShaderMPLexer.lex>
//  MPLEX frame file <mplex.frame>
//
//  Option settings: unicode, verbose, noparser, minimize, classes, compressmap, compressnext
//

#define BACKUP
#define LEFTANCHORS
#define STANDALONE
//
// mplex.frame
// Version 0.6.1 of 1 August 2007
// Left and Right Anchored state support.
// Start condition stack. Two generic params.
// Using fixed length context handling for right anchors
//
using System;
using System.IO;
using System.Collections.Generic;
#if !STANDALONE
using Babel.ParserGenerator;
#endif // STANDALONE


namespace NShader.Lexer
{   
    /// <summary>
    /// Summary Canonical example of MPLEX automaton
    /// </summary>
    
#if STANDALONE
    //
    // These are the dummy declarations for stand-alone MPLEX applications
    // normally these declarations would come from the parser.
    // If you declare /noparser, or %option noparser then you get this.
    //

    public enum Tokens
    { 
      EOF = 0, maxParseToken = int.MaxValue 
      // must have at least these two, values are almost arbitrary
    }

    public abstract class ScanBase
    {
        public abstract int yylex();
        protected abstract int CurrentSc { get; set; }
        //
        // Override this virtual EolState property if the scanner state is more
        // complicated then a simple copy of the current start state ordinal
        //
        public virtual int EolState { get { return CurrentSc; } set { CurrentSc = value; } }
    }
    
    public interface IColorScan
    {
        void SetSource(string source, int offset);
        int GetNext(ref int state, out int start, out int end);
    }
    
    

#endif // STANDALONE

    public abstract class ScanBuff
    {
        public const int EOF = -1;
        public abstract int Pos { get; set; }
        public abstract int Read();
        public abstract int Peek();
        public abstract int ReadPos { get; }
        public abstract string GetString(int b, int e);
    }
    
    // If the compiler can't find ScanBase maybe you need
    // to run mppg with /mplex, or run mplex with /noparser
    public sealed class Scanner : ScanBase, IColorScan
    {
   
        public ScanBuff buffer;
        private IErrorHandler handler;
        int scState;
        
        private static int GetMaxParseToken() {
            System.Reflection.FieldInfo f = typeof(Tokens).GetField("maxParseToken");
            return (f == null ? int.MaxValue : (int)f.GetValue(null));
        }
        
        static int parserMax = GetMaxParseToken();        
        
        protected override int CurrentSc 
        {
             // The current start state is a property
             // to try to avoid the user error of setting
             // scState but forgetting to update the FSA
             // start state "currentStart"
             //
             get { return scState; }
             set { scState = value; currentStart = startState[value]; }
        }
        
        enum Result {accept, noMatch, contextFound};

        const int maxAccept = 60;
        const int initial = 61;
        const int eofNum = 0;
        const int goStart = -1;
        const int INITIAL = 0;
        const int COMMENT = 1;

/*
//  ShaderMPLexer.lex.
//  Lexical description for MPLex. This file is inspired from http://svn.assembla.com/svn/ppjlab/trunk/scanner.lex
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
*/
/**********************************************************************************/
/********************************User Defined Code*********************************/
/**********************************************************************************/
public IShaderTokenProvider ShaderTokenProvider = null; // Token provider
/**********************************************************************************/
/**********Start Condition Declarations and Lexical Category Definitions***********/
/**********************************************************************************/
/**********************************************************************************/
/**********************************************************************************/
/********************************The Rules Section*********************************/
/**********************************************************************************/
/**********************************************************************************/
        int state;
        int currentStart = initial;
        int chr;           // last character read
        int cNum;          // ordinal number of chr
        int lNum = 0;      // current line number
        int lineStartNum;  // cNum at start of line

        //
        // The following instance variables are useful, among other
        // things, for constructing the yylloc location objects.
        //
        int tokPos;        // buffer position at start of token
        int tokNum;        // ordinal number of first character
        int tokLen;        // number of character in token
        int tokCol;        // zero-based column number at start of token
        int tokLin;        // line number at start of token
        int tokEPos;       // buffer position at end of token
        int tokECol;       // column number at end of token
        int tokELin;       // line number at end of token
        string tokTxt;     // lazily constructed text of token
#if STACK          
        private Stack<int> scStack = new Stack<int>();
#endif // STACK

#region ScannerTables
    struct Table {
        public int min; public int rng; public int dflt;
        public sbyte[] nxt;
        public Table(int m, int x, int d, sbyte[] n) {
            min = m; rng = x; dflt = d; nxt = n;
        }
    };

    static int[] startState = {61, 56, 0};

   static int[] anchorState = {62, 56, 0};

#region CharacterMap
    //
    // There are 35 equivalence classes
    // There are 2 character sequence regions
    // There are 1 tables, 126 entries
    // There are 1 runs, 0 singletons
    //
    static sbyte[] map0 = new sbyte[126] {
/* \0     */ 2, 2, 2, 2, 2, 2, 2, 2, 2, 5, 0, 5, 5, 5, 2, 2, 
/* \020   */ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 
/* \040   */ 5, 26, 17, 19, 2, 21, 22, 2, 31, 32, 3, 12, 30, 13, 14, 1, 
/* 0      */ 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 4, 27, 24, 20, 25, 2, 
/* @      */ 2, 10, 10, 10, 10, 11, 16, 6, 15, 6, 6, 6, 6, 6, 6, 6, 
/* P      */ 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 33, 18, 34, 2, 6, 
/* `      */ 2, 10, 10, 10, 10, 11, 16, 6, 15, 6, 6, 6, 6, 6, 6, 6, 
/* p      */ 6, 6, 6, 6, 6, 6, 6, 6, 9, 6, 6, 28, 23, 29 };

    sbyte Map(int chr)
    { // '\0' <= chr <= '\uFFFF'
      if (chr < 126) return map0[chr - 0];
      else return (sbyte)2;
    }
#endregion

    static Table[] NxS = new Table[75];

    static Scanner() {
    NxS[0] = // Shortest string ""
        new Table(0, 0, 0, null);
    NxS[1] = // Shortest string "^\t"
        new Table(0, 20, -1, new sbyte[] {73, -1, -1, -1, -1, 73, 
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 63});
    NxS[2] = // Shortest string "/"
        new Table(20, 19, -1, new sbyte[] {52, -1, -1, -1, -1, -1, 
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 50, -1, 51});
    NxS[3] = // Shortest string "\0"
        new Table(0, 0, -1, null);
    NxS[4] = // Shortest string "*"
        new Table(20, 1, -1, new sbyte[] {49});
    NxS[5] = // Shortest string ":"
        new Table(0, 17, -1, new sbyte[] {74, -1, -1, -1, -1, 74, 
        48, -1, -1, 48, 48, 48, -1, -1, -1, 48, 48});
    NxS[6] = // Shortest string "G"
        new Table(6, 11, -1, new sbyte[] {6, 6, 6, 6, 6, 6, 
        -1, -1, -1, 6, 6});
    NxS[7] = // Shortest string "1"
        new Table(7, 8, -1, new sbyte[] {7, 7, -1, -1, 69, -1, 
        -1, 43});
    NxS[8] = // Shortest string "0"
        new Table(7, 8, -1, new sbyte[] {7, 7, 68, -1, 69, -1, 
        -1, 43});
    NxS[9] = // Shortest string "+"
        new Table(20, 1, -1, new sbyte[] {42});
    NxS[10] = // Shortest string "-"
        new Table(20, 1, -1, new sbyte[] {41});
    NxS[11] = // Shortest string "."
        new Table(7, 2, -1, new sbyte[] {38, 38});
    NxS[12] = // Shortest string """
        new Table(17, 2, 64, new sbyte[] {37, 65});
    NxS[13] = // Shortest string "^#"
        new Table(0, 17, -1, new sbyte[] {63, -1, -1, -1, -1, 63, 
        36, -1, -1, 36, 36, 36, -1, -1, -1, 36, 36});
    NxS[14] = // Shortest string "="
        new Table(20, 1, -1, new sbyte[] {35});
    NxS[15] = // Shortest string "%"
        new Table(20, 1, -1, new sbyte[] {34});
    NxS[16] = // Shortest string "&"
        new Table(22, 1, -1, new sbyte[] {33});
    NxS[17] = // Shortest string "|"
        new Table(23, 1, -1, new sbyte[] {32});
    NxS[18] = // Shortest string "<"
        new Table(20, 1, -1, new sbyte[] {31});
    NxS[19] = // Shortest string ">"
        new Table(20, 1, -1, new sbyte[] {30});
    NxS[20] = // Shortest string "!"
        new Table(20, 1, -1, new sbyte[] {29});
    NxS[21] = // Shortest string ";"
        new Table(0, 0, -1, null);
    NxS[22] = // Shortest string "{"
        new Table(0, 0, -1, null);
    NxS[23] = // Shortest string "}"
        new Table(0, 0, -1, null);
    NxS[24] = // Shortest string ","
        new Table(0, 0, -1, null);
    NxS[25] = // Shortest string "("
        new Table(0, 0, -1, null);
    NxS[26] = // Shortest string ")"
        new Table(0, 0, -1, null);
    NxS[27] = // Shortest string "["
        new Table(0, 0, -1, null);
    NxS[28] = // Shortest string "]"
        new Table(0, 0, -1, null);
    NxS[29] = // Shortest string "!="
        new Table(0, 0, -1, null);
    NxS[30] = // Shortest string ">="
        new Table(0, 0, -1, null);
    NxS[31] = // Shortest string "<="
        new Table(0, 0, -1, null);
    NxS[32] = // Shortest string "||"
        new Table(0, 0, -1, null);
    NxS[33] = // Shortest string "&&"
        new Table(0, 0, -1, null);
    NxS[34] = // Shortest string "%="
        new Table(0, 0, -1, null);
    NxS[35] = // Shortest string "=="
        new Table(0, 0, -1, null);
    NxS[36] = // Shortest string "^#G"
        new Table(6, 11, -1, new sbyte[] {36, -1, -1, 36, 36, 36, 
        -1, -1, -1, 36, 36});
    NxS[37] = // Shortest string """"
        new Table(0, 0, -1, null);
    NxS[38] = // Shortest string ".1"
        new Table(7, 10, -1, new sbyte[] {38, 38, -1, -1, 66, -1, 
        -1, -1, 39, 39});
    NxS[39] = // Shortest string ".1H"
        new Table(0, 0, -1, null);
    NxS[40] = // Shortest string ".1E1"
        new Table(7, 10, -1, new sbyte[] {40, 40, -1, -1, -1, -1, 
        -1, -1, 39, 39});
    NxS[41] = // Shortest string "-="
        new Table(0, 0, -1, null);
    NxS[42] = // Shortest string "+="
        new Table(0, 0, -1, null);
    NxS[43] = // Shortest string "0."
        new Table(7, 10, -1, new sbyte[] {38, 38, -1, -1, 70, -1, 
        -1, -1, 44, 44});
    NxS[44] = // Shortest string "0.H"
        new Table(0, 0, -1, null);
    NxS[45] = // Shortest string "0.E1"
        new Table(7, 10, -1, new sbyte[] {45, 45, -1, -1, -1, -1, 
        -1, -1, 44, 44});
    NxS[46] = // Shortest string "0E1"
        new Table(7, 2, -1, new sbyte[] {46, 46});
    NxS[47] = // Shortest string "0x1"
        new Table(7, 10, -1, new sbyte[] {47, 47, -1, 47, 47, -1, 
        -1, -1, -1, 47});
    NxS[48] = // Shortest string ":G"
        new Table(6, 11, -1, new sbyte[] {48, 48, 48, 48, 48, 48, 
        -1, -1, -1, 48, 48});
    NxS[49] = // Shortest string "*="
        new Table(0, 0, -1, null);
    NxS[50] = // Shortest string "//"
        new Table(0, 1, 50, new sbyte[] {-1});
    NxS[51] = // Shortest string "/*"
        new Table(0, 4, 51, new sbyte[] {-1, 51, 51, 53});
    NxS[52] = // Shortest string "/="
        new Table(0, 0, -1, null);
    NxS[53] = // Shortest string "/**"
        new Table(1, 3, -1, new sbyte[] {54, -1, 53});
    NxS[54] = // Shortest string "/**/"
        new Table(0, 0, -1, null);
    NxS[55] = // Shortest string "\t"
        new Table(0, 0, -1, null);
    NxS[56] = // Shortest string ""
        new Table(0, 4, 58, new sbyte[] {57, 58, 58, 59});
    NxS[57] = // Shortest string ""
        new Table(0, 0, -1, null);
    NxS[58] = // Shortest string "/"
        new Table(0, 4, 58, new sbyte[] {-1, 58, 58, 59});
    NxS[59] = // Shortest string "*"
        new Table(1, 3, -1, new sbyte[] {60, -1, 59});
    NxS[60] = // Shortest string "*/"
        new Table(0, 0, -1, null);
    NxS[61] = // Shortest string ""
        new Table(12, 32, 6, new sbyte[] {9, 10, 11, 6, 6, 12, 
        3, 3, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 
        28, 55, 2, 3, 4, 5, 55, 6, 7, 8});
    NxS[62] = // Shortest string "^"
        new Table(12, 32, 6, new sbyte[] {9, 10, 11, 6, 6, 12, 
        3, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 
        28, 1, 2, 3, 4, 5, 1, 6, 7, 8});
    NxS[63] = // Shortest string "^#\t"
        new Table(0, 17, -1, new sbyte[] {63, -1, -1, -1, -1, 63, 
        36, -1, -1, 36, 36, 36, -1, -1, -1, 36, 36});
    NxS[64] = // Shortest string ""/"
        new Table(17, 2, 64, new sbyte[] {37, 65});
    NxS[65] = // Shortest string ""\"
        new Table(0, 1, 64, new sbyte[] {-1});
    NxS[66] = // Shortest string ".1E"
        new Table(7, 7, -1, new sbyte[] {40, 40, -1, -1, -1, 67, 
        67});
    NxS[67] = // Shortest string ".1E+"
        new Table(7, 2, -1, new sbyte[] {40, 40});
    NxS[68] = // Shortest string "0x"
        new Table(7, 10, -1, new sbyte[] {47, 47, -1, 47, 47, -1, 
        -1, -1, -1, 47});
    NxS[69] = // Shortest string "0E"
        new Table(7, 7, -1, new sbyte[] {46, 46, -1, -1, -1, 72, 
        72});
    NxS[70] = // Shortest string "0.E"
        new Table(7, 7, -1, new sbyte[] {45, 45, -1, -1, -1, 71, 
        71});
    NxS[71] = // Shortest string "0.E+"
        new Table(7, 2, -1, new sbyte[] {45, 45});
    NxS[72] = // Shortest string "0E+"
        new Table(7, 2, -1, new sbyte[] {46, 46});
    NxS[73] = // Shortest string "^\t\t"
        new Table(0, 20, -1, new sbyte[] {73, -1, -1, -1, -1, 73, 
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 63});
    NxS[74] = // Shortest string ":\t"
        new Table(0, 17, -1, new sbyte[] {74, -1, -1, -1, -1, 74, 
        48, -1, -1, 48, 48, 48, -1, -1, -1, 48, 48});
    }

int NextState(int qStat) {
    if (chr == ScanBuff.EOF)
        return (qStat <= maxAccept && qStat != currentStart ? currentStart : eofNum);
    else {
        int rslt;
        int idx = Map(chr) - NxS[qStat].min;
        if (idx < 0) idx += 35;
        if ((uint)idx >= (uint)NxS[qStat].rng) rslt = NxS[qStat].dflt;
        else rslt = NxS[qStat].nxt[idx];
        return (rslt == goStart ? currentStart : rslt);
    }
}

int NextState() {
    if (chr == ScanBuff.EOF)
        return (state <= maxAccept && state != currentStart ? currentStart : eofNum);
    else {
        int rslt;
        int idx = Map(chr) - NxS[state].min;
        if (idx < 0) idx += 35;
        if ((uint)idx >= (uint)NxS[state].rng) rslt = NxS[state].dflt;
        else rslt = NxS[state].nxt[idx];
        return (rslt == goStart ? currentStart : rslt);
    }
}
#endregion


#if BACKUP
        // ====================== Nested class ==========================

        internal class Context // class used for automaton backup.
        {
            public int bPos;
            public int cNum;
            public int state;
            public int cChr;
        }
#endif // BACKUP


        // ====================== Nested class ==========================

        public sealed class StringBuff : ScanBuff
        {
            string str;        // input buffer
            int bPos;          // current position in buffer
            int sLen;

            public StringBuff(string str)
            {
                this.str = str;
                this.sLen = str.Length;
            }

            public override int Read()
            {
                if (bPos < sLen) return str[bPos++];
                else if (bPos == sLen) { bPos++; return '\n'; }   // one strike, see newline
                else return EOF;                                  // two strikes and you're out!
            }
            
            public override int ReadPos { get { return bPos - 1; } }

            public override int Peek()
            {
                if (bPos < sLen) return str[bPos];
                else return '\n';
            }

            public override string GetString(int beg, int end)
            {
                //  "end" can be greater than sLen with the BABEL
                //  option set.  Read returns a "virtual" EOL if
                //  an attempt is made to read past the end of the
                //  string buffer.  Without the guard any attempt 
                //  to fetch yytext for a token that includes the 
                //  EOL will throw an index exception.
                if (end > sLen) end = sLen;
                if (end <= beg) return ""; 
                else return str.Substring(beg, end - beg);
            }

            public override int Pos
            {
                get { return bPos; }
                set { bPos = value; }
            }
        }

        // ====================== Nested class ==========================

        public sealed class StreamBuff : ScanBuff
        {
            BufferedStream bStrm;   // input buffer
            int delta = 1;

            public StreamBuff(Stream str) { this.bStrm = new BufferedStream(str); }

            public override int Read() {
                return bStrm.ReadByte(); 
            }
            
            public override int ReadPos {
                get { return (int)bStrm.Position - delta; }
            }

            public override int Peek()
            {
                int rslt = bStrm.ReadByte();
                bStrm.Seek(-delta, SeekOrigin.Current);
                return rslt;
            }

            public override string GetString(int beg, int end)
            {
                if (end - beg <= 0) return "";
                long savePos = bStrm.Position;
                char[] arr = new char[end - beg];
                bStrm.Position = (long)beg;
                for (int i = 0; i < (end - beg); i++)
                    arr[i] = (char)bStrm.ReadByte();
                bStrm.Position = savePos;
                return new String(arr);
            }

            // Pos is the position *after* reading chr!
            public override int Pos
            {
                get { return (int)bStrm.Position; }
                set { bStrm.Position = value; }
            }
        }

        // ====================== Nested class ==========================

        /// <summary>
        /// This is the Buffer for UTF8 files.
        /// It attempts to read the encoding preamble, which for 
        /// this encoding should be unicode point \uFEFF which is 
        /// encoded as EF BB BF
        /// </summary>
        public class TextBuff : ScanBuff
        {
            protected BufferedStream bStrm;   // input buffer
            protected int delta = 1;
            
            private Exception BadUTF8()
            { return new Exception(String.Format("BadUTF8 Character")); }

            /// <summary>
            /// TextBuff factory.  Reads the file preamble
            /// and returns a TextBuff, LittleEndTextBuff or
            /// BigEndTextBuff according to the result.
            /// </summary>
            /// <param name="strm">The underlying stream</param>
            /// <returns></returns>
            public static TextBuff NewTextBuff(Stream strm)
            {
                // First check if this is a UTF16 file
                //
                int b0 = strm.ReadByte();
                int b1 = strm.ReadByte();

                if (b0 == 0xfe && b1 == 0xff)
                    return new BigEndTextBuff(strm);
                if (b0 == 0xff && b1 == 0xfe)
                    return new LittleEndTextBuff(strm);
                
                int b2 = strm.ReadByte();
                if (b0 == 0xef && b1 == 0xbb && b2 == 0xbf)
                    return new TextBuff(strm);
                //
                // There is no unicode preamble, so we
                // must go back to the UTF8 default.
                //
                strm.Seek(0, SeekOrigin.Begin);
                return new TextBuff(strm);
            }

            protected TextBuff(Stream str) { 
                this.bStrm = new BufferedStream(str);
            }

            public override int Read()
            {
                int ch0 = bStrm.ReadByte();
                int ch1;
                int ch2;
                if (ch0 < 0x7f)
                {
                    delta = (ch0 == EOF ? 0 : 1);
                    return ch0;
                }
                else if ((ch0 & 0xe0) == 0xc0)
                {
                    delta = 2;
                    ch1 = bStrm.ReadByte();
                    if ((ch1 & 0xc0) == 0x80)
                        return ((ch0 & 0x1f) << 6) + (ch1 & 0x3f);
                    else
                        throw BadUTF8();
                }
                else if ((ch0 & 0xf0) == 0xe0)
                {
                    delta = 3;
                    ch1 = bStrm.ReadByte();
                    ch2 = bStrm.ReadByte();
                    if ((ch1 & ch2 & 0xc0) == 0x80)
                        return ((ch0 & 0xf) << 12) + ((ch1 & 0x3f) << 6) + (ch2 & 0x3f);
                    else
                        throw BadUTF8();
                }
                else
                    throw BadUTF8();
            }

            public sealed override int ReadPos
            {
                get { return (int)bStrm.Position - delta; }
            }

            public sealed override int Peek()
            {
                int rslt = Read();
                bStrm.Seek(-delta, SeekOrigin.Current);
                return rslt;
            }

            /// <summary>
            /// Returns the string from the buffer between
            /// the given file positions.  This needs to be
            /// done carefully, as the number of characters
            /// is, in general, not equal to (end - beg).
            /// </summary>
            /// <param name="beg">Begin filepos</param>
            /// <param name="end">End filepos</param>
            /// <returns></returns>
            public sealed override string GetString(int beg, int end)
            {
                int i;
                if (end - beg <= 0) return "";
                long savePos = bStrm.Position;
                char[] arr = new char[end - beg];
                bStrm.Position = (long)beg;
                for (i = 0; bStrm.Position < end; i++)
                    arr[i] = (char)Read();
                bStrm.Position = savePos;
                return new String(arr, 0, i);
            }

            // Pos is the position *after* reading chr!
            public sealed override int Pos
            {
                get { return (int)bStrm.Position; }
                set { bStrm.Position = value; }
            }
        }

        // ====================== Nested class ==========================
        /// <summary>
        /// This is the Buffer for Big-endian UTF16 files.
        /// </summary>
        public sealed class BigEndTextBuff : TextBuff
        {
            internal BigEndTextBuff(Stream str) : base(str) { } // 

            public override int Read()
            {
                int ch0 = bStrm.ReadByte();
                int ch1 = bStrm.ReadByte();
                return (ch0 << 8) + ch1;
            }
        }
        
        // ====================== Nested class ==========================
        /// <summary>
        /// This is the Buffer for Little-endian UTF16 files.
        /// </summary>
        public sealed class LittleEndTextBuff : TextBuff
        {
            internal LittleEndTextBuff(Stream str) : base(str) { } // { this.bStrm = new BufferedStream(str); }

            public override int Read()
            {
                int ch0 = bStrm.ReadByte();
                int ch1 = bStrm.ReadByte();
                return (ch1 << 8) + ch0;
            }
        }
        
        // =================== End Nested classes =======================

        public Scanner(Stream file) {
            buffer = TextBuff.NewTextBuff(file); // selected by /unicode option
            this.cNum = -1;
            this.chr = '\n'; // to initialize yyline, yycol and lineStart
            GetChr();
        }

        public Scanner() { }

        void GetChr()
        {
            if (chr == '\n') 
            { 
                lineStartNum = cNum + 1; 
                lNum++; 
            }
            chr = buffer.Read();
            cNum++;
        }

        void MarkToken()
        {
            tokPos = buffer.ReadPos;
            tokNum = cNum;
            tokLin = lNum;
            tokCol = cNum - lineStartNum;
        }
        
        void MarkEnd()
        {
            tokTxt = null;
            tokLen = cNum - tokNum;
            tokEPos = buffer.ReadPos;
            tokELin = lNum;
            tokECol = cNum - lineStartNum;
        }
 
        // ================ StringBuffer Initialization ===================

        public void SetSource(string source, int offset)
        {
            this.buffer = new StringBuff(source);
            this.buffer.Pos = offset;
            this.cNum = offset - 1;
            this.chr = '\n'; // to initialize yyline, yycol and lineStart
            GetChr();
        }
        
        public int GetNext(ref int state, out int start, out int end)
        {
            Tokens next;
            EolState = state;
            next = (Tokens)Scan();
            state = EolState;
            start = tokPos;
            end = tokEPos - 1; // end is the index of last char.
            return (int)next;
        }

        // ======== IScanner<> Implementation =========

        public override int yylex()
        {
            // parserMax is set by reflecting on the Tokens
            // enumeration.  If maxParseTokeen is defined
            // that is used, otherwise int.MaxValue is used.
            //
            int next;
            do { next = Scan(); } while (next >= parserMax);
            return next;
        }
        
        int yyleng { get { return tokLen; } }
        int yypos { get { return tokPos; } }
        int yyline { get { return tokLin; } }
        int yycol { get { return tokCol; } }

        public string yytext
        {
            get 
            {
                if (tokTxt == null) 
                    tokTxt = buffer.GetString(tokPos, tokEPos);
                return tokTxt;
            }
        }

        void yyless(int n) { 
            buffer.Pos = tokPos;
            cNum = tokNum;
            for (int i = 0; i <= n; i++) GetChr();
            MarkEnd();
        }

        public IErrorHandler Handler { get { return this.handler; }
                                       set { this.handler = value; }}

        // ============ methods available in actions ==============

        internal int YY_START {
            get { return CurrentSc; }
            set { CurrentSc = value; } 
        }

        // ============== The main tokenizer code =================

        int Scan()
        {
                for (; ; )
                {
                    int next;              // next state to enter                   
#if BACKUP
                    bool inAccept = false; // inAccept ==> current state is an accept state
                    Result rslt = Result.noMatch;
                    // skip "idle" transitions
#if LEFTANCHORS
                    if (lineStartNum == cNum && NextState(anchorState[CurrentSc]) != currentStart)
                        state = anchorState[CurrentSc];
                    else {
                        state = currentStart;
                        while (NextState() == state) {
                            GetChr();
                            if (lineStartNum == cNum) {
                                int anchor = anchorState[CurrentSc];
                                if (NextState(anchor) != state) {
                                    state = anchor; 
                                    break;
                                }
                            }
                        }
                    }
#else // !LEFTANCHORS
                    state = currentStart;
                    while (NextState() == state) 
                        GetChr(); // skip "idle" transitions
#endif // LEFTANCHORS
                    MarkToken();
                    
                    while ((next = NextState()) != currentStart)
                        if (inAccept && next > maxAccept) // need to prepare backup data
                        {
                            Context ctx = new Context();
                            rslt = Recurse2(ctx, next);
                            if (rslt == Result.noMatch) RestoreStateAndPos(ctx);
                            // else if (rslt == Result.contextFound) RestorePos(ctx);
                            break;
                        }
                        else
                        {
                            state = next;
                            GetChr();
                            if (state <= maxAccept) inAccept = true;
                        }
#else // !BACKUP
#if LEFTANCHORS
                    if (lineStartNum == cNum) {
                        int anchor = anchorState[CurrentSc];
                        if (NextState(anchor) != currentStart)
                            state = anchor;
                    }
                    else {
                        state = currentStart;
                        while (NextState() == state) {
                            GetChr();
                            if (lineStartNum == cNum) {
                                anchor = anchorState[CurrentSc];
                                if (NextState(anchor) != state) {
                                    state = anchor;
                                    break;
                                }
                            }
                        }
                    }
#else // !LEFTANCHORS
                    state = currentStart;
                    while (NextState() == state) 
                        GetChr(); // skip "idle" transitions
#endif // LEFTANCHORS
                    MarkToken();
                    // common code
                    while ((next = NextState()) != currentStart)
                    {
                        state = next;
                        GetChr();
                    }
#endif // BACKUP
                    if (state > maxAccept) 
                        state = currentStart;
                    else
                    {
                        MarkEnd();
#region ActionSwitch
#pragma warning disable 162
    switch (state)
    {
        case eofNum:
            return (int)Tokens.EOF;
        case 1:
        case 55:
/* Ignore */
            break;
        case 2:
return (int)ShaderToken.OPERATOR;
            break;
        case 3:
        case 5:
        case 12:
        case 13:
        case 17:
return (int)ShaderToken.UNDEFINED;
            break;
        case 4:
return (int)ShaderToken.OPERATOR;
            break;
        case 6:
return (int)ShaderTokenProvider.GetTokenFromIdentifier(yytext);
            break;
        case 7:
        case 8:
return (int)ShaderToken.NUMBER;
            break;
        case 9:
return (int)ShaderToken.OPERATOR;
            break;
        case 10:
return (int)ShaderToken.OPERATOR;
            break;
        case 11:
return (int)ShaderToken.OPERATOR;
            break;
        case 14:
return (int)ShaderToken.OPERATOR;
            break;
        case 15:
return (int)ShaderToken.OPERATOR;
            break;
        case 16:
return (int)ShaderToken.OPERATOR;
            break;
        case 18:
return (int)ShaderToken.OPERATOR;
            break;
        case 19:
return (int)ShaderToken.OPERATOR;
            break;
        case 20:
return (int)ShaderToken.OPERATOR;
            break;
        case 21:
return (int)ShaderToken.DELIMITER;
            break;
        case 22:
return (int)ShaderToken.LEFT_BRACKET;
            break;
        case 23:
return (int)ShaderToken.RIGHT_BRACKET;
            break;
        case 24:
return (int)ShaderToken.DELIMITER;
            break;
        case 25:
return (int)ShaderToken.LEFT_PARENTHESIS;
            break;
        case 26:
return (int)ShaderToken.RIGHT_PARENTHESIS;
            break;
        case 27:
return (int)ShaderToken.LEFT_SQUARE_BRACKET;
            break;
        case 28:
return (int)ShaderToken.RIGHT_SQUARE_BRACKET;
            break;
        case 29:
return (int)ShaderToken.OPERATOR;
            break;
        case 30:
return (int)ShaderToken.OPERATOR;
            break;
        case 31:
return (int)ShaderToken.OPERATOR;
            break;
        case 32:
return (int)ShaderToken.OPERATOR;
            break;
        case 33:
return (int)ShaderToken.OPERATOR;
            break;
        case 34:
return (int)ShaderToken.OPERATOR;
            break;
        case 35:
return (int)ShaderToken.OPERATOR;
            break;
        case 36:
return (int)ShaderToken.PREPROCESSOR;
            break;
        case 37:
return (int)ShaderToken.STRING_LITERAL;
            break;
        case 38:
        case 39:
        case 40:
return (int)ShaderToken.FLOAT;
            break;
        case 41:
return (int)ShaderToken.OPERATOR;
            break;
        case 42:
return (int)ShaderToken.OPERATOR;
            break;
        case 43:
        case 44:
        case 45:
return (int)ShaderToken.FLOAT;
            break;
        case 46:
return (int)ShaderToken.FLOAT;
            break;
        case 47:
return (int)ShaderToken.NUMBER;
            break;
        case 48:
return (int)ShaderTokenProvider.GetTokenFromSemantics(yytext);
            break;
        case 49:
return (int)ShaderToken.OPERATOR;
            break;
        case 50:
return (int)ShaderToken.COMMENT_LINE;
            break;
        case 51:
        case 53:
BEGIN(COMMENT); return (int)ShaderToken.COMMENT;
            break;
        case 52:
return (int)ShaderToken.OPERATOR;
            break;
        case 54:
return (int)ShaderToken.COMMENT;
            break;
        case 56:
        case 57:
        case 58:
        case 59:
return (int)ShaderToken.COMMENT;
            break;
        case 60:
BEGIN(INITIAL); return (int)ShaderToken.COMMENT;
            break;
        default:
            break;
    }
#pragma warning restore 162
#endregion
                    }
                }
        }

#if BACKUP
        Result Recurse2(Context ctx, int next)
        {
            // Assert: at entry "state" is an accept state AND
            //         NextState(state, chr) != currentStart AND
            //         NextState(state, chr) is not an accept state.
            //
            bool inAccept;
            SaveStateAndPos(ctx);
            state = next;
            if (state == eofNum) return Result.accept;
            GetChr();
            inAccept = false;

            while ((next = NextState()) != currentStart)
            {
                if (inAccept && next > maxAccept) // need to prepare backup data
                    SaveStateAndPos(ctx);
                state = next;
                if (state == eofNum) return Result.accept;
                GetChr(); 
                inAccept = (state <= maxAccept);
            }
            if (inAccept) return Result.accept; else return Result.noMatch;
        }

        void SaveStateAndPos(Context ctx)
        {
            ctx.bPos  = buffer.Pos;
            ctx.cNum  = cNum;
            ctx.state = state;
            ctx.cChr = chr;
        }

        void RestoreStateAndPos(Context ctx)
        {
            buffer.Pos = ctx.bPos;
            cNum = ctx.cNum;
            state = ctx.state;
            chr = ctx.cChr;
        }

        void RestorePos(Context ctx) { buffer.Pos = ctx.bPos; cNum = ctx.cNum; }
#endif // BACKUP

        // ============= End of the tokenizer code ================

        internal void BEGIN(int next)
        { CurrentSc = next; }

#if STACK        
        internal void yy_clear_stack() { scStack.Clear(); }
        internal int yy_top_state() { return scStack.Peek(); }
        
        internal void yy_push_state(int state)
        {
            scStack.Push(CurrentSc);
            CurrentSc = state;
        }
        
        internal void yy_pop_state()
        {
            // Protect against input errors that pop too far ...
            if (scStack.Count > 0) {
				int newSc = scStack.Pop();
				CurrentSc = newSc;
            } // Otherwise leave stack unchanged.
        }
 #endif // STACK

        internal void ECHO() { Console.Out.Write(yytext); }
        
    } // end class Scanner
} // end namespace
