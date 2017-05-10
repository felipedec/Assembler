using System.Text.RegularExpressions;
using System;
using System.IO;

namespace Assembler
{
    public abstract class MnemonicHandler
    {
        private Regex[][] m_ParametersRegex;
        private MnemonicSyntax[] SupportedSyntax;

        public static MnemonicSyntax CreateSyntax(MnemonicSyntax.AssembleDelegate assemble, params string[] parametersRegExpression)
        {
            var parametersRegEx = new Regex[parametersRegExpression.Length];
            for(int i = 0; i < parametersRegExpression.Length; i++)
            {
                parametersRegEx[i] = new Regex(parametersRegExpression[i], Assembler.kRegexOption);
            }

            return new MnemonicSyntax()
            {
                Assemble = assemble,
                ParametersRegEx = parametersRegEx
            };
        }

        public int GetParametersMatchedIndex(string args, out Match[] matches)
        {
            string[] argsArray = args.Split(new string[]{ " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);

            for(int i = 0; i < m_ParametersRegex.Length; i++)
            {
                if (m_ParametersRegex[i] == null && argsArray.Length > 0
                    ||m_ParametersRegex[i].Length != argsArray.Length)
                    continue;

                var tmpMatches = new Match[argsArray.Length];
                bool flag = false;

                for(int j = 0; j < argsArray.Length; j++)
                {
                    if (string.IsNullOrEmpty(argsArray[j]))
                        continue;

                    tmpMatches[j] = m_ParametersRegex[i][j].Match(argsArray[j]);

                    if(!tmpMatches[j].Success)
                    {
                        flag = true;
                        continue;
                    }
                }

                if (flag)
                    continue;

                matches = tmpMatches;
                return i;
            }
            matches = null;
            return -1;
        }

        public abstract Regex[][] GetParametersSupported();

        public abstract void AssembleMachineCode(int parametersDeclarationIndex,
                                               Match[] argumentsMatches,
                                               StreamWriter streamWriter,
                                               VirtualMachineSetup virtualMachineSetup);


        protected static Regex[] GetParametersRegex(params string[] patterns)
        {
            var result = new Regex[patterns.Length];

            for (int i = 0; i < patterns.Length; i++)
            {
                result[i] = new Regex(patterns[i], Assembler.kRegexOption);
            }
            return result;
        }

        public MnemonicHandler()
        {
            this.m_ParametersRegex = GetParametersSupported();
        }
    }
}