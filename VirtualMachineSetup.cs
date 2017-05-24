using System;

namespace Assembler
{
    public sealed class VirtualMachineSetup
    {
        public readonly int WordBitsLength;
        public readonly int InstructionAddressBitsLength;
        public readonly int ArgumentBitsLength;
        public readonly int MaxAddress;

        private VirtualMachineSetup()
        {
            // Private constructor..
        }

        public string IntToBinaryString(Int32 Value)
        {
            return Convert.ToString(Value, 2).PadLeft(InstructionAddressBitsLength, '0');
        }

        private VirtualMachineSetup(int InWordBitsLength, int InInstructionAddressBitsLength)
        {
            WordBitsLength = InWordBitsLength;
            InstructionAddressBitsLength = InInstructionAddressBitsLength;
            ArgumentBitsLength = (WordBitsLength - InstructionAddressBitsLength) / 2;
            MaxAddress = (1 << ArgumentBitsLength + 1) - 1;
            MaxAddress -= MaxAddress % ArgumentBitsLength;
        }

        public static VirtualMachineSetup CreateSetup(int InWordBitsLength, int InInstructionAddressBitsLength)
        {
            var VirtualMachineSetup = new VirtualMachineSetup(InWordBitsLength, InInstructionAddressBitsLength);

            if (InInstructionAddressBitsLength >= InWordBitsLength
                || (InWordBitsLength - InInstructionAddressBitsLength) % 2 == 1
                || VirtualMachineSetup.WordBitsLength <= 0
                || VirtualMachineSetup.InstructionAddressBitsLength <= 0)
                throw new Exception("Invalid virtual machine setup.");

            return VirtualMachineSetup;
        }
    }
}