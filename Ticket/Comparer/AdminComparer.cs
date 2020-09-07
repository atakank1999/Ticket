using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ticket.Models;

namespace Ticket.Comparer
{
    public class AdminComparer : IComparer<Assignment>
    {
        public int Compare(Assignment x, Assignment y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    // If x is null and y is null, they're
                    // equal.
                    return 0;
                }
                else
                {
                    // If x is null and y is not null, y
                    // is greater.
                    return -1;
                }
            }
            else
            {
                // If x is not null...
                //
                if (y == null)
                // ...and y is null, x is greater.
                {
                    return 1;
                }
                else
                {
                    return x.Admin.Username.CompareTo(y.Admin.Username);
                }
            }
        }
    }
}