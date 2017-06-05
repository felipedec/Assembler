using CommandLine;
using CommandLine.Text;
using System;

namespace Assembler
{
    public class AssemblyOptions
    {
        #region Fields

        /// <summary>
        /// Singleton 
        /// </summary>
        public static AssemblyOptions Options;

        #endregion Fields


        #region Properties

        /// <summary>
        /// Saída detalhada
        /// </summary>
        [Option('v', "verbose", DefaultValue = false, HelpText = "Verbose mode.")]
        public Boolean bUseVerbose { get; set; }

        /// <summary>
        /// Caminho do arquivo de entrada
        /// </summary>
        [Option('i', "input", Required = true, HelpText = "Input File.")]
        public String InputFile { get; set; }

        /// <summary>
        /// Caminho do arquivo de saída
        /// </summary>
        [Option('o', "output", DefaultValue ="output.txt", HelpText = "Output File.")]
        public String OutputFile { get; set; }

        /// <summary>
        /// Exibir o conteúdo do arquivo de entrada e de saída?
        /// </summary>
        [Option('p', "print-io", HelpText = "Print Input/Output files lines.")]
        public Boolean bPrintInputAndOutput { get; set; }

        #endregion Properties

        #region Methods

        [HelpOption]
        public String GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText Current) => HelpText.DefaultParsingErrorsHandler(this, Current));
        }

        public static Boolean GetOptions(String[] Arguments)
        {
            Options = new AssemblyOptions();
            return Parser.Default.ParseArguments(Arguments, Options);
        }

        #endregion Methods
    }
}