using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Exceptions
{
    class PlayerHistoryTableException: Exception
    {
        public PlayerHistoryTableException() { }

        public PlayerHistoryTableException(String message) : base(message) { }
    }
}

