using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static SlugcatStats;
using UnityEngine;
using BepInEx.Logging;

namespace RainWorldCE.Events
{

    //Catapults all players into the air
    internal class LiftOff : CEEvent
    {
        public LiftOff()
        {
            _name = "Lift off";
            _description = "in T-10 seconds";
            _activeTime = 30;
            _repeatEverySec = 10;
            _allowMultiple = true;
        }

        public override void RecurringTrigger()
        {
            foreach (AbstractCreature player in helper.AllPlayers)
            {
                player.realizedCreature.mainBodyChunk.vel.y += 100f;
            }
        }
    }
}
