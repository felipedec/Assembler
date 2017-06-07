using System;

namespace Assembler
{
    using static AssemblerCore;

    [Mnemonic("call")]
    public sealed class CallHandler : MnemonicHandler
    {
        public override ArgumentsPattern[] GetArgumentsPatterns()
        {
            return new ArgumentsPattern[]
            {
                CreateArgumentsPattern(new String[] { "(?<Label>[a-z0-9_]+)" }, () => { Goto(GetArgument("Label")); })
            };
        }
    }
}