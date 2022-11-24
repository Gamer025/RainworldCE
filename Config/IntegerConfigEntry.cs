using IL.RWCustom;
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
    internal class IntegerConfigEntry : EventConfigEntry
    {

        private RWCustom.IntVector2 range;
        public IntegerConfigEntry(string name, string description, string key, RWCustom.IntVector2 range, CEEvent ceevent) : base(name, description, key, ceevent)
        {
            this.range = range;
        }

        public override int size { get { return 30; } }

        public override Collection<UIelement> CMelement(Vector2 pos)
        {
            Collection<UIelement> elements = new Collection<UIelement>();
            elements.Add(new OpLabel(pos.x, pos.y, Name) { description = Description });
            elements.Add(new OpSlider(new Vector2(pos.x + 7f * Name.Length, pos.y - 5f), Key, range) { description = Description });
            return elements;
        }
    }
}
