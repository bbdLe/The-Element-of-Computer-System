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

        public void parseClass()
        {
            writeIndent(@"<class>");
            parseKeyWord();     // class
            parseIdentifiter(); // SquareGame
            parseSymbol();      // {
            while(mTokenizer.hasMoreTokens())
            {
                mTokenizer.advance();
                if(mTokenizer.tokenType() != "keyword" && mTokenizer.tokenType() != "symbol")
                {
                    Console.WriteLine("Need a keyword : field or other, but get " +  mTokenizer.tokenType() + mTokenizer.currToken());
                    Environment.Exit(-1);
                }
                string keyword = mTokenizer.currToken();
                mTokenizer.backward();
                if(fieldKeyword.Contains(keyword))
                {
                    parseClassVarDec();
                }
                else if(functionKeyword.Contains(keyword))
                {
                    parseSubroutineDec();
                }
                else
                {
                    parseSymbol();
                    break;
                }
            }
            writeIndent(@"</class>");
        }

        public void parseSubroutineDec()
        {
            writeIndent(@"<subroutineDec>");
            parseKeyWord();     // method or consturctor
            if(!mTokenizer.hasMoreTokens())
            {
                wrongMessage("Need more tokens");
            }
            mTokenizer.advance();
            string tokenType = mTokenizer.tokenType();  // return type
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
                wrongMessage("Need keyword or identifier, but get " + tokenType);
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
            writeIndent("<subroutineBody>");
            parseSymbol();  // {
            while(mTokenizer.hasMoreTokens())
            {
                mTokenizer.advance();
                string tokenType = mTokenizer.tokenType();
                string token = mTokenizer.currToken();
                mTokenizer.backward();
                if(tokenType == "symbol")       // }
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
            writeIndent("</subroutineBody>");

        }

        public void parseParameterList()
        {
            writeIndent("<parameterList>");
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
                        writeIndent("</parameterList>");
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
                    wrongMessage("Wrong type");
                }
                parseIdentifiter();             // name
                if (mTokenizer.hasMoreTokens())
                    mTokenizer.advance();
                else
                    wrongMessage("Need more token");
                tokenType = mTokenizer.tokenType();
                string token = mTokenizer.currToken();      // , or )
                mTokenizer.backward();
                if(tokenType != "symbol")
                {
                    wrongMessage("need Symbol" + tokenType + token);
                }
                else
                {
                    if (token == ")")   // )
                    {
                        writeIndent("</parameterList>");
                        return;
                    }
                    else
                    {
                        parseSymbol();  // ,
                    }
                }
            }
            writeIndent("</parameterList>");
        }

        public void parseVarDec()
        {
            writeIndent(@"<varDec>");
            parseKeyWord();      // var
            if (mTokenizer.hasMoreTokens())
            {
                mTokenizer.advance();
            }
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
            while (parseStatement()) ;
            writeIndent(@"</statements>");
        }

        public bool parseStatement()
        {
            if (mTokenizer.hasMoreTokens())
            {
                mTokenizer.advance();
            }
            string tokenType = mTokenizer.tokenType();
            string token = mTokenizer.currToken();
            mTokenizer.backward();
            if (tokenType != "keyword")
            {
                return false;
            }
            if (token == "let")
            {
                parseLetStatement();
            }
            else if (token == "if")
            {
                parseIfStatement();
            }
            else if (token == "while")
            {
                parseWhileStatement();
            }
            else if (token == "do")
            {
                parseDoStatement();
            }
            else if (token == "return")
            {
                parseReturn();
            }
            else
            {
                return false;
            }
            return true;
        }

        public void parseClassVarDec()
        {
            writeIndent(@"<classVarDec>");
            ++indentLevel;
            parseKeyWord();     // field;
            string tokenType = "";
            if (!mTokenizer.hasMoreTokens())     // type
            {
                wrongMessage("Wrong Result");
            }
            mTokenizer.advance();
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
                wrongMessage("Wrong type");
            }

            parseIdentifiter(); // name
            parseSymbol();      // symbol
            while (mTokenizer.currToken() == ",")
            {
                parseIdentifiter();
                parseSymbol();
            }
            --indentLevel;
            writeIndent(@"</classVarDec>");
        }

        public void parseExpression()
        {
            writeIndent("<expression>");
            parseTerm();
            if ( mTokenizer.hasMoreTokens())
            {
                mTokenizer.advance();
            }
            string tokenType = mTokenizer.tokenType();
            string token = mTokenizer.currToken();
            mTokenizer.backward();
            while (tokenType == "symbol" && opList.Contains(token))
            {
                parseSymbol();
                parseTerm();
                if (mTokenizer.hasMoreTokens())
                {
                    mTokenizer.advance();
                }
                tokenType = mTokenizer.tokenType();
                token = mTokenizer.currToken();
                mTokenizer.backward();
            }
            writeIndent("</expression>");
        }

        public void parseLetStatement()
        {
            writeIndent("<letStatement>");
            parseKeyWord();         // let
            parseIdentifiter();     // varName
            if (mTokenizer.hasMoreTokens())
            {
                mTokenizer.advance();
            }
            string token = mTokenizer.currToken();
            string tokenType = mTokenizer.tokenType();
            mTokenizer.backward();
            if (token == "[")
            {
                parseSymbol();      // [
                parseExpression();  // expression
                parseSymbol();      // ]
            }
            parseSymbol();      // =
            parseExpression();  // expression
            parseSymbol();      // ;
            writeIndent("</letStatement>");
        }

        public void parseIfStatement()
        {
            writeIndent("<ifStatement>");
            parseKeyWord();     // if
            parseSymbol();      // (
            parseExpression();  // expression
            parseSymbol();      // )
            parseSymbol();      // {
            parseStatements();  // statements
            parseSymbol();      // }
            if (mTokenizer.hasMoreTokens())
            {
                mTokenizer.advance();
            }
            string token = mTokenizer.currToken();
            string tokenType = mTokenizer.tokenType();
            mTokenizer.backward();
            if (tokenType == "keyword" && token == "else")
            {
                parseKeyWord();     // else
                parseSymbol();      // {
                parseStatements();
                parseSymbol();      // }
            }
            writeIndent("</ifStatement>");
        }

        public void parseWhileStatement()
        {
            writeIndent("<whileStatement>");
            parseKeyWord(); // while
            parseSymbol();  // (
            parseExpression(); // expression
            parseSymbol();  // )
            parseSymbol();  // {
            parseStatements();  // statements
            parseSymbol();  // }
            writeIndent("</whileStatement>");
        }

        public void parseDoStatement()
        {
            writeIndent("<doStatement>");
            parseKeyWord();     // do
            parseSubroutineCall();
            parseSymbol();      // ;
            writeIndent("</doStatement>");
        }

        public void parseReturn()
        {
            writeIndent("<returnStatement>");
            parseKeyWord();     // return
            if (mTokenizer.hasMoreTokens())
            {
                mTokenizer.advance();
            }
            else
            {
                wrongMessage("Need More token");
            }
            string token = mTokenizer.currToken();
            mTokenizer.backward();
            if (token != ";")
            {
                parseExpression();  // expression
            }
            parseSymbol();      // ;
            writeIndent("</returnStatement>");
        }

        public void parseSubroutineCall()
        {
            parseIdentifiter(); // var or className
            parseSymbol();      // ( or .
            string token = mTokenizer.currToken();
            if ( token == "(" )
            {
                parseExpressionList();
                parseSymbol();  // )
            }
            else if ( token == "." )
            {
                parseIdentifiter(); // class subprocess
                parseSymbol();      // (
                parseExpressionList();  // expressionList
                parseSymbol();      // )
            }
        }

        public void parseExpressionList()
        {
            writeIndent("<expressionList>");
            if (mTokenizer.hasMoreTokens())
            {
                mTokenizer.advance();
            }
            string token = mTokenizer.currToken();
            mTokenizer.backward();
            if (token == ")")
            {
                writeIndent("</expressionList>");
                return;
            }
            parseExpression();
            if (mTokenizer.hasMoreTokens())
            {
                mTokenizer.advance();
                token = mTokenizer.currToken();
                string tokenType = mTokenizer.tokenType();
                mTokenizer.backward();
                while (tokenType == "symbol" && token == ",")
                {
                    parseSymbol();
                    parseExpression();
                    mTokenizer.advance();
                    token = mTokenizer.currToken();
                    tokenType = mTokenizer.tokenType();
                    mTokenizer.backward();
                }
            }
            writeIndent("</expressionList>");
        }

        public void parseTerm()
        {
            writeIndent("<term>");
            if ( mTokenizer.hasMoreTokens())
            {
                mTokenizer.advance();
            }
            string tokenType = mTokenizer.tokenType();
            string token = mTokenizer.currToken();
            if (tokenType == "integerConstant")
            {
                writeIndent("<integerConstant>" + token + "</integerConstant>");
            }
            else if (tokenType == "stringConstant")
            {
                writeIndent("<stringConstant>" + token.Trim('\"') + "</stringConstant>");
            }
            else if (tokenType == "keyword" && (token == "true" || token == "false" || token == "null" || token == "this"))
            {
                writeKeyWord();
            }
            else if (tokenType == "identifier")
            {
                string identify = token;
                if (mTokenizer.hasMoreTokens())
                {
                    mTokenizer.advance();
                }
                token = mTokenizer.currToken();
                tokenType = mTokenizer.tokenType();
                mTokenizer.backward();
                if (tokenType == "symbol" && (token == "[" || token == "(" || token == "."))
                {
                    if (token == "[")
                    {
                        writeIndent("<identifier>" + identify + "</identifier>");
                        parseSymbol();          // [
                        parseExpression();
                        parseSymbol();
                    }
                    else
                    {

                        mTokenizer.backward();
                        parseSubroutineCall();
                    }
                    Console.WriteLine("you");

                }
                else
                {
                    writeIndent("<identifier>" + identify + "</identifier>");
                }
            }
            else if (tokenType == "symbol")
            {
                token = mTokenizer.currToken();
                if(token == "(")
                {
                    mTokenizer.backward();
                    parseSymbol();
                    parseExpression();
                    parseSymbol();
                }
                if(token == "~" || token == "-")
                {
                    mTokenizer.backward();
                    parseSymbol();
                    parseTerm();
                }
            }
            writeIndent("</term>");
        }

        public void parseKeyWord()
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
                    Console.Write("Need a keyword, but get " + mTokenizer.tokenType() + mTokenizer.currToken());
                    Environment.Exit(-1);
                }
            }
        }

        public void parseIdentifiter()
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
                    Console.Write("Need a Identifiter, but get " + mTokenizer.tokenType() + mTokenizer.currToken());
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

        private void writeIndent(string str)
        {
            for(int i = 0; i < indentLevel; ++i)
            {
                swriter.Write("\t");
            }
            swriter.WriteLine(str);
            Console.WriteLine(str);
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
            writeLabel("identifier");
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
        private List<string> opList = new List<string>
        {
            "+",
            "-",
            "*",
            "/",
            "&",
            "|",
            "<",
            ">",
            "="
        };
        private int indentLevel = 0;
        private List<string> functionKeyword = new List<string>
        {
            "method",
            "constructor",
            "function"
        };
        private List<string> fieldKeyword = new List<string>
        {
            "static",
            "field"
        };
        private List<string> keywordList = new List<string>
        {
            "true",
            "false",
            "null",
            "this"
        };
    }
}
