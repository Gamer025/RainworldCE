using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Spawns a spear on top of the player every second for 30 seconds
    /// </summary>
    internal class SpearMaster :CEEvent
    {
        public SpearMaster()
        {
            _name = "Spear Master";
            _description = "I heard you need a spear or two?";
            _activeTime = 30;
            _repeatEverySec = 1;
        }

        public override void RecurringTrigger()
        {
            AbstractPhysicalObject reward = new AbstractSpear(game.world, null, EventHelpers.MainPlayer.pos, game.GetNewID(), false);
            reward.Realize();
            reward.realizedObject.PlaceInRoom(EventHelpers.CurrentRoom.realizedRoom);
        }
    }
}
