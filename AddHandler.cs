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
                CreateSyntax(new String[] { "R(?<Register>[0-9]+)", "#(?<Decimal>[0-9]+)" }, (Info) =>
                {        
                    Int32 Decimal = Info.GetGroupValue<Int32>(1, "Decimal");
                    Int32 RegisterIndex = Info.GetGroupValue<Int32>(0, "Register");
                    Boolean bUseDoubleWord = (Decimal < Assembler.kMaxArgumentValue);
                }),

                /* Recebe um registrador e o endereço de outro registrador. */
                CreateSyntax(new String[] { "R(?<Register>[0-9]+)", "@R(?<Register2>[0-9]+)" }, (Info) =>
                {
                    Int32 RegisterIndex = Info.GetGroupValue<Int32>(0, "Register");
                    Int32 RegisterIndexTwo = Info.GetGroupValue<Int32>(1, "Register2");
                }),

                /* Recebe o registrador e um numero em binario. */
                CreateSyntax(new String[] { "R(?<Register>[0-9]+)", "(?<Binary>[01]+)" }, (Info) =>
                {
                    Int32 RegisterIndex = Info.GetGroupValue<Int32>(0, "Register");
                    String BinaryString = Info.GetGroupValue(1, "Binary");
                    Int32 Value = Convert.ToInt32(BinaryString, 2);
                    Boolean bUseDoubleWord = (Value < Assembler.kMaxArgumentValue);
                }),

                /* Recebe dois registradores. */
                CreateSyntax(new String[] { "R(?<Register>[0-9]+)", "R(?<Register>[0-9]+)" }, (Info) =>  
                {
                    Int32 RegisterIndexOne = Info.GetGroupValue<Int32>(0, "Register");
                    Int32 RegisterIndexTwo = Info.GetGroupValue<Int32>(1, "Register");
                }) 
            };
        }
    }
}