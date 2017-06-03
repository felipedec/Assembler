using System;

namespace Assembler
{
    /// <summary>
    /// Responsável por attribuir um mnemonico para um <see cref="MnemonicHandler"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class MnemonicAttribute : Attribute
    {
        #region Fields

        public readonly string[] Mnemonics;

        #endregion Fields

        #region Constructors

        public MnemonicAttribute(params string[] InMnemonics)
        {
            Mnemonics = InMnemonics;
        }

        #endregion Constructors
    }
}