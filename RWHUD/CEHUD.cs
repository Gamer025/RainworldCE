using System;
using System.Collections.Generic;
using System.Linq;
using RainWorldCE.Events;
using BepInEx.Logging;
using System.Reflection.Emit;
using System.Collections.ObjectModel;

namespace RainWorldCE.RWHUD
{
    public class CEHUD : HUD.HudPart
    {
        /// <summary>
        /// Label for when a new event is getting triggered containing its name
        /// </summary>
        private readonly FLabel eventNameLabel;

        /// <summary>
        /// //Label for when a new event is getting triggered containing its description
        /// </summary>
        private readonly FLabel eventDescriptionLabel;

        /// <summary>
        /// When true generate and display random events names
        /// </summary>
        private bool eventSelection = false;

        /// <summary>
        /// List of all existing event names
        /// </summary>
        private readonly List<string> eventNames = new List<string>();

        /// <summary>
        /// The currently displayed active events
        /// </summary>
        private readonly List<FLabel> activeEventLabels = new List<FLabel>();
        /// <summary>
        /// How long selected events will be displayed for
        /// </summary>
        public static Configurable<int> eventDisplayTime;

        readonly Random rand = new Random();

        public CEHUD(HUD.HUD hud, IEnumerable<CEEvent> activeEvents) : base(hud)
        {
            RainWorldCE.CEHUD = this;
            rand = new Random();
            //Event name label
            eventNameLabel = new FLabel("font", String.Empty)
            {
                y = hud.rainWorld.screenSize.y * 0.15f,
                x = hud.rainWorld.screenSize.x / 2,
                scale = 2f
            };
            hud.fContainers[1].AddChild(eventNameLabel);
            //Event description label
            eventDescriptionLabel = new FLabel("font", String.Empty)
            {
                y = eventNameLabel.y - 30f,
                x = hud.rainWorld.screenSize.x / 2,
                scale = 1.5f
            };
            hud.fContainers[1].AddChild(eventDescriptionLabel);

            eventNames = EventHelpers.GetAllEventNames();

            //Stuff like Warp mod deletes and readds all HUD elements when switching regions
            //For that case lets find all active events and readd them
            if (RainWorldCE.showActiveEvents && activeEvents.Any())
            {
                foreach (CEEvent ceevent in activeEvents)
                {
                    AddActiveEvent(ceevent.Name);
                }
            }

        }
        /// <summary>
        /// Used with timeDelta to get consistent execution time
        /// </summary>
        float timePool = 0;
        int displayCounter = 0;
        //timeStacker is delatime * 'framesPerSecond' (40)
        public override void Draw(float timeStacker)
        {
            timePool += timeStacker;
            //Should run about every 100ms assuming FPS is fine
            if (timePool > 3f)
            {
                //Show random event names if selection in progress
                if (eventSelection)
                {
                    //RainWorldCE.ME.Logger_p.Log(LogLevel.Debug, $"Count: {eventNames.Count} Events: {String.Join(",", eventNames.ToArray())}");
                    eventNameLabel.text = eventNames[rand.Next(eventNames.Count)];
                }
                //Otherwise keep up the current text for around config seconds and then remove it
                else if (eventNameLabel.text != String.Empty)
                {
                    displayCounter++;
                    if (displayCounter > eventDisplayTime.Value * 10)
                    {
                        eventNameLabel.text = String.Empty;
                        eventDescriptionLabel.text = String.Empty;
                        displayCounter = 0;
                    }

                }
                timePool -= 4f;
                //FPS seem to be really low / game window was froozen, reset timePool
                if (timePool > 4f) timePool = 0;
            }

        }

        internal void StartEventSelection()
        {
            eventNameLabel.text = String.Empty;
            eventDescriptionLabel.text = String.Empty;
            displayCounter = 0;
            eventSelection = true;
        }

        internal void StopEventSelection(CEEvent selectedEvent)
        {
            eventSelection = false;
            eventNameLabel.text = selectedEvent.Name;
            eventDescriptionLabel.text = selectedEvent.Description;

        }
        /// <summary>
        /// Add a label with the events name to the bottom left of the game
        /// </summary>
        /// <param name="eventName"></param>
        public void AddActiveEvent(string eventName)
        {
            FLabel newActiveEventLabel = new FLabel("font", eventName)
            {
                x = hud.rainWorld.screenSize.y * 0.01f,
                y = 150f + 30f * activeEventLabels.Count,
                scale = 1f,
                alignment = FLabelAlignment.Left
            };
            activeEventLabels.Add(newActiveEventLabel);
            hud.fContainers[1].AddChild(newActiveEventLabel);
        }
        /// <summary>
        /// Remove a label with the events name
        /// </summary>
        /// <param name="eventName"></param>
        internal void RemoveActiveEvent(string eventName)
        {
            for (int i = activeEventLabels.Count - 1; i >= 0; i--)
            {
                FLabel label = activeEventLabels[i];
                if (label.text == eventName)
                {
                    hud.fContainers[1].RemoveChild(label);
                    activeEventLabels.RemoveAt(i);
                    FixActiveEventHoles();
                    //Only remove the first occurence, in case event is active multiple times but only one is actually ending
                    return;
                }
            }
        }

        /// <summary>
        /// Moves activeEventLabels to fill out gaps created by removing events
        /// </summary>
        internal void FixActiveEventHoles()
        {
            //First label starts at 300f
            float startingY = 150f;
            int counter = 0;
            //Start from the lowest label and work our way up
            foreach (FLabel label in activeEventLabels.OrderBy(a => a.y))
            {
                //RainWorldCE.ME.Logger_p.Log(LogLevel.Debug, $"Label {label.text} at Ypos {label.y}. Should be at {startingY + 30f * counter}");
                //If label not at expected y move it
                if (label.y != startingY + 30f * counter)
                {
                    label.y = startingY + 30f * counter;
                }
                counter++;
            }
        }
    }
}
