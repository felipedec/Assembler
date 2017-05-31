using System;
using System.IO;

namespace Assembler
{
    class AssemblerMain
    {
        static void Main(String[] Args)
        {
            Assembler.SetStreams("input.txt", "output.txt");
            Assembler.Assemble();


            Console.WriteLine("Input:");
            Console.WriteLine(File.ReadAllText("input.txt"));

            Console.WriteLine("Output:");
            Console.WriteLine(File.ReadAllText("output.txt"));
            Console.ReadKey();
        }
    }
}
