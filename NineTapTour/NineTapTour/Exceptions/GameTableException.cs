using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NineTapTour.Exceptions
{
    class GameTableException : Exception
    {
        public GameTableException() { }

        public GameTableException(String message) : base(message) { }
    }
}
