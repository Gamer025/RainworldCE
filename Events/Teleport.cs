using BepInEx.Logging;
using System.Collections.Generic;
using System.Linq;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Teleports player to first connection of any safe room in the region
    /// </summary>
    internal class Teleport : CEEvent
    {
        public Teleport()
        {
            _name = "/tp";
            _description = "Slugcat ~ ~ ~";
        }

        AbstractRoom target;

        public override void StartupTrigger()
        {
            List<AbstractRoom> rooms = EventHelpers.GetAllConnectedRooms(EventHelpers.MainPlayer.Room);
            WriteLog(LogLevel.Debug, $"{string.Join(",", rooms.Select(x => x.name))}", true);
            target = rooms[rnd.Next(rooms.Count)];
            _description = $"Slugcat ~ ~ ~ {target.name}";

            if (EventHelpers.CurrentRoom.realizedRoom.shelterDoor != null && EventHelpers.CurrentRoom.realizedRoom.shelterDoor.IsClosing)
            {
                WriteLog(LogLevel.Debug, $"Cycle about to end, abort teleport");
                return;
            }
            if (target.realizedRoom == null)
            {
                target.RealizeRoom(game.world, game);
            }
            foreach (AbstractCreature player in game.Players)
            {
                WriteLog(LogLevel.Debug, $"Teleporting {player} to {target.name}");
                game.shortcuts.CreatureTeleportOutOfRoom(player.realizedCreature, player.pos, new WorldCoordinate(target.index, -1, -1, 0));
            }
        }
    }
}
