using System;
using System.Collections.Generic;
using System.IO;

namespace Assembler
{
    class Assembler
    {
        private static Stack<String> ErrorOutput = new Stack<string>();

        private const String kTestInputFile = "..\\..\\Testing\\input.txt";
        private const String kTestOutputFile = "..\\..\\Testing\\output.txt";

        static void Main(String[] Args)
        {
            AssemblerCore.SetIO(kTestInputFile, kTestOutputFile);
            AssemblerCore.Assemble();


            Console.WriteLine("Arquivo de entrada:");
            String[] Lines = File.ReadAllLines(kTestInputFile);

            for (int Index = 1; Index <= Lines.Length; Index++)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(Index.ToString().PadLeft(3, ' ') + " ");

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(Lines[Index - 1]);
            }

            Console.ResetColor();

            Console.WriteLine("Arquivo de saída:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(File.ReadAllText(kTestOutputFile));

            PrintOutputErrors();

            Console.ReadKey();
        }

        private static void PrintOutputErrors()
        {
            if (ErrorOutput.Count == 0)
                return;

            Console.WriteLine("Erros na Montagem:");

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkRed;

            while(ErrorOutput.Count > 0)
            {
                Console.WriteLine(ErrorOutput.Pop());
            }

            Console.ResetColor();
        }

        public static void LogError(Int32 Line, String Format, params object[] Arguments)
        {
            String Prefix = Line > 0 ? String.Format("Linha ({0}): ", Line) : "";
            ErrorOutput.Push(Prefix + String.Format(Format, Arguments));
        }
    }
}
