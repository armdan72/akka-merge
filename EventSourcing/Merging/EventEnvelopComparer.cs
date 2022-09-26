using Akka.Persistence.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcing.Merging
{
    public class EventEnvelopComparer : IComparer<EventEnvelope>
    {
        public int Compare(EventEnvelope x, EventEnvelope y)
        {
            if (x.Timestamp == y.Timestamp)
            {
                return 0;
            }
            if (x.Timestamp < y.Timestamp)
            {
                return -1;
            }
            return 1;
        }
    }
}
