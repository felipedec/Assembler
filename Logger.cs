using System;
using System.Collections.Generic;

namespace Assembler
{
    using System.IO;
    using static AssemblyOptions;

    /// <summary>
    /// Resposável por organizar a saída de dados
    /// </summary>
    public static class Logger
    {
        #region Fields

        /// <summary>
        /// Nível de indentação 
        /// </summary>
        public static Int32 IndentLevel;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Exibir uma linha horizontal no console, ideal para organização
        /// </summary>
        public static void HorizontalLine()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(new String('-', Console.BufferWidth - 1));
            Console.ResetColor();
        }

        /// <summary>
        /// Exibir mensagem
        /// </summary>
        public static void Log(String Format, params object[] Arguments)
        {
            Console.WriteLine(new String(' ', 1 << IndentLevel) + String.Format(Format, Arguments));
        }

        /// <summary>
        /// Reportar um error que não permitiu que
        /// a montagem ocorresse corretamente
        /// </summary>
        /// <param name="Line">Linha do error, 0 se for um erro independente da linha</param>
        /// <param name="Format">Formato da mensagem</param>
        /// <param name="Arguments">Objetos usados para escrever a mensagem</param>
        public static void LogFatalError(Int32 Line, String Format, params object[] Arguments)
        {
            String LineText = Line > 0 ? String.Format(" (Line {0})", Line) : "";

            Console.ForegroundColor = ConsoleColor.Red;
            Log("Fatal Error" + LineText + ": " + Format, Arguments);
            Console.ResetColor();

            AssemblerCore.bSuccessed = false;
        }

        /// <summary>
        /// Exibir conteúdo do arquivo de entrada e saída (--print-io)
        /// </summary>
        public static void PrintIOIfNeeded()
        {
            if (!Options.bPrintInputAndOutput)
                return;

            HorizontalLine();

            String InputFullPath = Path.GetFullPath(Options.InputFile);
            Log("Input File Content ({0}):", InputFullPath);
            PrintFileContent(InputFullPath);

            HorizontalLine();

            if (!AssemblerCore.bSuccessed)
                return;

            String OutputFullPath = Path.GetFullPath(Options.OutputFile);
            Log("Output File Content ({0}):", OutputFullPath);
            PrintFileContent(OutputFullPath);

            HorizontalLine();
        }

        /// <summary>
        /// Exibir o conteúdo de um arquivo de texto com coluna de linhas
        /// </summary>
        /// <param name="FilePath">Arquivo a exibir o conteúdo</param>
        public static void PrintFileContent(String FilePath)
        {
            String[] Lines = File.ReadAllLines(FilePath);

            IndentLevel++;
            Console.ForegroundColor = ConsoleColor.DarkGray;

            for (int Index = 1; Index <= Lines.Length; Index++)
                Log(Index.ToString().PadLeft(3, ' ') + " " + Lines[Index - 1]);

            IndentLevel--;
            Console.ResetColor();
        }

        #endregion Methods
    }
}