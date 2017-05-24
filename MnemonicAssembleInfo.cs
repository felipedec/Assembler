using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Assembler
{
    public struct MnemonicAssembleInfo
    {
        public MnemonicSyntax Syntax;
        public Match[] Matches;
        public StreamWriter StreamWriter;
        public StreamReader StreamReader;

        public MnemonicAssembleInfo(MnemonicSyntax InSyntax,
                                    Match[] InMatches,
                                    StreamWriter InStreamWriter,
                                    StreamReader InStreamReader)
        {
            Syntax = InSyntax;
            Matches = InMatches;
            StreamWriter = InStreamWriter;
            StreamReader = InStreamReader;
        }

        public string GetGroupValue(int ArgumentIndex, string GroupName)
        {
            Group Group = Matches[ArgumentIndex].Groups[GroupName.ToLower()];
            return Group == null ? null : Group.Value;
        }

        public T GetGroupValue<T>(int ArgumentIndex, string GroupName)
        {
            Group Group = Matches[ArgumentIndex].Groups[GroupName.ToLower()];
            return ConvertValue<T, string>(Group.Value);
        }

        private static T ConvertValue<T, U>(U value) where U : IConvertible
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}