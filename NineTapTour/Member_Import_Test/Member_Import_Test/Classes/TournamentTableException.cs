using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Member_Import_Test.Classes
{
    class TournamentTableException : Exception
    {
        public TournamentTableException() { }

        public TournamentTableException(String message) : base(message) { }
    }
}
