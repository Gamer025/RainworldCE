using BepInEx.Logging;
using System.Collections.Generic;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Spawns a pack of yellow lizards close to the player
    /// </summary>
    internal class LizardPack : CEEvent
    {
        public LizardPack()
        {
            _name = "The pack";
            _description = "Some believe their behaviour was learned from scavengers";
        }

        public override void StartupTrigger()
        {
            float chance = 100;
            float interpolation = 0.1f;
            List<AbstractRoom> adjacentRooms = EventHelpers.GetConnectedRooms(EventHelpers.CurrentRoom);
            AbstractRoom aRoom = adjacentRooms[rnd.Next(adjacentRooms.Count)];
            adjacentRooms = EventHelpers.GetConnectedRooms(aRoom);
            if (adjacentRooms.Count > 1)
            {
                adjacentRooms.Remove(EventHelpers.CurrentRoom);
            }
            aRoom = adjacentRooms[rnd.Next(adjacentRooms.Count)];
            while (rnd.Next(100) < chance)
            {

                AbstractCreature creature = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.YellowLizard), null, new WorldCoordinate(aRoom.index, -1, -1, 0), game.GetNewID());
                creature.ChangeRooms(new WorldCoordinate(aRoom.index, -1, -1, 0));
                WriteLog(LogLevel.Debug, $"Spawned {creature} in {aRoom.name}");
                EventHelpers.MakeCreatureAttackCreature(creature, EventHelpers.MainPlayer);

                chance = UnityEngine.Mathf.Lerp(chance, 0f, interpolation);
                interpolation += 0.1f;
                WriteLog(LogLevel.Debug, $"Chance now {chance}");
            }

        }
    }
}

