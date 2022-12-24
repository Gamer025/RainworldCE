using BepInEx.Logging;
using RainWorldCE.RWHUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Picks a random different event and runs it without telling the player what it is
    /// </summary>
    internal class SecretEvent : CEEvent
    {
        public SecretEvent()
        {
            _name = "Secret event";
            _description = "Chaos is truely unpredictable";
        }

        public override void StartupTrigger()
        {
            Type secretEventType = RainWorldCE.instance.PickEvent();
            // If we are the only enabled event this would cause a loop otherwise
            if (secretEventType == typeof(SecretEvent)) return;
            CEEvent selectedEvent = (CEEvent)Activator.CreateInstance(secretEventType);
            WriteLog(LogLevel.Info, $"Triggering secret event: '{selectedEvent.Name}'");
            selectedEvent.Name = "Secret Event";
            if (selectedEvent.ImplementsMethod("StartupTrigger"))
            {
                    selectedEvent.StartupTrigger();
            }
            if (selectedEvent.ActiveTime > 0)
            {
                RainWorldCE.activeEvents.Add(selectedEvent);
                if (RainWorldCE.showActiveEvents) RainWorldCE.CEHUD.AddActiveEvent(selectedEvent.Name);
                if (selectedEvent.RepeatEverySec > 0)
                {
                    selectedEvent.RecurringEventTime = RainWorldCE.gameTimer;
                }
            }
        }
    }
}
