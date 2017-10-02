using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vm
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length != 1)
            {
                Console.WriteLine("Usage: *.exe .vm");
                Environment.Exit(-1);
            }

            List<string> fileList = new List<string>();
            string fileName = args[0];
            string output = "";
            if(fileName.EndsWith(".vm"))
            {
                output = fileName.Replace(".vm", ".asm");
            }
            else
            {
                var dirName = new DirectoryInfo(fileName).Name;
                output = fileName + @"\" + dirName + ".asm";
                fileList.AddRange(Directory.GetFiles(fileName, "*.vm"));
            }
            codeWriter writer = new codeWriter(output);
            writer.writeBootLoader();
            foreach (var name in fileList)
            {
                Parse parse = new Parse(name);
                writer.changeParse(parse);
                while (parse.hasMoreCommand())
                {
                    Parse.CodeType type = parse.commandType();

                    if (type == Parse.CodeType.C_ARITHMETIC)
                    {
                        writer.writeArithmetic();
                    }
                    else if (type == Parse.CodeType.C_PUSH || type == Parse.CodeType.C_POP)
                    {
                        writer.writePushPop(type, parse.arg1(), Convert.ToInt32(parse.arg2()));
                    }
                    else if (type == Parse.CodeType.C_LABEL)
                    {
                        writer.writeLabel();
                    }
                    else if (type == Parse.CodeType.C_FUNCTION)
                    {
                        writer.writeFunction();
                    }
                    else if (type == Parse.CodeType.C_GOTO)
                    {
                        writer.writeGoto();
                    }
                    else if (type == Parse.CodeType.C_IF)
                    {
                        writer.writeIf();
                    }
                    else if (type == Parse.CodeType.C_RETURN)
                    {
                        writer.writeReturn();
                    }
                    else if (type == Parse.CodeType.C_CALL)
                    {
                        writer.writeCall();
                    }
                    else
                    {
                        Console.WriteLine("No Finish");
                        Environment.Exit(-1);
                    }

                    parse.advance();
                }
            }
            writer.close();
        }
    }
}
