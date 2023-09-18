using Menu.Remix.MixedUI;
using RainWorldCE.Events;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RainWorldCE.Config
{
    internal class IntegerConfigEntry : EventConfigEntry
    {

        private RWCustom.IntVector2 range;
        public IntegerConfigEntry(string name, string description, string key, RWCustom.IntVector2 range, int defaultValue, CEEvent ceevent) : base(name, description, key, ceevent)
        {
            this.range = range;
            DefaultValue = defaultValue.ToString();
            defaultInt = defaultValue;
        }

        public override int size { get { return 30; } }

        private int defaultInt;
        public override Collection<UIelement> CMelement(Vector2 pos)
        {
            Collection<UIelement> elements = new Collection<UIelement>();
            elements.Add(new OpLabel(pos.x, pos.y, Name) { description = Description });
            elements.Add(new OpSlider(CEEvent.configInt[Key], new Vector2(pos.x + 7f * Name.Length, pos.y - 5f), range.y - range.x) { description = Description });
            return elements;
        }

        public override void BindConfigs(OptionInterface oi)
        {
            if (!CEEvent.configInt.ContainsKey(Key))
            {
                Configurable<int> sliderConfig;
                sliderConfig = oi.config.Bind(Key, defaultInt, new ConfigurableInfo(Description));
                CEEvent.configInt.Add(Key, sliderConfig);
            }
        }
    }
}
