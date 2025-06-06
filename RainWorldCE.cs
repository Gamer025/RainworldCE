﻿using BepInEx;
using BepInEx.Logging;
using IL.Menu;
using Menu;
using RainWorldCE.Attributes;
using RainWorldCE.Config;
using RainWorldCE.Config.CustomChaos;
using RainWorldCE.Events;
using RainWorldCE.PostProcessing;
using RainWorldCE.RWHUD;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using UnityEngine;
using Random = System.Random;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace RainWorldCE;

[BepInPlugin(MOD_ID, "Rain World Chaos Edition", "3.1.0")]
public class RainWorldCE : BaseUnityPlugin
{
    public const string MOD_ID = "Gamer025.RainworldCE";
    /// <summary>
    /// Time in seconds since game start
    /// </summary>
    static public int gameTimer = 0;
    /// <summary>
    /// gameTimer value at which last chaos event was triggered
    /// </summary>
    static float CETriggerTime = 0;
    /// <summary>
    /// Time between chaos events
    /// </summary>
    public static Configurable<int> eventTimeout;
    /// <summary>
    /// Offset (negative and positive) applied to eventTimeout to randomize trigger time
    /// </summary>
    public static Configurable<int> eventTimeoutOffset;
    /// <summary>
    /// Percentage of events to not repeat
    /// </summary>
    public static Configurable<int> blockedEventPercent;
    /// <summary>
    /// CEHUD which contains the labels for new events happening and active events
    /// </summary>
    public static CEHUD CEHUD;
    /// <summary>
    /// Are we displaying active events as text to the player? (CM)
    /// </summary>
    public static Configurable<bool> _showActiveEvents;
    public static bool showActiveEvents => _showActiveEvents.Value;
    /// <summary>
    /// Currently active chaos events
    /// </summary>
    static public List<CEEvent> activeEvents = new List<CEEvent>();
    /// <summary>
    /// All CEEVent classes, will be set to all enabled events by CM otherwise all events (CM)
    /// </summary>
    public static List<Type> eventTypes = new List<Type>();
    /// <summary>
    /// Events that can't be triggered because they happened too recently
    /// </summary>
    public static Type[] blockedEvents;
    /// <summary>
    /// Current position in blockedEvents
    /// </summary>
    static int blockedEventCounter = 0;
    /// <summary>
    /// Amount of events already triggered
    /// </summary>
    static int eventCounter = 0;
    /// <summary>
    /// Maximum amount of events to execute per cycle (CM)
    /// </summary>
    static public Configurable<int> maxEventCount;
    static public Configurable<int> _eventDurationMult;
    /// <summary>
    /// Multiplier for event length
    /// </summary>
    static public float eventDurationMult => (float)(_eventDurationMult.Value) / 10;
    /// <summary>
    /// CE will only Update() (create events/call event triggers etc.) if game is active
    /// </summary>
    private static bool CEactive = false;
    /// <summary>
    /// Mod is running in CustomChaos mods and events are file driven
    /// </summary>
    public static bool CCMode = false;
    /// <summary>
    /// Current active seed for current cycle if seeded run, will advanced by 1000 for every cycle in current run
    /// </summary>
    static int currentSeed = -1;

    //Debug/Extra

    /// <summary>
    /// Key for manually triggering a random event
    /// </summary>
    public static Configurable<KeyCode> triggerEventKey;
    /// <summary>
    /// Write additional noisy debug logs
    /// </summary>
    public static Configurable<bool> debugLogs;
    /// <summary>
    /// Optional set seed for event selection
    /// </summary>
    static public Configurable<int> eventSeed;


    //Rainworld game loop
    private RainWorldGame game;
    /// <summary>
    /// BepInEx Plugin Version
    /// </summary>
    public static Version modVersion;

    public static UnityEngine.AssetBundle CEAssetBundle;
    static  Random rnd = new Random();

    public RainWorldCE()
    {
        __me = new(this);
        BepInPlugin attribute =
            (BepInPlugin)Attribute.GetCustomAttribute(typeof(RainWorldCE), typeof(BepInPlugin));
        modVersion = attribute.Version;
    }
    //Logging
    private static WeakReference __me;
    public static RainWorldCE ME => __me?.Target as RainWorldCE;
    public ManualLogSource Logger_p => Logger;

    public void OnEnable()
    {
        On.RainWorld.OnModsInit += OnModsInitHook;
    }

    bool initDone = false;
    private void OnModsInitHook(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        if (!initDone)
        {
            ME.Logger_p.Log(LogLevel.Info, "RainWorldCE init");
            try
            {
                MachineConnector.SetRegisteredOI(MOD_ID, new RainWorldCEOI());
            }
            catch (Exception e)
            {
                ME.Logger_p.Log(LogLevel.Error, $"Error creating options interface:\n {e}");
            }
            //Used for starting up everything
            On.RainWorldGame.ctor += RainWorldGameCtorHook;
            //Used for key input
            On.RainWorldGame.Update += RainWorldGameUpdateHook;
            //Triggers for resetting CEs state
            On.RainWorldGame.ExitGame += RainWorldGameExitGameHook;
            On.RainWorldGame.Win += RainWorldGameWinHook;
            //Used to pause CE on process switch (death screen ...)
            On.ProcessManager.RequestMainProcessSwitch_ProcessID += ProcessManagerRequestMainProcessSwitchHook;
            //Add own HUD to the game
            On.HUD.HUD.InitSinglePlayerHud += HUDInitSinglePlayerHudHook;
            //Needed for fixing teleports
            On.ShortcutHandler.TeleportingCreatureArrivedInRealizedRoom += ShortcutHandler_TeleportingCreatureArrivedInRealizedRoom;

            //Used for creating PostProcessing container
            On.RoomCamera.ctor += RoomCameraExtension.RoomCameraCtorHook;
            //Used for updating PostProcessing effects
            On.RoomCamera.DrawUpdate += RoomCameraExtension.RoomCameraDrawUpdateHook;
            //Used as trigger for PlayerChangingRoomTrigger
            On.ShortcutHandler.SuckInCreature += ShortcutHandlerSuckInCreatureHook;
            //Used as trigger for PlayerChangedRoomTrigger
            On.RoomCamera.ChangeRoom += RoomCameraChangeRoomHook;


            //Load asset bundle containing shaders, can only loaded once otherwise error
            try
            {
                CEAssetBundle = UnityEngine.AssetBundle.LoadFromFile(AssetManager.ResolveFilePath("AssetBundles/gamer025.rainworldce.assets"));
                if (CEAssetBundle == null)
                {
                    ME.Logger_p.Log(LogLevel.Error, $"RainWorldCE: Failed to load AssetBundle from {AssetManager.ResolveFilePath("AssetBundles/gamer025.rainworldce.assets")}");
                    Destroy(this);
                }
                ME.Logger_p.Log(LogLevel.Debug, $"Assetbundle content: {String.Join(", ", CEAssetBundle.GetAllAssetNames())}");
                self.Shaders.Add("FlipScreenPP", FShader.CreateShader("FlipScreenPP", CEAssetBundle.LoadAsset<UnityEngine.Shader>("flipscreen.shader")));
                self.Shaders.Add("MeltingPP", FShader.CreateShader("MeltingPP", CEAssetBundle.LoadAsset<UnityEngine.Shader>("melt.shader")));
                self.Shaders.Add("PixelizePP", FShader.CreateShader("PixelizePP", CEAssetBundle.LoadAsset<UnityEngine.Shader>("pixelize.shader")));
            }
            catch (Exception e)
            {
                ME.Logger_p.Log(LogLevel.Error, $"Error loading asset bundle:\n {e}");
            }
            initDone = true;
        }
    }

    //Start at -1 to give the game some time to start up fully
    static float timepool = -1;
    /// <summary>
    /// Used to drive SecondUpdate if the main game is active
    /// Doesn't really on the games internal updates/timing
    /// </summary>
    void Update()
    {
        //Only tick if the game seems to be running and is in story mode
        if (CEactive && game is not null && game.IsStorySession && game.pauseMenu == null && game.AllowRainCounterToTick() && game.manager.currentMainLoop is RainWorldGame)
        {
            timepool += Time.deltaTime;
            //We only need second precision, so lets only do stuff every full second
            if (timepool > 1)
            {
                timepool--;
                if (debugLogs.Value)
                    Logger_p.Log(LogLevel.Debug, $"{DateTime.Now:HH:mm:ss:fff} Calling SecondUpdate [{gameTimer}]");
                SecondUpdate();
            }
            //Reset if overtiming
            if (timepool > 2)
            {
                timepool = 0;
            }
        }
    }

    bool triggerEventDown = false;
    private void RainWorldGameUpdateHook(On.RainWorldGame.orig_Update orig, RainWorldGame self)
    {
        orig(self);
        bool triggerEvent = Input.GetKey(triggerEventKey.Value);
        if (triggerEvent && !triggerEventDown)
        {
            Logger_p.Log(LogLevel.Info, $"Triggering event from keypress");
            CreateNewEvent();
        }
        triggerEventDown = triggerEvent;
    }

    /// <summary>
    /// Creates and enables a new <see cref="CEEvent">chaos event</see>
    /// </summary>
    void CreateNewEvent()
    {
        Type eventClass = PickEvent();
        eventCounter++;
        CEEvent selectedEvent = (CEEvent)Activator.CreateInstance(eventClass);
        Logger_p.Log(LogLevel.Info, $"Triggering '{selectedEvent.Name}' event");
        ActivateEvent(selectedEvent);
    }

    public static void ActivateEvent(CEEvent ceevent)
    {
        if (ceevent.ImplementsMethod("StartupTrigger"))
        {
            ME.Logger_p.Log(LogLevel.Debug, $"Calline StartupTrigger of '{ceevent.Name}' event");
            try
            {
                ceevent.StartupTrigger();
            }
            catch (Exception e)
            {
                ME.Logger_p.Log(LogLevel.Error, $"'{ceevent.Name}' errored on startup, cancel event");
                ME.Logger_p.Log(LogLevel.Error, e.ToString());
                CEHUD.StopEventSelection(ceevent);
                return;
            }
        }
        //Stop the selecton after calling startup so that event can change its name/description before its displayed
        CEHUD.StopEventSelection(ceevent);
        if (ceevent.ActiveTime > 0)
        {
            activeEvents.Add(ceevent);
            if (showActiveEvents) CEHUD.AddActiveEvent(ceevent.Name);
            if (ceevent.RepeatEverySec > 0)
            {
                ceevent.RecurringEventTime = gameTimer;
            }
        }
    }

    public static Type PickEvent()
    {
        Type eventClass;
        if (eventTypes.Count == 0)
        {
            eventClass = typeof(NoEventsEnabled);
            //Disalbe CE since we have nothign to do
            CEactive = false;
            return eventClass;
        }

        //All events that aren't blocked or already active while not allowing multiple
        List<Type> allowedEvents = eventTypes.Except(blockedEvents).ToList();
        RemoveAlreadyActiveEvents(ref allowedEvents);

        //Can't find any possible event to trigger
        if (allowedEvents.Count == 0)
        {
            //Try again with normally blocked events included
            ME.Logger_p.Log(LogLevel.Info, $"All non blocked events already active, trying again with blocked events included");
            allowedEvents = eventTypes.ToList();
            RemoveAlreadyActiveEvents(ref allowedEvents);
        }

        //Still can't find possible event, even with blocked events included
        if (allowedEvents.Count == 0)
        {
            ME.Logger_p.Log(LogLevel.Warning, $"All enabled events already active, can't create new event");
            eventClass = typeof(AllEventsFiltered);
            //Disalbe CE since we have nothign to do
            return eventClass;
        }

        eventClass = allowedEvents[rnd.Next(allowedEvents.Count)];
        //Should fill up the array and then start overwriting the oldest blocked event
        if (blockedEvents.Length > 0)
        {
            blockedEvents[blockedEventCounter] = eventClass;
            blockedEventCounter = (blockedEventCounter + 1) % blockedEvents.Length;
            if (debugLogs.Value)
                ME.Logger_p.Log(LogLevel.Debug, $"Blocked events: {String.Join(",", blockedEvents.OfType<Type>().Select(x => x.ToString()).ToArray())}");
        }
        return eventClass;
    }

    private static void RemoveAlreadyActiveEvents(ref List<Type> allowedEvents)
    {
        for (int i = allowedEvents.Count - 1; i >= 0; i--)
        {
            Type allowedEvent = allowedEvents[i];
            foreach (CEEvent activeEvent in activeEvents)
            {
                if (activeEvent.AllowMultiple == false && allowedEvent == activeEvent.GetType())
                {
                    allowedEvents.Remove(allowedEvent);
                }
            }
        }
    }

    int currentEventOffset = 0;
    /// <summary>
    /// Runs every second and triggers new chaos events / RecurringTrigger methods and expiring events 
    /// </summary>
    void SecondUpdate()
    {
        //Process active events first bevore potentially spawning a new event
        //This prevents a newly triggered events timmer getting ticked down while it isn't even active
        foreach (CEEvent activeEvent in activeEvents.Where(x => !x.expired))
        {
            //Enough time has passed till last RecurringTrigger() call
            if (activeEvent.RepeatEverySec > 0 && gameTimer - activeEvent.RecurringEventTime >= activeEvent.RepeatEverySec)
            {
                Logger_p.Log(LogLevel.Debug, $"Calling RecurringTrigger of {activeEvent.Name}");
                try
                {
                    activeEvent.RecurringTrigger();
                }
                catch (Exception e)
                {
                    Logger_p.Log(LogLevel.Error, $"'{activeEvent.Name}' caused error during RecurringTrigger execution, removing event.");
                    Logger_p.Log(LogLevel.Error, e.ToString());
                    ShutdownCEEvent(activeEvent);
                }
                activeEvent.RecurringEventTime = gameTimer;
            }
            activeEvent.ActiveTime--;
        }

        //Either trigger events as defined in file or randomly picked
        if (CCMode)
        {
            CustomChaos.CustomChaosUpdate();
        }
        else
        {
            //Only create new event if we didn't yet reach the limit for the cycle
            if (eventCounter < maxEventCount.Value)
            {
                //Start the event selection HUD magic 3 seconds before the actual event
                if (gameTimer - (CETriggerTime - 3) >= eventTimeout.Value + currentEventOffset)
                {
                    CEHUD.StartEventSelection();
                }
                //Enough time has passed till last chaos event
                if (gameTimer - CETriggerTime >= eventTimeout.Value + currentEventOffset)
                {
                    CreateNewEvent();
                    CETriggerTime = gameTimer;
                    currentEventOffset = rnd.Next(-eventTimeoutOffset.Value, eventTimeoutOffset.Value);
                }
            }
        }

        //Deal with expired events
        foreach (CEEvent ceevent in activeEvents.Where(ceevent => ceevent.ActiveTime <= 0))
        {
            ShutdownCEEvent(ceevent);
        }

        activeEvents = activeEvents.Where(x => !x.expired).ToList();

        gameTimer++;
    }

    /// <summary>
    /// Reset CEs state by clearing timers, active events etc.
    /// Also deals with ShutdownTrigger of events
    /// </summary>
    void ResetState()
    {
        Logger_p.Log(LogLevel.Debug, $"Resetting state");
        CEactive = false;
        gameTimer = 0;
        CETriggerTime = 0;
        eventCounter = 0;
        CustomChaos.step = 0;
        CustomChaos.nextTrigger = 0;
        //Call the shutdown event of active events before we remove the reference to the game
        //In theory most events shouldn't need to cleanup since world is getting rebuild but some might have hooked stuff and need to undo these hooks
        foreach (var ceevent in activeEvents.Where(ceevent => ceevent.ImplementsMethod("ShutdownTrigger")))
        {
            ShutdownCEEvent(ceevent);
        }
        activeEvents.Clear();
        CEEvent.game = null;
    }

    /// <summary>
    /// Safely tries to shutdown a event, it will be removed from activeEvents on next SecondUpdate run
    /// </summary>
    /// <param name="ceevent">Event to shutdown</param>
    void ShutdownCEEvent(CEEvent ceevent)
    {
        try
        {
            if (ceevent.ImplementsMethod("ShutdownTrigger"))
            {
                ME.Logger_p.Log(LogLevel.Debug, $"Calline ShutdownTrigger of '{ceevent.Name}' event");
                ceevent.ShutdownTrigger();
            }
        }
        catch (Exception e)
        {
            ME.Logger_p.Log(LogLevel.Error, $"Error during '{ceevent.Name}' shutdown");
            ME.Logger_p.Log(LogLevel.Error, e.ToString());

        }
        finally
        {
            ceevent.expired = true;
            if (showActiveEvents) CEHUD.RemoveActiveEvent(ceevent.Name);
        }
    }

    public static void TryLoadCC()
    {
        string configFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..\\CC.txt");
        ME.Logger_p.Log(LogLevel.Info, $"Checking for CC.txt at {configFile}");
        CCMode = File.Exists(configFile);
        ME.Logger_p.Log(LogLevel.Info, $"CCMode is {CCMode}");
        if (CCMode)
        {
            CustomChaos.parseConfig(File.ReadAllLines(configFile));
        }
    }

    #region Hooks
    void RainWorldGameCtorHook(On.RainWorldGame.orig_ctor orig, RainWorldGame self, ProcessManager manager)
    {
        orig(self, manager);

        if (currentSeed != -1)
        {
            ME.Logger_p.Log(LogLevel.Info, $"Seeding event cycle with {currentSeed} as seed");
            rnd = new Random(currentSeed);
        }

        TryLoadCC();
        ME.Logger_p.Log(LogLevel.Info, $"Starting event cycle with {eventTimeout.Value} second timer");
        ResetState();
        CEEvent.game = self;
        game = self;

        //Determine enabled events
        eventTypes.Clear();
        foreach (KeyValuePair<string, Configurable<bool>> entry in RainWorldCEOI.eventStatus)
        {
            bool enabled = entry.Value.Value;
            ME.Logger_p.Log(LogLevel.Debug, $"Setting {entry.Key} to: {enabled} from Remix");
            if (enabled)
            {
                eventTypes.Add(Type.GetType($"RainWorldCE.Events.{entry.Key.Replace("CEEvent_", "")}"));
            }
        }

        //Calculate blocked event count
        if (eventTypes.Count > 1)
        {
            int blockCount =
                Convert.ToInt32(Math.Min(
                    (double)eventTypes.Count * blockedEventPercent.Value / 100,
                    (double)eventTypes.Count - 1));
            if (blockedEvents is null || blockedEvents.Length != blockCount)
            {
                ME.Logger_p.Log(LogLevel.Debug, $"Setting blockedEvents to {blockCount} from Remix");
                blockedEvents = new Type[blockCount];
                blockedEventCounter = 0;
            }
        }
        else
        {
            ME.Logger_p.Log(LogLevel.Debug, $"Less than two enabled events, setting blockedEvents to 0 from Remix");
            blockedEvents = Array.Empty<Type>();
        }

        currentEventOffset = rnd.Next(-eventTimeoutOffset.Value, eventTimeoutOffset.Value);
        CEactive = true;
    }

    void RainWorldGameExitGameHook(On.RainWorldGame.orig_ExitGame orig, RainWorldGame self, bool asDeath, bool asQuit)
    {
        ResetState();
        orig(self, asDeath, asQuit);
    }

    void RainWorldGameWinHook(On.RainWorldGame.orig_Win orig, RainWorldGame self, bool malnourished, bool fromWarpPoint)
    {
        ResetState();
        orig(self, malnourished, fromWarpPoint);
    }

    void ProcessManagerRequestMainProcessSwitchHook(On.ProcessManager.orig_RequestMainProcessSwitch_ProcessID orig, ProcessManager self, ProcessManager.ProcessID ID)
    {
        //Disable CE if game not in game mode
        if (ID != ProcessManager.ProcessID.Game)
            CEactive = false;
        else
            CEactive = true;

        //Make sure seed is inactive, for exampel after settings where changed
        if (eventSeed.Value == 0)
            currentSeed = -1;

        //Increment seed so multiple cycles in one run aren't the same after karma screen
        if (self.currentMainLoop is Menu.KarmaLadderScreen && currentSeed != -1)
        {
            ME.Logger_p.Log(LogLevel.Info, $"Incrementing currentSeed by 1000, currently at {currentSeed}");
            currentSeed = Math.Min(currentSeed + 1000, int.MaxValue);
        }
        else if (eventSeed.Value != 0 && ID == ProcessManager.ProcessID.Game)
        {
            currentSeed = eventSeed.Value;
        }
        //Blocked events always get reset each cycle in seeded runs for consistency
        if (currentSeed != -1 && ID == ProcessManager.ProcessID.Game)
        {
            ME.Logger_p.Log(LogLevel.Info, $"Resetting blocked events because seeded run");
            if (blockedEvents is not null)
                Array.Clear(blockedEvents, 0, blockedEvents.Length);
            blockedEventCounter = 0;
        }
        orig(self, ID);
    }

    void ShortcutHandlerSuckInCreatureHook(On.ShortcutHandler.orig_SuckInCreature orig, ShortcutHandler self, Creature creature, Room room, ShortcutData shortCut)
    {
        foreach (CEEvent activeEvent in activeEvents)
        {
            if (activeEvent.ImplementsMethod("PlayerChangingRoomTrigger") && creature is Player && shortCut.shortCutType == ShortcutData.Type.RoomExit)
            {
                try
                {
                    activeEvent.PlayerChangingRoomTrigger(ref self, ref creature, ref room, ref shortCut);
                }
                catch (Exception e)
                {
                    ME.Logger_p.Log(LogLevel.Error, $"Error during '{activeEvent.Name}' PlayerChangingRoomTrigger, removing event");
                    ME.Logger_p.Log(LogLevel.Error, e.ToString());
                    ShutdownCEEvent(activeEvent);
                }

            }
        }
        orig(self, creature, room, shortCut);
    }

    void RoomCameraChangeRoomHook(On.RoomCamera.orig_ChangeRoom orig, RoomCamera self, Room room, int camPos)
    {
        orig(self, room, camPos);
        //Check if room has important story elements and pause if true
        CEactive = true;
        for (int i = 0; i < room.physicalObjects.Length; i++)
        {
            for (int j = 0; j < room.physicalObjects[i].Count; j++)
            {
                if (room.physicalObjects[i][j] is Oracle)
                {
                    CEactive = false;
                }
            }
        }
        foreach (IDrawable drawable in room.drawableObjects)
        {
            if (drawable is Ghost)
            {
                CEactive = false;
            }
        }

        foreach (CEEvent activeEvent in activeEvents)
        {
            if (activeEvent.ImplementsMethod("PlayerChangedRoomTrigger"))
            {
                try
                {
                    activeEvent.PlayerChangedRoomTrigger(ref self, ref room, ref camPos);
                }
                catch (Exception e)
                {
                    Logger_p.Log(LogLevel.Error, $"Error during '{activeEvent.Name}' PlayerChangedRoomTrigger, removing event");
                    Logger_p.Log(LogLevel.Error, e.ToString());
                    ShutdownCEEvent(activeEvent);
                }
            }
        }
    }

    void HUDInitSinglePlayerHudHook(On.HUD.HUD.orig_InitSinglePlayerHud orig, HUD.HUD self, RoomCamera cam)
    {
        self.AddPart(new CEHUD(self, activeEvents));
        orig(self, cam);
    }

    //Thanks to Henpemaz for providing this code and explaining the issue to me
    //https://github.com/henpemaz/PartModPartMeme/blob/master/MapWarp/MapWarp.cs#L282
    private void ShortcutHandler_TeleportingCreatureArrivedInRealizedRoom(On.ShortcutHandler.orig_TeleportingCreatureArrivedInRealizedRoom orig, ShortcutHandler self, ShortcutHandler.TeleportationVessel tVessel)
    {
        try
        {
            orig(self, tVessel);
        }
        catch (System.NullReferenceException)
        {
            if (tVessel.creature is not ITeleportingCreature)
            {
                WorldCoordinate arrival = tVessel.destination;
                if (!arrival.TileDefined)
                {
                    arrival = tVessel.room.realizedRoom.LocalCoordinateOfNode(tVessel.entranceNode);
                    arrival.abstractNode = tVessel.entranceNode;
                }

                tVessel.creature.abstractCreature.pos = arrival;
                tVessel.creature.SpitOutOfShortCut(arrival.Tile, tVessel.room.realizedRoom, true);
            }
        }
    }

    #endregion

    /// <summary>
    /// Get all children classes of a type that aren't abstract and in the same namespace
    /// </summary>
    /// <typeparam name="T">Parent class</typeparam>
    /// <returns>Children types</returns>
    public static IEnumerable<Type> GetEnumerableOfType<T>() where T : class
    {
        IEnumerable<Type> objects =
            Assembly.GetAssembly(typeof(T)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T)));
        return objects;
    }

    /// <summary>
    /// Get all chaos events that are valid for random selection
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Type> GetAllCEEventTypes()
    {
        IEnumerable<Type> eventTypes =
            GetEnumerableOfType<CEEvent>().Where(x => !x.IsDefined(typeof(InternalCEEventAttribute), true));
        if (!ModManager.MSC)
            eventTypes = eventTypes.Where(x => !x.IsDefined(typeof(MSCEventAttribute), true));
        return eventTypes;
    }
}