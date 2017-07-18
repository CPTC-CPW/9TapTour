using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NineTapTour.Exceptions
{
    class ParticipantTableException : Exception
    {
        public ParticipantTableException() { }

        public ParticipantTableException(String message) : base(message) { }
    }
}
