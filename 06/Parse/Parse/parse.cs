using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Parse
{
    class parse
    {
        public parse(string filename)
        {
            var strs = File.ReadLines(filename);

            foreach(string str in strs)
            {
                string handledStr = str.Trim().Replace("", string.Empty);
                handledStr = Regex.Replace(handledStr, "//.*", string.Empty);
                if (handledStr.Length == 0)   // empty str
                {
                    continue;
                }

                _codeList.Add(handledStr);

                if(handledStr.StartsWith("("))
                {
                    var symbol = Regex.Match(handledStr, @"\((.*)\)").Groups[1].Value;
                    if(!_symbol_dict.ContainsKey(symbol))
                    {
                        _symbol_dict.Add(symbol, _currLine);
                    }
                }
                else if(handledStr.StartsWith("@"))
                {
                    
                }
            }
        }

        public void advance()
        {
           
        }

        public bool hasMoreLine()
        {
            string line;
            while((line = freader.ReadLine()) != null)
            {
                if(line.Trim() != "")
                {
                    break;
                }
            }

            code = line == null ? null : line.Trim();
            return code != null ? true : false;
        }

        public CodeType commandType()
        {
            if(code == null)
            {
                Console.WriteLine("code is null");
                Environment.Exit(-1);
            }

            char c = code[0];
            if(c == '@')
            {
                return CodeType.A_COMMAND;
            }
            else if(c != '(')
            {
                return CodeType.L_COMMAND;
            }
            else
            {
                return CodeType.C_COMMAND;
            }
        }

        public string symbol()
        {
            if(commandType() == CodeType.C_COMMAND)
            {
                Console.WriteLine("This is a C_COMMAND, you can't call symbol");
            }


            if(commandType() == CodeType.A_COMMAND)
            {
                string str = code.Substring(1, code.Length - 1); // fixme
                int num = Convert.ToInt32(str);
                return Convert.ToString(num, 2);
            }
            else
            {
                string str = code.Substring(1, code.Length - 1);
                return str;
            }
        }

        public string dest()
        {
            if(commandType() != CodeType.C_COMMAND)
            {
                Console.WriteLine("Call dest function : wrong type");
                Environment.Exit(-1);
            }

            int index = code.IndexOf('=');
            if(index == -1)
            {
                return "";
            }
            string str = code.Substring(0, index);
            return str.Trim();
        }

        public string comp()
        {
            int equal_index = code.IndexOf('=');
            int sem_index = code.IndexOf(';');
            string str = code;

            if(equal_index != -1)
            {
                str = str.Remove(0, equal_index + 1);
            }

            if(sem_index != -1)
            {
                str = str.Remove(sem_index, str.Length - sem_index + 1);
            }

            return str.Trim();
        }

        public string jump()
        {
            int equal_index = code.IndexOf('=');
            int sem_index = code.IndexOf(';');
            string str = code;

            if(equal_index != -1)
            {
                str = str.Remove(0, equal_index + 1);
            }

            if(sem_index != -1)
            {
                str = str.Remove(0, sem_index + 1);
            }

            return str;
        }

        public enum CodeType
        {
            A_COMMAND,
            C_COMMAND,
            L_COMMAND
        };

        private readonly List<string> _codeList = new List<string>();
        private Dictionary<string, int> _symbol_dict = new Dictionary<string, int>
        {
            {"SP" ,     0},
            {"LCL",     1},
            {"ARG",     2},
            {"THIS",    3},
            {"THAT",    4},
            {"R0",      0},
            {"R1",      1},
            {"R2",      2},
            {"R3",      3},
            {"R4",      4},
            {"R5",      5},
            {"R6",      6},
            {"R7",      7},
            {"R8",      8},
            {"R9",      9},
            {"R10",     10},
            {"R11",     11},
            {"R12",     12},
            {"R13",     13},
            {"R14",     14},
            {"R15",     15},
            {"SCREEN", 16384},
            {"KBD", 24576 }
        };
        private int _currLine = 0; 
    }
}
