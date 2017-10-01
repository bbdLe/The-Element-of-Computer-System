using System;
using System.Collections.Generic;
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

            string fileName = args[0];
            string output = fileName.Replace(".vm", ".asm");
            Parse parse = new Parse(fileName);
            codeWriter writer = new codeWriter(output, parse);
            while(parse.hasMoreCommand())
            {
                Parse.CodeType type = parse.commandType();

                if(type == Parse.CodeType.C_ARITHMETIC)
                {
                    writer.writeArithmetic();
                }
                else if(type == Parse.CodeType.C_PUSH || type == Parse.CodeType.C_POP)
                {
                    writer.writePushPop(type, parse.arg1(), Convert.ToInt32(parse.arg2()));
                }
                else
                {
                    Console.WriteLine("No Finish");
                    Environment.Exit(-1);
                }

                parse.advance();
            }
            writer.close();
        }
    }
}
