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

        public readonly string Mnemonic;

        #endregion Fields

        #region Constructors

        public MnemonicAttribute(string InMnemonic)
        {
            Mnemonic = InMnemonic.ToLower();
        }

        #endregion Constructors
    }
}