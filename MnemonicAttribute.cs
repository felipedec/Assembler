using System;

namespace Assembler
{

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class MnemonicAttribute : Attribute
    {
        public readonly string Mnemonic;

        public MnemonicAttribute(string mnemonic)
        {
            this.Mnemonic = mnemonic;
        }
    }
}