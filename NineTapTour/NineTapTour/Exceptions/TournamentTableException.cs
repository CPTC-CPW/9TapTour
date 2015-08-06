using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Exceptions
{
    class TournamentTableException : Exception
    {
        public TournamentTableException() { }

        public TournamentTableException(String message) : base(message) { }
    }
}
