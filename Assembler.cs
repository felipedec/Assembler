using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace Assembler
{
    public class Assembler
    {
        public const RegexOptions kRegexOption = RegexOptions.Compiled | RegexOptions.IgnoreCase;
        private static Regex MnemonicRegex = new Regex(@"^\s*(?<mnemonic>[a-z]+)(\s+(?<args>.*)\s*)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static Dictionary<string, MnemonicHandler> Mnemonics = new Dictionary<string, MnemonicHandler>();

        private readonly VirtualMachineSetup m_VirtualMachineSetup;
        private readonly StreamReader m_Input;
        private readonly StreamWriter m_Output;

        static Assembler()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();

            foreach(var type in types)
            {
                var attributes = (MnemonicAttribute[])type.GetCustomAttributes(typeof(MnemonicAttribute), false);

                foreach(var attr in attributes)
                {
                    Mnemonics.Add(attr.Mnemonic, (MnemonicHandler)Activator.CreateInstance(type));
                }
            }
        }

        public Assembler(VirtualMachineSetup virtualMachineSetup, StreamReader input, StreamWriter output)
        {
            this.m_VirtualMachineSetup = virtualMachineSetup;
            this.m_Input = input;
            this.m_Output = output;
        }


        public void Assemble()
        {
            string line;
            for (int counter = 0; (line = this.m_Input.ReadLine()) != null; counter++)
            {
                var match = MnemonicRegex.Match(line);
                if(match.Success)
                {
                    var groups = match.Groups;

                    MnemonicHandler mnemonicHandler;
                    if(Mnemonics.TryGetValue(groups["mnemonic"].Value, out mnemonicHandler))
                    {
                        string args = groups["args"].Value;

                        if(string.IsNullOrEmpty(args))
                        {
                            args = string.Empty;
                        }

                        Match[] matches;
                        int matchIndex = mnemonicHandler.GetParametersMatchedIndex(args, out matches);

                        if(matchIndex < 0)
                        {
                            throw new Exception("Invalid arguments.");
                        }

                        mnemonicHandler.AssembleMachineCode(0, matches, this.m_Output, this.m_VirtualMachineSetup);
                    }
                    else
                    {
                        throw new System.Exception("Invalid mnemonic.");
                    }
                }
            }
            this.m_Input.Close();
            this.m_Output.Close();
        }
    }
}