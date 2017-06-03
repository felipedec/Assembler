using System;


namespace Assembler
{
    using static AssemblerCore;

    [Mnemonic("add", "sub", "mult", "div")]
    public class ArithmeticHandler : MnemonicHandler
    {
        #region Methods

        public override ArgumentsPattern[] GetArgumentsPatterns()
        {
            return new ArgumentsPattern[]
            {
                CreateArgumentsPattern( new String[] { "R(?<FirstRegister>[0-9]+)", "R(?<SecondRegister>[0-9]+)" }, () => { WriteRegisterRegister("FirstRegister", "SecondRegister", 0); }),
                CreateArgumentsPattern( new String[] { "R(?<FirstRegister>[0-9]+)", "@R(?<SecondRegister>[0-9]+)" }, () => { WriteRegisterRegister("FirstRegister", "SecondRegister", 1); }),
                CreateArgumentsPattern( new String[] { "R(?<Register>[0-9]+)", "#(?<Decimal>[0-9]+)" }, () => { WriteRegisterConstantValue("Register", "Decimal", 2, 10); }),
                CreateArgumentsPattern( new String[] { "R(?<Register>[0-9]+)", "(?<Binary>[01]+)" }, () => { WriteRegisterConstantValue("Register", "Binary", 4, 2); }),
            };
        }

        private static void WriteRegisterConstantValue(String Register, String Constant, Int32 Offset, Int32 Base)
        {
            Int32 ConstantValue = GetIntArgument(Constant, Base);
            Boolean bUseDoubleWord = ConstantValue > kMaxArgumentValue;
            Int32 InstructionAddress = GetBaseIntructionAddress() + Offset + (bUseDoubleWord ? 1 : 0);

            Write(InstructionAddress, kInstructionAddressBitsLength);
            Write(GetIntArgument(Register) * kRegisterBitsLength, kArgumentBitsLength);

            if (bUseDoubleWord)
            {
                Write(kMaxArgumentValue, kArgumentBitsLength);
                Write(ConstantValue, kWordBitsLength);
            }
            else
            {
                Write(ConstantValue, kArgumentBitsLength);
            }
        }

        private static void WriteRegisterRegister(String FirstRegister, String SecondRegister, Int32 Offset)
        {
            Write(GetBaseIntructionAddress() + Offset, kInstructionAddressBitsLength);
            Write(GetIntArgument(FirstRegister) * kRegisterBitsLength, kArgumentBitsLength);
            Write(GetIntArgument(SecondRegister) * kRegisterBitsLength, kArgumentBitsLength);
        }

        private static Int32 GetBaseIntructionAddress()
        {
            switch(AssemblyEvent.Current.Mnemonic[0])
            {
                case 'a': return 0;
                case 's': return 6;
                case 'm': return 12;
            }
            return 18;
        }

        #endregion Methods
    }
}