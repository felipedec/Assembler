using System;
using System.IO;

namespace Assembler
{
    class Program
    {
        static void Main(string[] args)
        {
            VirtualMachineSetup virtualMachineSetup = null;

            try
            {
                virtualMachineSetup = VirtualMachineSetup.CreateSetup(24, 8);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            var inputStreamReader = new StreamReader("input.txt");
            var outputStreamWriter = new StreamWriter("output.txt");

            var assembler = new Assembler(virtualMachineSetup, inputStreamReader, outputStreamWriter);
            assembler.Assemble();


            Console.WriteLine("Input:");
            Console.WriteLine(File.ReadAllText("input.txt"));

            Console.WriteLine("Output:");
            Console.WriteLine(File.ReadAllText("output.txt"));

            Console.ReadKey();
        }
    }
}
