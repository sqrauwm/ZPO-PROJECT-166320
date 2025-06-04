using System;

namespace WardrobeApp.Exceptions
{
    public class DataLoadException : WardrobeException
    {
        public DataLoadException() { }

        public DataLoadException(string message) : base(message) { }

        public DataLoadException(string message, Exception inner) : base(message, inner) { }
    }
}
