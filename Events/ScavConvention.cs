using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using static SlugcatStats;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Spawns a random amount of scavengers in the region
    /// </summary>
    internal class ScavConvention : CEEvent
    {
        public ScavConvention()
        {
            _name = "Annual scavenger convention";
            _description = "Wasn't that already last week?";
        }

        public override void StartupTrigger()
        {
            float chance = 100;
            float interpolation = 0;
            while (rnd.Next(100) < chance)
            {

                AbstractRoom aRoom = EventHelpers.RandomRegionRoom();
                for (int count = rnd.Next(2, 5); count > 0; count--)
                {
                    //Spawn the scavs in the worlds offscreenDen so that they get equipped with gear, then move them to the choosen room
                    AbstractCreature creature = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Scavenger), null, new WorldCoordinate(game.world.offScreenDen.index, -1, -1, 0), game.GetNewID());
                    creature.ChangeRooms(new WorldCoordinate(aRoom.index, -1, -1, 0));
                    WriteLog(LogLevel.Debug, $"Spawned {creature} in {aRoom.name}");
                }
                chance = UnityEngine.Mathf.Lerp(chance, 0f, interpolation);
                interpolation += 0.1f;
                WriteLog(LogLevel.Debug, $"Chance now {chance}");
            }
            
        }
    }
}
