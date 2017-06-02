using System.Text.RegularExpressions;
using System;
using System.IO;

namespace Assembler
{
    using static AssemblerCore;
    using static AssemblyEvent;
    
    /// <summary>
    /// Base para todo manipulador de comando mnemonico,
    /// é responsável pela montagem das instruções
    /// 
    /// A class derivada dela só sera reconhecida como
    /// um mnemonico se for atribuida a ela o attributo
    /// <see cref="MnemonicAttribute"/>
    /// </summary>
    public abstract class MnemonicHandler
    {
        #region Fields

        /// <summary>
        /// Padrões de argumentos suportada por este comando.
        /// </summary>
        private ArgumentsPattern[] SupportedArgumentsPattern;

        #endregion Fields

        #region Constructors

        public MnemonicHandler()
        {
            SupportedArgumentsPattern = GetArgumentsPatterns();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Cria um padrão de argumento passando as expressões
        /// regulares de cada parametro e o metodo que será
        /// resposável pela montagem da instrução
        /// </summary>
        /// <param name="ParametersRegExpression">Expressões regulares de cada parametro</param>
        /// <param name="AssembleAction">Métdo resposável pela montagem da instrução</param>
        /// <returns></returns>
        public static ArgumentsPattern CreateArgumentsPattern(string[] ParametersRegExpression, Action AssembleAction)
        {
            return new ArgumentsPattern(GetParametersRegex(ParametersRegExpression), AssembleAction);
        }

        /// <summary>
        /// Retorna todos os padrões de argumentos suportados por este mnemonico
        /// </summary>
        public abstract ArgumentsPattern[] GetArgumentsPatterns();

        /// <summary>
        /// Tenta achar um padrão de argumentos que combina com os argumentos passados
        /// e volta as combinações alcançadas e o padrão que combinou
        /// </summary>
        /// <param name="Args">Argumentos</param>
        /// <param name="OutMatches">Combinações alcançadas pelos argumentos</param>
        /// <param name="OutMatchedPattern">Padraõ que combinou com os argumentos</param>
        /// <returns>
        /// Retorna verdadeiro caso encontre uma sintaxe que combina
        /// com os argumentos passados, caso contrário retorna falso
        /// </returns>
        public bool TryGetValidArgumentsPattern(string Args, out Match[] OutMatches, out ArgumentsPattern OutMatchedPattern)
        {
            OutMatches = null;

            string[] ArgsArray = Args.Split(new string[]{ " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);

            for (int Index = 0; Index < SupportedArgumentsPattern.Length; Index++)
            {
                if(SupportedArgumentsPattern[Index].Match(ArgsArray, out OutMatches))
                {
                    OutMatchedPattern = SupportedArgumentsPattern[Index];
                    return true;
                }
            }
            OutMatchedPattern = default(ArgumentsPattern);
            return false;
        }
        /// <summary>
        /// Recebe um vetor de padrões de expressões regulares em expressões regulares
        /// </summary>
        /// <param name="Patterns">Padrões das expressões regulares dos argumentos</param>
        /// <returns>Expressõe regulares dos argumentos</returns>
        protected static Regex[] GetParametersRegex(params string[] Patterns)
        {
            var Result = new Regex[Patterns.Length];

            for (int Index = 0; Index < Patterns.Length; Index++)
            {
                Result[Index] = new Regex("^" + Patterns[Index].ToLower() + "$", kRegexOption);
            }
            return Result;
        }


        /// <summary>
        /// Retorna o valor de um argumento inteiro
        /// </summary>
        /// <param name="ParameterName">Nome do parametro</param>
        /// <param name="FromBase">Base numérica do argumento</param>
        /// <returns></returns>
        public static Int32 GetIntArgument(String ParameterName, Int32 FromBase = 10)
        {
            return Convert.ToInt32(GetArgument(ParameterName), FromBase);
        }

        /// <summary>
        /// Retorna o valor de um argumento
        /// </summary>
        /// <param name="ParameterName">Nome do parametro</param>
        public static String GetArgument(String ParameterName)
        {
            ParameterName = ParameterName.ToLower();

            for (Int32 Index = 0; Index < Current.ArgumentsPattern.ArgumentsCount; Index++)
            {
                Int32 GroupNum = Current.ArgumentsPattern[Index].GroupNumberFromName(ParameterName);

                if (GroupNum > -1)
                {
                    return Current.Matches[Index].Groups[GroupNum].Value;
                }
            }

            throw new ArgumentException("Invalid parameter name", "ParameterName");
        }

        /// <summary>
        /// Escreve no arquivo de saída
        /// </summary>
        /// <param name="Value">O valor a escrever</param>
        public static void Write(string Value)
        {
            Boolean bMustAppendLineNumber = !Current.bWasInstrucitonAssembled;

            Int32 WordBitsLength = (bUseLineNumber ? 32 : 24);
            if (Current.InstructionBuffer.Length % WordBitsLength == 0 && Current.bWasInstrucitonAssembled)
            {
                bMustAppendLineNumber = bUseLineNumber;

                Current.InstructionCount++;
                Current.InstructionBuffer += Environment.NewLine;
            }

            if(!Current.bWasInstrucitonAssembled)
                Current.InstructionCount = 1;

            if (bMustAppendLineNumber)
            {
                Int32 Line = Current.InstructionCount == 1 ? Current.Line : 0;
                Current.InstructionBuffer += IntToBinaryPadded(Line, kArgumentBitsLength);
            }

            Current.bWasInstrucitonAssembled = true;
            Current.InstructionBuffer += Value;
        }


        /// <summary>
        /// Escrever um valor em binario com um tamanho específico da saída
        /// </summary>
        /// <param name="Value">Valor a escrever</param>
        /// <param name="Pad">Tamanho minímo da saída</param>
        public static void Write(Int32 Value, Int32 Pad)
        {
            Write(IntToBinaryPadded(Value, Pad));
        }

        /// <summary>
        /// Converte um valor inteiro para um string
        /// em formato binário complentando os zero
        /// as esquerda de acordo com o tamano do pad
        /// </summary>
        /// <param name="Value">Valor a ser convertido</param>
        /// <param name="Pad">Tamanho da minimo da saída</param>
        /// <returns></returns>
        public static String IntToBinaryPadded(Int32 Value, Int32 Pad)
        {
            return Convert.ToString(Value, 2).PadLeft(Pad, '0');
        }

        #endregion Methods
    }
}
