using RainWorldCE.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainWorldCE.Events
{
    [InternalCEEvent]
    internal class AllEventsFiltered : CEEvent
    {
        public AllEventsFiltered()
        {
            _name = "Nothing";
            _description = "Seems like you ran all out of events";
        }
    }
}
