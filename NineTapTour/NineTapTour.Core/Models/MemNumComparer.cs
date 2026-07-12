using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Models
{
    public class MemberNumComparer : IComparer<Member>
    {
        int IComparer<Member>.Compare(Member x, Member y)
        {
            int mem1 = x.Number;
            int mem2 = y.Number;
            return mem1.CompareTo(mem2);
        }
    }
}
