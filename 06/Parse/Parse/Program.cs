using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            }
        }


    }
}
