using System;
using System.Runtime.Serialization;

namespace HistoryBuffer
{
    public class HistoryIsEmptyException : Exception
    {
        public HistoryIsEmptyException()
        {
        }

        public HistoryIsEmptyException(string message) : base(message)
        {
        }

        public HistoryIsEmptyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HistoryIsEmptyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}