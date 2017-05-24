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

        private static Regex MnemonicRegex = new Regex(@"^\s*(?<Mnemonic>[a-z]+)(\s+(?<Args>.*)\s*)?$", kRegexOption);
        private static Dictionary<String, MnemonicHandler> Mnemonics = new Dictionary<String, MnemonicHandler>();

        public static Assembler Instance { get; private set; }

        private readonly VirtualMachineSetup VMSetup;
        private readonly StreamReader StreamReader;
        private readonly StreamWriter StreamWriter;

        public static VirtualMachineSetup VirtualMachineSetup
        {
            get
            {
                return Instance.VMSetup;
            }
        }

        static Assembler()
        {
            var Assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var Types = Assembly.GetTypes();

            foreach(var Type in Types)
            {
                var Attributes = (MnemonicAttribute[])Type.GetCustomAttributes(typeof(MnemonicAttribute), false);

                foreach(var Attr in Attributes)
                {
                    Mnemonics.Add(Attr.Mnemonic, (MnemonicHandler)Activator.CreateInstance(Type));
                }
            }
        }

        public static Assembler Init(int InWordBitsLength,
                                     int InInstructionAddressBitsLength,
                                     StreamReader InInputStream,
                                     StreamWriter InOutputStream)
        {
            return new Assembler(InWordBitsLength, InInstructionAddressBitsLength, InInputStream, InOutputStream);
        }

        private Assembler()
        {
            if (Instance != null)
            {
                throw new Exception("Cannot instance more than one assembler.");
            }

            Instance = this;
        }

        private Assembler(int InWordBitsLength,
                          int InInstructionAddressBitsLength,
                          StreamReader InInputStream,
                          StreamWriter InOutputStream)
                          : this()
        {
            VMSetup = VirtualMachineSetup.CreateSetup(InWordBitsLength, InInstructionAddressBitsLength);

            StreamReader = InInputStream;
            StreamWriter = InOutputStream;
        }


        public void Assemble()
        {
            String Line;
            Match[] Matches;
            MnemonicSyntax? Syntax = null;
            MnemonicHandler MnemonicHandler;

            for (Int32 LineNumber = 1; (Line = StreamReader.ReadLine()) != null; LineNumber++)
            {
                var Match = MnemonicRegex.Match(Line);
                if(Match.Success)
                {
                    var Groups = Match.Groups;

                    if(Mnemonics.TryGetValue(Groups["Mnemonic"].Value, out MnemonicHandler))
                    {
                        String Args = Groups["Args"].Value;


                        if(Args == null)
                            Args = String.Empty;


                        if(MnemonicHandler.TryGetMatchedSyntax(Args, out Matches, out Syntax))
                        {
                            Syntax.GetValueOrDefault().AssembleInstruction(StreamWriter, StreamReader, Matches);
                        }
                        else
                        {
                            throw new Exception("Invalid arguments syntax.");
                        }
                    }
                    else
                    {
                        throw new Exception("Invalid mnemonic.");
                    }
                }
            }
            StreamReader.Close();
            StreamWriter.Close();
        }
    }
}