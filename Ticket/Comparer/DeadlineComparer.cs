using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ticket.Models;

namespace Ticket.Comparer
{
    public class DeadlineComparer : IComparer<Assignment>
    {
        public int Compare(Assignment x, Assignment y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (y == null)
                {
                    return 1;
                }
                else
                {
                    return x.Deadline.CompareTo(y.Deadline);
                }
            }
        }
    }
}