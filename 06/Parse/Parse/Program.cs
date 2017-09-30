using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Parse
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length != 1)
            {
                Console.WriteLine("Program.exe asmfile");
                return;
            }

            string filename = args[0];
            List<string> resultCode = new List<string>();
            var parse = new Parse(filename);

            while(parse.hasMoreLine())
            {
                Parse.CodeType type = parse.commandType();
                if (type == Parse.CodeType.A_COMMAND)
                {
                    int result;
                    string symbol = parse.symbol();
                    if (Int32.TryParse(symbol, out result) == true)
                    {
                        resultCode.Add(padString(result, 16));
                    }
                    else
                    {
                        if (parse._symbol_dict.ContainsKey(symbol))
                        {
                        }
                        else
                        {
                            parse._symbol_dict[symbol] = parse.baseAddress + parse.varCount;
                            ++parse.varCount;
                        }
                        resultCode.Add(padString(parse._symbol_dict[symbol], 16));
                    }
                }
                else if (type == Parse.CodeType.C_COMMAND)
                {
                    resultCode.Add("111" + Code.comp(parse.comp()) + Code.dest(parse.dest()) + Code.jump(parse.jump()));
                }
                else
                {
                    Console.WriteLine("Something wrong");
                    Environment.Exit(-1);
                }
                parse.advance();
            }

            string outfile = filename.Replace(".asm", ".hack");
            File.WriteAllLines(outfile, resultCode);
        }

        static string padString(int num, int width)
        {
            return Convert.ToString(num, 2).PadLeft(width, '0');
        }
    }
}
