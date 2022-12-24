using BepInEx.Logging;
using OptionalUI;
using RainWorldCE.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RainWorldCE.Config
{
    public class RainWorldCEOI : OptionInterface
    {
        public RainWorldCEOI() : base(plugin: RainWorldCE.instance)
        {

        }

        /// <summary>
        /// Scrollbox for event enable/disable status
        /// </summary>
        OpScrollBox eventSB;
        /// <summary>
        /// Scrollbox for event specific configs
        /// </summary>
        OpScrollBox eventConfigSB;

        public override void Initialize()
        {
            base.Initialize(); // This should be called before everything else
            RainWorldCE.ME.Logger_p.Log(LogLevel.Info, "[CM] ConfigMachine init");

            Tabs = new OpTab[3]; // Each OpTab is 600 x 600 pixel sized canvas
            Tabs[0] = new OpTab("Main");
            Tabs[1] = new OpTab("Events");
            Tabs[2] = new OpTab("Event Config");

            //Event Duration slider
            Tabs[0].AddItems(new OpLabel(260f, 570f, "Duration between events:")
            { description = "Duration in seconds between events." });
            Tabs[0].AddItems(new OpSlider(new Vector2(10f, 530f), "eventTimer", new RWCustom.IntVector2(10, 600), defaultValue: 60)
            { description = "Duration in seconds between events." });
            //Event repeat slider
            Tabs[0].AddItems(new OpLabel(10f, 500f, "Don't repeat events for (%):")
            { description = "Prevent events from repeating until X% of the other enabled events triggered." });
            Tabs[0].AddItems(new OpSlider(new Vector2(175f, 495f), "blockedEventCount", new RWCustom.IntVector2(0, 100), defaultValue: 50)
            { description = "Prevent events from repeating until X% of the other enabled events triggered." });
            //Max events slider
            Tabs[0].AddItems(new OpLabel(10f, 460f, "Max events per cycle:")
            { description = "Maximum amount of chaos events to trigger per ingame cycle." });
            Tabs[0].AddItems(new OpSlider(new Vector2(140f, 455f), "maxEventCount", new RWCustom.IntVector2(1, 200), defaultValue: 200)
            { description = "Maximum amount of chaos events to trigger per ingame cycle." });
            //Event time multiplier
            Tabs[0].AddItems(new OpLabel(10f, 420f, "Event duration multiplier (base is 10):")
            {
                description = "Allows you to decrase/increase the length of some time based events.\r\n" +
            "10 is normal event length, 1 would be 10 times shorter, 50 would be 5 times longer events."
            });
            Tabs[0].AddItems(new OpSlider(new Vector2(220f, 415f), "eventDurationMult", new RWCustom.IntVector2(1, 50), 300, defaultValue: 10)
            {
                description = "Allows you to decrase/increase the length of some time based events.\r\n" +
            "10 is normal event length, 1 would be 10 times shorter, 50 would be 5 times longer events."
            });
            //Show active events checkbox
            Tabs[0].AddItems(new OpCheckBox(new Vector2(10f, 390f), "showActiveEvents", defaultBool: true)
            { description = "Show active events in the bottom left?" });
            Tabs[0].AddItems(new OpLabel(45f, 390f, "Show active events")
            { description = "Show active events in the bottom left?" });
            Tabs[0].AddItems(new OpLabel(250f, 5f, $"RainWorldCE Version {RainWorldCE.modVersion}")
            { description = "Made by Gamer025" });


            //Generate checkboxes for disabling/enabling events + custom event config
            Tabs[1].AddItems(new OpLabel(260f, 580f, "Active events:", bigText: true)
            { description = "Here you can enable and disable which events can happen." });
            Tabs[2].AddItems(new OpLabel(260f, 580f, "Event config:", bigText: true)
            { description = "Here you can change the configuration of specific events" });

            //All CEEvent types from which we can pick and create objects
            RainWorldCE.eventTypes = RainWorldCE.GetAllCEEventTypes().OrderBy(x => x.Name).ToList();
            List<CEEvent> events = RainWorldCE.eventTypes.Select(x => (CEEvent)Activator.CreateInstance(x)).OrderBy(e => e.Name).ToList();
            eventSB = new OpScrollBox(new Vector2(10f, 10f), new Vector2(580f, 560f), events.Count * 30f + 20f);
            Tabs[1].AddItems(eventSB);
            eventConfigSB = new OpScrollBox(new Vector2(10f, 10f), new Vector2(580f, 560f), 0f);
            Tabs[2].AddItems(eventConfigSB);

            float eventsY = eventSB.GetContentSize() - 40f;
            float configY = eventConfigSB.GetContentSize() - 40f;
            int sbContentSize = 0;

            foreach (CEEvent ceevent in events)
            {
                //Event enable/disable checkboxes
                eventSB.AddItems(new OpCheckBox(new Vector2(10f, eventsY), $"CEEvent_{ceevent.GetType().Name}", defaultBool: true)
                { description = ceevent.Name });
                eventSB.AddItems(new OpLabel(45f, eventsY, ceevent.Name)
                { description = ceevent.Name });
                eventsY -= 30f;

                //Custom event configs
                if (ceevent.ConfigEntries.Count > 0)
                {
                    sbContentSize += 60;
                    eventConfigSB.AddItems(new OpLabel(20f, configY, ceevent.Name, bigText: true));
                    configY -= 30f;
                    foreach (EventConfigEntry entry in ceevent.ConfigEntries)
                    {
                        foreach (UIelement element in entry.CMelement(new Vector2(40f, configY)))
                        {
                            Tabs[2].AddItems(element);
                        }
                        sbContentSize += entry.size;
                        configY -= entry.size;
                    }
                    configY -= 30f;
                }
            }
        }

        public override void ConfigOnChange()
        {
            base.ConfigOnChange();
            //CE internal configs
            RainWorldCE.ME.Logger_p.Log(LogLevel.Debug, $"[CM] Setting internal configs");
            RainWorldCE.eventTimeout = int.Parse(config["eventTimer"]);
            RainWorldCE.maxEventCount = int.Parse(config["maxEventCount"]);
            RainWorldCE.showActiveEvents = bool.Parse(config["showActiveEvents"]);
            RainWorldCE.eventDurationMult = float.Parse(config["eventDurationMult"]) / 10;

            //Config field for events to pull from via GetConfig
            CEEvent.config = config;

            //Clear enabled events and add all checked events
            RainWorldCE.eventTypes.Clear();
            foreach (UIelement element in eventSB.children)
            {
                if (element.GetType() == typeof(OpCheckBox))
                {
                    OpCheckBox box = (OpCheckBox)element;

                    //Even though all the event checkboxes are set to true by default they won't have a entry in config on bootup
                    //This means adding a new event to the code and trying to access its key without checking if it exists caused an exception
                    //This caused CM to reset/loose the whole config
                    bool enabled = true;
                    if (config.ContainsKey(box.key))
                    {
                        bool.TryParse(config[box.key], out enabled);
                    }

                    RainWorldCE.ME.Logger_p.Log(LogLevel.Debug, $"[CM] Setting {box.key} to: {enabled} from ConfigMachine");
                    if (enabled)
                    {
                        RainWorldCE.eventTypes.Add(Type.GetType($"RainWorldCE.Events.{box.key.Replace("CEEvent_", "")}"));
                    }
                }

            }
            //If we have more than 1 enabled event calculate the amount of event repettion block
            if (RainWorldCE.eventTypes.Count > 1)
            {
                int blockCount =
                    Convert.ToInt32(Math.Min(
                        (double)RainWorldCE.eventTypes.Count * int.Parse(config["blockedEventCount"]) / 100,
                        (double)RainWorldCE.eventTypes.Count - 1));

                RainWorldCE.ME.Logger_p.Log(LogLevel.Debug, $"[CM] Setting blockedEvents to {blockCount} from ConfigMachine");
                RainWorldCE.blockedEvents = new Type[blockCount];
            }
            else
            {
                RainWorldCE.ME.Logger_p.Log(LogLevel.Debug, $"[CM] Less than two enabled events, setting blockedEvents to 0 from ConfigMachine");
                RainWorldCE.blockedEvents = new Type[0];
            }

        }
    }
}
