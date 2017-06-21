using System;
using System.IO;

namespace Assembler
{
    using static Logger;
    using static AssemblyOptions;

    static class Assembler
    {
        #region Methods

        static void Main(String[] Args)
        {
            Console.Title = "Palomar 11059";

            // Validar opções de entrada
            Validate(Args);

            try
            {
                // Começar processo de montagem
                AssemblerCore.Assemble();
            }
            catch(Exception e)
            {
                LogFatalError(0, e.Message + "\n\n\n" + e.StackTrace);
            }

            // Exibir conteúdo dos arquivos de entrada e saída caso requisitado
            PrintIOIfNeeded();

            Exit(AssemblerCore.bSuccessed ? 1 : 0);
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
               LogFatalError(0, "Invalid input file.");
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

        #endregion Methods
    }
}