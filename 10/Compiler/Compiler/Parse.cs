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

        //public void start()
        //{
        //    ++indentLevel;
        //    writeIndent("<tokens>");
        //    tempParse();
        //    writeIndent("</tokens>");
        //    --indentLevel;
        //}

        public void parseClass()
        {
            writeIndent(@"<class>");
            ++indentLevel;
            parseKeyWord();     // class
            parseIdentifiter(); // SquareGame
            parseSymbol();      // {
            while(mTokenizer.hasMoreTokens())
            {
                if(mTokenizer.tokenType() != "keyword")
                {
                    Console.WriteLine("Need a keyword : field or other, but get ", mTokenizer.tokenType());
                    Environment.Exit(-1);
                }
                string keyword = mTokenizer.currToken();
                if(keyword == "field")
                {
                    parseClassVarDec();
                }
                else
                {
                    parseSubroutineDec();
                }
            }
            --indentLevel;
            writeIndent(@"<\class>");
        }

        public void parseSubroutineDec()
        {
            writeIndent(@"<subroutineDec>");
            parseKeyWord();     // method or consturctor
            if(!mTokenizer.hasMoreTokens())
            {
                Console.WriteLine("Need more tokens");
            }
            mTokenizer.advance();
            string tokenType = mTokenizer.tokenType();  // return type
            if(tokenType == "keyword")
            {
                parseKeyWord();
            }
            else if(tokenType == "identifier")
            {
                parseIdentifiter();
            }
            else
            {
                Console.WriteLine("Need keyword or identifier, but get " + tokenType);
            }
            parseIdentifiter();     // function name
            parseSymbol();          // (
            parseParameterList();
            parseSymbol();          //  )
            parseSubroutineBody();
            writeIndent(@"</subroutineDec>");
        }

        public void parseSubroutineBody()
        {
            parseSymbol();  // {
            while(mTokenizer.hasMoreTokens())
            {
                mTokenizer.advance();
                string tokenType = mTokenizer.tokenType();
                string token = mTokenizer.currToken();
                mTokenizer.backward();
                if(tokenType == "symbol")       // {
                {
                    break;
                }
                if(tokenType == "keyword" && token == "var")
                {
                    parseVarDec();
                }
                else
                {
                    parseStatements();
                    break;
                }

            }
            parseSymbol();  // }
        }

        public void parseParameterList()
        {
            if(mTokenizer.hasMoreTokens())
            {
                mTokenizer.advance();
                string token = mTokenizer.currToken();
                string tokenType = mTokenizer.tokenType();
                mTokenizer.backward();
                if (tokenType == "symbol")
                {
                    if(token == ")")
                    {
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Wrong type");
                        Environment.Exit(-1);
                    }
                }
            }

            while(mTokenizer.hasMoreTokens())
            {
                mTokenizer.advance();                       // type
                string tokenType = mTokenizer.tokenType();
                mTokenizer.backward();
                if (tokenType == "keyword")
                {
                    parseKeyWord();
                }
                else if(tokenType == "identifier")
                {
                    parseIdentifiter();
                }
                else
                {
                    Console.WriteLine("Wrong type");
                    Environment.Exit(-1);
                }
                parseIdentifiter();             // name
                mTokenizer.advance();
                tokenType = mTokenizer.tokenType();
                string token = mTokenizer.currToken();      // , or )
                mTokenizer.backward();
                if(tokenType != "symbol")
                {
                    Console.WriteLine("need Symbol");
                }
                else
                {
                    if (token == ")")   // )
                    {
                        return;
                    }
                    else
                    {
                        parseSymbol();  // ,
                    }
                }
            }
        }

        public void parseVarDec()
        {
            writeIndent(@"<varDec>");
            parseSymbol();      // var
            mTokenizer.advance();
            string token = mTokenizer.currToken();
            string tokenType = mTokenizer.tokenType();
            mTokenizer.backward();
            if(tokenType == "keyword")
            {
                parseKeyWord();
            }
            else if(tokenType == "identifier")
            {
                parseIdentifiter();
            }
            else
            {
                wrongMessage("Wrong type");
            }
            while(mTokenizer.hasMoreTokens())
            {
                parseIdentifiter();

                if(!mTokenizer.hasMoreTokens())
                {
                    wrongMessage("Need more tokens");
                }
                parseSymbol();      // , or ;
                if (mTokenizer.currToken() == ";")
                    break;
            }
            writeIndent(@"</varDec>");
        }

        public void parseStatements()
        {
            writeIndent(@"<statements>");
            

            writeIndent(@"</statements>");
        }

        public void parseClassVarDec()
        {
            writeIndent(@"<classVarDec>");
            ++indentLevel;
            parseKeyWord();     // field;
            string tokenType = "";
            if (!mTokenizer.hasMoreTokens())     // type
            {
                Console.Write("Wrong Result");
                Environment.Exit(-1);
            }
            tokenType = mTokenizer.tokenType();
            mTokenizer.backward();

            if (tokenType == "keyword")
            {
                parseKeyWord();
            }
            else if (tokenType == "identifier")
            {
                parseIdentifiter();
            }
            else
            {
                Console.WriteLine("Wrong type");
                Environment.Exit(-1);
            }

            parseIdentifiter(); // name
            parseSymbol();      // symbol
            --indentLevel;
            writeIndent(@"</classVarDec>");
        }

        public void parseKeyWord()
        {
            if (mTokenizer.hasMoreTokens())
            {
                mTokenizer.advance();
                if (mTokenizer.tokenType() == "identifier")     // class
                {
                    writeIdentifiter();
                }
                else
                {
                    Console.Write("Need a identifier, but get " + mTokenizer.tokenType());
                    Environment.Exit(-1);
                }
            }
        }

        public void parseIdentifiter()
        {
            if (mTokenizer.hasMoreTokens())
            {
                mTokenizer.advance();
                if (mTokenizer.tokenType() == "keyword")     // class
                {
                    writeKeyWord();
                }
                else
                {
                    Console.Write("Need a keyword, but get " + mTokenizer.tokenType());
                    Environment.Exit(-1);
                }
            }
        }

        public void parseSymbol()
        {
            if (mTokenizer.hasMoreTokens())
            {
                mTokenizer.advance();
                if (mTokenizer.tokenType() == "symbol")     // class
                {
                    writeSymbol();
                }
                else
                {
                    Console.Write("Need a symbol, but get " + mTokenizer.tokenType());
                    Environment.Exit(-1);
                }
            }
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

        private void writeKeyWord()
        {
            writeLabel("keyword");
        }

        private void writeSymbol()
        {
            writeLabel("symbol");
        }

        private void writeIdentifiter()
        {
            writeLabel("identifiter");
        }

        private void wrongMessage(string msg)
        {
            Console.WriteLine(msg);
            Environment.Exit(-1);
        }

        private void writeLabel(string label)
        {
            writeIndent(@"<" + label + @"> " + WebUtility.HtmlEncode(mTokenizer.currToken()) + @" </" + label + @">");
        }

        private Tokenizer mTokenizer;
        private StreamWriter swriter;
        int indentLevel = 0;
    }
}
