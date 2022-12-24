using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Causes all creatures in the region to migrate to the players current room
    /// </summary>
    internal class CreatureMigration : CEEvent
    {
        public CreatureMigration()
        {
            _name = "Coming through";
            _description = "Who pressed the E key";
        }

        public override void StartupTrigger()
        {
            foreach (AbstractRoom room in EventHelpers.AllRegionRooms)
            {
                foreach (AbstractCreature creature in room.creatures)
                {
                    if (creature.abstractAI != null)
                    {
                        creature.abstractAI.SetDestination(EventHelpers.MainPlayer.pos);
                    }
                }
            }
                
        }
    }
}
