using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Flings the player a little bit to the left or right and upwards
    /// </summary>
    internal class Hiccup : CEEvent
    {
        public Hiccup()
        {
            _name = "Hiccup";
            _description = "Try to not think about it, maybe it will go away";
            _repeatEverySec = 5;
            _activeTime = 60;
            _allowMultiple = true;
        }

        public override void RecurringTrigger()
        {
            foreach (AbstractCreature player in EventHelpers.AllPlayers)
            {
                player.realizedCreature.mainBodyChunk.vel.y += UnityEngine.Random.Range(5f, 15f);
                player.realizedCreature.mainBodyChunk.vel.x += UnityEngine.Random.Range(-10f, 10f);
            }
        }
    }
}
