using System;

namespace Assembler
{
    using static AssemblerCore;
    using static AssemblyEvent;

    [Mnemonic("desq", "ddir")]
    public sealed class ShiftHandler : MnemonicHandler
    {
        public override ArgumentsPattern[] GetArgumentsPatterns()
        {
            return new ArgumentsPattern[]
            {
                CreateArgumentsPattern(new String[] { "R(?<Register>[0-9]+)" }, () =>
                {
                    Write(Current.Mnemonic.Equals("desq") ? 24 : 25, kInstructionAddressBitsLength);
                    Write(GetIntArgument("Register") * kRegisterBitsLength, kArgumentBitsLength);
                    Write(0xFF, kArgumentBitsLength);
                })
            };
        }
    }
}