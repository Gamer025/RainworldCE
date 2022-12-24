using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Makes the player spit out the current item
    /// </summary>
    internal class SpitOutItem : CEEvent
    {
        public SpitOutItem()
        {
            _name = "Stomach ache";
            _description = "Maybe eating random things is bad even for Slugcats";
        }

        public override void StartupTrigger()
        {
            foreach (AbstractCreature player in EventHelpers.AllPlayers)
            {
                (player.realizedCreature as Player).Regurgitate();
            }
        }
    }
}
