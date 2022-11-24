using OptionalUI;
using RainWorldCE.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RainWorldCE.Config
{
    enum ConfigType
    {
        boolean
    }

    public abstract class EventConfigEntry
    {
        protected EventConfigEntry(string name, string description, string key, CEEvent ceevent)
        {
            this.Name = name;
            this.Description = description;
            this.Key = $"EC_{ceevent.GetType().Name}_{key}";
        }

        /// <summary>
        /// Display name of the config (lablel)
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Description of the config
        /// </summary>
        public string Description { get; }
        /// <summary>
        /// Event unique identifier to retrieve config
        /// </summary>
        public string Key { get; }
        /// <summary>
        /// How much space (height) this element will take in the UI
        /// </summary>
        public abstract int size { get; }


        public abstract Collection<UIelement> CMelement(Vector2 pos);
    }
}
