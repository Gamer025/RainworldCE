using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Spawns a red lizard in a random room of the region
    /// </summary>
    internal class SpawnRedLizard : CEEvent
    {
        public SpawnRedLizard()
        {
            _name = "Danger lurks around";
            _description = "Something dangerous has awakened in this region";
        }

        public override void StartupTrigger()
        {
            AbstractRoom aRoom = helper.RandomRegionRoom;
            AbstractCreature creature = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.RedLizard), null, new WorldCoordinate(aRoom.index, -1, -1, 0), game.GetNewID());
            creature.ChangeRooms(new WorldCoordinate(aRoom.index, -1, -1, 0));
            WriteLog(LogLevel.Debug, $"Spawned {creature} in {aRoom.name}");
        }
    }
}
