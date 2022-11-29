using BepInEx.Configuration;
using BepInEx.Logging;
using RainWorldCE.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Reference implementation of a chaos event
    /// </summary>
    abstract public class CEEvent
    {
        #region eventProps
        //Properties related to event configuration
        //Set their backing field in your events constructor

        protected string _name;
        /// <summary>
        /// Display Name of the event
        /// </summary>
        public string Name { get => _name; set => _name = value; }

        protected string _description;
        /// <summary>
        /// Description of the event
        /// </summary>
        public string Description { get => _description; set => _description = value; }

        protected int _repeatEverySec;
        /// <summary>
        /// How often RecurringTrigger should be called in seconds
        /// </summary>
        public int RepeatEverySec => _repeatEverySec;

        protected int _activeTime = 0;
        /// <summary>
        /// How long the event will be active for, will tick down every second and event will be deleted on 0
        /// </summary>
        public int ActiveTime { get => _activeTime; set => _activeTime = value; }


        protected bool _allowMultiple = false;
        /// <summary>
        /// Controls if an event can be active multiple times
        /// Only relevant to events with <see cref="ActiveTime">ActiveTime</see> set
        /// </summary>
        public bool AllowMultiple { get => _allowMultiple; set => _allowMultiple = value; }

        /// <summary>
        /// List of config menu entries that should be added to CM if present
        /// Override this prop to add config elements to your event
        /// </summary>
        public virtual List<EventConfigEntry> ConfigEntries
        {
            get
            {
                return new List<EventConfigEntry>();
            }
        }
        #endregion

        #region helperFields
        //Fields you can use in your event, don't modify these
        public static RainWorldGame game;
        public static readonly Random rnd = new Random();
        public static EventHelpers helper;
        #endregion

        #region internalFields
        public static Dictionary<string, string> config = new Dictionary<string, string>();
        public bool expired = false;
        protected float _recurringEventTime = 0;
        /// <summary>
        /// gameTimer value at which <see cref="RecurringTrigger">RecurringTrigger</see> was called
        /// </summary>
        public float RecurringEventTime { get => _recurringEventTime; set => _recurringEventTime = value; }
        #endregion

        #region Triggers
        /// <summary>
        /// Instantly executed after event gets selected
        /// </summary>
        public virtual void StartupTrigger()
        {
            RainWorldCE.ME.Logger_p.Log(LogLevel.Error, $"Abstract event StartupTrigger called by {GetType().Name}!");
        }

        /// <summary>
        /// Executed when the event is being removed from the active event pool
        /// </summary>
        /// <param name="gameReset">Game has been reset</param>
        public virtual void ShutdownTrigger()
        {
            RainWorldCE.ME.Logger_p.Log(LogLevel.Error, $"Abstract event ShutdownTrigger called by {GetType().Name}!");
        }

        /// <summary>
        /// Executed every <see cref="RepeatEverySec">repeatEverySec</see> seconds
        /// </summary>
        public virtual void RecurringTrigger()
        {
            RainWorldCE.ME.Logger_p.Log(LogLevel.Error, $"Abstract event RecurringTrigger called by {GetType().Name}!");
        }

        /// <summary>
        /// Executed when room is about to change (SuckInCreature to be exact)
        /// </summary>
        public virtual void PlayerChangingRoomTrigger(ref ShortcutHandler self, ref Creature creature, ref Room room, ref ShortcutData shortCut)
        {
            RainWorldCE.ME.Logger_p.Log(LogLevel.Error, $"Abstract event PlayerChangingRoomTrigger called by {GetType().Name}!");
        }

        /// <summary>
        /// Executed when the player changed room (RoomCamera.ChangeRoom to be exact)
        /// </summary>
        public virtual void PlayerChangedRoomTrigger(ref RoomCamera self, ref Room room, ref int camPos)
        {
            RainWorldCE.ME.Logger_p.Log(LogLevel.Error, $"Abstract event PlayerChangedRoomTrigger called by {GetType().Name}!");
        }

        #endregion

        /// <summary>
        /// Create an log entry for your event
        /// </summary>
        /// <param name="level">The log level </param>
        /// <param name="message">Message to print/log</param>
        protected void WriteLog(LogLevel level, string message)
        {
            RainWorldCE.ME.Logger_p.Log(LogLevel.Debug, $"[{GetType().Name}] {message}");
        }

        /// <summary>
        /// Get config value for specified key
        /// </summary>
        /// <param name="key">Config identifier</param>
        /// <returns>Value as string if found otherwise null</returns>
        protected string GetConfig(string key)
        {
            return config.ContainsKey($"EC_{GetType().Name}_{key}") ? config[$"EC_{GetType().Name}_{key}"] : null;
        }

        /// <summary>
        /// Tries to get the specified config as bool, returns if config does not exist
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Config as bool, false on error/null</returns>
        protected bool TryGetConfigAsBool(string key)
        {
            config.TryGetValue($"EC_{GetType().Name}_{key}", out string configResult);
            bool.TryParse(configResult, out bool result);
            return result;
        }

        /// <summary>
        /// Tries to get the specified config as bool, returns default if config does not exist
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Config as int, 0 on error/null</returns>
        protected int TryGetConfigAsInt(string key)
        {
            config.TryGetValue($"EC_{GetType().Name}_{key}", out string configResult);
            int.TryParse(configResult, out int result);
            return result;
        }

    }
}
