using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

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
            if (EventHelpers.StoryModeActive)
            {
                target = EventHelpers.RandomRegionRoom();
                _description = $"Slugcat ~ ~ ~ {target.name}";
            }
        }

        readonly AbstractRoom target;

        public override void StartupTrigger()
        {
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
