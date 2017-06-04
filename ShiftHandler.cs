using System;

namespace Assembler
{
    using static AssemblerCore;

    [Mnemonic("desq", "ddir")]
    public sealed class ShiftHandler : MnemonicHandler
    {
        public override ArgumentsPattern[] GetArgumentsPatterns()
        {
            return new ArgumentsPattern[]
            {
                CreateArgumentsPattern(new String[] { "R(?<Register>[0-9]+)" }, () =>
                {
                    Boolean bIsLeftShift = AssemblyEvent.Current.Mnemonic.Equals("desq");

                    Write(bIsLeftShift ? 24 : 25, kInstructionAddressBitsLength);
                    Write(GetIntArgument("Register") * kRegisterBitsLength, kArgumentBitsLength);
                    Write(0xFFFFFF, kArgumentBitsLength);
                })
            };
        }
    }
}