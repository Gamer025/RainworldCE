using RainWorldCE.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainWorldCE.Config.CustomChaos
{
    internal class CCExecuteEvent : CCEntry
    {
        internal int time;
        internal string name;
        internal bool timedEvent = false;
        internal Type eventClass;

        public CCExecuteEvent(Type eventClass, int time)
        {
            this.time = time;
            CEEvent ceevent;
            this.eventClass = eventClass;
            ceevent = (CEEvent)Activator.CreateInstance(eventClass);
            name = ceevent.Name;
            if (ceevent.ActiveTime > 0)
                timedEvent = true;
        }

        public override int doAction()
        {
            //Need to recreate the event here and not sure it in the constructor since ctor may have run while game not active
            CEEvent ceevent = (CEEvent)Activator.CreateInstance(eventClass);
            if (time > 0 && ceevent.ActiveTime > 0)
                ceevent.ActiveTime = time;
            RainWorldCE.activateEvent(ceevent);
            return 0;
        }

        public override string ToString()
        {
            if (time > 0 && timedEvent)
                return $"Activate the '{name}' event for {time} seconds";
            else
                return $"Activate the '{name}' event";
        }
    }
}
