using System;

namespace WardrobeApp.Exceptions
{
    public class WardrobeException: Exception
    {
        public WardrobeException() { }

        public WardrobeException(string message) : base(message) { }

        public WardrobeException(string message, Exception inner) : base(message, inner) { }
    }
}
