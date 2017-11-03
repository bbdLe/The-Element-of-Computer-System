using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class SymbolTable
    {

        struct varDec
        {
            public string name;
            public string type;
            public string kind;
            public int index;

            public varDec(string name, string type, string kind, int index) : this()
            {
                this.name = name;
                this.type = type;
                this.kind = kind;
                this.index = index;
            }
        };

        public void startSubroutine(string functionType)
        {
            localVarDict.Clear();
            if (functionType == "method")
                argsCount = 1;
            else
                argsCount = 0;
            varsCount = 0;
        }

        public void define(string name, string type, string kind)
        {
            type = type.ToUpper();
            Console.WriteLine(type + "|" + name);
            if(globalType.Contains(type))
            {
                globalVarDict.Add(name, new varDec(name, type, kind, varCount(type)));
                if (type == "STATIC")
                {
                    ++staticsCount;
                }
                else
                {
                    ++fieldsCount;
                }
            }
            else if(localType.Contains(type))
            {
                localVarDict.Add(name, new varDec(name, type, kind, varCount(type)));
                if (type == "ARG")
                {
                    ++argsCount;
                }
                else
                {
                    ++varsCount;
                }
            }
            else
            {
                Console.WriteLine("don't match----");
                Environment.Exit(-1);
            }
        }

        public bool contains(string name)
        {
            if (globalVarDict.ContainsKey(name) || localVarDict.ContainsKey(name))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int varCount(string type)
        {
            if (type == "STATIC")
            {
                return staticsCount;
            }
            else if(type == "FIELD")
            {
                return fieldsCount;
            }
            else if(type == "ARG")
            {
                return argsCount;
            }
            else if(type == "VAR")
            {
                return varsCount;
            }
            else
            {
                errorMessage("Don't match");
                return -1;
            }
        }

        public string kindOf(string name)
        {
            if(localVarDict.ContainsKey(name))
            {
                return localVarDict[name].kind;
            }
            else if(globalVarDict.ContainsKey(name))
            {
                return globalVarDict[name].kind;
            }
            else
            {
                errorMessage("Don't contains this key");
                return "";
            }
        }

        public string typeOf(string name)
        {
            if (localVarDict.ContainsKey(name))
            {
                return localVarDict[name].type;
            }
            else if (globalVarDict.ContainsKey(name))
            {
                return globalVarDict[name].type;
            }
            else
            {
                errorMessage("Don't contains this key");
                return "";
            }
        }

        public int indexOf(string name)
        {
            if (localVarDict.ContainsKey(name))
            {
                return localVarDict[name].index;
            }
            else if (globalVarDict.ContainsKey(name))
            {
                return globalVarDict[name].index;
            }
            else
            {
                errorMessage("Don't contains this key");
                return -1;
            }
        }



        private void errorMessage(string msg)
        {
            Console.WriteLine(msg);
            Environment.Exit(-1);
        }

        private List<string> globalType = new List<string>(){
            "STATIC",
            "FIELD",
        };
        private List<string> localType = new List<string>()
        {
            "ARG",
            "VAR",
        };


        private int staticsCount = 0;
        private int fieldsCount = 0;
        private int argsCount = 0;
        private int varsCount = 0;
        private Dictionary<string, varDec> globalVarDict = new Dictionary<string, varDec>();
        private Dictionary<string, varDec> localVarDict = new Dictionary<string, varDec>();
    }


}
