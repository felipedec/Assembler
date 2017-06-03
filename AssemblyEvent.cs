using System;
using System.Text.RegularExpressions;

namespace Assembler
{
    /// <summary>
    /// Cada linha analisada pelo montador é um evento
    /// que é representado por objetos desta classe
    /// </summary>
    public class AssemblyEvent
    {

        #region Fields

        /// <summary>
        /// Evento atual do montador
        /// </summary>
        public static AssemblyEvent Current;

        /// <summary>
        /// Evento anterior do montador
        /// </summary>
        public static AssemblyEvent Previously;

        /// <summary>
        /// Ultimo padrão de argumentos combinado
        /// com o ultimo comando de mnemonico
        /// </summary>
        public ArgumentsPattern ArgumentsPattern;

        /// <summary>
        /// A ultima instrução teve uma instrução montada?
        /// 
        /// Alguns comandos não escreverão nada na saída, logo
        /// não podemos definir que ele foi montado, então essa
        /// variável só sera verdadeira se alguma instrução for
        /// escrita para a saída
        /// </summary>
        public Boolean bWasInstrucitonAssembled;

        /// <summary>
        /// Buffer para a instrução que esta sendo
        /// escrita ou foi escrita neste evento
        /// </summary>
        public String InstructionBuffer = String.Empty;

        /// <summary>
        /// Linha que esta sendo analisada neste evento
        /// </summary>
        public Int32 Line;

        /// <summary>
        /// Ultimas combinações do ultimo commando mnemonico
        /// </summary>
        public Match[] Matches;

        /// <summary>
        /// Tipo de evento
        /// </summary>
        public AssemblyEventType Type;

        /// <summary>
        /// Quantas instruções este evento gerou
        /// </summary>
        public Int32 InstructionCount;

        /// <summary>
        /// Caso este evento esteja montando a instrução
        /// de um mnemonico ele ficara armazenado aqui
        /// </summary>
        public String Mnemonic;

        #endregion Fields


        #region Properties

        /// <summary>
        /// Linha de entrada (detalhada) deste evento
        /// </summary>
        public AssemblerCore.InputLine InputLine
        {
            get
            {
                return AssemblerCore.InputLines[Line];
            }
        }

        #endregion Properties

        #region Constructors

        public AssemblyEvent(Int32 InLine, AssemblyEventType InEventType)
        {
            Type = InEventType;
            Line = InLine;
        }

        #endregion Constructors

    }

    /// <summary>
    /// Tipo de evento, como cada linha tem
    /// seu tipo, como chamar um mnemonic
    /// ou declarar um rotúlo
    /// </summary>
    public enum AssemblyEventType
    {
        Mnemonic,
        Label,
    }
}