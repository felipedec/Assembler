using System.IO;
using System.Text.RegularExpressions;

namespace Assembler
{
    public struct MnemonicSyntax
    {
        public delegate void AssembleDelegate(Match[] matches, StreamWriter streamWriter, VirtualMachineSetup vmSetup);

        public Regex[] ParametersRegEx;
        public AssembleDelegate Assemble;
    }
}