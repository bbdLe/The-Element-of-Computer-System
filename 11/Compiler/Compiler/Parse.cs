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
            outputFileName = pOutFileName;
            mTokenizer = pTokenizer;
            swriter = new StreamWriter(pOutFileName);
        }

        public void close()
        {
            swriter.Close();
        }

        public void parseClass()
        {
            parseKeyWord();     // class
            parseIdentifiter(); // SquareGame
            className = mTokenizer.currToken();
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
        }

        public void parseSubroutineDec()
        {
            
            parseKeyWord();     // method or consturctor or function
            string functionType = mTokenizer.currToken();
            symbolTable.startSubroutine(functionType);
            if (!mTokenizer.hasMoreTokens())
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
            currFunctionName = mTokenizer.currToken();
            parseSymbol();          // (
            parseParameterList();
            parseSymbol();          //  )
            parseSubroutineBody(functionType);
        }

        public void parseSubroutineBody(string functionType)
        {
            int count = 0;
            bool firstTime = true;
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
                    count += parseVarDec();
                }
                else
                {
                    if (firstTime == true)
                    {
                        writeFunction(currFunctionName, count);
                        if (functionType == "constructor")
                        {
                            writePush("constant", symbolTable.varCount("FIELD"));
                            writeCall("Memory.alloc", 1);
                            writePop("pointer", 0);
                        }
                        else if(functionType != "function")
                        {
                            writePush("argument", 0);
                            writePop("pointer", 0);
                        }
                        firstTime = false;
                    }
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
                string type = mTokenizer.currToken();
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
                symbolTable.define(mTokenizer.currToken(), "ARG", type);
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
                        return;
                    }
                    else
                    {
                        parseSymbol();  // ,
                    }
                }
            }
        }

        public int parseVarDec()
        {
            int count = 0;
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
            string kind = mTokenizer.currToken();
            while(mTokenizer.hasMoreTokens())
            {
                parseIdentifiter();
                string name = mTokenizer.currToken();
                symbolTable.define(name, "VAR", kind);
                ++count;

                if(!mTokenizer.hasMoreTokens())
                {
                    wrongMessage("Need more tokens");
                }
                parseSymbol();      // , or ;
                if (mTokenizer.currToken() == ";")
                    break;
            }
            return count;
        }

        public void parseStatements()
        {
            while (parseStatement()) ;
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
            parseKeyWord();     // field;
            string type = mTokenizer.currToken();
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
            string kind = mTokenizer.currToken();
            parseIdentifiter(); // name
            string name = mTokenizer.currToken();
            symbolTable.define(name, type, kind);
            parseSymbol();      // symbol
            while (mTokenizer.currToken() == ",")
            {
                parseIdentifiter();
                name = mTokenizer.currToken();
                symbolTable.define(name, type, kind);
                parseSymbol();
            }
        }

        public void parseExpression()
        {
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
                string opSymbol = mTokenizer.currToken();
                parseTerm();
                writeArithmetic(opMap[opSymbol]);
                if (mTokenizer.hasMoreTokens())
                {
                    mTokenizer.advance();
                }
                tokenType = mTokenizer.tokenType();
                token = mTokenizer.currToken();
                mTokenizer.backward();
            }
        }

        public void parseLetStatement()
        {
            parseKeyWord();         // let
            parseIdentifiter();     // varName
            bool isArray = false;
            string varName = mTokenizer.currToken();
            if (mTokenizer.hasMoreTokens())
            {
                mTokenizer.advance();
            }
            string token = mTokenizer.currToken();
            string tokenType = mTokenizer.tokenType();
            mTokenizer.backward();
            if (token == "[")
            {
                writePush(segmentMap[symbolTable.typeOf(varName)], symbolTable.indexOf(varName));
                parseSymbol();      // [
                parseExpression();  // expression
                parseSymbol();      // ]
                writeArithmetic("add");
                isArray = true;
            }
            parseSymbol();      // =
            parseExpression();  // expression
            string typeOfVar = symbolTable.typeOf(varName);
            if (isArray == false)
            {
                writePop(segmentMap[typeOfVar], symbolTable.indexOf(varName));
            }
            else if(isArray == true)
            {
                writePop("temp", 1);
                writePop("pointer", 1);
                writePush("temp", 1);
                writePop("that", 0);
            }
            parseSymbol();      // ;
        }

        public void parseIfStatement()
        {
            string falseLabel = "label" + labelIndex;
            ++labelIndex;
            string endLabel = "label" + labelIndex;
            ++labelIndex;
            parseKeyWord();     // if
            parseSymbol();      // (
            parseExpression();  // expression
            parseSymbol();      // )
            writeArithmetic("not");
            writeIf(falseLabel);
            parseSymbol();      // {
            parseStatements();  // statements
            parseSymbol();      // }
            writeGoto(endLabel);
            writeLabel1(falseLabel);
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
            writeLabel1(endLabel);
        }

        public void parseWhileStatement()
        {
            string whileLabel = "label" + labelIndex;
            ++labelIndex;
            string falseLabel = "label" + labelIndex;
            ++labelIndex;
            writeLabel1(whileLabel);
            parseKeyWord(); // while
            parseSymbol();  // (
            parseExpression(); // expression
            parseSymbol();  // )
            writeArithmetic("not");
            writeIf(falseLabel);
            parseSymbol();  // {
            parseStatements();  // statements
            parseSymbol();  // }
            writeGoto(whileLabel);
            writeLabel1(falseLabel);
        }

        public void parseDoStatement()
        {
            parseKeyWord();     // do
            parseSubroutineCall();
            parseSymbol();      // ;
            writePop("temp", 0);
        }

        public void parseReturn()
        {
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
            else
            {
                writePush("constant", 0);
            }
            parseSymbol();      // ;
            writeReturn();
        }

        public void parseSubroutineCall()
        {
            parseIdentifiter(); // var or className
            string functionName = mTokenizer.currToken();
            parseSymbol();      // ( or .
            string token = mTokenizer.currToken();
            if ( token == "(" )
            {
                writePush("pointer", 0);
                int paraCount = parseExpressionList();
                parseSymbol();  // )
                writeCall(className + "." + functionName, paraCount + 1);
            }
            else if ( token == "." )
            {
                parseIdentifiter(); // class subprocess
                if (symbolTable.contains(functionName))
                {
                    writePush(segmentMap[symbolTable.typeOf(functionName)], symbolTable.indexOf(functionName));
                }
                string funcName = mTokenizer.currToken();
                parseSymbol();      // (
                int paraCount = parseExpressionList();  // expressionList
                parseSymbol();      // )
                if (symbolTable.contains(functionName))
                {
                    writeCall(symbolTable.kindOf(functionName) + "." + funcName, paraCount + 1);
                }
                else
                {
                    writeCall(functionName + "." + funcName, paraCount);
                }
            }
        }

        public int parseExpressionList()
        {
            int count = 0;
            if (mTokenizer.hasMoreTokens())
            {
                mTokenizer.advance();
            }
            string token = mTokenizer.currToken();
            mTokenizer.backward();
            if (token == ")")
            {
                return 0;
            }
            parseExpression();
            ++count;

            if (mTokenizer.hasMoreTokens())
            {
                mTokenizer.advance();
            }
            else
            {
                wrongMessage("Need more token");
            }

            token = mTokenizer.currToken();
            string tokenType = mTokenizer.tokenType();
            mTokenizer.backward();
            while (tokenType == "symbol" && token == ",")
            {
                ++count;
                parseSymbol();
                parseExpression();
                if (mTokenizer.hasMoreTokens())
                {
                    mTokenizer.advance();
                }
                else
                {
                    wrongMessage("Need more token");
                }
                token = mTokenizer.currToken();
                tokenType = mTokenizer.tokenType();
                mTokenizer.backward();
            }
            return count;
        }

        public void parseTerm()
        {
            if ( mTokenizer.hasMoreTokens())
            {
                mTokenizer.advance();
            }
            string tokenType = mTokenizer.tokenType();
            string token = mTokenizer.currToken();
            if (tokenType == "integerConstant")
            {
                writePush("constant", Convert.ToInt32(token));
            }
            else if (tokenType == "stringConstant")
            {
                token = token.Trim('\"');
                writePush("constant", token.Count());
                writeCall("String.new", 1);
                foreach (char k in token)
                {
                    writePush("constant", Convert.ToInt32(k));
                    writeCall("String.appendChar", 2);
                }
            }
            else if (tokenType == "keyword" && (token == "true" || token == "false" || token == "null" || token == "this"))
            {
                if (token == "true")
                {
                    writePush("constant", 1);
                    writeArithmetic("neg");
                }
                else if(token == "false")
                {
                    writePush("constant", 0);
                }
                else if(token == "null")
                {
                    writePush("constant", 0);
                }
                else
                {
                    writePush("pointer", 0);
                }
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
                        writePush(segmentMap[symbolTable.typeOf(identify)], symbolTable.indexOf(identify));
                        parseSymbol();          // [
                        parseExpression();
                        parseSymbol();
                        writeArithmetic("add");
                        writePop("pointer", 1);
                        writePush("that", 0);
                    }
                    else
                    {
                        mTokenizer.backward();
                        parseSubroutineCall();
                    }

                }
                else
                {
                    writePush(segmentMap[symbolTable.typeOf(identify)], symbolTable.indexOf(identify));
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
                    if (token == "-")
                        writeArithmetic("neg");
                    else
                 
                        writeArithmetic("not");
                }
            }
        }

        public void parseKeyWord()
        {
            if (mTokenizer.hasMoreTokens())
            {
                mTokenizer.advance();
                if (mTokenizer.tokenType() == "keyword")     // class
                {
                    //writeKeyWord();
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
                    //writeIdentifiter();
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
                    //writeSymbol();
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

        private void writePush(string segement, int index)
        {
            writeIndent(string.Format("push {0} {1}", segement.ToLower(), index));
        }
        
        private void writePop(string segement, int index)
        {
            writeIndent(string.Format("pop {0} {1}", segement.ToLower(), index));
        }

        private void writeArithmetic(string command)
        {
            if (command == "div")
            {
                writeIndent("call Math.divide 2");
            }
            else if(command == "mul")
            {
                writeIndent("call Math.multiply 2");
            }
            else
            {
                writeIndent(command.ToLower());
            }
        }

        private void writeLabel1(string label)
        {
            writeIndent("label " + label.ToLower());
        }

        private void writeGoto(string label)
        {
            writeIndent(string.Format("goto {0}", label.ToLower()));
        }

        private void writeIf(string label)
        {
            writeIndent(string.Format("if-goto {0}", label.ToLower()));
        }

        private void writeCall(string name, int nArgs)
        {
            writeIndent(string.Format("call {0} {1}", name, nArgs));
        }

        private void writeFunction(string name, int nArgs)
        {
            writeIndent(string.Format("function {0}.{1} {2}", outputFileName.Remove(outputFileName.IndexOf(".")), name, nArgs));
        }

        private void writeReturn()
        {
            writeIndent("return");
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

        private SymbolTable symbolTable = new SymbolTable();
        private string className;
        private string currFunctionName;
        private string outputFileName;
        private int labelIndex = 0;
        private Tokenizer mTokenizer;
        private StreamWriter swriter;
        private Dictionary<string, string> segmentMap = new Dictionary<string, string>()
        {
            { "VAR" , "local" },
            { "ARG" , "argument" },
            { "FIELD", "this" },
            { "STATIC", "static" }
        };
        private Dictionary<string, string> opMap = new Dictionary<string, string>()
        {
            { "<" , "lt"},
            { ">" , "gt"},
            { "=" , "eq"},
            { "~" , "neq"},
            { "+" , "add"},
            { "-" , "sub"},
            { "*" , "mul"},
            { "/" , "div" },
            { "&" , "and" },
            { "|" , "or" }

        };

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
