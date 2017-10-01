using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Vm
{
    class Parse
    {
        public Parse(string filename)
        {
            string[] strs = File.ReadAllLines(filename);

            foreach(var str in strs)
            {
                string temp = Regex.Replace(str, @"//.*", string.Empty);
                
                if(temp == string.Empty)
                {
                    continue;
                }

                _codeList.Add(temp.Trim());
            }
        }

        public bool hasMoreCommand()
        {
            return _codeList.Count() != 0 && _currenLine != _codeList.Count();
        }

        public void advance()
        {
            ++_currenLine;
        }

        public CodeType commandType()
        {
            if(!hasMoreCommand())
            {
                Console.WriteLine("Don't have more lines");
                Environment.Exit(-1);
            }

            var code = _codeList[_currenLine];
            string command = code.Split(' ')[0];
            if(codeTypeDict.ContainsKey(command))
            {
                return codeTypeDict[command];
            }
            else
            {
                return CodeType.C_WRONGTYPE;
            }
        }

        public string arg1()
        {
            var type = commandType();
            if(type == CodeType.C_RETURN)
            {
                Console.WriteLine("Can't be C_RETURN TYPE");
                Environment.Exit(-1);
            }

            string code = _codeList[_currenLine];
            string[] strs = code.Split(' ');

            if(type == CodeType.C_ARITHMETIC)
            {
                return strs[0];
            }
            else
            {
                return strs[1];
            }
        }

        public string arg2()
        {
            var type = commandType();
            if(!args2Type.Contains(type))
            {
                Console.WriteLine("Can't be another type");
                Environment.Exit(-1);
            }

            var str = _codeList[_currenLine];
            return str.Split(' ')[2];
        }

        public enum CodeType
        {
            C_ARITHMETIC,
            C_PUSH,
            C_POP,
            C_LABEL,
            C_GOTO,
            C_IF,
            C_FUNCTION,
            C_RETURN,
            C_CALL,
            C_WRONGTYPE
        };

        public Dictionary<string, CodeType> codeTypeDict = new Dictionary<string, CodeType>
        {
            {"add",         CodeType.C_ARITHMETIC },
            {"sub",         CodeType.C_ARITHMETIC },
            {"neg",         CodeType.C_ARITHMETIC },
            {"eq",          CodeType.C_ARITHMETIC },
            {"gt",          CodeType.C_ARITHMETIC },
            {"lt",          CodeType.C_ARITHMETIC },
            {"and",         CodeType.C_ARITHMETIC },
            {"or",          CodeType.C_ARITHMETIC },
            {"not",         CodeType.C_ARITHMETIC },
            {"label",       CodeType.C_LABEL },
            {"goto",        CodeType.C_GOTO },
            {"if-goto",     CodeType.C_GOTO },
            {"return",      CodeType.C_RETURN },
            {"call",        CodeType.C_CALL },
            {"function",    CodeType.C_FUNCTION },
            {"push",        CodeType.C_PUSH },
            {"pop",         CodeType.C_POP}
        };

        public List<CodeType> args2Type = new List<CodeType>
        {
            CodeType.C_POP,
            CodeType.C_PUSH,
            CodeType.C_FUNCTION,
            CodeType.C_CALL
        };

        private List<string> _codeList = new List<string>();
        int _currenLine;
    }
}
