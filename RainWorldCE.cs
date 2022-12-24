using BepInEx;
using BepInEx.Logging;
using OptionalUI;
using System;
using RainWorldCE.Events;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RainWorldCE.RWHUD;
using RainWorldCE.Config;
using System.ComponentModel.Design;
using RainWorldCE.Attributes;

namespace RainWorldCE;

[BepInPlugin("Gamer025.RainworldCE", "Rain World Chaos Edition", "1.1.0")]
public class RainWorldCE : BaseUnityPlugin
{
    /// <summary>
    /// Time in seconds since game start
    /// </summary>
    static public int gameTimer = 0;
    /// <summary>
    /// gameTimer value at which last chaos event was triggered
    /// </summary>
    static float CETriggerTime = 0;
    /// <summary>
    /// Time between chaos events (CM)
    /// </summary>
    public static int eventTimeout = 60;
    /// <summary>
    /// CEHUD which contains the labels for new events happening and active events
    /// </summary>
    public static CEHUD CEHUD;
    /// <summary>
    /// Are we displaying active events as text to the player? (CM)
    /// </summary>
    public static bool showActiveEvents = true;
    /// <summary>
    /// Currently active chaos events
    /// </summary>
    static public List<CEEvent> activeEvents = new List<CEEvent>();
    /// <summary>
    /// All CEEVent classes, will be set to all enabled events by CM otherwise all events (CM)
    /// </summary>
    public static List<Type> eventTypes;
    /// <summary>
    /// Events that can't be triggered because they happened too recently
    /// </summary>
    public static Type[] blockedEvents;
    /// <summary>
    /// Amount of events already triggered
    /// </summary>
    static int eventCounter = 0;
    /// <summary>
    /// Maximum amount of events to execute per cycle (CM)
    /// </summary>
    static public int maxEventCount = 1000;
    /// <summary>
    /// Multiplier for event length
    /// </summary>
    static public float eventDurationMult = 1;
    /// <summary>
    /// CE will only Update() (create events/call event triggers etc.) if game is active
    /// </summary>
    private static bool gameRunning = false;
    //Rainworld game loop
    private RainWorldGame game;
    /// <summary>
    /// This is us
    /// </summary>
    public static RainWorldCE instance;
    /// <summary>
    /// BepInEx Plugin Version
    /// </summary>
    public static Version modVersion;
    static readonly Random rnd = new Random();

    //Mod options menu
    public static OptionInterface LoadOI()
    {
        return new RainWorldCEOI();
    }

    public RainWorldCE()
    {
        __me = new(this);
        instance = this;
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
        //Used for starting up everything
        On.RainWorldGame.ctor += RainWorldGameCtorHook;
        //Triggers for resetting CEs state
        On.RainWorldGame.ExitGame += RainWorldGameExitGameHook;
        On.RainWorldGame.Win += RainWorldGameWinHook;
        //Add own HUD to the game
        On.HUD.HUD.InitSinglePlayerHud += HUDInitSinglePlayerHudHook;

        //Used as trigger for PlayerChangingRoomTrigger
        On.ShortcutHandler.SuckInCreature += ShortcutHandlerSuckInCreatureHook;
        //Used as trigger for PlayerChangedRoomTrigger
        On.RoomCamera.ChangeRoom += RoomCameraChangeRoomHook;

        //Load all events in case CM hasn't already populated the list
        eventTypes ??= RainWorldCE.GetAllCEEventTypes().OrderBy(x => x.Name).ToList();
        //Set event repetition blocker to 25% as default in case CM doesn't override it
        blockedEvents = new Type[Convert.ToInt32(Math.Min((double)RainWorldCE.eventTypes.Count * 25 / 100, (double)RainWorldCE.eventTypes.Count - 1))];
    }

    static float timepool = 0;
    void Update()
    {
        //Only tick if the game seems to be running and is in story mode
        if (gameRunning && game.IsStorySession && game.pauseMenu == null && game.AllowRainCounterToTick())
        {
            timepool += UnityEngine.Time.deltaTime;
            //We only need second precision, so lets only do stuff every full second
            if (timepool > 1)
            {
                timepool--;
                RainWorldCE.ME.Logger_p.Log(LogLevel.Debug, $"{DateTime.Now:HH:mm:ss:fff} Calling SecondUpdate [{gameTimer}]");
                SecondUpdate();
                gameTimer++;
            }
            //Reset if overtiming
            if (timepool > 2)
            {
                timepool = 0;
            }
        }

    }

    /// <summary>
    /// Creates and enables a new <see cref="CEEvent">chaos event</see>
    /// </summary>
    void CreateNewEvent()
    {
        Type eventClass = PickEvent();
        eventCounter++;
        CEEvent selectedEvent = (CEEvent)Activator.CreateInstance(eventClass);
        RainWorldCE.ME.Logger_p.Log(LogLevel.Info, $"Triggering '{selectedEvent.Name}' event");
        CEHUD.StopEventSelection(selectedEvent);
        if (selectedEvent.ImplementsMethod("StartupTrigger"))
        {
            RainWorldCE.ME.Logger_p.Log(LogLevel.Debug, $"Calline StartupTrigger of '{selectedEvent.Name}' event");
            try
            {
                selectedEvent.StartupTrigger();
            }
            catch (Exception e)
            {
                RainWorldCE.ME.Logger_p.Log(LogLevel.Error, $"'{selectedEvent.Name}' errored on startup, cancel event");
                RainWorldCE.ME.Logger_p.Log(LogLevel.Error, e.ToString());
                return;
            }
        }
        if (selectedEvent.ActiveTime > 0)
        {
            activeEvents.Add(selectedEvent);
            if (showActiveEvents) CEHUD.AddActiveEvent(selectedEvent.Name);
            if (selectedEvent.RepeatEverySec > 0)
            {
                selectedEvent.RecurringEventTime = gameTimer;
            }
        }
    }

    public Type PickEvent()
    {
        Type eventClass;
        if (eventTypes.Count == 0)
        {
            eventClass = typeof(NoEventsEnabled);
            //Disalbe CE since we have nothign to do
            gameRunning = false;
            return eventClass;
        }

        //All events that aren't blocked or already active while not allowing multiple
        List<Type> allowedEvents = eventTypes.Except(blockedEvents).ToList();
        RemoveAlreadyActiveEvents(ref allowedEvents);

        //Can't find any possible event to trigger
        if (allowedEvents.Count == 0)
        {
            //Try again with normally blocked events included
            RainWorldCE.ME.Logger_p.Log(LogLevel.Info, $"All non blocked events already active, trying again with blocked events included");
            allowedEvents = eventTypes.ToList();
            RemoveAlreadyActiveEvents(ref allowedEvents);
        }

        //Still can't find possible event, even with blocked events included
        if (allowedEvents.Count == 0)
        {
            RainWorldCE.ME.Logger_p.Log(LogLevel.Warning, $"All enabled events already active, can't create new event");
            eventClass = typeof(AllEventsFiltered);
            //Disalbe CE since we have nothign to do
            return eventClass;
        }

        eventClass = allowedEvents[rnd.Next(allowedEvents.Count)];
        //Should fill up the array and then start overwriting the oldest blocked event
        if (blockedEvents.Length > 0)
        {
            blockedEvents[eventCounter % blockedEvents.Length] = eventClass;
            RainWorldCE.ME.Logger_p.Log(LogLevel.Debug, $"Blocked events: {String.Join(",", blockedEvents.OfType<Type>().Select(x => x.ToString()).ToArray())}");
        }
        return eventClass;
    }

    private void RemoveAlreadyActiveEvents(ref List<Type> allowedEvents)
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

    /// <summary>
    /// Add default values to CEEvents config if no value provided by CM
    /// </summary>
    private void GenerateDefaultConfigs()
    {
        List<CEEvent> events = GetAllCEEventTypes().Select(x => (CEEvent)Activator.CreateInstance(x)).OrderBy(e => e.Name).ToList();
        //Add to config if key not already exist
        foreach (var entry in events.SelectMany(ceevent => ceevent.ConfigEntries.Where(entry => !CEEvent.config.ContainsKey(entry.Key))))
        {
            CEEvent.config.Add(entry.Key, entry.DefaultValue);
        }
    }


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
                RainWorldCE.ME.Logger_p.Log(LogLevel.Debug, $"Calling RecurringTrigger of {activeEvent.Name}");
                try
                {
                    activeEvent.RecurringTrigger();
                }
                catch (Exception e)
                {
                    RainWorldCE.ME.Logger_p.Log(LogLevel.Error, $"'{activeEvent.Name}' caused error during RecurringTrigger execution, removing event.");
                    RainWorldCE.ME.Logger_p.Log(LogLevel.Error, e.ToString());
                    ShutdownCEEvent(activeEvent);
                }
                activeEvent.RecurringEventTime = gameTimer;
            }
            activeEvent.ActiveTime--;
        }

        //Only create new event if we didn't yet reach the limit for the cycle
        if (eventCounter < maxEventCount)
        {
            //Start the event selection HUD magic 3 seconds before the actual event
            if (gameTimer - (CETriggerTime - 3) >= eventTimeout)
            {
                CEHUD.StartEventSelection();
            }
            //Enough time has passed till last chaos event
            if (gameTimer - CETriggerTime >= eventTimeout)
            {
                CreateNewEvent();
                CETriggerTime = gameTimer;
            }
        }

        //Deal with expired events
        foreach (CEEvent ceevent in activeEvents.Where(ceevent => ceevent.ActiveTime <= 0))
        {
            ShutdownCEEvent(ceevent);
        }

        activeEvents = activeEvents.Where(x => !x.expired).ToList();

    }

    /// <summary>
    /// Reset CEs state by clearing timers, active events etc.
    /// Also deals with ShutdownTrigger of events
    /// </summary>
    void ResetState()
    {
        RainWorldCE.ME.Logger_p.Log(LogLevel.Debug, $"Resetting state");
        gameRunning = false;
        gameTimer = 0;
        CETriggerTime = 0;
        eventCounter = 0;
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
                RainWorldCE.ME.Logger_p.Log(LogLevel.Debug, $"Calline ShutdownTrigger of '{ceevent.Name}' event");
                ceevent.ShutdownTrigger();
            }
        }
        catch (Exception e)
        {
            RainWorldCE.ME.Logger_p.Log(LogLevel.Error, $"Error during '{ceevent.Name}' shutdown");
            RainWorldCE.ME.Logger_p.Log(LogLevel.Error, e.ToString());

        }
        finally
        {
            ceevent.expired = true;
            if (showActiveEvents) CEHUD.RemoveActiveEvent(ceevent.Name);
        }
    }

    #region Hooks
    void RainWorldGameCtorHook(On.RainWorldGame.orig_ctor orig, RainWorldGame self, ProcessManager manager)
    {
        orig(self, manager);
        RainWorldCE.ME.Logger_p.Log(LogLevel.Info, $"Starting event cycle with {eventTimeout} second timer");
        ResetState();
        CEEvent.game = self;
        game = self;
        gameRunning = true;
        //At this point CM should have defenitly loaded its config if it exists, so its safe to add defaults now
        GenerateDefaultConfigs();
    }

    void RainWorldGameExitGameHook(On.RainWorldGame.orig_ExitGame orig, RainWorldGame self, bool asDeath, bool asQuit)
    {
        ResetState();
        orig(self, asDeath, asQuit);
    }

    void RainWorldGameWinHook(On.RainWorldGame.orig_Win orig, RainWorldGame self, bool malnourished)
    {
        ResetState();
        orig(self, malnourished);
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
                    RainWorldCE.ME.Logger_p.Log(LogLevel.Error, $"Error during '{activeEvent.Name}' PlayerChangingRoomTrigger, removing event");
                    RainWorldCE.ME.Logger_p.Log(LogLevel.Error, e.ToString());
                    ShutdownCEEvent(activeEvent);
                }

            }
        }
        orig(self, creature, room, shortCut);
    }

    void RoomCameraChangeRoomHook(On.RoomCamera.orig_ChangeRoom orig, RoomCamera self, Room room, int camPos)
    {
        orig(self, room, camPos);
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
                    RainWorldCE.ME.Logger_p.Log(LogLevel.Error, $"Error during '{activeEvent.Name}' PlayerChangedRoomTrigger, removing event");
                    RainWorldCE.ME.Logger_p.Log(LogLevel.Error, e.ToString());
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

    public static IEnumerable<Type> GetAllCEEventTypes()
    {
        IEnumerable<Type> eventTypes =
            GetEnumerableOfType<CEEvent>().Where(x => !x.IsDefined(typeof(InternalCEEvent), true));
        return eventTypes;
    }
}