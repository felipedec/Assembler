using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Text;

namespace Assembler
{
    using static AssemblyEvent;
    using static Assembler;

    /// <summary>
    /// Resposável por fazer a montagem
    /// </summary>
    public static class AssemblerCore
    {
        /// <summary>
        /// Proporciona informações adicionais para as linhas
        /// podendo garantir que a mesma linha não precise
        /// ser montada mais de uma vez, e
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
            /// Caso já tenha sido montado, o resultado sera quardado
            /// para que possa ser reutilizado se necessário 
            /// </summary>
            public String CachedResult;


            public static implicit operator String(InputLine InputLine)
            {
                return InputLine.Raw;
            }

            public static implicit operator InputLine(String Raw)
            {
                return new InputLine() { Raw = Raw };
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
        /// Valor máximo aceitado por um argumento
        /// </summary>
        public const Int32 kMaxArgumentValue = 0xFF;

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
        /// Stream para o arquivo de saída
        /// </summary>
        public static StreamWriter Output { get; private set; }

        /// <summary>
        /// Lista das linhas geradas pelo montador
        /// </summary>
        public static List<String> OutputInstructions { get; private set; }

        /// <summary>
        /// Linha que está sendo lida pelo montador
        /// </summary>
        public static Int32 Line { get; private set; }


        public static Boolean bUseLineNumber = true;

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
            for (Line = 0; Line < InputLines.Length; Line++)
            {
                if (InputLines[Line].bIsLabel || String.IsNullOrWhiteSpace(InputLines[Line]))
                {
                    continue;
                }

                AssembleCurrentLine();
            }

            // Aclopar todo conteudo de saida.
            StringBuilder OutputContent = new StringBuilder();
            foreach (String Line in OutputInstructions)
                OutputContent.Append(Line + Environment.NewLine);

        
            Output.Write(OutputContent);
            Output.Close();
        }

        /// <summary>
        /// Montar linha atual
        /// </summary>
        public static void AssembleCurrentLine()
        {
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

                Current = new AssemblyEvent(Line, AssemblyEventType.Mnemonic);

                Current.Mnemonic = Mnemonic;

                AssembleMnemonic(Mnemonic, Groups["Args"].Value);

                Previously = Current;
            }
        }

        /// <summary>
        /// Montar a instrução do comando mnemonico.
        /// </summary>
        /// <param name="Mnemonic">Mnemonico</param>
        /// <param name="Args">Argumentos</param>
        public static void AssembleMnemonic(String Mnemonic, String Args)
        {
            MnemonicHandler MnemonicHandler;
            if (Mnemonics.TryGetValue(Mnemonic.ToLower(), out MnemonicHandler))
            {
                // Evitar que seja nulo 
                Args = Args == null ? String.Empty : Args;

                if (MnemonicHandler.TryGetValidArgumentsPattern(Args, out Current.Matches, out Current.ArgumentsPattern))
                {
                    Current.ArgumentsPattern.AssembleInstruction();

                    if (Current.bWasInstrucitonAssembled)
                    {
                        InputLines[Current.Line].bHasBeenAssembled = true;
                        InputLines[Current.Line].CachedResult = Current.InstructionBuffer;

                        OutputInstructions.Add(Current.InstructionBuffer);
                    }
                }
            }
            else
            {
                LogError(Line + 1, "\'{0}\' comando inválido.", Mnemonic);
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
        public static void Goto(String LabelName)
        {
            Line = Labels[LabelName];
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
        /// Definir arquivo de entra e saida do montador
        /// </summary>
        /// <param name="InputPath">Arquivo de entrada</param>
        /// <param name="OutputPath">Arquivo de saída</param>
        public static void SetIO(String InputPath, String OutputPath)
        {
            var LabelRegex = new Regex(@"^\s*(?<LabelName>[a-z0-9]+)\s*:\s*$", kRegexOption);

            Output = new StreamWriter(OutputPath);
            var Lines = File.ReadAllLines(InputPath);

            InputLines = new InputLine[Lines.Length];
            Labels = new Dictionary<string, int>();

            for (Line = 0; Line < Lines.Length; Line++)
            {
                Current = new AssemblyEvent(Line, AssemblyEventType.Label);

                InputLines[Line] = Lines[Line];
                var Match = LabelRegex.Match(Lines[Line]);

                if (Match.Success)
                {
                    InputLines[Line].bIsLabel = true;

                    Labels.Add(Match.Groups[1].Value, Line);
                }

                Previously = Current;
            }
        }

        /// <summary>
        /// Procurar por classes válidas para manipular mnemonicos
        /// </summary>
        private static void FindMnemonicHandlers()
        {
            Mnemonics = new Dictionary<String, MnemonicHandler>();

            foreach (var Type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (var Attr in (MnemonicAttribute[])Type.GetCustomAttributes(typeof(MnemonicAttribute), false))
                    Mnemonics.Add(Attr.Mnemonic, (MnemonicHandler)Activator.CreateInstance(Type));
            }
        }


        #endregion Methods
    }
}