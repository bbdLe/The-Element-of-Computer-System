using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class Parse
    {
        public Parse(Tokenizer pTokenizer, string pOutFileName)
        {
            mTokenizer = pTokenizer;
            swriter = new StreamWriter(pOutFileName);
        }

        public void close()
        {
            swriter.Close();
        }

        public void start()
        {
            ++indentLevel;
            writeIndent("<tokens>");
            tempParse();
            writeIndent("</tokens>");
            --indentLevel;
        }

        public void tempParse()
        {
            ++indentLevel;
            while(mTokenizer.hasMoreTokens())
            {
                mTokenizer.advance();
                var tokenType = mTokenizer.tokenType();
                writeIndent("<" + tokenType + ">" + WebUtility.HtmlEncode(mTokenizer.currToken()) + "</" + tokenType + ">");
            }
            --indentLevel;
        }

        private void writeIndent(string str)
        {
            for(int i = 0; i < indentLevel; ++i)
            {
                swriter.Write("\t");
            }
            swriter.WriteLine(str);
        }

        private Tokenizer mTokenizer;
        private StreamWriter swriter;
        int indentLevel = -1;
    }
}
