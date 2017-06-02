using System;

namespace Assembler
{
    using static AssemblerCore;

    [Mnemonic("call")]
    public sealed class CallHandler : MnemonicHandler
    {
        #region Methods

        public override ArgumentsPattern[] GetArgumentsPatterns()
        {
            return new ArgumentsPattern[]
            {
                CreateArgumentsPattern(new String[] { "(?<Label>[a-zA-Z][a-zA-Z0-9]*)" }, () =>
                {
                    Goto(GetArgument("Label"));
                })
            };
        }

        #endregion Methods
    }
}