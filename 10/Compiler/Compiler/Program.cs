using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net;


namespace Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: program filename");
                Environment.Exit(-1);
            }
            Tokenizer tokenizer = new Tokenizer(args[0]);
            Parse parse = new Parse(tokenizer, args[0].Replace(".jack", "T.xml"));
            parse.start();
            parse.close();
        }
    }
}
