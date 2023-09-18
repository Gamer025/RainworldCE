using BepInEx.Logging;
using System.Collections.Generic;

namespace RainWorldCE.Events
{
    internal class NoodleInvasion : CEEvent
    {
        public NoodleInvasion()
        {
            _name = "Noodle invasion";
            _description = "Free food, but for whom?";
        }

        public override void StartupTrigger()
        {
            float chance = 100;
            float interpolation = 0.1f;
            List<AbstractRoom> adjancedRooms = EventHelpers.GetConnectedRooms(EventHelpers.CurrentRoom);
            foreach (AbstractRoom room in adjancedRooms)
            {
                chance = UnityEngine.Mathf.Lerp(chance, 0f, interpolation);
                interpolation += 0.25f;
                WriteLog(LogLevel.Debug, $"Chance now {chance}");
                if (rnd.Next(100) < chance)
                {
                    int destRoomNodeId = EventHelpers.GetNodeIdOfRoomConnection(room, EventHelpers.CurrentRoom);
                    WriteLog(LogLevel.Debug, $"Spawned noodles in {room.name}");
                    AbstractCreature creature1 = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.SmallNeedleWorm), null, new WorldCoordinate(room.index, -1, -1, 0), game.GetNewID());
                    AbstractCreature creature2 = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.SmallNeedleWorm), null, new WorldCoordinate(room.index, -1, -1, 0), game.GetNewID());
                    AbstractCreature creature3 = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.BigNeedleWorm), null, new WorldCoordinate(room.index, -1, -1, 0), game.GetNewID());
                    EventHelpers.MakeCreatureAttackCreature(creature3, EventHelpers.MainPlayer);
                    creature1.ChangeRooms(new WorldCoordinate(EventHelpers.CurrentRoom.index, -1, -1, destRoomNodeId));
                    creature2.ChangeRooms(new WorldCoordinate(EventHelpers.CurrentRoom.index, -1, -1, destRoomNodeId));
                    creature3.ChangeRooms(new WorldCoordinate(EventHelpers.CurrentRoom.index, -1, -1, destRoomNodeId));
                }
            }
        }
    }
}
