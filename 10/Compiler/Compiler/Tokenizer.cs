using System.Text.RegularExpressions;
using System.IO;
using System;

namespace Compiler
{
    class Tokenizer
    {
        public Tokenizer(string fileName)
        {
            strs = File.ReadAllText(fileName);
        }

        public bool hasMoreTokens()
        {
            skitWhiteSpace();
            skitComment();

            return curr_count >= strs.Length? false : true;
        }

        public void advance()
        {
            string str = keyWord();
            if(str != string.Empty)
            {
                mKeyword = str;
                mTokenType = "KeyWord";
                return;
            }

            str = findSymbol();
            if(str != string.Empty)
            {
                mSymbol = str;
                mTokenType = "Symbol";
                return;
            }

            str = intNumber();
            if(str != string.Empty)
            {
                mInt = str;
                mTokenType = "Int";
            }

            str = constString();
            if(str != string.Empty)
            {
                mString = str;
                mTokenType = "String";
            }

            str = identify();
            if(str != string.Empty)
            {
                midentifier = str;
                mTokenType = "Identify";
            }

            Console.WriteLine("Bad Result");
            Environment.Exit(-1);
        }

        public string identifier()
        {
            return midentifier;
        }

        public string symbol()
        {
            return mSymbol;
        }

        public string stringVal()
        {
            return mString;
        }

        public string intVal()
        {
            return mInt;
        }

        public string tokenType()
        {
            return mTokenType;
        }

        private string singleLineComment()
        {
            return regexTemplate(singleLineCommentRegex);
        }

        private string multiLineComment()
        {
            return regexTemplate(multiLineCommentRegex);
        }

        private string keyWord()
        {
            return regexTemplate(keyWordRegex);
        }

        private string identify()
        {
            return regexTemplate(identifyRegex);
        }

        private string constString()
        {
            return regexTemplate(stringRegex);
        }

        private string intNumber()
        {
            return regexTemplate(intRegex);
        }

        private string findSymbol()
        {
            return regexTemplate(symbolRegex);
        }


        private string regexTemplate(Regex regex)
        {
            var match = regex.Match(strs, curr_count);

            if(match.Success == true && match.Index == curr_count)
            {
                return strs.Substring(match.Index, match.Length);
            }
            else
            {
                return string.Empty;
            }
        }

        private void skitWhiteSpace()
        {
            var match = whiteSpaceRegex.Match(strs, curr_count);

            if (match.Success == true && match.Index == curr_count)
                curr_count += match.Length;
        }

        private void skitComment()
        {
            var str = singleLineComment();
            curr_count += str.Length;
            str = multiLineComment();
            curr_count += str.Length;
        }

        private Regex singleLineCommentRegex = new Regex(@"//.*");
        private Regex keyWordRegex = new Regex(@"class\s+|method\s+|int\s+|
                                                 function\s+|boolean\s+|
                                                 constructor\s+|char\s+|
                                                 void\s+|var\s+|static\s+|
                                                 field\s+|let\s+|do\s+|
                                                 if\s+|else\s+|while\s+|
                                                 return\s+|true\s+|false\s+|
                                                 null\s+|this\s+");
        private Regex symbolRegex = new Regex(@"\{|\}|\(|\)|\[|\]|\.|,|;|\+|-|\*|/|&|\||<|>|=|~");
        private Regex identifyRegex = new Regex(@"[a-zA-Z_]\w*");
        private Regex stringRegex = new Regex(@""".+""");
        private Regex multiLineCommentRegex = new Regex(@"/\*.*\*/");
        private Regex intRegex = new Regex(@"\d*");
        private Regex whiteSpaceRegex = new Regex(@"\s+");

        private string strs;
        private string tokens;
        private int curr_count;

        private string mSymbol;
        private string midentifier;
        private string mInt;
        private string mString;
        private string mKeyword;
        private string mTokenType;
    }
}
