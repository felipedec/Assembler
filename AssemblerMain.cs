using System;
using System.IO;

namespace Assembler
{
    class AssemblerMain
    {
        static void Main(String[] Args)
        {
            var InputStreamReader = new StreamReader("input.txt");
            var OutputStreamWriter = new StreamWriter("output.txt");

            Assembler.SetStreams(InputStreamReader, OutputStreamWriter);
            Assembler.Assemble();


            Console.WriteLine("Input:");
            Console.WriteLine(File.ReadAllText("input.txt"));

            Console.WriteLine("Output:");
            Console.WriteLine(File.ReadAllText("output.txt"));
            Console.ReadKey();
        }
    }
}
