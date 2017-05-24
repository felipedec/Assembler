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

            Assembler.Init(24, 8, InputStreamReader, OutputStreamWriter);
            Assembler.Instance.Assemble();


            Console.WriteLine("Input:");
            Console.WriteLine(File.ReadAllText("input.txt"));

            Console.WriteLine("Output:");
            Console.WriteLine(File.ReadAllText("output.txt"));
            Console.ReadKey();
        }
    }
}
