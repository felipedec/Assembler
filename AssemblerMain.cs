using System;
using System.IO;

namespace Assembler
{
    class AssemblerMain
    {
        private const String kTestInputFile = "..\\..\\Testing\\input.txt";
        private const String kTestOutputFile = "..\\..\\Testing\\output.txt";

        static void Main(String[] Args)
        {
            Assembler.SetStreams(kTestInputFile, kTestOutputFile);
            Assembler.Assemble();


            Console.WriteLine("Input:");
            Console.WriteLine(File.ReadAllText(kTestInputFile));

            Console.WriteLine("Output:");
            Console.WriteLine(File.ReadAllText(kTestOutputFile));
            Console.ReadKey();
        }
    }
}
