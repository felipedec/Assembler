using System;

namespace Assembler
{
    [Mnemonic("Add")]
    public sealed class AddHandler : MnemonicHandler
    {
        public override MnemonicSyntax[] GetParametersSupported()
        {
            return new MnemonicSyntax[]
            {
                /* Recebe um registrador e um numero decimal. */
                CreateSyntax(new String[] { "R(?<Register>[0-9]+)", "#(?<Decimal>[0-9]+)" }, () =>
                {        
            
                }),

                /* Recebe um registrador e o endereço de outro registrador. */
                CreateSyntax(new String[] { "R(?<Register>[0-9]+)", "@R(?<Register2>[0-9]+)" }, () =>
                {
                }),

                /* Recebe o registrador e um numero em binario. */
                CreateSyntax(new String[] { "R(?<Register>[0-9]+)", "(?<Binary>[01]+)" }, () =>
                {

                }),

                /* Recebe dois registradores. */
                CreateSyntax(new String[] { "R(?<Register>[0-9]+)", "R(?<Register>[0-9]+)" }, () =>  
                {

                }) 
            };
        }
    }
}