using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Exceptions
{
    class InvalidMemberImportationException: Exception
    {
        public InvalidMemberImportationException() { }

        public InvalidMemberImportationException(String message) : base(message) { }
    }
}
