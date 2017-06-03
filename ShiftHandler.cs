using System;

namespace Assembler
{
    using static AssemblerCore;

    [Mnemonic("desq"), Mnemonic("ddir")]
    public sealed class ShiftHandler : MnemonicHandler
    {
        public override ArgumentsPattern[] GetArgumentsPatterns()
        {
            return new ArgumentsPattern[]
            {
                CreateArgumentsPattern(new String[] { "R(?<Register>[0-9]+)" }, () =>
                {
                    Int32 RegisterIndex = GetIntArgument("Register");
                    Boolean bIsLeftShift = AssemblyEvent.Current.Mnemonic.Equals("desq");

                    Write(bIsLeftShift ? 24 : 25, kInstructionAddressBitsLength);
                    Write(RegisterIndex * kRegisterBitsLength, kArgumentBitsLength);
                    Write(0xFFFFFF, kArgumentBitsLength);
                })
            };
        }
    }
}