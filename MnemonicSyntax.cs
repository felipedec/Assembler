using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Assembler
{
    public delegate void AssembleDelegate(MnemonicAssembleInfo Args);


    public struct MnemonicSyntax
    {
        public Regex[] ParametersRegExpressions { private set; get; }
        public AssembleDelegate Assemble { private set; get; }

        public int ParametersCount
        {
            get
            {
                return ParametersRegExpressions.Length;
            }
        }

        public MnemonicSyntax(Regex[] InParametersRegExpressions, AssembleDelegate InAssemble)
        {
            ParametersRegExpressions = InParametersRegExpressions;
            Assemble = InAssemble;
        }

        public void AssembleInstruction(StreamWriter StreamWriter,
            StreamReader StreamReader,
            Match[] Matches)
        {
            Assemble(new MnemonicAssembleInfo(this, Matches, StreamWriter, StreamReader));
        }

        public bool Match(string[] Args, out Match[] OutMatches)
        {
            OutMatches = null;

            if (Args.Length != ParametersCount)
                return false;

            var TempMatches = new Match[Args.Length];

            for (int j = 0; j < Args.Length; j++)
            {
                if (string.IsNullOrEmpty(Args[j]))
                    continue;

                TempMatches[j] = ParametersRegExpressions[j].Match(Args[j]);

                if (!TempMatches[j].Success)
                {
                    return false;
                }
            }

            OutMatches = TempMatches;
            return true;
        }
    }
}