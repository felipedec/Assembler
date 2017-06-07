using System;

namespace Assembler
{
    using static AssemblerCore;

    [Mnemonic("jmp")]
    public sealed class JumpHandler : MnemonicHandler
    {
        public override ArgumentsPattern[] GetArgumentsPatterns()
        {
            return new ArgumentsPattern[]
            {
                CreateArgumentsPattern(new [] { "(?<LineNumber>[0-9]+)" }, () => { Jump(GetIntArgument("LineNumber") - 2); })
            };
        }
    }
}