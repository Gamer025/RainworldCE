using Menu.Remix.MixedUI;
using RainWorldCE.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RainWorldCE.Config
{
    internal class BooleanConfigEntry : EventConfigEntry
    {
        public BooleanConfigEntry(string name, string description, string key, bool defaultValue, CEEvent ceevent) : base(name, description, key, ceevent)
        {
            DefaultValue = defaultValue.ToString();
            defaultBool = defaultValue;
        }

        public override int size { get { return 30; } }
        private readonly bool defaultBool;

        public override Collection<UIelement> CMelement(Vector2 pos)
        {
            Collection<UIelement> elements = new Collection<UIelement>();
            elements.Add(new OpCheckBox(CEEvent.configBool[Key], pos) { description = Description });
            elements.Add(new OpLabel(pos.x + 35f, pos.y, Name) { description = Description });
            return elements;
        }

        public override void BindConfigs(OptionInterface oi)
        {
            if (!CEEvent.configBool.ContainsKey(Key))
            {
                Configurable<bool> checkboxConfig;
                checkboxConfig = oi.config.Bind(Key, defaultBool, new ConfigurableInfo(Description));
                CEEvent.configBool.Add(Key, checkboxConfig);
            }
        }
    }
}
