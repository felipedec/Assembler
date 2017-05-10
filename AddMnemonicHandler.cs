using System.IO;
using System.Text.RegularExpressions;

namespace Assembler
{

    [Mnemonic("add")]
    public sealed class AddMnemonicHandler : MnemonicHandler
    {
        public delegate void AssembleDelegate(Match[] matches, StreamWriter sw, VirtualMachineSetup vms);

        public static AssembleDelegate[] Assemble = new AssembleDelegate[]
        {
            AddDecimal,
            AddAt,
            AddBinary,
            Add
        };

        public override Regex[][] GetParametersSupported()
        {
            var firstParameter = new Regex(@"R(?<register>[0-9]+)", Assembler.kRegexOption);

            return new Regex[][]
            {
                new Regex[] { firstParameter, new Regex(@"#(?<decimal>[0-9]+)", Assembler.kRegexOption) },
                new Regex[] { firstParameter, new Regex(@"@R(?<register>[0-9]+)", Assembler.kRegexOption) },
                new Regex[] { firstParameter, new Regex("(?<binary>[01]+)", Assembler.kRegexOption) },
                new Regex[] { firstParameter, new Regex(@"R(?<register>[0-9]+)", Assembler.kRegexOption) },
            };
        }

        public override void AssembleMachineCode(int parameterSupportedIndex, Match[] matches, StreamWriter sw, VirtualMachineSetup vms)
        {
            Assemble[parameterSupportedIndex](matches, sw, vms);
        }

        private static void AddDecimal(Match[] matches, StreamWriter sw, VirtualMachineSetup vms)
        {

        }

        private static void AddBinary(Match[] matches, StreamWriter sw, VirtualMachineSetup vms)
        {

        }

        private static void AddAt(Match[] matches, StreamWriter sw, VirtualMachineSetup vms)
        {

        }

        private static void Add(Match[] matches, StreamWriter sw, VirtualMachineSetup vms)
        {

        }
    }
}