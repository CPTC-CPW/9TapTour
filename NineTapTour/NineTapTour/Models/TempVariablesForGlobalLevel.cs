using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Models
{
     /// <summary>
     /// This class is made to hold and store variables for use in a form 
     /// and held when the form is closed allowing them to be useable
     /// again when the form is reopened. For example if you
     /// wanted to access store a game score in form 1 but needed to go look 
     /// at something in form 2, when you reopened form 1 it would be 
     /// accesible in form 1 again.
     /// </summary>
    public static class TempVariablesForGlobalLevel
    {
    
        
        public static List<double> MoneyEarnings { get; set; }

    }
}
