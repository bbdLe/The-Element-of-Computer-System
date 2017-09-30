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
        public codeWriter(string fileName, Parse pParse)
        {
            parse = pParse;
            FileStream fstream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter swriter = new StreamWriter(fstream);
        }

        ~codeWriter()
        {
            fstream.Close();
        }

        public void writeArithmetic(string code)
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
            else if(func == "eq")
            {
                eqFunc();
            }
        }

        private void eqFunc()
        {

        }

        private void negFunc()
        {
            string str = "";
            str += "D=-D" + newLine;
            popToD();
            swriter.Write(str);
            DtoStack();
            incSP();
        }

        private void subFunc()
        {
            string str = "";
            str += "D=A-D" + newLine;
            helpFunc(str);
        }

        private void addFunc()
        {
            string str = "";
            str += "D=A+D" + newLine;
            helpFunc(str);
        }

        private void helpFunc(string str)
        {
            popToD();
            popToA();
            swriter.Write(str);
            DtoStack();
            incSP();
        }

        private void DtoStack()
        {
            string str = "";
            str += "@SP" + newLine;
            str += "A=M" + newLine;
            str += "M=D" + newLine;
            swriter.Write(str);
        }

        private void incSP()
        {
            string str = "";
            str += "@SP" + newLine;
            str += "M=M+1" + newLine;
            swriter.Write(str);
        }

        private void decSp()
        {
            string str = "";
            str += "@SP" + newLine;
            str += "M=M-1" + newLine;
            swriter.Write(str);
        }

        private void popToD()
        {
            popToA();
            string str = ""; 
            str += "D=A" + newLine;
            swriter.Write(str);
        }

        private void popToA()
        {
            string str = "";
            str += "@SP" + newLine;
            str += "M=M-1" + newLine;
            str += "@SP" + newLine;
            str += "A=M" + newLine;
            str += "A=M" + newLine;
            swriter.Write(str);
        }

        private FileStream fstream;
        private readonly string newLine = "\r\n";
        private StreamWriter swriter;
        private readonly Parse parse;
    }
}
