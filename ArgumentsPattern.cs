using System;
using System.Text.RegularExpressions;

namespace Assembler
{
    /// <summary>
    /// Padrão de argumentos, utilizado por <see cref="MnemonicHandler" />
    /// para definir quaís os formatos de argumentos são aceitáveis pelo comando
    /// </summary>
    public struct ArgumentsPattern
    {
        #region Properties

        /// <summary>
        /// Numero de argumentos
        /// </summary>
        public int ArgumentsCount
        {
            get
            {
                return ArgumentsRegExpressions.Length;
            }
        }

        /// <summary>
        /// Expressão regular de cada argumento suportado
        /// </summary>
        public Regex[] ArgumentsRegExpressions { private set; get; }

        /// <summary>
        /// Método responsável por montar a instrução
        /// </summary>
        public Action AssembleInstruction { private set; get; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Utilize <see cref="MnemonicHandler.CreateArgumentsPattern(string[], Action)" />
        /// </summary>
        public ArgumentsPattern(Regex[] InArgumentsRegExpressions, Action InAssembleInstruction)
        {
            ArgumentsRegExpressions = InArgumentsRegExpressions;
            AssembleInstruction = InAssembleInstruction;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Verifica se os argumentos passado combinam 
        /// com o padrão de argumentos especificado
        /// </summary>
        /// <param name="Args">Argumentos</param>
        /// <param name="OutMatches">Combinão de cada argumento</param>
        /// <returns></returns>
        public bool Match(string[] Args, ref Match[] OutMatches)
        {
            if (Args.Length != ArgumentsCount)
                return false;

            var TempMatches = new Match[Args.Length];

            for (int j = 0; j < Args.Length; j++)
            {
                TempMatches[j] = ArgumentsRegExpressions[j].Match(Args[j]);

                if (!TempMatches[j].Success)
                    return false;
            }

            OutMatches = TempMatches;
            return true;
        }

        public Regex this[Int32 Index]
        {
            get
            {
                return ArgumentsRegExpressions[Index];
            }
        }

        #endregion Methods
    }
}