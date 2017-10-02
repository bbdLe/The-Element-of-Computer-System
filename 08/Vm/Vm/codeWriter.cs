using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vm
{
    class codeWriter
    {
        public codeWriter(string fileName)
        {
            fstream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write | FileAccess.Read);
            swriter = new StreamWriter(fstream);
        }

        public void close()
        {
            
            foreach (var str in codeList)
            {
                swriter.WriteLine(str);
            }
            swriter.Flush();
            swriter.Close();
            fstream.Close();
            Console.WriteLine("ok");
        }

        public void changeParse(Parse pParse)
        {
            parse = pParse;
        }

        public void writeArithmetic()
        {
            string func = parse.arg1();

            if(func == "add")
            {
                addFunc();
            }
            else if(func == "sub")
            {
                subFunc();
            }
            else if(func == "neg")
            {
                negFunc();
            }
            else if(func == "eq" || func == "gt" || func == "lt")
            {
                compareFunc(func);
            }
            else if(func == "and" || func == "or")
            {
                logicFunc(parse.arg1());
            }
            else if(func == "not")
            {
                notFunc();
            }
            else
            {
                Console.WriteLine("Wrong type");
                Environment.Exit(-1);
            }
        }

        public void writeBootLoader()
        {
            addCode("@256");
            addCode("D=A");
            addCode("@SP");
            addCode("M=D");
            List<string> strList = new List<string>
            {
                "@LCL",
                "@ARG",
                "@THIS",
                "@THAT"
            };

            addCode("@RETURNFLAG" + index.ToString());
            addCode("D=A");
            DtoStack();
            foreach (var str in strList)
            {
                addCode(str);
                addCode("D=M");
                DtoStack();
            }
            addCode("@SP");
            addCode("D=M");
            addCode("@5");
            addCode("D=D-A");
            addCode("@0");
            addCode("D=D-A");
            addCode("@ARG");
            addCode("M=D");
            addCode("@SP");
            addCode("D=M");
            addCode("@LCL");
            addCode("M=D");
            addCode("@Sys.init");
            addCode("D;JMP");
            addCode("(RETURNFLAG" + index.ToString() + ")");
            ++index;
        }

        public void writePushPop(Parse.CodeType codeType, string segment, int segmentIndex)
        {
            if(codeType == Parse.CodeType.C_PUSH)
            {
                if(segment == "constant")
                {
                    addCode("@" + segmentIndex.ToString());
                    addCode("D=A");
                    DtoStack();
                }
                else if(segment == "local")
                {
                    pushSegmentHelper("@LCL");
                }
                else if(segment == "argument")
                {
                    pushSegmentHelper("@ARG");
                }
                else if(segment == "this")
                {
                    pushSegmentHelper("@THIS");
                }
                else if(segment == "that")
                {
                    pushSegmentHelper("@THAT");
                }
                else if(segment == "static")
                {
                    pushStatic();
                }
                else if(segment == "temp")
                {
                    pushSegmentTemp();
                }
                else if(segment == "pointer")
                {
                    pushSegmentPointer();
                }
                else
                {
                    Console.WriteLine("No finish:push");
                    Environment.Exit(-1);
                }
            }
            else if(codeType == Parse.CodeType.C_POP)
            {
                if(segment == "constant")
                {
                    popToA();
                }
                else if(segment == "local")
                {
                    popSegmentHelper("@LCL");
                }
                else if (segment == "argument")
                {
                    popSegmentHelper("@ARG");
                }
                else if (segment == "this")
                {
                    popSegmentHelper("@THIS");
                }
                else if (segment == "that")
                {
                    popSegmentHelper("@THAT");
                }
                else if (segment == "static")
                {
                    popStatic();
                }
                else if (segment == "temp")
                {
                    popSegmentTemp();
                }
                else if (segment == "pointer")
                {
                    popSegmentPointer();
                }
                else
                {
                    Console.WriteLine("No finish:pop");
                    Environment.Exit(-1);
                }

            }
            else
            {
                Console.WriteLine("Wrong Type : writePushPop");
                Environment.Exit(-1);
            }
        }

        public void writeLabel()
        {
            if(!isInFunc)
            {
                addCode("(" + parse.arg1() + ")");
            }
            else
            {
                addCode("(" + FuncName + "$" + parse.arg1() + ")");
            }
        }

        public void writeGoto()
        {
            if(!isInFunc)
            {
                addCode("@" + parse.arg1());
            }
            else
            {
                addCode("@" + FuncName + "$" + parse.arg1());
            }
            addCode("D;JMP");
        }

        public void writeIf()
        {
            popToD();
            if(!isInFunc)
            {
                addCode("@" + parse.arg1());
            }
            else
            {
                addCode("@" + FuncName + "$" + parse.arg1());
            }
            addCode("D;JNE");
        }

        public void writeFunction()
        {
            isInFunc = true;
            FuncName = parse.arg1();
            int localnums = Convert.ToInt32(parse.arg2());
            addCode("(" + FuncName + ")");
            for(var i = 0; i < localnums; ++i)
            {
                addCode("D=0");
                DtoStack();
            }
        }

        public void writeReturn()
        {
            addCode("@LCL");            // save return address
            addCode("D=M");
            addCode("@5");
            addCode("A=D-A");
            addCode("D=M");
            addCode("@R13");
            addCode("M=D");
            popToD();

            addCode("@ARG");            // save return value
            addCode("A=M");
            addCode("M=D");
            addCode("@ARG");
            addCode("D=M");
            addCode("@SP");
            addCode("M=D+1");

            writeReturnHelper("THAT", 1);
            writeReturnHelper("THIS", 2);
            writeReturnHelper("ARG", 3);
            writeReturnHelper("LCL", 4);

            addCode("@R13");
            addCode("A=M");
            addCode("D;JMP");
        }

        public void writeCall()
        {
            List<string> strList = new List<string>
            {
                "@LCL",
                "@ARG",
                "@THIS",
                "@THAT"
            };

            addCode("@RETURNFLAG" + index.ToString());
            addCode("D=A");
            DtoStack();
            foreach(var str in strList)
            {
                addCode(str);
                addCode("D=M");
                DtoStack();
            }
            addCode("@SP");
            addCode("D=M");
            addCode("@5");
            addCode("D=D-A");
            addCode("@" + parse.arg2().ToString());
            addCode("D=D-A");
            addCode("@ARG");
            addCode("M=D");
            addCode("@SP");
            addCode("D=M");
            addCode("@LCL");
            addCode("M=D");
            addCode("@" + parse.arg1());
            addCode("D;JMP");
            addCode("(RETURNFLAG" + index.ToString() + ")");
            ++index;

        }

        private void writeReturnHelper(string segmentName, int distanceFromLCL)
        {
            addCode("@LCL");
            addCode("D=M");
            addCode("@" + distanceFromLCL.ToString());
            addCode("A=D-A");
            addCode("D=M");
            addCode("@" + segmentName);
            addCode("M=D");
        }

        private void notFunc()
        {
            popToD();
            addCode("D=!D");
            DtoStack();
        }

        private void pushSegmentTemp()
        {
            addCode("@R" + (Convert.ToInt32(parse.arg2()) + 5).ToString());
            addCode("D=M");
            DtoStack();
        }

        private void popSegmentTemp()
        {
            popToD();
            addCode("@R" + (Convert.ToInt32(parse.arg2()) + 5).ToString());
            addCode("M=D");
        }

        private void pushSegmentPointer()
        {
            if(parse.arg2() == "0")
            {
                addCode("@THIS");
            }
            else
            {
                addCode("@THAT");
            }
            addCode("D=M");
            DtoStack();
        }

        private void popSegmentPointer()
        {
            popToD();
            if (parse.arg2() == "0")
            {
                addCode("@THIS");
            }
            else
            {
                addCode("@THAT");
            }
            addCode("M=D");
        }

        private void popSegmentHelper(string code)
        {
            addCode(code);
            addCode("D=M");
            addCode("@" + parse.arg2());
            addCode("D=D+A");
            addCode("@R13");
            addCode("M=D");
            popToD();
            addCode("@R13");
            addCode("A=M");
            addCode("M=D");
        }

        private void pushSegmentHelper(string code)
        {
            addCode(code);
            addCode("D=M");
            addCode("@" + parse.arg2());
            addCode("A=D+A");
            addCode("D=M");
            DtoStack();
        }

        private void pushStatic()
        {
            addCode("@16");
            addCode("D=A");
            addCode("@" + parse.mFileName + parse.arg2());
            addCode("A=D+A");
            addCode("D=M");
            DtoStack();
        }

        private void popStatic()
        {
            addCode("@16");
            addCode("D=A");
            addCode("@" + parse.mFileName + parse.arg2());
            addCode("D=D+A");
            addCode("@R13");
            addCode("M=D");
            popToD();
            addCode("@R13");
            addCode("A=M");
            addCode("M=D");
        }

        private void logicFunc(string code)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                {"and", "D=D&A"},
                {"or",  "D=D|A"}
            };

            popToD();
            popToA();
            addCode(dict[code]);
            DtoStack();
        }

        private void compareFunc(string code)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                {"eq", "D;JEQ"},
                {"gt", "D;JGT"},
                {"lt", "D;JLT"}
            };

            string indexStr = index.ToString();
            string trueLabel = "TRUELABEL" + indexStr;
            string falseLabel = "FALSELABEL" + indexStr;
            string endLabel = "END" + indexStr;
            ++index;
            popToD();
            popToA();
            addCode("D=A-D");
            addCode("@" + trueLabel);
            addCode(dict[code]);
            addCode("@" + falseLabel);
            addCode("D;JMP");
            addCode("(" + trueLabel + ")");
            addCode("@1");
            addCode("D=-A");
            addCode("@SP");
            addCode("A=M");
            addCode("M=D");
            addCode("@" + endLabel);
            addCode("D;JMP");
            addCode("(" + falseLabel + ")");
            addCode("@SP");
            addCode("A=M");
            addCode("M=0");
            addCode("(" + endLabel + ")");
            incSP();
        }

        private void negFunc()
        {
            popToD();
            addCode("D=-D");
            DtoStack();
        }

        private void subFunc()
        {
            helpFunc("D=A-D");
        }

        private void addFunc()
        {
            helpFunc("D=A+D");
        }

        private void helpFunc(string str)
        {
            popToD();
            popToA();
            addCode(str);
            DtoStack();
        }

        private void DtoStack()
        {
            addCode("@SP");
            addCode("A=M");
            addCode("M=D");
            incSP();
        }

        private void incSP()
        {
            addCode("@SP");
            addCode("M=M+1");
        }

        private void decSp()
        {
            addCode("@SP");
            addCode("M=M-1");
        }

        private void popToD()
        {
            popToA();
            addCode("D=A");
        }

        private void popToA()
        {
            decSp();
            addCode("@SP");
            addCode("A=M");
            addCode("A=M");
        }

        private void addCode(string code)
        {
            codeList.Add(code);
        }

        private List<string> codeList = new List<string>();
        private StreamWriter swriter;
        private FileStream fstream;
        private Parse parse;
        private bool isInFunc = false;
        private string FuncName = "";
        private int index = 0;
    }
}
