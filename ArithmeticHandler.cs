using System;


namespace Assembler
{
    using static AssemblerCore;

    public abstract class ArithmeticHandler : MnemonicHandler
    {
        #region Properties

        protected abstract Int32 LeastInstructionAddress { get; }

        #endregion Properties

        #region Methods

        public override ArgumentsPattern[] GetArgumentsPatterns()
        {
            return new ArgumentsPattern[]
            {
                CreateArgumentsPattern( new String[] { "R(?<FirstRegister>[0-9]+)", "R(?<SecondRegister>[0-9]+)" },
                                        () => { WriteRegisterRegister("FirstRegister", "SecondRegister", 0); }),

                CreateArgumentsPattern( new String[] { "R(?<FirstRegister>[0-9]+)", "@R(?<SecondRegister>[0-9]+)" },
                                        () => { WriteRegisterRegister("FirstRegister", "SecondRegister", 1); }),

                CreateArgumentsPattern( new String[] { "R(?<Register>[0-9]+)", "#(?<Decimal>[0-9]+)" },
                                        () => { WriteRegisterConstantValue("Register", "Decimal", 2, 10); }),

                CreateArgumentsPattern( new String[] { "R(?<Register>[0-9]+)", "(?<Binary>[01]+)" },
                                        () => { WriteRegisterConstantValue("Register", "Binary", 4, 2); }),
            };
        }

        private void WriteRegisterConstantValue(String Register, String Constant, Int32 Offset, Int32 Base)
        {
            Int32 ConstantValue = GetIntArgument(Constant, Base);
            Int32 RegisterIndex = GetIntArgument(Register);

            Boolean bUseDoubleWord = ConstantValue > kMaxArgumentValue;

            Int32 InstructionAddress = LeastInstructionAddress + Offset + (bUseDoubleWord ? 1 : 0);

            Write(InstructionAddress, kInstructionAddressBitsLength);
            Write(RegisterIndex * kRegisterBitsLength, kArgumentBitsLength);

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

        private void WriteRegisterRegister(String FirstRegister, String SecondRegister, Int32 Offset)
        {
            Write(IntToBinaryPadded(LeastInstructionAddress + Offset, kInstructionAddressBitsLength));

            Int32 FirstRegisterIndex = GetIntArgument(FirstRegister);
            Write(FirstRegisterIndex * kRegisterBitsLength, kArgumentBitsLength);

            Int32 SecondRegisterIndex = GetIntArgument(SecondRegister);
            Write(SecondRegisterIndex * kRegisterBitsLength, kArgumentBitsLength);
        }

        #endregion Methods
    }

    [Mnemonic("add")]
    public sealed class AddArithmeticHandler : ArithmeticHandler
    {
        #region Properties

        protected override Int32 LeastInstructionAddress { get { return 0; } }

        #endregion Properties
    }

    [Mnemonic("div")]
    public sealed class DivArithmeticHandler : ArithmeticHandler
    {
        #region Properties

        protected override Int32 LeastInstructionAddress { get { return 18; } }

        #endregion Properties
    }

    [Mnemonic("mult")]
    public sealed class MultArithmeticHandler : ArithmeticHandler
    {
        #region Properties

        protected override Int32 LeastInstructionAddress { get { return 12; } }

        #endregion Properties
    }

    [Mnemonic("sub")]
    public sealed class SubArithmeticHandler : ArithmeticHandler
    {
        #region Properties

        protected override Int32 LeastInstructionAddress { get { return 6; } }

        #endregion Properties
    }
}