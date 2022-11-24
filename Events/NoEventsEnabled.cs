﻿using RainWorldCE.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainWorldCE.Events
{
    [InternalCEEvent]
    internal class NoEventsEnabled : CEEvent
    {
        public NoEventsEnabled()
        {
            _name = "No events enabled";
            _description = "Go check your settings and enable some of the fun";
            _activeTime = 9999;

        }
    }
}
