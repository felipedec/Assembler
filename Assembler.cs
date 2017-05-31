using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace Assembler
{
    public static class Assembler
    {
        public const Int32 kInstructionAddressBitsLength = 8;
        public const Int32 kArgumentBitsLength = 8;
        public const Int32 kWordBitsLength = 24;
        public const Int32 kMaxArgumentValue = 0xFF;

        public const RegexOptions kRegexOption = RegexOptions.Compiled | RegexOptions.IgnoreCase;

        private static Regex MnemonicRegex = new Regex(@"^\s*(?<Mnemonic>[a-z]+)(\s+(?<Args>.*)\s*)?$", kRegexOption);
        private static Dictionary<String, MnemonicHandler> Mnemonics = new Dictionary<String, MnemonicHandler>();

        private static Match[] PreviouslyMatches = new Match[];

        private static String[] InputLines;

        public static StreamReader StreamReader { get; private set; }
        public static StreamWriter OutputStream { get; private set; }

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

        public static void SetStreams(String InputPath, String OutputPath)
        {
            OutputStream = new StreamWriter(OutputPath);
            InputLines = File.ReadAllLines(InputPath);
        }
        public static void Assemble()
        {
            for (Int32 LineIndex = 0; LineIndex < InputLines.Length; LineIndex++)
            {
                String Line = InputLines[LineIndex];

                var Match = MnemonicRegex.Match(Line);

                if(Match.Success)
                {
                    var Groups = Match.Groups;

                    MnemonicHandler MnemonicHandler;
                    if (Mnemonics.TryGetValue(Groups["Mnemonic"].Value, out MnemonicHandler))
                    {
                        String Args = Groups["Args"].Value;


                        if(Args == null)
                            Args = String.Empty;

                        MnemonicSyntax Syntax;
                        if(MnemonicHandler.TryGetMatchedSyntax(Args, out PreviouslyMatches, out Syntax))
                        {
                            Syntax.AssembleInstruction();
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
            OutputStream.Close();
        }
    }
}