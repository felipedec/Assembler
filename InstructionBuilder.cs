using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Assembler
{
    public delegate Int32 ValueModifierDelegate(Int32 Value);

    public class InstructionBuilder
    {
        private String CachedInstruction;

        public InstructionBuilder AppendInstructionAddress(String GroupName, ValueModifierDelegate ValueModifier = null)
        {
            return AppendByGroupName(AppendInstructionAddress, GroupName, ValueModifier);
        }

        public InstructionBuilder AppendInstructionAddress(Int32 InstructionAddress)
        {
            return AppendValue(Assembler.kInstructionAddressBitsLength, InstructionAddress);
        }

        public InstructionBuilder AppendArgument(String GroupName, ValueModifierDelegate ValueModifier = null)
        {
            return AppendByGroupName(AppendArgument, GroupName, ValueModifier);
        }

        public InstructionBuilder AppendArgument(Int32 ArgumentValue)
        {
            return AppendValue(Assembler.kArgumentBitsLength, ArgumentValue);
        }

        public InstructionBuilder AppendWord(String GroupName, ValueModifierDelegate ValueModifier = null)
        {
            return AppendByGroupName(AppendWord, GroupName, ValueModifier);
        }

        public InstructionBuilder AppendWord(Int32 WordValue)
        {
            return AppendValue(Assembler.kWordBitsLength, WordValue);
        }

        public InstructionBuilder Write()
        {
            Assembler.OutputLines.AddLast(CachedInstruction);
            return this;
        }

        public InstructionBuilder AddString(String String)
        {
            CachedInstruction += String;
            return this;
        }

        public InstructionBuilder NextWord()
        {
            CachedInstruction += "\n";
            return this;
        }

        private InstructionBuilder AppendByGroupName(Func<Int32, InstructionBuilder> Append,
                                                     String GroupName,
                                                     ValueModifierDelegate ValueModifier)
        {
            Int32 Value = GetValueByGroupName<Int32>(GroupName);

            if (ValueModifier != null)
            {
                Value = ValueModifier(Value);
            }

            Append(Value);
            return this;
        }

        private InstructionBuilder AppendValue(Int32 MaxLength, Int32 Value)
        {
            String BinaryString = Convert.ToString(Value, 2);

            if (BinaryString.Length > MaxLength)
            {
                throw new Exception("Argument greater than supported size.");
            }

            BinaryString = BinaryString.PadLeft(MaxLength);

            CachedInstruction += BinaryString;

            return this;
        }

        private T GetValueByGroupName<T>(string GroupName)
        {
            foreach (Match Match in Assembler.PreviouslyMatches)
            {
                Group Group = Match.Groups[GroupName];

                if(Group == null)
                {
                    continue;
                }

                String Value = Group.Value;

                return ConvertValue<T, string>(Value);
            }
            throw new Exception("Invalid group name.");
        }

        private T GetGroupValue<T>(int ArgumentIndex, string GroupName)
        {
            Group Group = Assembler.PreviouslyMatches[ArgumentIndex].Groups[GroupName.ToLower()];
            return ConvertValue<T, string>(Group.Value);
        }

        private static T ConvertValue<T, U>(U value) where U : IConvertible
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}