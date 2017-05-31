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

        public static StreamReader StreamReader { get; private set; }
        public static StreamWriter StreamWriter { get; private set; }

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

        public static void SetStreams(StreamReader InInputStream, StreamWriter InOutputStream)
        {
            StreamReader = InInputStream;
            StreamWriter = InOutputStream;
        }
        public static void Assemble()
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