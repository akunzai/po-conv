using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace PoConvert.Exception
{
    class InvalidFormatException : System.Exception, ISerializable
    {
        public InvalidFormatException()
            : base("Invalid format") { }

        public InvalidFormatException(string message)
            : base(message) { }

        public InvalidFormatException(string message, System.Exception inner)
            : base(message, inner) { }

        protected InvalidFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
