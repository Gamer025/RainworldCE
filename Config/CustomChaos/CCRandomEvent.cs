using RainWorldCE.Events;
using System;

namespace RainWorldCE.Config.CustomChaos
{
    internal class CCRandomEvent : CCEntry
    {
        public CCRandomEvent()
        {
        }

        public override int doAction()
        {
            //Need to recreate the event here and not sure it in the constructor since ctor may have run while game not active
            Type eventClass = RainWorldCE.PickEvent();
            CEEvent ceevent = (CEEvent)Activator.CreateInstance(eventClass);
            RainWorldCE.ActivateEvent(ceevent);
            return 0;
        }

        public override string ToString()
        {
            return $"Activate a random event";
        }
    }
}
