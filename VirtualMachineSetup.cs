namespace Assembler
{
    public sealed class VirtualMachineSetup
    {
        private readonly int m_WordBitsLength;
        private readonly int m_InstructionAddressBitsLength;
        private readonly int m_ParametersBitsLength;
        private readonly int m_MaxInstructionAddress;

        public int ParametersBitsLength
        {
            get
            {
                return this.m_ParametersBitsLength;
            }
        }

        public int MaxInstructionAddress
        {
            get
            {
                return m_MaxInstructionAddress;
            }
        }

        public int InstructionAddressBitsLength
        {
            get
            {
                return m_InstructionAddressBitsLength;
            }
        }

        public int WordBitsLength
        {
            get
            {
                return m_WordBitsLength;
            }
        }

        private VirtualMachineSetup()
        {
            // Private constructor..
        }

        private VirtualMachineSetup(int wordBitsLength, int instructionAddressBitsLength)
        {
            this.m_WordBitsLength = wordBitsLength;
            this.m_InstructionAddressBitsLength = instructionAddressBitsLength;
            this.m_ParametersBitsLength = (this.m_WordBitsLength - this.m_InstructionAddressBitsLength) / 2;
            this.m_MaxInstructionAddress = (1 << this.m_ParametersBitsLength + 1) - 1;
            this.m_MaxInstructionAddress -= this.m_MaxInstructionAddress % this.m_ParametersBitsLength;
        }

        public static VirtualMachineSetup CreateSetup(int wordBitsLength, int instructionAddressBitsLength)
        {
            var virtualMachineSetup = new VirtualMachineSetup(wordBitsLength, instructionAddressBitsLength);

            if (instructionAddressBitsLength >= wordBitsLength
                || (wordBitsLength - instructionAddressBitsLength) % 2 == 1
                || virtualMachineSetup.WordBitsLength <= 0
                || virtualMachineSetup.InstructionAddressBitsLength <= 0)
                throw new InvalidVirutalMachineSetupException();

            return virtualMachineSetup;
        }
    }
}