using BepInEx.Logging;
using RainWorldCE.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Menu.Remix.MixedUI;
using System.IO;
using System.Reflection;
using RainWorldCE.RWHUD;

namespace RainWorldCE.Config
{
    public class RainWorldCEOI : OptionInterface
    {
        public RainWorldCEOI()
        {
            RainWorldCE.eventTimeout = config.Bind("eventTimeout", 60, new ConfigAcceptableRange<int>(10, 600));
            RainWorldCE.eventTimeoutOffset = config.Bind("eventTimeoutOffset", 15, new ConfigAcceptableRange<int>(0, 600));
            RainWorldCE.blockedEventPercent = config.Bind("blockedEventPercent", 50, new ConfigAcceptableRange<int>(0, 100));
            RainWorldCE.maxEventCount = config.Bind("maxEventCount", 200, new ConfigAcceptableRange<int>(1, 200));
            RainWorldCE._eventDurationMult = config.Bind("eventDurationMult", 10, new ConfigAcceptableRange<int>(1, 50));
            CEHUD.eventDisplayTime = config.Bind("eventDisplayTime", 5, new ConfigAcceptableRange<int>(0, 15));
            RainWorldCE._showActiveEvents = config.Bind("showActiveEvents", true, new ConfigurableInfo("Show active events in the bottom left?", null, "Show active events"));
            RainWorldCE.triggerEventKey = config.Bind("triggerEeventKey", KeyCode.None);
            RainWorldCE.debugLogs = config.Bind("debugLogs2", false, new ConfigurableInfo("Enable extra debug logs", null, "May decrease performance"));

            // Enable/Disable event checkboxes
            List<Type> allEventTypes = RainWorldCE.GetAllCEEventTypes().OrderBy(x => x.Name).ToList();
            allCEEvents = allEventTypes.Select(x => (CEEvent)Activator.CreateInstance(x)).OrderBy(e => e.Name).ToList();
            foreach (CEEvent ceevent in allCEEvents)
            {
                //Event enable/disable checkboxes
                Configurable<bool> checkboxConfig;
                checkboxConfig = config.Bind($"CEEvent_{ceevent.GetType().Name}", true, new ConfigurableInfo("Enable/Disable event"));
                eventStatus.Add($"CEEvent_{ceevent.GetType().Name}", checkboxConfig);
            }

            //Custom event configs, need to retrieve these on construction so they bind their configs
            foreach (CEEvent ceevent in allCEEvents)
            {
                if (ceevent.ConfigEntries.Count > 0)
                {
                    RainWorldCE.ME.Logger_p.Log(LogLevel.Info, $"Binding event config for: {ceevent.Name}");
                    foreach (EventConfigEntry entry in ceevent.ConfigEntries)
                    {
                        entry.BindConfigs(this);
                    }
                }
            }
        }

        /// <summary>
        /// Scrollbox for event enable/disable status
        /// </summary>
        OpScrollBox eventSB;
        /// <summary>
        /// Scrollbox for event specific configs
        /// </summary>
        OpScrollBox eventConfigSB;
        /// <summary>
        /// All CEEvents defined in code
        /// </summary>
        List<CEEvent> allCEEvents;

        public static Dictionary<string, Configurable<bool>> eventStatus = new Dictionary<string, Configurable<bool>>();

        public override void Initialize()
        {
            base.Initialize();
            RainWorldCE.TryLoadCC();
            RainWorldCE.ME.Logger_p.Log(LogLevel.Info, "Remix OI init");

            Tabs = new OpTab[4]; // Each OpTab is 600 x 600 pixel sized canvas
            Tabs[0] = new OpTab(this, "Main");
            Tabs[1] = new OpTab(this, "Events");
            Tabs[2] = new OpTab(this, "Event Config");
            Tabs[3] = new OpTab(this, "Debug/Extras");

            //Event Duration slider
            Tabs[0].AddItems(new OpLabel(240f, 570f, "Duration between events:")
            { description = "Duration in seconds between events." });
            Tabs[0].AddItems(new OpSlider(RainWorldCE.eventTimeout, new Vector2(10f, 530f), 570)
            { description = "Duration in seconds between events." });

            //Event Duration Offset slider
            Tabs[0].AddItems(new OpLabel(210f, 500f, "Offset/Random time between events:")
            { description = "The duration between events will randomly be increased or decreased by up to this many seconds.\nFor example 15 means that every event can happen up to 15 seconds sooner or later but also 10,7,3 seconds etc." });
            Tabs[0].AddItems(new OpSlider(RainWorldCE.eventTimeoutOffset, new Vector2(10f, 460f), 570)
            { description = "The duration between events will randomly be increased or decreased by up to this many seconds.\nFor example 15 means that every event can happen up to 15 seconds sooner or later but also 10,7,3 seconds etc." });

            //Event repeat slider
            Tabs[0].AddItems(new OpLabel(10f, 430f, "Don't repeat events for (%):")
            { description = "Prevent events from repeating until X% of the other enabled events triggered." });
            Tabs[0].AddItems(new OpSlider(RainWorldCE.blockedEventPercent, new Vector2(175f, 425f), 100)
            { description = "Prevent events from repeating until X% of the other enabled events triggered." });

            //Max events slider
            Tabs[0].AddItems(new OpLabel(10f, 400f, "Max events per cycle:")
            { description = "Maximum amount of chaos events to trigger per ingame cycle." });
            Tabs[0].AddItems(new OpSlider(RainWorldCE.maxEventCount, new Vector2(140f, 395f), 200)
            { description = "Maximum amount of chaos events to trigger per ingame cycle." });

            //Event time multiplier
            Tabs[0].AddItems(new OpLabel(10f, 370f, "Event duration multiplier (base is 10):")
            {
                description = "Allows you to decrase/increase the length of some time based events.\r\n" +
            "10 is normal event length, 1 would be 10 times shorter, 50 would be 5 times longer events."
            });
            Tabs[0].AddItems(new OpSlider(RainWorldCE._eventDurationMult, new Vector2(220f, 365f), 100)
            {
                description = "Allows you to decrase/increase the length of some time based events.\r\n" +
            "10 is normal event length, 1 would be 10 times shorter, 50 would be 5 times longer events."
            });

            //Event display time
            Tabs[0].AddItems(new OpLabel(10f, 340f, "Event name/description display time:")
            {
                description = "Determines how long event name and description will be displayed for, when an event gets selected in seconds."
            });
            Tabs[0].AddItems(new OpSlider(CEHUD.eventDisplayTime, new Vector2(220f, 335f), 100)
            {
                description = "Determines how long event name and description will be displayed for, when an event gets selected in seconds."
            });

            //Show active events checkbox
            Tabs[0].AddItems(new OpCheckBox(RainWorldCE._showActiveEvents, new Vector2(10f, 310f)));
            Tabs[0].AddItems(new OpLabel(45f, 310f, "Show active events")
            { description = "Show active events in the bottom left?" });

            //Open in explorer button
            OpSimpleButton openButton = new OpSimpleButton(new Vector2(225, 50f), new Vector2(150, 30f), "Open Mod directory")
            {
                description = "Opens the mods current mod directory for easy access to its files"
            };
            openButton.OnClick += OpenModFolder;
            Tabs[0].AddItems(openButton);

            //Credits
            Tabs[0].AddItems(new OpLabel(225f, 5f, $"RainWorldCE Version {RainWorldCE.modVersion}")
            { description = "Made by Gamer025" });


            //Generate checkboxes for disabling/enabling events + custom event config
            Tabs[1].AddItems(new OpLabel(260f, 580f, "Active events:", bigText: true)
            { description = "Here you can enable and disable which events can happen." });
            Tabs[2].AddItems(new OpLabel(260f, 580f, "Event config:", bigText: true)
            { description = "Here you can change the configuration of specific events" });

            float eventSBSize = RainWorldCE.CCMode ? CustomChaos.CustomChaos.CCConfig.Length * 15f + 120f : allCEEvents.Count * 30f + 20f;

            eventSB = new OpScrollBox(new Vector2(10f, 10f), new Vector2(580f, 560f), eventSBSize);
            Tabs[1].AddItems(eventSB);
            eventConfigSB = new OpScrollBox(new Vector2(10f, 10f), new Vector2(580f, 560f), 0f);
            Tabs[2].AddItems(eventConfigSB);

            float eventsY = eventSB.contentSize - 40f;
            float configY = eventConfigSB.contentSize - 40f;
            int sbContentSize = 0;

            if (RainWorldCE.CCMode)
            {
                eventSB.AddItems(new OpLabel(180f, eventsY, "CustomChaos active (CC.txt found)."));
                eventsY -= 20f;
                eventSB.AddItems(new OpLabel(140f, eventsY, "Chaos Edition will execute the following orders:")); 
                string[] orders = new string[CustomChaos.CustomChaos.CCConfig.Length];
                eventsY -= 60;
                for (int i = 0; i < CustomChaos.CustomChaos.CCConfig.Length; i++)
                {
                    eventSB.AddItems(new OpLabel(25f, eventsY, $"{i + 1}.   {CustomChaos.CustomChaos.CCConfig[i]} and then"));
                    eventsY -= 15f;
                }
                eventSB.AddItems(new OpLabel(25f, eventsY, $"FINISH"));

            }
            else
            {
                foreach (CEEvent ceevent in allCEEvents)
                {
                    //Event enable/disable checkboxes
                    Configurable<bool> checkboxConfig = eventStatus[$"CEEvent_{ceevent.GetType().Name}"];
                    eventSB.AddItems(new OpCheckBox(checkboxConfig, new Vector2(10f, eventsY))
                    { description = ceevent.Name });
                    eventSB.AddItems(new OpLabel(45f, eventsY, ceevent.Name)
                    { description = ceevent.Name });
                    eventsY -= 30f;
                }
            }
            //Custom event configs
            foreach (CEEvent ceevent in allCEEvents)
            {
                if (ceevent.ConfigEntries.Count > 0)
                {
                    sbContentSize += 60;
                    eventConfigSB.AddItems(new OpLabel(20f, configY, ceevent.Name, bigText: true));
                    configY -= 30f;
                    foreach (EventConfigEntry entry in ceevent.ConfigEntries)
                    {
                        foreach (UIelement element in entry.CMelement(new Vector2(40f, configY)))
                        {
                            eventConfigSB.AddItems(element);
                        }
                        sbContentSize += entry.size;
                        configY -= entry.size;
                    }
                    configY -= 30f;
                }
            }
            eventConfigSB.SetContentSize(sbContentSize);

            //Extras/Debug
            Tabs[3].AddItems(new OpLabel(220f, 580f, "Debug/Extra features:", bigText: true)
            { description = "Debug and extra fun features" });

            //Extra debug logs
            Tabs[3].AddItems(new OpCheckBox(RainWorldCE.debugLogs, new Vector2(10f, 540f)));
            Tabs[3].AddItems(new OpLabel(45f, 540f, "Enable extra debug logs")
            { description = "Log extra debug info. Only enable when debugging the mod!\n May cause decreased performance and bigger log files" });
            //Event trigger key
            Tabs[3].AddItems(new OpLabel(10f, 500f, "Trigger event key:")
            { description = "Key to trigger random event instantly" });
            Tabs[3].AddItems(new OpKeyBinder(RainWorldCE.triggerEventKey, new Vector2(120f, 495f), new Vector2(120f, 15f), collisionCheck: false)
            { description = "Key to trigger random event instantly" });
        }

        private void OpenModFolder(UIfocusable trigger)
        {
            Application.OpenURL($"file://{Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..")}");
        }
    }
}

