using System;
using System.IO;

namespace Assembler
{
    using static AssemblyOptions;
    using static AssemblerCore;
    using static Logger;

    static class Assembler
    {
        #region Methods

        static void Main(String[] Args)
        {
            Console.Title = "11059NULLIUSINVERBA";

            Validate(Args);
            Assemble();
            PrintIOIfNeeded();
            PrintErrors();

            Exit();
        }

        /// <summary>
        /// Validar linha de comando e arquivos de entrada
        /// </summary>
        static void Validate(String[] Args)
        {
            if (!GetOptions(Args))
            {
                Exit(1);
            }

            if (!File.Exists(Options.InputFile))
            {
                Console.Write("Invalid input file.");
                Exit(1);
            }
        }

        /// <summary>
        /// Finalizar o processo
        /// </summary>
        static void Exit(Int32 ExitCode = 0)
        {
            if (Environment.UserInteractive)
                Console.ReadKey();
            Environment.Exit(ExitCode);
        }

        static void PrintIOIfNeeded()
        {
            if (!Options.bPrintInputAndOutput)
                return;


            Console.WriteLine("Input File Content:");
            String[] Lines = File.ReadAllLines(Options.InputFile);

            for (int Index = 1; Index <= Lines.Length; Index++)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(Index.ToString().PadLeft(3, ' ') + " ");

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(Lines[Index - 1]);
            }

            Console.ResetColor();

            Console.WriteLine("Output File Content:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(File.ReadAllText(Options.OutputFile));

        }

        #endregion Methods
    }
}
