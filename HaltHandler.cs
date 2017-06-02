using System;

namespace Assembler
{
    using static AssemblerCore;

    [Mnemonic("halt")]
    public sealed class HaltHandler : MnemonicHandler
    {
        #region Methods

        public override ArgumentsPattern[] GetArgumentsPatterns()
        {
            return new ArgumentsPattern[]
            {
                CreateArgumentsPattern(new String[0], () =>
                {
                    Write(0xFFFFFF, kWordBitsLength);
                    GotoEndOfFile();
                })
            };
        }

        #endregion Methods
    }
}