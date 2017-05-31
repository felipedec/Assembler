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

        public static Match[] PreviouslyMatches { get; private set; }
        public static String[] InputLines { get; private set; }
        public static StreamWriter Output { get; private set; }
        public static Int32 CurrentLine { get; private set; }
        public static LinkedList<String> OutputLines { get; private set; }

        static Assembler()
        {
            OutputLines = new LinkedList<String>();
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
            Output = new StreamWriter(OutputPath);
            InputLines = File.ReadAllLines(InputPath);
        }
        public static void Assemble()
        {
            for (CurrentLine = 0; CurrentLine < InputLines.Length; CurrentLine++)
            {
                String Line = InputLines[CurrentLine];
          
                var Match = MnemonicRegex.Match(Line);

                if(Match.Success)
                {
                    var Groups = Match.Groups;

                    MnemonicHandler MnemonicHandler;
                    if (Mnemonics.TryGetValue(Groups["Mnemonic"].Value.ToLower(), out MnemonicHandler))
                    {
                        String Args = Groups["Args"].Value;


                        if(Args == null)
                            Args = String.Empty;

                        MnemonicSyntax? Syntax;
                        Match[] Matches;
                        if(MnemonicHandler.TryGetMatchedSyntax(Args, out Matches, out Syntax))
                        {
                            PreviouslyMatches = Matches;
                            Syntax.GetValueOrDefault().AssembleInstruction();
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
            Output.Close();
        }
    }
}