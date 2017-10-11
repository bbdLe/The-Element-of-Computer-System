using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length < 1)
            {
                Console.WriteLine("Usage: program filename");
                Environment.Exit(-1);
            }
            Console.WriteLine(args);
            Tokenizer tokenizer = new Tokenizer(args[0]);
            while(tokenizer.hasMoreTokens())
            {
                var tokenType = tokenizer.tokenType();
                Console.WriteLine(tokenType);
                tokenizer.advance();
            }
        }
    }
}
