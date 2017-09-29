using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Parse
{
    class Parse
    {
        public Parse(string filename)
        {
            var strs = File.ReadLines(filename);

            foreach(string str in strs)
            {
                string handledStr = str.Trim().Replace(" ", string.Empty);
                handledStr = Regex.Replace(handledStr, "//.*", string.Empty);
                if (handledStr.Length == 0)   // empty str
                {
                    continue;
                }

                if(handledStr.StartsWith("("))
                {
                    var symbol = Regex.Match(handledStr, @"\((.*)\)").Groups[1].Value;
                    if(!_symbol_dict.ContainsKey(symbol))
                    {
                        _symbol_dict.Add(symbol, _currLine);
                        continue;
                    }
                }
                _codeList.Add(handledStr);
                ++_currLine;
            }
            _currLine = 0;
        }

        public void advance()
        {
            if(hasMoreLine())
                ++_currLine;
        }

        public bool hasMoreLine()
        {
            return _codeList.Count() != 0 && _currLine != _codeList.Count() - 1;
        }

        public CodeType commandType()
        {
            if(!hasMoreLine())
            {
                Console.WriteLine("code is null");
                Environment.Exit(-1);
            }

            string code = _codeList[_currLine];

            if(code.StartsWith("@"))
            {
                return CodeType.A_COMMAND;
            }
            if(code.StartsWith("("))
            {
                return CodeType.L_COMMAND;
            }

            return CodeType.C_COMMAND;
        }

        public string symbol()
        {
            if(commandType() == CodeType.C_COMMAND)
            {
                Console.WriteLine("This is a C_COMMAND, you can't call symbol");
                Environment.Exit(-1);
            }

            string code = _codeList[_currLine];
            if (commandType() == CodeType.A_COMMAND)
            {
                var symbol = Regex.Match(code, @"@(.*)").Groups[1].Value;

                if(_symbol_dict.ContainsKey(symbol))
                {
                    return _symbol_dict[symbol].ToString();
                }
                else
                {
                    return symbol;
                }
            }
            else
            {
                var symbol = Regex.Match(code, @"\((.*)\)").Groups[1].Value;
                return _symbol_dict[symbol].ToString();
            }
        }

        public string dest()
        {
            if(commandType() != CodeType.C_COMMAND)
            {
                Console.WriteLine("Call dest function : wrong type");
                Environment.Exit(-1);
            }

            string code = _codeList[_currLine];
            int index = code.IndexOf('=');
            if(index == -1)
            {
                return string.Empty;
            }
            return Regex.Match(code, @"(.*)=").Groups[1].Value;
        }

        public string comp()
        {
            if(commandType() != CodeType.C_COMMAND)
            {
                Console.WriteLine("Call comp function : wrong type");
                Environment.Exit(-1);
            }
            var code = _codeList[_currLine];
            code = Regex.Replace(code, "(.*)=", string.Empty);
            code = Regex.Replace(code, ";(.*)", string.Empty);

            return code;
        }

        public string jump()
        {
            if(commandType() != CodeType.C_COMMAND)
            {
                Console.WriteLine("Call jump function : wrong type");
                Environment.Exit(-1);

            }
            var code = _codeList[_currLine];
            if(!code.Contains(";"))
            {
                return string.Empty;
            }

            return Regex.Match(code, @";(.*)").Groups[1].Value;
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