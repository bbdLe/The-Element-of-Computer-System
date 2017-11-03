using System.Text.RegularExpressions;
using System.IO;
using System;
using System.Collections.Generic;

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
            if (tokenList.Count - 1 == tokenListIndex && lastTimeIndex != tokenListIndex)
            {
                curr_count += mcurr_token.Length;
                while (skitComment() || skitWhiteSpace()) { }
                lastTimeIndex = tokenListIndex;
                return curr_count >= strs.Length ? false : true;
            }
            else
            {
                return true;
            }
        }

        public void backward()
        {
            if(curr_count > 0)
            {
                --tokenListIndex;
            }
            else
            {
                Console.WriteLine("curr_count < 0");
                Environment.Exit(-1);
            }
        }

        public void advance()
        {
            if(tokenList.Count != 0 && tokenListIndex != tokenList.Count - 1)
            {
                ++tokenListIndex;
            }
            else
            {
                _advance();
                tokenList.Add(mcurr_token);
                tokenTypeList.Add(mTokenType);
                ++tokenListIndex;
            }
        }

        private void _advance()
        {
            string str = keyWord();
            if(str != string.Empty)
            {
                mcurr_token = str;
                mTokenType = "keyword";
                return;
            }

            str = findSymbol();
            if(str != string.Empty)
            {
                mcurr_token = str;
                mTokenType = "symbol";
                return;
            }

            str = intNumber();
            if(str != string.Empty)
            {
                mcurr_token = str;
                mTokenType = "integerConstant";
                return;
            }

            str = constString();
            if(str != string.Empty)
            {
                mcurr_token = str;
                mTokenType = "stringConstant";
                return;
            }

            str = identify();
            if(str != string.Empty)
            {
                mcurr_token = str;
                mTokenType = "identifier";
                return;
            }

            Console.WriteLine(strs.Substring(curr_count, 6));
            Console.WriteLine("Bad Result");
            Environment.Exit(-1);
        }

        public string identifier()
        {
            return currToken();
        }

        public string symbol()
        {
            return currToken();
        }

        public string stringVal()
        {
            return currToken();
        }

        public string intVal()
        {
            return currToken();
        }

        public string tokenType()
        {
            return tokenTypeList[tokenListIndex];
        }

        public string currToken()
        {
            return tokenList[tokenListIndex];
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
                return strs.Substring(match.Index, match.Length).TrimEnd();
            }
            else
            {
                return string.Empty;
            }
        }

        private bool skitWhiteSpace()
        {
            var match = whiteSpaceRegex.Match(strs, curr_count);

            if (match.Success == true && match.Index == curr_count)
            {
                curr_count += match.Length;
                return true;
            }

            return false;
        }

        private bool skitComment()
        {
            int temp_count = curr_count;
            var str = singleLineComment();
            curr_count += str.Length;
            str = multiLineComment();
            curr_count += str.Length;

            return curr_count != temp_count ? true : false;
        }

        private Regex singleLineCommentRegex = new Regex(@"//.*\n");
        private Regex keyWordRegex = new Regex(@"class\s+|method\s+|int\s+|function\s+|boolean\s+|constructor\s*|char\s+|void\s+|var\s+|static\s+|field\s+|let\s+|do\s+|if\s*|else\s*|while\s*|return\s*|true\s*|false\s*|null\s*|this\s*");
        private Regex symbolRegex = new Regex(@"\{|\}|\(|\)|\[|\]|\.|,|;|\+|-|\*|/|&|\||<|>|=|~");
        private Regex identifyRegex = new Regex(@"[a-zA-Z_]\w*");
        private Regex stringRegex = new Regex(@""".+""");
        private Regex multiLineCommentRegex = new Regex(@"/\*[\s\S]*?\*/");
        private Regex intRegex = new Regex(@"\d*");
        private Regex whiteSpaceRegex = new Regex(@"\s+");

        private string strs;
        private int lastTimeIndex = -2;
        private List<string> tokenList = new List<string>();
        private List<string> tokenTypeList = new List<string>();
        private int tokenListIndex = -1;
        private int curr_count = 0;
        public string mcurr_token = string.Empty;
        private string mTokenType;
    }
}
