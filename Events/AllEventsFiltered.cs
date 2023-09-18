using RainWorldCE.Attributes;

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
