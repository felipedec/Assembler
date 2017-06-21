using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Text;

namespace Assembler
{
    using static AssemblyEvent;
    using static AssemblyOptions;
    using static Logger;

    /// <summary>
    /// Resposável por fazer a montagem
    /// </summary>
    public static class AssemblerCore
    {
        /// <summary>
        /// Proporciona informações adicionais para as linhas
        /// podendo garantir que a mesma linha não precise
        /// ser montada mais de uma vez e algumas informações
        /// extras
        /// </summary>
        public struct InputLine
        {
            /// <summary>
            /// Já foi montado?
            /// </summary>
            public Boolean bHasBeenAssembled;

            /// <summary>
            /// É um label?
            /// </summary>
            public Boolean bIsLabel;

            /// <summary>
            /// Texto puro do arquivo de entrada
            /// </summary>
            public String Raw;

            /// <summary>
            /// Caso já tenha sido montado, o resultado sera guardado
            /// para que possa ser reutilizado se necessário 
            /// </summary>
            public String CachedResult;


            public static implicit operator String(InputLine InputLine)
            {
                return InputLine.Raw;
            }

            public static implicit operator InputLine(String Raw)
            {
                return new InputLine() { Raw = Raw.Trim() };
            }
        }

        #region Fields

        /// <summary>
        /// Tamanho em bits de um argumento
        /// </summary>
        public const Int32 kArgumentBitsLength = 8;

        /// <summary>
        /// Tamanho em bits de endereço de instrução
        /// </summary>
        public const Int32 kInstructionAddressBitsLength = 8;

        /// <summary>
        /// Tamanho em bits de um registrador
        /// </summary>
        public const Int32 kRegisterBitsLength = kWordBitsLength;

        /// <summary>
        /// Label de entrada
        /// </summary>
        public const String kEntryLabelName = "MAIN";

        /// <summary>
        /// Opções de uso de qualquer expressão regular deste projeto
        /// </summary>
        public const RegexOptions kRegexOption = RegexOptions.Compiled | RegexOptions.IgnoreCase;

        /// <summary>
        /// Tamanho em bits de uma plavra
        /// </summary>
        public const Int32 kWordBitsLength = 24;

        /// <summary>
        /// Expressão regular de uma comando mnemonico
        /// </summary>
        public static readonly Regex MnemonicRegex;

        /// <summary>
        /// Mnemonicos e seus manipuladores. Auto populado por reflexão
        /// </summary>
        private static Dictionary<String, MnemonicHandler> Mnemonics;

        #endregion Fields


        #region Properties

        /// <summary>
        /// Linhas do arquivo de entrada
        /// </summary>
        public static InputLine[] InputLines { get; private set; }

        /// <summary>
        /// Numero da linha referente a um rótulo
        /// </summary>
        public static Dictionary<String, Int32> Labels { get; private set; }

        /// <summary>
        /// Lista das linhas geradas pelo montador
        /// </summary>
        public static List<String> OutputInstructions { get; private set; }

        /// <summary>
        /// Linha que está sendo lida pelo montador
        /// </summary>
        public static Int32 Line { get; private set; }

        /// <summary>
        /// A montagem foi bem sucedida?
        /// </summary>
        public static Boolean bSuccessed = true;

        #endregion Properties


        #region Constructors

        static AssemblerCore()
        {
            MnemonicRegex = new Regex(@"^\s*(?<Mnemonic>[a-z]+)(\s+(?<Args>.*)\s*)?$", kRegexOption); 
            OutputInstructions = new List<String>();

            FindMnemonicHandlers();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Inicar processo de montagem
        /// </summary>
        public static void Assemble()
        {
            FindLabels();

            Goto(kEntryLabelName);

            for (; Line < InputLines.Length; Line++)
            {
                if(!bSuccessed)
                    return;

                if (InputLines[Line].bIsLabel || String.IsNullOrWhiteSpace(InputLines[Line]))
                    continue;

                AssembleCurrentLine();
            }

            SaveOutput();
        }

        /// <summary>
        /// Salvar arquivo de saída
        /// </summary>
        private static void SaveOutput()
        {
            StringBuilder OutputContent = new StringBuilder();
            foreach (String Instruction in OutputInstructions)
                OutputContent.Append(Instruction + Environment.NewLine);

            try
            {
                File.WriteAllText(Options.OutputFile, OutputContent.ToString());
            }
            catch (Exception e)
            {
                LogFatalError(0, e.Message);
            }
        }

        /// <summary>
        /// Montar linha atual
        /// </summary>
        private static void AssembleCurrentLine()
        {
            Log(InputLines[Line]);

            // Se a linha já foi montada então só adicionar a instrução já gerada na saída.
            if (InputLines[Line].bHasBeenAssembled)
            {
                OutputInstructions.Add(InputLines[Line].CachedResult);
                return;
            }
            var Match = MnemonicRegex.Match(InputLines[Line]);

            if (Match.Success)
            {
                var Groups = Match.Groups;
                String Mnemonic = Groups["Mnemonic"].Value;

                IndentLevel++;

                Current = new AssemblyEvent(Line, AssemblyEventType.Mnemonic);
                Current.Mnemonic = Mnemonic;

                AssembleMnemonic(Mnemonic, Groups["Args"].Value);

                Previously = Current;

                IndentLevel--;

            }
        }

        /// <summary>
        /// Montar a instrução do comando mnemonico.
        /// </summary>
        /// <param name="Mnemonic">Mnemonico</param>
        /// <param name="Args">Argumentos</param>
        private static void AssembleMnemonic(String Mnemonic, String Args)
        {
            MnemonicHandler MnemonicHandler;
            if (Mnemonics.TryGetValue(Mnemonic.ToLower(), out MnemonicHandler))
            {
                // Evitar que seja nulo 
                Args = Args == null ? String.Empty : Args;

                if (MnemonicHandler.TryGetValidArgumentsPattern(Args, out Current.Matches, ref Current.ArgumentsPattern))
                {
                    IndentLevel++;

                    Current.ArgumentsPattern.AssembleInstruction();

                    IndentLevel--;

                    if (Current.bWasInstrucitonAssembled)
                    {

                        InputLines[Current.Line].bHasBeenAssembled = true;
                        InputLines[Current.Line].CachedResult = Current.InstructionBuffer;

                        //String Comment = String.Format("// Line {0}: {1}", Current.Line, InputLines[Current.Line].Raw);
                        //OutputInstructions.Add(Comment);
 
                        OutputInstructions.Add(Current.InstructionBuffer);
                    }
                } 
                else
                {
                    LogFatalError(Line + 1, "\'{0}\' doesn't support this arguments pattern.", Mnemonic);
                }
            }
            else
            {
                LogFatalError(Line + 1, "\'{0}\' invalid mnemonic.", Mnemonic);
            }
        }

        /// <summary>
        /// Pular para uma linha específica
        /// </summary>
        /// <param name="TargetLine">Número da linha</param>
        public static void Jump(Int32 TargetLine)
        {
            Line = TargetLine;
        }

        /// <summary>
        /// Ir para um rótulo
        /// </summary>
        /// <param name="LabelName">Nome do rotúlo</param>
        public static bool Goto(String LabelName)
        {
            Int32 LabelLine;
            if(Labels.TryGetValue(LabelName, out LabelLine))
            {
                Jump(LabelLine);
                return true;
            }

            LogFatalError(Line + 1, "Label \'{0}\' not found.");
            GotoEndOfFile();
            return false;
        }

        /// <summary>
        /// Ir para o final do arquivo, consequentemente
        /// finalizar o processo de montagem
        /// </summary>
        public static void GotoEndOfFile()
        {
            Line = InputLines.Length;
        }

        /// <summary>
        /// Procurar labels no arquivo de entrada
        /// </summary>
        private static void FindLabels()
        {
            var LabelRegex = new Regex(@"^\s*(?<LabelName>[a-z0-9_]+)\s*:\s*$", kRegexOption);
            var Lines = File.ReadAllLines(Options.InputFile);

            InputLines = new InputLine[Lines.Length];
            Labels = new Dictionary<String, Int32>();

            Boolean bEntryLabelFound = false;

            for (Line = 0; Line < Lines.Length; Line++)
            {
                InputLines[Line] = Lines[Line];
                var Match = LabelRegex.Match(Lines[Line]);

                if (Match.Success)
                {
                    Current = new AssemblyEvent(Line, AssemblyEventType.Label);
                    String Label = Match.Groups[1].Value;

                    bEntryLabelFound |= Label.Equals(kEntryLabelName);

                    InputLines[Line].bIsLabel = true;

                    Labels.Add(Label, Line);

                    Previously = Current;
                }
            }

            if(!bEntryLabelFound)
            {
                LogFatalError(0, "\'{0}\' label not found.", kEntryLabelName);
            }
        }

        /// <summary>
        /// Procurar por classes válidas para manipular mnemonicos
        /// </summary>
        private static void FindMnemonicHandlers()
        {
            IndentLevel++;

            Mnemonics = new Dictionary<String, MnemonicHandler>();

            foreach (var Type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!Type.IsSubclassOf(typeof(MnemonicHandler)))
                {
                    continue;
                }

                foreach (var Attr in (MnemonicAttribute[])Type.GetCustomAttributes(typeof(MnemonicAttribute), false))
                {
                    foreach (String Mnemonic in Attr.Mnemonics)
                    {
                        MnemonicHandler MnemonicHandler = (MnemonicHandler)Activator.CreateInstance(Type);
                        Mnemonics.Add(Mnemonic, MnemonicHandler);
                    }
                }
            }

            IndentLevel--;
        }

        #endregion Methods
    }
}