using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NineTapTour.Exceptions
{
    class MemberTableException : Exception
    {
        public MemberTableException() { }

        public MemberTableException(String message) : base(message) { }
    }
}
