using System.Text.RegularExpressions;
using System;
using System.IO;

namespace Assembler
{
    public abstract class MnemonicHandler
    {
        private MnemonicSyntax[] SupportedSyntaxes;

        public static MnemonicSyntax CreateSyntax(string[] ParametersRegExpression, AssembleDelegate AssembleDelegate)
        {
            return new MnemonicSyntax(GetParametersRegex(ParametersRegExpression), AssembleDelegate);
        }

        public bool TryGetMatchedSyntax(string Args, out Match[] OutMatches, out MnemonicSyntax? OutSyntax)
        {
            OutSyntax = null;
            OutMatches = null;

            string[] ArgsArray = Args.Split(new string[]{ " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);

            for (int Index = 0; Index < SupportedSyntaxes.Length; Index++)
            {
                if(SupportedSyntaxes[Index].Match(ArgsArray, out OutMatches))
                {
                    OutSyntax = SupportedSyntaxes[Index];
                    return true;
                }
            }

            return false;
        }

        public abstract MnemonicSyntax[] GetParametersSupported();


        protected static Regex[] GetParametersRegex(params string[] Patterns)
        {
            var Result = new Regex[Patterns.Length];

            for (int Index = 0; Index < Patterns.Length; Index++)
            {
                Result[Index] = new Regex("^" + Patterns[Index].ToLower() + "$", Assembler.kRegexOption);
            }
            return Result;
        }

        public MnemonicHandler()
        {
            SupportedSyntaxes = GetParametersSupported();
        }
    }
}