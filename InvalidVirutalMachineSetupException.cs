using System;
using System.Runtime.Serialization;

namespace Assembler
{
    [Serializable]
    public class InvalidVirutalMachineSetupException : Exception
    {
        private const string kMessage = "Invalid virtual machine setup, you must follow thoses rules below:\n"
                                        + "\tWord bits length has to be greater than instruction bits length.\n"
                                        + "\tWord bits minus instruction address bits length has to be even.\n";

        public InvalidVirutalMachineSetupException() : base(kMessage) { }
        public InvalidVirutalMachineSetupException(Exception inner) : base(kMessage, inner) { }

        protected InvalidVirutalMachineSetupException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}