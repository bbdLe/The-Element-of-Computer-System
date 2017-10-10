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
            return curr_count >= tokens.Length? false : true;
        }

        public void advance()
        {

        }

        private string singleLineComment()
        {
            var match = singleLineCommentRegex.Match(strs, curr_count);

            if (match.Success == false)
                return string.Empty;
            else
            {
                return strs.Substring(match.Index, match.Length);
            }
        }

        private string multiLineComment()
        {
            var match = multiLineCommentRegex.Match(strs, curr_count);

            if (match.Success == false)
                return string.Empty;
            else
            {
                return strs.Substring(match.Index, match.Length);
            }
        }

        private string whitSpace()
        {
            var match = multiLineCommentRegex.Match(strs, curr_count);

            if (match.Success == false)
                return string.Empty;
            else
            {
                return strs.Substring(match.Index, match.Length);
            }
        }

        private string 

        private Regex singleLineCommentRegex = new Regex(@"//.*");
        private Regex multiLineCommentRegex = new Regex(@"/\*.*\*/");
        private Regex whiteSpaceRegex = new Regex(@"\s");
        private string strs;
        private string tokens;
        private int curr_count;
    }
}
